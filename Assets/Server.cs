using System;
using System.Collections;
using System.Collections.Generic;
using Riptide;
using Riptide.Transports;
using Riptide.Utils;
using System.Linq;
using UnityEngine;
using UnityEditor;
using static Server;
using System.ComponentModel;
using System.Text.RegularExpressions;
using UnityEditor.PackageManager;
using static Board;


public partial class Server : MonoBehaviour
{
    public static Message CreateMessage(MessageType type)
    {
        return Message.Create(MessageSendMode.Reliable, (ushort)type);
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
        ConfirmAttackMinion,

        AttackFace,
        ConfirmAttackFace,

        SwingMinion,
        ConfirmSwingMinion,

        SwingFace,
        ConfirmSwingFace,

        UpdateMinion,
        UpdateHero,
        Trigger,

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
        
        server.Send(m, matchList[0].players[0].connection.clientID); ;
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
                ulong playMatchID = message.GetULong();
                ulong playPlayerID = message.GetULong();
                int playIndex = message.GetInt();
                int playTarget = message.GetInt();
                int playPosition = message.GetInt();
                bool playFriendlySide = message.GetBool();
                bool playIsHero = message.GetBool();
                PlayCard(playMatchID, clientID, playPlayerID, playIndex, playTarget, playPosition, playFriendlySide, playIsHero);
                break;
            case MessageType.EndTurn:
                ulong endMatchID = message.GetULong();
                ulong endPlayerID = message.GetULong();
                EndTurn(endMatchID, clientID, endPlayerID);
                break;
            case MessageType.AttackMinion:
                ulong attackMatchID = message.GetULong();
                ulong attackPlayerID = message.GetULong();
                int attackerInd = message.GetInt();
                int targetMinionInd = message.GetInt();
                AttackMinion(attackMatchID, clientID, attackPlayerID, attackerInd, targetMinionInd);
                break;
            case MessageType.AttackFace:
                AttackFace(message, clientID);
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

        Message m1 = CreateMessage(MessageType.ConfirmMatch);
        m1.AddULong(currMatchID);
        server.Send(m1,p1.clientID);

        Message m2 = CreateMessage(Server.MessageType.ConfirmMatch);
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

    List<Card.Cardname> TESTCARDS = new List<Card.Cardname>() { Card.Cardname.Mortal_Coil};

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

        Message m1 = CreateMessage(Server.MessageType.DrawHand);
        string jsonText = JsonUtility.ToJson(m.players[0].hand);
        m1.AddString(jsonText);
        m1.AddInt(m.players[1].hand.Count());
        server.Send(m1, m.players[0].connection.clientID);

        Message m2 = CreateMessage(Server.MessageType.DrawHand);
        jsonText = JsonUtility.ToJson(m.players[1].hand);
        m2.AddString(jsonText);
        m2.AddInt(m.players[0].hand.Count());
        server.Send(m2, m.players[1].connection.clientID);
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

        Message message = CreateMessage(Server.MessageType.ConfirmMulligan);
        string jsonText = JsonUtility.ToJson(match.players[player].hand);
        message.AddString(jsonText);
        server.Send(message, match.players[player].connection.clientID);

        Message enemyMullMessage = CreateMessage(MessageType.EnemyMulligan);
        enemyMullMessage.AddInts(inds);
        server.Send(enemyMullMessage, match.OtherPlayer(match.players[player]).connection.clientID);

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
        server.Send(messageFirst, match.players[f].connection.clientID);
        server.Send(messageSecond, match.players[s].connection.clientID);

        StartTurn(match);
    }

    public void StartTurn(Match match)
    {
        Message message = CreateMessage(Server.MessageType.StartTurn);
        Message messageEnemy = CreateMessage(Server.MessageType.StartTurn);
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
        messageEnemy.Add(false);
        messageEnemy.Add(match.currPlayer.maxMana);
        messageEnemy.Add(match.currPlayer.currMana);


        server.Send(message, match.currPlayer.connection.clientID);
        server.Send(messageEnemy, match.enemyPlayer.connection.clientID);

        DrawCards(match, match.turn, 1);
    }
    public void EndTurn(ulong matchID, ushort clientID, ulong playerID)
    {
        Match m = currentMatches[matchID];
        PlayerConnection player = m.players[(int)m.turn].connection;
        if (player.clientID != clientID || player.playerID != playerID) return;

        //TODO: end of turn effects
        m.turn = m.turn == Turn.player1 ? Turn.player2 : Turn.player1;

        m.currPlayer = m.players[(int)m.turn];
        m.enemyPlayer = m.players[m.Opponent((int)m.turn)];

        Message msg = CreateMessage(Server.MessageType.EndTurn);
        server.Send(msg, player.clientID);

        StartTurn(m);
    }
    public void DrawCards(Match match, Turn player, int count = 1)
    {
        int p = (int)player;
        List<Card.Cardname> drawnCards = new List<Card.Cardname>();
        for (int i = 0; i < count; i++)
        {
            Card.Cardname top = match.players[p].deck[0];
            match.players[p].deck.RemoveAt(0);
            match.players[p].hand.Add(top);
            drawnCards.Add(top);

            Message message = CreateMessage(Server.MessageType.DrawCards);
            message.AddInt((int)top);
            server.Send(message, match.players[p].connection.clientID);
        
            Message messageOpp = CreateMessage(Server.MessageType.DrawEnemy);
            messageOpp.AddInt(1);
            server.Send(messageOpp, match.OtherPlayer(match.players[p]).connection.clientID);
        }

    }
        
