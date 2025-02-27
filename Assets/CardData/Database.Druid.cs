using System.Collections.Generic;
public partial class Database
{
    static CardInfo Shapeshift()
    {
        CardInfo c = new();

        c.name = "Shapeshift";
        c.text = "+1 Attack this turn.\nGain +1 armor.";
        c.manaCost = 2;

        c.classType = Card.Class.Druid;

        c.SPELL = true;
        c.TOKEN = true;
        return c;
    }
    static CardInfo Innervate()
    {
        CardInfo c = new();

        c.name = "Innervate";
        c.text = "Gain 2 mana this turn only.";
        c.manaCost = 0;

        c.classType = Card.Class.Druid;

        c.SPELL = true;
        return c;
    }
    static CardInfo Wild_Growth()
    {
        CardInfo c = new();

        c.name = "Wild Growth";
        c.text = "Gain +1 max mana.";
        c.manaCost = 2;

        c.classType = Card.Class.Druid;

        c.SPELL = true;
        return c;
    }
    static CardInfo Excess_Mana()
    {
        CardInfo c = new();

        c.name = "Excess Mana";
        c.text = "Draw a card.";
        c.manaCost = 0;

        c.classType = Card.Class.Druid;

        c.SPELL = true;
        c.TOKEN = true;
        return c;
    }
    static CardInfo Savage_Roar()
    {
        CardInfo c = new();

        c.name = "Savage Roar";
        c.text = "Give your characters +2 attack this turn.";
        c.manaCost = 3;

        c.classType = Card.Class.Druid;

        c.SPELL = true;
        return c;
    }
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
        c.choices = new(){ Card.Cardname.Wrath_Big, Card.Cardname.Wrath_Small};

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
        c.choices = new() { Card.Cardname.Ancient_of_War_Attack, Card.Cardname.Ancient_of_War_Taunt };

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
        c.choices = new List<Card.Cardname>() { Card.Cardname.Keeper_of_the_Grove_Damage, Card.Cardname.Keeper_of_the_Grove_Silence };

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

    static CardInfo Druid_of_the_Flame()
    {
        CardInfo c = new();

        c.name = "Druid of the Flame";
        c.text = "Choose one: Transform into a 5/2; or a 2/5 minion.";
        c.manaCost = 3;
        c.classType = Card.Class.Druid;
        c.damage = 2;
        c.health = 2;

        c.MINION = true;
        c.CHOOSE = true;
        c.choices = new List<Card.Cardname>() { Card.Cardname.Druid_of_the_Flame_Attack, Card.Cardname.Druid_of_the_Flame_Health };

        return c;
    }
    static CardInfo Druid_of_the_Flame_Attack()
    {
        CardInfo c = new();

        c.name = "Druid of the Flame";
        c.text = "";
        c.manaCost = 3;
        c.classType = Card.Class.Druid;

        c.damage = 5;
        c.health = 2;

        c.MINION=true;
        c.TOKEN = true;

        return c;
    }
    static CardInfo Druid_of_the_Flame_Health()
    {
        CardInfo c = new();

        c.name = "Druid of the Flame";
        c.text = "";
        c.manaCost = 3;
        c.classType = Card.Class.Druid;

        c.damage = 2;
        c.health = 5;

        c.MINION = true;
        c.TOKEN = true;

        return c;
    }
    static CardInfo Swipe()
    {
        CardInfo c = new();

        c.name = "Swipe";
        c.text = "Deal {0} damage to target enemy and {1} damage to all other enemies.";
        c.manaCost = 4;
        c.classType = Card.Class.Druid;


        c.spellDamage = 4;
        c.comboSpellDamage = 1;
        c.SPELL = true;
        c.TARGETED = true;
        c.eligibleTargets = Board.EligibleTargets.EnemyCharacters;
            

        return c;
    }

    static CardInfo Ancient_of_Lore()
    {
        CardInfo c = new();

        c.name = "Ancient of Lore";
        c.text = "Choose one: Draw 2 cards; or Restore 5 Health.";
        c.manaCost = 7;
        c.classType = Card.Class.Druid;
        c.damage = 5;
        c.health = 5;

        c.MINION = true;
        c.CHOOSE = true;
        c.choices = new List<Card.Cardname>() { Card.Cardname.Ancient_of_Lore_Draw, Card.Cardname.Ancient_of_Lore_Heal };

        return c;
    }
    static CardInfo Ancient_of_Lore_Draw()
    {
        CardInfo c = new();

        c.name = "Ancient Teachings";
        c.text = "Draw 2 cards.";
        c.manaCost = 7;
        c.classType = Card.Class.Druid;

        c.SPELL = true;
        c.TARGETED = false;


        c.TOKEN = true;

        return c;
    }
    static CardInfo Ancient_of_Lore_Heal()
    {
        CardInfo c = new();

        c.name = "Ancient Secrets";
        c.text = "Restore 5 Health.";
        c.manaCost = 4;
        c.classType = Card.Class.Druid;

        c.SPELL = true;
        c.TARGETED = true;

        c.eligibleTargets = Board.EligibleTargets.AllCharacters;

        c.TOKEN = true;

        return c;
    }

