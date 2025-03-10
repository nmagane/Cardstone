public partial class Database
{

    static CardInfo Lifetap()
    {
        CardInfo c = new();

        c.classType = Card.Class.Warlock;
        c.name = "Life Tap";
        c.text = "Take 2 damage. Draw a card.";
        c.manaCost = 2;
        c.SPELL = true;
        c.TARGETED = false;
        c.eligibleTargets = Board.EligibleTargets.FriendlyHero;
        c.TOKEN = true;

        return c;
    }
    static CardInfo Soulfire()
    {
        CardInfo c = new();

        c.name = "Soulfire";
        c.text = "Deal {0} damage. Discard a random card.";

        c.classType = Card.Class.Warlock;
        c.eligibleTargets = Board.EligibleTargets.AllCharacters;

        c.manaCost = 1;
        c.spellDamage = 4;
        c.SPELL = true;
        c.TARGETED = true;

        return c;
    }
    static CardInfo Mortal_Coil()
    {
        CardInfo c = new();

        c.name = "Mortal Coil";
        c.text = "Deal {0} damage to a minion. If it kills it, draw a card";

        c.classType = Card.Class.Warlock;
        c.eligibleTargets = Board.EligibleTargets.AllMinions;

        c.manaCost = 1;
        c.spellDamage = 1;
        c.SPELL = true;
        c.TARGETED = true;

        return c;
    }
    static CardInfo Power_Overwhelming()
    {
        CardInfo c = new();

        c.name = "Power Overwhelming";
        c.text = "Give a friendly minion +4/+4 until end of turn.\nThen, it dies.";

        c.classType = Card.Class.Warlock;
        c.eligibleTargets = Board.EligibleTargets.FriendlyMinions;

        c.manaCost = 1;
        c.SPELL = true;
        c.TARGETED = true;

        return c;
    }
    static CardInfo Flame_Imp()
    {
        CardInfo c = new();

        c.classType = Card.Class.Warlock;
        c.name = "Flame Imp";
        c.text = "Battlecry: Deal 3 damage to your hero.";
        c.manaCost = 1;
        c.damage = 3;
        c.health = 2;
        c.tribe = Card.Tribe.Demon;

        c.MINION = true;
        c.BATTLECRY = true;
        c.TARGETED = false;

        return c;
    }
    static CardInfo Voidwalker()
    {
        CardInfo c = new();

        c.classType = Card.Class.Warlock;
        c.name = "Voidwalker";
        c.text = "Taunt.";

        c.manaCost = 1;
        c.damage = 1;
        c.health = 3;
        c.tribe = Card.Tribe.Demon;

        c.MINION = true;
        c.auras.Add(Aura.Type.Taunt);

        return c;
    }
    static CardInfo Imp()
    {
        CardInfo c = new();

        c.classType = Card.Class.Warlock;
        c.name = "Imp";
        c.text = "Its laughter drives needles through your mind.";

        c.manaCost = 1;
        c.damage = 1;
        c.health = 1;
        c.tribe = Card.Tribe.Demon;

        c.MINION = true;
        c.TOKEN = true;

        return c;
    }
    static CardInfo Lord_Jaraxxus()
    {
        CardInfo c = new();

        c.classType = Card.Class.Warlock;
        c.name = "Lord Jaraxxus";
        c.text = "Battlecry: Replace your hero with \nLord Jaraxxus.";

        c.manaCost = 9;
        c.damage = 3;
        c.health = 15;
        c.tribe = Card.Tribe.Demon;

        c.MINION = true;
        c.BATTLECRY = true;
        c.LEGENDARY = true;

        return c;
    }
    static CardInfo Infernal()
    {
        CardInfo c = new();

        c.classType = Card.Class.Warlock;
        c.name = "Infernal";
        c.text = "Demon from below.";

        c.manaCost = 6;
        c.damage = 6;
        c.health = 6;
        c.tribe = Card.Tribe.Demon;

        c.MINION = true;
        c.TOKEN = true;

        return c;
    }
    static CardInfo Inferno()
    {
        CardInfo c = new();

        c.classType = Card.Class.Warlock;
        c.name = "INFERNO!";
        c.text = "Summon a 6/6 Infernal.";

        c.manaCost = 2;

        c.SPELL = true;
        c.TARGETED = false;
        c.TOKEN = true;

        return c;
    }
    static CardInfo Blood_Fury()
    {
        CardInfo c = new();

        c.classType = Card.Class.Warlock;
        c.name = "Blood Fury";
        c.text = "";

        c.manaCost = 3;
        c.damage = 3;
        c.health = 8;

        c.WEAPON = true;
        c.TOKEN = true;

        return c;
    }

    static CardInfo Imp_Gang_Boss()
    {
        CardInfo c = new();

        c.classType = Card.Class.Warlock;
        c.name = "Imp Gang Boss";
        c.text = "Whenever this minion takes damage, summon a 1/1 Imp.";

        c.manaCost = 3;
        c.damage = 2;
        c.health = 4;
        c.tribe = Card.Tribe.Demon;

        c.MINION = true;

        c.triggers.Add((Trigger.Type.OnDamageTaken, Trigger.Side.Both, Trigger.Ability.Imp_Gang_Boss));

        return c;
    }
    static CardInfo Doomguard()
    {
        CardInfo c = new();

        c.classType = Card.Class.Warlock;
        c.name = "Doomguard";
        c.text = "Charge. Battlecry: Discard 2 random cards.";

        c.manaCost = 5;
        c.damage = 5;
        c.health = 7;
        c.tribe = Card.Tribe.Demon;

        c.MINION = true;
        c.BATTLECRY = true;
        c.auras.Add(Aura.Type.Charge);

        return c;
    }
    static CardInfo Implosion()
    {
        CardInfo c = new();

        c.classType = Card.Class.Warlock;
        c.name = "Imp-losion";
        c.text = "Deal {0}-{1} damage to a minion.\nSummon that many imps.";
        c.spellDamage = 2;
        c.comboSpellDamage = 4;

        c.eligibleTargets = Board.EligibleTargets.AllMinions;
        c.manaCost = 4;

        c.SPELL = true;
        c.TARGETED = true;

        return c;
    }
    static CardInfo Voidcaller()
    {
        CardInfo c = new();

        c.classType = Card.Class.Warlock;
        c.name = "Voidcaller";
        c.text = "Deathrattle: Put a Demon from your hand into play.";

        c.tribe = Card.Tribe.Demon;
        c.manaCost = 4;

        c.damage = 3;
        c.health = 4;

        c.MINION = true;


        c.triggers.Add((Trigger.Type.Deathrattle, Trigger.Side.Both, Trigger.Ability.Voidcaller));

        return c;
    }
    
    static CardInfo Darkbomb()
    {
        CardInfo c = new();

        c.classType = Card.Class.Warlock;
        c.name = "Darkbomb";
        c.text = "Deal {0} damage.";

        c.manaCost = 2;
        c.spellDamage = 3;

        c.SPELL = true;
        c.TARGETED = true;
        c.eligibleTargets = Board.EligibleTargets.AllCharacters;

        return c;
    }
    static CardInfo Hellfire()
    {
        CardInfo c = new();

        c.classType = Card.Class.Warlock;
        c.name = "Hellfire";
        c.text = "Deal {0} damage to all characters.";

        c.manaCost = 4;
        c.spellDamage = 3;

        c.SPELL = true;
        c.TARGETED = false;

        return c;
    }
    static CardInfo Shadowflame()
    {
        CardInfo c = new();

        c.classType = Card.Class.Warlock;
        c.name = "Shadowflame";
        c.text = "Destroy a friendly minion. Deal its attack damage to all enemy minoins.";

        c.manaCost = 4;

        c.SPELL = true;
        c.TARGETED = true;
        c.eligibleTargets = Board.EligibleTargets.FriendlyMinions;

        return c;
    }
    static CardInfo Siphon_Soul()
    {
        CardInfo c = new();

        c.classType = Card.Class.Warlock;
        c.name = "Siphon Soul";
        c.text = "Destroy a minion.\nRestore 3 health to your hero.";

        c.manaCost = 6;

        c.SPELL = true;
        c.TARGETED = true;
        c.eligibleTargets = Board.EligibleTargets.AllMinions;

        return c;
    }

    private static CardInfo Queen_of_Pain()
    {
        CardInfo c = new();

        c.name = "Queen of Pain";
        c.text = "Lifesteal.";

        c.manaCost = 2;
        c.damage = 1;
        c.health = 4;

        c.MINION = true;
        c.classType = Card.Class.Warlock;
        c.auras.Add(Aura.Type.Lifesteal);

        return c;
    }
    private static CardInfo Succubus()
    {
        CardInfo c = new();

        c.name = "Succubus";
        c.text = "Battlecry: Discard a random card.";

        c.manaCost = 2;
        c.damage = 4;
        c.health = 3;
        c.classType = Card.Class.Warlock;
        c.tribe = Card.Tribe.Demon;

        c.MINION = true;
        c.BATTLECRY = true;

        return c;
    }
}
