public partial class Database
{
    static CardInfo Armor_Up()
    {
        CardInfo c = new();

        c.name = "Armor Up";
        c.text = "Give your hero +2 armor.";

        c.classType = Card.Class.Warrior;

        c.manaCost = 2;

        c.SPELL = true;
        c.TARGETED = false;
        c.TOKEN = true;
        return c;
    }

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
    static CardInfo Shield_Slam()
    {
        CardInfo c = new();

        c.name = "Shield Slam";
        c.text = "Deal damage to a minion equal to your armor.";

        c.classType = Card.Class.Warrior;

        c.manaCost = 1;

        c.eligibleTargets = Board.EligibleTargets.AllMinions;

        c.SPELL = true;
        c.TARGETED = true;
        return c;
    }
    static CardInfo Shield_Block()
    {
        CardInfo c = new();

        c.name = "Shield Block";
        c.text = "Gain +5 Armor.\nDraw a card.";

        c.classType = Card.Class.Warrior;

        c.manaCost = 3;

        c.SPELL = true;
        c.TARGETED = false;
        return c;
    }
    static CardInfo Revenge()
    {
        CardInfo c = new();

        c.name = "Revenge";
        c.text = "Deal {0} damage to all minions.\nIf you have 12 or less Health, deal {1}.";

        c.classType = Card.Class.Warrior;
        c.spellDamage = 1;
        c.comboSpellDamage = 3;

        c.manaCost = 2;

        c.SPELL = true;
        c.TARGETED = false;
        return c;
    }
    static CardInfo Brawl()
    {
        CardInfo c = new();

        c.name = "Brawl";
        c.text = "Destroy all minions except one.\n(Chosen randomly)";

        c.classType = Card.Class.Warrior;

        c.manaCost = 5;

        c.SPELL = true;
        c.TARGETED = false;
        return c;
    }
    static CardInfo Shieldmaiden()
    {
        CardInfo c = new();

        c.name = "Shieldmaiden";
        c.text = "Battlecry: Gain +5 Armor.";

        c.classType = Card.Class.Warrior;

        c.manaCost = 6;
        c.damage = 5;
        c.health = 5;

        c.MINION=true;
        c.BATTLECRY = true;
        return c;
    }
    static CardInfo Inner_Rage()
    {
        CardInfo c = new();

        c.name = "Inner Rage";
        c.text = "Deal {0} damage to a minion and give it +2 attack.";

        c.classType = Card.Class.Warrior;

        c.manaCost = 0;
        c.spellDamage = 1;
        c.eligibleTargets = Board.EligibleTargets.AllMinions;

        c.SPELL = true;
        c.TARGETED = true;
        return c;
    }
    
    static CardInfo Execute()
    {
        CardInfo c = new();

        c.name = "Execute";
        c.text = "Destroy target damaged minion.";

        c.classType = Card.Class.Warrior;

        c.manaCost = 1;
        c.eligibleTargets = Board.EligibleTargets.DamagedMinions;

        c.SPELL = true;
        c.TARGETED = true;
        return c;
    }
    static CardInfo Whirlwind()
    {
        CardInfo c = new();

        c.name = "Whirlwind";
        c.text = "Deal {0} damage to all minions.";

        c.classType = Card.Class.Warrior;

        c.manaCost = 1;
        c.spellDamage = 1;

        c.SPELL = true;
        c.TARGETED = false;
        return c;
    }
    static CardInfo Slam()
    {
        CardInfo c = new();

        c.name = "Slam";
        c.text = "Deal {0} damage to minion.\nIf it survives, draw a card.";

        c.classType = Card.Class.Warrior;
        

        c.manaCost = 2;
        c.spellDamage = 2; 
        c.eligibleTargets = Board.EligibleTargets.AllMinions;

        c.SPELL = true;
        c.TARGETED = true;
        return c;
    }
    static CardInfo Battle_Rage()
    {
        CardInfo c = new();

        c.name = "Battle Rage";
        c.text = "Draw a card for each damaged friendly character.";

        c.classType = Card.Class.Warrior;

        c.manaCost = 2;

        c.SPELL = true;
        c.TARGETED = false;
        return c;
    }
    static CardInfo Fiery_War_Axe()
    {
        CardInfo c = new();

        c.name = "Fiery War Axe";
        c.text = "";

        c.classType = Card.Class.Warrior;

        c.manaCost = 2;
        c.damage = 3;
        c.health = 2;

        c.WEAPON = true;
        return c;
    }
    static CardInfo Armorsmith()
    {
        CardInfo c = new();

        c.name = "Armorsmith";
        c.text = "Whenever a friendly minion takes damage, give your hero 1 armor";

        c.classType = Card.Class.Warrior;

        c.manaCost = 2;
        c.damage = 1;
        c.health = 4;

        c.MINION = true;
        c.triggers.Add((Trigger.Type.OnMinionDamage, Trigger.Side.Friendly, Trigger.Ability.Armorsmith));
        return c;
    }
    
    static CardInfo Cruel_Taskmaster()
    {
        CardInfo c = new();

        c.name = "Cruel Taskmaster";
        c.text = "Battlecry: Deal 1 damage to a minion and give it +2 attack.";

        c.classType = Card.Class.Warrior;

        c.manaCost = 2;
        c.damage = 2;
        c.health = 2;

        c.eligibleTargets = Board.EligibleTargets.AllMinions;

        c.MINION = true;
        c.BATTLECRY = true;
        c.TARGETED = true;
        return c;
    }
    
    static CardInfo Frothing_Berserker()
    {
        CardInfo c = new();

        c.name = "Frothing Berserker";
        c.text = "Whenever a minion takes damage, gain +1 attack.";

        c.classType = Card.Class.Warrior;

        c.manaCost = 3;
        c.damage = 2;
        c.health = 4;

        c.MINION = true;
        c.triggers.Add((Trigger.Type.OnMinionDamage, Trigger.Side.Both, Trigger.Ability.Frothing_Berserker));
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

        c.triggers.Add((Trigger.Type.AfterSummonMinion, Trigger.Side.Friendly, Trigger.Ability.Warsong_Commander));
        c.triggers.Add((Trigger.Type.OnPlayMinion, Trigger.Side.Friendly, Trigger.Ability.Warsong_Commander));

        return c;
    }

    static CardInfo Deaths_Bite()
    {
        CardInfo c = new();

        c.name = "Death's Bite";
        c.text = "Deathrattle: Deal 1 damage to all minions.";

        c.classType = Card.Class.Warrior;

        c.manaCost = 4;
        c.damage = 4;
        c.health = 2;

        c.WEAPON = true;

        c.triggers.Add((Trigger.Type.Deathrattle, Trigger.Side.Both, Trigger.Ability.Unstable_Ghoul));

        return c;
    }
    private static CardInfo Grommash_Hellscream()
    {
        CardInfo c = new();

        c.name = "Grommash Hellscream";
        c.text = "Charge.\nEnrage: +6 Attack.";

        c.manaCost = 8;
        c.damage = 4;
        c.health = 9;

        c.classType = Card.Class.Warrior;

        c.MINION = true;
        c.LEGENDARY = true;

        c.auras.Add(Aura.Type.Charge);
        c.auras.Add(Aura.Type.Grommash);

        return c;
    }
}
