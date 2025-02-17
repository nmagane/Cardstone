using System.Data.Common;
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

        AnimationManager.AnimationInfo anim = new AnimationManager.AnimationInfo
        {
            card = Card.Cardname.Knife_Juggler,
            sourceIsHero = false,
            sourceIsFriendly = true,
            sourceIndex = minion.index,
            targetIndex = tar,
            targetIsFriendly = false,
            targetIsHero = tar == -1,
        };
        match.server.ConfirmAnimation(match, minion.player, anim);

        if (tar == -1)
        {
            match.server.DamageFace(match, opponent, damage, minion.player);
            return;
        }
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

        Minion sac = match.server.SummonMinion(match, owner, Card.Cardname.Damaged_Golem, MinionBoard.MinionSource.Summon, -1);

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
            spell.match.server.SummonToken(spell.match, spell.player, Card.Cardname.Grim_Patron, trigger.minion.index + 1);
        }
    }

    public static void DrawCard(Match match, Trigger trigger, CastInfo spell)
    {
        match.server.Draw(trigger.minion.player);
    }

    internal static void Violet_Teacher(Match match, Trigger trigger, CastInfo spell)
    {
        spell.match.server.SummonToken(spell.match, spell.player, Card.Cardname.Violet_Apprentice, trigger.minion.index + 1);
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


        if (tar == -1)
        {
            match.server.DamageFace(match, opponent, damage, opponent.opponent);
            return;
        }
        match.server.DamageMinion(match, opponent.board[tar], damage, opponent.opponent);
    }

    public static void Armorsmith(Match match, Trigger trigger, CastInfo spell)
    {
        trigger.minion.player.armor += 1;
    }
    
    public static void Unstable_Ghoul(Match match, Trigger trigger, CastInfo spell)
    {
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

    

}
