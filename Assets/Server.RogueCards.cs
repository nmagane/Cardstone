using System.Collections.Generic;

public partial class Server
{
    public void Dagger_Mastery(CastInfo spell)
    {
        spell.match.server.SummonWeapon(spell.match, spell.player, Card.Cardname.Dagger);
    }
    void Preparation(CastInfo spell)
    {
        spell.player.AddAura(new Aura(Aura.Type.Preparation, 0, true));
        spell.player.AddTrigger(Trigger.Type.OnPlaySpell, Trigger.Side.Friendly, Trigger.Ability.Preparation_Cast, spell.playOrder);
    }
    private void Sap(CastInfo spell)
    {
        Minion m = spell.GetTargetMinion();
        spell.match.server.AddCard(spell.match, m.player, m.card, m, 0);
        spell.match.server.RemoveMinion(spell.match, m);
    }
    private void Shadowstep(CastInfo spell)
    {
        Minion m = spell.GetTargetMinion();
        HandCard c = spell.match.server.AddCard(spell.match, m.player, m.card, m, -2);
        AddCardAura(spell.match,c,new Aura(Aura.Type.Cost, -2));
        spell.match.server.RemoveMinion(spell.match, m);
    }
    private void SI7_Agent(CastInfo spell)
    {
        if (spell.player.combo == false) return;
        var anim = new AnimationInfo(Card.Cardname.SI7_Agent, spell.player, spell.minion, spell);
                                
        DamageTarget(2, spell);
    }

    private void Shiv(CastInfo spell)
    {
        var anim = new AnimationInfo(Card.Cardname.Eviscerate, spell.player, spell);
        DamageTarget(1, spell);

        spell.match.ResolveTriggerQueue(ref spell);

        Draw(spell.player);
    }

    private void Eviscerate(CastInfo spell)
    {
        var anim = new AnimationInfo(Card.Cardname.Eviscerate, spell.player, spell);
        int dmg = 2;
        if (spell.player.combo) dmg = 4;
        DamageTarget(dmg, spell);
    }
    private void Deadly_Poison(CastInfo spell)
    {
        if (spell.player.weapon == null) return;

        var anim = new AnimationInfo(Card.Cardname.Deadly_Poison, spell.player);

        spell.player.weapon.AddAura(new Aura(Aura.Type.Damage, 2));
    }
    private void Blade_Flurry(CastInfo spell)
    {
        if (spell.player.weapon == null) return;
        spell.player.weapon.DEAD = true;

        var anim = new AnimationInfo(Card.Cardname.Blade_Flurry, spell.player);
        int dmg = spell.player.weapon.damage;

        MinionBoard b = spell.player.opponent.board;
        foreach (Minion m in b)
        {
            Damage(m, dmg, spell);
        }
        Damage(spell.player.opponent, dmg, spell);
    }

    private void Backstab(CastInfo spell)
    {
        var anim = new AnimationInfo(Card.Cardname.Backstab, spell.player, spell);
        DamageTarget(2, spell);
    }
    private void Fan_of_Knives(CastInfo spell)
    {
        MinionBoard b = spell.player.opponent.board;
        if (b.GetCount()>0)
        {
            var anim = new AnimationInfo(Card.Cardname.Fan_of_Knives, spell.player);
        }
        foreach (Minion m in b)
        {
            Damage(m, 1, spell);
        }

        spell.match.ResolveTriggerQueue(ref spell);

        Draw(spell.player);
    }
    private void Tinkers_Oil(CastInfo spell)
    {
        bool hasWeapon = spell.player.weapon != null;
        bool combo = spell.combo;
        bool hasMinions = spell.player.board.GetCount() > 0;

        if (hasWeapon)
            spell.player.weapon.AddAura(new Aura(Aura.Type.Damage, 3));

        List<Minion> minions = spell.player.board.minions;
        Minion m = null;
        if (minions.Count != 0)
        {
           m = Board.RandElem(minions);
        }

        if (combo && hasMinions)
        {
            var anim = new AnimationInfo(Card.Cardname.Tinkers_Oil, spell.player, m);
        }
        else if (hasWeapon)
        { 
            var anim = new AnimationInfo(Card.Cardname.Tinkers_Oil, spell.player);
        }

        if (combo && hasMinions)
        {
            spell.match.server.AddAura(spell.match, m, new Aura(Aura.Type.Damage, 3));
        }

    }
    void Sprint(CastInfo spell)
    {
        for (int i = 0; i < 4; i++)
            Draw(spell.player);
    }
    void Edwin_VanCleef(CastInfo spell)
    {
        if (spell.combo == false) return;
        int i = spell.player.comboCounter * 2;
        if (i>0)
        {
            AddAura(spell.match, spell.minion, new Aura(Aura.Type.Health, i, false, false, null, Card.Cardname.Edwin_VanCleef));
            AddAura(spell.match, spell.minion, new Aura(Aura.Type.Damage, i, false, false, null, Card.Cardname.Edwin_VanCleef));
        }
    }
    void Goblin_Auto_Barber(CastInfo spell)
    {
        if (spell.player.weapon == null) return;

        var anim = new AnimationInfo(Card.Cardname.Deadly_Poison, spell.player,spell.minion,spell.player);
        spell.player.weapon.AddAura(new Aura(Aura.Type.Damage, 1));
    }    
    void Defias_Ringleader(CastInfo spell)
    {
        if (spell.combo)
            SummonToken(spell.match, spell.player, Card.Cardname.Defias_Bandit, spell.minion.index + 1);
    }
}
