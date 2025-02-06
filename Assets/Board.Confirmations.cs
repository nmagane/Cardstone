using UnityEngine;
using System.Collections.Generic;

public partial class Board
{/*
    public static Message CreateMessage(Server.MessageType type)
    {
        Message m = Message.Create(MessageSendMode.Reliable, (ushort)type);
        m.ReserveBits(16);
        return m;

    public void SendMessage(Message message, bool UNORDERED=false)
    {
        //todo: not sure if this check is ok. are there ever messages sent on enemy turn?
        if (UNORDERED == false && currTurn == false)
            return;
        message.SetBits(matchMessageOrder, 16, 28);
        client.Send(message);
        if (UNORDERED == false)
            matchMessageOrder++;
    }
    }*/

    public bool disableInput = false;
    public static Server.CustomMessage CreateMessage(Server.MessageType type)
    {
        Server.CustomMessage m = new Server.CustomMessage();
        m.type = type;
        return m;
    }
    public void SendMessage(Server.CustomMessage message, bool UNORDERED = false)
    {
        //todo: not sure if this check is ok. are there ever messages sent on enemy turn?
        if (UNORDERED == false && currTurn == false)
            return;
        message.order = matchMessageOrder;
        message.clientID = (int)playerID;
        //client.Send(message);
        mirror.SendClient(message);
        if (UNORDERED == false)
            matchMessageOrder++;
    }
    void ConfirmMulligan(List<ushort> cards)
    {
        foreach (int i in selectedMulligans)
        {
            //TODO: mull anim goes here
            currHand.MulliganReplace(i, (Card.Cardname)cards[i]);
        }
        currHand.EndMulligan();
        waitingEnemyMulliganMessage.transform.localScale = Vector3.one;
        mulliganButton.transform.localPosition += new Vector3(0, -10);

        VisualInfo anim = new VisualInfo();
        anim.type = Server.MessageType.ConfirmMulligan;
        QueueAnimation(anim);
    }
    void ConfirmEnemyMulligan(List<int> inds)
    {
        foreach (int i in inds)
        {
            //TODO: enemy mull anim
            //enemyHand.cardObjects[enemyHand[i]].mulliganMark.enabled = true;
        }
    }
    void StartGame(bool isTurn)
    {
        VisualInfo anim = new VisualInfo();
        anim.type = Server.MessageType.StartGame;
        anim.isFriendly = isTurn;
        QueueAnimation(anim);
    }

    public void ConfirmPlayPlayer(HandCard card, int pos)
    {
        //return;
        currHand.RemoveAt(card.index);
        currHand.RemoveCard(card,Hand.RemoveCardType.Play,card.card,pos);
        if (card.MINION)
        {
            int p = pos;

            if (pos >= 7) pos = currMinions.Count();
            Minion m = new Minion(card.card, pos, currMinions);
            prePlayMinions.Add(p, m);
            currMinions.AddCreature(m);

        }
    }
    public Dictionary<int, Minion> prePlayMinions = new Dictionary<int, Minion>();
    public void SummonMinion(bool friendlySide, Card.Cardname card, int position, MinionBoard.MinionSource source)
    {
        if (friendlySide && source == MinionBoard.MinionSource.Play)
        {
            if (prePlayMinions.ContainsKey(position) == false) Debug.LogError("NOT FOUND PREPLAY");
            if (prePlayMinions[position].card != card) Debug.LogError("NOT FOUND PREPLAY");

            Minion ppm = prePlayMinions[position];

            if (position == -1)
            {
                position = currMinions.Count();
            }
            if (currMinions.Count() == 0)
            {
                currMinions.minions.Add(ppm);
            }
            else if (currMinions.Count() != 0 && position >= currMinions.Count())
            {
                currMinions.minions.Add(ppm);
            }
            else
            {
                currMinions.minions.Insert(position, ppm);
            }
            currMinions.OrderInds();
            currMinions.OrderCreatures();
            prePlayMinions.Remove(position);
            return;
        }

        MinionBoard board = friendlySide ? currMinions : enemyMinions;
        Minion m = board.Add(card, position);

        //===========ANIM
        VisualInfo anim = new VisualInfo();
        anim.type = Server.MessageType.SummonMinion;
        anim.minions.Add(m);
        anim.isFriendly = friendlySide;

        QueueAnimation(anim);
    }
    public void ConfirmPlayCard(bool friendlySide, int index, int manaCost, Card.Cardname card, int pos)
    {
        if (friendlySide) return;

        HandCard hc = null;
        if (friendlySide == false)
        {
            hc = enemyHand.RemoveAt(index);
        }
        else
            hc = currHand.RemoveAt(index);

        //===================ANIM
        VisualInfo anim = new VisualInfo();
        anim.type = Server.MessageType.PlayCard;
        anim.handCards.Add(hc);
        anim.names.Add(card);
        anim.isFriendly = friendlySide;
        anim.manaCost = manaCost;
        anim.index = pos;

        QueueAnimation(anim);

        CheckHighlights();
    }



