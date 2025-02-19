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
    private void SI7_Agent(CastInfo spell)
    {
        if (spell.player.combo == false) return;
        var anim = new AnimationInfo(Card.Cardname.SI7_Agent, spell.player, spell.minion, spell);
                                
        DamageTarget(2, spell);
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
        spell.player.weapon.AddAura(new Aura(Aura.Type.Damage, 2));
    }
    private void Blade_Flurry(CastInfo spell)
    {
        if (spell.player.weapon == null) return;
        spell.player.weapon.DEAD = true;
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
        foreach (Minion m in b)
        {
            Damage(m, 1, spell);
        }

        spell.match.ResolveTriggerQueue(ref spell);

        Draw(spell.player);
    }
    private void Tinkers_Oil(CastInfo spell)
    {
        if (spell.player.weapon != null) 
            spell.player.weapon.AddAura(new Aura(Aura.Type.Damage, 3));

        //there are no possible triggers that trigger on "addaura" right?
        //spell.match.ResolveTriggerQueue(ref spell);

        if (spell.combo)
        {
            List<Minion> minions = spell.player.board.minions;
            Minion m = Board.RandElem(minions);
            while ((m.DEAD || m.health<m.maxHealth) && minions.Count>0)
            {
                minions.Remove(m);
                m = Board.RandElem(minions);
            }
            spell.match.server.AddAura(spell.match, m, new Aura(Aura.Type.Damage, 3));
        }
    }
    void Sprint(CastInfo spell)
    {
        for (int i = 0; i < 4; i++)
            Draw(spell.player);
    }
}
