using System;
using System.Collections;
using System.Collections.Generic;
using Riptide;
using Riptide.Transports;
using Riptide.Utils;
using System.Linq;
using UnityEngine;
using UnityEditor.PackageManager;
using UnityEditor;


public partial class Server : MonoBehaviour
{
    public enum MessageType
    {
        Matchmaking,
        ConfirmMatch,

        SubmitMulligan,
        ConfirmMulligan,

        PlayCard,

        StartGame,
        StartTurn,
        EndTurn,

        DrawCards,
        DrawHand,

        SummonMinion,
        DestroyMinion,
        Attack,


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
        ushort clientID = eventArgs.FromConnection.Id;
        Message message = eventArgs.Message;
        switch ((MessageType)messageID)
        {
            case MessageType.Matchmaking:
                ulong queuePlayerID = message.GetULong();
                ushort queueClientID = clientID;
                AddToQueue(queueClientID, queuePlayerID);
                break;
            case MessageType.SubmitMulligan:
                int[] mullInds = message.GetInts();
                ulong mullMatchID = message.GetULong();
                ulong mullPlayerID = message.GetULong();
                Mulligan(mullInds, mullMatchID, mullPlayerID);
                break;
            case MessageType.PlayCard:
                break;
            case MessageType.EndTurn:
                ulong endMatchID = message.GetULong();
                ulong endPlayerID = message.GetULong();
                EndTurn(endMatchID, clientID, endPlayerID);
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
    public Dictionary<ulong, Match> currentMatches = new Dictionary<ulong, Match>();
    public List<Match> matchList = new List<Match>();
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
        currentMatches.Add(currMatchID,match);
        matchList.Add(match);
        DrawStarterHands(match);
        currMatchID += 1;
    }
    public enum Turn
    {
        player1,
        player2,
    }
    public void DrawStarterHands(Match m)
    {
        int p1cards = m.turn == Turn.player1 ? 3 : 4;
        int p2cards = m.turn == Turn.player2 ? 3 : 4;

        for (int i=0;i< p1cards; i++)
        {
            m.hands[0].Add(m.decks[0][0]);
            m.decks[0].RemoveAt(0);
        }
        for (int i=0;i< p2cards; i++)
        {
            m.hands[1].Add(m.decks[1][0]);
            m.decks[1].RemoveAt(0);
        }
        if (m.turn==Turn.player1)
        {
            //TODO: add coin p2
        }
        else
        {
            //TODO: add coin p1
        }

        Message m1 = Message.Create(MessageSendMode.Reliable, (ushort)Server.MessageType.DrawHand);
        string jsonText = JsonUtility.ToJson(m.hands[0]);
        m1.AddString(jsonText);
        server.Send(m1, m.players[0].clientID);

        Message m2 = Message.Create(MessageSendMode.Reliable, (ushort)Server.MessageType.DrawHand);
        jsonText = JsonUtility.ToJson(m.hands[1]);
        m2.AddString(jsonText);
        server.Send(m2, m.players[1].clientID);
    }

    public void Mulligan(int[] inds, ulong matchID, ulong playerID)
    {
        if (currentMatches.ContainsKey(matchID) == false) return;
        Match match = currentMatches[matchID];
        Board.Hand hand= match.hands[0];
        int player = 0;
        if (playerID == match.players[0].playerID)
        {
            player = 0;
        }
        else if (playerID == match.players[1].playerID)
        {
            player = 1;
        }
        else return;

        if (match.mulligans[player]) return; //already mulliganed this player

        hand = match.hands[player];
        List<Card.Cardname> returningCards = new List<Card.Cardname>();

        foreach (int i in inds)
        {
            if (i >= hand.Count())
                continue;
            returningCards.Add(hand[i].card);
            Card.Cardname newCard = match.decks[player][0];
            match.decks[player].RemoveAt(0);
            hand[i] = new Board.HandCard(newCard, i);
        }
        match.decks[player].AddRange(returningCards);
        match.decks[player] = Board.Shuffle(match.decks[player]);
        match.mulligans[player] = true;

        Message message = Message.Create(MessageSendMode.Reliable, (ushort)Server.MessageType.ConfirmMulligan);
        string jsonText = JsonUtility.ToJson(match.hands[player]);
        message.AddString(jsonText);
        server.Send(message, match.players[player].clientID);

        if (match.mulligans[0] && match.mulligans[1])
        {
            StartGame(match);
            Debug.Log("both mulls complete");
        }
    }

