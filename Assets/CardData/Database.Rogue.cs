public partial class Database
{
    static CardInfo Preparation()
    {
        CardInfo c = new();

        c.name = "Preparation";
        c.text = "The next spell you cast this turn costs 3 less.";

        c.classType = Card.Class.Rogue;

        c.manaCost = 0;

        c.SPELL = true;
        c.TARGETED = false;
        return c;
    }

    static CardInfo Dagger()
    {
        CardInfo c = new();

        c.name = "Dagger";
        c.text = "";

        c.classType = Card.Class.Rogue;

        c.manaCost = 2;
        c.damage = 1;
        c.health = 2;

        c.WEAPON = true;

        return c;
    }

    static CardInfo Deadly_Poison()
    {
        CardInfo c = new();

        c.name = "Deadly Poison";
        c.text = "Give your weapon +2 attack.";

        c.classType = Card.Class.Rogue;
        c.eligibleTargets = Board.EligibleTargets.Weapon;

        c.manaCost = 1;

        c.SPELL = true;
        c.TARGETED = false;
        return c;
    }
    static CardInfo Blade_Flurry()
    {
        CardInfo c = new();

        c.name = "Blade Flurry";
        c.text = "Destroy your weapon and deal its damage to all enemies.";

        c.classType = Card.Class.Rogue;
        c.eligibleTargets = Board.EligibleTargets.Weapon;

        c.manaCost = 1;

        c.SPELL = true;
        c.TARGETED = false;
        return c;
    }
    static CardInfo SI7_Agent()
    {
        CardInfo c = new();

        c.name = "SI:7 Agent";
        c.text = "Combo: Deal 2 damage to target.";

        c.classType = Card.Class.Rogue;
        c.eligibleTargets = Board.EligibleTargets.AllCharacters;

        c.manaCost = 3;

        c.damage = 3;
        c.health = 3;

        c.MINION = true;
        c.COMBO = true;
        c.COMBO_TARGETED = true;
        return c;
    }
    static CardInfo Eviscerate()
    {
        CardInfo c = new();

        c.name = "Eviscerate";
        c.text = "Deal 2 damage. Combo: Deal 4 instead.";

        c.classType = Card.Class.Rogue;
        c.eligibleTargets = Board.EligibleTargets.AllCharacters;

        c.SPELL = true;
        c.TARGETED = true;
        c.COMBO = true;
        return c;
    }
}
