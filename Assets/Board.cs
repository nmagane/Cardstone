using System.Collections.Generic;
using Riptide;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using System.Collections;

public partial class Board : MonoBehaviour
{
    public AnimationManager animationManager;
    public GameObject gameAnchor;   
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
    public GameObject UISprite;
    public GameObject splashObject;

    public Client client = new Client();

    public ulong playerID = 100;
    public ulong currentMatchID;

    public Hand currHand = new Hand();
    public Hand enemyHand = new Hand();
    public bool currTurn = false;
    public MinionBoard currMinions;
    public MinionBoard enemyMinions;
    public Hero currHero;
    public Hero enemyHero;

    public ManaBar mana;
    public ManaBar enemyMana;

    public HeroPower heroPower;
    public HeroPower enemyHeroPower;

    public Deck deck;
    public Deck enemyDeck;
    public int currMana => mana.curr;

    /*
    public int currMana = 0;
    public int maxMana = 0;
    */

    public Card CreateCard()
    {
        Card c = Instantiate(cardObject).GetComponent<Card>();
        return c;
    }
    public Splash CreateSplash(MonoBehaviour obj,int value)
    {
        Splash splash = Instantiate(splashObject).GetComponent<Splash>();
        Splash.Type type = value < 0 ? Splash.Type.Damage : Splash.Type.Heal;
        splash.Set(type, value, obj);
        return splash;
    }
    public Creature CreateCreature()
    {
        Creature c = Instantiate(minionObject).GetComponent<Creature>();
        return c;
    }
    public void DestroyObject(MonoBehaviour o)
    {
        Destroy(o.gameObject);
    }
    public NetworkHandler mirror;
    void Start()
    {
        Application.targetFrameRate = 60;
#if (UNITY_EDITOR == false)
        playerID = (ulong)Random.Range(-1000000, 1000000);
#endif
        mirror.StartClient();
        //client.Connect("127.0.0.1:8888",5,0,null,false);
        //client.MessageReceived += OnMessageReceived;
        currHand.board = this;
        currHand.server = false;
    }
    Queue<Server.CustomMessage> confirmedMessages = new Queue<Server.CustomMessage>();
    public ushort matchMessageOrder = 0;
    public ushort messageReceivedOrder = 0;
    List<(Server.MessageType, Server.CustomMessage, ushort)> messageQue = new();


