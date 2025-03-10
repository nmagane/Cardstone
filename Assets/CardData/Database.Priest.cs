public partial class Database
{
    static CardInfo Lesser_Heal()
    {
        CardInfo c = new();

        c.classType = Card.Class.Priest;
        c.name = "Lesser Heal";
        c.text = "Restore 2 Health.";

        c.manaCost = 2;

        c.SPELL = true;
        c.TARGETED = true;
        c.eligibleTargets = Board.EligibleTargets.AllCharacters;
        c.TOKEN = true;

        return c;
    }
    static CardInfo Silence()
    {
        CardInfo c = new();

        c.classType = Card.Class.Priest;
        c.name = "Silence";
        c.text = "Silence target minion.";

        c.manaCost = 0;

        c.SPELL = true;
        c.TARGETED = true;
        c.eligibleTargets = Board.EligibleTargets.AllMinions;

        return c;
    }
    static CardInfo Northshire_Cleric()
    {
        CardInfo c = new();

        c.classType = Card.Class.Priest;
        c.name = "Northshire Cleric";
        c.text = "Whenever a minion is Healed, draw a card.";

        c.manaCost = 1;

        c.damage = 1;
        c.health = 3;

        c.MINION = true;

        c.triggers.Add((Trigger.Type.OnMinionHealed, Trigger.Side.Both, Trigger.Ability.DrawCard));

        return c;
    }
    static CardInfo Circle_of_Healing()
    {
        CardInfo c = new();

        c.classType = Card.Class.Priest;
        c.name = "Circle of Healing";
        c.text = "Restore 4 Health to all minions.";

        c.manaCost = 0;

        c.SPELL = true;
        c.TARGETED = false;

        return c;
    }
    static CardInfo Holy_Smite()
    {
        CardInfo c = new();

        c.classType = Card.Class.Priest;
        c.name = "Holy Smite";
        c.text = "Deal {0} damage.";
        c.spellDamage = 2;

        c.manaCost = 1;

        c.SPELL = true;
        c.TARGETED = true;
        c.eligibleTargets = Board.EligibleTargets.AllCharacters;

        return c;
    }
    static CardInfo Light_of_the_Naaru()
    {
        CardInfo c = new();

        c.classType = Card.Class.Priest;
        c.name = "Light of the Naaru";
        c.text = "Restore 3 Health. If target is still damaged, summon a Lightwarden.";

        c.manaCost = 1;

        c.SPELL = true;
        c.TARGETED = true;
        c.eligibleTargets = Board.EligibleTargets.AllCharacters;

        return c;
    }
    static CardInfo Velens_Chosen()
    {
        CardInfo c = new();

        c.classType = Card.Class.Priest;
        c.name = "Velen's Chosen";
        c.text = "Give a minion +2/+4 and +1 Spellpower.";

        c.manaCost = 3;

        c.SPELL = true;
        c.TARGETED = true;
        c.eligibleTargets = Board.EligibleTargets.AllCharacters;

        return c;
    }
    static CardInfo Resurrect()
    {
        CardInfo c = new();

        c.classType = Card.Class.Priest;
        c.name = "Resurrect";
        c.text = "Summon a random friendly minion that died this game.";

        c.manaCost = 2;

        c.SPELL = true;
        c.TARGETED = false;

        return c;
    }
    static CardInfo Power_Word_Shield()
    {
        CardInfo c = new();

        c.classType = Card.Class.Priest;
        c.name = "Power Word: Shield";
        c.text = "Give a minion +2 Health. Draw a card.";

        c.manaCost = 1;

        c.SPELL = true;
        c.TARGETED = true;
        c.eligibleTargets = Board.EligibleTargets.AllMinions;

        return c;
    }
    static CardInfo Inner_Fire()
    {
        CardInfo c = new();

        c.classType = Card.Class.Priest;
        c.name = "Inner Fire";
        c.text = "Set target minion's attack equal to its Health.";

        c.manaCost = 1;

        c.SPELL = true;
        c.TARGETED = true;
        c.eligibleTargets = Board.EligibleTargets.AllMinions;

        return c;
    }
    static CardInfo Divine_Spirit()
    {
        CardInfo c = new();

        c.classType = Card.Class.Priest;
        c.name = "Divine Sprit";
        c.text = "Double target minon's Health.";

        c.manaCost = 2;

        c.SPELL = true;
        c.TARGETED = true;
        c.eligibleTargets = Board.EligibleTargets.AllMinions;

        return c;
    }
    static CardInfo Mind_Blast()
    {
        CardInfo c = new();

        c.classType = Card.Class.Priest;
        c.name = "Mind Blast";
        c.text = "Deal {0} damage to enemy hero.";
        c.spellDamage = 5;

        c.manaCost = 2;

        c.SPELL = true;
        c.TARGETED = false;

        return c;
    }

    static CardInfo Shadow_Word_Pain()
    {
        CardInfo c = new();

        c.classType = Card.Class.Priest;
        c.name = "Shadow Word: Pain";
        c.text = "Destroy target minion with 3 or less attack.";

        c.manaCost = 2;

        c.SPELL = true;
        c.TARGETED = true;
        c.eligibleTargets = Board.EligibleTargets.Attack_Under_4;

        return c;
    }
    static CardInfo Shadow_Word_Death()
    {
        CardInfo c = new();

        c.classType = Card.Class.Priest;
        c.name = "Shadow Word: Death";
        c.text = "Destroy target minion with 5 or more attack.";

        c.manaCost = 3;

        c.SPELL = true;
        c.TARGETED = true;
        c.eligibleTargets = Board.EligibleTargets.Attack_Over_4;

        return c;
    }
    static CardInfo Thoughtsteal()
    {
        CardInfo c = new();

        c.classType = Card.Class.Priest;
        c.name = "Thoughtsteal";
        c.text = "Copy 2 cards from your opponent's deck into your hand.";

        c.manaCost = 3;

        c.SPELL = true;
        c.TARGETED = false;

        return c;
    }
    static CardInfo Shadow_of_Nothing()
    {
        CardInfo c = new();

        c.classType = Card.Class.Priest;
        c.name = "Shadow of Nothing";
        c.text = "Your opponent has no cards left!";

        c.manaCost = 1;
        c.health = 1;
        c.damage = 0;

        c.MINION = true;
        c.TOKEN = true;

        return c;
    }

    static CardInfo Holy_Nova()
    {
        CardInfo c = new();

        c.classType = Card.Class.Priest;
        c.name = "Holy Nova";
        c.text = "Deal {0} damage to all enemies. Restore 2 Health to all allies.";
        c.spellDamage = 2;

        c.manaCost = 5;

        c.SPELL = true;
        c.TARGETED = false;

        return c;
    }
    static CardInfo Lightbomb()
    {
        CardInfo c = new();

        c.classType = Card.Class.Priest;
        c.name = "Lightbomb";
        c.text = "Deal damage to all enemy minions equal to their Attack.";

        c.manaCost = 6;

        c.SPELL = true;
        c.TARGETED = false;

        return c;
    }
    static CardInfo Dark_Cultist()
    {
        CardInfo c = new();

        c.classType = Card.Class.Priest;
        c.name = "Dark Cultist";
        c.text = "Deathrattle: Give a random friendly minion +3 Health.";

        c.manaCost = 3;
        c.damage = 3;
        c.health = 4;

        c.MINION = true;

        c.triggers.Add((Trigger.Type.Deathrattle, Trigger.Side.Both, Trigger.Ability.Dark_Cultist));

        return c;
    }
    static CardInfo Auchenai_Soulpriest()
    {
        CardInfo c = new();

        c.classType = Card.Class.Priest;
        c.name = "Auchenai Soulpriest";
        c.text = "Your healing deals damage instead.";

        c.manaCost = 4;
        c.damage = 3;
        c.health = 5;

        c.MINION = true;

        c.auras.Add(Aura.Type.Auchenai_Soulpriest);

        return c;
    }
    static CardInfo Cabal_Shadow_Priest()
    {
        CardInfo c = new();

        c.classType = Card.Class.Priest;
        c.name = "Cabal Shadow Priest";
        c.text = "Battlecry: Take control of enemy minion with 2 or less Attack.";

        c.manaCost = 6;
        c.damage = 4;
        c.health = 5;

        c.MINION=true;
        c.BATTLECRY = true;
        c.TARGETED = true;
        c.eligibleTargets = Board.EligibleTargets.Cabal_Shadow_Priest;

        return c;
    }
}
