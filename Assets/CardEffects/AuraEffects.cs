using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AuraEffects
{
    public static void StormwindChampion(Match match, Minion sourceMinion)
    {
        Player owner = match.FindOwner(sourceMinion);
        foreach (var m in owner.board)
        {
            if (m == sourceMinion)
            {
                continue;
            }
            match.server.AddAura(match,m,new Aura(Aura.Type.Health, 2, false, true, sourceMinion));
            match.server.AddAura(match,m,new Aura(Aura.Type.Damage, 2, false, true, sourceMinion));
        }
    }
    public static void DireWolfAlpha(Match match, Minion sourceMinion)
    {
        Player owner = match.FindOwner(sourceMinion);
        foreach (var m in owner.board)
        {
            if (m.index == sourceMinion.index - 1 || m.index == sourceMinion.index + 1)
                match.server.AddAura(match, m, new Aura(Aura.Type.Damage, 1, false, true, sourceMinion));
        }
    }

    public static void Amani(Match match, Minion sourceMinion)
    {
        match.server.AddAura(match, sourceMinion, new Aura(Aura.Type.Damage, 3, false, true, sourceMinion));
    }

    public static void Mana_Wraith(Match match, Minion sourceMinion)
    {
        Player owner = match.FindOwner(sourceMinion);
        foreach (Player p in match.players)
        {
            foreach (HandCard c in p.hand)
                if (c.MINION)
                    match.server.AddCardAura(match, c, new Aura(Aura.Type.Cost, 1, false, true, sourceMinion));
        }    
    }
}
