using System;
using System.Collections.Generic;
using UnityEngine;

public partial class Match
{
    public enum Result
    {
        Win,
        Lose,
        Draw
    }
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
        OnMillCard,
        OnFatigue,

        AfterHeroPower,
    }

    public void StartSequenceEndTurn(CastInfo spell)
    {
        StartPhase(Phase.EndTurn, ref spell);

        BetweenTurnEvents(spell);
        WinCheck();
    }

    public void BetweenTurnEvents(CastInfo spell)
    {
        Player player = spell.player;
        Player opponent = player.opponent;

        //===============
        //minion auras
        foreach (var m in player.board)
        {
            m.RemoveTemporaryAuras();
        }
        foreach (var m in opponent.board)
        {
            m.RemoveTemporaryAuras();
        }
        //================
        //card auras
        foreach (var c in player.hand)
        {
            c.RemoveTemporaryAuras();
        }
        foreach (var c in opponent.hand)
        {
            c.RemoveTemporaryAuras();
        }
        //=====================
        //player auras
        players[0].RemoveTemporaryAuras();
        players[1].RemoveTemporaryAuras();

        ResolveTriggerQueue(ref spell);
    }

    public void StartSequenceStartTurn(CastInfo spell)
    {
        StartPhase(Phase.StartTurn, ref spell);

        if (WinCheck()) return;

        StartSequenceDrawCard(spell);
    }

    public void StartSequenceDrawCard(CastInfo spell)
    {
        if (spell.player.deck.Count == 0)
        {
            server.FatiguePlayer(spell.match, spell.player);
            StartPhase(Phase.OnFatigue,ref spell);
            return;
        }
        if (spell.player.hand.Count()>=10)
        {
            StartSequenceMillCard(spell);
            return;
        }
        HandCard card = server.DrawCard(spell.match, spell.player);
        spell.card = card;
        StartPhase(Phase.OnDrawCard, ref spell);

        WinCheck();
    }
    public void StartSequenceDiscardCard(CastInfo spell)
    {
        StartPhase(Phase.OnDiscardCard, ref spell);
        WinCheck();
    }
    public void StartSequenceMillCard(CastInfo spell)
    {
        server.MillCard(spell.match, spell.player);
        StartPhase(Phase.OnMillCard, ref spell);
        WinCheck();
    }

    public void StartSequenceAttackMinion(CastInfo spell)
    {
        StartPhase(Phase.BeforeAttack, ref spell);

        if (WinCheck()) return;

        bool successfulAttack = server.ExecuteAttack(ref spell);
        ResolveTriggerQueue(ref spell);

        if (successfulAttack == false)
        {
            return;
        }

        StartPhase(Phase.AfterAttack, ref spell);
        WinCheck();
    }
    public void StartSequenceAttackFace(CastInfo spell)
    {
        StartPhase(Phase.BeforeAttackFace, ref spell);

        if (WinCheck()) return;

        bool successfulAttack = server.ExecuteAttack(ref spell);
        ResolveTriggerQueue(ref spell);

        if (successfulAttack == false)
        {
            return;
        }

        StartPhase(Phase.AfterAttackFace, ref spell);
        WinCheck();
    }
    public void StartSequencePlaySpell(CastInfo spell)
    {
        StartPhase(Phase.OnPlayCard, ref spell);
        StartPhase(Phase.OnPlaySpell, ref spell);

        server.CastSpell(spell);
        ResolveTriggerQueue(ref spell);

        StartPhase(Phase.AfterPlayCard, ref spell);
        StartPhase(Phase.AfterPlaySpell, ref spell);
        WinCheck();
    }

    public void StartSequencePlayMinion(CastInfo spell)
    {
        Minion minion = server.SummonMinion(this, spell.player, spell.card.card,MinionBoard.MinionSource.Play, spell.position);
        if (minion == null) return;

        spell.minion = minion;
        spell.position = minion.index;

        StartPhase(Phase.OnPlayCard, ref spell);
        StartPhase(Phase.OnPlayMinion, ref spell);

        if (spell.card.BATTLECRY)
        {
            //DONT TRIGGER IF NO TARGET WAS CHOSEN
            if ((spell.card.TARGETED && spell.target == -1) == false)
            {
                server.ConfirmBattlecry(spell.match, minion);
                server.CastSpell(spell);
                ResolveTriggerQueue(ref spell);
            }
        }

        StartPhase(Phase.AfterPlayCard, ref spell);
        StartPhase(Phase.AfterPlayMinion, ref spell);
        WinCheck();
    }
    public void StartSequenceSummonMinion(CastInfo spell, Card.Cardname card)
    {

        Minion m = server.SummonMinion(this, spell.player, card,MinionBoard.MinionSource.Summon, spell.position);
        if (m == null) return;
        spell.minion = m;

        StartPhase(Phase.OnSummonMinion, ref spell);

        StartPhase(Phase.AfterSummonMinion, ref spell);
        WinCheck();
    }
    public void StartSequenceHeroPower(CastInfo spell)
    {
        server.CastSpell(spell);
        StartPhase(Phase.AfterHeroPower, ref spell);
        WinCheck();
    }
    public void TriggerMinion(Trigger.Type type, Minion target)
    {
        CastInfo cast = new CastInfo();
        cast.minion = target;
        triggerBuffer.AddRange(target.CheckTriggers(type, Trigger.Side.Both, cast));
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
            triggerBuffer.AddRange(minion.CheckTriggers(type, p1Side, spell));
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
            triggerBuffer.AddRange(minion.CheckTriggers(type, p1Side, spell));
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
            triggerBuffer.AddRange(minion.CheckTriggers(phaseTrigger, p1Side, spell));
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
            server.ConfirmBattlecry(this, t.minion,true,t.type==Trigger.Type.Deathrattle);
            t.ActivateTrigger(this, ref spell);
        }

        UpdateAuras();
    }
    void ReadTriggerBuffer()
    {
        UpdateStats();
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
    public List<(Minion, Aura, bool)> auraChanges = new List<(Minion, Aura, bool)>();

    public List<Minion> damagedMinions = new List<Minion>();
    public List<Minion> healedMinions = new List<Minion>();
    public List<Player> damagedPlayers = new List<Player>();
    public List<Player> healedPlayers = new List<Player>();
    public void UpdateStats()
    {

        foreach (var a in auraChanges)
        {
            server.ConfirmAuraChange(this, a.Item1, a.Item2, a.Item3);
        }
        if (auraChanges.Count > 0) auraChanges.Clear();
        
        //====================
        //cards
        foreach (HandCard c in players[0].hand)
        {
            server.UpdateCard(this, c, players[0]);
        }
        foreach (HandCard c in players[1].hand)
        {
            server.UpdateCard(this, c, players[1]);
        }
        
        //====================

        foreach (Minion minion in players[0].board)
        {
            server.UpdateMinion(this, minion,damagedMinions.Contains(minion),healedMinions.Contains(minion));
        }
        foreach (Minion minion in players[1].board)
        {
            server.UpdateMinion(this, minion, damagedMinions.Contains(minion), healedMinions.Contains(minion));
        }

        server.UpdateHero(this, players[0], damagedPlayers.Contains(players[0]), healedPlayers.Contains(players[0]));
        server.UpdateHero(this, players[1], damagedPlayers.Contains(players[1]), healedPlayers.Contains(players[1]));

        damagedMinions.Clear();
        healedMinions.Clear();
        damagedPlayers.Clear();
        healedPlayers.Clear();
    }
    void UpdateAuras()
    {
        //Initial visual update


        //update minions
        UpdateStats();
        //=====================================
        //MINION DEATHS
        List<Minion> destroyList = new List<Minion>();

        foreach (Minion minion in players[0].board)
        {
            if (minion.health <= 0) destroyList.Add(minion);
        }
        foreach (Minion minion in players[1].board)
        {
            if (minion.health <= 0) destroyList.Add(minion);
        }

        //death resolution phase
        foreach (var m in destroyList)
        {
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


        //HAND CARD AURAS
        //=====================================

        //no activatable auras on individual cards implemented.

        foreach (HandCard card in players[0].hand)
        {
            card.RefreshForeignAuras();
        }
        foreach (HandCard card in players[1].hand)
        {
            card.RefreshForeignAuras();
        }
        //=====================================

        foreach (Player p in players)
        {
            List<Aura> pAuras = new List<Aura>(p.auras);
            foreach (var aura in pAuras)
                aura.ActivateAura(this);
        }

        foreach (Player p in players)
        {
            p.RefreshForeignAuras();
        }
        //=====================================
        UpdateStats();
        //todo: check gameover
    }

    bool WinCheck()
    {
        if (players[0].health<=0 && players[1].health<=0)
        {
            //Draw
            server.EndMatch(this,null);
            return true;
        }
        else if (players[0].health<=0)
        {
            server.EndMatch(this,players[1]);
            return true;
        }
        else if (players[1].health<=0)
        {
            server.EndMatch(this,players[0]);
            return true;
        }

        return false;
    }
}