    bool executing = false;
    public IEnumerator ExecuteMessages()
    {
        executing = true;
        while (confirmedMessages.Count>0)
        {
            Server.CustomMessage msg = confirmedMessages.Dequeue();

            //if (playerID == 100) Debug.Log(msg.type);
            Coroutine c = ParseMessage(msg);
            if (c != null) yield return c;

            //if (playerID == 100) Debug.Log("finished "+msg.type);
        }
        executing = false;
    }
    public void OnMessageReceived(Server.CustomMessage message)
    {

        Server.MessageType messageID = message.type;
        //Message originalMessage = eventArgs.Message;
        ushort count = message.order;
        //Debug.Log("Received Message " + count);
        //Server.CustomMessage newMessage = CopyMessage(originalMessage, messageID);
        ReceiveMessage(message.type, message, count);

    }
    public void ReceiveMessage(Server.MessageType type, Server.CustomMessage message, ushort order)
    {
        if (order == messageReceivedOrder)
        {
            messageReceivedOrder++;
            //ParseMessage(message);
            confirmedMessages.Enqueue(message);
            if (!executing) StartCoroutine(ExecuteMessages());
        }
        else
        {
            messageQue.Add((type,message, order));
        }

        for (int i = 0; i < messageQue.Count; i++)
        {
            var v = messageQue[i];
            if (v.Item3 == messageReceivedOrder)
            {
                ReceiveMessage(v.Item1, v.Item2, v.Item3);
                messageQue.Remove(v);
                break;
            }
        }
    }
    public Coroutine ParseMessage(Server.CustomMessage message)
    {
        Server.MessageType messageID = message.type;
        Coroutine animation = null;
        switch (messageID)
        {
            case Server.MessageType._TEST:
                break;
            case Server.MessageType.ConfirmMatch:
                ulong matchID = message.GetULong();
                InitGame(matchID);
                break;
            case Server.MessageType.DrawHand:
                List<ushort> hand = message.GetUShorts();
                int enemyCardCount = message.GetInt();
                InitHand(hand, enemyCardCount);
                break;
            case Server.MessageType.ConfirmMulligan:
                List<ushort> mulliganNewHand = message.GetUShorts();
                ConfirmMulligan(mulliganNewHand);
                animation = StartCoroutine(AnimationManager.Wait(50));
                break;
            case Server.MessageType.EnemyMulligan:
                List<int> enemyMulligan = message.GetInts();
                ConfirmEnemyMulligan(enemyMulligan);
                break;
            case Server.MessageType.DrawCards:
                int draw = message.GetInt();
                DrawCard((Card.Cardname)draw); 
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
                bool startAllyTurn = message.GetBool();

                int startMaxMana = message.GetInt();
                int startCurrMana = message.GetInt();
                ushort startMessageCount = message.GetUShort();
                StartTurn(startAllyTurn,startMaxMana,startCurrMana,startMessageCount);
                break;
            case Server.MessageType.EndTurn:
                EndTurn();
                break;
            case Server.MessageType.PlayCard:
                bool playedFriendlySide = message.GetBool();
                int playedIndex = message.GetInt();
                int playedManaCost = message.GetInt();
                int playedCard = message.GetInt();
                int playedPos = message.GetInt();
                if (playedPos != -1)
                {
                    if (playedFriendlySide && currMinions.previewMinion != null && currMinions.currPreview == playedPos)
                    {
                        animation = currMinions.previewMinion.index == playedPos? null : StartCoroutine(AnimationManager.Wait(10));
                    }
                    else animation = StartCoroutine(AnimationManager.Wait(10));
                }

                ConfirmPlayCard(playedFriendlySide, playedIndex, playedManaCost, (Card.Cardname)playedCard,playedPos);
                break;
            case Server.MessageType.SummonMinion:
                bool summonedFriendlySide = message.GetBool();
                int summonedMinion = message.GetInt();
                int summonedPos = message.GetInt();
                SummonMinion(summonedFriendlySide,(Card.Cardname)summonedMinion,summonedPos);
                break;
            case Server.MessageType.UpdateMinion:
                string minionUpdateJson = message.GetString();
                bool minionUpdateFriendly = message.GetBool();

                bool UpdateMinionDamaged = message.GetBool();
                bool UpdateMinionHealed = message.GetBool();

                UpdateMinion(minionUpdateJson, minionUpdateFriendly, UpdateMinionDamaged, UpdateMinionHealed);
                break;
            case Server.MessageType.ConfirmPreAttackMinion:
                bool preMinionAllyAttack = message.GetBool();
                int preMinionAttackerIndex = message.GetInt();
                int preMinionTargetIndex = message.GetInt();
                animation = ConfirmPreAttackMinion(preMinionAllyAttack, preMinionAttackerIndex,preMinionTargetIndex);
                break;
            case Server.MessageType.ConfirmAttackMinion:
                bool minionAllyAttack = message.GetBool();
                int minionAttackerIndex = message.GetInt();
                int minionTargetIndex = message.GetInt();
                animation = ConfirmAttackMinion(minionAllyAttack, minionAttackerIndex, minionTargetIndex);
                break;
            case Server.MessageType.ConfirmPreAttackFace:
                bool preFaceAllyAttack = message.GetBool();
                int preFaceAttackerIndex = message.GetInt();
                animation = ConfirmPreAttackFace(preFaceAllyAttack, preFaceAttackerIndex);
                break;
            case Server.MessageType.ConfirmAttackFace:
                bool faceAllyAttack = message.GetBool();
                int faceAttackerIndex = message.GetInt();
                animation = ConfirmAttackFace(faceAllyAttack, faceAttackerIndex);
                break;
            case Server.MessageType.DestroyMinion:

                int DestroyInd = message.GetInt();
                bool DestroyFriendly = message.GetBool();
                DestroyMinion(DestroyInd, DestroyFriendly);
                break;
            case Server.MessageType.UpdateHero:
                int UpdateHeroHP = message.GetInt();
                bool UpdateHeroFriendly = message.GetBool();
                int UpdateHeroDeckCount = message.GetInt();
                int UpdateHeroCurrMana = message.GetInt();
                int UpdateHeroMaxMana = message.GetInt();
                bool UpdateHeroDamaged = message.GetBool();
                bool UpdateHeroHealed = message.GetBool();
                UpdateHero(UpdateHeroHP,UpdateHeroFriendly, UpdateHeroDeckCount,UpdateHeroCurrMana,UpdateHeroMaxMana, UpdateHeroDamaged, UpdateHeroHealed);
                break;
            case Server.MessageType.AddAura:
            case Server.MessageType.RemoveAura:
                int addAuraMinionIndex = message.GetInt();
                bool addAuraFriendly = message.GetBool();
                ushort addAuraType = message.GetUShort();
                ushort addAuraValue = message.GetUShort();
                bool addAuraTemp = message.GetBool();
                bool addAuraForeign = message.GetBool();
                int addAuraSourceInd = message.GetInt();
                bool addAuraSourceFriendly = message.GetBool();
                if (messageID==Server.MessageType.RemoveAura)
                {
                    RemoveAura(addAuraMinionIndex, addAuraFriendly, addAuraType, addAuraValue, addAuraTemp, addAuraForeign, addAuraSourceInd, addAuraSourceFriendly);
                    break;
                }
                AddAura(addAuraMinionIndex, addAuraFriendly, addAuraType, addAuraValue, addAuraTemp, addAuraForeign, addAuraSourceInd, addAuraSourceFriendly);
                break;
            case Server.MessageType.AddTrigger:
            case Server.MessageType.RemoveTrigger:
                int addTriggerMinionIndex = message.GetInt();
                bool addTriggerFriendly = message.GetBool();
                ushort addTriggerType = message.GetUShort();
                ushort addTriggerSide = message.GetUShort();
                ushort addTriggerAbility = message.GetUShort();
                if (messageID == Server.MessageType.RemoveTrigger)
                {
                    RemoveTrigger(addTriggerMinionIndex, addTriggerFriendly, addTriggerType, addTriggerSide, addTriggerAbility);
                    break;
                }
                AddTrigger(addTriggerMinionIndex, addTriggerFriendly, addTriggerType, addTriggerSide, addTriggerAbility);
                break;
            case Server.MessageType.DiscardCard:
                bool discardFriendly = message.GetBool();
                int discardCardInd = message.GetInt();
                int discardCardName = message.GetInt();
                DiscardCard(discardFriendly, discardCardInd, (Card.Cardname)discardCardName);
                break;
            case Server.MessageType.MillCard:
                bool millFriendly = message.GetBool();
                int millCardName = message.GetInt();
                MillCard(millFriendly, (Card.Cardname)millCardName);
                break;
            case Server.MessageType.ConfirmHeroPower:
                bool heroPowerFriendly = message.GetBool();
                ConfirmHeroPower(heroPowerFriendly);
                break;
            case Server.MessageType.ConfirmBattlecry:
            case Server.MessageType.ConfirmTrigger:
                bool battlecryFriendly = message.GetBool();
                int battlecryIndex = message.GetInt();
                if (messageID == Server.MessageType.ConfirmTrigger)
                {
                    bool triggerDeathrattle = message.GetBool();
                    animation = ConfirmTrigger(battlecryFriendly, battlecryIndex,triggerDeathrattle);
                }
                else
                    animation = ConfirmBattlecry(battlecryFriendly, battlecryIndex);
                break;
            default:
                Debug.LogError("UNKNOWN MESSAGE TYPE");
                break;
        }
        return animation;
    }
    public void InitGame(ulong matchID)
    {
        Debug.Log("Player " + playerID + " entered game " + matchID);
        currentMatchID = matchID;
    }
    public void InitHand(List<ushort> hand, int enemyCards=4)
    {
        //Debug.Log(jsonText);
        //Hand hand = JsonUtility.FromJson<Hand>(jsonText);
        if (hand.Contains((ushort)Card.Cardname.Coin))
            currHand.coinHand = true;
        foreach (var c in hand)
        {
            currHand.Add(((Card.Cardname)c),-1,Hand.CardSource.Start);
        }
        //currHand = hand;
        string s = "";

        foreach (var c in hand)
        {
            s += ((Card.Cardname)c).ToString()+" ";
        }

        enemyHand.enemyHand = true;
        enemyHand.board = this;
        enemyHand.mulliganMode = Hand.MulliganState.Done;
        enemyHand.server = false;
        for (int i=0;i<enemyCards;i++)
            enemyHand.Add(Card.Cardname.Cardback,-1,Hand.CardSource.Start);

        //Debug.Log(playerID+" Hand: " + s);
        currHand.mulliganMode = Hand.MulliganState.None;
        mulliganButton.transform.localScale = Vector3.one;

        deck.Set(30 - currHand.Count()+(currHand.coinHand?1:0));
        enemyDeck.Set(30 - enemyCards+(currHand.coinHand?0:1));
    }

