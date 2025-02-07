using System;
using System.Collections.Generic;
using Riptide;
using Riptide.Utils;
using UnityEngine;

public partial class Server : MonoBehaviour
{
    public NetworkHandler mirror;
    List<Card.Cardname> TESTCARDS = new List<Card.Cardname>() { Card.Cardname.Knife_Juggler, Card.Cardname.Flame_Imp, Card.Cardname.Soulfire};
    List<Card.Cardname> TESTCARDS2 = new List<Card.Cardname>() { Card.Cardname.Argent_Squire};

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

        AddAura,
        RemoveAura,

        AddTrigger,
        RemoveTrigger,

        HeroPower,
        ConfirmHeroPower,

        ConfirmBattlecry,
        ConfirmTrigger,

        ConfirmAnimation,
        
        AddCard,

        Concede,

        EndGame,

        StartSequence,
        EndSequence,
        _TEST
    }
    public Riptide.Server server = new Riptide.Server();

    void Start()
    {
        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);
        //ushort port = 8888;
        //ushort maxPlayers = 65534; //ushort.max-1
        //server.Start(port, maxPlayers,0,false);
        mirror.StartServer();
        //server.MessageReceived += OnMessageReceived;
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
        CustomMessage m = CreateMessage(MessageType._TEST);
        //m.ReserveBits(16);
        m.AddInt(8885444);
        m.AddBool(false);
        //ushort messageOrder = 1;
        //m.SetBits(messageOrder, 16, 28);
        
        //server.Send(m, matchList[0].players[0].connection.clientID);
        //SendMessage(m, matchList[0].players[0]);
    }
    private void FixedUpdate()
    {
        server.Update();
    }

    public void OnMessageReceived(CustomMessage message)//public void OnMessageReceived(object sender, MessageReceivedEventArgs eventArgs)
    {
        MessageType messageID = message.type;
        int clientID = message.clientID;
        ushort count = message.order;
        //CustomMessage message;
        bool orderedMessage = false;
        //UNORDERED MESSAGES, OUT OF GAME (NO MATCH ID ATTACHED)
        switch (messageID)
        {
            case MessageType.Matchmaking:
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
                int queueClientID = clientID;
                AddToQueue(queueClientID, queuePlayerID);
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
        public string deck;
    }

    List<PlayerConnection> playerQueue = new List<PlayerConnection>();
    public Dictionary<ulong, Match> currentMatches = new Dictionary<ulong, Match>();
    public List<Match> matchList = new List<Match>();
    void AddToQueue(int clientID, ulong playerID, string deck="")
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
            playerQueue.RemoveAt(0);
            playerQueue.RemoveAt(0);
        }
    }
    public ulong currMatchID = 1000;
    public void StartMatch(PlayerConnection p1, PlayerConnection p2)
    {
        Match match = new Match();
        match.server = this;

        CustomMessage m1 = CreateMessage(MessageType.ConfirmMatch);
        m1.AddULong(currMatchID);
        //server.Send(m1,p1.clientID);

        CustomMessage m2 = CreateMessage(MessageType.ConfirmMatch);
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
        Debug.Log(p);
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
            m.canAttack = true;
        }
        //TODO: start of turn effects
        message.AddBool(true);
        message.AddInt(match.currPlayer.maxMana);
        message.AddInt(match.currPlayer.currMana);
        message.AddUShort(match.messageCount);
        messageEnemy.AddBool(false);
        messageEnemy.AddInt(match.currPlayer.maxMana);
        messageEnemy.AddInt(match.currPlayer.currMana);
        messageEnemy.AddUShort(match.messageCount);

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
        
    public void PlayCard(ulong matchID, int clientID, ulong playerID, int index, int target, int position, bool friendlySide, bool isHero)
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
        confirmPlay.AddInt((int)card.card); confirmPlayOpponent.AddInt((int)card.card);
        confirmPlay.AddInt(position); confirmPlayOpponent.AddInt(position);

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
    public Minion SummonMinion(Match match, Player player, Card.Cardname minion,MinionBoard.MinionSource source, int position=-1)
    {
        if (player.board.Count() >= 7) return null;

        Player opponent = match.Opponent(player);
        Minion m = player.board.Add(minion, position,match.playOrder);
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

    public void SummonToken(Match match, Player player, Card.Cardname minion, int position = -1)
    {
        CastInfo summonCast = new CastInfo(match, player, null, -1, position,false,false);
        match.StartSequenceSummonMinion(summonCast, minion);
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

    public void UpdateMinion(Match match, Minion minion, bool damaged = false, bool healed=false)
    {
        CustomMessage messageOwner = CreateMessage(MessageType.UpdateMinion);
        CustomMessage messageOpponent = CreateMessage(MessageType.UpdateMinion);

        string jsonText = JsonUtility.ToJson(minion);
        //Debug.Log(jsonText);
        messageOwner.AddString(jsonText);
        messageOpponent.AddString(jsonText);

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
        
        //TODO: ON MINION DEATH TRIGGERS
        //TODO: DEATHRATTLE TRIGGERS
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

        messageOwner.AddBool(damaged);
        messageOpponent.AddBool(damaged);

        messageOwner.AddBool(healed);
        messageOpponent.AddBool(healed);

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
    public void ConfirmAuraChange(Match match, Minion minion, Aura aura, bool REMOVE = false)
    {

        CustomMessage messageOwner = CreateMessage(REMOVE ? MessageType.RemoveAura : MessageType.AddAura);
        CustomMessage messageOpponent = CreateMessage(REMOVE ? MessageType.RemoveAura : MessageType.AddAura);

        Player owner = match.FindOwner(minion);
        Player opponent = owner.opponent;

        messageOwner.AddInt(minion.index);
        messageOpponent.AddInt(minion.index);

        messageOwner.AddBool(true);
        messageOpponent.AddBool(false);

        messageOwner.AddUShort((ushort)aura.type);
        messageOwner.AddUShort(aura.value);
        messageOwner.AddBool(aura.temporary);
        messageOwner.AddBool(aura.foreignSource);

        messageOpponent.AddUShort((ushort)aura.type);
        messageOpponent.AddUShort(aura.value);
        messageOpponent.AddBool(aura.temporary);
        messageOpponent.AddBool(aura.foreignSource);

        if (aura.source == null)
        {
            messageOwner.AddInt(-1);
            messageOwner.AddBool(false);

            messageOpponent.AddInt(-1);
            messageOpponent.AddBool(false);
        }
        else
        {
            messageOwner.AddInt(aura.source.index);
            messageOwner.AddBool(owner.board.Contains(aura.source));

            messageOpponent.AddInt(aura.source.index);
            messageOpponent.AddBool(opponent.board.Contains(aura.source));
        }
        SendMessage(messageOwner, owner);
        SendMessage(messageOpponent, opponent);
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
        player.hand.RemoveAt(index);
        match.StartSequenceDiscardCard(discardAction);

        CustomMessage messageOwner = CreateMessage(MessageType.DiscardCard);
        CustomMessage messageOpponent = CreateMessage(MessageType.DiscardCard);

        messageOwner.AddBool(true);
        messageOpponent.AddBool(false);

        messageOwner.AddInt(index);
        messageOpponent.AddInt(index);

        messageOwner.AddInt((int)discardAction.card.card);
        messageOpponent.AddInt((int)discardAction.card.card);

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
        HandCard card = new HandCard(ability, 0);

        if (player.currMana < card.manaCost) return;

        player.currMana -= card.manaCost;

        CastInfo spell = new CastInfo(match, player, card, target, -1, isFriendly, isHero);

        match.StartSequenceHeroPower(spell);
        ConfirmHeroPower(spell);
    }

    public void ConfirmBattlecry(Match match, Minion minion, bool trigger = false, bool deathrattle=false)
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
        }
        Player player = match.FindOwner(minion);

        SendMessage(messageOwner, player);
        SendMessage(messageOpponent, player.opponent);
    }
    public void ConfirmAnimation(Match match, Player player, AnimationManager.AnimationInfo anim)
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
    /*
    public static CustomMessage CopyMessage(Message message, MessageType type)
    {
        CustomMessage result = new CustomMessage();
        result.type = type;

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
            case MessageType.HeroPower:
                ulong heroPowerPlayerID = message.GetULong();
                ushort heroPower = message.GetUShort();
                int heroPowerTargetInd = message.GetInt();
                bool heroPowerFriendly = message.GetBool();
                bool heroIsHero = message.GetBool();
                result.AddULong(heroPowerPlayerID);
                result.AddUShort(heroPower);
                result.AddInt(heroPowerTargetInd);
                result.AddBool(heroPowerFriendly);
                result.AddBool(heroIsHero);
                break;
            default:
                Debug.LogError("MESSAGE TYPE UNRECOGNIZED");
                break;

        }
        return result;
    }
    */
}
