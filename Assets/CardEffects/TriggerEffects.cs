using System.Collections.Generic;
using System.Data.Common;
using Mirror.BouncyCastle.Bcpg;
using Mirror.Examples.CharacterSelection;
using UnityEngine;

public class TriggerEffects
{
    public static void KnifeJuggler(Match match, Minion minion)
    {
        int damage = 1;
        Player opponent = match.FindOpponent(minion);
        int tar = Random.Range(-1, opponent.board.Count());

        if (tar != -1)
        {
            while (opponent.board[tar].health <= 0 || opponent.board[tar].DEAD)
            {
                tar = Random.Range(-1, opponent.board.Count());
                if (tar == -1) break;
            }
        }
        
        

        if (tar == -1)
        {
            AnimationInfo animFace = new AnimationInfo(Card.Cardname.Knife_Juggler, minion.player, minion, opponent);
            match.server.DamageFace(match, opponent, damage, minion.player);
            return;
        }
        AnimationInfo anim = new AnimationInfo(Card.Cardname.Knife_Juggler, minion.player, minion, opponent.board[tar]);
        match.server.DamageMinion(match, opponent.board[tar], damage, minion.player);
    }

    public static void AcolyteOfPain(Match match, Minion minion)
    {
        match.server.Draw(minion.player);
    }

    public static void YoungPriestess(Match match, Minion minion)
    {
        Player p = match.FindOwner(minion);

        //Skip trigger if no targets available
        if (p.board.Count() == 0) return;
        if (p.board.Count() == 1 && p.board[0] == minion) return;

        Minion m = p.board[Random.Range(0, p.board.Count())];
        //Cant target self
        while (m == minion) m = p.board[Random.Range(0, p.board.Count())];

        match.server.AddAura(match, m, new Aura(Aura.Type.Health, 1));
    }

    public static void HarvestGolem(Match match, Minion minion)
    {
        match.server.SummonToken(match, match.FindOwner(minion), Card.Cardname.Damaged_Golem, minion.index);
    }

    public static void Emperor_Thaurissan(Match match, Minion minion)
    {
        Player p = minion.player;
        foreach (HandCard c in p.hand)
        {
            match.server.AddCardAura(match, c, new Aura(Aura.Type.Cost, -1));
        }
    }
    public static void Loatheb(Match match, Minion minion, Trigger t)
    {
        Player p = minion.player;
        p.AddAura(new Aura(Aura.Type.Loatheb, 0, true));
        p.RemoveTrigger(t);
    }
    public static void Millhouse(Match match, Minion minion, Trigger t)
    {
        Player p = minion.player;
        p.AddAura(new Aura(Aura.Type.Millhouse, 0, true));
        p.RemoveTrigger(t);
    }

    public static void Prep_Cast(Match m, Minion minion, Trigger t)
    {
        Player p = minion.player;
        p.RemoveAura(Aura.Type.Preparation);
    }
    public static void Archmage_Antonidas(Match m, Minion minion, Trigger t)
    {
        Player p = minion.player;
        HandCard c = m.server.AddCard(m, p, Card.Cardname.Fireball, minion);
    }
    public static void Ice_Barrier(Match m, Minion minion, Trigger t)
    {
        Player p = minion.player;
        if (m.currPlayer == p) return;

        m.server.TriggerSecret(t.secret);

        p.armor += 8;

    }
    public static void Ice_Block(Match m, Minion minion, Trigger t)
    {
        Player p = minion.player;
        if (m.currPlayer == p) return;

        m.server.TriggerSecret(t.secret);

        p.AddAura(new Aura(Aura.Type.Immune, 0, true));

    }
    public static void Noble_Sacrifice(Match match, Trigger trigger, CastInfo spell)
    {
        Player owner = trigger.minion.player;
        if (owner.board.Count() >= 7) return;

        match.server.TriggerSecret(trigger.secret);

        Minion sac = match.server.SummonToken(match, trigger.player, Card.Cardname.Damaged_Golem);
        if (sac == null) return;
        spell.attack.faceAttack = false;
        spell.attack.target = sac;
    }
    public static void Warsong_Commander(Match match, Trigger trigger, CastInfo spell)
    {
        if (spell.minion.damage <= 3)
        {
            spell.match.server.AddAura(spell.match, spell.minion, new Aura(Aura.Type.Charge));
        }
    }
    public static void Grim_Patron(Match match, Trigger trigger, CastInfo spell)
    {
        if (trigger.minion.health > 0 && trigger.minion.DEAD == false)
        {
            trigger.minion.player.match.server.SummonToken(trigger.minion.player.match, trigger.minion.player, Card.Cardname.Grim_Patron, trigger.minion.index + 1);
        }
    }

    public static void DrawCard(Match match, Trigger trigger, CastInfo spell)
    {
        match.server.Draw(trigger.minion.player);
    }

    internal static void Violet_Teacher(Match match, Trigger trigger, CastInfo spell)
    {
        trigger.match.server.SummonToken(trigger.match, trigger.minion.player, Card.Cardname.Violet_Apprentice, trigger.minion.index + 1);
    }

