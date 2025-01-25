using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public partial class Minion
{
    public Card.Cardname card;

    public int index;

    public int damage;
    public int health;
    public int maxHealth;

    public int baseHealth;
    public int baseDamage;

    public bool canAttack = false;

    public bool STEALTH = false;
    public bool WINDFURY = false;

    public bool DEAD = false;

    public List<Aura> auras = new List<Aura>();
    public List<Trigger> triggers = new List<Trigger>();

    public int previewIndex = -1;
    public int playOrder = 0;

    [System.NonSerialized]
    public MinionBoard board;

    public void Set(Card.Cardname name, int ind)
    {
        //transform into another minion
        card = name;
        index = ind;
        //manaCost
    }
    public Minion(Card.Cardname c, int ind, MinionBoard _board)
    {
        board = _board;
        card = c;
        health = 3;
        damage = 1;
        index = ind;

        maxHealth = health;
        baseHealth = maxHealth;
        baseDamage = damage;

        if (c == Card.Cardname.Doomguard)
        {
            AddAura(new Aura(Aura.Type.Charge));
        }
        if (c == Card.Cardname.Amani)
        {
            AddAura(new Aura(Aura.Type.Amani));
        }
        if (c == Card.Cardname.Squire)
        {
            AddAura(new Aura(Aura.Type.Shield));
        }
        if (c == Card.Cardname.ShieldBearer)
        {
            AddAura(new Aura(Aura.Type.Taunt));
        }
        if (c == Card.Cardname.SWChamp)
        {
            AddAura(new Aura(Aura.Type.StormwindChampion));
        }
        if (c == Card.Cardname.DireWolf)
        {
            AddAura(new Aura(Aura.Type.DireWolfAlpha));
        }
        if (c == Card.Cardname.KnifeJuggler)
        {
            AddTrigger(Trigger.Type.AfterSummonMinion, Trigger.Side.Friendly, Trigger.Ability.KnifeJuggler);
            AddTrigger(Trigger.Type.AfterPlayMinion, Trigger.Side.Friendly, Trigger.Ability.KnifeJuggler);
        }
        if (c == Card.Cardname.Acolyte)
        {
            AddTrigger(Trigger.Type.OnMinionDeath, Trigger.Side.Both, Trigger.Ability.AcolyteOfPain);
        }
        if (c == Card.Cardname.YoungPri)
        {
            AddTrigger(Trigger.Type.Deathrattle, Trigger.Side.Both, Trigger.Ability.YoungPriestess);
        }

        if (c == Card.Cardname.HarvestGolem)
        {
            AddTrigger(Trigger.Type.Deathrattle, Trigger.Side.Both, Trigger.Ability.HarvestGolem);
        }
    }
    public override string ToString()
    {
        return card.ToString();
    }

    public List<Trigger> CheckTriggers(Trigger.Type type, Trigger.Side side, CastInfo spell)
    {
        List<Trigger> result = new List<Trigger>();
        foreach (Trigger t in triggers)
        {
            if (t.CheckTrigger(type, side, spell)) result.Add(t);
        }
        return result;
    }
    public void AddTrigger(Trigger.Type type, Trigger.Side side, Trigger.Ability ability)
    {
        Trigger t = new Trigger(type, side, ability, this, playOrder);
        triggers.Add(t);
    }
    public void RemoveTrigger(Trigger t)
    {
        triggers.Remove(t);
    }
    public void RemoveMatchingTrigger(Trigger g)
    {
        List<Trigger> remover = new List<Trigger>();
        foreach (Trigger t in triggers)
        {
            if (t.ability == g.ability && t.side == g.side && t.type == g.type)
            {
                remover.Add(t);
                break;
            }
        }
        foreach (Trigger t in remover)
        {
            RemoveTrigger(t);
        }
    }
    public void AddAura(Aura a)
    {
        Aura finder = FindAura(a.type);
        Debug.Log("starting add aura " + a.type);
        if (finder != null)
        {
            if (finder.stackable == false)
            {
                Debug.Log("cantstack " + a.type);
                return;
            }
        }

        finder = FindForeignAura(a);
        if (finder != null)
        {
            if (finder.foreignSource && finder.source == a.source)
            {
                //Refresh and don't re-add.

                Debug.Log("already on it " + a.type);
                finder.refreshed = true;
                return;
            }
        }

        if (finder == null && a.foreignSource)
        {
            //First time application of foreign aura. Add and considered it refreshed.

            Debug.Log("first time" + a.type);
            a.refreshed = true;
        }

        Debug.Log("adding aura " + a.type);
        a.minion = this;
        a.InitAura();
        auras.Add(a);

    }
    public bool HasAura(Aura.Type t)
    {
        return (FindAura(t) != null);
    }
    public Aura FindAura(Aura.Type t)
    {
        foreach (Aura a in auras)
        {
            if (a.type == t) return a;
        }
        return null;
    }

    public Aura FindForeignAura(Aura a)
    {
        if (a.foreignSource == false) return null;

        foreach (Aura x in auras)
        {
            if (a.type == x.type && x.foreignSource && a.source == x.source)
                return x;
        }
        return null;
    }

    public bool RemoveAura(Aura a)
    {
        auras.Remove(a);
        Debug.Log("removing aura " + a.type);
        switch (a.type)
        {
            case Aura.Type.Health:
                maxHealth -= a.value;
                if (health > maxHealth)
                    health = maxHealth;
                break;
            case Aura.Type.Damage:
                damage -= a.value;
                break;
            case Aura.Type.Taunt:
                if (board.server == false)
                {
                    board.minionObjects[this].DisableTaunt();
                }
                break;
            case Aura.Type.Shield:
                if (board.server == false)
                {
                    board.minionObjects[this].DisableShield();
                }
                break;
        }
        return true;
    }

    public void RemoveAura(Aura.Type t)
    {
        while (FindAura(t) != null)
        {
            RemoveAura(FindAura(t));
        }
    }

    /*
        *             public Type type = Type.Health;
        public bool temporary = false;
        public bool foreignSource = false;
        public bool trigger = false;
        public bool stackable = false;
        public ushort value = 0;
        public Minion minion;
        public Minion source;*/
    public void RemoveMatchingAura(Aura a)
    {
        List<Aura> removers = new List<Aura>();
        foreach (Aura aura in auras)
        {
            if (a.type == aura.type
                && a.temporary == aura.temporary
                && a.value == aura.value
                && a.foreignSource == aura.foreignSource
                && a.source == aura.source)
            {
                removers.Add(aura);
                break;
            }
        }
        foreach (Aura aura in removers)
        {
            RemoveAura(a);
        }
    }

    public void RefreshForeignAuras()
    {
        List<Aura> removeList = new List<Aura>();
        foreach (var aura in auras)
        {
            if (aura.foreignSource && aura.refreshed == false)
                removeList.Add(aura);
            else if (aura.foreignSource && aura.refreshed == true)
            {
                aura.refreshed = false;
            }
        }
        foreach (var aura in removeList)
            RemoveAura(aura);
    }
    public void RemoveTemporaryAuras()
    {
        List<Aura> removeList = new List<Aura>();
        foreach (var aura in auras)
        {
            if (aura.temporary)
                removeList.Add(aura);

        }
        foreach (var aura in removeList)
            RemoveAura(aura);
    }

}
