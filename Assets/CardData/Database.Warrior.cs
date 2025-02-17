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

    static CardInfo Warsong_Commander()
    {
        CardInfo c = new();

        c.name = "Warsong Commander";
        c.text = "Whenever you summon a minion with 3 or less attack,\ngive it Charge.";

        c.classType = Card.Class.Warrior;

        c.manaCost = 3;
        c.damage = 2;
        c.health = 3;

        c.MINION = true;

        c.triggers.Add((Trigger.Type.OnSummonMinion, Trigger.Side.Friendly, Trigger.Ability.Warsong_Commander));
        c.triggers.Add((Trigger.Type.OnPlayMinion, Trigger.Side.Friendly, Trigger.Ability.Warsong_Commander));

        return c;
    }
}
