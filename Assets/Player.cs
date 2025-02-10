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

    public Card.Cardname heroPower = Card.Cardname.Lifetap;
    public int heroPowerCost = 2;
    public int fatigue = 0;

    public bool turn = false;
    public List<Card.Cardname> deck = new List<Card.Cardname>();
    public Hand hand = new Hand();
    public MinionBoard board = new MinionBoard();

    public bool mulligan = false;

    public Player opponent;

    [System.NonSerialized]
    public Match match;

    public ushort messageCount = 0; //Server messages sent to player 

    Minion sentinel= new Minion(Card.Cardname.Shieldbearer,0,null,0);
    public List<Aura> auras => sentinel.auras;
    public List<Trigger> triggers => sentinel.triggers;

    public void AddAura(Aura a) => sentinel.AddAura(a);
    public void RemoveAura(Aura a) => sentinel.RemoveAura(a);
    public void RemoveAura(Aura.Type t) => sentinel.RemoveAura(t);
    public void RefreshForeignAuras() => sentinel.RefreshForeignAuras();
    public void RemoveTemporaryAuras() => sentinel.RemoveTemporaryAuras();
    public void RemoveMatchingAura(Aura a) => sentinel.RemoveMatchingAura(a);
    public void FindAura(Aura.Type t) => sentinel.FindAura(t);
    public bool HasAura(Aura.Type t) => sentinel.HasAura(t);

    public Player()
    {
        sentinel.board = board;
        sentinel.player = this;
    }
}