    public void StartMatchmaking()
    {
        Server.CustomMessage message = CreateMessage(Server.MessageType.Matchmaking);
        message.AddULong(playerID);
        string deck = "";
        message.AddString(deck);
        //client.Send(message);
        SendMessage(message,true);
    }
    public List<int> selectedMulligans = new List<int>(){};
    public void SubmitMulligan()
    {
        Server.CustomMessage message = CreateMessage(Server.MessageType.SubmitMulligan);
        message.AddULong(currentMatchID);
        message.AddInts(selectedMulligans);
        message.AddULong(playerID);
        //client.Send(message);
        SendMessage(message,true);
    }

    void ConfirmMulligan(List<ushort> cards)
    {
        foreach (int i in selectedMulligans)
        {
            //TODO: mull anim goes here
            currHand.MulliganReplace(i, (Card.Cardname)cards[i]);
        }
        currHand.EndMulligan();
        waitingEnemyMulliganMessage.transform.localScale = Vector3.one;
        mulliganButton.transform.localPosition += new Vector3(0, -10);
    }
    void ConfirmEnemyMulligan(List<int> inds)
    {
        foreach(int i in inds)
        {
            //TODO: enemy mull anim
            //enemyHand.cardObjects[enemyHand[i]].mulliganMark.enabled = true;
        }
    }
    void StartGame(bool isTurn)
    {
        heroPower.Set(Card.Cardname.Lifetap);

        //TODO: Get rid of mulligan screen
        waitingEnemyMulliganMessage.transform.localScale = Vector3.zero;
        currHand.ConfirmBothMulligans();
        currHand.OrderInds();
        currMinions = new MinionBoard();
        enemyMinions = new MinionBoard();
        enemyMinions.board = currMinions.board = this;
        currMinions.server = enemyMinions.server = false;

    }
    public void SubmitEndTurn()
    {
        if (!currTurn) return;

        Server.CustomMessage message = CreateMessage(Server.MessageType.EndTurn);
        message.AddULong(currentMatchID);
        message.AddULong(playerID);
        SendMessage(message);
    }

