using System.Data.Common;
using UnityEngine;

public partial class Server
{
    void Lifetap(CastInfo spell)
    {
        AnimationManager.AnimationInfo anim = new AnimationManager.AnimationInfo
        {
            card = Card.Cardname.Lifetap,
            sourceIsHero = true,
            sourceIsFriendly = true,
            targetIsHero = true,
            targetIsFriendly = true,
        };

        ConfirmAnimation(spell.match, spell.player, anim);

        Damage(spell.player, 2, spell);
        spell.match.ResolveTriggerQueue(ref spell);
        Draw(spell, 1);
    }

    void Flame_Imp(CastInfo spell)
    {
        AnimationManager.AnimationInfo anim = new AnimationManager.AnimationInfo
        {
            card = Card.Cardname.Flame_Imp,
            sourceIsHero = false,
            sourceIsFriendly = true,
            sourceIndex = spell.minion.index,
            targetIndex = -1,
            targetIsFriendly = true,
            targetIsHero = true,
        };

        ConfirmAnimation(spell.match, spell.player, anim);

        Damage(spell.player, 3, spell);
    }

    void Soulfire(CastInfo spell)
    {
        AnimationManager.AnimationInfo anim = new AnimationManager.AnimationInfo
        {
            card = Card.Cardname.Soulfire,
            sourceIsHero = true,
            sourceIsFriendly = true,
            sourceIndex = -1,
            targetIndex = spell.targetMinion == null ? -1 : spell.targetMinion.index,
            targetIsFriendly = spell.isFriendly,
            targetIsHero = spell.isHero,
        };

        ConfirmAnimation(spell.match, spell.player, anim);

        DamageTarget(4, spell);
        spell.match.ResolveTriggerQueue(ref spell);
        Discard(spell, 1);
    }
    void Mortal_Coil(CastInfo spell)
    {
        if (spell.targetMinion == null) return;
        AnimationManager.AnimationInfo anim = new AnimationManager.AnimationInfo
        {
            card = Card.Cardname.Soulfire,
            sourceIsHero = true,
            sourceIsFriendly = true,
            sourceIndex = -1,
            targetIndex = spell.targetMinion == null ? -1 : spell.targetMinion.index,
            targetIsFriendly = spell.isFriendly,
            targetIsHero = spell.isHero,
        };

        ConfirmAnimation(spell.match, spell.player, anim);

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
        AnimationManager.AnimationInfo anim = new AnimationManager.AnimationInfo
        {
            card = Card.Cardname.Soulfire,
            sourceIsHero = true,
            sourceIsFriendly = true,
            sourceIndex = -1,
            targetIndex = spell.targetMinion == null ? -1 : spell.targetMinion.index,
            targetIsFriendly = spell.isFriendly,
            targetIsHero = spell.isHero,
        };

        ConfirmAnimation(spell.match, spell.player, anim);

        int damage = Random.Range(2, 5);


        DamageTarget(damage, spell);

        damage += spell.player.spellpower;

        spell.match.ResolveTriggerQueue(ref spell);

        for (int i=0;i<damage;i++)
        {
            SummonToken(spell.match, spell.player, Card.Cardname.Imp);
        }
    }

    }
