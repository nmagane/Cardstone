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
            OnSummonMinion, //Tokens
            AfterSummonMinion,

            OnPlaySpell,
            AfterPlaySpell,

            BeforeAttack,
            AfterAttack,

            BeforeAttackFace,
            AfterAttackFace,

            StartTurn,
            EndTurn,
            CardDraw,
        }

        public void StartSequenceAttackMinion(CastInfo spell)
        {
            StartPhase(Phase.BeforeAttack, ref spell);

            bool successfulAttack = server.ExecuteAttack(ref spell);

            if (successfulAttack == false)
            {
                ResolveTriggerQueue(ref spell);
                return;
            }

            StartPhase(Phase.AfterAttack, ref spell);
        }
        public void StartSequenceAttackFace(CastInfo spell)
        {
            StartPhase(Phase.BeforeAttackFace, ref spell);

            bool successfulAttack = server.ExecuteAttack(ref spell);

            if (successfulAttack == false)
            {
                ResolveTriggerQueue(ref spell);
                return;
            }

            StartPhase(Phase.AfterAttackFace, ref spell);
        }
        public void StartSequencePlaySpell(CastInfo spell)
        {
            StartPhase(Phase.OnPlayCard, ref spell);
            StartPhase(Phase.OnPlaySpell, ref spell);

            server.CastSpell(spell);
            
            StartPhase(Phase.AfterPlayCard, ref spell);
            StartPhase(Phase.AfterPlaySpell, ref spell);
        }

        public void StartSequencePlayMinion(CastInfo spell)
        {
            server.SummonMinion(this, spell.player, spell.card.card, spell.position);

            StartPhase(Phase.OnPlayCard, ref spell);
            StartPhase(Phase.OnPlayMinion, ref spell);

            if (spell.card.BATTLECRY)
            {
                server.CastSpell(spell);
            }

            StartPhase(Phase.AfterPlayCard, ref spell);
            StartPhase(Phase.AfterSummonMinion, ref spell);
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

            List<Board.Minion> destroyList = new List<Board.Minion>();
            foreach (Board.Minion minion in players[0].board)
            {
                server.UpdateMinion(this, minion);
                if (minion.health <= 0) destroyList.Add(minion);
            }
            foreach (Board.Minion minion in players[1].board)
            {
                server.UpdateMinion(this, minion);
                if (minion.health <= 0) destroyList.Add(minion);
            }

            foreach (var m in destroyList) server.DestroyMinion(this, m);

            //TODO: update and check hero health for game over
            server.UpdateHero(this, players[0]);
            server.UpdateHero(this, players[1]);
        }
    }
}
