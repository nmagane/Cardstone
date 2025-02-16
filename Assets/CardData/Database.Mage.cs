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
        c.text = "Secret: When your hero is attacked, gain 8 armor.";

        c.classType = Card.Class.Mage;

        c.manaCost = 3;

        c.SPELL = true;
        c.SECRET = true;

        c.triggers.Add((Trigger.Type.BeforeAttackFace, Trigger.Side.Enemy, Trigger.Ability.Ice_Block));
        c.triggers.Add((Trigger.Type.BeforeSwingFace, Trigger.Side.Enemy, Trigger.Ability.Ice_Barrier));
        return c;
    }

    static CardInfo Frostbolt()
    {
        CardInfo c = new();

        c.name = "Frostbolt";
        c.text = "Deal {0} damage to a target and Freeze it.";

        c.classType = Card.Class.Mage;

        c.manaCost = 2;
        c.spellDamage = 3;

        c.SPELL = true;
        c.TARGETED = true;

        return c;
    }
}
