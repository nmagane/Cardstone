using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class Server
{

    public partial class Match
    {
        public enum Phase
        {
            NONE,

            OnPlayCard,
            AfterPlayCard,

            OnPlayMinion,
            OnSummonMinion, //Tokens
            AfterPlayMinion,
            AfterSummonMinion,

            OnPlaySpell,
            AfterPlaySpell,

            BeforeAttack,
            AfterAttack,

            BeforeAttackFace,
            AfterAttackFace,

            StartTurn,
            EndTurn,

            OnDrawCard,
            OnDiscardCard,
        }

        public void StartSequenceEndTurn(CastInfo spell)
        {
            StartPhase(Phase.EndTurn, ref spell);

            BetweenTurnEvents(spell);
        }

        public void BetweenTurnEvents(CastInfo spell)
        {
            Player player = spell.player;
            Player opponent = player.opponent;

            foreach(var m in player.board)
            {
                m.RemoveTemporaryAuras();
            }
            foreach(var m in opponent.board)
            {
                m.RemoveTemporaryAuras();
            }

            ResolveTriggerQueue(ref spell);
        }

        public void StartSequenceStartTurn(CastInfo spell)
        {
            StartPhase(Phase.StartTurn, ref spell);
            StartSequenceDrawCard(spell);
        }

        public void StartSequenceDrawCard(CastInfo spell)
        {
            if (spell.player.deck.Count == 0)
            {
                //TODO: FatiguePlayer(spell.player)
                return;
            }
            Board.HandCard card = server.DrawCard(spell.match, spell.player);
            spell.card = card;
            StartPhase(Phase.OnDrawCard, ref spell);
        }
        public void StartSequenceDiscardCard(CastInfo spell)
        {
            StartPhase(Phase.OnDiscardCard, ref spell);
        }

        public void StartSequenceAttackMinion(CastInfo spell)
        {
            StartPhase(Phase.BeforeAttack, ref spell);

            bool successfulAttack = server.ExecuteAttack(ref spell);
            ResolveTriggerQueue(ref spell);

            if (successfulAttack == false)
            {
                return;
            }

            StartPhase(Phase.AfterAttack, ref spell);
        }
        public void StartSequenceAttackFace(CastInfo spell)
        {
            StartPhase(Phase.BeforeAttackFace, ref spell);

            bool successfulAttack = server.ExecuteAttack(ref spell);
            ResolveTriggerQueue(ref spell);

            if (successfulAttack == false)
            {
                return;
            }

            StartPhase(Phase.AfterAttackFace, ref spell);
        }
        public void StartSequencePlaySpell(CastInfo spell)
        {
            StartPhase(Phase.OnPlayCard, ref spell);
            StartPhase(Phase.OnPlaySpell, ref spell);

            server.CastSpell(spell);
            ResolveTriggerQueue(ref spell);

            StartPhase(Phase.AfterPlayCard, ref spell);
            StartPhase(Phase.AfterPlaySpell, ref spell);
        }

        public void StartSequencePlayMinion(CastInfo spell)
        {
            Board.Minion m = server.SummonMinion(this, spell.player, spell.card.card, spell.position);
            if (m == null) return;
            spell.minion = m;

            StartPhase(Phase.OnPlayCard, ref spell);
            StartPhase(Phase.OnPlayMinion, ref spell);

            if (spell.card.BATTLECRY)
            {
                server.CastSpell(spell);
                ResolveTriggerQueue(ref spell);
            }

            StartPhase(Phase.AfterPlayCard, ref spell);
            StartPhase(Phase.AfterPlayMinion, ref spell);
        }
        public void StartSequenceSummonMinion(CastInfo spell, Card.Cardname card)
        {

            Board.Minion m = server.SummonMinion(this, spell.player, card, spell.position);
            if (m == null) return;
            spell.minion = m;

            StartPhase(Phase.OnSummonMinion, ref spell);

            StartPhase(Phase.AfterSummonMinion, ref spell);
        }

        public void TriggerMinion(Board.Trigger.Type type,Board.Minion target)
        {
            triggerBuffer.AddRange(target.CheckTriggers(type, Board.Trigger.Side.Both, null));
        }

        public void AddTrigger(Board.Trigger.Type type, CastInfo spell = null, Board.Minion source = null)
        {
            //todo: check secrets for triggers
            Player owner = FindOwner(source);
            Board.Trigger.Side p0Side = owner == players[0] ? Board.Trigger.Side.Friendly : Board.Trigger.Side.Enemy;
            Board.Trigger.Side p1Side = owner == players[1] ? Board.Trigger.Side.Friendly : Board.Trigger.Side.Enemy;

            if (spell==null) spell = new CastInfo();
            spell.minion = source;
            spell.player = owner;

            foreach (Board.Minion minion in players[0].board)
            {
                triggerBuffer.AddRange(minion.CheckTriggers(type, p0Side,spell));
            }
            foreach (Board.Minion minion in players[1].board)
            {
                triggerBuffer.AddRange(minion.CheckTriggers(type, p0Side,spell));
            }
        }
        public void AddTrigger(Board.Trigger.Type type, CastInfo spell = null, Player source=null)
        {            
            //todo: check secrets for triggers
            Board.Trigger.Side p0Side = source == players[0] ? Board.Trigger.Side.Friendly : Board.Trigger.Side.Enemy;
            Board.Trigger.Side p1Side = source == players[1] ? Board.Trigger.Side.Friendly : Board.Trigger.Side.Enemy;

            if (spell==null) spell = new CastInfo();
            spell.player = source;

            foreach (Board.Minion minion in players[0].board)
            {
                triggerBuffer.AddRange(minion.CheckTriggers(type, p0Side,spell));
            }
            foreach (Board.Minion minion in players[1].board)
            {
                triggerBuffer.AddRange(minion.CheckTriggers(type, p0Side,spell));
            }
        }
        public CastInfo StartPhase(Phase phase, ref CastInfo spell)
        {
            Board.Trigger.Type phaseTrigger = Board.Trigger.GetPhaseTrigger(phase);
            List<Board.Minion> triggeredMinions = new List<Board.Minion>();

            //todo: check secrets for triggers
            //todo: make this a call to addtrigger(type,player)? ^func above this
            Board.Trigger.Side p0Side = spell.player == players[0] ? Board.Trigger.Side.Friendly : Board.Trigger.Side.Enemy;
            Board.Trigger.Side p1Side = spell.player == players[1] ? Board.Trigger.Side.Friendly : Board.Trigger.Side.Enemy;
            
            foreach (Board.Minion minion in players[0].board)
            {
                triggerBuffer.AddRange(minion.CheckTriggers(phaseTrigger, p0Side,spell));
            }
            foreach (Board.Minion minion in players[1].board)
            {
                triggerBuffer.AddRange(minion.CheckTriggers(phaseTrigger, p0Side,spell));
            }

            ResolveTriggerQueue(ref spell);

            return spell;
        }


        public List<Board.Trigger> triggerQueue = new List<Board.Trigger>();
        public List<Board.Trigger> triggerBuffer = new List<Board.Trigger>();
        public void ResolveTriggerQueue(ref CastInfo spell)
        {
            ReadTriggerBuffer();
            while (triggerQueue.Count>0 || triggerBuffer.Count>0)
            {
                ReadTriggerBuffer();

                Board.Trigger t = triggerQueue[0];
                triggerQueue.Remove(t);
                t.ActivateTrigger(this,ref spell);
            }

            UpdateAuras();
        }
        void ReadTriggerBuffer()
        {
            if (triggerBuffer.Count > 0)
            {
                //Sort by playorder and add to resolve queue
                triggerBuffer.Sort((x, y) => y.playOrder.CompareTo(x.playOrder));
                foreach (var t in triggerBuffer)
                {
                    triggerQueue.Insert(0, t);
                }
                triggerBuffer.Clear();
            }
        }
        void UpdateAuras()
        {
            //Initial visual update

            //update minions
            foreach (Board.Minion minion in players[0].board)
            {
                server.UpdateMinion(this, minion);
            }
            foreach (Board.Minion minion in players[1].board)
            {
                server.UpdateMinion(this, minion);
            }
            //TODO: update and check hero health for game over
            server.UpdateHero(this, players[0]);
            server.UpdateHero(this, players[1]);

            //=====================================
            //MINION DEATHS
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

            //death resolution phase
            foreach (var m in destroyList)
            {
                Debug.Log("kill " + m.card);
                AddTrigger(Board.Trigger.Type.OnMinionDeath, null, m);
                server.DestroyMinion(this, m);
            }
            if (triggerBuffer.Count > 0 || triggerQueue.Count > 0)
            {
                CastInfo deathResolution = new CastInfo();
                ResolveTriggerQueue(ref deathResolution);
            }

            //deathrattles happen after "on deaths"
            foreach (var m in destroyList)
            {
                TriggerMinion(Board.Trigger.Type.Deathrattle, m);
            }
            if (triggerBuffer.Count > 0 || triggerQueue.Count > 0)
            {
                CastInfo deathResolution = new CastInfo();
                ResolveTriggerQueue(ref deathResolution);
            }
            //=====================================
            //Aura activation
            foreach (Board.Minion minion in players[0].board)
            {
                List<Board.Minion.Aura> auras = new List<Board.Minion.Aura>(minion.auras);
                foreach (var aura in auras)
                    aura.ActivateAura(this);
            }
            foreach (Board.Minion minion in players[1].board)
            {
                List<Board.Minion.Aura> auras = new List<Board.Minion.Aura>(minion.auras);
                foreach (var aura in auras)
                    aura.ActivateAura(this);
            }

            //Remove foreign effects if no longer refreshed
            foreach (Board.Minion minion in players[0].board)
            {
                minion.RefreshForeignAuras();
            }
            foreach (Board.Minion minion in players[1].board)
            {
                minion.RefreshForeignAuras();
            }

            //TODO: HAND CARD AURAS
            //=====================================


            //=====================================
            //update minions after aura recalculation and deaths
            foreach (Board.Minion minion in players[0].board)
            {
                server.UpdateMinion(this, minion);
            }
            foreach (Board.Minion minion in players[1].board)
            {
                server.UpdateMinion(this, minion);
            }
            //TODO: update and check hero health for game over
            server.UpdateHero(this, players[0]);
            server.UpdateHero(this, players[1]);
        }
    }
}
    