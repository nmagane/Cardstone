using UnityEngine;

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

    public void ConfirmPlayCard(bool friendlySide, int index, int manaCost, Card.Cardname card, int pos)
    {
        if (friendlySide == false)
        {
            enemyHand.RemoveAt(index, Hand.RemoveCardType.Play, card, pos);
            ShowEnemyPlay(card);
            enemyMana.Spend(manaCost);
            return;
        }
        //ally played card
        currHand.RemoveAt(index, Hand.RemoveCardType.Play, card, pos);
        this.mana.Spend(manaCost);
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
        return animationManager.PreAttackMinion(atkCreature, tarCreature.transform.localPosition);
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

        //TODO: attack animation
        CheckHighlights();
        return animationManager.ConfirmAttackMinion(atkCreature, tarCreature.transform.localPosition);
    }

    public Coroutine ConfirmPreAttackFace(bool allyAttack, int attackerIndex)
    {
        Minion attacker = allyAttack ? currMinions[attackerIndex] : enemyMinions[attackerIndex];
        Creature atkCreature = allyAttack ? currMinions.minionObjects[attacker] : enemyMinions.minionObjects[attacker];
        Hero tar = allyAttack ? enemyHero : currHero;

        CheckHighlights();
        return animationManager.PreAttackMinion(atkCreature, tar.transform.localPosition);
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

        return animationManager.ConfirmAttackMinion(atkCreature, tar.transform.localPosition);
        //todo: attack face animation
    }

    public Coroutine ConfirmBattlecry(bool friendly, int index)
    {
        Creature m = friendly ? currMinions.minionObjects[currMinions[index]] : enemyMinions.minionObjects[enemyMinions[index]];

        CheckHighlights();
        return m.TriggerBattlecry();
    }
    public Coroutine ConfirmTrigger(bool friendly, int index, bool deathrattle)
    {
        if (deathrattle)
        {
            return StartCoroutine(AnimationManager.Wait(30));
        }
        Creature m = friendly ? currMinions.minionObjects[currMinions[index]] : enemyMinions.minionObjects[enemyMinions[index]];

        CheckHighlights();
        return m.TriggerTrigger();
    }
    void ConfirmHeroPower(bool ally)
    {
        if (ally)
        {
            heroPower.Disable();
        }
        else
        {
            enemyHeroPower.Disable();
        }
        CheckHighlights();
    }
}
