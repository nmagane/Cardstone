using System;
using System.Collections.Generic;
using Mirror.Examples.MultipleAdditiveScenes;
using Mirror.SimpleWeb;
using UnityEngine;

public partial class Server : MonoBehaviour
{
    public NetworkHandler mirror;
#if UNITY_EDITOR
    List<Card.Cardname> TESTCARDS = new List<Card.Cardname>() {Card.Cardname.Ragnaros,Card.Cardname.Baron_Geddon,Card.Cardname.Grommash_Hellscream,Card.Cardname.Brawl, Card.Cardname.Shield_Slam, Card.Cardname.Shieldmaiden, Card.Cardname.Shield_Block, Card.Cardname.Revenge };
    List<Card.Cardname> TESTCARDS2 = new List<Card.Cardname>() {Card.Cardname.Ragnaros, Card.Cardname.Baron_Geddon };
    
    
#else
    List<Card.Cardname> TESTCARDS = new List<Card.Cardname>() { };
    List<Card.Cardname> TESTCARDS2 = new List<Card.Cardname>() { };
#endif

    public static CustomMessage CreateMessage(MessageType type)
    {
        CustomMessage m = new CustomMessage();
        m.type = type;
        return m;
    }
    public void SendMessage(CustomMessage message, Player player)
    {
        message.order = player.messageCount++;
        mirror.SendServer(message, player);
    }
    public enum MessageType
    {
        Matchmaking,
        LeaveMatchmaking,

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
        UpdateCard,

        DiscardCard,
        MillCard,

        AddAura,
        AddAuraPlayer,
        RemoveAura,
        RemoveAuraPlayer,

        AddTrigger,
        RemoveTrigger,

        HeroPower,
        ConfirmHeroPower,

        ConfirmBattlecry,
        ConfirmTrigger,

        ConfirmAnimation,
        
        AddCard,
        RemoveMinion,

        EquipWeapon,
        DestroyWeapon,

        AddSecret,
        RemoveSecret,
        TriggerSecret,

        Fatigue,
        TransformMinion,
        StealMinion,

        Concede,

        EndGame,

        StartSequence,
        EndSequence,
        _TEST
    }


    void Start()
    {
        mirror.StartServer();
    }

    private static Server _instance;

    public static Server Instance { get { return _instance; } }


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public void HandleConnection()
    {
        
    }
    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.P))
        {
            Tester();
        }
