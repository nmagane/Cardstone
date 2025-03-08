using System.Data.Common;
using UnityEngine;

public partial class Server
{
    void Lifetap(CastInfo spell)
    {
        AnimationInfo anim = new AnimationInfo(Card.Cardname.Lifetap, spell.player, spell.player);
        
        Damage(spell.player, 2, spell);
        spell.match.ResolveTriggerQueue(ref spell);
        Draw(spell.player, 1);
    }

    void Flame_Imp(CastInfo spell)
    {
        AnimationInfo anim = new AnimationInfo(Card.Cardname.Flame_Imp, spell.player,spell.minion,spell.player);

        Damage(spell.player, 3, spell);
    }

    void Soulfire(CastInfo spell)
    {
        AnimationInfo anim = new AnimationInfo(Card.Cardname.Soulfire, spell.player, spell);
        DamageTarget(4, spell);
        spell.match.ResolveTriggerQueue(ref spell);
        Discard(spell, 1);
    }
    void Mortal_Coil(CastInfo spell)
    {
        if (spell.targetMinion == null) return; 
        AnimationInfo anim = new AnimationInfo(Card.Cardname.Mortal_Coil, spell.player, spell);

        DamageTarget(1, spell);

        spell.match.ResolveTriggerQueue(ref spell);

        if (spell.targetMinion.DEAD || spell.targetMinion.health<0)
        {
            Draw(spell.player);
        }

    }
    void Power_Overwhelming(CastInfo spell)
    {
        if (spell.targetMinion == null) return;
        Minion m = spell.targetMinion;
        spell.match.server.AddAura(spell.match, m, new Aura(Aura.Type.Damage, 4, true, false, null, Card.Cardname.Power_Overwhelming));
        spell.match.server.AddAura(spell.match, m, new Aura(Aura.Type.Health, 4, true, false, null, Card.Cardname.Power_Overwhelming));
        spell.match.server.AddTrigger(spell.match, m, Trigger.Type.EndTurn, Trigger.Side.Friendly, Trigger.Ability.Power_Overwhelming);

    }

    void Implosion(CastInfo spell)
    {
        if (spell.targetMinion == null) return;
        AnimationInfo anim = new AnimationInfo(Card.Cardname.Implosion, spell.player, spell);

        int damage = Random.Range(2, 5);

        DamageTarget(damage, spell);

        damage += spell.player.spellpower;

        spell.match.ResolveTriggerQueue(ref spell);

        for (int i=0;i<damage;i++)
        {
            SummonToken(spell.match, spell.player, Card.Cardname.Imp);
        }
    }

    void Darkbomb(CastInfo spell)
    {
        AnimationInfo anim = new AnimationInfo(Card.Cardname.Darkbomb, spell.player, spell);
        DamageTarget(3, spell);
    }
    void Hellfire(CastInfo spell)
    {
        AnimationInfo anim = new AnimationInfo(Card.Cardname.Hellfire, spell.player);
        int damage = 3;

        MinionBoard b = spell.player.board;
        MinionBoard b2 = spell.player.opponent.board;

        Damage(spell.player, damage, spell);
        Damage(spell.player.opponent, damage, spell);
        foreach (Minion m in b)
        {
            Damage(m, damage, spell);
        }
        foreach (Minion m in b2)
        {
            Damage(m, damage, spell);
        }
    }
    void Shadowflame(CastInfo spell)
    {
        Minion m = spell.GetTargetMinion();
        if (m == null) return;
        AnimationInfo anim = new AnimationInfo(Card.Cardname.Shadowflame, spell.player, spell.targetMinion, spell.player.opponent);

        foreach (Minion x in spell.player.opponent.board)
        {
            Damage(x, m.damage, spell);
        }

        spell.match.ResolveTriggerQueue(ref spell);

        m.DEAD = true;
    }
    void Siphon_Soul(CastInfo spell)
    {
        Minion m = spell.GetTargetMinion();
        if (m == null) return;
        AnimationInfo anim = new AnimationInfo(Card.Cardname.Siphon_Soul, spell.player, spell.targetMinion);

        m.DEAD = true;

        Heal(spell.player, 3, spell);

    }


    void Inferno(CastInfo spell)
    {
        SummonToken(spell.match, spell.player, Card.Cardname.Infernal);
    }
    void Lord_Jarraxus(CastInfo spell)
    {
        RemoveMinion(spell.match, spell.minion);

        spell.player.maxHealth = 15;
        spell.player.health = 15;
        ReplaceHero(spell.player, Card.Cardname.Lord_Jarraxus);
        ReplaceHeroPower(spell.player, Card.Cardname.Inferno);
        SummonWeapon(spell.match, spell.player, Card.Cardname.Blood_Fury);

    }

}
