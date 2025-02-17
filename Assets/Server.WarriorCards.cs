public partial class Server
{

    private void Armor_Up(CastInfo spell)
    {
        spell.player.armor += 2;
    }
    private void Heroic_Strike(CastInfo spell)
    {
        spell.player.AddAura(new Aura(Aura.Type.Damage, 4, true));
    }

    void Inner_Rage(CastInfo spell)
    {
        DamageTarget(1, spell);
        spell.match.ResolveTriggerQueue(ref spell);
        Minion m = spell.GetTargetMinion();
        spell.match.server.AddAura(spell.match, m, new Aura(Aura.Type.Damage, 2,false,false,null,Card.Cardname.Inner_Rage));
    }
    void Execute(CastInfo spell)
    {
        Minion m = spell.GetTargetMinion();
        if (m.health >= m.maxHealth) return;
        m.DEAD = true;
    }
    void Whirlwind(CastInfo spell)
    {
        MinionBoard b = spell.player.board;
        MinionBoard b2 = spell.player.opponent.board;
        foreach (Minion m in b)
        {
            Damage(m, 1, spell);
        }
        foreach (Minion m in b2)
        {
            Damage(m, 1, spell);
        }
    }
    void Battle_Rage(CastInfo spell)
    {
        int count = 0;
        Player p = spell.player;
        if (p.health < p.maxHealth) count++;

        foreach(Minion m in p.board)
        {
            if (m.health < m.maxHealth) count++;
        }

        for (int i = 0; i < count; i++)
            Draw(spell.player);
    }

    void Slam(CastInfo spell)
    {
        Minion m = spell.GetTargetMinion();
        if (m == null) return;
        Damage(m, 2, spell);
        spell.match.ResolveTriggerQueue(ref spell);
        if (m.DEAD == false && m.health>0)
        {
            Draw(spell.player);
        }
    }
    void Cruel_Taskmaster(CastInfo spell)
    {
        Minion m = spell.GetTargetMinion();
        if (m == null) return;
        Damage(m, 1, spell);
        spell.match.ResolveTriggerQueue(ref spell);
        if (m.DEAD == false && m.health>0)
        {
            spell.match.server.AddAura(spell.match, m, new Aura(Aura.Type.Damage, 2, false, false, null, Card.Cardname.Cruel_Taskmaster));
        }
    }
}