    public static void Boom_Bot(Match match, Trigger trigger, CastInfo spell)
    {
        int damage = Random.Range(1,5);
        Player opponent = trigger.minion.player.opponent;

        int tar = Random.Range(-1, opponent.board.Count());

        if (tar != -1)
        {
            while (opponent.board[tar].health <= 0 || opponent.board[tar].DEAD)
            {
                tar = Random.Range(-1, opponent.board.Count());
                if (tar == -1) break;
            }
        }

        int ind = (trigger.player.board.Count() > trigger.minion.index) ? trigger.minion.index : -1;

        if (tar == -1)
        {
            if (ind != -1)
            {
                var animFace = new AnimationInfo(Card.Cardname.Boom_Bot, trigger.player, trigger.player.board[ind], opponent);
            }
            else
            {
                var animFace = new AnimationInfo(Card.Cardname.Boom_Bot, trigger.player, opponent);
            }
            match.server.DamageFace(match, opponent, damage, opponent.opponent);
            return;
        }
        if (ind != -1)
        {
            var animFace = new AnimationInfo(Card.Cardname.Boom_Bot, trigger.player, trigger.player.board[ind], opponent.board[tar]);
        }
        else
        {
            var animFace = new AnimationInfo(Card.Cardname.Boom_Bot, trigger.player, opponent.board[tar]);
        }
        match.server.DamageMinion(match, opponent.board[tar], damage, opponent.opponent);
    }

    public static void Armorsmith(Match match, Trigger trigger, CastInfo spell)
    {
        trigger.minion.player.armor += 1;
    }
    
    public static void Unstable_Ghoul(Match match, Trigger trigger, CastInfo spell)
    {
        if (trigger.minion.card==Card.Cardname.Deaths_Bite)
        {
            var anim = new AnimationInfo(Card.Cardname.Deaths_Bite,trigger.player);
        }
        else
        {
            var anim = new AnimationInfo(Card.Cardname.Unstable_Ghoul, trigger.player);
        }
        MinionBoard b = trigger.minion.player.board;
        MinionBoard b2 = trigger.minion.player.opponent.board;
        foreach (Minion m in b)
        {
            match.server.DamageMinion(match, m, 1, trigger.minion.player);
        }
        foreach (Minion m in b2)
        {
            match.server.DamageMinion(match, m, 1, trigger.minion.player);
        }
    }

    public static void Frothing_Berserker(Match match, Trigger trigger, CastInfo spell)
    {
        Minion minion = trigger.minion;
        match.server.AddAura(match, minion, new Aura(Aura.Type.Damage, 1));
    }
    public static void Doomsayer(Match match, Trigger trigger, CastInfo spell)
    {
        MinionBoard b = trigger.minion.player.board;
        MinionBoard b2 = trigger.minion.player.opponent.board;
        foreach (Minion m in b)
        {
            m.DEAD = true;
        }
        foreach (Minion m in b2)
        {
            m.DEAD = true;
        }
    }
    public static void Mad_Scientist(Match match, Trigger trigger, CastInfo spell)
    {
        Player p = trigger.minion.player;
        Card.Cardname secret = Card.Cardname.Cardback;
        foreach (Card.Cardname c in p.deck)
        {
            if (Database.GetCardData(c).SECRET)
            {
                if (p.HasSecret(c) == false)
                {
                    match.server.AddSecret(c, p, match);
                    secret = c;
                    break;
                }
            }
        }
        if (secret != Card.Cardname.Cardback)
            p.deck.Remove(secret);
    }
    public static void Power_Overwhelming(Match match, Trigger trigger, CastInfo spell)
    {
        trigger.minion.DEAD = true;
    }
    public static void Imp_Gang_Boss(Match match, Trigger trigger, CastInfo spell)
    {
        trigger.match.server.SummonToken(trigger.minion.player.match, trigger.minion.player, Card.Cardname.Imp, trigger.minion.index + 1);
    }
    public static void Haunted_Creeper(Match match, Trigger trigger, CastInfo spell)
    {
        match.server.SummonToken(match, trigger.minion.player, Card.Cardname.Spectral_Spider, trigger.minion.index);
        match.server.SummonToken(match, trigger.minion.player, Card.Cardname.Spectral_Spider, trigger.minion.index);
    }
    public static void Nerubian_Egg(Match match, Trigger trigger)
    {
        match.server.SummonToken(match, trigger.minion.player, Card.Cardname.Nerubian, trigger.minion.index);
    }
    public static void Voidcaller(Match match, Trigger trigger)
    {
        List<HandCard> demons = new List<HandCard>();
        foreach (HandCard card in trigger.minion.player.hand)
        {
            if (card.tribe == Card.Tribe.Demon)
                demons.Add(card);
        }
        if (demons.Count == 0) return;
        HandCard c = Board.RandElem(demons);
        match.server.DiscardCard(match, trigger.minion.player, c.index);
        match.server.SummonToken(match, trigger.minion.player, c.card);
    }

    public static void Sludge_Belcher(Match match, Trigger trigger)
    {
        match.server.SummonToken(match, trigger.minion.player, Card.Cardname.Putrid_Slime, trigger.minion.index);
    }
    public static void Zombie_Chow(Match match, Trigger trigger)
    {
        match.server.HealFace(match, trigger.player.opponent, 5);
    }
    public static void Shade_of_Naxxrammas(Match match, Trigger trigger)
    {
        match.server.AddAura(match, trigger.minion, new Aura(Aura.Type.Health,1,false,false,null,Card.Cardname.Shade_of_Naxxrammas));
        match.server.AddAura(match, trigger.minion, new Aura(Aura.Type.Damage,1,false,false,null,Card.Cardname.Shade_of_Naxxrammas));
    }
    
    public static void Sylvanas_Windrunner(Match match, Trigger trigger)
    {
        Player enemy = trigger.player.opponent;
        if (enemy.board.Count() == 0) return;
        Minion m = Board.RandElem(enemy.board.minions);

        match.server.StealMinion(match, trigger.player, m);
    }

}