    public void EndTurn()
    {
        if (!currTurn) return;
        currTurn = false;
        CheckHighlights();
    }
    public void StartTurn(bool allyTurn, int maxMana, int currMana, ushort messageCount)
    {

        if (!allyTurn)
        {
            enemyMana.SetMax(maxMana);
            enemyMana.SetCurrent(currMana);
            enemyHeroPower.Enable();
            return;
        }
        matchMessageOrder = messageCount;
        if (currTurn) return;

        currTurn = true;

        mana.SetMax(maxMana);
        mana.SetCurrent(currMana);

        heroPower.Enable();
        foreach (Minion m in currMinions)
        {
            m.canAttack = true;
        }

        CheckHighlights();
    }
    public void DrawCard(Card.Cardname card)
    {
        currHand.Add(card);
        CheckHighlights();
    }
    public void DrawEnemy(int x)
    {
        for (int i = 0; i < x; i++)
            enemyHand.Add(Card.Cardname.Cardback);
    }

    public void PlayCard(HandCard card, int target = -1, int position = -1, bool friendlySide = false, bool isHero=false)
    {
        if (!currTurn) return;
        if (card.played) return;
        card.played = true;
        EndTargeting();
        playingCard = null;
        //Debug.Log("Playing card " + card.card);
        //send message to server to play card index
        Server.CustomMessage message = CreateMessage(Server.MessageType.PlayCard);
        message.AddULong(currentMatchID);
        message.AddULong(playerID);
        message.AddInt(card.index);
        message.AddInt(target);
        message.AddInt(position);
        message.AddBool(friendlySide);
        message.AddBool(isHero);

        //client.Send(message);
        SendMessage(message);
    }

