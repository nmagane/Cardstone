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
            while (opponent.board[tar].health <= 0)
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

        if (tar==-1)
        {
            match.server.DamageFace(match, opponent, damage);
            return;
        }
        match.server.DamageMinion(match, opponent.board[tar], damage);
    }

    public static void AcolyteOfPain(Match match,Minion minion)
    {
        match.server.AddAura(match, minion, new Aura(Aura.Type.Damage, 2));
    }

    public static void YoungPriestess(Match match, Minion minion)
    {
        Player p = match.FindOwner(minion);

        //Skip trigger if no targets available
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
        foreach(HandCard c in p.hand)
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
}
