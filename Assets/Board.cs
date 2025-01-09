using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Riptide;
using Riptide.Utils;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public partial class Board : MonoBehaviour
{
    public Riptide.Client client = new Riptide.Client();

    public ulong playerID = 100;
    public ulong currentMatchID;

    public Hand currHand;
    public bool currTurn = false;
    public MinionBoard currMinions;
    public MinionBoard enemyMinions;
    public int currMana = 1;
    public int maxMana = 1;

    void Start()
    {
        client.Connect("127.0.0.1:7777",5,0,null,false);
        client.MessageReceived += OnMessageReceived;
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
                InitHand(handJson);
                break;
            case Server.MessageType.ConfirmMulligan:
                string mulliganJson = message.GetString();
                ConfirmMulligan(mulliganJson);
                break;
            case Server.MessageType.DrawCards:
                Card.Cardname draw = (Card.Cardname)message.GetInt();
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
        }
    }
    public void InitGame(ulong matchID)
    {
        Debug.Log("Player " + playerID + " entered game " + matchID);
        currentMatchID = matchID;
    }
    public void InitHand(string jsonText)
    {
        //Debug.Log(jsonText);
        Hand hand = JsonUtility.FromJson<Hand>(jsonText);
        currHand = hand;
        string s = "";

        foreach (HandCard c in hand)
        {
            s += c.ToString()+" ";
        }
        Debug.Log(playerID+" Hand: " + s);
    }

    void StartMatchmaking()
    {
        Message message = Message.Create(MessageSendMode.Reliable, (ushort)Server.MessageType.Matchmaking);
        message.AddULong(playerID);
        string deck = "";
        message.AddString(deck);
        client.Send(message);
    }
    int[] selectedMulligans = { 0, 2 };
    void SubmitMulligan()
    {
        Message message = Message.Create(MessageSendMode.Reliable, (ushort)Server.MessageType.SubmitMulligan);
        message.AddInts(selectedMulligans,true);
        message.AddULong(currentMatchID);
        message.AddULong(playerID);
        client.Send(message);
    }

    void ConfirmMulligan(string jsonText)
    {
        Hand hand = JsonUtility.FromJson<Hand>(jsonText);
        foreach (int i in selectedMulligans)
        {
            //TODO: mull anim goes here
            currHand[i].Set(hand[i].card, hand[i].index);
        }
    }
    void StartGame(bool isTurn)
    {
        //TODO: Get rid of mulligan screen
        currMinions = new MinionBoard();
        enemyMinions = new MinionBoard();
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

        Debug.Log(playerID + "'s turn. - Mana: " +currMana+"/"+maxMana );
    }
    public void DrawCard(Card.Cardname card)
    {
        //todo: anim draw
        Debug.Log("Drawn " + card);
        currHand.Add(card);
    }

    public void PlayCard(HandCard card)//Character target
    {
        if (card.played) return;
    }

    public void SummonMinion()
    {
        
    }
    public void DestroyMinion()
    {

    }
    public void ConfirmPlayCard(int index)
    {
        currHand.RemoveAt(index);
        int manaCost = 9;
        currMana -= manaCost;
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
        
        
    }
    private void FixedUpdate()
    {
        client.Update();
    }

}