    public void SummonMinion(bool friendlySide, Card.Cardname card, int position)
    {
        MinionBoard board = friendlySide ? currMinions : enemyMinions;
        board.Add(card, position);


        //string s = "";
        //foreach (Minion m in board) s += m.ToString()+" ";
        //Debug.Log((friendlySide ? "Ally" : "Enemy") + " board: " + s);
    }
    public void DestroyMinion(int ind, bool friendlySide)
    {
        if (friendlySide)
        {
            currMinions.RemoveAt(ind);
        }
        else
        {
            enemyMinions.RemoveAt(ind);
        }
        CheckHighlights();
    }

    public void AttackMinion(Minion attacker, Minion target)
    {

        int attackerInd = attacker.index;
        int targetInd = target.index;
        bool enemyTaunting = false;


        //todo: valid attack function
        foreach (Minion m in enemyMinions)
        {
            if (m.HasAura(Aura.Type.Taunt)) enemyTaunting = true;
        }
        if (enemyTaunting && target.HasAura(Aura.Type.Taunt)==false)
        {
            //can't attack non taunter
            Debug.Log("Taunt in the way");
            return;
        }

        EndTargeting();

        Server.CustomMessage message = CreateMessage(Server.MessageType.AttackMinion);
        message.AddULong(currentMatchID);
        message.AddULong(playerID);
        message.AddInt(attackerInd);
        message.AddInt(targetInd);
        //client.Send(message);
        SendMessage(message);
    }

    public void UpdateMinion(string minionJson,bool friendly, bool damaged, bool healed)
    {
        Minion updatedMinion = JsonUtility.FromJson<Minion>(minionJson);
        Minion minion = friendly ? currMinions[updatedMinion.index] : enemyMinions[updatedMinion.index];

        int diff = updatedMinion.health - minion.health;
        minion.health = updatedMinion.health;
        minion.damage = updatedMinion.damage;

        if (damaged || healed) CreateSplash(minion.creature, diff);
        //todo: splash damage/heal
        
    }

    public void AttackFace(Minion minion, Hero h)
    {

        int attackerInd = minion.index;
        bool enemyTaunting = false;

        if (CheckTargetEligibility(h) == false)
        {
            //invalid target todo:check these on server
            Debug.Log("Invalid target");
            return;
        }

        foreach (Minion m in enemyMinions)
        {
            if (m.HasAura(Aura.Type.Taunt)) enemyTaunting = true;
        }
        if (enemyTaunting)
        {
            //can't attack non taunter
            Debug.Log("Taunt in the way");
            return;
        }

        EndTargeting();

        Server.CustomMessage message = CreateMessage(Server.MessageType.AttackFace);
        message.AddULong(currentMatchID);
        message.AddULong(playerID);
        message.AddInt(attackerInd);
        //client.Send(message);
        SendMessage(message);
    }