#endif //TEST HOTKEYS
    }
    void Tester()
    {
    }

    public void OnMessageReceived(CustomMessage message)//public void OnMessageReceived(object sender, MessageReceivedEventArgs eventArgs)
    {
        MessageType messageID = message.type;
        int clientID = message.clientID;
        int count = message.order;
        //CustomMessage message;
        bool orderedMessage = false;
        //UNORDERED MESSAGES, OUT OF GAME (NO MATCH ID ATTACHED)
        switch (messageID)
        {
            case MessageType.Matchmaking:
                ParseMessage(messageID, clientID, message, 0);
                orderedMessage = false;
                break;
            case MessageType.LeaveMatchmaking:
                ParseMessage(messageID, clientID, message, 0);
                orderedMessage = false;
                break;

            case MessageType.SubmitMulligan:
                ParseMessage(messageID, clientID, message, 0);
                orderedMessage = false;
                break;
                
            case MessageType.Concede:
                ParseMessage(messageID, clientID, message, 0);
                orderedMessage = false;
                break;

            default:
                orderedMessage = true;
                break;
        };
        if (orderedMessage)
        {
            ulong matchID = message.GetULong();
            //message = CopyMessage(originalMessage, messageID);
            if (currentMatches.ContainsKey(matchID) == false) return;
            currentMatches[matchID].ReceiveMessage(messageID, clientID, message, count);
        }
    }

    public void ParseMessage(MessageType messageID, int clientID, CustomMessage message, ulong matchID)//(object sender, MessageReceivedEventArgs eventArgs, ulong matchID = 0)
    {
        //int messageID = eventArgs.MessageId;
        //ushort clientID = eventArgs.FromConnection.Id;
       // Message message = eventArgs.Message;

        switch (messageID)
        {
            case MessageType.Matchmaking:
                ulong queuePlayerID = message.GetULong();
                string queuePlayerName = message.GetString();
                List<int> queuePlayerDeck = message.GetInts();
                int queuePlayerClass = message.GetInt();
                int queueClientID = clientID;
                AddToQueue(queueClientID, queuePlayerID, queuePlayerName,queuePlayerDeck, queuePlayerClass);
                break;
            case MessageType.LeaveMatchmaking:
                LeaveQueue(clientID);
                break;
            case MessageType.SubmitMulligan:
                ulong mullMatchID = message.GetULong();
                List<int> mullInds = message.GetInts();
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
                int playChoice = message.GetInt();
                PlayCard(matchID, clientID, playPlayerID, playIndex, playTarget, playPosition, playFriendlySide, playIsHero, playChoice);
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
            case MessageType.SwingMinion:
                ulong swingMinionPlayerID = message.GetULong();
                int swingTargetMinionInd = message.GetInt();
                SwingMinion(matchID, clientID, swingMinionPlayerID, swingTargetMinionInd);
                break;
            case MessageType.SwingFace:
                ulong swingFacePlayerID = message.GetULong();
                SwingFace(matchID, clientID, swingFacePlayerID);
                break;
            case MessageType.HeroPower:
                ulong heroPowerPlayerID = message.GetULong();
                ushort heroPower = message.GetUShort();
                int heroPowerTargetInd = message.GetInt();
                bool heroPowerFriendly = message.GetBool();
                bool heroIsHero = message.GetBool();
                CastHeroPower(matchID,clientID,heroPowerPlayerID,heroPower, heroPowerTargetInd, heroPowerFriendly, heroIsHero);
                break;
            case MessageType.Concede:
                ulong concedeMatchID = message.GetULong();
                ulong concedePlayerID = message.GetULong();
                ConcedeMatch(concedeMatchID, clientID, concedePlayerID);
                break;
        }
        
    }

    [Serializable]
    public struct PlayerConnection
    {
        public int clientID;
        public ulong playerID;
        public Card.Class classType;
        public string name;
        public List<int> deck;
    }

    List<PlayerConnection> playerQueue = new List<PlayerConnection>();
    public Dictionary<ulong, Match> currentMatches = new Dictionary<ulong, Match>();
    public Dictionary<int, Match> clientConnections = new Dictionary<int, Match>();
    public List<Match> matchList = new List<Match>();
    void AddToQueue(int clientID, ulong playerID, string name,List<int> deck, int classType)
    {
        foreach (PlayerConnection p in playerQueue)
        {
            if (p.playerID == playerID || p.clientID == clientID)
                return;
        }
        if (clientConnections.ContainsKey(clientID))
        {
#if UNITY_EDITOR
            Debug.Log($"client {clientID} already in queue");
#endif
            return;
        }
        PlayerConnection pc;
        pc.clientID = clientID;
        pc.playerID = playerID;
        pc.classType = (Card.Class)classType;
        pc.name = name;
        pc.deck = deck;
        Debug.Log($"{name} entered the queue.");
        playerQueue.Add(pc);

        //MatchmakingLogic();
        
        //matchmaking placeholder:
        if (playerQueue.Count>=2)
        {
            PlayerConnection p0 = playerQueue[0];
            PlayerConnection p1 = playerQueue[1];
            playerQueue.Remove(p0);
            playerQueue.Remove(p1);

            StartMatch(p0, p1);
        }
    }
    public ulong currMatchID = 1000;
    
    public void DisconnectClient(int clientID, bool matchmaking=false)
    {
        //if player is in queue
        List<PlayerConnection> removers = new List<PlayerConnection>();
        foreach (PlayerConnection c in playerQueue)
        {
            if (c.clientID == clientID)
                removers.Add(c);
        }
        foreach (PlayerConnection c in removers)
        {
            Debug.Log($"{c.name} left the queue.");
            playerQueue.Remove(c);
        }

        //if player is in game
        if (clientConnections.ContainsKey(clientID) == false)
        {
            return;
        }
        Match match = clientConnections[clientID];

        if (!matchmaking) mirror.connections.Remove(clientID);
        EndMatch(match, match.FindClientID(clientID).opponent);
    }

    public void LeaveQueue(int clientID)
    {
        DisconnectClient(clientID, true);
    }

    public void StartMatch(PlayerConnection p0, PlayerConnection p1)
    {
        Match match = new Match();
        match.server = this;

        CustomMessage m0 = CreateMessage(MessageType.ConfirmMatch);
        m0.AddULong(currMatchID);
        m0.AddString(p1.name);
        m0.AddInt((int)p0.classType);
        m0.AddInt((int)p1.classType);

        CustomMessage m1 = CreateMessage(MessageType.ConfirmMatch);
        m1.AddULong(currMatchID); 
        m1.AddString(p0.name);
        m1.AddInt((int)p1.classType);
        m1.AddInt((int)p0.classType);

        match.InitMatch(p0, p1, currMatchID);
        SendMessage(m0, match.players[0]);
        SendMessage(m1, match.players[1]);

        currentMatches.Add(currMatchID,match);

        Debug.Log("Started match " + currMatchID);
        matchList.Add(match);
        DrawStarterHands(match);
        currMatchID += 1;

        clientConnections.Add(p0.clientID, match);
        clientConnections.Add(p1.clientID, match);
    }
    public void ConcedeMatch(ulong matchID,int clientID, ulong playerID)
    {
        if (currentMatches.ContainsKey(matchID) == false) return;
        Match match = currentMatches[matchID];
        int p = -1;
        if (match.players[0].connection.playerID == playerID)
        {
            p = 0;
        }
        if (match.players[1].connection.playerID==playerID)
        {
            p = 1;
        }
        if (p == -1) return;
        PlayerConnection player = match.players[p].connection;
        PlayerConnection opponent = match.players[match.Opponent(p)].connection;
        if (player.clientID != clientID || player.playerID != playerID) return;

        EndMatch(match, match.players[p].opponent);
    }
    public void EndMatch(Match match, Player winner)
    {
        CustomMessage message0 = CreateMessage(MessageType.EndGame);
        CustomMessage message1 = CreateMessage(MessageType.EndGame);

        if (winner==null)
        {
            message0.AddInt((int)Match.Result.Draw);
            message1.AddInt((int)Match.Result.Draw);
        }
        else if (winner == match.players[0])
        {
            message0.AddInt((int)Match.Result.Win);
            message1.AddInt((int)Match.Result.Lose);
        }
        if (winner== match.players[1])
        {
            message0.AddInt((int)Match.Result.Lose);
            message1.AddInt((int)Match.Result.Win);
        }

        SendMessage(message0, match.players[0]);
        SendMessage(message1, match.players[1]);

        matchList.Remove(match);
        currentMatches.Remove(match.matchID);
        if (winner==null)
        {
            Debug.Log($"{match.matchID}: {match.players[0].connection.name} ties {match.players[1].connection.name}");
        }
        else
        {
            Debug.Log($"{match.matchID}: {winner.connection.name} ({winner.connection.classType}) defeats {winner.opponent.connection.name} ({winner.opponent.connection.classType})"); 
        }
        clientConnections.Remove(match.players[0].connection.clientID);
        clientConnections.Remove(match.players[1].connection.clientID);
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

        foreach (var v in TESTCARDS) m.players[0].hand.Add(v);
        foreach (var v in TESTCARDS2) m.players[1].hand.Add(v);
        if (TESTCARDS.Count>0)
        {
            m.players[0].maxMana = 10;
            m.players[0].currMana = 10;
            m.players[1].maxMana = 10;
            m.players[1].currMana = 10;
        }
        if (m.turn==Turn.player1)
        {
            //TODO: add coin p2
            m.players[1].hand.Add(Card.Cardname.Coin);
        }
        else
        {
            //TODO: add coin p1
            m.players[0].hand.Add(Card.Cardname.Coin);
        }


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
        CustomMessage m1 = CreateMessage(Server.MessageType.DrawHand);
        //string jsonText = JsonUtility.ToJson(m.players[0].hand);
        //m1.AddString(jsonText);
        m1.AddUShorts(hand1);
        m1.AddInt(m.players[1].hand.Count());
        //server.Send(m1, m.players[0].connection.clientID);
        SendMessage(m1, m.players[0]);

        CustomMessage m2 = CreateMessage(Server.MessageType.DrawHand);
        //jsonText = JsonUtility.ToJson(m.players[1].hand);
        //m2.AddString(jsonText);
        m2.AddUShorts(hand2);
        m2.AddInt(m.players[0].hand.Count());
        //server.Send(m2, m.players[1].connection.clientID);
        SendMessage(m2, m.players[1]);
    }

    public void Mulligan(List<int> inds, ulong matchID, ulong playerID)
    {
        if (currentMatches.ContainsKey(matchID) == false) return;
        Match match = currentMatches[matchID];
        Hand hand= match.players[0].hand;
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
            hand[i] = new HandCard(newCard, i);
        }
        match.players[player].deck.AddRange(returningCards);
        match.players[player].deck = Board.Shuffle(match.players[player].deck);
        match.players[player].mulligan = true;

        List<ushort> newhand = new List<ushort>();
        foreach (var c in match.players[player].hand)
        {
            newhand.Add((ushort)c.card);
        }
        CustomMessage message = CreateMessage(Server.MessageType.ConfirmMulligan);
        message.AddUShorts(newhand);
        //server.Send(message, match.players[player].connection.clientID);
        SendMessage(message, match.players[player]);

        CustomMessage enemyMullMessage = CreateMessage(MessageType.EnemyMulligan);
        enemyMullMessage.AddInts(inds);
        //server.Send(enemyMullMessage, match.Opponent(match.players[player]).connection.clientID);
        SendMessage(enemyMullMessage, match.players[player].opponent);

        if (match.players[0].mulligan && match.players[1].mulligan)
        {
            StartGame(match);
        }
    }

    public void StartGame(Match match)
    {

        CustomMessage messageFirst = CreateMessage(Server.MessageType.StartGame);
        CustomMessage messageSecond = CreateMessage(Server.MessageType.StartGame);
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
        CustomMessage message = CreateMessage(MessageType.StartTurn);
        CustomMessage messageEnemy = CreateMessage(MessageType.StartTurn);
        //int p = (int)match.turn;

        match.currPlayer.maxMana = Mathf.Min(match.currPlayer.maxMana + 1, 10);
        match.currPlayer.currMana = match.currPlayer.maxMana;
        foreach (var m in match.currPlayer.board)
        {
            RefreshAttackCharge(m);
        }
        RefreshAttackCharge(match.currPlayer);
        match.currPlayer.comboCounter = 0;
        //TODO: start of turn effects
        message.AddBool(true);
        message.AddInt(match.currPlayer.maxMana);
        message.AddInt(match.currPlayer.currMana);
        message.AddInt(match.messageCount);
        messageEnemy.AddBool(false);
        messageEnemy.AddInt(match.currPlayer.maxMana);
        messageEnemy.AddInt(match.currPlayer.currMana);
        messageEnemy.AddInt(match.messageCount);

        //server.Send(message, match.currPlayer.connection.clientID);
        SendMessage(message, match.currPlayer);
        //server.Send(messageEnemy, match.enemyPlayer.connection.clientID);
        SendMessage(messageEnemy, match.enemyPlayer);

        CastInfo startTurnInfo = new CastInfo(match, match.currPlayer, null, -1, -1, false, false);
        match.StartSequenceStartTurn(startTurnInfo);
        //DrawCard(match, match.currPlayer);
    }
    public void EndTurn(ulong matchID, int clientID, ulong playerID)
    {
        Match m = currentMatches[matchID];
        PlayerConnection player = m.players[(int)m.turn].connection;
        Player Ender = m.players[(int)m.turn];
        if (player.clientID != clientID || player.playerID != playerID)
        {
            return;
        }

        //SEQUENCE
        CastInfo startTurnInfo = new CastInfo(m, m.currPlayer, null, -1, -1, false, false);
        m.StartSequenceEndTurn(startTurnInfo);

        //====LOGIC
        m.turn = m.turn == Turn.player1 ? Turn.player2 : Turn.player1;

        m.currPlayer = m.players[(int)m.turn];
        m.enemyPlayer = m.players[m.Opponent((int)m.turn)];

        CustomMessage msg = CreateMessage(Server.MessageType.EndTurn);
        //server.Send(msg, player.clientID);
        SendMessage(msg, Ender);
        //=========
        StartTurn(m);
    }
    public HandCard DrawCard(Match match, Player player)
    {
        //int p = (int)player;
        List<Card.Cardname> drawnCards = new List<Card.Cardname>();

        Card.Cardname top = player.deck[0];
        player.deck.RemoveAt(0);
        HandCard drawnCard = player.hand.Add(top);
        drawnCards.Add(top);

        CustomMessage message = CreateMessage(Server.MessageType.DrawCards);
        message.AddInt((int)top);
        //server.Send(message, player.connection.clientID);
        SendMessage(message, player);

        CustomMessage messageOpp = CreateMessage(Server.MessageType.DrawEnemy);
        messageOpp.AddInt(1);
        //server.Send(messageOpp, player.opponent.connection.clientID);
        SendMessage(messageOpp, player.opponent);

        return drawnCard;

    }

    public HandCard AddCard(Match match, Player player, Card.Cardname card,Minion source=null, int costChange = 0)
    {
        if (player.hand.Count() >= 10) return new HandCard(card,0);
        HandCard c = player.hand.Add(card);

        CustomMessage messageOwner = CreateMessage(MessageType.AddCard);
        CustomMessage messageOpponent = CreateMessage(MessageType.AddCard);

        messageOwner.AddBool(true);
        messageOpponent.AddBool(false);

        messageOwner.AddInt((int)card);
        messageOpponent.AddInt(0);

        if (source!=null)
        {
            messageOwner.AddBool(source.player == player);
            messageOpponent.AddBool(source.player == player.opponent);

            messageOwner.AddInt(source.index);
            messageOpponent.AddInt(source.index);
        }
        else
        {
            messageOwner.AddBool(false);
            messageOpponent.AddBool(false);

            messageOwner.AddInt(-1);
            messageOpponent.AddInt(-1);
        }

        messageOwner.AddInt(costChange);
        messageOpponent.AddInt(0);

        SendMessage(messageOwner, player);
        SendMessage(messageOpponent, player.opponent);

        return c;
    }
        
    public void PlayCard(ulong matchID, int clientID, ulong playerID, int index, int target, int position, bool friendlySide, bool isHero, int choice)
    {
        if (currentMatches.ContainsKey(matchID) == false) return;
        Match match = currentMatches[matchID];
        int p = (int)match.turn;
        PlayerConnection player = match.players[p].connection;
        PlayerConnection opponent = match.players[match.Opponent(p)].connection;
        if (player.clientID != clientID || player.playerID != playerID) return;

        if (index >= match.players[p].hand.Count()) return;
        HandCard card = match.players[p].hand[index];
        
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
        CustomMessage confirmPlay = CreateMessage(Server.MessageType.PlayCard);
        CustomMessage confirmPlayOpponent = CreateMessage(Server.MessageType.PlayCard);

        confirmPlay.AddBool(true); confirmPlayOpponent.AddBool(false);
        confirmPlay.AddInt(index); confirmPlayOpponent.AddInt(index);
        confirmPlay.AddInt(card.manaCost); confirmPlayOpponent.AddInt(card.manaCost);

        confirmPlay.AddInt((int)card.card);
        if (card.SECRET)
        {
            Database.CardInfo c = Database.GetCardData(card.card);
            int sc = (int)Database.GetClassSecret(c.classType);
            confirmPlayOpponent.AddInt(sc);
        }
        else
        {
            confirmPlayOpponent.AddInt((int)card.card);
        }
        confirmPlay.AddInt(position); confirmPlayOpponent.AddInt(position);

        //server.Send(confirmPlay, player.clientID);
        SendMessage(confirmPlay, match.players[p]);
        //server.Send(confirmPlayOpponent, opponent.clientID);
        SendMessage(confirmPlayOpponent, match.players[p].opponent);

        //summon minion or execute spell effects

        match.playOrder++;

        CastInfo spell = new CastInfo(match, match.players[p], card, target,position, friendlySide, isHero);
        spell.playOrder = match.playOrder;
        spell.choice = choice;

        if (card.SPELL)
        {
            match.StartSequencePlaySpell(spell);
        }

        if (card.MINION)
        {
            match.StartSequencePlayMinion(spell);
        }
        
        if (card.WEAPON)
        {
            match.StartSequencePlayWeapon(spell);
        }

        match.players[p].comboCounter++;
    }
    public Minion SummonMinion(Match match, Player player, Card.Cardname minion,MinionBoard.MinionSource source, int position=-1)
    {
        if (player.board.Count() >= 7) return null;

        Player opponent = match.Opponent(player);
        Minion m = player.board.Add(minion, position,match.playOrder++,player);
        m.player = player;

        CustomMessage message = CreateMessage(Server.MessageType.SummonMinion);
        message.AddBool(true);
        message.AddInt((int)minion);
        message.AddInt(position);
        message.AddInt((int)source);
        SendMessage(message, player);

        CustomMessage messageOp = CreateMessage(Server.MessageType.SummonMinion);
        messageOp.AddBool(false);
        messageOp.AddInt((int)minion);
        messageOp.AddInt(position);
        messageOp.AddInt((int)source);
        SendMessage(messageOp, opponent);

        return m;
    }

    public Minion SummonToken(Match match, Player player, Card.Cardname minion, int position = -1)
    {
        CastInfo summonCast = new CastInfo(match, player, null, -1, position,false,false);
        return match.StartSequenceSummonMinion(summonCast, minion);
    }

    public void AttackMinion(ulong matchID, int clientID, ulong playerID, int attackerInd, int targetInd)
    {
        if (currentMatches.ContainsKey(matchID) == false) return;
        Match match = currentMatches[matchID];

        PlayerConnection player = match.currPlayer.connection;
        PlayerConnection enemy = match.enemyPlayer.connection;
        if (player.clientID != clientID || player.playerID != playerID) return;

        if (attackerInd >= match.currPlayer.board.Count()) return;
        if (targetInd >= match.enemyPlayer.board.Count()) return;

        Minion attacker = match.currPlayer.board[attackerInd];
        Minion target = match.enemyPlayer.board[targetInd];
        if (ValidAttackMinion(match, attackerInd, targetInd) == false) return;

        AttackInfo attackInfo = new AttackInfo(match.currPlayer, attacker, target, false, false, false);
        CastInfo attackAction = new CastInfo(match, attackInfo);

        //preattack confirmation -> start sequence
        ConfirmAttackGeneral(attackAction, true);

        match.StartSequenceAttackMinion(attackAction);

    }
    public void SwingMinion(ulong matchID, int clientID, ulong playerID, int targetInd)
    {
        if (currentMatches.ContainsKey(matchID) == false) return;
        Match match = currentMatches[matchID];

        PlayerConnection player = match.currPlayer.connection;
        PlayerConnection enemy = match.enemyPlayer.connection;
        if (player.clientID != clientID || player.playerID != playerID) return;

        if (targetInd >= match.enemyPlayer.board.Count()) return;

        Minion target = match.enemyPlayer.board[targetInd];
        if (ValidAttackMinion(match, -10, targetInd) == false) return;

        AttackInfo attackInfo = new AttackInfo(match.currPlayer, null, target, true, false, false);
        CastInfo attackAction = new CastInfo(match, attackInfo);

        //preattack confirmation -> start sequence
        ConfirmAttackGeneral(attackAction, true);

        match.StartSequenceSwingMinion(attackAction);
    }

    public void SwingFace(ulong matchID, int clientID, ulong playerID)
    {
        if (currentMatches.ContainsKey(matchID) == false) return;
        Match match = currentMatches[matchID];

        PlayerConnection player = match.currPlayer.connection;
        PlayerConnection enemy = match.enemyPlayer.connection;
        if (player.clientID != clientID || player.playerID != playerID) return;

        if (ValidAttackFace(match, match.currPlayer, match.enemyPlayer,-10) == false) return;

        AttackInfo attackInfo = new AttackInfo(match.currPlayer, null, null, true, true, false);
        CastInfo attackAction = new CastInfo(match, attackInfo);

        //preattack confirmation -> start sequence
        ConfirmAttackGeneral(attackAction, true);

        match.StartSequenceSwingFace(attackAction);
    }

    public void UpdateMinion(Match match, Minion minion, bool damaged = false, bool healed=false)
    {
        CustomMessage messageOwner = CreateMessage(MessageType.UpdateMinion);
        CustomMessage messageOpponent = CreateMessage(MessageType.UpdateMinion);

        string jsonText = JsonUtility.ToJson(minion);

        messageOwner.AddInt(minion.health);
        messageOpponent.AddInt(minion.health);

        messageOwner.AddInt(minion.maxHealth);
        messageOpponent.AddInt(minion.maxHealth);

        messageOwner.AddInt(minion.damage);
        messageOpponent.AddInt(minion.damage);

        messageOwner.AddInt(minion.index);
        messageOpponent.AddInt(minion.index);

        messageOwner.AddBool(true);
        messageOpponent.AddBool(false);

        messageOwner.AddBool(damaged);
        messageOpponent.AddBool(damaged);

        messageOwner.AddBool(healed);
        messageOpponent.AddBool(healed);

        Player owner = match.FindOwner(minion);
        Player opponent = match.FindOpponent(minion);

        //server.Send(messageOwner, owner.clientID);
        SendMessage(messageOwner, owner);
        //server.Send(messageOpponent, opponent.clientID);
        SendMessage(messageOpponent, opponent);
    }
    public void DestroyMinion(Match match, Minion minion)
    {
        minion.DEAD = true;
        Player owner = match.FindOwner(minion);
        Player opponent = match.Opponent(owner);

        int ind = minion.index;

        owner.board.RemoveAt(ind);
        CustomMessage messageOwner = CreateMessage(MessageType.DestroyMinion);
        CustomMessage messageOpponent = CreateMessage(MessageType.DestroyMinion);

        messageOwner.AddInt(ind);
        messageOpponent.AddInt(ind);

        messageOwner.AddBool(true);
        messageOpponent.AddBool(false);

        //server.Send(messageOwner, owner.connection.clientID);
        SendMessage(messageOwner, owner);
        //server.Send(messageOpponent, opponent.connection.clientID);
        SendMessage(messageOpponent, opponent);
    }
    public void RemoveMinion(Match match, Minion minion)
    {
        minion.DEAD = true;
        Player owner = match.FindOwner(minion);
        Player opponent = match.Opponent(owner);

        int ind = minion.index;

        owner.board.RemoveAt(ind);
        CustomMessage messageOwner = CreateMessage(MessageType.RemoveMinion);
        CustomMessage messageOpponent = CreateMessage(MessageType.RemoveMinion);

        messageOwner.AddInt(ind);
        messageOpponent.AddInt(ind);

        messageOwner.AddBool(true);
        messageOpponent.AddBool(false);

        SendMessage(messageOwner, owner);
        SendMessage(messageOpponent, opponent);
    }

    public void SummonWeapon(Match match, Player player, Card.Cardname card)
    {
        CastInfo summonCast = new CastInfo(match, player, null, -1, -1, false, false);
        match.StartSequenceEquipWeapon(summonCast, card);
    }
    public Weapon EquipWeapon(Match match, Player player, Card.Cardname card)
    {
        Weapon weapon = new Weapon(card, player, match.playOrder);

        foreach(Weapon w in player.weaponList)
        {
            w.DEAD = true;
        }
        player.weaponList.Add(weapon);

        CustomMessage messageOwner = CreateMessage(MessageType.EquipWeapon);
        CustomMessage messageOpponent = CreateMessage(MessageType.EquipWeapon);

        messageOwner.AddBool(true);
        messageOpponent.AddBool(false);

        messageOwner.AddInt((int)card);
        messageOpponent.AddInt((int)card);

        SendMessage(messageOwner, player);
        SendMessage(messageOpponent, player.opponent);

        return weapon;
    }
    public void DestroyWeapon(Match match, Weapon weapon)
    {
        Player player = weapon.player;
        player.weaponList.Remove(weapon);

        if (player.weapon != null)
        {
            //the weapon has been replaced with something else already
            return;
        }
        CustomMessage messageOwner = CreateMessage(MessageType.DestroyWeapon);
        CustomMessage messageOpponent = CreateMessage(MessageType.DestroyWeapon);

        messageOwner.AddBool(true);
        messageOpponent.AddBool(false);

        SendMessage(messageOwner, player);
        SendMessage(messageOpponent, player.opponent);
    }

    public void AttackFace(ulong matchID, int clientID, ulong playerID, int attackerInd)
    {
        if (currentMatches.ContainsKey(matchID) == false) return;
        Match match = currentMatches[matchID];

        PlayerConnection player = match.currPlayer.connection;
        PlayerConnection enemy = match.enemyPlayer.connection;
        if (player.clientID != clientID || player.playerID != playerID) return;

        if (attackerInd >= match.currPlayer.board.Count()) return;
        Minion attacker = match.currPlayer.board[attackerInd];
        
        if (ValidAttackFace(match,match.currPlayer,match.enemyPlayer,attackerInd) == false) return;

        AttackInfo attackInfo = new AttackInfo(match.currPlayer, attacker, null, false, true, false);
        CastInfo attackAction = new CastInfo(match, attackInfo);

        //preattack confirmation -> start sequence
        ConfirmAttackGeneral(attackAction, true);

        match.StartSequenceAttackFace(attackAction);
    }

    public void UpdateHero(Match match, Player player, bool damaged=false, bool healed=false)
    {
        CustomMessage messageOwner = CreateMessage(MessageType.UpdateHero);
        CustomMessage messageOpponent = CreateMessage(MessageType.UpdateHero);

        messageOwner.AddInt(player.health);
        messageOpponent.AddInt(player.health);

        messageOwner.AddBool(true);
        messageOpponent.AddBool(false);

        messageOwner.AddInt(player.deck.Count);
        messageOpponent.AddInt(player.deck.Count);

        messageOwner.AddInt(player.currMana);
        messageOwner.AddInt(player.maxMana);

        messageOpponent.AddInt(player.currMana);
        messageOpponent.AddInt(player.maxMana);

        messageOwner.AddInt(player.damage);
        messageOwner.AddInt(player.armor);

        messageOpponent.AddInt(player.damage);
        messageOpponent.AddInt(player.armor);

        if (player.weapon!=null)
        {
            messageOwner.AddInt(player.weapon.damage);
            messageOpponent.AddInt(player.weapon.damage);

            messageOwner.AddInt(player.weapon.durability);
            messageOpponent.AddInt(player.weapon.durability);

        }
        else
        {
            messageOwner.AddInt(-1);
            messageOpponent.AddInt(-1);

            messageOwner.AddInt(-1);
            messageOpponent.AddInt(-1);
        }

        messageOwner.AddInt(player.spellpower);
        messageOpponent.AddInt(player.spellpower);

        messageOwner.AddBool(damaged);
        messageOpponent.AddBool(damaged);

        messageOwner.AddBool(healed);
        messageOpponent.AddBool(healed);

        SendMessage(messageOwner, player);
        SendMessage(messageOpponent, player.opponent);
    }

    public void AddPlayerAura(Player player, Aura a)
    {
        if (a.type != Aura.Type.Freeze && a.type != Aura.Type.Immune) return;
        CustomMessage messageOwner = CreateMessage(MessageType.AddAuraPlayer);
        CustomMessage messageOpponent = CreateMessage(MessageType.AddAuraPlayer);

        messageOwner.AddInt((int)a.type);
        messageOpponent.AddInt((int)a.type);

        messageOwner.AddBool(true);
        messageOpponent.AddBool(false);

        SendMessage(messageOwner, player);
        SendMessage(messageOpponent, player.opponent);

    }
    public void RemovePlayerAura(Player player, Aura a)
    {
        if (a.type != Aura.Type.Freeze && a.type != Aura.Type.Immune) return;
        CustomMessage messageOwner = CreateMessage(MessageType.RemoveAuraPlayer);
        CustomMessage messageOpponent = CreateMessage(MessageType.RemoveAuraPlayer);

        messageOwner.AddInt((int)a.type);
        messageOpponent.AddInt((int)a.type);

        messageOwner.AddBool(true);
        messageOpponent.AddBool(false);

        SendMessage(messageOwner, player);
        SendMessage(messageOpponent, player.opponent);
    }

    public void RemoveTrigger(Match match, Minion minion, Trigger trigger)
    {
        AddTrigger(match, minion, trigger.type, trigger.side, trigger.ability, true);
    }
    public void AddTrigger(Match match, Minion minion,Trigger.Type type, Trigger.Side side, Trigger.Ability ability, bool REMOVE = false)
    {
        Trigger t = new Trigger(type, side, ability, null);
        if (REMOVE)
        {
            minion.RemoveMatchingTrigger(t);
        }
        else
        {
            minion.AddTrigger(type, side, ability);
        }

        CustomMessage messageOwner = CreateMessage(REMOVE ? MessageType.RemoveTrigger : MessageType.AddTrigger);
        CustomMessage messageOpponent = CreateMessage(REMOVE ? MessageType.RemoveTrigger : MessageType.AddTrigger);

        Player owner = match.FindOwner(minion);
        Player opponent = owner.opponent;

        messageOwner.AddInt(minion.index);
        messageOpponent.AddInt(minion.index);

        messageOwner.AddBool(true);
        messageOpponent.AddBool(false);

        messageOwner.AddUShort((ushort)type);
        messageOpponent.AddUShort((ushort)type);

        messageOwner.AddUShort((ushort)side);
        messageOpponent.AddUShort((ushort)side);

        messageOwner.AddUShort((ushort)ability);
        messageOpponent.AddUShort((ushort)ability);

        SendMessage(messageOwner, owner);
        SendMessage(messageOpponent, opponent);
    }
    public void RemoveAura(Match match, Minion minion, Aura aura)
    {
        AddAura(match, minion, aura, true);
    }
    public void AddAura(Match match, Minion minion, Aura aura, bool REMOVE=false)
    {
        if (REMOVE)
        {
            if (minion.RemoveAura(aura) == false) return;
        }
        else minion.AddAura(aura);
        match.auraChanges.Add((minion, aura, REMOVE));
    }

    public void RemoveCardAura(Match match, HandCard card, Aura aura)
    {
        AddCardAura(match, card, aura, true);
    }
    public void AddCardAura(Match match, HandCard card, Aura aura, bool REMOVE=false)
    {
        if (REMOVE)
        {
            if (card.RemoveAura(aura) == false) return;
        }
        else card.AddAura(aura);
    }
    public void ConfirmAuraChange(Match match, Minion minion, Aura aura, bool REMOVE = false)
    {
        if (minion.DEAD) return;
        CustomMessage messageOwner = CreateMessage(REMOVE ? MessageType.RemoveAura : MessageType.AddAura);
        CustomMessage messageOpponent = CreateMessage(REMOVE ? MessageType.RemoveAura : MessageType.AddAura);

        Player owner = match.FindOwner(minion);
        Player opponent = owner.opponent;

        messageOwner.AddInt(minion.index);
        messageOpponent.AddInt(minion.index);

        messageOwner.AddBool(true);
        messageOpponent.AddBool(false);

        messageOwner.AddUShort((ushort)aura.type);
        messageOwner.AddInt(aura.value);
        messageOwner.AddBool(aura.temporary);
        messageOwner.AddBool(aura.foreignSource);

        messageOpponent.AddUShort((ushort)aura.type);
        messageOpponent.AddInt(aura.value);
        messageOpponent.AddBool(aura.temporary);
        messageOpponent.AddBool(aura.foreignSource);

        if (aura.sourceAura == null || aura.minion == null)
        {
            messageOwner.AddInt(0);
            messageOpponent.AddInt(0);
        }
        else
        {
            messageOwner.AddInt((int)aura.minion.card);
            messageOpponent.AddInt((int)aura.minion.card);
        }
        SendMessage(messageOwner, owner);
        SendMessage(messageOpponent, opponent);
    }
    public void UpdateCard(Match match, HandCard card, Player owner)
    {
        CustomMessage message = CreateMessage(MessageType.UpdateCard);

        message.AddInt(card.index);
        message.AddInt(card.manaCost);
        message.AddBool(card.TARGETED);

        SendMessage(message, owner);
    }

    public void TransformMinionMessage(Match match, Minion minion, Card.Cardname newMinion)
    {
        CustomMessage messageOwner = CreateMessage(MessageType.TransformMinion);
        CustomMessage messageOpponent = CreateMessage(MessageType.TransformMinion);

        messageOwner.AddBool(true);
        messageOpponent.AddBool(false);

        messageOwner.AddInt(minion.index);
        messageOpponent.AddInt(minion.index);
        
        messageOwner.AddInt((int)newMinion);
        messageOpponent.AddInt((int)newMinion);


        SendMessage(messageOwner, minion.player);
        SendMessage(messageOpponent, minion.player.opponent);
    }
    public void StealMinionMessage(Match match, Player player, Minion minion, int newInd, bool canAttack)
    {
        CustomMessage messageOwner = CreateMessage(MessageType.StealMinion);
        CustomMessage messageOpponent = CreateMessage(MessageType.StealMinion);

        messageOwner.AddBool(true);
        messageOpponent.AddBool(false);

        messageOwner.AddInt(minion.index);
        messageOpponent.AddInt(minion.index);

        messageOwner.AddInt(newInd);
        messageOpponent.AddInt(newInd);

        messageOwner.AddBool(canAttack);
        messageOpponent.AddBool(canAttack);

        SendMessage(messageOwner, player);
        SendMessage(messageOpponent, player.opponent);
    }

    public void Fatigue(Match match, Player player)
    {
        CustomMessage messageOwner = CreateMessage(MessageType.Fatigue);
        CustomMessage messageOpponent = CreateMessage(MessageType.Fatigue);

        messageOwner.AddBool(true);
        messageOpponent.AddBool(false);

        messageOwner.AddInt(player.fatigue);
        messageOpponent.AddInt(player.fatigue);

        SendMessage(messageOwner, player);
        SendMessage(messageOpponent, player.opponent);
    }
    public void MillCard(Match match, Player player)
    {
        Card.Cardname c = player.deck[0];
        player.deck.RemoveAt(0);
        CustomMessage messageOwner = CreateMessage(MessageType.MillCard);
        CustomMessage messageOpponent = CreateMessage(MessageType.MillCard);

        messageOwner.AddBool(true);
        messageOpponent.AddBool(false);

        messageOwner.AddInt((int)c);
        messageOpponent.AddInt((int)c);

        SendMessage(messageOwner, player);
        SendMessage(messageOpponent, player.opponent);
    }   
    public void DiscardCard(Match match, Player player, int index)
    {
        if (player.hand.Count() == 0 || index>=player.hand.Count()) return;


        CastInfo discardAction = new CastInfo(match, player, player.hand[index], -1, -1, false, false);
        HandCard c = player.hand.RemoveAt(index);
        match.StartSequenceDiscardCard(discardAction);

        CustomMessage messageOwner = CreateMessage(MessageType.DiscardCard);
        CustomMessage messageOpponent = CreateMessage(MessageType.DiscardCard);

        messageOwner.AddBool(true);
        messageOpponent.AddBool(false);

        messageOwner.AddInt(index);
        messageOpponent.AddInt(index);
        
        messageOwner.AddInt((int)c.card);
        messageOpponent.AddInt((int)c.card);

        messageOwner.AddInt(c.manaCost);
        messageOpponent.AddInt(c.manaCost);

        SendMessage(messageOwner, player);
        SendMessage(messageOpponent, player.opponent);
    }


    private void CastHeroPower(ulong matchID, int clientID, ulong playerID, ushort heroPower, int target, bool isFriendly, bool isHero)
    {

        if (currentMatches.ContainsKey(matchID) == false) return;
        Match match = currentMatches[matchID];
        Player player = match.currPlayer;
        PlayerConnection connection = player.connection;
        if (connection.clientID != clientID || connection.playerID != playerID) return;

        Card.Cardname ability = (Card.Cardname)heroPower;
        if (ability != player.heroPower) return;

        HandCard card = new HandCard(ability, 0);

        if (player.currMana < card.manaCost) return;

        player.currMana -= card.manaCost;

        CastInfo spell = new CastInfo(match, player, card, target, -1, isFriendly, isHero);

        match.StartSequenceHeroPower(spell);
        ConfirmHeroPower(spell);
    }

    public void ConfirmBattlecry(Match match, Minion minion, bool trigger = false, bool deathrattle=false, bool isWeapon=false)
    {
        CustomMessage messageOwner = CreateMessage(MessageType.ConfirmBattlecry);
        CustomMessage messageOpponent = CreateMessage(MessageType.ConfirmBattlecry);

        messageOwner.AddBool(true);
        messageOpponent.AddBool(false);

        messageOwner.AddInt(minion.index);
        messageOpponent.AddInt(minion.index);
       
        if (trigger)
        {
            messageOwner.type = messageOpponent.type = MessageType.ConfirmTrigger;
            messageOwner.AddBool(deathrattle);
            messageOpponent.AddBool(deathrattle);
            
            messageOwner.AddBool(isWeapon);
            messageOpponent.AddBool(isWeapon);

        }
        Player player = match.FindOwner(minion);

        SendMessage(messageOwner, player);
        SendMessage(messageOpponent, player.opponent);
    }
    public void ConfirmAnimation(Match match, Player player, AnimationInfo anim)
    {
        CustomMessage messageOwner = CreateMessage(MessageType.ConfirmAnimation);
        CustomMessage messageOpponent = CreateMessage(MessageType.ConfirmAnimation);

        messageOwner.AddBool(true);
        messageOpponent.AddBool(false);

        string jsonText = JsonUtility.ToJson(anim);

        messageOwner.AddString(jsonText);
        messageOpponent.AddString(jsonText);
        
        SendMessage(messageOwner, player);
        SendMessage(messageOpponent, player.opponent);

    }

    public Secret AddSecret(Card.Cardname card, Player player, Match match)
    {
        if (player.secrets.Count >= 5) return null;
        if (player.HasSecret(card)) return null;
        Secret s = player.AddSecret(card, match.playOrder);

        Database.CardInfo info = Database.GetCardData(card);
        CustomMessage messageOwner = CreateMessage(MessageType.AddSecret);
        CustomMessage messageOpponent = CreateMessage(MessageType.AddSecret);

        messageOwner.AddBool(true);
        messageOpponent.AddBool(false);

        messageOwner.AddInt((int)card);
        messageOpponent.AddInt((int)Database.GetClassSecret(info.classType));

        SendMessage(messageOwner, player);
        SendMessage(messageOpponent, player.opponent);
        return s;
    }

    public void TriggerSecret(Secret s)
    {
        RemoveSecret(s, true);
    }
    public void RemoveSecret(Secret secret, bool trigger=false)
    {
        int i = 0;
        foreach(Secret s in secret.player.secrets)
        {
            if (s == secret) break;
            i++;
        }
        Player player = secret.player;

        player.RemoveSecret(secret);

        MessageType msg = trigger ? MessageType.TriggerSecret : MessageType.RemoveSecret;
        CustomMessage messageOwner = CreateMessage(msg);
        CustomMessage messageOpponent = CreateMessage(msg);

        messageOwner.AddBool(true);
        messageOpponent.AddBool(false);

        messageOwner.AddInt(i);
        messageOpponent.AddInt(i);

        messageOwner.AddInt((int)secret.card);
        messageOpponent.AddInt((int)secret.card);

        SendMessage(messageOwner, player);
        SendMessage(messageOpponent, player.opponent);

    }
}
