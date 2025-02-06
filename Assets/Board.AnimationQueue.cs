using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public partial class Board
{
    public static IEnumerator Wait(int x)
    {
        yield return AnimationManager.Wait(x);
    }
    public class VisualInfo
    {
        public Server.MessageType type;

        public List<Creature> creatures = new();
        public List<Card.Cardname> names = new();
        public List<Vector3> vectors = new();
        public List<Card> cards = new();
        public List<Minion> minions = new List<Minion>();
        public List<HandCard> handCards = new();
        public List<string> strings = new();
        public List<int> ints = new();

        public bool isFriendly;
        public int health, damage, manaCost, index, target;
    }
    public void QueueAnimation(VisualInfo message)
    {
        visualMessageQueue.Enqueue(message);
        if (exec==false)
        {
            StartCoroutine(ExecuteAnimationQueue());
        }
    }
    Queue<VisualInfo> visualMessageQueue = new Queue<VisualInfo>();
    bool exec = false;
    IEnumerator ExecuteAnimationQueue()
    {
        exec = true;
        while (visualMessageQueue.Count>0)
        {
            Coroutine c = ResolveAnimation(visualMessageQueue.Dequeue());
            if (c == null) continue;
            yield return c;
        }
        exec = false;
    }

    public Coroutine ResolveAnimation(VisualInfo message)
    {
        switch(message.type)
        {
          
            case Server.MessageType.ConfirmMulligan:
                return ConfirmMulliganVisual(message);
                
            case Server.MessageType.StartGame:
                return StartGameVisual(message);
                
            case Server.MessageType.StartTurn:
                return StartTurnVisual(message);
                
            case Server.MessageType.SummonMinion:
                return SummonMinionVisual(message);
           
            case Server.MessageType.DestroyMinion:
                return DestroyMinionVisual(message);
            
            case Server.MessageType.DrawCards:
                return DrawVisual(message);

            case Server.MessageType.DrawEnemy:
                return DrawEnemyVisual(message);

            case Server.MessageType.DiscardCard:
                return DiscardVisual(message);

            case Server.MessageType.PlayCard:
                return PlayCardVisual(message);
                
            case Server.MessageType.ConfirmPreAttackFace:
            case Server.MessageType.ConfirmPreAttackMinion:
                return PreAttackMinionVisual(message);

            case Server.MessageType.ConfirmAttackFace:
            case Server.MessageType.ConfirmAttackMinion:
                return ConfirmAttackMinionVisual(message);

            case Server.MessageType.UpdateMinion:
                return UpdateMinionVisual(message);

            case Server.MessageType.UpdateHero:
                return UpdateHeroVisual(message);

                /*
            case Server.MessageType.ConfirmBattlecry:
                return UpdateHeroVisual(message);
            case Server.MessageType.ConfirmTrigger:
                return UpdateHeroVisual(message);
                */
            
            default:
                Debug.LogError("ANIMATION NOT IMPLEMENTED: " + message.type);
                return null;
        }
    }

    Coroutine ConfirmMulliganVisual(VisualInfo message)
    {
        return StartCoroutine(Wait(50));
    }
    Coroutine StartGameVisual(VisualInfo message)
    {

        waitingEnemyMulliganMessage.transform.localScale = Vector3.zero;
        currHand.ConfirmBothMulligans();
        currHand.OrderInds();
        currMinions = new MinionBoard();
        enemyMinions = new MinionBoard();
        enemyMinions.board = currMinions.board = this;
        currMinions.server = enemyMinions.server = false;
        currTurn = message.isFriendly;
        return null;
    }

    Coroutine StartTurnVisual(VisualInfo message)
    {
        Debug.Log(playerID+ " STARTING TURN: "+message.isFriendly);
        if (!message.isFriendly)
        {
            currTurn = false;
            enemyMana.SetMax(message.ints[1]);
            enemyMana.SetCurrent(message.ints[0]);
            enemyHeroPower.Enable();

            CheckHighlights();
            return null;
        }


        if (currTurn) return null;
        currTurn = true;

        mana.SetMax(message.ints[1]);
        mana.SetCurrent(message.ints[0]);

        heroPower.Enable();
        foreach (Minion m in currMinions)
        {
            m.canAttack = true;
        }
        CheckHighlights();
        return null;
    }

    Coroutine SummonMinionVisual(VisualInfo message)
    {
        MinionBoard board = message.isFriendly ? currMinions : enemyMinions;
        board.AddCreature(message.minions[0]);
        return StartCoroutine(Wait(15));
    }
    Coroutine DestroyMinionVisual(VisualInfo message)
    {
        MinionBoard board = message.isFriendly ? currMinions : enemyMinions;
        board.RemoveCreature(message.minions[0]);
        return StartCoroutine(Wait(15));
    }

    Coroutine PlayCardVisual(VisualInfo message)
    {
        Hand h = message.isFriendly ? currHand : enemyHand;
        h.RemoveCard(message.handCards[0], Hand.RemoveCardType.Play, message.names[0],message.index);

        if (message.isFriendly == false)
        {
            ShowEnemyPlay(message.names[0]);
            enemyMana.Spend(message.manaCost);
        }
        else
        {
            mana.Spend(message.manaCost);
        }


        if (message.isFriendly && currMinions.previewMinion != null && currMinions.currPreview == message.index)
        {
            if (currMinions.previewMinion.index == message.index) return null;
        }
        return StartCoroutine(Wait(10));
    }
    Coroutine DiscardVisual(VisualInfo message)
    {
        Hand h = message.isFriendly ? currHand : enemyHand;
        h.RemoveCard(message.handCards[0], Hand.RemoveCardType.Discard, message.names[0]);

        return StartCoroutine(Wait(1));
    }
    Coroutine DrawVisual(VisualInfo message)
    {
        currHand.AddCard(message.handCards[0],Hand.CardSource.Deck);
        return StartCoroutine(Wait(15));
    }
    Coroutine DrawEnemyVisual(VisualInfo message)
    {
        foreach (var c in message.handCards)
            enemyHand.AddCard(c, Hand.CardSource.Deck);
        return StartCoroutine(Wait(15));
    }
    Coroutine PreAttackMinionVisual(VisualInfo message)
    {
        return animationManager.PreAttackMinion(message.creatures[0], message.vectors[0]);
    }
    Coroutine ConfirmAttackMinionVisual(VisualInfo message)
    {
        return animationManager.ConfirmAttackMinion(message.creatures[0], message.vectors[0]);
    }

    Coroutine UpdateMinionVisual(VisualInfo message)
    {
        return null;
    }
    Coroutine UpdateHeroVisual(VisualInfo message)
    {
        return null;
    }

}