    public Coroutine ConfirmPreAttackMinion(bool allyAttack, int attackerIndex, int targetIndex)
    {
        //TODO: preattack animation
        Minion attacker = allyAttack ? currMinions[attackerIndex] : enemyMinions[attackerIndex];
        Minion target = allyAttack ? enemyMinions[targetIndex] : currMinions[targetIndex];

        Creature atkCreature = allyAttack ? currMinions.minionObjects[attacker] : enemyMinions.minionObjects[attacker];
        Creature tarCreature = allyAttack ? enemyMinions.minionObjects[target] : currMinions.minionObjects[target];

        CheckHighlights();

        VisualInfo anim = new VisualInfo();
        anim.type = Server.MessageType.ConfirmPreAttackMinion;
        anim.creatures.Add(atkCreature);
        anim.vectors.Add(tarCreature.transform.localPosition);

        QueueAnimation(anim);


        return null;
    }
    public Coroutine ConfirmAttackMinion(bool allyAttack, int attackerIndex, int targetIndex)
    {
        
        Minion attacker = allyAttack ? currMinions[attackerIndex] : enemyMinions[attackerIndex];
        Minion target = allyAttack ? enemyMinions[targetIndex] : currMinions[targetIndex];

        Creature atkCreature = allyAttack ? currMinions.minionObjects[attacker] : enemyMinions.minionObjects[attacker];
        Creature tarCreature = allyAttack ? enemyMinions.minionObjects[target] : currMinions.minionObjects[target];

        if (allyAttack)
        {
            Server.ConsumeAttackCharge(attacker);
        }

        CheckHighlights();


        VisualInfo anim = new VisualInfo();
        anim.type = Server.MessageType.ConfirmAttackMinion;
        anim.creatures.Add(atkCreature);
        anim.vectors.Add(tarCreature.transform.localPosition);

        QueueAnimation(anim);

        return null;
    }

    public Coroutine ConfirmPreAttackFace(bool allyAttack, int attackerIndex)
    {
        Minion attacker = allyAttack ? currMinions[attackerIndex] : enemyMinions[attackerIndex];
        Creature atkCreature = allyAttack ? currMinions.minionObjects[attacker] : enemyMinions.minionObjects[attacker];
        Hero tar = allyAttack ? enemyHero : currHero;


        VisualInfo anim = new VisualInfo();
        anim.type = Server.MessageType.ConfirmPreAttackMinion;
        anim.creatures.Add(atkCreature);
        anim.vectors.Add(tar.transform.localPosition);

        QueueAnimation(anim);

        CheckHighlights();
        return null;
    }
    public Coroutine ConfirmAttackFace(bool allyAttack, int attackerIndex)
    {

        Minion attacker = allyAttack ? currMinions[attackerIndex] : enemyMinions[attackerIndex];
        Creature atkCreature = allyAttack ? currMinions.minionObjects[attacker] : enemyMinions.minionObjects[attacker];
        Hero tar = allyAttack ? enemyHero : currHero;
        if (allyAttack)
        {
            Server.ConsumeAttackCharge(attacker);
        }

        VisualInfo anim = new VisualInfo();
        anim.type = Server.MessageType.ConfirmAttackFace;
        anim.creatures.Add(atkCreature);
        anim.vectors.Add(tar.transform.localPosition);

        QueueAnimation(anim);

        return null;
    }

    public Coroutine ConfirmBattlecry(bool friendly, int index)
    {
        VisualInfo anim = new();
        anim.type = Server.MessageType.ConfirmBattlecry;
        Minion m = friendly ? currMinions[index] : enemyMinions[index];
        anim.minions.Add(m);

        QueueAnimation(anim);
        return null;
    }
    public Coroutine ConfirmTrigger(bool friendly, int index, bool deathrattle)
    {
        if (deathrattle)
        {
            VisualInfo animDR = new();
            animDR.type = Server.MessageType.ConfirmTrigger;
            animDR.trigger = true;
            //anim.creatures.Add(m);

            QueueAnimation(animDR);
            return null;
        }

        VisualInfo anim = new();
        Minion m = friendly ? currMinions[index] : enemyMinions[index];
        anim.minions.Add(m);
        anim.type = Server.MessageType.ConfirmTrigger;

        QueueAnimation(anim);

        return null;
    }
    void ConfirmHeroPower(bool ally)
    {
        VisualInfo anim = new();
        anim.type = Server.MessageType.ConfirmHeroPower;
        anim.isFriendly = ally;

        QueueAnimation(anim);

        CheckHighlights();
    }

    void ConfirmAnimation(AnimationManager.AnimationInfo animInfo, bool friendly)
    {

    }
}
