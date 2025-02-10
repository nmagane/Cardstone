using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
public partial class Server
{
    public static void RefreshAttackCharge(Minion m)
    {
        m.canAttack = true;
    }
    public static void ConsumeAttackCharge(Minion m)
    {
        //if (m.WINDFURY) m.WINDFURY = false;
        //else
        m.canAttack = false;
    }
    public void DamageMinionsAOE()
    {
        //TODO: AOE EFFECTS DONT ACTIVATE TRIGGERS UNTIL ALL UNITS TAKE DAMAGE

    }

    public void HealMinion(Match match, Minion minion, int heal)
    {
        if (minion.health<minion.maxHealth)
        {
            //HEAL TRIGGERS HERE
        }
        minion.health = Mathf.Min(minion.health+heal,minion.maxHealth);

        //this is for the client to know if its not an aura change
        match.healedMinions.Add(minion);
    }

    public void DamageMinion(Match match, Minion minion, int damage)
    {
        if (minion.HasAura(Aura.Type.Shield))
        {
            match.server.RemoveAura(match,minion,minion.FindAura(Aura.Type.Shield));
            return;
        }
        minion.health -= damage;

        match.TriggerMinion(Trigger.Type.OnDamageTaken,minion);
        match.AddTrigger(Trigger.Type.OnMinionDamage, null, minion);

        //this is for the client to know if its not an aura change
        match.damagedMinions.Add(minion);
    }


    public void DamageFace(Match match, Player target, int damage)
    {
        target.health -= damage;

        match.AddTrigger(Trigger.Type.OnFaceDamage, null, target);

        //this is for the client to know if its not an aura change
        match.damagedPlayers.Add(target);
    }
    
    public void HealFace(Match match, Player target, int heal)
    {
        target.health -= heal;

        match.AddTrigger(Trigger.Type.OnFaceDamage, null, target);

        //this is for the client to know if its not an aura change
        match.healedPlayers.Add(target);
    }

    public void FatiguePlayer(Match match, Player target)
    {
        target.fatigue++;

        DamageFace(match, target, target.fatigue);
    }

    public bool ExecuteAttack(ref CastInfo action)
    {
        bool success = ExecuteAttackLogic(ref action);
        if (success) ConfirmAttackGeneral(action);
        return success;
    }

    bool ExecuteAttackLogic(ref CastInfo action)
    {
        AttackInfo attack = action.attack;
        Match match = action.match;

        if (attack.faceAttack)
        {
            Player targetPlayer = attack.friendlyFire ? attack.player : attack.player.opponent;
            if (attack.weaponSwing)
            {
                //Face to face
                //Check for failed attack
                if (attack.player.health<=0)
                {
                    return false;
                }

                return true;
            }
            //Check for failed attacks
            if (attack.attacker.DEAD) 
                return false;

            //Minion to face
            ConsumeAttackCharge(attack.attacker);
            DamageFace(match, targetPlayer, attack.attacker.damage);
            return true;
        }

        //Check for failed attacked
        if (attack.weaponSwing && attack.target.DEAD)
        {
            return false;
        }
        else if (attack.attacker.DEAD || attack.target.DEAD)
        {
            return false;
        }

        //Successful attack
        if (attack.weaponSwing)
        {
            //Face to Minion
            return true;
        }
        //Minion to minion
        ConsumeAttackCharge(attack.attacker);
        DamageMinion(match, attack.target, attack.attacker.damage);
        DamageMinion(match, attack.attacker, attack.target.damage);
        return true;
    }
    public void CastSpell(CastInfo spell)
    {
        //TODO: check for target survival/validity after prespell phase. fizzle if invalid.
        switch(spell.card.card)
        {
            case Card.Cardname.Coin:
                Coin(spell);
                break;
            case Card.Cardname.Ping:
                Ping(spell);
                break;
            case Card.Cardname.Arcane_Explosion:
                Arcane_Explosion(spell);
                break;
            case Card.Cardname.Shattered_Sun_Cleric:
                ShatteredSunCleric(spell);
                break;
            case Card.Cardname.Defender_of_Argus:
                Defender_of_Argus(spell);
                break;
            case Card.Cardname.Abusive_Sergeant:
            case Card.Cardname.Dark_Iron_Dwarf:
                Abusive_Sergeant(spell);
                break;
            case Card.Cardname.Ironbeak_Owl:
                SilenceMinion(spell);
                break;
            case Card.Cardname.Voodoo_Doctor:
                Heal(spell, 2);
                break;
            case Card.Cardname.Soulfire:
                Soulfire(spell);
                break;
            case Card.Cardname.Doomguard:
                Discard(spell, 2);
                break;
            case Card.Cardname.Lifetap:
                Lifetap(spell);
                break;
            case Card.Cardname.Flame_Imp:
                Flame_Imp(spell);
                break;
            case Card.Cardname.Loatheb:
                Loatheb(spell);
                break;
            default:
                Debug.LogError("MISSING SPELL " + spell.card.card);
                break;
        }
    }

