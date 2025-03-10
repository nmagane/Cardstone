using System.Collections.Generic;

public partial class Server
{
    void Shapeshift(CastInfo spell)
    {
        spell.player.AddAura(new Aura(Aura.Type.Damage, 1, true));
        spell.player.armor += 1;
    }
    void Innervate(CastInfo spell)
    {
        spell.player.currMana += 2;
    }
    void Wild_Growth(CastInfo spell)
    {
        if (spell.player.maxMana<10 && spell.player.currMana<10)
        {
            spell.player.maxMana++;
        }
        else
        {
            AddCard(spell.match, spell.player, Card.Cardname.Excess_Mana);
        }
    }
    void Excess_Mana(CastInfo spell)
    {
        Draw(spell.player);
    }
    void Savage_Roar(CastInfo spell)
    {
        var anim = new AnimationInfo(Card.Cardname.Savage_Roar, spell.player);
        spell.player.AddAura(new Aura(Aura.Type.Damage, 2, true));
        foreach (Minion m in spell.player.board)
        {
            AddAura(spell.match, m, new Aura(Aura.Type.Damage, 2, true,false,null,Card.Cardname.Savage_Roar));
        }
    }

    void Wrath(CastInfo spell)
    {

        var anim = new AnimationInfo(Card.Cardname.Wrath, spell.player,spell);
        if (spell.choice == 0)
        {
            DamageTarget(3, spell);
        }
        if (spell.choice == 1)
        {
            DamageTarget(1, spell);
            spell.match.ResolveTriggerQueue(ref spell);
            Draw(spell.player);
        }
    }
    
    void Ancient_of_War(CastInfo spell)
    {
        if (spell.choice == 0)
        {
            AddAura(spell.match, spell.minion, new Aura(Aura.Type.Damage, 5));
        }
        if (spell.choice == 1)
        {
            AddAura(spell.match, spell.minion, new Aura(Aura.Type.Health, 5));
            AddAura(spell.match, spell.minion, new Aura(Aura.Type.Taunt));
        }
    }
    void Keeper_of_the_Grove(CastInfo spell)
    {
        if (spell.choice == 0)
        {
            var anim = new AnimationInfo(Card.Cardname.Keeper_of_the_Grove, spell.player, spell.minion,spell);
            DamageTarget(2, spell);
        }
        if (spell.choice == 1)
        {
            SilenceMinion(spell);
        }
    }
    void Druid_of_the_Flame(CastInfo spell)
    {
        if (spell.choice == 0)
        {
            TransformMinion(spell.match, spell.minion, Card.Cardname.Druid_of_the_Flame_Attack);
        }
        if (spell.choice == 1)
        {
            TransformMinion(spell.match, spell.minion, Card.Cardname.Druid_of_the_Flame_Health);
        }
    }
    void Druid_of_the_Claw(CastInfo spell)
    {
        if (spell.choice == 0)
        {
            TransformMinion(spell.match, spell.minion, Card.Cardname.Druid_of_the_Claw_Charge);
        }
        if (spell.choice == 1)
        {
            TransformMinion(spell.match, spell.minion, Card.Cardname.Druid_of_the_Claw_Taunt);
        }
    }
    void Ancient_of_Lore(CastInfo spell)
    {
        if (spell.choice == 0)
        {
            Draw(spell.player);
            Draw(spell.player);
        }
        if (spell.choice == 1)
        {
            HealTarget(5,spell);
        }
    }
    void Force_of_Nature(CastInfo spell)
    {
        for (int i=0;i<3;i++)
        {
            SummonToken(spell.match, spell.player, Card.Cardname.Treant_Charge);
        }
    }
    void Cenarius(CastInfo spell)
    {
        if (spell.choice == 0)
        {
            foreach (Minion m in spell.player.board)
            {
                if (m == spell.minion) continue;

                AddAura(spell.match, m, new Aura(Aura.Type.Health, 2));
                AddAura(spell.match, m, new Aura(Aura.Type.Damage, 2));

            }
        }
        if (spell.choice == 1)
        {
            SummonToken(spell.match, spell.player, Card.Cardname.Treant_Taunt,spell.minion.index);
            SummonToken(spell.match, spell.player, Card.Cardname.Treant_Taunt,spell.minion.index+1);
         
        }
    }

