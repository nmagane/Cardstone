using System;

public partial class Database
{
    static CardInfo Ping()
    {
        CardInfo c = new();

        c.name = "Ping";
        c.text = "Deal 1 damage.";

        c.classType = Card.Class.Mage;
        c.eligibleTargets = Board.EligibleTargets.AllCharacters;

        c.manaCost = 2;
        c.spellDamage = 1;

        c.SPELL = true;
        c.TARGETED = true;
        c.TOKEN = true;

        return c;
    }
    static CardInfo Mage_Secret()
    {
        CardInfo c = new();

        c.name = "Secret";
        c.text = "Hidden effect until triggered.";

        c.classType = Card.Class.Mage;

        c.manaCost = 3;

        c.SPELL = true;
        c.SECRET = true;
        c.TOKEN = true;

        return c;
    }
    static CardInfo Ice_Lance()
    {
        CardInfo c = new();

        c.name = "Ice Lance";
        c.text = "Freeze a character.\nIf it's already frozen, deal {0} damage instead.";

        c.classType = Card.Class.Mage;
        c.eligibleTargets = Board.EligibleTargets.AllCharacters;

        c.manaCost = 1;
        c.spellDamage = 4;

        c.SPELL = true;
        c.TARGETED = true;

        return c;
    }

    static CardInfo Frostbolt()
    {
        CardInfo c = new();

        c.name = "Frostbolt";
        c.text = "Deal {0} damage to a target and Freeze it.";

        c.classType = Card.Class.Mage;
        c.eligibleTargets = Board.EligibleTargets.AllCharacters;

        c.manaCost = 2;
        c.spellDamage = 3;

        c.SPELL = true;
        c.TARGETED = true;

        return c;
    }
    static CardInfo Polymorph()
    {
        CardInfo c = new();

        c.name = "Polymorph";
        c.text = "Transform a minion into a 1/1 sheep.";

        c.classType = Card.Class.Mage;
        c.eligibleTargets = Board.EligibleTargets.AllMinions;

        c.manaCost = 4;

        c.SPELL = true;
        c.TARGETED = true;

        return c;
    }

    private static CardInfo Sheep()
    {
        CardInfo c = new();

        c.name = "Sheep";
        c.text = "A lamb without blemish.";

        c.classType = Card.Class.Mage;

        c.manaCost = 1;
        c.damage = 1;
        c.health = 1;

        c.MINION = true;
        c.TOKEN = true;

        return c;
    }

    static CardInfo Arcane_Intellect()
    {
        CardInfo c = new();

        c.name = "Arcane Intellect";
        c.text = "Draw 2 cards.";

        c.classType = Card.Class.Mage;

        c.manaCost = 3;

        c.SPELL = true;
        c.TARGETED = false;

        return c;
    }
    static CardInfo Frost_Nova()
    {
        CardInfo c = new();

        c.name = "Frost Nova";
        c.text = "Freeze all enemy minions.";

        c.classType = Card.Class.Mage;

        c.manaCost = 3;

        c.SPELL = true;
        c.TARGETED = false;

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

        c.triggers.Add((Trigger.Type.BeforeAttackFace, Trigger.Side.Enemy, Trigger.Ability.Ice_Barrier));
        c.triggers.Add((Trigger.Type.BeforeSwingFace, Trigger.Side.Enemy, Trigger.Ability.Ice_Barrier));
        return c;
    }
    static CardInfo Ice_Block()
    {
        CardInfo c = new();

        c.name = "Ice Block";
        c.text = "Secret: When your hero takes lethal damage, become Immune this turn.";

        c.classType = Card.Class.Mage;

        c.manaCost = 3;

        c.SPELL = true;
        c.SECRET = true;

        c.triggers.Add((Trigger.Type.OnLethalFaceDamage, Trigger.Side.Enemy, Trigger.Ability.Ice_Block));
        return c;
    }
    
    static CardInfo Fireball()
    {
        CardInfo c = new();

        c.name = "Fireball";
        c.text = "Deal {0} damage.";

        c.classType = Card.Class.Mage;

        c.manaCost = 4;
        c.spellDamage = 6;

        c.eligibleTargets = Board.EligibleTargets.AllCharacters;

        c.SPELL = true;
        c.TARGETED = true;

        return c;
    }
    static CardInfo Flamestrike()
    {
        CardInfo c = new();

        c.name = "Flamestrike";
        c.text = "Deal {0} damage to all enemy minions.";

        c.classType = Card.Class.Mage;

        c.manaCost = 7;
        c.spellDamage = 4;

        c.SPELL = true;
        c.TARGETED = false;

        return c;
    }
    static CardInfo Blizzard()
    {
        CardInfo c = new();

        c.name = "Blizzard";
        c.text = "Deal {0} damage to all enemy minions and Freeze them.";

        c.classType = Card.Class.Mage;

        c.manaCost = 6;
        c.spellDamage = 2;

        c.SPELL = true;
        c.TARGETED = false;

        return c;
    }
    
