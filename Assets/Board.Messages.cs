using System.Collections.Generic;
using UnityEngine;

public partial class Board
{
    public List<int> GetDeckList(List<Card.Cardname> deck)
    {
        List<int> ints = new List<int>();
        foreach (var c in deck)
        {
            ints.Add((int)c);
        }
        return ints;
    }
    public void StartMatchmaking(SaveManager.Decklist list)
    {
        Server.CustomMessage message = CreateMessage(Server.MessageType.Matchmaking);
        message.AddULong(playerID);
        message.AddString(playerName);

        message.AddInts(GetDeckList(list.cards));

        message.AddInt((int)list.classType);
        //client.Send(message);
        SendMessage(message, true);
    }

    public void LeaveMatchmaking()
    {
        Server.CustomMessage message = CreateMessage(Server.MessageType.LeaveMatchmaking);
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
    public void SubmitConcede()
    {
        Server.CustomMessage message = CreateMessage(Server.MessageType.Concede);
        message.AddULong(currentMatchID);
        message.AddULong(playerID);

        SendMessage(message,true);
    }

    public void PlayCard(HandCard card, int target = -1, int position = -1, bool friendlySide = false, bool isHero = false)
    {
        if (!currTurn) return;
        if (card.played) return;
        card.played = true;
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

        EndTargeting();
        playingCard = null;

        ConfirmPlayPlayer(card, position);
        SendMessage(message);
    }


    public void AttackMinion(Minion attacker, Minion target)
    {

        int attackerInd = attacker.index;
        int targetInd = target.index;
        bool enemyTaunting = false;

        if (attacker.canAttack == false)
        {
            animationManager.CancelLiftMinion(attacker.creature);
        }
        //todo: valid attack function
        foreach (Minion m in enemyMinions)
        {
            if (m.HasAura(Aura.Type.Taunt) && m.HasAura(Aura.Type.Stealth)==false) enemyTaunting = true;
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

        if (minion.canAttack == false)
        {
            animationManager.CancelLiftMinion(minion.creature);
        }

        if (CheckTargetEligibility(h) == false)
        {
            //invalid target todo:check these on server
            Debug.Log("Invalid target");
            return;
        }

        foreach (Minion m in enemyMinions)
        {
            if (m.HasAura(Aura.Type.Taunt) && m.HasAura(Aura.Type.Stealth)==false) enemyTaunting = true;
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


        mana.Spend(heroPower.card.manaCost);
        mana.UpdateDisplay();

        heroPower.Disable();

        SendMessage(message);
    }

    public void SwingMinion(Hero targetingHero, bool isFriendly ,Minion target)
    {

        bool enemyTaunting = false;

        //todo: valid attack function
        foreach (Minion m in enemyMinions)
        {
            if (m.HasAura(Aura.Type.Taunt) && m.HasAura(Aura.Type.Stealth)==false) enemyTaunting = true;
        }
        if (enemyTaunting && target.HasAura(Aura.Type.Taunt) == false)
        {
            //can't attack non taunter
            Debug.Log("Taunt in the way");
            return;
        }

        EndTargeting();
        Server.CustomMessage message = CreateMessage(Server.MessageType.SwingMinion);

        message.AddULong(currentMatchID);
        message.AddULong(playerID);
        message.AddInt(target.index); 
        
        SendMessage(message);
    }
    public void SwingFace(Hero targetingHero, Hero h)
    {
        bool enemyTaunting = false;

        if (CheckTargetEligibility(h) == false)
        {
            //invalid target todo:check these on server
            Debug.Log("Invalid target");
            return;
        }
        //todo: valid attack function
        foreach (Minion m in enemyMinions)
        {
            if (m.HasAura(Aura.Type.Taunt) && m.HasAura(Aura.Type.Stealth)==false) enemyTaunting = true;
        }
        if (enemyTaunting)
        {
            //can't attack non taunter
            Debug.Log("Taunt in the way");
            return;
        }

        EndTargeting();
        Server.CustomMessage message = CreateMessage(Server.MessageType.SwingFace);

        message.AddULong(currentMatchID);
        message.AddULong(playerID);
        SendMessage(message);
    }
}
