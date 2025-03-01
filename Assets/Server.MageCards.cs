using System.Collections.Generic;

public partial class Server
{
    void Ping(CastInfo spell)
    {
        AnimationInfo anim = new AnimationInfo(Card.Cardname.Ping, spell.player, spell);
        int damage = 1;
        DamageTarget(damage, spell);
    }
    void Polymorph(CastInfo spell)
    {
        if (spell.GetTargetMinion() == null) return;

        AnimationInfo anim = new AnimationInfo(Card.Cardname.Polymorph, spell.player, spell);
        TransformMinion(spell.match, spell.GetTargetMinion(), Card.Cardname.Sheep);
    }

    private void Frostbolt(CastInfo spell)
    {
        AnimationInfo anim = new AnimationInfo(Card.Cardname.Frostbolt, spell.player, spell);

        Minion m = null;
        if (!spell.isHero)
            m = spell.GetTargetMinion();

        int damage = 3;
        DamageTarget(damage, spell);

        spell.match.ResolveTriggerQueue(ref spell);

        if (spell.isHero)
        {
            Player p = spell.isFriendly ? spell.player : spell.player.opponent;
            p.AddAura(new Aura(Aura.Type.Freeze));
        }
        else
        {
            spell.match.server.AddAura(spell.match, m, new Aura(Aura.Type.Freeze));
        }
    }
    
    private void Ice_Lance(CastInfo spell)
    {
        AnimationInfo anim = new AnimationInfo(Card.Cardname.Ice_Lance, spell.player, spell);

        if (spell.isHero)
        {
            if (spell.targetPlayer.HasAura(Aura.Type.Freeze))
            {
                DamageTarget(4, spell);
            }
            else
                spell.targetPlayer.AddAura(new Aura(Aura.Type.Freeze));
        }
        else
        {
            if (spell.targetMinion.HasAura(Aura.Type.Freeze))
            {
                DamageTarget(4, spell);
            }
            else
                spell.match.server.AddAura(spell.match, spell.targetMinion, new Aura(Aura.Type.Freeze));
        }
    }
    private void Frost_Nova(CastInfo spell)
    {

        AnimationInfo anim = new AnimationInfo(Card.Cardname.Frost_Nova, spell.player);
        foreach (Minion m in spell.player.opponent.board)
        {
            spell.match.server.AddAura(spell.match, m, new Aura(Aura.Type.Freeze));
        }
    }
    private void Arcane_Intellect(CastInfo spell)
    {
        for (int i = 0; i < 2; i++)
            Draw(spell.player);
    }
    private void Fireball(CastInfo spell)
    {
        AnimationInfo anim = new AnimationInfo(Card.Cardname.Fireball, spell.player, spell);
        DamageTarget(6, spell);
    }
    private void Pyroblast(CastInfo spell)
    {
        AnimationInfo anim = new AnimationInfo(Card.Cardname.Pyroblast, spell.player, spell);
        DamageTarget(10, spell);
    }
    void Flamestrike(CastInfo spell)
    {
        int damage = 4;

        Player opp = spell.match.Opponent(spell.player);

        AnimationInfo anim = new AnimationInfo(Card.Cardname.Flamestrike, spell.player);
        foreach (var m in opp.board)
        {
            Damage(m, damage, spell);
        }

    }
    void Blizzard(CastInfo spell)
    {
        int damage = 2;

        Player opp = spell.match.Opponent(spell.player);
        AnimationInfo anim = new AnimationInfo(Card.Cardname.Blizzard, spell.player);
        foreach (var m in opp.board)
        {
            Damage(m, damage, spell);
        }

        spell.match.midPhase = true;
        spell.match.ResolveTriggerQueue(ref spell);
        spell.match.midPhase = false;

        foreach (var m in opp.board)
        {
            spell.match.server.AddAura(spell.match, m, new Aura(Aura.Type.Freeze));
        }

    }
}
