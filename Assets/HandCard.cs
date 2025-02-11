
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class HandCard
{
    public int index = 0;
    public int _manaCost = 1;
    public int manaCost
    {
        get
        {
            return Mathf.Max(_manaCost, 0);
        }
        set
        {
            _manaCost = value;
        }
    }
    public int health = 3;
    public int damage = 1;

    public int baseCost = 1;

    public Card.Cardname card;
    public Board.EligibleTargets eligibleTargets = Board.EligibleTargets.AllCharacters;

    public bool SPELL = false;
    public bool MINION = false;
    public bool SECRET = false;
    public bool WEAPON = false;

    public bool TARGETED = false;
    public bool COMBO = false;
    public bool BATTLECRY = false;

    public bool played = false;

    public Card cardObject;
    public List<Aura> auras = new List<Aura>();
    public void Set(Card.Cardname name, int ind)
    {
        card = name;
        index = ind;

        if (name == Card.Cardname.Cardback) return;

        Database.CardInfo cardInfo = Database.GetCardData(name);

        manaCost = cardInfo.manaCost;
        baseCost = manaCost;
        health = cardInfo.health;
        damage = cardInfo.damage;

        SPELL = cardInfo.SPELL;
        MINION = cardInfo.MINION;
        SECRET = cardInfo.SECRET;
        WEAPON = cardInfo.WEAPON;

        TARGETED = cardInfo.TARGETED;
        BATTLECRY = cardInfo.BATTLECRY;
        COMBO = cardInfo.COMBO;

        eligibleTargets = cardInfo.eligibleTargets;
    }

    public HandCard(Card.Cardname name, int ind)
    {
        Set(name, ind);
    }
    public override string ToString()
    {
        return card.ToString();
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
            //First time application of foreign aura. Add and considered it refreshed
            a.refreshed = true;
        }

        a.card = this;
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
            if (a.type == x.type && x.foreignSource && a.sourceAura == x.sourceAura && a.name == x.name)//is checking names ok?
                return x;
        }
        return null;
    }


    public bool RemoveAura(Aura a)
    {
        auras.Remove(a);
        switch (a.type)
        {
            case Aura.Type.Cost:
                _manaCost += -a.value;
                break;
        }
        return true;
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