    public void PlayCard(ulong matchID, ushort clientID, ulong playerID, int index, int target, int position, bool friendlySide, bool isHero)
    {
        if (currentMatches.ContainsKey(matchID) == false) return;
        Match match = currentMatches[matchID];
        int p = (int)match.turn;
        PlayerConnection player = match.players[p].connection;
        PlayerConnection opponent = match.players[match.Opponent(p)].connection;
        if (player.clientID != clientID || player.playerID != playerID) return;
        
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

        server.Send(confirmPlay, player.clientID); 
        server.Send(confirmPlayOpponent, opponent.clientID);
        //summon minion or execute spell effects

        if (card.SPELL)
        {
            //trigger event: ON PLAY SPELL (antonidas)
        }

        if (card.MINION)
        {
            SummonMinion(match, match.turn, card.card, position);
            //trigger event: ON PLAY MINION (juggler)
        }


        //TODO: BATTLECRY (if minion) Trigger(Battlecry(Target X) - passed in the target spot of this func)

    }
    
    public void SummonMinion(Match match, Turn side, Card.Cardname minion, int position=-1)
    {
        int p = (int)side;
        int o = match.Opponent(p);
        if (match.players[p].board.Count() >= 7) return;

        match.players[p].board.Add(minion, position);

        Message message = CreateMessage(Server.MessageType.SummonMinion);
        message.AddBool(true);
        message.AddInt((int)minion);
        message.AddInt(position);
        server.Send(message, match.players[p].connection.clientID);
        
        Message messageOp = CreateMessage(Server.MessageType.SummonMinion);
        messageOp.AddBool(false);
        messageOp.AddInt((int)minion);
        messageOp.AddInt(position);
        server.Send(messageOp, match.players[o].connection.clientID);
    }

    public void SummonToken(Match match, Turn side, Card.Cardname minion, int position = -1)
    {
        //TODO: Summon token
        //TRIGGER: ON MINION SUMMON
    }

    public void AttackMinion(ulong matchID, ushort clientID, ulong playerID, int attackerInd, int targetInd)
    {
        if (currentMatches.ContainsKey(matchID) == false) return;
        Match match = currentMatches[matchID];

        PlayerConnection player = match.currPlayer.connection;
        PlayerConnection enemy = match.enemyPlayer.connection;
        if (player.clientID != clientID || player.playerID != playerID) return;

        Board.Minion attacker = match.currPlayer.board[attackerInd];
        Board.Minion target = match.enemyPlayer.board[targetInd];
        if (ValidAttackMinion(match, attackerInd, targetInd) == false) return;

        ConfirmAttackMinion(match, attackerInd, targetInd);
        Debug.Log("Attack " + attacker.ToString() + " " + target.ToString());

        //TODO: onattack triggers

        ConsumeAttackCharge(attacker);

        DamageMinion(match, attacker, target.damage);
        DamageMinion(match, target, target.damage);

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

        PlayerConnection owner = match.FindOwner(minion).connection;
        PlayerConnection opponent = match.FindOpponent(minion).connection;

        server.Send(messageOwner, owner.clientID);
        server.Send(messageOpponent, opponent.clientID);
    }
    public void DestroyMinion(Match match, Board.Minion minion)
    {
        Player owner = match.FindOwner(minion);
        Player opponent = match.OtherPlayer(owner);

        int ind = minion.index;

        owner.board.RemoveAt(ind);
        Message messageOwner = CreateMessage(MessageType.DestroyMinion);
        Message messageOpponent = CreateMessage(MessageType.DestroyMinion);

        messageOwner.AddInt(ind);
        messageOpponent.AddInt(ind);

        messageOwner.AddBool(true);
        messageOpponent.AddBool(false);

        server.Send(messageOwner, owner.connection.clientID);
        server.Send(messageOpponent, opponent.connection.clientID);
        
        //TODO: ON MINION DEATH TRIGGERS
        //TODO: DEATHRATTLE TRIGGERS
    }

    public void AttackFace(Message message, ushort clientID)
    {
        ulong matchID = message.GetULong();
        ulong playerID = message.GetULong();
        
        int attackerInd = message.GetInt();

        if (currentMatches.ContainsKey(matchID) == false) return;
        Match match = currentMatches[matchID];

        PlayerConnection player = match.currPlayer.connection;
        PlayerConnection enemy = match.enemyPlayer.connection;
        if (player.clientID != clientID || player.playerID != playerID) return;

        Board.Minion attacker = match.currPlayer.board[attackerInd];
        
        if (ValidAttackFace(match,match.currPlayer,match.enemyPlayer,attackerInd) == false) return;
        ConfirmAttackFace(match,attackerInd);
        ConsumeAttackCharge(attacker);

        //TODO: ON ATTACK TRIGGERS

        DamageFace(match, match.enemyPlayer, attacker.damage);
    }

    public void UpdateHero(Match match, Player player)
    {
        Message messageOwner = CreateMessage(MessageType.UpdateHero);
        Message messageOpponent = CreateMessage(MessageType.UpdateHero);

        messageOwner.AddInt(player.health);
        messageOpponent.AddInt(player.health);

        messageOwner.AddBool(true);
        messageOpponent.AddBool(false);

        server.Send(messageOwner, player.connection.clientID);
        server.Send(messageOpponent, match.OtherPlayer(player).connection.clientID);
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
    }


    [Serializable]
    public class Match
    {
        public List<Player> players = new List<Player>() { new Player(), new Player() };
        public ulong matchID;
        public Turn turn = Turn.player1;
        public Player currPlayer;
        public Player enemyPlayer;
        //todo: secrets
        //todo: graveyards

        public void InitMatch(PlayerConnection p1, PlayerConnection p2, ulong mID)
        {
            players[0].connection=p1;
            players[1].connection=p2;
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
        public Player OtherPlayer(Player p)
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
            return OtherPlayer(FindOwner(minion));
        }
    }
    
}
