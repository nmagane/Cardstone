using System;
using System.Collections;
using System.Collections.Generic;
using Riptide;
using Riptide.Transports;
using Riptide.Utils;
using System.Linq;
using UnityEngine;

public partial class Server
{
    public bool ValidAttackMinion(Match m, int attackerInd, int targetInd)
    {
        Board.Minion attacker = m.currPlayer.board[attackerInd];
        Board.Minion target = m.enemyPlayer.board[targetInd];

        if (attacker.canAttack == false) return false;
        if (target.STEALTH) return false;
        bool enemyTaunting = false;
        foreach (var minion in m.enemyPlayer.board)
        {
            if (minion.TAUNT) { enemyTaunting = true; break; }
        }
        if (target.TAUNT == false && enemyTaunting) return false;

        return true;
    }

    public void ConfirmAttackMinion(Match match, int attackerInd, int targetInd)
    {
        Message mOwner = Message.Create(MessageSendMode.Reliable, (int)MessageType.ConfirmAttackMinion);
        Message mOpp = Message.Create(MessageSendMode.Reliable, (int)MessageType.ConfirmAttackMinion);
        mOwner.AddBool(true);
        mOpp.AddBool(false);

        mOwner.AddInt(attackerInd);
        mOpp.AddInt(attackerInd);

        mOwner.AddInt(targetInd);
        mOpp.AddInt(targetInd);

        server.Send(mOwner, match.currPlayer.connection.clientID);
        server.Send(mOpp, match.enemyPlayer.connection.clientID);
    }
    public void ConfirmAttackFace()
    {

    }
    public void ConfirmSwingMinion()
    {

    }
    public void ConfirmSwingFace()
    {

    }
}