    static CardInfo Druid_of_the_Claw()
    {
        CardInfo c = new();

        c.name = "Druid of the Claw";
        c.text = "Choose one: Gain Charge; or a +2 Health and Taunt.";
        c.manaCost = 5;
        c.classType = Card.Class.Druid;
        c.damage = 4;
        c.health = 4;

        c.MINION = true;
        c.CHOOSE = true;
        c.choices = new List<Card.Cardname>() { Card.Cardname.Druid_of_the_Claw_Charge, Card.Cardname.Druid_of_the_Claw_Taunt };

        return c;
    }
    static CardInfo Druid_of_the_Claw_Charge()
    {
        CardInfo c = new();

        c.name = "Druid of the Claw";
        c.text = "Charge.";
        c.manaCost = 5;
        c.classType = Card.Class.Druid;

        c.damage = 4;
        c.health = 4;

        c.auras.Add(Aura.Type.Charge);

        c.MINION = true;
        c.TOKEN = true;

        return c;
    }
    static CardInfo Druid_of_the_Claw_Taunt()
    {
        CardInfo c = new();

        c.name = "Druid of the Claw";
        c.text = "Taunt.";
        c.manaCost = 5;
        c.classType = Card.Class.Druid;

        c.damage = 4;
        c.health = 6;

        c.auras.Add(Aura.Type.Taunt);

        c.MINION = true;
        c.TOKEN = true;

        return c;
    }

    static CardInfo Force_of_Nature()
    {
        CardInfo c = new();

        c.name = "Force of Nature";
        c.text = "Summon three 2/2 treants with Charge.\nThey die at end of turn.";
        c.manaCost = 6;
        c.classType = Card.Class.Druid;

        c.SPELL = true;
        c.TARGETED = false;

        return c;
    }
    static CardInfo Treant_Charge()
    {
        CardInfo c = new();

        c.name = "Treant";
        c.text = "Charge.\nDies at end of turn.";
        c.manaCost = 1;
        c.classType = Card.Class.Druid;

        c.damage = 2;
        c.health = 2;

        c.auras.Add(Aura.Type.Charge);
        c.triggers.Add((Trigger.Type.EndTurn, Trigger.Side.Friendly, Trigger.Ability.Power_Overwhelming));

        c.MINION = true;
        c.TOKEN = true;

        return c;
    }
    static CardInfo Treant()
    {
        CardInfo c = new();

        c.name = "Treant";
        c.text = "";
        c.manaCost = 1;
        c.classType = Card.Class.Druid;

        c.damage = 2;
        c.health = 2;

        c.MINION = true;
        c.TOKEN = true;

        return c;
    }
    static CardInfo Treant_Taunt()
    {
        CardInfo c = new();

        c.name = "Treant";
        c.text = "Taunt.";
        c.manaCost = 1;
        c.classType = Card.Class.Druid;

        c.damage = 2;
        c.health = 2;

        c.auras.Add(Aura.Type.Taunt);

        c.MINION = true;
        c.TOKEN = true;

        return c;
    }


    static CardInfo Cenarius()
    {
        CardInfo c = new();

        c.name = "Cenarius";
        c.text = "\nChoose one: Give your other minions +2/+2; or Summon two 2/2 Treants with Taunt.";
        c.manaCost = 9;
        c.classType = Card.Class.Druid;
        c.damage = 5;
        c.health = 8;

        c.MINION = true;
        c.LEGENDARY = true;
        c.CHOOSE = true;
        c.choices = new List<Card.Cardname>() { Card.Cardname.Cenarius_Buff, Card.Cardname.Cenarius_Treants };

        return c;
    }
    static CardInfo Cenarius_Buff()
    {
        CardInfo c = new();

        c.name = "Demigod's Favor";
        c.text = "Give your other minions +2/+2.";
        c.manaCost = 9;
        c.classType = Card.Class.Druid;

        c.SPELL = true;
        c.TARGETED = false;


        c.TOKEN = true;

        return c;
    }
    static CardInfo Cenarius_Treants()
    {
        CardInfo c = new();

        c.name = "Shan'do's Lesson";
        c.text = "Summon two 2/2 Treants with Taunt.";
        c.manaCost = 9;
        c.classType = Card.Class.Druid;

        c.SPELL = true;
        c.TARGETED = false;


        c.TOKEN = true;

        return c;
    }
}