    static CardInfo Pyroblast()
    {
        CardInfo c = new();

        c.name = "Pyroblast";
        c.text = "Deal {0} damage.";

        c.classType = Card.Class.Mage;
        c.eligibleTargets = Board.EligibleTargets.AllCharacters;

        c.manaCost = 10;
        c.spellDamage = 10;

        c.SPELL = true;
        c.TARGETED = true;

        return c;
    }
    static CardInfo Archmage_Antonidas()
    {
        CardInfo c = new();

        c.name = "Archmage Antonidas";
        c.text = "Whenever you cast a spell, add a Fireball spell to your hand.";

        c.classType = Card.Class.Mage;

        c.manaCost = 7;
        c.damage = 5;
        c.health = 7;

        c.MINION = true;
        c.LEGENDARY = true;

        c.triggers.Add((Trigger.Type.OnPlaySpell, Trigger.Side.Friendly, Trigger.Ability.Archmage_Antonidas));
        return c;
    }
    static CardInfo Sorcerers_Apprentice()
    {
        CardInfo c = new();

        c.name = "Sorcerers Apprentice";
        c.text = "Your spells cost 1 less.";

        c.classType = Card.Class.Mage;

        c.manaCost = 2;
        c.damage = 3;
        c.health = 2;

        c.MINION = true;

        c.auras.Add(Aura.Type.Sorcerers_Apprentice);
        return c;
    }

    private static CardInfo Snowchugger()
    {
        CardInfo c = new();

        c.name = "Snowchugger";
        c.text = "Freeze any character damaged by this minion";

        c.manaCost = 2;
        c.damage = 2;
        c.health = 3;
        c.tribe = Card.Tribe.Mech;
        c.classType = Card.Class.Mage;

        c.auras.Add(Aura.Type.Frostbite);

        c.MINION = true;

        return c;
    }

    static CardInfo Arcane_Missiles()
    {
        CardInfo c = new();

        c.name = "Arcane Missiles";
        c.text = "Deal {0} damage randomly split among all enemies.";

        c.classType = Card.Class.Mage;

        c.manaCost = 1;
        c.spellDamage = 3;

        c.SPELL = true;
        c.TARGETED = false;

        return c;
    }

    static CardInfo Arcane_Explosion()
    {
        CardInfo c = new();

        c.name = "Arcane Explosion";
        c.text = "Deal {0} damage to all enemy minions.";

        c.classType = Card.Class.Mage;

        c.manaCost = 2;
        c.spellDamage = 1;

        c.SPELL = true;
        c.TARGETED = false;

        return c;
    }

    static CardInfo Mirror_Image()
    {
        CardInfo c = new();

        c.name = "Mirror Image";
        c.text = "Summon two 0/2 minions with Taunt.";

        c.classType = Card.Class.Mage;

        c.SPELL = true;
        c.TARGETED = false;

        return c;
    }

    static CardInfo Mirror_Image_Token()
    {
        CardInfo c = new();

        c.name = "Mirror Image";
        c.text = "Taunt";

        c.classType = Card.Class.Mage;

        c.manaCost = 0;
        c.damage = 0;
        c.health = 2;

        c.MINION = true;
        c.TOKEN = true;

        c.auras.Add(Aura.Type.Taunt);

        return c;
    }

    static CardInfo Cone_of_Cold()
    {
        CardInfo c = new();

        c.name = "Cone of Cold";
        c.text = "Freeze a minion and the minions next to it, and deal {0} damage to them.";

        c.classType = Card.Class.Mage;

        c.manaCost = 4;
        c.spellDamage = 1;

        c.SPELL = true;
        c.TARGETED = true;

        return c;
    }

    static CardInfo Mana_Wyrm()
    {
        CardInfo c = new();

        c.name = "Mana Wyrm";
        c.text = "Whenever you cast a spell, gain +1 Attack.";

        c.classType = Card.Class.Mage;

        c.manaCost = 1;
        c.damage = 1;
        c.health = 3;

        c.MINION = true;

        c.triggers.Add((Trigger.Type.OnPlaySpell, Trigger.Side.Friendly, Trigger.Ability.Mana_Wyrm));

        return c;
    }
    
    static CardInfo Ethereal_Arcanist()
    {
        CardInfo c = new();

        c.name = "Ethereal Arcanist";
        c.text = "If you control a Secret at the end of your turn, gain +2/+2.";

        c.classType = Card.Class.Mage;

        c.manaCost = 4;
        c.damage = 3;
        c.health = 3;

        c.MINION = true;

        c.triggers.Add((Trigger.Type.EndTurn, Trigger.Side.Friendly, Trigger.Ability.Ethereal_Arcanist));

        return c;
    }
}
