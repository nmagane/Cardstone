using System;
using System.Collections.Generic;
using UnityEngine;

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

        AfterHeroPower,
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

        foreach (var m in player.board)
        {
            m.RemoveTemporaryAuras();
        }
        foreach (var m in opponent.board)
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
        HandCard card = server.DrawCard(spell.match, spell.player);
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
        Minion m = server.SummonMinion(this, spell.player, spell.card.card, spell.position);
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

        Minion m = server.SummonMinion(this, spell.player, card, spell.position);
        if (m == null) return;
        spell.minion = m;

        StartPhase(Phase.OnSummonMinion, ref spell);

        StartPhase(Phase.AfterSummonMinion, ref spell);
    }
    public void StartSequenceHeroPower(CastInfo spell)
    {
        server.CastSpell(spell);
        StartPhase(Phase.AfterHeroPower, ref spell);
    }
    public void TriggerMinion(Trigger.Type type, Minion target)
    {
        triggerBuffer.AddRange(target.CheckTriggers(type, Trigger.Side.Both, null));
    }

    public void AddTrigger(Trigger.Type type, CastInfo spell = null, Minion source = null)
    {
        //todo: check secrets for triggers
        Player owner = FindOwner(source);
        Trigger.Side p0Side = owner == players[0] ? Trigger.Side.Friendly : Trigger.Side.Enemy;
        Trigger.Side p1Side = owner == players[1] ? Trigger.Side.Friendly : Trigger.Side.Enemy;

        if (spell == null) spell = new CastInfo();
        spell.minion = source;
        spell.player = owner;

        foreach (Minion minion in players[0].board)
        {
            triggerBuffer.AddRange(minion.CheckTriggers(type, p0Side, spell));
        }
        foreach (Minion minion in players[1].board)
        {
            triggerBuffer.AddRange(minion.CheckTriggers(type, p0Side, spell));
        }
    }
    public void AddTrigger(Trigger.Type type, CastInfo spell = null, Player source = null)
    {
        //todo: check secrets for triggers
        Trigger.Side p0Side = source == players[0] ? Trigger.Side.Friendly : Trigger.Side.Enemy;
        Trigger.Side p1Side = source == players[1] ? Trigger.Side.Friendly : Trigger.Side.Enemy;

        if (spell == null) spell = new CastInfo();
        spell.player = source;

        foreach (Minion minion in players[0].board)
        {
            triggerBuffer.AddRange(minion.CheckTriggers(type, p0Side, spell));
        }
        foreach (Minion minion in players[1].board)
        {
            triggerBuffer.AddRange(minion.CheckTriggers(type, p0Side, spell));
        }
    }
    public CastInfo StartPhase(Phase phase, ref CastInfo spell)
    {
        Trigger.Type phaseTrigger = Trigger.GetPhaseTrigger(phase);
        List<Minion> triggeredMinions = new List<Minion>();

        //todo: check secrets for triggers
        //todo: make this a call to addtrigger(type,player)? ^func above this
        Trigger.Side p0Side = spell.player == players[0] ? Trigger.Side.Friendly : Trigger.Side.Enemy;
        Trigger.Side p1Side = spell.player == players[1] ? Trigger.Side.Friendly : Trigger.Side.Enemy;

        foreach (Minion minion in players[0].board)
        {
            triggerBuffer.AddRange(minion.CheckTriggers(phaseTrigger, p0Side, spell));
        }
        foreach (Minion minion in players[1].board)
        {
            triggerBuffer.AddRange(minion.CheckTriggers(phaseTrigger, p0Side, spell));
        }

        ResolveTriggerQueue(ref spell);

        return spell;
    }


    public List<Trigger> triggerQueue = new List<Trigger>();
    public List<Trigger> triggerBuffer = new List<Trigger>();
    public void ResolveTriggerQueue(ref CastInfo spell)
    {
        ReadTriggerBuffer();
        while (triggerQueue.Count > 0 || triggerBuffer.Count > 0)
        {
            ReadTriggerBuffer();

            Trigger t = triggerQueue[0];
            triggerQueue.Remove(t);
            t.ActivateTrigger(this, ref spell);
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
        foreach (Minion minion in players[0].board)
        {
            server.UpdateMinion(this, minion);
        }
        foreach (Minion minion in players[1].board)
        {
            server.UpdateMinion(this, minion);
        }
        //TODO: update and check hero health for game over
        server.UpdateHero(this, players[0]);
        server.UpdateHero(this, players[1]);

        //=====================================
        //MINION DEATHS
        List<Minion> destroyList = new List<Minion>();
        foreach (Minion minion in players[0].board)
        {
            server.UpdateMinion(this, minion);
            if (minion.health <= 0) destroyList.Add(minion);
        }
        foreach (Minion minion in players[1].board)
        {
            server.UpdateMinion(this, minion);
            if (minion.health <= 0) destroyList.Add(minion);
        }

        //death resolution phase
        foreach (var m in destroyList)
        {
            Debug.Log("kill " + m.card);
            AddTrigger(Trigger.Type.OnMinionDeath, null, m);
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
            TriggerMinion(Trigger.Type.Deathrattle, m);
        }
        if (triggerBuffer.Count > 0 || triggerQueue.Count > 0)
        {
            CastInfo deathResolution = new CastInfo();
            ResolveTriggerQueue(ref deathResolution);
        }
        //=====================================
        //Aura activation
        foreach (Minion minion in players[0].board)
        {
            List<Aura> auras = new List<Aura>(minion.auras);
            foreach (var aura in auras)
                aura.ActivateAura(this);
        }
        foreach (Minion minion in players[1].board)
        {
            List<Aura> auras = new List<Aura>(minion.auras);
            foreach (var aura in auras)
                aura.ActivateAura(this);
        }

        //Remove foreign effects if no longer refreshed
        foreach (Minion minion in players[0].board)
        {
            minion.RefreshForeignAuras();
        }
        foreach (Minion minion in players[1].board)
        {
            minion.RefreshForeignAuras();
        }

        //TODO: HAND CARD AURAS
        //=====================================


        //=====================================
        //update minions after aura recalculation and deaths
        foreach (Minion minion in players[0].board)
        {
            server.UpdateMinion(this, minion);
        }
        foreach (Minion minion in players[1].board)
        {
            server.UpdateMinion(this, minion);
        }
        //TODO: update and check hero health for game over
        server.UpdateHero(this, players[0]);
        server.UpdateHero(this, players[1]);
    }
}