    public void UpdateHero(int hp, bool friendly, int deckCount, int currMana,int maxMana, bool damaged, bool healed)
    {

        if (friendly)
        {
            int diff = hp - currHero.health;

            currHero.SetHealth(hp);
            deck.Set(deckCount);
            mana.SetCurrent(currMana);
            mana.SetMax(maxMana);

            if (damaged || healed) CreateSplash(currHero, diff);
        }
        else
        {
            int diff = hp - enemyHero.health;

            enemyHero.SetHealth(hp);
            enemyDeck.Set(deckCount);
            enemyMana.SetCurrent(currMana);
            enemyMana.SetMax(maxMana);

            if (damaged || healed) CreateSplash(enemyHero, diff);
        }

        //TODO: Damage/Heal splashes

        CheckHighlights();
    }
    public void AddAura(int minionIndex,bool friendly, ushort auraType, ushort value, bool temp, bool foreign,int sourceInd, bool sourceFriendly)
    {
        Minion target = friendly? currMinions[minionIndex] : enemyMinions[minionIndex];
        Minion source = sourceInd == -1 ? null : (sourceFriendly ? currMinions[sourceInd] : enemyMinions[sourceInd]);
        target.AddAura(new Aura((Aura.Type)auraType, value, temp, foreign, source));
        
    }
    public void RemoveAura(int minionIndex,bool friendly, ushort auraType, ushort value, bool temp, bool foreign,int sourceInd, bool sourceFriendly)
    {
        Minion target = friendly? currMinions[minionIndex] : enemyMinions[minionIndex];
        Minion source = sourceInd == -1 ? null : (sourceFriendly ? currMinions[sourceInd] : enemyMinions[sourceInd]);
        target.RemoveMatchingAura(new Aura((Aura.Type)auraType, value, temp, foreign, source));
    }
    public void RemoveTrigger(int minionIndex, bool friendly, ushort type, ushort side, ushort ability)
    {
        Minion target = friendly ? currMinions[minionIndex] : enemyMinions[minionIndex];
        target.RemoveMatchingTrigger(new Trigger((Trigger.Type)type, (Trigger.Side)side, (Trigger.Ability)ability,target));
    }
    public void AddTrigger(int minionIndex, bool friendly, ushort type, ushort side, ushort ability)
    {
        Minion target = friendly ? currMinions[minionIndex] : enemyMinions[minionIndex];
        target.AddTrigger((Trigger.Type)type, (Trigger.Side)side, (Trigger.Ability)ability);
    }

    public void DiscardCard(bool friendly, int ind, Card.Cardname card)
    {
        Hand hand = friendly ? currHand : enemyHand;
        HandCard c = hand[ind];
        Debug.Log((friendly?"":"enemy ")+"discarded " + card);
        hand.RemoveAt(ind,Hand.RemoveCardType.Discard,card);
        //TODO: discard anim
    }
    public void MillCard(bool friendly, Card.Cardname card)
    {
        Card c = CreateCard();
        c.transform.parent = deck.transform.parent;
        c.Set(new HandCard(card, 0));
        c.transform.localPosition = (friendly) ? deck.transform.localPosition : enemyDeck.transform.localPosition;
        c.SetFlipped();
        c.Flip();
        animationManager.MillAnim(c,friendly);
    }
    public void CastHeroPower(Card.Cardname ability, int target, bool isFriendly, bool isHero)
    {
        Server.CustomMessage message = CreateMessage(Server.MessageType.HeroPower);

        if (targeting) EndTargeting();
        message.AddULong(currentMatchID);
        message.AddULong(playerID);
        message.AddUShort((ushort)ability);
        message.AddInt(target);
        message.AddBool(isFriendly);
        message.AddBool(isHero);

        SendMessage(message);
    }

    public bool IsFriendly(Minion m)
    {
        if (currMinions.Contains(m)) return true;
        return false;
    }    

    public bool IsFriendly(Hero h)
    {
        return h == currHero;
    }
    void Update()
    {
#if (UNITY_EDITOR)
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
            Message m = Message.Create(MessageSendMode.Reliable, 1);

            m.AddInt(123);
            //m.AddInt(123);
            
            Debug.Log(m.GetInt());
        }
        
        if (Input.GetKey(KeyCode.BackQuote))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Camera.main.transform.localPosition = new Vector3(0,0,-10);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Camera.main.transform.localPosition = new Vector3(50,0,-10);
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            currHand.OrderInds();
        }
#endif //END TESTING HOTKEYS
    }
    void Tester(Server.CustomMessage m)
    {
     Debug.Log(m.GetUShort()+" "+m.GetInt()+" "+m.GetBool());
     //Debug.Log(" "+m.GetInt()+" "+m.GetBool());
    }
    private void FixedUpdate()
    {
        client.Update();
    }

}
