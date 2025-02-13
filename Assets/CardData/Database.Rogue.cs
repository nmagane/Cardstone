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
}