    public void StartGame(Match match)
    {

        Message messageFirst = Message.Create(MessageSendMode.Reliable, (ushort)Server.MessageType.StartGame);
        Message messageSecond = Message.Create(MessageSendMode.Reliable, (ushort)Server.MessageType.StartGame);
        messageFirst.AddBool(true);
        messageSecond.AddBool(false);
        int f = match.turn == Turn.player1 ? 0 : 1;
        int s = match.turn == Turn.player1 ? 1 : 0;
        server.Send(messageFirst, match.players[f].clientID);
        server.Send(messageSecond, match.players[s].clientID);

        StartTurn(match);
    }

    public void StartTurn(Match match)
    {
        Message message = Message.Create(MessageSendMode.Reliable, (ushort)Server.MessageType.StartTurn);
        int p = (int)match.turn;

        match.maxMana[p] = Mathf.Min(match.maxMana[p] + 1, 10);
        match.currentMana[p] = match.maxMana[p];

        //TODO: start of turn effects

        message.Add(match.maxMana[p]);
        message.Add(match.currentMana[p]);

        server.Send(message, match.players[p].clientID);

        DrawCards(match, match.turn, 1);
    }
    public void EndTurn(ulong matchID, ushort clientID, ulong playerID)
    {
        Match m = currentMatches[matchID];
        PlayerConnection player = m.players[(int)m.turn];
        if (player.clientID != clientID || player.playerID != playerID) return;

        //TODO: end of turn effects
        m.turn = m.turn == Turn.player1 ? Turn.player2 : Turn.player1;

        Message msg = Message.Create(MessageSendMode.Reliable, (ushort)Server.MessageType.EndTurn);
        server.Send(msg, player.clientID);

        StartTurn(m);
    }
    public void DrawCards(Match match, Turn player, int count = 1)
    {
        int p = (int)player;
        List<Card.Cardname> drawnCards = new List<Card.Cardname>();
        for (int i = 0; i < count; i++)
        {
            Card.Cardname top = match.decks[p][0];
            match.decks[p].RemoveAt(0);
            match.hands[p].Add(top);
            drawnCards.Add(top);

            Message message = Message.Create(MessageSendMode.Reliable, (ushort)Server.MessageType.DrawCards);
            message.AddInt((int)top);
            server.Send(message, match.players[p].clientID);
            
        }

    }

    public void PlayCard(ulong matchID, ushort clientID, ulong playerID, int index) //todo: target
    {
        if (currentMatches.ContainsKey(matchID) == false) return;
        Match match = currentMatches[matchID];
        int p = (int)match.turn;
        PlayerConnection player = match.players[p];
        if (player.clientID != clientID || player.playerID != playerID) return;

   
        Board.HandCard card = match.hands[p][index];
        match.boards[p].Add(card.card);
        match.hands[p].RemoveAt(index);
        
        //TODO: ON PLAY EFFECTS (JUGGLER ETC)
        //TODO: BATTLECRY

    }
    

    [Serializable]
    public class Match
    {
        public List<PlayerConnection> players = new List<PlayerConnection>();
        public ulong matchID;
        public List<int> healths = new List<int>() { 30, 30 };

        public List<int> maxMana = new List<int>() { 0, 0 };
        public List<int> currentMana = new List<int>() { 0, 0 };

        public Turn turn = Turn.player1;

        public List<List<Card.Cardname>> decks = new List<List<Card.Cardname>>() { new List<Card.Cardname>(), new List<Card.Cardname>()};

        public List<Board.Hand> hands = new List<Board.Hand>() {  new Board.Hand(), new Board.Hand()};

        public List<bool> mulligans = new List<bool>() { false, false };

        public List<Board.MinionBoard> boards = new List<Board.MinionBoard>();

        //todo: on board minions
        //todo: secrets
        //todo: decks, graveyards
        
        public void InitMatch(PlayerConnection p1, PlayerConnection p2, ulong mID)
        {
            players.Add(p1);
            players.Add(p2);
            List<Card.Cardname> sampleTestDeck = new List<Card.Cardname>();

            for (int i=0;i<15;i++)
            {
                sampleTestDeck.Add((Card.Cardname)i);
                sampleTestDeck.Add((Card.Cardname)i);
            }
            //decks = new List<List<Card.Cardname>>();
            
            decks[0] = new List<Card.Cardname>(Board.Shuffle(sampleTestDeck));
            decks[1] = new List<Card.Cardname>(Board.Shuffle(sampleTestDeck));

            turn = Board.RNG(50) ? Turn.player1 : Turn.player2;

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
