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

        public int manaCost=1;
        public int damage=1;
        public int health=1;
        public int spellDamage = 0;

        public bool SPELL = false;
        public bool MINION = false;
        public bool SECRET = false;
        public bool WEAPON = false;

        public bool TARGETED = false;
        public bool COMBO = false;
        public bool BATTLECRY = false;

        public Board.EligibleTargets eligibleTargets = Board.EligibleTargets.AllCharacters;

        public List<Aura.Type> auras = new List<Aura.Type>();
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
                return Thaurissan();

            default:
                Debug.LogError("ERROR: UNDEFINED CARD: " + card);
                break;
        }
        return info;
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
    
    static CardInfo Thaurissan()
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


}
