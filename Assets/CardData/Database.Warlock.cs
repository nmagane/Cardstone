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

        c.classType = Card.Class.Warlock;
        c.name = "Soulfire";
        c.text = "Deal {0} damage. Discard a random card.";
        c.manaCost = 0;
        c.spellDamage = 4;
        c.SPELL = true;
        c.TARGETED = true;
        c.eligibleTargets = Board.EligibleTargets.AllCharacters;

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

        c.MINION = true;
        c.auras.Add(Aura.Type.Taunt);

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

        c.MINION = true;
        c.BATTLECRY = true;
        c.auras.Add(Aura.Type.Charge);

        return c;
    }
}
