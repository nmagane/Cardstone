using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEffects
{
    public static void KnifeJuggler(Server.Match match, Board.Minion minion)
    {
        int damage = 1;
        Server.Player opponent = match.FindOpponent(minion);
        int tar = Random.Range(-1, opponent.board.Count());
        if (tar==-1)
        {
            match.server.DamageFace(match, opponent, damage);
            return;
        }

        //TODO: CONFIRM TRIGGER MESSAGE TO PLAYERS?
        match.server.DamageMinion(match, opponent.board[tar], damage);
    }

    public static void AcolyteOfPain(Server.Match match,Board.Minion minion)
    {
        minion.AddAura(new Board.Minion.Aura(Board.Minion.Aura.Type.Damage, 2));
    }
}
