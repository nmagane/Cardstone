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
        c.text = "Give a friendly minion +4/+4 until end of turn.\nThen, it dies. Horribly.";

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

        return c;
    }
    static CardInfo Imp_Gang_Boss()
    {
        CardInfo c = new();

        c.classType = Card.Class.Warlock;
        c.name = "Imp Gang Boss";
        c.text = "Whenever this minoin takes damage, summon a 1/1 Imp.";

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
        c.text = "Deathrattle: Put a random Demon from your hand into the battlefield.";

        c.tribe = Card.Tribe.Demon;
        c.manaCost = 4;

        c.damage = 3;
        c.health = 4;

        c.MINION = true;


        c.triggers.Add((Trigger.Type.Deathrattle, Trigger.Side.Both, Trigger.Ability.Voidcaller));

        return c;
    }

}
