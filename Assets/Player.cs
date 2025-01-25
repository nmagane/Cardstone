using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Player
{
    public Server.PlayerConnection connection = new Server.PlayerConnection();

    public int health = 30;
    public int maxMana = 0;
    public int currMana = 0;

    public bool turn = false;
    public List<Card.Cardname> deck = new List<Card.Cardname>();
    public Hand hand = new Hand();
    public MinionBoard board = new MinionBoard();

    public bool mulligan = false;

    public Player opponent;

    [System.NonSerialized]
    public Match match;

    public ushort messageCount = 0; //Server messages sent to player 
}
