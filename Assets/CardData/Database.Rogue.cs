using Mirror.BouncyCastle.Asn1.X509;

public partial class Database
{
    static CardInfo Backstab()
    {
        CardInfo c = new();

        c.name = "Backstab";
        c.text = "Deal {0} damage to an undamaged minion.";

        c.classType = Card.Class.Rogue;
        c.eligibleTargets = Board.EligibleTargets.HealthyMinions;

        c.manaCost = 0;
        c.spellDamage = 2;

        c.SPELL = true;
        c.TARGETED = true;
        return c;
    }
    static CardInfo Preparation()
    {
        CardInfo c = new();

        c.name = "Preparation";
        c.text = "The next spell you cast this turn costs 3 less.";

        c.classType = Card.Class.Rogue;

        c.manaCost = 0;

        c.SPELL = true;
        c.TARGETED = false;
        return c;
    }

    static CardInfo Dagger_Mastery()
    {
        CardInfo c = new();

        c.name = "Dagger Mastery";
        c.text = "Equip a 1/2 Dagger.";

        c.classType = Card.Class.Rogue;

        c.manaCost = 2;

        c.SPELL = true;
        c.TARGETED = false;
        c.TOKEN = true;

        return c;
    }
    static CardInfo Dagger()
    {
        CardInfo c = new();

        c.name = "Dagger";
        c.text = "";

        c.classType = Card.Class.Rogue;

        c.manaCost = 2;
        c.damage = 1;
        c.health = 2;

        c.WEAPON = true;
        c.TOKEN = true;

        return c;
    }

    static CardInfo Deadly_Poison()
    {
        CardInfo c = new();

        c.name = "Deadly Poison";
        c.text = "Give your weapon +2 attack.";

        c.classType = Card.Class.Rogue;
        c.eligibleTargets = Board.EligibleTargets.Weapon;

        c.manaCost = 1;

        c.SPELL = true;
        c.TARGETED = false;
        return c;
    }
    static CardInfo Blade_Flurry()
    {
        CardInfo c = new();

        c.name = "Blade Flurry";
        c.text = "Destroy your weapon and deal its damage to all enemies.";

        c.classType = Card.Class.Rogue;
        c.eligibleTargets = Board.EligibleTargets.Weapon;

        c.manaCost = 2;

        c.SPELL = true;
        c.TARGETED = false;
        return c;
    }
    static CardInfo SI7_Agent()
    {
        CardInfo c = new();

        c.name = "SI:7 Agent";
        c.text = "Combo: Deal 2 damage to target.";

        c.classType = Card.Class.Rogue;
        c.eligibleTargets = Board.EligibleTargets.AllCharacters;

        c.manaCost = 3;

        c.damage = 3;
        c.health = 3;

        c.comboSpellDamage = 2;

        c.MINION = true;
        c.COMBO = true;
        c.COMBO_TARGETED = true;
        return c;
    }
    static CardInfo Eviscerate()
    {
        CardInfo c = new();

        c.name = "Eviscerate";
        c.text = "Deal {0} damage. Combo: Deal {1} instead.";
        c.spellDamage = 2;
        c.comboSpellDamage = 4;

        c.classType = Card.Class.Rogue;
        c.eligibleTargets = Board.EligibleTargets.AllCharacters;

        c.manaCost = 2;

        c.SPELL = true;
        c.TARGETED = true;
        c.COMBO = true;
        return c;
    }
    static CardInfo Shiv()
    {
        CardInfo c = new();

        c.name = "Shiv";
        c.text = "Deal {0} damage.\nDraw a card.";
        c.spellDamage = 1;

        c.classType = Card.Class.Rogue;
        c.eligibleTargets = Board.EligibleTargets.AllCharacters;

        c.manaCost = 2;

        c.SPELL = true;
        c.TARGETED = true;
        return c;
    }
    static CardInfo Sap()
    {
        CardInfo c = new();

        c.name = "Sap";
        c.text = "Return an enemy minion to your opponent's hand.";

        c.classType = Card.Class.Rogue;
        c.eligibleTargets = Board.EligibleTargets.EnemyMinions;

        c.manaCost = 2;

        c.SPELL = true;
        c.TARGETED = true;
        return c;
    }
    static CardInfo Shadowstep()
    {
        CardInfo c = new();

        c.name = "Shadowstep";
        c.text = "Return a friendly minion to your hand.\nIt costs 2 less.";

        c.classType = Card.Class.Rogue;
        c.eligibleTargets = Board.EligibleTargets.FriendlyMinions;

        c.manaCost = 0;

        c.SPELL = true;
        c.TARGETED = true;

        return c;
    }

