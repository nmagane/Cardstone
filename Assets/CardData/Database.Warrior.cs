public partial class Database
{

    static CardInfo Heroic_Strike()
    {
        CardInfo c = new();

        c.name = "Heroic Strike";
        c.text = "Give your hero +4 attack this turn.";

        c.classType = Card.Class.Warrior;

        c.manaCost = 2;

        c.SPELL = true;
        c.TARGETED = false;
        return c;
    }
    static CardInfo Armor_Up()
    {
        CardInfo c = new();

        c.name = "Armor Up";
        c.text = "Give your hero +2 armor.";

        c.classType = Card.Class.Warrior;

        c.manaCost = 2;

        c.SPELL = true;
        c.TARGETED = false;
        return c;
    }
}
