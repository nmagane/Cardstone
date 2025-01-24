using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq.Expressions;
using System.Runtime.InteropServices.WindowsRuntime;
using Riptide;
using Riptide.Utils;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;


public partial class Server : MonoBehaviour
{

    List<Card.Cardname> TESTCARDS = new List<Card.Cardname>() { Card.Cardname.YoungPri,Card.Cardname.Acolyte,Card.Cardname.DireWolf,Card.Cardname.KnifeJuggler, Card.Cardname.Ping };
    public static Message CreateMessage(MessageType type)
    {
        Message m = Message.Create(MessageSendMode.Reliable, (ushort)type);
        m.ReserveBits(16);
        return m;
    }
    public void SendMessage(Message message, Player player)
    {
        message.SetBits(player.messageCount++, 16, 28);
        server.Send(message, player.connection.clientID);
    }
    public enum MessageType
    {
        Matchmaking,
        ConfirmMatch,

        SubmitMulligan,
        ConfirmMulligan,
        EnemyMulligan,

        PlayCard,

        StartGame,
        StartTurn,
        EndTurn,

        DrawCards,
        DrawHand,
        DrawEnemy,

        SummonMinion,
        DestroyMinion,

        AttackMinion,
        ConfirmPreAttackMinion,
        ConfirmAttackMinion,

        AttackFace,
        ConfirmPreAttackFace,
        ConfirmAttackFace,

        SwingMinion,
        ConfirmPreSwingMinion,
        ConfirmSwingMinion,

        SwingFace,
        ConfirmPreSwingFace,
        ConfirmSwingFace,

        UpdateMinion,
        UpdateHero,

        TriggerMinion,

        DiscardCard,
        MillCard,

        Concede,

        _TEST
    }
    public Riptide.Server server = new Riptide.Server();

