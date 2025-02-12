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
}
