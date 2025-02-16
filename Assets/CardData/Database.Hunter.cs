public partial class Database
{
    static CardInfo Hunter_Secret()
    {
        CardInfo c = new();

        c.name = "Secret";
        c.text = "Hidden effect until triggered.";

        c.classType = Card.Class.Hunter;

        c.manaCost = 2;

        c.SPELL = true;
        c.SECRET = true;

        return c;
    }
    static CardInfo Hunters_Mark()
    {
        CardInfo c = new();

        c.name = "Hunter's Mark";
        c.text = "Set target minion's health to 1";

        c.classType = Card.Class.Hunter;

        c.manaCost = 0;

        c.SPELL = true;
        c.TARGETED = true;
        return c;
    }
}
