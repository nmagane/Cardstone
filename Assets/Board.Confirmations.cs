using UnityEngine;
using Riptide;
using System.Data;

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
    /*
    public Server.CustomMessage CopyMessage(Message message, Server.MessageType type)
    {
        Server.CustomMessage result = new Server.CustomMessage();
        result.type = type;
        switch (type)
        {
            case Server.MessageType._TEST:
                break;
            case Server.MessageType.ConfirmMatch:
                ulong matchID = message.GetULong();
                result.AddULong(matchID);
                break;
            case Server.MessageType.DrawHand:
                ushort[] hand = message.GetUShorts();
                int enemyCardCount = message.GetInt();
                result.AddUShorts(hand);
                result.AddInt(enemyCardCount);
                break;
            case Server.MessageType.ConfirmMulligan:
                ushort[] mulliganNewHand = message.GetUShorts();
                result.AddUShorts(mulliganNewHand);
                break;
            case Server.MessageType.EnemyMulligan:
                int enemyMulligan = message.GetInts();
                result.AddInts(enemyMulligan);
                break;
            case Server.MessageType.DrawCards:
                int draw = message.GetInt();
                result.AddInt(draw);
                break;
            case Server.MessageType.DrawEnemy:
                int enemyDraws = message.GetInt();
                result.AddInt(enemyDraws);
                break;
            case Server.MessageType.StartGame:
                bool isTurn = message.GetBool();
                result.AddBool(isTurn);
                break;
            case Server.MessageType.StartTurn:
                bool startAllyTurn = message.GetBool();
                int startMaxMana = message.GetInt();
                int startCurrMana = message.GetInt();
                ushort startMessageCount = message.GetUShort();
                result.AddBool(startAllyTurn);
                result.AddInt(startMaxMana);
                result.AddInt(startCurrMana);
                result.AddUShort(startMessageCount);
                break;
            case Server.MessageType.EndTurn:
                break;
            case Server.MessageType.PlayCard:
                bool playedFriendlySide = message.GetBool();
                int playedIndex = message.GetInt();
                int playedManaCost = message.GetInt();
                int playedCard = message.GetInt();
                result.AddBool(playedFriendlySide);
                result.AddInt(playedIndex);
                result.AddInt(playedManaCost);
                result.AddInt(playedCard);
                break;
            case Server.MessageType.SummonMinion:
                bool summonedFriendlySide = message.GetBool();
                int summonedMinion = message.GetInt();
                int summonedPos = message.GetInt();
                result.AddBool(summonedFriendlySide);
                result.AddInt(summonedMinion);
                result.AddInt(summonedPos);
                break;
            case Server.MessageType.UpdateMinion:
                string minionUpdateJson = message.GetString();
                bool minionUpdateFriendly = message.GetBool();
                result.AddString(minionUpdateJson);
                result.AddBool(minionUpdateFriendly);
                break;
            case Server.MessageType.ConfirmPreAttackMinion:
                bool preminionAllyAttack = message.GetBool();
                int preminionAttackerIndex = message.GetInt();
                int preminionTargetIndex = message.GetInt();
                result.AddBool(preminionAllyAttack);
                result.AddInt(preminionAttackerIndex);
                result.AddInt(preminionTargetIndex);
                break;
            case Server.MessageType.ConfirmAttackMinion:
                bool minionAllyAttack = message.GetBool();
                int minionAttackerIndex = message.GetInt();
                int minionTargetIndex = message.GetInt();
                result.AddBool(minionAllyAttack);
                result.AddInt(minionAttackerIndex);
                result.AddInt(minionTargetIndex);
                break;
            case Server.MessageType.ConfirmPreAttackFace:
                bool preFaceAllyAttack = message.GetBool();
                int preFaceAttackerIndex = message.GetInt();
                result.AddBool(preFaceAllyAttack);
                result.AddInt(preFaceAttackerIndex);
                break;
            case Server.MessageType.ConfirmAttackFace:
                bool FaceAllyAttack = message.GetBool();
                int FaceAttackerIndex = message.GetInt();
                result.AddBool(FaceAllyAttack);
                result.AddInt(FaceAttackerIndex);
                break;
            case Server.MessageType.DestroyMinion:
                int DestroyInd = message.GetInt();
                bool DestroyFriendly = message.GetBool();
                result.AddInt(DestroyInd);
                result.AddBool(DestroyFriendly);
                break;
            case Server.MessageType.UpdateHero:
                int UpdateHeroHP = message.GetInt();
                bool UpdateHeroFriendly = message.GetBool();
                result.AddInt(UpdateHeroHP);
                result.AddBool(UpdateHeroFriendly);
                break;
            case Server.MessageType.AddAura:
            case Server.MessageType.RemoveAura:
                int addAuraMinionIndex = message.GetInt();
                bool addAuraFriendly = message.GetBool();
                ushort addAuraType = message.GetUShort();
                ushort addAuraValue= message.GetUShort();
                bool addAuraTemp = message.GetBool();
                bool addAuraForeign = message.GetBool();
                int addAuraSourceInd = message.GetInt();
                bool addAuraSourceFriendly = message.GetBool();
                result.AddInt(addAuraMinionIndex);
                result.AddBool(addAuraFriendly);
                result.AddUShort(addAuraType);
                result.AddUShort(addAuraValue);
                result.AddBool(addAuraTemp);
                result.AddBool(addAuraForeign);
                result.AddInt(addAuraSourceInd);
                result.AddBool(addAuraSourceFriendly);
                break;
            case Server.MessageType.AddTrigger:
            case Server.MessageType.RemoveTrigger:
                int addTriggerMinionIndex = message.GetInt();
                bool addTriggerFriendly = message.GetBool();
                ushort addTriggerType = message.GetUShort();
                ushort addTriggerSide = message.GetUShort();
                ushort addTriggerAbility = message.GetUShort();
                result.AddInt(addTriggerMinionIndex);
                result.AddBool(addTriggerFriendly);
                result.AddUShort(addTriggerType);
                result.AddUShort(addTriggerSide);
                result.AddUShort(addTriggerAbility);
                break;
            case Server.MessageType.DiscardCard:
                bool discardFriendly = message.GetBool();
                int discardCardInd = message.GetInt();
                int discardCardName = message.GetInt();
                result.AddBool(discardFriendly);
                result.AddInt(discardCardInd);
                result.AddInt(discardCardName);
                break;
            case Server.MessageType.MillCard:
                bool millFriendly = message.GetBool();
                int millCardName = message.GetInt();
                result.AddBool(millFriendly);
                result.AddInt(millCardName);
                break;
            case Server.MessageType.ConfirmHeroPower:
                bool heroPowerFriendly = message.GetBool();
                int heroPowerManaCost = message.GetInt();
                result.AddBool(heroPowerFriendly);
                result.AddInt(heroPowerManaCost);
                break;
            default:
                Debug.LogError("UNKNOWN MESSAGE TYPE");
                break;
        }

        return result;
    }*/
    public void ConfirmPreAttackMinion(bool allyAttack, int attackerIndex, int targetIndex)
    {
        //TODO: preattack animation
    }
    public void ConfirmAttackMinion(bool allyAttack, int attackerIndex, int targetIndex)
    {
        
        Minion attacker = allyAttack ? currMinions[attackerIndex] : enemyMinions[attackerIndex];
        Minion target = allyAttack ? enemyMinions[targetIndex] : currMinions[targetIndex];

        if (allyAttack)
        {
            Server.ConsumeAttackCharge(attacker);
        }

        //Debug.Log(playerID + ": " + (allyAttack ? "ally " : "enemy ") + attacker.ToString() + " hits " + target.ToString());
        //TODO: attack animation
    }

    public void ConfirmPreAttackFace(bool allyAttack, int attackerIndex)
    {
        //TODO: preattack animation
    }
    void ConfirmAttackFace(bool allyAttack, int attackerIndex)
    {

        Minion attacker = allyAttack ? currMinions[attackerIndex] : enemyMinions[attackerIndex];

        if (allyAttack)
        {
            Server.ConsumeAttackCharge(attacker);
        }
        Debug.Log(playerID + ": " + (allyAttack ? "ally " : "enemy ") + attacker.ToString() + " hits face");
        //todo: attack face animation
    }

    void ConfirmHeroPower(bool ally, int manaCost)
    {
        if (ally)
        {
            heroPower.Disable();
            mana.Spend(manaCost);
        }
        else
        {
            enemyHeroPower.Disable();
            enemyMana.Spend(manaCost);
        }
    }
}
