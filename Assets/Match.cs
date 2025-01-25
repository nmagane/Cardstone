using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public partial class Match
{
    public List<Player> players = new List<Player>() { new Player(), new Player() };
    public ulong matchID;
    public Server.Turn turn = Server.Turn.player1;
    public Player currPlayer;
    public Player enemyPlayer;
    public Server server;
    public int playOrder = 0;
    public bool started = false;
    public ushort messageCount = 0;
    List<(Server.MessageType, ushort, Server.CustomMessage, ushort)> messageQue = new();
    //todo: secrets
    //todo: graveyards

    public void InitMatch(Server.PlayerConnection p1, Server.PlayerConnection p2, ulong mID)
    {
        players[0].connection = p1;
        players[1].connection = p2;
        players[0].match = this;
        players[1].match = this;
        players[0].opponent = players[1];
        players[1].opponent = players[0];
        List<Card.Cardname> sampleTestDeck = new List<Card.Cardname>();

        for (int i = 0; i < 15; i++)
        {
            sampleTestDeck.Add((Card.Cardname)i);
            sampleTestDeck.Add((Card.Cardname)i);
        }
        //decks = new List<List<Card.Cardname>>();

        players[0].deck = new List<Card.Cardname>(Board.Shuffle(sampleTestDeck));
        players[1].deck = new List<Card.Cardname>(Board.Shuffle(sampleTestDeck));

        turn = Board.RNG(50) ? Server.Turn.player1 : Server.Turn.player2;
        currPlayer = players[(int)turn];
        enemyPlayer = players[Opponent((int)turn)];

        matchID = mID;

        players[0].hand.server = true;
        players[1].hand.server = true;

        players[0].board.server = true;
        players[1].board.server = true;
    }

    public void ReceiveMessage(Server.MessageType messageID, ushort clientID, Server.CustomMessage message, ushort count)
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

        for (int i = 0; i < messageQue.Count; i++)
        {
            var v = messageQue[i];
            if (v.Item4 == messageCount)
            {
                Debug.Log("executing message " + messageCount);
                ReceiveMessage(v.Item1, v.Item2, v.Item3, v.Item4);
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
    public Player FindOwner(Minion minion)
    {
        if (players[0].board.Contains(minion))
            return players[0];
        if (players[1].board.Contains(minion))
            return players[1];
        return players[0];
    }
    public Player FindOpponent(Minion minion)
    {
        return Opponent(FindOwner(minion));
    }
}