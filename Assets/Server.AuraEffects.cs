using UnityEngine;

public partial class Server
{
    public static class AuraEffects
    {

        public static void StormwindChampion(Match match, Board.Minion sourceMinion)
        {
            Player owner = match.FindOwner(sourceMinion);
            foreach (var m in owner.board)
            {
                if (m==sourceMinion)
                {
                    Debug.Log("skipped " + m.card);
                    continue;
                }
                m.AddAura(new Board.Minion.Aura(Board.Minion.Aura.Type.Health, 2, false, true, sourceMinion));
                m.AddAura(new Board.Minion.Aura(Board.Minion.Aura.Type.Damage, 2, false, true, sourceMinion));
                Debug.Log("Aura hits"+ m.card);
            }
        }
        public static void DireWolfAlpha(Match match, Board.Minion sourceMinion)
        {
            Player owner = match.FindOwner(sourceMinion);
            foreach (var m in owner.board)
            {
                if (m.index == sourceMinion.index - 1 || m.index == sourceMinion.index + 1)
                    m.AddAura(new Board.Minion.Aura(Board.Minion.Aura.Type.Damage, 1, false, true, sourceMinion));
            }
        }

    }

}