    void Swipe(CastInfo spell)
    {
        var anim = new AnimationInfo(Card.Cardname.Swipe, spell.player,spell);

        DamageTarget(4, spell);
        if (spell.isHero)
        {
            foreach (Minion m in spell.targetPlayer.board)
            {
                Damage(m, 1, spell);
            }
        }
        else
        {
            Minion target = spell.targetMinion;
            Damage(target.player, 1, spell);
            foreach (Minion m in target.player.board)
            {
                if (m == target) continue;
                Damage(m, 1, spell);
            }
        }
    }

    void Naturalize(CastInfo spell)
    {
        Minion m = spell.GetTargetMinion();
        AnimationInfo anim = new AnimationInfo(Card.Cardname.Naturalize, spell.player, spell);
        m.DEAD = true;
        Draw(spell.player.opponent, 2);
    }

    void Bite(CastInfo spell)
    {
        spell.player.AddAura(new Aura(Aura.Type.Damage, 4, true));
        spell.player.armor += 4;
    }

    void Healing_Touch(CastInfo spell)
    {
        HealTarget(8,spell);
    }

    void Starfall(CastInfo spell)
    {
        if (spell.choice == 0)
        {
            AnimationInfo anim = new AnimationInfo(Card.Cardname.Starfall_Single, spell.player, spell);
            DamageTarget(5, spell);
        }
        if (spell.choice == 1)
        {
            int damage = 2;
            Player opp = spell.match.Opponent(spell.player);

            AnimationInfo anim = new AnimationInfo(Card.Cardname.Starfall_AoE, spell.player);
            foreach (var m in opp.board)
            {
                Damage(m, damage, spell);
            }
        }
    }

    void Starfire(CastInfo spell)
    {
        AnimationInfo anim = new AnimationInfo(Card.Cardname.Starfire, spell.player, spell);
        DamageTarget(5, spell);

        spell.match.midPhase = true;
        spell.match.ResolveTriggerQueue(ref spell);
        Draw(spell.player);
        spell.match.midPhase = false;
    }

    void Tree_of_Life(CastInfo spell)
    {
        Player p = spell.player;
        Player opp = spell.player.opponent;

        Heal(p,p.maxHealth - p.health,spell);
        Heal(opp,opp.maxHealth - opp.health,spell);

        MinionBoard[] boards = { opp.board, p.board };
        foreach (MinionBoard board in boards)
        {
            foreach (Minion m in board)
            {
                Heal(m, m.maxHealth - m.health, spell);
            }
        }
        
    }

    void Poison_Seeds(CastInfo spell)
    {

        MinionBoard pb = spell.player.board;
        MinionBoard ob = spell.player.opponent.board;
        int p_count = pb.Count();
        int o_count = ob.Count();

        if (p_count + o_count > 0)
        {
            //don't waste sending anim data without targets present
            AnimationInfo anim = new AnimationInfo(Card.Cardname.Poison_Seeds, spell.player);
        }

        foreach (MinionBoard board in new List<MinionBoard>(){pb,ob})
        {
            foreach (Minion m in board)
            {
                m.DEAD = true;
            }
        }

        spell.match.ResolveTriggerQueue(ref spell);

        for (int i = 0; i < p_count; i++)
        {
            spell.match.server.SummonToken(spell.match, spell.player, Card.Cardname.Treant, i);
        }

        for (int i = 0; i < o_count; i++)
        {
            spell.match.server.SummonToken(spell.match, spell.player.opponent, Card.Cardname.Treant, i);
        }
    }

    void Grove_Tender(CastInfo spell)
    {
        spell.match.midPhase = true;
        foreach (Player p in new List<Player>(){spell.player,spell.player.opponent})
        {
            if (spell.choice == 0)
            {
                if (p.maxMana<10)
                {
                    p.maxMana++;
                    if (p.currMana < 10)
                        p.currMana++;
                }
                else if (p.currMana<10)
                {
                    p.currMana++;
                }
            }

            if (spell.choice == 1)
            {
                Draw(p);
            }
        }
        spell.match.midPhase = false;
    }

    void Anodized_Robo_Cub(CastInfo spell)
    {
        if (spell.choice == 0)
        {
            AddAura(spell.match, spell.minion, new Aura(Aura.Type.Damage, 1));
        }
        if (spell.choice == 1)
        {
            AddAura(spell.match, spell.minion, new Aura(Aura.Type.Health, 1));
        }
    }
}
