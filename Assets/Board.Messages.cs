using System.Collections.Generic;
using Riptide;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using System.Collections;
public partial class Board
{
    public void StartMatchmaking()
    {
        Server.CustomMessage message = CreateMessage(Server.MessageType.Matchmaking);
        message.AddULong(playerID);
        string deck = "";
        message.AddString(deck);
        //client.Send(message);
        SendMessage(message, true);
    }



    public List<int> selectedMulligans = new List<int>() { };
    public void SubmitMulligan()
    {
        Server.CustomMessage message = CreateMessage(Server.MessageType.SubmitMulligan);
        message.AddULong(currentMatchID);
        message.AddInts(selectedMulligans);
        message.AddULong(playerID);
        //client.Send(message);
        SendMessage(message, true);
    }


    public void SubmitEndTurn()
    {
        if (!currTurn) return;

        Server.CustomMessage message = CreateMessage(Server.MessageType.EndTurn);
        message.AddULong(currentMatchID);
        message.AddULong(playerID);

        SendMessage(message);
    }

    public void PlayCard(HandCard card, int target = -1, int position = -1, bool friendlySide = false, bool isHero = false)
    {
        if (!currTurn) return;
        if (card.played) return;
        card.played = true;
        EndTargeting();
        playingCard = null;
        //Debug.Log("Playing card " + card.card);
        //send message to server to play card index
        Server.CustomMessage message = CreateMessage(Server.MessageType.PlayCard);
        message.AddULong(currentMatchID);
        message.AddULong(playerID);
        message.AddInt(card.index);
        message.AddInt(target);
        message.AddInt(position);
        message.AddBool(friendlySide);
        message.AddBool(isHero);

        mana.Spend(card.manaCost);
        mana.UpdateDisplay();

        if (card.MINION)
        {
            //todo - insta summon minions
        }
        else
        {

        }
        //client.Send(message);
        SendMessage(message);
    }


    public void AttackMinion(Minion attacker, Minion target)
    {

        int attackerInd = attacker.index;
        int targetInd = target.index;
        bool enemyTaunting = false;


        //todo: valid attack function
        foreach (Minion m in enemyMinions)
        {
            if (m.HasAura(Aura.Type.Taunt)) enemyTaunting = true;
        }
        if (enemyTaunting && target.HasAura(Aura.Type.Taunt) == false)
        {
            //can't attack non taunter
            Debug.Log("Taunt in the way");
            return;
        }

        EndTargeting();

        Server.CustomMessage message = CreateMessage(Server.MessageType.AttackMinion);
        message.AddULong(currentMatchID);
        message.AddULong(playerID);
        message.AddInt(attackerInd);
        message.AddInt(targetInd);
        //client.Send(message);
        SendMessage(message);
    }

    public void AttackFace(Minion minion, Hero h)
    {

        int attackerInd = minion.index;
        bool enemyTaunting = false;

        if (CheckTargetEligibility(h) == false)
        {
            //invalid target todo:check these on server
            Debug.Log("Invalid target");
            return;
        }

        foreach (Minion m in enemyMinions)
        {
            if (m.HasAura(Aura.Type.Taunt)) enemyTaunting = true;
        }
        if (enemyTaunting)
        {
            //can't attack non taunter
            Debug.Log("Taunt in the way");
            return;
        }

        EndTargeting();

        Server.CustomMessage message = CreateMessage(Server.MessageType.AttackFace);
        message.AddULong(currentMatchID);
        message.AddULong(playerID);
        message.AddInt(attackerInd);
        //client.Send(message);
        SendMessage(message);
    }

    public void CastHeroPower(Card.Cardname ability, int target, bool isFriendly, bool isHero)
    {
        Server.CustomMessage message = CreateMessage(Server.MessageType.HeroPower);

        if (targeting) EndTargeting();
        message.AddULong(currentMatchID);
        message.AddULong(playerID);
        message.AddUShort((ushort)ability);
        message.AddInt(target);
        message.AddBool(isFriendly);
        message.AddBool(isHero);

        heroPower.Disable();

        SendMessage(message);
    }

}