    static CardInfo Fan_of_Knives()
    {
        CardInfo c = new();

        c.name = "Fan of Knives";
        c.text = "Deal {0} damage to all enemy minions.\nDraw a card.";

        c.classType = Card.Class.Rogue;

        c.manaCost = 3;
        c.spellDamage = 1;

        c.SPELL = true;
        c.TARGETED = false;
        return c;
    }
    static CardInfo Sprint()
    {
        CardInfo c = new();

        c.name = "Sprint";
        c.text = "Draw 4 cards.";

        c.classType = Card.Class.Rogue;

        c.manaCost = 7;

        c.SPELL = true;
        c.TARGETED = false;
        return c;
    }
    static CardInfo Tinkers_Oil()
    {
        CardInfo c = new();

        c.name = "Tinker's Sharpsword Oil";
        c.text = "Give your\nweapon +3 attack.\nCombo: Give a random friendly minion +3 attack.";

        c.classType = Card.Class.Rogue;

        c.manaCost = 4;

        c.SPELL = true;
        c.COMBO = true;
        c.TARGETED = false;
        return c;
    }
    static CardInfo Edwin_VanCleef()
    {
        CardInfo c = new();

        c.name = "Edwin VanCleef";
        c.text = "Combo: Gain +2/+2 for each card other card you've played this turn.";

        c.classType = Card.Class.Rogue;

        c.manaCost = 3;
        c.damage = 2;
        c.health = 2;

        c.MINION = true;
        c.COMBO = true;
        c.LEGENDARY = true;

        return c;
    }
    private static CardInfo Goblin_Auto_Barber()
    {
        CardInfo c = new();

        c.name = "Goblin Auto-Barber";
        c.text = "Battlecry: Give your weapon +1 Attack.";

        c.manaCost = 2;
        c.damage = 3;
        c.health = 2;

        c.MINION = true;
        c.BATTLECRY = true;
        c.classType = Card.Class.Rogue;
        c.tribe = Card.Tribe.Mech;

        return c;
    }
    private static CardInfo One_eyed_Cheat()
    {
        CardInfo c = new();

        c.name = "One-eyed Cheat";
        c.text = "Whenever you summon a pirate, gain Stealth.";

        c.manaCost = 2;
        c.damage = 4;
        c.health = 1;

        c.tribe = Card.Tribe.Pirate;
        c.MINION = true;
        c.triggers.Add((Trigger.Type.OnPlayMinion, Trigger.Side.Friendly, Trigger.Ability.One_Eyed_Cheat));
        c.triggers.Add((Trigger.Type.AfterSummonMinion, Trigger.Side.Friendly, Trigger.Ability.One_Eyed_Cheat));

        return c;
    }
    private static CardInfo Defias_Ringleader()
    {
        CardInfo c = new();

        c.name = "Defias Ringleader";
        c.text = "Combo: Summon a 2/1 Defias Bandit.";

        c.manaCost = 2;
        c.damage = 2;
        c.health = 2;
        c.classType = Card.Class.Rogue;

        c.MINION = true;
        c.COMBO = true;

        return c;
    }
    private static CardInfo Defias_Bandit()
    {
        CardInfo c = new();

        c.name = "Defias Bandit";
        c.text = "";

        c.manaCost = 1;
        c.damage = 2;
        c.health = 1;
        c.classType = Card.Class.Rogue;

        c.MINION = true;
        c.TOKEN = true;

        return c;
    }
    private static CardInfo Patient_Assassin()
    {
        CardInfo c = new();

        c.name = "Patient Assassin";
        c.text = "Stealth.\nPoisonous.";

        c.manaCost = 2;
        c.damage = 1;
        c.health = 1;
        c.classType = Card.Class.Rogue;

        c.MINION = true;
        c.auras.Add(Aura.Type.Stealth);
        c.auras.Add(Aura.Type.Poisonous);

        return c;
    }
}
