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
        public Server.CustomMessage customMessage = new();
        public AnimationManager.AnimationData anim = new();

        public bool isFriendly;
        public bool trigger = false;
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
            VisualInfo info = visualMessageQueue.Dequeue();

            Coroutine c = ResolveAnimation(info);
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
                
            case Server.MessageType.EndGame:
                return EndGameVisual(message);
                
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

            case Server.MessageType.UpdateCard:
                return UpdateCardVisual(message);

            case Server.MessageType.ConfirmBattlecry:
                return ConfirmBattlecryVisual(message);

            case Server.MessageType.ConfirmTrigger:
                return ConfirmTriggerVisual(message);

            case Server.MessageType.MillCard:
                return MillVisual(message);

            case Server.MessageType.ConfirmHeroPower:
                return ConfirmHeroPowerVisual(message);
                
            case Server.MessageType.AddAura:
                return AddAuraVisual(message);
                
            case Server.MessageType.RemoveAura:
                return RemoveAuraVisual(message);

            case Server.MessageType.AddTrigger:
                return AddTriggerVisual(message);
                
            case Server.MessageType.RemoveTrigger:
                return RemoveTriggerVisual(message);
                
            case Server.MessageType.ConfirmAnimation:
                return ConfirmAnimationVisual(message);
                
            
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

        foreach (int i in enemyMulls)
        {
            animationManager.MulliganEnemyAnim(enemyHand.cardObjects[enemyHand[i]]);
        }
        enemyMulls.Clear();

        currHand.OrderInds();
        currMinions = new MinionBoard();
        enemyMinions = new MinionBoard();
        enemyMinions.board = currMinions.board = this;
        currMinions.server = enemyMinions.server = false;
        currTurn = message.isFriendly;
        if (mulliganBG!=null) animationManager.SpriteFade(mulliganBG.gameObject, 10, true);
        return null;
    }
    Coroutine EndGameVisual(VisualInfo message)
    {
        Match.Result result = (Match.Result)message.ints[0];
        gameoverText.text = result == Match.Result.Draw ? "DRAW." : "YOU " + result.ToString().ToUpper()+".";
        
        return animationManager.LerpTo(this.gameObject,new Vector3(this.transform.position.x,40),10,0.2f);
    }
    Coroutine StartTurnVisual(VisualInfo message)
    {
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
        CheckHighlights();
        return StartCoroutine(Wait(15));
    }
    Coroutine DestroyMinionVisual(VisualInfo message)
    {
        MinionBoard board = message.isFriendly ? currMinions : enemyMinions;
        board.RemoveCreature(message.minions[0]);

        if (visualMessageQueue.Count == 0)
        {
            return StartCoroutine(Wait(15));
        }
        else if (visualMessageQueue.Peek().type == Server.MessageType.DestroyMinion) return null;
        else return StartCoroutine(Wait(15));
    }

    Coroutine PlayCardVisual(VisualInfo message)
    {
        Hand h = message.isFriendly ? currHand : enemyHand;
        h.RemoveCard(message.handCards[0], Hand.RemoveCardType.Play, message.names[0],message.index);

        if (message.isFriendly == false)
        {
            ShowEnemyPlay(message.names[0], message.manaCost);
            //enemyMana.Spend(message.manaCost);
        }
        else
        {
            //mana.Spend(message.manaCost);
        }


        if (message.isFriendly && currMinions.previewMinion != null && currMinions.currPreview == message.index)
        {
            if (currMinions.previewMinion.index == message.index) return null;
        }
        return StartCoroutine(Wait(8));
    }
    Coroutine DiscardVisual(VisualInfo message)
    {
        Hand h = message.isFriendly ? currHand : enemyHand;
        h.RemoveCard(message.handCards[0], Hand.RemoveCardType.Discard, message.names[0], -1, message.ints[0]);

        return StartCoroutine(Wait(1));
    }
    Coroutine DrawVisual(VisualInfo message)
    {
        currHand.AddCard(message.handCards[0],Hand.CardSource.Deck);
        CheckHighlights();
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
        message.minions[0].creature.hp = message.ints[0]; 
        message.minions[0].creature.maxhp = message.ints[2]; 
        message.minions[0].creature.dmg = message.ints[1];
        message.minions[0].creature.UpdateDisplay(); 

        if (message.trigger)
        {
            CreateSplash(message.minions[0].creature, message.damage);
        }
        return null;
    }
    Coroutine UpdateHeroVisual(VisualInfo message)
    {
        if (message.isFriendly)
        {
            currHero.UpdateText(message.ints[0]);
            deck.UpdateDisplay(message.ints[1]);
            mana.UpdateDisplay(message.ints[2], message.ints[3]);
        }
        else
        {
            enemyHero.UpdateText(message.ints[0]);
            enemyDeck.UpdateDisplay(message.ints[1]);
            enemyMana.UpdateDisplay(message.ints[2], message.ints[3]);
        }

        if (message.trigger)
        {
            CreateSplash(message.isFriendly ? currHero : enemyHero, message.damage);
        }
        return null;
    }
    Coroutine UpdateCardVisual(VisualInfo message)
    {
        Card c = message.handCards[0].cardObject;
        c.UpdateManaCost();
        return null;
    }

    Coroutine ConfirmBattlecryVisual(VisualInfo message)
    {
        bool friendly = message.isFriendly;
        int index = message.index;

        if (message.minions[0].creature == null) return null;
        Creature m = message.minions[0].creature;
        return m.TriggerBattlecry();
    }
    Coroutine ConfirmTriggerVisual(VisualInfo message)
    {
        if (message.trigger==true)
        {
            return StartCoroutine(Wait(30));
        }

        if (message.minions[0].creature == null) return null;
        Creature m = message.minions[0].creature; 
        return m.TriggerTrigger();
    }

    Coroutine MillVisual(VisualInfo message)
    {
        Card.Cardname card = message.names[0];
        bool friendly = message.isFriendly;
        Card c = CreateCard();
        c.GetComponent<BoxCollider2D>().enabled = false;
        c.transform.parent = deck.transform.parent;
        c.Set(new HandCard(card, 0));
        c.transform.localPosition = (friendly) ? deck.transform.localPosition : enemyDeck.transform.localPosition;
        c.SetFlipped();
        c.Flip();
        animationManager.MillAnim(c, friendly);

        return StartCoroutine(Wait(15));
    }

    Coroutine ConfirmHeroPowerVisual(VisualInfo message)
    {
        
        if (message.isFriendly)
        {
            heroPower.Disable();
        }
        else
        {
            enemyHeroPower.Disable();
        }

        return null;
    }

    Coroutine RemoveAuraVisual(VisualInfo message)
    {
        if (message.minions[0].creature == null) return null;
        message.minions[0].creature.CheckAuras();
        CheckHighlights();
        return null;
    }
    Coroutine AddAuraVisual(VisualInfo message)
    {
        if (message.minions[0].creature == null) return null;
        message.minions[0].creature.CheckAuras();
        CheckHighlights();
        return null;
    }

    Coroutine AddTriggerVisual(VisualInfo message)
    {
        if (message.minions[0].creature == null) return null;
        message.minions[0].creature.CheckTriggers();
        return null;
    }
    Coroutine RemoveTriggerVisual(VisualInfo message)
    {
        if (message.minions[0].creature == null) return null;
        message.minions[0].creature.CheckTriggers();
        return null;
    }

    Coroutine ConfirmAnimationVisual(VisualInfo message)
    {
        AnimationManager.AnimationData animData = message.anim;
        return animationManager.StartAnimation(animData);
    }
}
