using System.Collections.Generic;
using UnityEngine;

public partial class Server
{

    public void HealTarget(int heal, CastInfo spell)
    {
        if (spell.isHero)
        {
            Player player = spell.isFriendly ? spell.player : spell.player.opponent;
            Heal(player, heal, spell);
            return;
        }
        Minion minion = spell.GetTargetMinion();
        Heal(minion, heal, spell);

    }
    public void Heal(Minion target, int heal, CastInfo spell)
    {
        HealMinion(spell.match, target, heal);
    }
    public void Heal(Player target, int heal, CastInfo spell)
    {
        HealFace(spell.match, target, heal);
    }

    public void DamageTarget(int damage, CastInfo spell)
    {
        if (spell.isHero)
        {
            Player player = spell.isFriendly ? spell.player : spell.player.opponent;
            Damage(player, damage, spell);
            return;
        }
        Minion minion = spell.GetTargetMinion();
        Damage(minion, damage, spell);

    }
    public void Damage(Minion target,int damage,CastInfo spell)
    {
        if (spell.card != null)
        {
            if (spell.card.SPELL)
            {
                damage += spell.player.spellpower;
            }
        }
        DamageMinion(spell.match, target, damage, spell.player);
    }
    public void Damage(Player target,int damage, CastInfo spell)
    {
        if (spell.card != null)
        {
            if (spell.card.SPELL)
            {
                damage += spell.player.spellpower;
            }
        }
        DamageFace(spell.match, target,damage,spell.player);
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

    public void Draw(Player p)
    {
        p.match.StartSequenceDrawCard(new CastInfo(p.match,p,null,-1,-1,false,false));
    }

    public void SilenceMinion(CastInfo spell)
    {
        Player p = spell.player;
        Match match = spell.match;
        Minion minion = spell.GetTargetMinion();

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

    void ShatteredSunCleric(CastInfo spell)
    {
        Player p = spell.player;
        Match m = spell.match;
        Minion tar = spell.GetTargetMinion();
        m.server.AddAura(m, tar, new Aura(Aura.Type.Health, 1));
        m.server.AddAura(m, tar, new Aura(Aura.Type.Damage, 1));
    }
    void Abusive_Sergeant(CastInfo spell)
    {
        Player p = spell.player;
        Match m = spell.match;
        Minion tar = spell.GetTargetMinion();
        m.server.AddAura(m, tar, new Aura(Aura.Type.Damage, 2, true));
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


    void Hunters_Mark(CastInfo spell)
    {
        Minion m = spell.GetTargetMinion();
        SetHealth(spell.match, m, 1);
    }

    void Crazed_Alchemist(CastInfo spell)
    {
        Minion m = spell.GetTargetMinion();
        if (m == null) return;
        var anim = new AnimationInfo(Card.Cardname.Crazed_Alchemist, spell.player, spell.minion, m);
        int att = m.damage;
        int hp = m.health;
        SetDamage(spell.match, m, hp);
        SetHealth(spell.match, m, att);
    }

    void Earthen_Ring_Farseer(CastInfo spell)
    {
        HealTarget(3, spell);
    }

    void Antique_Healbot(CastInfo spell)
    {
        Heal(spell.player,8,spell);
    }

    void Azure_Drake(CastInfo spell)
    {
        Draw(spell.player);
    }
    void Gnomish_Inventor(CastInfo spell)
    {
        Draw(spell.player);
    }

    void Dr_Boom(CastInfo spell)
    {
        spell.match.server.SummonToken(spell.match, spell.player, Card.Cardname.Boom_Bot, spell.minion.index);
        spell.match.server.SummonToken(spell.match, spell.player, Card.Cardname.Boom_Bot, spell.minion.index + 1);
    }
    
    void Alexstrasza(CastInfo spell)
    {
        if (spell.targetPlayer == null) return;
        var anim = new AnimationInfo(Card.Cardname.Alexstrasza, spell.player,spell.minion, spell.targetPlayer);
        spell.targetPlayer.health = 15;
    }
    void Blackwing_Technician(CastInfo spell)
    {
        if (spell.player.HasTribe(Card.Tribe.Dragon) == false) return;
        AddAura(spell.match, spell.minion, new Aura(Aura.Type.Health, 1));
        AddAura(spell.match, spell.minion, new Aura(Aura.Type.Damage, 1));
    }
    void Blackwing_Corruptor(CastInfo spell)
    {
        if (spell.player.HasTribe(Card.Tribe.Dragon) == false) return;
        var anim = new AnimationInfo(Card.Cardname.Blackwing_Corruptor, spell.player, spell.minion, spell);
        DamageTarget(5, spell);
    }
    void Twilight_Drake(CastInfo spell)
    {
        AddAura(spell.match, spell.minion, new Aura(Aura.Type.Health, spell.player.hand.Count()));
    }
    void Big_Game_Hunter(CastInfo spell)
    {
        if (spell.GetTargetMinion() == null) return;
        if (spell.GetTargetMinion().damage < 7) return;
        var anim = new AnimationInfo(Card.Cardname.Big_Game_Hunter, spell.player, spell.minion, spell);
        spell.GetTargetMinion().DEAD = true;
    }
    void Sunfury_Protector(CastInfo spell)
    {
        Player p = spell.player;
        Match match = spell.match;
        foreach (var m in spell.player.board)
        {
            if (m.index == spell.position - 1 || m.index == spell.position + 1)
            {
                match.server.AddAura(match, m, new Aura(Aura.Type.Taunt));
            }
        }
    }
    void Acidic_Swamp_Ooze(CastInfo spell)
    {
        if (spell.player.opponent.weapon == null) return;
        var anim = new AnimationInfo(Card.Cardname.Acidic_Swamp_Ooze, spell.player, spell.minion, spell.player.opponent);
        spell.player.opponent.weapon.DEAD = true;
    }
    void Novice_Engineer(CastInfo spell)
    {
        Draw(spell.player);
    }
    void Harrison_Jones(CastInfo spell)
    {
        if (spell.player.opponent.weapon == null) return;

        int count = spell.player.opponent.weapon.durability;

        var anim = new AnimationInfo(Card.Cardname.Acidic_Swamp_Ooze, spell.player, spell.minion, spell.player.opponent);
        spell.player.opponent.weapon.DEAD = true;

        spell.match.ResolveTriggerQueue(ref spell);

        for (int i=0;i<count;i++)
            Draw(spell.player);
    }
}
