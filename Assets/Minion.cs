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

    bool _canAttack = false;
    public bool canAttack
    {
        get
        {
            return _canAttack && damage>0 && HasAura(Aura.Type.NoAttack)==false;
        }
        set
        {
            //TODO: windfury here maybe?
            _canAttack = value;
        }
    }

    public bool TAUNT => HasAura(Aura.Type.Taunt) && !STEALTH;
    public bool STEALTH => HasAura(Aura.Type.Stealth);

    public bool DEAD = false;

    public List<Aura> auras = new List<Aura>();
    public List<Trigger> triggers = new List<Trigger>();

    [System.NonSerialized]
    public int previewIndex = -1;

    public int playOrder = 0;

    [System.NonSerialized]
    public Player player=null;

    [System.NonSerialized]
    public MinionBoard board;

    [System.NonSerialized]
    public Creature creature;

    public void Set(Card.Cardname name, int ind)
    {
        //transform into another minion
        card = name;
        index = ind;
        //manaCost
    }
    public Minion(Card.Cardname c, int ind, MinionBoard _board, int order=0, Player p=null)
    {
        board = _board;
        card = c;
        index = ind;
        playOrder = order;
        player = p;

        Database.CardInfo info = Database.GetCardData(c);
        health = info.health;
        damage = info.damage;

        foreach (Aura.Type aura in info.auras)
        {
            AddAura(new Aura(aura));
        }

        foreach (var triggerInfo in info.triggers)
        {
            AddTrigger(triggerInfo.Item1, triggerInfo.Item2, triggerInfo.Item3);
        }

        maxHealth = health;
        baseHealth = maxHealth;
        baseDamage = damage;

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
    public Trigger AddTrigger(Trigger.Type type, Trigger.Side side, Trigger.Ability ability)
    {
        Trigger t = new Trigger(type, side, ability, this, playOrder);
        triggers.Add(t);
        return t;
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
        if (finder != null)
        {
            if (finder.stackable == false)
            {
                return;
            }
        }

        finder = FindForeignAura(a);
        if (finder != null)
        {
            if (finder.foreignSource && finder.sourceAura == a.sourceAura)
            {
                //Refresh and don't re-add.
                finder.refreshed = true;
                return;
            }
        }

        if (finder == null && a.foreignSource)
        {
            //First time application of foreign aura. Add and considered it refreshed.
            a.refreshed = true;
        }

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
        Aura result = null;
        foreach (Aura a in auras)
        {
            if (a.type == t) result = a;
        }
        return result;
    }

    public Aura FindForeignAura(Aura a)
    {
        if (a.foreignSource == false) return null;

        foreach (Aura x in auras)
        {
            if (a.type == x.type && x.foreignSource && a.sourceAura == x.sourceAura && a.name==x.name)//is checking names ok?
                return x;
        }
        return null;
    }

    public bool RemoveAura(Aura a)
    {
        auras.Remove(a);
        switch (a.type)
        {
            case Aura.Type.Health:
                maxHealth -= a.value;
                if (a.value > 0)
                {
                    if (health > maxHealth)
                        health = maxHealth;
                }
                else
                {
                    health -= a.value;
                }
                break;
            case Aura.Type.Damage:
                damage -= a.value;
                break;
            case Aura.Type.SP_PLAYER_BUFF:
                if (player != null)
                    player.spellpower -= 1;
                break;
            case Aura.Type.Taunt:
                break;
            case Aura.Type.Shield:
                break;
        }
        return true;
    }

    void RecalculateAuras()
    {
        damage = baseDamage;
        maxHealth = baseHealth;
        foreach (Aura a in auras)
        {
            switch (a.type)
            {
                case Aura.Type.Health:
                    break;
                case Aura.Type.Damage:
                    damage += a.value;
                    break;
            }
        }
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
                && a.name == aura.name)
            {
                removers.Add(aura);
                break;
            }
        }

        foreach (Aura aura in removers)
        {
            RemoveAura(aura);
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
    public void RemoveTemporaryAuras(Aura.Type t)
    {
        List<Aura> removeList = new List<Aura>();
        foreach (var aura in auras)
        {
            if (aura.temporary && aura.type==t)
                removeList.Add(aura);

        }
        foreach (var aura in removeList)
            RemoveAura(aura);
    }

}