    void Start()
    {
        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);
        ushort port = 8888;
        ushort maxPlayers = 65534; //ushort.max-1
        server.Start(port, maxPlayers,0,false);
        server.MessageReceived += OnMessageReceived;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Tester();
        }
    }
    void Tester()
    {
        Message m = CreateMessage(MessageType._TEST);
        m.ReserveBits(16);
        m.AddInt(8885444);
        m.AddBool(false);
        ushort messageOrder = 1;
        m.SetBits(messageOrder, 16, 28);
        
        //server.Send(m, matchList[0].players[0].connection.clientID);
        SendMessage(m, matchList[0].players[0]);
    }
    private void FixedUpdate()
    {
        server.Update();
    }

    public void OnMessageReceived(object sender, MessageReceivedEventArgs eventArgs)
    {
        MessageType messageID = (MessageType)eventArgs.MessageId;
        ushort clientID = eventArgs.FromConnection.Id;
        Message originalMessage = eventArgs.Message;
        ushort count = originalMessage.GetUShort();
        CustomMessage message;
        bool orderedMessage = false;
        //UNORDERED MESSAGES, OUT OF GAME (NO MATCH ID ATTACHED)
        switch (messageID)
        {
            case MessageType.Matchmaking:
                message = CopyMessage(originalMessage, messageID);
                ParseMessage(messageID, clientID, message, 0);
                orderedMessage = false;
                break;

            case MessageType.SubmitMulligan:
                //ulong matchID = message.GetULong();
                message = CopyMessage(originalMessage, messageID);
                ParseMessage(messageID, clientID, message, 0);
                orderedMessage = false;
                break;

            default:
                orderedMessage = true;
                break;
        }
        Debug.Log("SERVER RECEIVED MESSAGE: " + count);
        if (orderedMessage)
        {
            ulong matchID = originalMessage.GetULong();
            message = CopyMessage(originalMessage, messageID);
            if (currentMatches.ContainsKey(matchID) == false) return;
            currentMatches[matchID].ReceiveMessage(messageID, clientID, message, count);
        }
    }

    public void ParseMessage(MessageType messageID, ushort clientID, CustomMessage message, ulong matchID)//(object sender, MessageReceivedEventArgs eventArgs, ulong matchID = 0)
    {
        //int messageID = eventArgs.MessageId;
        //ushort clientID = eventArgs.FromConnection.Id;
       // Message message = eventArgs.Message;

        switch (messageID)
        {
            case MessageType.Matchmaking:
                ulong queuePlayerID = message.GetULong();
                ushort queueClientID = clientID;
                AddToQueue(queueClientID, queuePlayerID);
                break;
            case MessageType.SubmitMulligan:
                ulong mullMatchID = message.GetULong();
                int[] mullInds = message.GetInts();
                ulong mullPlayerID = message.GetULong();
                Mulligan(mullInds, mullMatchID, mullPlayerID);
                break;
            case MessageType.PlayCard:
                //ulong playMatchID = message.GetULong();
                ulong playPlayerID = message.GetULong();
                int playIndex = message.GetInt();
                int playTarget = message.GetInt();
                int playPosition = message.GetInt();
                bool playFriendlySide = message.GetBool();
                bool playIsHero = message.GetBool();
                PlayCard(matchID, clientID, playPlayerID, playIndex, playTarget, playPosition, playFriendlySide, playIsHero);
                break;
            case MessageType.EndTurn:
               //ulong endMatchID = message.GetULong();
                ulong endPlayerID = message.GetULong();
                EndTurn(matchID, clientID, endPlayerID);
                break;
            case MessageType.AttackMinion:
                //ulong attackMatchID = message.GetULong();
                ulong attackPlayerID = message.GetULong();
                int attackerInd = message.GetInt();
                int targetMinionInd = message.GetInt();
                AttackMinion(matchID, clientID, attackPlayerID, attackerInd, targetMinionInd);
                break;
            case MessageType.AttackFace:
                ulong playerIDFace = message.GetULong();
                int attackerIndFace = message.GetInt();
                AttackFace(matchID, clientID,playerIDFace,attackerIndFace);
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
        match.server = this;

        Message m1 = CreateMessage(MessageType.ConfirmMatch);
        m1.AddULong(currMatchID);
        //server.Send(m1,p1.clientID);

        Message m2 = CreateMessage(MessageType.ConfirmMatch);
        m2.AddULong(currMatchID);
        //server.Send(m2,p2.clientID);

        match.InitMatch(p1, p2, currMatchID);
        SendMessage(m1, match.players[0]);
        SendMessage(m2, match.players[1]);
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
            m.players[0].hand.Add(m.players[0].deck[0]);
            m.players[0].deck.RemoveAt(0);
        }
        for (int i=0;i< p2cards; i++)
        {
            m.players[1].hand.Add(m.players[1].deck[0]);
            m.players[1].deck.RemoveAt(0);
        }
        if (m.turn==Turn.player1)
        {
            //TODO: add coin p2
        }
        else
        {
            //TODO: add coin p1
        }

        foreach (var v in TESTCARDS) m.players[0].hand.Add(v);

        //TODO: THIS MESSAGE SIZE MIGHT GET TOO LARGE TO SEND, CHANGE TO ARRAY OF ENUMS ONLY?
        List<ushort> hand1 = new List<ushort>();
        List<ushort> hand2 = new List<ushort>();
        foreach(var v in m.players[0].hand)
        {
            hand1.Add((ushort)v.card);
        }
        
        foreach(var v in m.players[1].hand)
        {
            hand2.Add((ushort)v.card);
        }
        Message m1 = CreateMessage(Server.MessageType.DrawHand);
        //string jsonText = JsonUtility.ToJson(m.players[0].hand);
        //m1.AddString(jsonText);
        m1.AddUShorts(hand1.ToArray());
        m1.AddInt(m.players[1].hand.Count());
        //server.Send(m1, m.players[0].connection.clientID);
        SendMessage(m1, m.players[0]);

        Message m2 = CreateMessage(Server.MessageType.DrawHand);
        //jsonText = JsonUtility.ToJson(m.players[1].hand);
        //m2.AddString(jsonText);
        m2.AddUShorts(hand2.ToArray());
        m2.AddInt(m.players[0].hand.Count());
        //server.Send(m2, m.players[1].connection.clientID);
        SendMessage(m2, m.players[1]);
    }

    public void Mulligan(int[] inds, ulong matchID, ulong playerID)
    {
        if (currentMatches.ContainsKey(matchID) == false) return;
        Match match = currentMatches[matchID];
        Board.Hand hand= match.players[0].hand;
        int player = 0;
        if (playerID == match.players[0].connection.playerID)
        {
            player = 0;
        }
        else if (playerID == match.players[1].connection.playerID)
        {
            player = 1;
        }
        else return;

        if (match.players[player].mulligan) return; //already mulliganed this player

        hand = match.players[player].hand;
        List<Card.Cardname> returningCards = new List<Card.Cardname>();

        foreach (int i in inds)
        {
            if (i >= hand.Count())
                continue;
            returningCards.Add(hand[i].card);
            Card.Cardname newCard = match.players[player].deck[0];
            match.players[player].deck.RemoveAt(0);
            hand[i] = new Board.HandCard(newCard, i);
        }
        match.players[player].deck.AddRange(returningCards);
        match.players[player].deck = Board.Shuffle(match.players[player].deck);
        match.players[player].mulligan = true;

        List<ushort> newhand = new List<ushort>();
        foreach (var c in match.players[player].hand)
        {
            newhand.Add((ushort)c.card);
        }
        Message message = CreateMessage(Server.MessageType.ConfirmMulligan);
        message.AddUShorts(newhand.ToArray());
        //server.Send(message, match.players[player].connection.clientID);
        SendMessage(message, match.players[player]);

        Message enemyMullMessage = CreateMessage(MessageType.EnemyMulligan);
        enemyMullMessage.AddInts(inds);
        //server.Send(enemyMullMessage, match.Opponent(match.players[player]).connection.clientID);
        SendMessage(enemyMullMessage, match.players[player].opponent);

        if (match.players[0].mulligan && match.players[1].mulligan)
        {
            StartGame(match);
            Debug.Log("both mulls complete");
        }
    }

    public void StartGame(Match match)
    {

        Message messageFirst = CreateMessage(Server.MessageType.StartGame);
        Message messageSecond = CreateMessage(Server.MessageType.StartGame);
        messageFirst.AddBool(true);
        messageSecond.AddBool(false);
        int f = match.turn == Turn.player1 ? 0 : 1;
        int s = match.turn == Turn.player1 ? 1 : 0;
        match.started = true;
        match.messageCount = 0;
        //server.Send(messageFirst, match.players[f].connection.clientID);
        //server.Send(messageSecond, match.players[s].connection.clientID);
        SendMessage(messageFirst, match.players[f]);
        SendMessage(messageSecond, match.players[s]);
        StartTurn(match);
    }

    public void StartTurn(Match match)
    {
        Message message = CreateMessage(MessageType.StartTurn);
        Message messageEnemy = CreateMessage(MessageType.StartTurn);
        //int p = (int)match.turn;

        match.currPlayer.maxMana = Mathf.Min(match.currPlayer.maxMana + 1, 10);
        match.currPlayer.currMana = match.currPlayer.maxMana;
        foreach (var m in match.currPlayer.board)
        {
            m.canAttack = true;
        }
        //TODO: start of turn effects
        message.Add(true);
        message.Add(match.currPlayer.maxMana);
        message.Add(match.currPlayer.currMana);
        message.AddUShort(match.messageCount);
        messageEnemy.Add(false);
        messageEnemy.Add(match.currPlayer.maxMana);
        messageEnemy.Add(match.currPlayer.currMana);
        messageEnemy.AddUShort(match.messageCount);

        //server.Send(message, match.currPlayer.connection.clientID);
        SendMessage(message, match.currPlayer);
        //server.Send(messageEnemy, match.enemyPlayer.connection.clientID);
        SendMessage(messageEnemy, match.enemyPlayer);

        CastInfo startTurnInfo = new CastInfo(match, match.currPlayer, null, -1, -1, false, false);
        match.StartSequenceStartTurn(startTurnInfo);
        //DrawCard(match, match.currPlayer);
    }
    public void EndTurn(ulong matchID, ushort clientID, ulong playerID)
    {
        Match m = currentMatches[matchID];
        PlayerConnection player = m.players[(int)m.turn].connection;
        Player Ender = m.players[(int)m.turn];
        if (player.clientID != clientID || player.playerID != playerID) return;

        //SEQUENCE
        CastInfo startTurnInfo = new CastInfo(m, m.currPlayer, null, -1, -1, false, false);
        m.StartSequenceEndTurn(startTurnInfo);

        //====LOGIC
        m.turn = m.turn == Turn.player1 ? Turn.player2 : Turn.player1;

        m.currPlayer = m.players[(int)m.turn];
        m.enemyPlayer = m.players[m.Opponent((int)m.turn)];

        Message msg = CreateMessage(Server.MessageType.EndTurn);
        //server.Send(msg, player.clientID);
        SendMessage(msg, Ender);
        //=========
        StartTurn(m);
    }
    public Board.HandCard DrawCard(Match match, Player player)
    {
        //int p = (int)player;
        List<Card.Cardname> drawnCards = new List<Card.Cardname>();

        Card.Cardname top = player.deck[0];
        player.deck.RemoveAt(0);
        Board.HandCard drawnCard = player.hand.Add(top);
        drawnCards.Add(top);

        Message message = CreateMessage(Server.MessageType.DrawCards);
        message.AddInt((int)top);
        //server.Send(message, player.connection.clientID);
        SendMessage(message, player);
        
        Message messageOpp = CreateMessage(Server.MessageType.DrawEnemy);
        messageOpp.AddInt(1);
        //server.Send(messageOpp, player.opponent.connection.clientID);
        SendMessage(messageOpp, player.opponent);

        return drawnCard;

    }
        
    public void PlayCard(ulong matchID, ushort clientID, ulong playerID, int index, int target, int position, bool friendlySide, bool isHero)
    {
        if (currentMatches.ContainsKey(matchID) == false) return;
        Match match = currentMatches[matchID];
        int p = (int)match.turn;
        PlayerConnection player = match.players[p].connection;
        PlayerConnection opponent = match.players[match.Opponent(p)].connection;
        if (player.clientID != clientID || player.playerID != playerID) return;

        if (index >= match.players[p].hand.Count()) return;
        Board.HandCard card = match.players[p].hand[index];
        
        //check if play is legal
        if (match.players[p].currMana < card.manaCost) return;


        if (card.MINION && match.players[p].board.Count()>=7)
        {
            return;
        }

        //====================
        match.players[p].currMana -= card.manaCost;
        match.players[p].hand.RemoveAt(index);
        
        //send confirm play message to both sides
        Message confirmPlay = CreateMessage(Server.MessageType.PlayCard);
        Message confirmPlayOpponent = CreateMessage(Server.MessageType.PlayCard);

        confirmPlay.AddBool(true); confirmPlayOpponent.AddBool(false);
        confirmPlay.AddInt(index); confirmPlayOpponent.AddInt(index);
        confirmPlay.AddInt(card.manaCost); confirmPlayOpponent.AddInt(card.manaCost);
        confirmPlay.AddInt((int)card.card); confirmPlayOpponent.AddInt((int)card.card);

        //server.Send(confirmPlay, player.clientID);
        SendMessage(confirmPlay, match.players[p]);
        //server.Send(confirmPlayOpponent, opponent.clientID);
        SendMessage(confirmPlayOpponent, match.players[p].opponent);

        //summon minion or execute spell effects

        match.playOrder++;

        CastInfo spell = new CastInfo(match, match.players[p], card, target,position, friendlySide, isHero);
        if (card.SPELL)
        {
            match.StartSequencePlaySpell(spell);
        }

        if (card.MINION)
        {
            match.StartSequencePlayMinion(spell);
        }
    }
    public Board.Minion SummonMinion(Match match, Player player, Card.Cardname minion, int position=-1)
    {

        Player opponent = match.Opponent(player);
        if (player.board.Count() >= 7) return null;

        Board.Minion m = player.board.Add(minion, position,match.playOrder);

        Message message = CreateMessage(Server.MessageType.SummonMinion);
        message.AddBool(true);
        message.AddInt((int)minion);
        message.AddInt(position);
        //server.Send(message, player.connection.clientID);
        SendMessage(message, player);
        
        Message messageOp = CreateMessage(Server.MessageType.SummonMinion);
        messageOp.AddBool(false);
        messageOp.AddInt((int)minion);
        messageOp.AddInt(position);
        //server.Send(messageOp, opponent.connection.clientID);
        SendMessage(messageOp, opponent);

        return m;
    }

    public void SummonToken(Match match, Turn side, Card.Cardname minion, int position = -1)
    {
        //TODO: Summon token
        //StartSequenceSummonMinion(spell); - different sequence than play minion
    }

    public void AttackMinion(ulong matchID, ushort clientID, ulong playerID, int attackerInd, int targetInd)
    {
        if (currentMatches.ContainsKey(matchID) == false) return;
        Match match = currentMatches[matchID];

        PlayerConnection player = match.currPlayer.connection;
        PlayerConnection enemy = match.enemyPlayer.connection;
        if (player.clientID != clientID || player.playerID != playerID) return;

        if (attackerInd >= match.currPlayer.board.Count()) return;
        if (targetInd >= match.enemyPlayer.board.Count()) return;

        Board.Minion attacker = match.currPlayer.board[attackerInd];
        Board.Minion target = match.enemyPlayer.board[targetInd];
        if (ValidAttackMinion(match, attackerInd, targetInd) == false) return;

        Debug.Log("Attack " + attacker.ToString() + " " + target.ToString());

        AttackInfo attackInfo = new AttackInfo(match.currPlayer, attacker, target, false, false, false);
        CastInfo attackAction = new CastInfo(match, attackInfo);

        //preattack confirmation -> start sequence
        ConfirmAttackGeneral(attackAction, true);

        match.StartSequenceAttackMinion(attackAction);

    }
    public void UpdateMinion(Match match, Board.Minion minion)
    {
        Message messageOwner = CreateMessage(MessageType.UpdateMinion);
        Message messageOpponent = CreateMessage(MessageType.UpdateMinion);

        string jsonText = JsonUtility.ToJson(minion);
        messageOwner.AddString(jsonText);
        messageOpponent.AddString(jsonText);

        messageOwner.AddBool(true);
        messageOpponent.AddBool(false);

        Player owner = match.FindOwner(minion);
        Player opponent = match.FindOpponent(minion);

        //server.Send(messageOwner, owner.clientID);
        SendMessage(messageOwner, owner);
        //server.Send(messageOpponent, opponent.clientID);
        SendMessage(messageOpponent, opponent);
    }
    public void DestroyMinion(Match match, Board.Minion minion)
    {
        minion.DEAD = true;
        Player owner = match.FindOwner(minion);
        Player opponent = match.Opponent(owner);

        int ind = minion.index;

        owner.board.RemoveAt(ind);
        Message messageOwner = CreateMessage(MessageType.DestroyMinion);
        Message messageOpponent = CreateMessage(MessageType.DestroyMinion);

        messageOwner.AddInt(ind);
        messageOpponent.AddInt(ind);

        messageOwner.AddBool(true);
        messageOpponent.AddBool(false);

        //server.Send(messageOwner, owner.connection.clientID);
        SendMessage(messageOwner, owner);
        //server.Send(messageOpponent, opponent.connection.clientID);
        SendMessage(messageOpponent, opponent);
        
        //TODO: ON MINION DEATH TRIGGERS
        //TODO: DEATHRATTLE TRIGGERS
    }
    public void AttackFace(ulong matchID,ushort clientID, ulong playerID, int attackerInd)
    {
        if (currentMatches.ContainsKey(matchID) == false) return;
        Match match = currentMatches[matchID];

        PlayerConnection player = match.currPlayer.connection;
        PlayerConnection enemy = match.enemyPlayer.connection;
        if (player.clientID != clientID || player.playerID != playerID) return;

        if (attackerInd >= match.currPlayer.board.Count()) return;
        Board.Minion attacker = match.currPlayer.board[attackerInd];
        
        if (ValidAttackFace(match,match.currPlayer,match.enemyPlayer,attackerInd) == false) return;

        AttackInfo attackInfo = new AttackInfo(match.currPlayer, attacker, null, false, true, false);
        CastInfo attackAction = new CastInfo(match, attackInfo);

        //preattack confirmation -> start sequence
        ConfirmAttackGeneral(attackAction, true);

        match.StartSequenceAttackFace(attackAction);
    }



    public void UpdateHero(Match match, Player player)
    {
        Message messageOwner = CreateMessage(MessageType.UpdateHero);
        Message messageOpponent = CreateMessage(MessageType.UpdateHero);

        messageOwner.AddInt(player.health);
        messageOpponent.AddInt(player.health);

        messageOwner.AddBool(true);
        messageOpponent.AddBool(false);

        //server.Send(messageOwner, player.connection.clientID);
        SendMessage(messageOwner, player);
        //server.Send(messageOpponent, match.Opponent(player).connection.clientID);
        SendMessage(messageOpponent, player.opponent);
    }

    public void DiscardCard(Match m, Player p, int index)
    {
        Message messageOwner = CreateMessage(MessageType.DiscardCard);
        Message messageOpponent = CreateMessage(MessageType.DiscardCard);

        messageOwner.AddBool(true);
        messageOpponent.AddBool(false);
    }

    [Serializable]
    public class Player
    {
        public PlayerConnection connection = new PlayerConnection();

        public int health = 30;
        public int maxMana = 0;
        public int currMana = 0;

        public bool turn = false;
        public List<Card.Cardname> deck = new List<Card.Cardname>();
        public Board.Hand hand = new Board.Hand();
        public Board.MinionBoard board = new Board.MinionBoard();

        public bool mulligan = false;

        public Player opponent;

        [System.NonSerialized]
        public Match match;

        public ushort messageCount = 0; //Server messages sent to player 
    }


    [Serializable]
    public partial class Match
    {
        public List<Player> players = new List<Player>() { new Player(), new Player() };
        public ulong matchID;
        public Turn turn = Turn.player1;
        public Player currPlayer;
        public Player enemyPlayer;
        public Server server;
        public int playOrder = 0;
        public bool started = false;
        public ushort messageCount = 0;
        List<(MessageType, ushort, CustomMessage, ushort)> messageQue = new();
        //todo: secrets
        //todo: graveyards

        public void InitMatch(PlayerConnection p1, PlayerConnection p2, ulong mID)
        {
            players[0].connection=p1;
            players[1].connection=p2;
            players[0].match=this;
            players[1].match=this;
            players[0].opponent = players[1];
            players[1].opponent = players[0];
            List<Card.Cardname> sampleTestDeck = new List<Card.Cardname>();

            for (int i=0;i<15;i++)
            {
                sampleTestDeck.Add((Card.Cardname)i);
                sampleTestDeck.Add((Card.Cardname)i);
            }
            //decks = new List<List<Card.Cardname>>();
            
            players[0].deck = new List<Card.Cardname>(Board.Shuffle(sampleTestDeck));
            players[1].deck = new List<Card.Cardname>(Board.Shuffle(sampleTestDeck));

            turn = Board.RNG(50) ? Turn.player1 : Turn.player2;
            currPlayer = players[(int)turn];
            enemyPlayer = players[Opponent((int)turn)];
            
            matchID = mID;

            players[0].hand.server = true;
            players[1].hand.server = true;

            players[0].board.server = true;
            players[1].board.server = true;
        }

        public void ReceiveMessage(MessageType messageID,ushort clientID,CustomMessage message, ushort count)
        {
            if (count == messageCount)
            {
                messageCount++;
                server.ParseMessage(messageID, clientID, message, matchID);
            }
            else
            {
                messageQue.Add((messageID, clientID, message, count));
            }

            for (int i=0;i<messageQue.Count;i++)
            {
                var v = messageQue[i];
                if (v.Item4 == messageCount)
                {
                    Debug.Log("executing message " + messageCount);
                    ReceiveMessage(v.Item1, v.Item2, v.Item3,v.Item4);
                    messageQue.Remove(v);
                    break;
                }
            }
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

        public int Opponent(int x)
        {
            if (x == 1) return 0;
            else if (x == 0) return 1;
            return 1;
        }
        public Player Opponent(Player p)
        {
            if (p == players[0]) return players[1];
            if (p == players[1]) return players[0];

            return players[0];
        }
        public Player FindOwner(Board.Minion minion)
        {
            if (players[0].board.Contains(minion))
                return players[0];
            if (players[1].board.Contains(minion))
                return players[1];
            return players[0];
        }
        public Player FindOpponent(Board.Minion minion)
        {
            return Opponent(FindOwner(minion));
        }
    }

    public CustomMessage CopyMessage(Message message, MessageType type)
    {
        CustomMessage result = new CustomMessage();
        switch (type)
        {
            case MessageType.Matchmaking:
                ulong queuePlayerID = message.GetULong();
                result.AddULong(queuePlayerID);
                break;
            case MessageType.SubmitMulligan:
                ulong mullMatchID = message.GetULong();
                int[] mullInds = message.GetInts();
                ulong mullPlayerID = message.GetULong();
                result.AddULong(mullMatchID);
                result.AddInts(mullInds);
                result.AddULong(mullPlayerID);
                break;
            case MessageType.PlayCard:
                //ulong playMatchID = message.GetULong();
                ulong playPlayerID = message.GetULong();
                int playIndex = message.GetInt();
                int playTarget = message.GetInt();
                int playPosition = message.GetInt();
                bool playFriendlySide = message.GetBool();
                bool playIsHero = message.GetBool();
                result.AddULong(playPlayerID);
                result.AddInt(playIndex);
                result.AddInt(playTarget);
                result.AddInt(playPosition);
                result.AddBool(playFriendlySide);
                result.AddBool(playIsHero);
                break;
            case MessageType.EndTurn:
                //ulong endMatchID = message.GetULong();
                ulong endPlayerID = message.GetULong();
                result.AddULong(endPlayerID);
                break;
            case MessageType.AttackMinion:
                //ulong attackMatchID = message.GetULong();
                ulong attackPlayerID = message.GetULong();
                int attackerInd = message.GetInt();
                int targetMinionInd = message.GetInt();
                result.AddULong(attackPlayerID);
                result.AddInt(attackerInd);
                result.AddInt(targetMinionInd);
                break;
            case MessageType.AttackFace:
                ulong playerIDFace = message.GetULong();
                int attackerIndFace = message.GetInt();
                result.AddULong(playerIDFace);
                result.AddInt(attackerIndFace);
                break;
            default:
                Debug.LogError("MESSAGE TYPE UNRECOGNIZED");
                break;

        }
        return result;
    }
}
