using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Riptide;
using Riptide.Utils;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor;
using UnityEngine;

public partial class Board : MonoBehaviour
{
    public UIButton mulliganButton;
    public GameObject waitingEnemyMulliganMessage;

    public Card hoveredCard = null;
    public BoardSide hoveredSide = null;
    public Creature hoveredMinion = null;
    public Hero hoveredHero = null;

    public BoardSide friendlySide;
    public BoardSide enemySide;

    public GameObject cardObject;
    public GameObject minionObject;

    public Riptide.Client client = new Riptide.Client();

    public ulong playerID = 100;
    public ulong currentMatchID;

    public Hand currHand = new Hand();
    public Hand enemyHand = new Hand();
    public bool currTurn = false;
    public MinionBoard currMinions;
    public MinionBoard enemyMinions;
    public int currMana = 0;
    public int maxMana = 0;

    public int enemyMana = 0;
    public int enemyMaxMana = 0;

    void Start()
    {
        Application.targetFrameRate = 60;
        client.Connect("127.0.0.1:8888",5,0,null,false);
        client.MessageReceived += OnMessageReceived;
        currHand.board = this;
        currHand.server = false;
    }
    public void OnMessageReceived(object sender, MessageReceivedEventArgs eventArgs)
    {

        int messageID = eventArgs.MessageId;
        Message message = eventArgs.Message;

        switch ((Server.MessageType)messageID)
        {
            case Server.MessageType.ConfirmMatch:
                ulong matchID = message.GetULong();
                InitGame(matchID);
                break;
            case Server.MessageType.DrawHand:
                string handJson = message.GetString();
                int enemyCardCount = message.GetInt();
                InitHand(handJson, enemyCardCount);
                break;
            case Server.MessageType.ConfirmMulligan:
                string mulliganJson = message.GetString();
                ConfirmMulligan(mulliganJson);
                break;
            case Server.MessageType.EnemyMulligan:
                int[] enemyMulligan = message.GetInts();
                ConfirmEnemyMulligan(enemyMulligan);
                break;
            case Server.MessageType.DrawCards:
                Card.Cardname draw = (Card.Cardname)message.GetInt();
                DrawCard(draw); 
                break;
            case Server.MessageType.DrawEnemy:
                int enemyDraws = message.GetInt();
                DrawEnemy(enemyDraws);
                break;
            case Server.MessageType.StartGame:
                bool isTurn = message.GetBool();
                StartGame(isTurn);
                break;
            case Server.MessageType.StartTurn:
                StartTurn(message);
                break;
            case Server.MessageType.EndTurn:
                EndTurn(message);
                break;
            case Server.MessageType.PlayCard:
                bool playedFriendlySide = message.GetBool();
                int playedIndex = message.GetInt();
                int playedManaCost = message.GetInt();
                Card.Cardname playedCard = (Card.Cardname)message.GetInt();
                ConfirmPlayCard(playedFriendlySide, playedIndex, playedManaCost,playedCard);
                break;
            case Server.MessageType.SummonMinion:
                bool summonedFriendlySide = message.GetBool();
                Card.Cardname summonedMinion = (Card.Cardname)message.GetInt();
                int summonedPos = message.GetInt();
                SummonMinion(summonedFriendlySide,summonedMinion,summonedPos);
                break;
            case Server.MessageType.UpdateMinion:
                string minionUpdateJson = message.GetString();
                bool minionUpdateFriendly = message.GetBool();
                UpdateMinion(minionUpdateJson, minionUpdateFriendly);
                break;
            case Server.MessageType.ConfirmAttackMinion:
                ConfirmAttackMinion(message);
                break;
        }
    }
    public void InitGame(ulong matchID)
    {
        Debug.Log("Player " + playerID + " entered game " + matchID);
        currentMatchID = matchID;
    }
    public void InitHand(string jsonText, int enemyCards=4)
    {
        //Debug.Log(jsonText);
        Hand hand = JsonUtility.FromJson<Hand>(jsonText);
        foreach (var c in hand)
        {
            currHand.Add(c.card);
        }
        //currHand = hand;
        string s = "";

        foreach (HandCard c in hand)
        {
            s += c.ToString()+" ";
        }

        enemyHand.enemyHand = true;
        enemyHand.board = this;
        enemyHand.mulliganMode = Hand.MulliganState.Done;
        enemyHand.server = false;
        for (int i=0;i<enemyCards;i++)
            enemyHand.Add(Card.Cardname.Cardback);

        Debug.Log(playerID+" Hand: " + s);
        hand.mulliganMode = Hand.MulliganState.None;
        mulliganButton.transform.localScale = Vector3.one;
    }

    public void StartMatchmaking()
    {
        Message message = Message.Create(MessageSendMode.Reliable, (ushort)Server.MessageType.Matchmaking);
        message.AddULong(playerID);
        string deck = "";
        message.AddString(deck);
        client.Send(message);
    }
    public List<int> selectedMulligans = new List<int>(){};
    public void SubmitMulligan()
    {
        Message message = Message.Create(MessageSendMode.Reliable, (ushort)Server.MessageType.SubmitMulligan);
        message.AddInts(selectedMulligans.ToArray(),true);
        message.AddULong(currentMatchID);
        message.AddULong(playerID);
        client.Send(message);
    }

