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
        Debug.Log("juggle " + tar);
        if (tar==-1)
        {
            match.server.DamageFace(match, opponent, damage);
            return;
        }
        match.server.DamageMinion(match, opponent.board[tar], damage);
    }
}
