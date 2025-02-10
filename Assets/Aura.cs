using System;
using System.Collections.Generic;
using UnityEngine;

public class Aura
{
    public enum Type
    {
        Health,
        Damage,
        Charge,
        Taunt,
        Shield,
        Stealth,
        Windfury,
        NoAttack,
        Silence,

        Cost,

        Amani,
        StormwindChampion,
        DireWolfAlpha,
    }

    public Type type = Type.Health;
    public bool temporary = false;
    public bool foreignSource = false;
    public bool trigger = false;
    public bool stackable = false;
    public int value = 0;
    [System.NonSerialized]
    public Minion minion;

    [System.NonSerialized]
    public HandCard card;

    [System.NonSerialized]
    public Minion source;
    public bool enrage = false;
    public bool refreshed = false;

    public void InitAura()
    {
        switch (type)
        {
            case Type.Health:
                minion.health += value;
                minion.maxHealth += value;
                break;
            case Type.Damage:
                minion.damage += value;
                break;
            case Type.NoAttack:
                minion.canAttack = false;
                break;
            case Type.Charge:
                minion.canAttack = true;
                break;
            case Type.Taunt:
                break;
            case Type.Shield:
                break;

            case Type.Cost:
                card.manaCost += value;
                break;

            case Type.Amani: //ENRAGE AURAS GO HERE?
                enrage = true;
                break;
        }
    }

    public void ActivateAura(Match match)
    {
        //Debug.Log("active");
        if (enrage == true && EnrageCheck() == false)
        {
            //Debug.Log("failenrage");
            return;
        }

        switch (type)
        {
            case Type.StormwindChampion:
                AuraEffects.StormwindChampion(match, this.minion);
                break;
            case Type.DireWolfAlpha:
                AuraEffects.DireWolfAlpha(match, this.minion);
                break;
            case Type.Amani:
                AuraEffects.Amani(match, this.minion);
                break;
        }
    }

    bool EnrageCheck()
    {
        return minion.health < minion.maxHealth;
    }

    public Aura(Type t, int val = 0, bool temp = false, bool foreign = false, Minion provider = null)
    {
        type = t;
        value = val;
        temporary = temp;
        foreignSource = foreign;
        source = provider;

        switch (type)
        {
            case Type.Health:
            case Type.Damage:
            case Type.Cost:
                stackable = true;
                break;
        }
    }
}
