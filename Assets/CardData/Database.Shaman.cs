public partial class Database
{
    private static CardInfo Vitality_Totem()
    {
        CardInfo c = new();

        c.name = "Vitality Totem";
        c.text = "At the end of your turn, restore 4 Health to your hero.";

        c.manaCost = 2;
        c.damage = 0;
        c.health = 4;
        c.classType = Card.Class.Shaman;

        c.MINION = true;
        c.tribe = Card.Tribe.Totem;

        return c;
    }
    private static CardInfo Whirling_Zapomatic()
    {
        CardInfo c = new();

        c.name = "Whirling Zap-o-matic";
        c.text = "Windfury.";

        c.manaCost = 2;
        c.damage = 3;
        c.health = 2;
        c.classType = Card.Class.Shaman;
        c.tribe = Card.Tribe.Mech;

        c.auras.Add(Aura.Type.Windfury);

        c.MINION = true;

        return c;
    }
    private static CardInfo Flametongue_Totem()
    {
        CardInfo c = new();

        c.name = "Flametongue Totem";
        c.text = "Give adjacent minions +2 attack.";

        c.manaCost = 2;
        c.damage = 0;
        c.health = 3;
        c.classType = Card.Class.Shaman;
        c.tribe = Card.Tribe.Totem;

        c.MINION = true;
        c.auras.Add(Aura.Type.DireWolfAlpha);
        c.auras.Add(Aura.Type.DireWolfAlpha);

        return c;
    }

}
