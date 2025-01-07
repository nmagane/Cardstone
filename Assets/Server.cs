using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using JetBrains.Annotations;
using Riptide;
using Riptide.Transports;
using Riptide.Utils;
using Unity.Networking.Transport;
using UnityEditor.PackageManager;
using UnityEngine;

public class Server : MonoBehaviour
{
    public enum MessageType
    {
        Matchmaking,
        ConfirmMatch,
        PlayCard,
        EndTurn,

        Concede,
    }
    public Riptide.Server server = new Riptide.Server();

    void Start()
    {
        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);
        ushort port = 7777;
        ushort maxPlayers = 65534; //ushort.max-1
        server.Start(port, maxPlayers,0,false);
        server.MessageReceived += OnMessageReceived;
    }


    private void SendInt(int x, ushort clientID, ushort messageID = 1)
    {
        Message message = Message.Create(MessageSendMode.Reliable, messageID);
        message.AddInt(x);
        server.Send(message,clientID);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        server.Update();
    }

    public void OnMessageReceived(object sender, MessageReceivedEventArgs eventArgs)
    {

        int messageID = eventArgs.MessageId;
        switch ((MessageType)messageID)
        {
            case MessageType.Matchmaking:
                ulong queuePlayerID = eventArgs.Message.GetULong();
                ushort queueClientID = eventArgs.FromConnection.Id;
                AddToQueue(queueClientID, queuePlayerID);
                break;
            case MessageType.PlayCard:
                break;
            case MessageType.EndTurn:
                break;
        }
        
    }
    

    public struct PlayerConnection
    {
        public ushort clientID;
        public ulong playerID;
        public string deck;
    }

    List<PlayerConnection> playerQueue = new List<PlayerConnection>();
    public List<Match> currentMatches = new List<Match>();

    void AddToQueue(ushort clientID, ulong playerID, string deck="")
    {
        foreach (PlayerConnection p in playerQueue)
        {
            if (p.playerID == playerID || p.clientID == clientID)
                return;
        }
        PlayerConnection pc;
        pc.clientID = clientID;
        pc.playerID = playerID;
        pc.deck = deck;
        playerQueue.Add(pc);

        //MatchmakingLogic();
        
        //matchmaking placeholder:
        if (playerQueue.Count>=2)
        {
            StartMatch(playerQueue[0], playerQueue[1]);
        }
    }
    public ulong currMatchID = 1000;
    public void StartMatch(PlayerConnection p1, PlayerConnection p2)
    {
        Match match = new Match();

        Message m1 = Message.Create(MessageSendMode.Reliable, (ushort)Server.MessageType.ConfirmMatch);
        m1.AddULong(currMatchID);
        server.Send(m1,p1.clientID);

        Message m2 = Message.Create(MessageSendMode.Reliable, (ushort)Server.MessageType.ConfirmMatch);
        m2.AddULong(currMatchID);
        server.Send(m2,p2.clientID);

        match.InitMatch(p1, p2, currMatchID);
        currentMatches.Add(match);
        currMatchID += 1;
    }
    public enum Turn
    {
        player1,
        player2,
    }
    public class Match
    {
        PlayerConnection player1, player2;
        public ulong matchID;
        public int player1_HP = 20;
        public int player2_HP = 20;

        public List<Card.Cardname> player1_deck = new List<Card.Cardname>();
        public List<Card.Cardname> player2_deck = new List<Card.Cardname>();


        //todo: hands
        //todo: on board minions
        //todo: secrets
        //todo: decks, graveyards
        
        public void InitMatch(PlayerConnection p1, PlayerConnection p2, ulong mID)
        {
            player1 = p1;
            player2 = p2;
            List<Card.Cardname> sampleTestDeck = new List<Card.Cardname>();

            for (int i=0;i<15;i++)
            {
                sampleTestDeck.Add((Card.Cardname)i);
                sampleTestDeck.Add((Card.Cardname)i);
            }
            
            player1_deck = new List<Card.Cardname>(Board.Shuffle(sampleTestDeck));
            player2_deck = new List<Card.Cardname>(Board.Shuffle(sampleTestDeck));
            
            matchID = mID;
        }
        public static bool operator ==(Match x, ulong y)
        {
            return (x.matchID == y);
        }
        public static bool operator !=(Match x, ulong y)
        {
            return (x.matchID != y);
        }
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
    
}
