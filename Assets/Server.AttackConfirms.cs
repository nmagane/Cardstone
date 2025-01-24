using System;
using System.Collections;
using System.Collections.Generic;
using Riptide;
using Riptide.Transports;
using Riptide.Utils;
using System.Linq;
using UnityEngine;
using UnityEditor.Experimental.GraphView;

public partial class Server
{
    public bool ValidAttackMinion(Match m, int attackerInd, int targetInd)
    {
        //TODO: reuse this function in board code
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
    public bool ValidAttackFace(Match match,Player attacker,Player defender, int attackerInd)
    {

        if (attacker.board[attackerInd].canAttack == false) return false;
        foreach (var minion in defender.board)
        {
            if (minion.TAUNT) 
            {
                return false;
            }
        }
        return true;
    }

    public void ConfirmAttackGeneral(CastInfo action)
    {
        Match match = action.match;
        AttackInfo attack = action.attack;

        if (attack.faceAttack)
        {
            if (attack.weaponSwing)
            {
                //Face to face
            }
            else
            {
                //Minion to face
                ConfirmAttackFace(match, attack.attacker.index, attack.friendlyFire);
            }
        }
        else
        {
            if (attack.weaponSwing)
            {
                //Face to minion
            }
            else
            {
                //Minion to minion
                ConfirmAttackMinion(match, attack.attacker.index, attack.target.index, attack.friendlyFire);
            }
        }
    }

    public void ConfirmAttackMinion(Match match, int attackerInd, int targetInd, bool friendlyFire)
    {
        Message mOwner = CreateMessage(MessageType.ConfirmAttackMinion);
        Message mOpp = CreateMessage(MessageType.ConfirmAttackMinion);
        mOwner.AddBool(true);
        mOpp.AddBool(false);

        mOwner.AddInt(attackerInd);
        mOpp.AddInt(attackerInd);

        mOwner.AddInt(targetInd);
        mOpp.AddInt(targetInd);

        //server.Send(mOwner, match.currPlayer.connection.clientID);
        SendMessage(mOwner, match.currPlayer);
        //server.Send(mOpp, match.enemyPlayer.connection.clientID);
        SendMessage(mOpp, match.enemyPlayer);
    }
    public void ConfirmAttackFace(Match match, int attackerInd,bool friendlyFire)
    {
        Message mOwner = CreateMessage(MessageType.ConfirmAttackFace);
        Message mOpp = CreateMessage(MessageType.ConfirmAttackFace);
        mOwner.AddBool(true);
        mOpp.AddBool(false);

        mOwner.AddInt(attackerInd);
        mOpp.AddInt(attackerInd);

        //server.Send(mOwner, match.currPlayer.connection.clientID);
        SendMessage(mOwner, match.currPlayer);
        //server.Send(mOpp, match.enemyPlayer.connection.clientID);
        SendMessage(mOpp, match.enemyPlayer);
    }
    public void ConfirmSwingMinion()
    {

    }
    public void ConfirmSwingFace()
    {

    }
}