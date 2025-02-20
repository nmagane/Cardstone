using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Player
{
    public Server.PlayerConnection connection = new Server.PlayerConnection();

    public int health = 30;
    public int maxHealth = 30;

    public int armor = 0;
    public int spellpower = 0;

    bool _canAttack = false;
    public bool canAttack
    {
        get
        {
            return _canAttack && damage > 0 && HasAura(Aura.Type.NoAttack) == false && HasAura(Aura.Type.Freeze) == false;
        }
        set
        {
            _canAttack = value;
            sentinel.canAttack = value;
        }
    }
    public int damage
    {
        get
        {
            if (weapon != null) return sentinel.damage + weapon.damage;
            return sentinel.damage;
        }
        set
        {
            sentinel.damage = value;
        }
    }

    public int maxMana = 0;
    public int currMana = 0;

    public int fatigue = 0;

    public bool combo => comboCounter > 0;

    public int comboCounter = 0;

    public Card.Cardname heroPower = Card.Cardname.Lifetap;
    public int heroPowerCost = 2;

    public Weapon weapon => weaponList.Count == 0? null : weaponList[weaponList.Count-1];
    public List<Weapon> weaponList = new List<Weapon>();
    public bool turn = false;
    public List<Card.Cardname> deck = new List<Card.Cardname>();
    public Hand hand = new Hand();
    public MinionBoard board = new MinionBoard();
    public List<Secret> secrets = new List<Secret>();

    public bool mulligan = false;

    public Player opponent;

    [System.NonSerialized]
    public Match match;

    public int messageCount = 0; //Server messages sent to player 

    public Minion sentinel= new Minion(Card.Cardname.Damaged_Golem,0,null,0);
    public List<Aura> auras => sentinel.auras;
    public List<Trigger> triggers => sentinel.triggers;

    public void ConsumeDurability()
    {
        if (weapon == null) return;
        weapon.durability--;
    }
    //===========================
    public Secret AddSecret(Card.Cardname card, int order)
    {
        if (secrets.Count >= 5) return null;
        Secret secret = new Secret(card, this, order);
        secrets.Add(secret);

        return secret;
    }

    public void RemoveSecret(Secret secret)
    {
        secrets.Remove(secret);
    }

    public bool HasSecret(Card.Cardname card)
    {
        foreach (Secret s in secrets)
            if (s.card == card) return true;
        return false;
    }
    public bool HasTribe(Card.Tribe tribe)
    {
        foreach (HandCard c in hand)
            if (c.tribe == tribe) return true;
        return false;
    }
    //===========================
    public void AddAura(Aura a)
    {
        if (a.type == Aura.Type.Freeze || a.type == Aura.Type.Immune)
            match.server.AddPlayerAura(this, a);

        sentinel.AddAura(a);
    }
    public void RemoveAura(Aura a)
    {
        if (a.type == Aura.Type.Freeze || a.type == Aura.Type.Immune)
            match.server.RemovePlayerAura(this, a);

        sentinel.RemoveAura(a);
    }
    public void RemoveAura(Aura.Type t)
    {
        if (t == Aura.Type.Freeze || t == Aura.Type.Immune)
            match.server.RemovePlayerAura(this, new Aura(t));

        sentinel.RemoveAura(t);
    }
    public void RefreshForeignAuras() => sentinel.RefreshForeignAuras();
    public void RemoveTemporaryAuras() => sentinel.RemoveTemporaryAuras();
    public void RemoveMatchingAura(Aura a) => sentinel.RemoveMatchingAura(a);
    public Aura FindAura(Aura.Type t) => sentinel.FindAura(t);
    public bool HasAura(Aura.Type t) => sentinel.HasAura(t);

    //=============================
    public void AddTrigger(Trigger.Type type, Trigger.Side side, Trigger.Ability ability, int playOrder)
    {
        Trigger t = sentinel.AddTrigger(type, side, ability);
        t.playOrder = playOrder;
    }
    public void RemoveTrigger(Trigger t) => sentinel.RemoveTrigger(t);
    public void RemoveMatchingTrigger(Trigger g) => sentinel.RemoveMatchingTrigger(g);
    public List<Trigger> CheckTriggers(Trigger.Type type, Trigger.Side side, CastInfo spell)
    {
        List<Trigger> trigs = new List<Trigger>();
        foreach (Secret s in secrets)
        {
            trigs.AddRange(s.CheckTriggers(type,side,spell));
        }
        trigs.AddRange(sentinel.CheckTriggers(type, side, spell));
        return trigs;
    }

    public Player()
    {
        sentinel.board = board;
        sentinel.player = this;
        sentinel.damage = 0;
        sentinel.baseDamage = 0;
        sentinel.health = 0;
        sentinel.baseHealth = sentinel.maxHealth = 0;
    }
}