    void ConfirmMulligan(string jsonText)
    {
        Hand newHand = JsonUtility.FromJson<Hand>(jsonText);
        foreach (int i in selectedMulligans)
        {
            //TODO: mull anim goes here
            currHand.MulliganReplace(i, newHand[i].card);
        }
        currHand.EndMulligan();
        waitingEnemyMulliganMessage.transform.localScale = Vector3.one;
        mulliganButton.transform.localPosition += new Vector3(0, -10);
    }
    void ConfirmEnemyMulligan(int[] inds)
    {
        foreach(int i in inds)
        {
            //TODO: enemy mull anim
            //enemyHand.cardObjects[enemyHand[i]].mulliganMark.enabled = true;
        }
    }
    void StartGame(bool isTurn)
    {
        //TODO: Get rid of mulligan screen
        waitingEnemyMulliganMessage.transform.localScale = Vector3.zero;
        currHand.ConfirmBothMulligans();
        currHand.OrderInds();
        currMinions = new MinionBoard();
        enemyMinions = new MinionBoard();
        enemyMinions.board = currMinions.board = this;
        currMinions.server = enemyMinions.server = false;
        /*if (isTurn)
        {
            currTurn = true;
            Debug.Log(playerID + "'s turn.");
        }*/
    }
    public void SubmitEndTurn()
    {
        if (!currTurn) return;

        Message message = Message.Create(MessageSendMode.Reliable, (ushort)Server.MessageType.EndTurn);
        message.AddULong(currentMatchID);
        message.AddULong(playerID);
        client.Send(message);
    }

    public void EndTurn(Message message)
    {
        if (!currTurn) return;
        currTurn = false;
    }
    public void StartTurn(Message message)
    {
        if (currTurn) return;

        currTurn = true;

        int max = message.GetInt();
        int cur = message.GetInt();
        maxMana = max;
        currMana = cur;

        foreach (Minion m in currMinions)
        {
            m.canAttack = true;
        }

        Debug.Log(playerID + "'s turn. - Mana: " +currMana+"/"+maxMana );
    }
    public void DrawCard(Card.Cardname card)
    {
        //todo: anim draw
        Debug.Log("Drawn " + card);
        currHand.Add(card);
    }
    public void DrawEnemy(int x)
    {
        for (int i = 0; i < x; i++)
            enemyHand.Add(Card.Cardname.Cardback);
    }

    public void PlayCard(HandCard card,int target=-1,int position=-1)
    {
        if (!currTurn) return;
        if (card.played) return;
        Debug.Log("Playing card " + card.card);
        //send message to server to play card index
        Message message = Message.Create(MessageSendMode.Reliable, (ushort)Server.MessageType.PlayCard);
        message.AddULong(currentMatchID);
        message.AddULong(playerID);
        message.AddInt(card.index);
        message.AddInt(target);
        message.AddInt(position);

        client.Send(message);
    }
    public void ConfirmPlayCard(bool side, int index, int mana, Card.Cardname card)
    {
        if (side ==false)
        {
            enemyHand.RemoveAt(index);
            Debug.Log("TODO: Enemy played " + card);
            return;
        }
        //ally played card
        currHand.RemoveAt(index);
        int manaCost = mana;
        currMana -= manaCost;
    }

    public void SummonMinion(bool friendlySide, Card.Cardname card, int position)
    {
        MinionBoard board = friendlySide ? currMinions : enemyMinions;
        board.Add(card, position);


        string s = "";
        foreach (Minion m in board) s += m.ToString()+" ";
        Debug.Log((friendlySide ? "Ally" : "Enemy") + " board: " + s);
    }
    public void DestroyMinion()
    {

    }

    public void AttackMinion(Minion attacker, Minion target)
    {

        int attackerInd = attacker.index;
        int targetInd = target.index;
        bool enemyTaunting = false;



        if (CheckTargetEligibility(target) == false)
        {
            //invalid target todo:check these on server
            Debug.Log("Invalid target");
            return;
        }

        foreach (Minion m in enemyMinions)
        {
            if (m.TAUNT) enemyTaunting = true;
        }
        if (enemyTaunting && target.TAUNT==false)
        {
            //can't attack non taunter
            Debug.Log("Taunt in the way");
            return;
        }

        EndTargeting();

        Message message = Message.Create(MessageSendMode.Reliable, (int)Server.MessageType.AttackMinion);
        message.AddULong(currentMatchID);
        message.AddULong(playerID);
        message.AddInt(attackerInd);
        message.AddInt(targetInd);
        client.Send(message);
    }

    public void UpdateMinion(string minionJson,bool friendly)
    {
        Minion updatedMinion = JsonUtility.FromJson<Minion>(minionJson);
        Minion minion = friendly ? currMinions[updatedMinion.index] : enemyMinions[updatedMinion.index];

        minion.health = updatedMinion.health;
        minion.damage = updatedMinion.damage;

        //auras (buffs/debuffs) update
    }

    public bool IsFriendly(Minion m)
    {
        if (currMinions.Contains(m)) return true;
        return false;
    }    

    public bool IsFriendly(Hero h)
    {
        
        return false;
    }
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Q) && playerID==100)
        {
            StartMatchmaking();
        }
        if (Input.GetKeyDown(KeyCode.A) && playerID==100)
        {
            SubmitMulligan();
        }
        if (Input.GetKeyDown(KeyCode.Z) && playerID==100)
        {
            SubmitEndTurn();
        }
        if (Input.GetKeyDown(KeyCode.R) && playerID==100)
        {
            PlayCard(currHand[0],-1,0);
        }

        if (Input.GetKeyDown(KeyCode.W) && playerID==101)
        {
            StartMatchmaking();
        }
        if (Input.GetKeyDown(KeyCode.S) && playerID==101)
        {
            SubmitMulligan();
        }
        if (Input.GetKeyDown(KeyCode.X) && playerID==101)
        {
            SubmitEndTurn();
        }
        if (Input.GetKeyDown(KeyCode.R) && playerID == 101)
        {
            PlayCard(currHand[0], -1, 0);
        }


        if (Input.GetKeyDown(KeyCode.J) && playerID==101)
        {
        }
        
        
    }
    private void FixedUpdate()
    {
        client.Update();
    }

}
