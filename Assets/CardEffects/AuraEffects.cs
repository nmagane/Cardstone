using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AuraEffects
{
    public static void StormwindChampion(Server.Match match, Board.Minion sourceMinion)
    {
        Server.Player owner = match.FindOwner(sourceMinion);
        foreach (var m in owner.board)
        {
            if (m == sourceMinion)
            {
                continue;
            }
            match.server.AddAura(match,m,new Board.Minion.Aura(Board.Minion.Aura.Type.Health, 2, false, true, sourceMinion));
            match.server.AddAura(match,m,new Board.Minion.Aura(Board.Minion.Aura.Type.Damage, 2, false, true, sourceMinion));
        }
    }
    public static void DireWolfAlpha(Server.Match match, Board.Minion sourceMinion)
    {
        Server.Player owner = match.FindOwner(sourceMinion);
        foreach (var m in owner.board)
        {
            if (m.index == sourceMinion.index - 1 || m.index == sourceMinion.index + 1)
                match.server.AddAura(match, m, new Board.Minion.Aura(Board.Minion.Aura.Type.Damage, 1, false, true, sourceMinion));
        }
    }
}
