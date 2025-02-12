using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon
{
    public int damage
    {
        get
        {
            return sentinel.damage;
        }
        set
        {
            sentinel.damage = value;
        }
    }
    public int durability
    {
        get 
        {
            return sentinel.health;
        }
        set 
        {
            sentinel.health = value;
        }
    }
    public bool DEAD 
    {
        get 
        {
            return sentinel.DEAD;
        }
        set 
        {
            sentinel.DEAD = value;
        }
    }
    public int playOrder=0;
    public Card.Cardname card => sentinel.card;

    [System.NonSerialized]
    public Player player=null;

    public List<Trigger> triggers => sentinel.triggers;
    public List<Aura> auras => sentinel.auras;

    public Minion sentinel;

    public void Set(Card.Cardname c)
    {
        sentinel = new Minion(c, 0, null, playOrder);
        if (player != null)
        {
            sentinel.board = player.board;
            sentinel.player = player;
        }

    }

    public Weapon(Card.Cardname c, Player p=null, int order=0)
    {
        playOrder = order;
        player = p;
        Set(c);
    }


    public void AddAura(Aura a) => sentinel.AddAura(a);
    public void RemoveAura(Aura a) => sentinel.RemoveAura(a);
    public void RemoveAura(Aura.Type t) => sentinel.RemoveAura(t);
    public void RefreshForeignAuras() => sentinel.RefreshForeignAuras();
    public void RemoveTemporaryAuras() => sentinel.RemoveTemporaryAuras();
    public void RemoveMatchingAura(Aura a) => sentinel.RemoveMatchingAura(a);
    public void FindAura(Aura.Type t) => sentinel.FindAura(t);
    public bool HasAura(Aura.Type t) => sentinel.HasAura(t);

    //=============================
    public void AddTrigger(Trigger.Type type, Trigger.Side side, Trigger.Ability ability)
    {
        Trigger t = sentinel.AddTrigger(type, side, ability);
        t.playOrder = playOrder;
    }
    public void RemoveTrigger(Trigger t) => sentinel.RemoveTrigger(t);
    public void RemoveMatchingTrigger(Trigger g) => sentinel.RemoveMatchingTrigger(g);
    public List<Trigger> CheckTriggers(Trigger.Type type, Trigger.Side side, CastInfo spell) => sentinel.CheckTriggers(type, side, spell);

}
