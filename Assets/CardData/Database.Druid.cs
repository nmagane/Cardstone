public partial class Database
{
    static CardInfo Wrath()
    {
        CardInfo c = new();

        c.name = "Wrath";
        c.text = "Choose one: Deal {0} damage to a minion; or {1} damage and draw a card.";
        c.manaCost = 2;
        
        c.spellDamage = 3;
        c.comboSpellDamage = 1;

        c.classType = Card.Class.Druid;
        c.eligibleTargets = Board.EligibleTargets.AllMinions;

        c.SPELL = true;
        c.TARGETED = true;

        c.CHOOSE = true;
        c.choice1 = Card.Cardname.Wrath_Big;
        c.choice2 = Card.Cardname.Wrath_Small;

        return c;
    }
    static CardInfo Wrath_Big()
    {
        CardInfo c = new();

        c.name = "Wrath";
        c.text = "Deal {0} damage to a minion.";
        c.manaCost = 2;
        c.classType = Card.Class.Druid;

        c.spellDamage = 3;
        c.SPELL = true;
        c.TARGETED = true;
        c.TOKEN = true;

        c.eligibleTargets = Board.EligibleTargets.AllMinions;

        return c;
    }
    static CardInfo Wrath_Small()
    {
        CardInfo c = new();

        c.name = "Wrath";
        c.text = "Deal {0} damage to a minion.\nDraw a card.";
        c.manaCost = 2;
        c.classType = Card.Class.Druid;

        c.spellDamage = 1;
        c.SPELL = true;
        c.TARGETED = true;
        c.TOKEN = true;
        c.eligibleTargets = Board.EligibleTargets.AllMinions;

        return c;
    }
    
    static CardInfo Ancient_of_War()
    {
        CardInfo c = new();

        c.name = "Ancient of War";
        c.text = "Choose one:\n+5 Attack; or +5 Health and Taunt.";
        c.manaCost = 7;
        c.classType = Card.Class.Druid;
        c.damage = 5;
        c.health = 5;

        c.MINION = true;
        c.CHOOSE = true;
        c.choice1 = Card.Cardname.Ancient_of_War_Attack;
        c.choice2 = Card.Cardname.Ancient_of_War_Taunt;

        return c;
    }
    static CardInfo Ancient_of_War_Attack()
    {
        CardInfo c = new();

        c.name = "Uproot";
        c.text = "+5 Attack.";
        c.manaCost = 7;
        c.classType = Card.Class.Druid;

        c.SPELL = true;
        c.TARGETED = false;
        c.TOKEN = true;

        return c;
    }
    static CardInfo Ancient_of_War_Taunt()
    {
        CardInfo c = new();

        c.name = "Rooted";
        c.text = "+5 Health\nand Taunt.";
        c.manaCost = 7;
        c.classType = Card.Class.Druid;

        c.SPELL = true;
        c.TARGETED = false;
        c.TOKEN = true;

        return c;
    }
    
    static CardInfo Keeper_of_the_Grove()
    {
        CardInfo c = new();

        c.name = "Keeper of the Grove";
        c.text = "Choose one: Deal 2 damage; or Silence a minion.";
        c.manaCost = 4;
        c.classType = Card.Class.Druid;
        c.damage = 2;
        c.health = 4;

        c.spellDamage = 2;

        c.MINION = true;
        c.CHOOSE = true;
        c.choice1 = Card.Cardname.Keeper_of_the_Grove_Damage;
        c.choice2 = Card.Cardname.Keeper_of_the_Grove_Silence;

        return c;
    }
    static CardInfo Keeper_of_the_Grove_Damage()
    {
        CardInfo c = new();

        c.name = "Moonfire";
        c.text = "Deal 2 damage";
        c.manaCost = 4;
        c.classType = Card.Class.Druid;

        c.SPELL = true;
        c.TARGETED = true;

        c.eligibleTargets = Board.EligibleTargets.AllCharacters;
        c.spellDamage = 2;

        c.TOKEN = true;

        return c;
    }
    static CardInfo Keeper_of_the_Grove_Silence()
    {
        CardInfo c = new();

        c.name = "Dispel";
        c.text = "Silence a minion.";
        c.manaCost = 4;
        c.classType = Card.Class.Druid;

        c.SPELL = true;
        c.TARGETED = true;

        c.eligibleTargets = Board.EligibleTargets.AllMinions;

        c.TOKEN = true;

        return c;
    }

    
    
}
