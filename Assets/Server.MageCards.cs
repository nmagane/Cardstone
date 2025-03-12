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

    void Arcane_Missiles(CastInfo spell)
    {
        int damage = 3;
        Player opp = spell.match.Opponent(spell.player);
        List<Minion> minions = new List<Minion>();

        spell.match.midPhase = true;
        
        for (int i = 0; i < damage; i++)
        {
            minions.AddRange(opp.board.minions);
            minions.Add(opp.sentinel);

            List<Minion> removes = new List<Minion>();
            foreach (Minion x in minions)
            {
                if ((x.PLAYER==false && (x.DEAD || x.health <= 0)) || (x.PLAYER && x.player.health <= 0))
                    removes.Add(x);
            }
            foreach (Minion x in removes)
                minions.Remove(x);

            Minion m = Board.RandElem(minions);
            
            if (m.PLAYER)
            {
                var animFace = new AnimationInfo(Card.Cardname.Keeper_of_the_Grove,spell.player,spell.player.opponent);
                Damage(m.player, 1, spell);
            }
            else
            {
                var anim = new AnimationInfo(Card.Cardname.Keeper_of_the_Grove, spell.player, m);
                Damage(m, 1, spell);
            }
            
            spell.match.ResolveTriggerQueue(ref spell);
        }

        spell.match.midPhase = false;
    }

    void Arcane_Explosion(CastInfo spell)
    {
        int damage = 1;
        Player opp = spell.match.Opponent(spell.player);

        AnimationInfo anim = new AnimationInfo(Card.Cardname.Fan_of_Knives, spell.player);

        foreach (var m in opp.board)
        {
            Damage(m, damage, spell);
        }
    }

    void Mirror_Image(CastInfo spell)
    {
        spell.match.midPhase = true;
        for (int i = 0; i < 2; i++)
        {
            if (spell.player.board.GetCount() >= 7) return;
            spell.match.server.SummonToken(spell.match, spell.player, Card.Cardname.Mirror_Image_Token, spell.player.board.GetCount());
        }
        spell.match.midPhase = false;
    }

    void Cone_of_Cold(CastInfo spell)
    {
        int damage = 1;
        Player opp = spell.match.Opponent(spell.player);

        AnimationInfo anim = new AnimationInfo(Card.Cardname.Cone_of_Cold, spell.player, spell);

        Minion target = spell.GetTargetMinion();

        if (target != null)
        {
            Damage(target, damage, spell);
            spell.match.server.AddAura(spell.match, target, new Aura(Aura.Type.Freeze));

            if (target.index > 0)
            {
                Damage(opp.board[target.index - 1], damage, spell);
                spell.match.server.AddAura(spell.match, opp.board[target.index - 1], new Aura(Aura.Type.Freeze));
            }
            if (target.index < opp.board.GetCount() - 1)
            {
                Damage(opp.board[target.index + 1], damage, spell);
                spell.match.server.AddAura(spell.match, opp.board[target.index + 1], new Aura(Aura.Type.Freeze));
            }
        }
    }
}
