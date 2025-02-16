public partial class Database
{
    static CardInfo Paladin_Secret()
    {
        CardInfo c = new();

        c.name = "Secret";
        c.text = "Hidden effect until triggered.";

        c.classType = Card.Class.Paladin;

        c.manaCost = 1;

        c.SPELL = true;
        c.SECRET = true;

        return c;
    }
}
