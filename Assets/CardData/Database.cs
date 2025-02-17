using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public partial class Database
{
    public class CardInfo
    {
        public string name="UNDEFINED CARD";
        public string text="UNDEFINED CARD";

        public Card.Class classType = Card.Class.Neutral;
        public Card.Tribe tribe = Card.Tribe.None;

        public int manaCost=1;
        public int damage=1;
        public int health=1;
        public int spellDamage = 0;
        public int comboSpellDamage = 0;

        public bool SPELL = false;
        public bool MINION = false;
        public bool SECRET = false;
        public bool WEAPON = false;

        public bool BATTLECRY = false;
        public bool TARGETED = false;
        public bool COMBO = false;
        public bool COMBO_TARGETED = false; //for cards that are only targeted when combo is active (SI7)
        public bool CHOOSE = false;

        public Board.EligibleTargets eligibleTargets = Board.EligibleTargets.AllCharacters;

        public List<Aura.Type> auras = new List<Aura.Type>();
        public List<Aura.Type> cardAuras = new List<Aura.Type>();
        public List<(Trigger.Type, Trigger.Side, Trigger.Ability)> triggers = new List<(Trigger.Type, Trigger.Side, Trigger.Ability)>();
        public CardInfo()
        {

        }
    }
    public static CardInfo GetCardData(Card.Cardname card)
    {

        CardInfo info = new CardInfo();
        switch (card)
        {
            case Card.Cardname.Coin:
                return Coin();
            case Card.Cardname.Argent_Squire:
                return Argent_Squire();

            case Card.Cardname.Knife_Juggler:
                return Knife_Juggler();

            case Card.Cardname.Amani_Berserker:
                return Amani_Berserker();

            case Card.Cardname.Harvest_Golem:
                return Harvest_Golem();

            case Card.Cardname.Damaged_Golem:
                return Damaged_Golem();

            case Card.Cardname.Young_Priestess:
                return Young_Priestess();

            case Card.Cardname.Shieldbearer:
                return Shieldbearer();

            case Card.Cardname.Defender_of_Argus:
                return Defender_of_Argus();

            case Card.Cardname.Dire_Wolf_Alpha:
                return Dire_Wolf_Alpha();

            case Card.Cardname.Shattered_Sun_Cleric:
                return Shattered_Sun_Cleric();

            case Card.Cardname.Abusive_Sergeant:
                return Abusive_Sergeant();

            case Card.Cardname.Dark_Iron_Dwarf:
                return Dark_Iron_Dwarf();

            case Card.Cardname.Ironbeak_Owl:
                return Ironbeak_Owl();

            case Card.Cardname.Flame_Imp:
                return Flame_Imp();

            case Card.Cardname.Voidwalker:
                return Voidwalker();

            case Card.Cardname.Soulfire:
                return Soulfire();

            case Card.Cardname.Doomguard:
                return Doomguard();

            case Card.Cardname.Lifetap:
                return Lifetap();

            case Card.Cardname.Ping:
                return Ping();

            case Card.Cardname.Emperor_Thaurissan:
                return Emperor_Thaurissan();

            case Card.Cardname.Mana_Wraith:
                return Mana_Wraith();
                
            case Card.Cardname.Loatheb:
                return Loatheb();

            case Card.Cardname.Stormwind_Champion:
                return Stormwind_Champion();

            case Card.Cardname.Preparation:
                return Preparation();

            case Card.Cardname.Millhouse_Manastorm:
                return Millhouse_Manastorm();

            case Card.Cardname.Hunters_Mark:
                return Hunters_Mark();

            case Card.Cardname.Crazed_Alchemist:
                return Crazed_Alchemist();

            case Card.Cardname.Chillwind_Yeti:
                return Chillwind_Yeti();

            case Card.Cardname.Dagger:
                return Dagger();

            case Card.Cardname.Heroic_Strike:
                return Heroic_Strike();
                
            case Card.Cardname.Deadly_Poison:
                return Deadly_Poison();

            case Card.Cardname.Blade_Flurry:
                return Blade_Flurry();

            case Card.Cardname.Armor_Up:
                return Armor_Up();

            case Card.Cardname.SI7_Agent:
                return SI7_Agent();
                
            case Card.Cardname.Eviscerate:
                return Eviscerate();

            case Card.Cardname.Archmage_Antonidas:
                return Archmage_Antonidas();

            case Card.Cardname.Sap:
                return Sap();

            case Card.Cardname.Ogre_Magi:
                return Ogre_Magi();
                
            case Card.Cardname.Ice_Barrier:
                return Ice_Barrier();

            case Card.Cardname.Mage_Secret:
                return Mage_Secret();

            case Card.Cardname.Hunter_Secret:
                return Hunter_Secret();

            case Card.Cardname.Paladin_Secret:
                return Paladin_Secret();

            case Card.Cardname.Frostbolt:
                return Frostbolt();

            case Card.Cardname.Southsea_Deckhand:
                return Southsea_Deckhand();

            case Card.Cardname.Warsong_Commander:
                return Warsong_Commander();

            case Card.Cardname.Grim_Patron:
                return Grim_Patron();

            case Card.Cardname.Mountain_Giant:
                return Mountain_Giant();
            case Card.Cardname.Sea_Giant:
                return Sea_Giant();
            case Card.Cardname.Molten_Giant:
                return Molten_Giant();

            case Card.Cardname.Dagger_Mastery:
                return Dagger_Mastery();
                
            case Card.Cardname.Backstab:
                return Backstab();
                
            case Card.Cardname.Bloodmage_Thalnos:
                return Bloodmage_Thalnos();

            case Card.Cardname.Fan_of_Knives:
                return Fan_of_Knives();

            case Card.Cardname.Earthen_Ring_Farseer:
                return Earthen_Ring_Farseer();

            case Card.Cardname.Tinkers_Oil:
                return Tinkers_Oil();
                
            case Card.Cardname.Violet_Teacher:
                return Violet_Teacher();
           case Card.Cardname.Violet_Apprentice:
                return Violet_Apprentice();

            default:
                Debug.LogError("ERROR: UNDEFINED CARD: " + card);
                break;
        }
        return info;
    }

    public static Card.Cardname GetClassSecret(Card.Class c)
    {
        switch(c)
        {
            case Card.Class.Mage:
                return Card.Cardname.Mage_Secret;

            case Card.Class.Hunter:
                return Card.Cardname.Hunter_Secret;

            case Card.Class.Paladin:
                return Card.Cardname.Paladin_Secret;
            /*
        case Card.Class.Warrior:
            break;
        case Card.Class.Rogue:
            break;
        case Card.Class.Warlock:
            break;
        case Card.Class.Priest:
            break;
        case Card.Class.Druid:
            break;
        case Card.Class.Shaman:
            break;
        case Card.Class.Neutral:*/
            default:
                Debug.LogError("CLASS SECRET UNKNOWN");
                return Card.Cardname.Cardback;
        }
    }
    public static Card.Cardname GetClassHeroPower(Card.Class c)
    {
        switch(c)
        {
            case Card.Class.Mage:
                return Card.Cardname.Ping;

            case Card.Class.Warrior:
                return Card.Cardname.Armor_Up;

            case Card.Class.Rogue:
                return Card.Cardname.Dagger_Mastery;

            case Card.Class.Warlock:
                return Card.Cardname.Lifetap;

            /*
           case Card.Class.Hunter:
               break;
           case Card.Class.Paladin:
               break;
           case Card.Class.Priest:
               break;
           case Card.Class.Druid:
               break;
           case Card.Class.Shaman:
               break;
           case Card.Class.Neutral:*/
            default:
                Debug.LogError("CLASS SECRET UNKNOWN");
                return Card.Cardname.Cardback;
        }
    }

    static CardInfo Coin()
    { 
        CardInfo c = new();

        c.name = "The Coin";
        c.text = "Gain 1 mana this turn only.";
        c.manaCost = 0;
        c.SPELL = true;
        c.TARGETED = false;

        return c;
    }
    static CardInfo Ping()
    { 
        CardInfo c = new();

        c.name = "Ping";
        c.text = "Deal 1 damage.";
        c.manaCost = 2;
        c.spellDamage = 1;
        c.SPELL = true;
        c.TARGETED = true;
        c.eligibleTargets = Board.EligibleTargets.AllCharacters;

        return c;
    }
    static CardInfo Argent_Squire()
    { 
        CardInfo c = new();

        c.name = "Argent Squire";
        c.text = "Divine Shield.";

        c.manaCost = 1;
        c.damage = 1;
        c.health = 1;

        c.MINION = true;
        c.auras.Add(Aura.Type.Shield);

        return c;
    }
    static CardInfo Shieldbearer()
    { 
        CardInfo c = new();

        c.name = "Shieldbearer";
        c.text = "Taunt.";

        c.manaCost = 1;
        c.damage = 0;
        c.health = 4;

        c.MINION = true;
        c.auras.Add(Aura.Type.Taunt);

        return c;
    }
    static CardInfo Amani_Berserker()
    { 
        CardInfo c = new();

        c.name = "Amani Berserker";
        c.text = "Enrage: +3 Attack.";

        c.manaCost = 2;
        c.damage = 2;
        c.health = 3;

        c.MINION = true;
        c.auras.Add(Aura.Type.Amani);

        return c;
    }
    static CardInfo Dire_Wolf_Alpha()
    { 
        CardInfo c = new();

        c.name = "Dire Wolf Alpha";
        c.text = "Adjacent minions have +1 attack.";

        c.manaCost = 2;
        c.damage = 2;
        c.health = 2;

        c.MINION = true;
        c.auras.Add(Aura.Type.DireWolfAlpha);

        return c;
    } 
    static CardInfo Abusive_Sergeant()
    { 
        CardInfo c = new();

        c.name = "Abusive Sergeant";
        c.text = "Battlecry: Give a minion +2 attack this turn.";

        c.manaCost = 1;
        c.damage = 2;
        c.health = 1;

        c.MINION = true;
        c.BATTLECRY = true;
        c.TARGETED = true;

        c.eligibleTargets = Board.EligibleTargets.AllMinions;

        return c;
    }
    static CardInfo Ironbeak_Owl()
    { 
        CardInfo c = new();

        c.name = "Ironbeak Owl";
        c.text = "Battlecry: Silence a minion.";

        c.manaCost = 2;
        c.damage = 2;
        c.health = 1;

        c.MINION = true;
        c.BATTLECRY = true;
        c.TARGETED = true;
        c.eligibleTargets = Board.EligibleTargets.AllMinions;

        return c;
    }
    static CardInfo Dark_Iron_Dwarf()
    { 
        CardInfo c = new();

        c.name = "Dark Iron Dwarf";
        c.text = "Battlecry: Give a minion +2 attack this turn.";

        c.manaCost = 4;
        c.damage = 4;
        c.health = 4;

        c.MINION = true;
        c.BATTLECRY = true;
        c.TARGETED = true;
        c.eligibleTargets = Board.EligibleTargets.AllMinions;

        return c;
    }
    static CardInfo Shattered_Sun_Cleric()
    { 
        CardInfo c = new();

        c.name = "Shattered Sun Cleric";
        c.text = "Battlecry: Give a friendly minion +1/+1.";

        c.manaCost = 3;
        c.damage = 3;
        c.health = 2;

        c.MINION = true;
        c.BATTLECRY = true;
        c.TARGETED = true;
        c.eligibleTargets = Board.EligibleTargets.FriendlyMinions;

        return c;
    }
    
    static CardInfo Defender_of_Argus()
    { 
        CardInfo c = new();

        c.name = "Defender of Argus";
        c.text = "Battlecry: Give adjacent minions +1/+1 and Taunt.";

        c.manaCost = 4;
        c.damage = 2;
        c.health = 3;

        c.MINION = true;
        c.BATTLECRY = true;
        c.TARGETED = false;

        return c;
    }
    static CardInfo Knife_Juggler()
    { 
        CardInfo c = new();

        c.name = "Knife Juggler";
        c.text = "After you summon a minion, deal 1 damage to a random enemy.";

        c.manaCost = 2;
        c.damage = 3;
        c.health = 2;

        c.MINION = true;

        c.triggers.Add((Trigger.Type.AfterPlayMinion, Trigger.Side.Friendly, Trigger.Ability.KnifeJuggler));
        c.triggers.Add((Trigger.Type.AfterSummonMinion, Trigger.Side.Friendly, Trigger.Ability.KnifeJuggler));

        return c;
    }
    static CardInfo Young_Priestess()
    { 
        CardInfo c = new();

        c.name = "Young Priestess";
        c.text = "End of Turn: Give another random friendly minion +1 health.";

        c.manaCost = 1;
        c.damage = 2;
        c.health = 1;

        c.MINION = true;

        c.triggers.Add((Trigger.Type.EndTurn, Trigger.Side.Friendly, Trigger.Ability.YoungPriestess));

        return c;
    }
    static CardInfo Harvest_Golem()
    { 
        CardInfo c = new();

        c.name = "Harvest Golem";
        c.text = "Deathrattle: Summon a 2/1 Damaged Golem.";

        c.manaCost = 3;
        c.damage = 2;
        c.health = 3;

        c.MINION = true;

        c.triggers.Add((Trigger.Type.Deathrattle, Trigger.Side.Friendly, Trigger.Ability.HarvestGolem));

        return c;
    }
    static CardInfo Damaged_Golem()
    { 
        CardInfo c = new();

        c.name = "Damaged Golem";
        c.text = "";

        c.manaCost = 1;
        c.damage = 2;
        c.health = 1;

        c.MINION = true;

        return c;
    }
    
    static CardInfo Emperor_Thaurissan()
    { 
        CardInfo c = new();

        c.name = "Emperor Thaurissan";
        c.text = "End of Turn: Reduce the cost of cards in your hand by 1.";

        c.manaCost = 1;
        c.damage = 2;
        c.health = 1;

        c.MINION = true;

        c.triggers.Add((Trigger.Type.EndTurn, Trigger.Side.Friendly, Trigger.Ability.Emperor_Thaurissan));
        return c;
    }
    static CardInfo Mana_Wraith()
    { 
        CardInfo c = new();

        c.name = "Mana Wraith";
        c.text = "ALL minions cost 1 more.";

        c.manaCost = 2;
        c.damage = 2;
        c.health = 2;

        c.MINION = true;

        c.auras.Add(Aura.Type.Mana_Wraith);
        return c;
    }
    
    static CardInfo Loatheb()
    { 
        CardInfo c = new();

        c.name = "Loatheb";
        c.text = "Battlecry: Enemy spells cost 5 more next turn.";

        c.manaCost = 1;
        c.damage = 5;
        c.health = 5;

        c.MINION = true;
        c.BATTLECRY = true;
        return c;
    }
    static CardInfo Stormwind_Champion()
    { 
        CardInfo c = new();

        c.name = "Stormwind Champion";
        c.text = "Ally minions have +1/+1";

        c.manaCost = 1;
        c.damage = 6;
        c.health = 6;

        c.MINION = true;
        c.auras.Add(Aura.Type.StormwindChampion);
        return c;
    }
    static CardInfo Millhouse_Manastorm()
    { 
        CardInfo c = new();

        c.name = "Millhouse Manastorm";
        c.text = "Battlecry: Enemy spells cost 0 next turn.";

        c.manaCost = 2;
        c.damage = 4;
        c.health = 4;

        c.MINION = true;
        c.BATTLECRY = true;
        return c;
    }
    static CardInfo Crazed_Alchemist()
    { 
        CardInfo c = new();

        c.name = "Crazed Alchemist";
        c.text = "Battlecry: Swap health and attack of target minion.";

        c.manaCost = 2;
        c.damage = 2;
        c.health = 2;

        c.MINION = true;
        c.BATTLECRY = true;
        c.TARGETED = true;
        return c;
    }
    static CardInfo Chillwind_Yeti()
    { 
        CardInfo c = new();

        c.name = "Chillwind Yeti";
        c.text = "";

        c.manaCost = 4;
        c.damage = 4;
        c.health = 5;

        c.MINION = true;

        c.auras.Add(Aura.Type.Stealth);
        return c;
    }
    static CardInfo Archmage_Antonidas()
    { 
        CardInfo c = new();

        c.name = "Archmage Antonidas";
        c.text = "Whenever you cast a spell, add a Fireball spell to your hand.";

        c.manaCost = 1;
        c.damage = 5;
        c.health = 7;

        c.MINION = true;

        c.triggers.Add((Trigger.Type.OnPlaySpell,Trigger.Side.Friendly,Trigger.Ability.Archmage_Antonidas));
        return c;
    }
    static CardInfo Ogre_Magi()
    { 
        CardInfo c = new();

        c.name = "Ogre Magi";
        c.text = "+1 Spellpower.";

        c.manaCost = 1;
        c.damage = 4;
        c.health = 4;

        c.MINION = true;

        c.auras.Add(Aura.Type.Spellpower);
        return c;
    }
    static CardInfo Southsea_Deckhand()
    { 
        CardInfo c = new();

        c.name = "Southsea Deckhand";
        c.text = "Has Change while you have a weapon equipped.";

        c.manaCost = 1;
        c.damage = 2;
        c.health = 1;

        c.MINION = true;

        c.auras.Add(Aura.Type.Southsea_Deckhand);
        return c;
    }
    static CardInfo Grim_Patron()
    { 
        CardInfo c = new();

        c.name = "Grim Patron";
        c.text = "Whenever this minion survives damage, summon a Grim Patron";

        c.manaCost = 5;
        c.damage = 3;
        c.health = 3;

        c.MINION = true;

        c.triggers.Add((Trigger.Type.OnDamageTaken, Trigger.Side.Both, Trigger.Ability.Grim_Patron));
        return c;
    }

    static CardInfo Mountain_Giant()
    { 
        CardInfo c = new();

        c.name = "Mountain Giant";
        c.text = "Costs 1 less for each other card in your hand.";

        c.manaCost = 12;
        c.damage = 8;
        c.health = 8;

        c.MINION = true;

        c.cardAuras.Add(Aura.Type.Mountain_Giant);
        return c;
    }
    static CardInfo Sea_Giant()
    { 
        CardInfo c = new();

        c.name = "Sea Giant";
        c.text = "Costs 1 less for each minion on the board.";

        c.manaCost = 10;
        c.damage = 8;
        c.health = 8;

        c.MINION = true;

        c.cardAuras.Add(Aura.Type.Sea_Giant);
        return c;
    }
    static CardInfo Molten_Giant()
    { 
        CardInfo c = new();

        c.name = "Molten Giant";
        c.text = "Costs 1 less for each health you're missing.";

        c.manaCost = 20;
        c.damage = 8;
        c.health = 8;

        c.MINION = true;

        c.cardAuras.Add(Aura.Type.Molten_Giant);
        return c;
    }
    private static CardInfo Bloodmage_Thalnos()
    {
        CardInfo c = new();

        c.name = "Bloodmage Thalnos";
        c.text = "+1 Spellpower.\nDeathrattle: Draw a card.";

        c.manaCost = 2;
        c.damage = 1;
        c.health = 1;

        c.MINION = true;

        c.auras.Add(Aura.Type.Spellpower);
        c.triggers.Add((Trigger.Type.Deathrattle, Trigger.Side.Both, Trigger.Ability.DrawCard));

        return c;
    }
    private static CardInfo Earthen_Ring_Farseer()
    {
        CardInfo c = new();

        c.name = "Earthen Ring Farseer";
        c.text = "Battlecry: Restore 3 health.";

        c.manaCost = 3;
        c.damage = 3;
        c.health = 3;

        c.MINION = true;
        c.BATTLECRY = true;
        c.TARGETED = true;

        c.eligibleTargets = Board.EligibleTargets.AllCharacters;


        return c;
    } 
    private static CardInfo Violet_Teacher()
    {
        CardInfo c = new();

        c.name = "Violet Teacher";
        c.text = "Whenever you cast a spell, summon a 1/1 Violet Apprentice";

        c.manaCost = 4;
        c.damage = 3;
        c.health = 5;

        c.MINION = true;

        c.triggers.Add((Trigger.Type.OnPlaySpell, Trigger.Side.Friendly, Trigger.Ability.Violet_Teacher));

        return c;
    }
    private static CardInfo Violet_Apprentice()
    {
        CardInfo c = new();

        c.name = "Violet Apprentice";
        c.text = "Ready to learn.";

        c.manaCost = 1;
        c.damage = 1;
        c.health = 1;

        c.MINION = true;

        return c;
    }

}
