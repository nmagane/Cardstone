using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

public partial class Server
{

    public partial class Match
    {
        public enum Phase
        {
            OnPlayCard,
            AfterPlayCard,

            OnPlayMinion,
            OnSummonMinion,
            AfterSummonMinion,

            OnPlaySpell,
            OnCastSpell,
            AfterSpell,

            BeforeAttack,
            Attack,
            AfterAttack,
        }
        public void StartSequencePlayMinion(CastInfo cast)
        {
            server.SummonMinion(this, cast.player, cast.card.card, cast.position);

            StartPhase(Phase.OnPlayCard, ref cast);
            StartPhase(Phase.OnPlayMinion, ref cast);

            if (cast.card.BATTLECRY)
            {
                    
            }

            StartPhase(Phase.AfterPlayCard, ref cast);
            StartPhase(Phase.AfterSummonMinion, ref cast);
        }
        
        public CastInfo StartPhase(Phase phase, ref CastInfo spell)
        {
            List<Board.Minion> triggeredMinions = new List<Board.Minion>();
            //todo: secrets
            foreach (Board.Minion minion in players[0].board)
            {
                //TODO: CHECK FOR TRIGGERS
            }
            foreach (Board.Minion minion in players[1].board)
            {
                //TODO: TRIGGERS
            }

            ResolveTriggerQueue(ref spell);

            UpdateAuras();

            return spell;
        }

        public List<Board.Trigger> triggerQueue = new List<Board.Trigger>();
        public List<Board.Trigger> triggerBuffer = new List<Board.Trigger>();
        public void ResolveTriggerQueue(ref CastInfo spell)
        {
            while (triggerQueue.Count>0)
            {
                if (triggerBuffer.Count>0)
                {
                    //Sort by playorder and add to resolve queue
                    triggerBuffer.Sort((x, y) => y.PlayOrder.CompareTo(x.PlayOrder));
                    foreach (var t in triggerBuffer)
                    {
                        triggerBuffer.Insert(0, t);
                    }
                }

                //triggerQueue[0].Activate();
                triggerQueue.Remove(triggerQueue[0]);
            }
        }

        void UpdateAuras()
        {
            foreach (Board.Minion minion in players[0].board)
            {
                //Process Auras
            }
            foreach (Board.Minion minion in players[1].board)
            {
                //Process Auras
            }

            foreach (Board.Minion minion in players[0].board)
            {
                server.UpdateMinion(this, minion);
                if (minion.health <= 0) server.DestroyMinion(this, minion);
            }
            foreach (Board.Minion minion in players[1].board)
            {
                server.UpdateMinion(this, minion);
                if (minion.health <= 0) server.DestroyMinion(this, minion);
            }
        }
    }
}
