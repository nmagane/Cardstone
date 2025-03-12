using System.Collections.Generic;
using UnityEngine;

public partial class Server
{

    public void HealTarget(int heal, CastInfo spell)
    {

        if (spell.player.HasAura(Aura.Type.AUCH_PLAYER_BUFF))
        {
            DamageTarget(heal, spell);
            return;
        }

        if (spell.targetMinion == null && spell.targetPlayer == null) return;
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
        if (spell.player.HasAura(Aura.Type.AUCH_PLAYER_BUFF))
        {
            Damage(target, heal, spell);
            return;
        }
        HealMinion(spell.match, target, heal,spell.player);
    }
    public void Heal(Player target, int heal, CastInfo spell)
    {
        if (spell.player.HasAura(Aura.Type.AUCH_PLAYER_BUFF))
        {
            Damage(target, heal, spell);
            return;
        }
        HealFace(spell.match, target, heal,spell.player);
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

    public void Draw(Player p,int count=1)
    {
        for (int i=0;i<count;i++)
            p.match.StartSequenceDrawCard(new CastInfo(p.match,p,null,-1,-1,false,false));
    }

    public void SilenceMinion(CastInfo spell)
    {
        Player p = spell.player;
        Match match = spell.match;
        Minion minion = spell.GetTargetMinion();
        if (minion == null) return;
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

    void Coldlight_Oracle(CastInfo spell)
    {
        spell.match.midPhase = true;
        Draw(spell.player, 2);
        Draw(spell.player.opponent, 2);
        spell.match.midPhase = false;
    }

    void Youthful_Brewmaster(CastInfo spell)
    {
        Minion m = spell.GetTargetMinion();
        if (m == null) return;
        
        var anim = new AnimationInfo(Card.Cardname.Youthful_Brewmaster, spell.player, spell.minion,spell);

        spell.match.server.AddCard(spell.match, m.player, m.card, m, 0);
        spell.match.server.RemoveMinion(spell.match, m);
    }

    void King_Mukla(CastInfo spell)
    {
        for (int i = 0; i < 2; i++)
        {
            spell.match.server.AddCard(spell.match, spell.player.opponent, Card.Cardname.Bananas, null, 0);
        }
    }

    void Bananas(CastInfo spell)
    {
        if (spell.targetMinion == null) return;
        Minion m = spell.targetMinion;
        spell.match.server.AddAura(spell.match, m, new Aura(Aura.Type.Damage, 1, false, false, null, Card.Cardname.Bananas));
        spell.match.server.AddAura(spell.match, m, new Aura(Aura.Type.Health, 1, false, false, null, Card.Cardname.Bananas));

    }
    
    void Leeroy_Jenkins(CastInfo spell)
    {
        spell.match.midPhase = true;
        for (int i = 0; i < 2; i++)
        {
            if (spell.player.opponent.board.GetCount() >= 7) return;
            spell.match.server.SummonToken(spell.match, spell.player.opponent, Card.Cardname.Whelp, spell.player.opponent.board.GetCount());
        }
        spell.match.midPhase = false;
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
        DamageTarget(3, spell);
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
    void Mind_Control_Tech(CastInfo spell)
    {
        Player enemy = spell.player.opponent;
        if (enemy.board.GetCount() < 4) return;

        Minion m = Board.RandElem(enemy.board.minions);

        spell.match.server.StealMinion(spell.match, spell.player, m);
    }
    void Injured_Blademaster(CastInfo spell)
    {
        Damage(spell.minion, 4, spell);
    }
    void Bloodsail_Raider(CastInfo spell)
    {
        if (spell.player.weapon!=null)
        {
            AddAura(spell.match, spell.minion, new Aura(Aura.Type.Damage, spell.player.weapon.damage));
        }
    }

    public void Captains_Parrot(CastInfo spell)
    {
        Player p = spell.player;
        Card.Cardname card = Card.Cardname.Cardback;
        foreach (Card.Cardname c in p.deck)
        {
            if (Database.GetCardData(c).tribe==Card.Tribe.Pirate)
            {
                card = c;
                break;
            }
        }
        if (card != Card.Cardname.Cardback)
        {
            p.deck.Remove(card);
            if (spell.player.hand.Count() < 10) AddCard(spell.match, spell.player, card);
            else
            {
                p.deck.Insert(0, card);
                MillCard(spell.match,spell.player);
            }

        }
    }

    public void Mad_Bomber(CastInfo spell)
    {
        List<Minion> minions = new List<Minion>();
        minions.AddRange(spell.player.board.minions);
        minions.AddRange(spell.player.opponent.board.minions);
        minions.Add(spell.player.sentinel);
        minions.Add(spell.player.opponent.sentinel);

        minions.Remove(spell.minion); //cant hit self

        spell.match.midPhase = true;

        for (int i = 0; i < 3; i++)
        {
            minions = new List<Minion>();
            minions.AddRange(spell.player.board.minions);
            minions.AddRange(spell.player.opponent.board.minions);
            minions.Add(spell.player.sentinel);
            minions.Add(spell.player.opponent.sentinel);
            
            minions.Remove(spell.minion); //cant hit self

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
                var animFace = new AnimationInfo(Card.Cardname.Boom_Bot, spell.player, spell.minion, m.player);
                Damage(m.player, 1, spell);
            }
            else
            {
                var anim = new AnimationInfo(Card.Cardname.Boom_Bot, spell.player, spell.minion, m);
                Damage(m, 1, spell);
            }

            spell.match.ResolveTriggerQueue(ref spell);
        }

        spell.match.midPhase = false;
    }

    void Murloc_Tidehunter(CastInfo spell)
    {
        SummonToken(spell.match, spell.player, Card.Cardname.Murloc_Scout, spell.minion.index + 1);
    }
    void Echoing_Ooze(CastInfo spell)
    {
        AddTrigger(spell.match, spell.minion, Trigger.Type.EndTurn, Trigger.Side.Friendly, Trigger.Ability.Echoing_Ooze);
    }
}
