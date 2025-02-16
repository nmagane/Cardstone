using System;

public partial class Database
{
    static CardInfo Mage_Secret()
    {
        CardInfo c = new();

        c.name = "Secret";
        c.text = "Hidden effect until triggered.";

        c.classType = Card.Class.Mage;

        c.manaCost = 3;

        c.SPELL = true;
        c.SECRET = true;

        return c;
    }
    static CardInfo Ice_Barrier()
    {
        CardInfo c = new();

        c.name = "Ice Barrier";
        c.text = "";

        c.classType = Card.Class.Mage;

        c.manaCost = 3;

        c.SPELL = true;
        c.SECRET = true;

        c.triggers.Add((Trigger.Type.BeforeAttackFace, Trigger.Side.Enemy, Trigger.Ability.Ice_Block));
        return c;
    }
}
