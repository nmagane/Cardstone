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
    
}