    public void Heal(CastInfo spell, int heal)
    {
        if (spell.isHero)
        {
            Player player = spell.isFriendly ? spell.player : spell.player.opponent;
            //HealFace(spell.match, player, damage);
            return;
        }
        Minion minion = spell.GetTargetMinion();
        HealMinion(spell.match, minion, heal);
        //TODO: TRIGGER ANIMATION
    }
    public void Damage(CastInfo spell, int damage)
    {
        if (spell.isHero)
        {
            Player player = spell.isFriendly ? spell.player : spell.player.opponent;
            DamageFace(spell.match, player, damage);
            return;
        }
        Minion minion = spell.GetTargetMinion();
        DamageMinion(spell.match, minion, damage);
        //TODO: TRIGGER ANIMATION

    }
    public void Discard(CastInfo spell, int count=1, bool enemyDiscard = false)
    {
        Player player = enemyDiscard ? spell.player.opponent : spell.player;   
        for (int i=0;i<count;i++)
        {
            int rand = Random.Range(0, player.hand.Count());
            spell.match.server.DiscardCard(spell.match, player, rand);
        }
    }
    public void Draw(CastInfo spell, int count, bool enemyDraw=false)
    {
        for (int i = 0; i < count; i++)
        {
            if (enemyDraw)  spell.player =spell.player.opponent;
            spell.match.StartSequenceDrawCard(spell);
        }
    }
    public void SilenceMinion(CastInfo spell)
    {
        Player p = spell.player;
        Match match = spell.match;
        if (spell.isFriendly == false) p = p.opponent;
        Minion minion = p.board[spell.target];

        List<Aura> auras = new List<Aura>(minion.auras);
        List<Trigger> triggers = new List<Trigger>(minion.triggers);
        foreach (var a in auras)
        {
            if (a.foreignSource && a.source != minion) continue;
            match.server.RemoveAura(match, minion, a);
        }
        foreach (var t in triggers)
        {
            match.server.RemoveTrigger(match, minion, t);
        }

        match.server.AddAura(match, minion, new Aura(Aura.Type.Silence));
    }
    void Coin(CastInfo spell)
    {
        spell.player.currMana++;
    }
    void Ping(CastInfo spell)
    {
        int damage = 1;

        if (spell.isHero)
        {
            if (spell.isFriendly)
                DamageFace(spell.match, spell.player, damage);
            else
                DamageFace(spell.match, spell.match.Opponent(spell.player), damage);
            return;
        }

        if (spell.isFriendly)
            DamageMinion(spell.match, spell.player.board[spell.target], damage);
        else
            DamageMinion(spell.match, spell.match.Opponent(spell.player).board[spell.target], damage);

    }

    void Arcane_Explosion(CastInfo spell)
    {
        int damage = 1;

        Player opp = spell.match.Opponent(spell.player);
        List<Minion> minions = new List<Minion>();
        foreach (var m in opp.board)
        {
            minions.Add(m);
        }

        foreach (var m in minions)
            DamageMinion(spell.match, m, damage);
    }
    void ShatteredSunCleric(CastInfo spell)
    {
        Player p = spell.player;
        Match m = spell.match;
        Minion tar = p.board[spell.target];
        //TODO: SILENCABLE AURAS
        m.server.AddAura(m, tar, new Aura(Aura.Type.Health, 1));
        m.server.AddAura(m, tar, new Aura(Aura.Type.Damage, 1));
        //p.board[spell.target].AddAura(new Aura(Aura.Type.Damage, 1));
    }
    void Abusive_Sergeant(CastInfo spell)
    {
        Player p = spell.player;
        Match m = spell.match;
        if (spell.isFriendly == false) p = p.opponent;

        Minion tar = p.board[spell.target];
        //TODO: SILENCABLE AURAS
        m.server.AddAura(m, tar, new Aura(Aura.Type.Damage, 2, true));
        //p.board[spell.target].AddAura(new Aura(Aura.Type.Damage, 2,true));
    }
    void Defender_of_Argus(CastInfo spell)
    {
        Player p = spell.player;
        Match match = spell.match;
        foreach(var m in spell.player.board)
        {
            if (m.index == spell.position-1 || m.index == spell.position+1)
            {
                match.server.AddAura(match, m, new Aura(Aura.Type.Health, 1));
                match.server.AddAura(match, m, new Aura(Aura.Type.Damage, 1));
                match.server.AddAura(match, m, new Aura(Aura.Type.Taunt));
                //m.AddAura(new Aura(Aura.Type.Taunt));
                //m.AddAura(new Aura(Aura.Type.Health, 1));
                //m.AddAura(new Aura(Aura.Type.Damage, 1));
            }
        }
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

        spell.isFriendly = true;
        spell.isHero = true;
        Damage(spell, 3);
    }

    void Lifetap(CastInfo spell)
    {
        AnimationManager.AnimationInfo anim = new AnimationManager.AnimationInfo
        {
            card = Card.Cardname.Lifetap,
            sourceIsHero =true,
            sourceIsFriendly = true,
            targetIsHero = true,
            targetIsFriendly = true,
        };

        ConfirmAnimation(spell.match, spell.player, anim);

        Damage(spell, 2);
        spell.match.ResolveTriggerQueue(ref spell);
        Draw(spell, 1);
    }

    void Soulfire(CastInfo spell)
    {
        AnimationManager.AnimationInfo anim = new AnimationManager.AnimationInfo
        {
            card = Card.Cardname.Soulfire,
            sourceIsHero = true,
            sourceIsFriendly = true,
            sourceIndex = -1,
            targetIndex = spell.target,
            targetIsFriendly = spell.isFriendly,
            targetIsHero = spell.isHero,
        };

        ConfirmAnimation(spell.match, spell.player, anim);

        Damage(spell, 4);
        spell.match.ResolveTriggerQueue(ref spell);
        Discard(spell, 1);
    }

    void Loatheb(CastInfo spell)
    {
        Player opponent = spell.player.opponent;
        opponent.AddTrigger(Trigger.Type.StartTurn, Trigger.Side.Friendly, Trigger.Ability.Loatheb, spell.minion.playOrder);
    }
}
