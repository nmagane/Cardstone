using System.Collections.Generic;
using UnityEngine;

public partial class Server
{

    public void HealTarget(CastInfo spell, int heal)
    {
        if (spell.isHero)
        {
            Player player = spell.isFriendly ? spell.player : spell.player.opponent;
            //HealFace(spell.match, player, damage);
            return;
        }
        Minion minion = spell.GetTargetMinion();
        HealMinion(spell.match, minion, heal);
    }
    public void DamageTarget(CastInfo spell, int damage)
    {
        if (spell.isHero)
        {
            Player player = spell.isFriendly ? spell.player : spell.player.opponent;
            DamageFace(spell.match, player, damage);
            return;
        }
        Minion minion = spell.GetTargetMinion();
        DamageMinion(spell.match, minion, damage);

    }
    public void Discard(CastInfo spell, int count = 1, bool enemyDiscard = false)
    {
        Player player = enemyDiscard ? spell.player.opponent : spell.player;
        for (int i = 0; i < count; i++)
        {
            int rand = Random.Range(0, player.hand.Count());
            spell.match.server.DiscardCard(spell.match, player, rand);
        }
    }
    public void Draw(CastInfo spell, int count, bool enemyDraw = false)
    {
        for (int i = 0; i < count; i++)
        {
            if (enemyDraw) spell.player = spell.player.opponent;
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
            //if (a.foreignSource && a.source != minion) continue;
            if (a.foreignSource) continue;
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
        foreach (var m in spell.player.board)
        {
            if (m.index == spell.position - 1 || m.index == spell.position + 1)
            {
                match.server.AddAura(match, m, new Aura(Aura.Type.Health, 1));
                match.server.AddAura(match, m, new Aura(Aura.Type.Damage, 1));
                match.server.AddAura(match, m, new Aura(Aura.Type.Taunt));
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
        DamageTarget(spell, 3);
    }

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

        DamageTarget(spell, 2);
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

        DamageTarget(spell, 4);
        spell.match.ResolveTriggerQueue(ref spell);
        Discard(spell, 1);
    }

    void Loatheb(CastInfo spell)
    {
        Player opponent = spell.player.opponent;
        opponent.AddTrigger(Trigger.Type.StartTurn, Trigger.Side.Friendly, Trigger.Ability.Loatheb, spell.minion.playOrder);
    }
    void Millhouse_Manastorm(CastInfo spell)
    {
        Player opponent = spell.player.opponent;
        opponent.AddTrigger(Trigger.Type.StartTurn, Trigger.Side.Friendly, Trigger.Ability.Millhouse, spell.minion.playOrder);
    }

    void Preparation(CastInfo spell)
    {
        spell.player.AddAura(new Aura(Aura.Type.Preparation, 0, true));
        spell.player.AddTrigger(Trigger.Type.OnPlaySpell, Trigger.Side.Friendly, Trigger.Ability.Preparation_Cast, spell.playOrder);
    }

    void Hunters_Mark(CastInfo spell)
    {
        Minion m = spell.GetTargetMinion();
        SetHealth(spell.match, m, 1);
    }

    void Crazed_Alchemist(CastInfo spell)
    {
        Minion m = spell.GetTargetMinion();
        int att = m.damage;
        int hp = m.health;
        SetDamage(spell.match, m, hp);
        SetHealth(spell.match, m, att);
    }
    private void Heroic_Strike(CastInfo spell)
    {
        spell.player.AddAura(new Aura(Aura.Type.Damage, 4, true));
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
        foreach(Minion m in b)
        {
            DamageMinion(spell.match, m, dmg);
        }
        DamageFace(spell.match, spell.player.opponent, dmg);
    }
    private void Armor_Up(CastInfo spell)
    {
        spell.player.armor += 2;
    }
    private void SI7_Agent(CastInfo spell)
    {
        if (spell.player.combo == false) return;
        DamageTarget(spell, 2);
    }
    private void Eviscerate(CastInfo spell)
    {
        int dmg = 2;
        if (spell.player.combo) dmg = 4;
        DamageTarget(spell, dmg);
    }
    private void Sap(CastInfo spell)
    {
        Minion m = spell.GetTargetMinion();
        spell.match.server.AddCard(spell.match, m.player, m.card, m, 0);
        spell.match.server.RemoveMinion(spell.match,m);
    }
}
