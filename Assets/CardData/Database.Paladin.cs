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

        c.TOKEN = true;

        return c;
    }

    private static CardInfo Shielded_Minibot()
    {
        CardInfo c = new();

        c.name = "Shielded Minibot";
        c.text = "Divine Sheild.";

        c.manaCost = 2;
        c.damage = 2;
        c.health = 2;
        c.classType = Card.Class.Paladin;

        c.MINION = true;

        c.tribe = Card.Tribe.Mech;
        c.auras.Add(Aura.Type.Shield);

        return c;
    }
    private static CardInfo Argent_Protector()
    {
        CardInfo c = new();

        c.name = "Argent Protector";
        c.text = "Battlecry: Give a friendly minion Divine Shield.";

        c.manaCost = 2;
        c.damage = 2;
        c.health = 2;
        c.classType = Card.Class.Paladin;

        c.eligibleTargets = Board.EligibleTargets.FriendlyMinions;
        c.BATTLECRY = true;
        c.TARGETED = true;
        c.MINION = true;

        return c;
    }
}
