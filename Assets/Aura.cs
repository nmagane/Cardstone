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
        Immune,
        Spellpower,
        Freeze,

        SP_PLAYER_BUFF,
        SetHealth,
        SetDamage,

        //======== CARD AURAS
        Cost,
        SetCost,
        HoldingDragon,
        HoldingDragonTargeted,
        Molten_Giant,
        Mountain_Giant,
        Sea_Giant,
        Dread_Corsair,

        //========

        Amani,
        StormwindChampion,
        DireWolfAlpha,
        Mana_Wraith,
        Sorcerers_Apprentice,
        Loatheb,
        Preparation,
        Millhouse,
        Southsea_Deckhand,
    }

    public Type type = Type.Health;
    public bool temporary = false;
    public bool foreignSource = false;
    public bool trigger = false;
    public bool stackable = false;
    public int value = 0;
    public Card.Cardname name;
    [System.NonSerialized]
    public Minion minion;

    [System.NonSerialized]
    public HandCard card;

    [System.NonSerialized]
    public Aura sourceAura;

    public bool enrage = false;
    public bool refreshed = false;

    public void InitAura()
    {
        switch (type)
        {
            case Type.Health:
                minion.maxHealth += value;
                if (value>0)
                    minion.health += value;
                else
                {
                    if (minion.health > minion.maxHealth)
                        minion.health = minion.maxHealth;
                }
                break;
            case Type.Damage:
                minion.damage += value;
                break;

            case Type.SP_PLAYER_BUFF:
                if (minion.player!=null)
                    minion.player.spellpower += 1;
                break;

            case Type.NoAttack:
                break;
            case Type.Charge:
                if (minion.SICKNESS)
                {
                    //minion.SICKNESS = false;
                    minion.canAttack = true;
                }
                break;
            case Type.Taunt:
                break;
            case Type.Shield:
                break;

            case Type.Cost:
                card._manaCost += value;
                break;

            case Type.SetCost:
                card._manaCost = value;
                break;

            case Type.Amani: //ENRAGE AURAS GO HERE?
                enrage = true;
                break;
        }
    }

    public void ActivateAura(Match match)
    {
        if (enrage == true && EnrageCheck() == false)
        {
            return;
        }

        switch (type)
        {
            //======CARD AURAS
            case Type.HoldingDragonTargeted:
                AuraEffects.HoldingDragonTargeted(match, card, this);
                break;
            case Type.Mountain_Giant:
                AuraEffects.Mountain_Giant(match, card, this);
                break;
            case Type.Sea_Giant:
                AuraEffects.Sea_Giant(match, card, this);
                break;
            case Type.Molten_Giant:
                AuraEffects.Molten_Giant(match, card, this);
                break;
            case Type.Dread_Corsair:
                AuraEffects.Dread_Corsair(match, card, this);
                break;
            //================

            case Type.Spellpower:
                AuraEffects.Spellpower(match, minion, this);
                break;
            case Type.StormwindChampion:
                AuraEffects.StormwindChampion(match, minion, this);
                break;
            case Type.DireWolfAlpha:
                AuraEffects.DireWolfAlpha(match, minion, this);
                break;
            case Type.Amani:
                AuraEffects.Amani(match, minion, this);
                break;
            case Type.Mana_Wraith:
                AuraEffects.Mana_Wraith(match, minion, this);
                break;
            case Type.Loatheb:
                AuraEffects.Loatheb(match, minion, this);
                break;
            case Type.Preparation:
                AuraEffects.Preparation(match, minion, this);
                break;
            case Type.Millhouse:
                AuraEffects.Millhouse(match, minion, this);
                break;
            case Type.Southsea_Deckhand:
                AuraEffects.Southsea_Deckhand(match, minion, this);
                break;
        }
    }

    bool EnrageCheck()
    {
        return minion.health < minion.maxHealth;
    }

    public Aura(Type t, int val = 0, bool temp = false, bool foreign = false, Aura source=null, Card.Cardname cardname=Card.Cardname.Cardback)
    {
        type = t;
        value = val;
        temporary = temp;
        foreignSource = foreign;
        sourceAura = source;
        name = cardname;

        switch (type)
        {
            case Type.Health:
            case Type.Damage:
            case Type.Cost:
            case Type.SetCost:
            case Type.Loatheb:
            case Type.Spellpower:
            case Type.SP_PLAYER_BUFF:
            case Type.Charge: //should this be stackable? idk
                stackable = true;
                break;
        }
    }
}
