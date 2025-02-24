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
        public List<SecretDisplay> secrets = new();
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
        //yield return null;
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
                
            case Server.MessageType.EquipWeapon:
                return EquipWeaponVisual(message);
           
            case Server.MessageType.DestroyWeapon:
                return DestroyWeaponVisual(message);
            
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

            case Server.MessageType.ConfirmPreSwingFace:
            case Server.MessageType.ConfirmPreSwingMinion:
                return PreSwingVisual(message);

            case Server.MessageType.ConfirmSwingFace:
            case Server.MessageType.ConfirmSwingMinion:
                return ConfirmSwingVisual(message);

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

            case Server.MessageType.Fatigue:
                return FatigueVisual(message);
                
            case Server.MessageType.AddCard:
                return AddCardVisual(message);

            case Server.MessageType.ConfirmHeroPower:
                return ConfirmHeroPowerVisual(message);

            case Server.MessageType.AddSecret:
                return AddSecretVisual(message);

            case Server.MessageType.TriggerSecret:
                return TriggerSecretVisual(message);
            case Server.MessageType.RemoveSecret:
                return RemoveSecretVisual(message);
                
            case Server.MessageType.AddAuraPlayer:
            case Server.MessageType.RemoveAuraPlayer:
                return AuraPlayerChangeVisual(message, message.type == Server.MessageType.RemoveAuraPlayer);

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

            case Server.MessageType.RemoveMinion:
                return RemoveMinionVisual(message);
                
            
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
        if (mulliganBG!=null) animationManager.SpriteFade(mulliganBG.gameObject, 10);
        CheckHighlights();
        return null;
    }
    Coroutine EndGameVisual(VisualInfo message)
    {
        Match.Result result = (Match.Result)message.ints[0];
        gameoverText.text = result == Match.Result.Draw ? "DRAW." : "YOU " + result.ToString().ToUpper()+".";

        return StartCoroutine(deather());
    }
    IEnumerator deather()
    {
        yield return Wait(20);
        animationManager.LerpTo(this.gameObject, new Vector3(this.transform.position.x, 40), 10, 0.2f);
    }
    Coroutine StartTurnVisual(VisualInfo message)
    {
        if (!message.isFriendly)
        {
            currTurn = false;
            enemyMana.SetMax(message.ints[1]);
            enemyMana.SetCurrent(message.ints[0]);
            enemyHeroPower.Enable();
            enemyHero.EnableWeapon();
            //currHero.DisableWeapon();
            CheckHighlights();
            return null;
        }


        if (currTurn) return null;
        currTurn = true;

        mana.SetMax(message.ints[1]);
        mana.SetCurrent(message.ints[0]);

        heroPower.Enable();
        currHero.EnableWeapon();
        enemyHero.DisableWeapon();

        foreach (Minion m in currMinions)
        {
            m.SICKNESS = false;
            m.canAttack = true;
        }
        currHero.canAttack = true;
        CheckHighlights();
        return null;
    }

    Coroutine SummonMinionVisual(VisualInfo message)
    {
        MinionBoard board = message.isFriendly ? currMinions : enemyMinions;
        board.AddCreature(message.minions[0], (MinionBoard.MinionSource)message.ints[0]);
        CheckHighlights();
        return StartCoroutine(Wait(15));
    }

    Coroutine RemoveMinionVisual(VisualInfo message)
    {
        MinionBoard board = message.isFriendly ? currMinions : enemyMinions;
        board.RemoveCreature(message.minions[0], false,true);
        return StartCoroutine(Wait(10));
    }

    Coroutine DestroyMinionVisual(VisualInfo message)
    {
        destroyChainQueue.Enqueue(message);
        bool going = destroying;
        if (!going) StartCoroutine(DestroyChain());

        if (visualMessageQueue.Count>0)
        {
            if (visualMessageQueue.Peek().type!=Server.MessageType.DestroyMinion)
            {
                return StartCoroutine(Wait(15));
            }
        }
        return null;
    }

    Queue<VisualInfo> destroyChainQueue = new Queue<VisualInfo>();
    bool destroying = false;
    IEnumerator DestroyChain()
    {
        destroying = true;
        yield return null; //One frame delay to fill up the destroy queue
        List<MinionBoard> boards = new List<MinionBoard>();
        while (destroyChainQueue.Count>0)
        {
            VisualInfo message = destroyChainQueue.Dequeue();
            MinionBoard board = message.isFriendly ? currMinions : enemyMinions;
            if (!boards.Contains(board)) boards.Add(board);
            board.RemoveCreature(message.minions[0], true);
        }
        foreach (var b in boards)
            b.OrderCreatures();
        destroying = false;
    }

    Coroutine PlayCardVisual(VisualInfo message)
    {
        Hand h = message.isFriendly ? currHand : enemyHand;
        bool wep = Database.GetCardData(message.names[0]).WEAPON;
        h.RemoveCard(message.handCards[0], Hand.RemoveCardType.Play, message.names[0],message.index,-1,wep);

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
        discardChainQueue.Enqueue(message);
        bool going = discarding;
        if (!going) StartCoroutine(DiscardChain());

        if (visualMessageQueue.Count > 0)
        {
            Server.MessageType t = visualMessageQueue.Peek().type;
            if (t!= Server.MessageType.DiscardCard && t != Server.MessageType.UpdateCard)
            {
                return StartCoroutine(Wait(15));
            }
        }
        return null;
    }

    Queue<VisualInfo> discardChainQueue = new Queue<VisualInfo>();
    bool discarding = false;
    IEnumerator DiscardChain()
    {
        discarding = true;
        yield return null; //One frame delay to fill up the destroy queue
        List<Hand> hands = new List<Hand>();
        while (discardChainQueue.Count > 0)
        {
            VisualInfo message = discardChainQueue.Dequeue();
            Hand hand = message.isFriendly ? currHand : enemyHand;
            if (!hands.Contains(hand)) hands.Add(hand);
            hand.RemoveCard(message.handCards[0], Hand.RemoveCardType.Discard, message.names[0], -1, message.isFriendly ? -1 : message.ints[0],false,true);
        }

        discarding = false;
        foreach (var h in hands)
        {
            StartCoroutine(DelayedOrderCards(h));
        }
    }

    bool orderDelay = false;
    IEnumerator DelayedOrderCards(Hand h)
    {
        if (orderDelay) yield break;
        orderDelay = true;
        yield return Wait(20);
        h.OrderCards();
        CheckHighlights();
        orderDelay = false;
    }


    Coroutine DrawVisual(VisualInfo message)
    {
        currHand.AddCard(message.handCards[0],Hand.CardSource.Deck);
        CheckHighlights();
        return StartCoroutine(Wait(30));
    }
    Coroutine DrawEnemyVisual(VisualInfo message)
    {
        foreach (var c in message.handCards)
            enemyHand.AddCard(c, Hand.CardSource.Deck);
        return StartCoroutine(Wait(15));
    }

    Coroutine EquipWeaponVisual(VisualInfo message)
    {
        Hero hero = message.isFriendly ? currHero : enemyHero;
        hero.DisplayWeapon();

        return StartCoroutine(Wait(10));
    }

    Coroutine DestroyWeaponVisual(VisualInfo message)
    {
        Hero hero = message.isFriendly ? currHero : enemyHero;
        hero.HideWeapon();

        return StartCoroutine(Wait(15));
        //return null;
    }

    Coroutine PreAttackMinionVisual(VisualInfo message)
    {
        if (message.minions.Count > 1) message.vectors.Insert(0, message.minions[1].creature.transform.localPosition);
        return animationManager.PreAttackMinion(message.minions[0].creature, message.vectors[0]);
    }
    Coroutine ConfirmAttackMinionVisual(VisualInfo message)
    {
        if (message.minions.Count > 1) message.vectors.Insert(0, message.minions[1].creature.transform.localPosition);
        return animationManager.ConfirmAttackMinion(message.minions[0].creature, message.vectors[0]);
    }
    Coroutine PreSwingVisual(VisualInfo message)
    {
        if (message.minions.Count > 0) message.vectors.Insert(0, message.minions[0].creature.transform.localPosition);
        return animationManager.PreSwing(message.isFriendly? currHero:enemyHero, message.vectors[0]);
    }
    Coroutine ConfirmSwingVisual(VisualInfo message)
    {
        if (message.minions.Count > 0) message.vectors.Insert(0, message.minions[0].creature.transform.localPosition);
        return animationManager.ConfirmSwing(message.isFriendly ? currHero : enemyHero, message.vectors[0]);
    }

    Coroutine UpdateMinionVisual(VisualInfo message)
    {
        message.minions[0].creature.hp = message.ints[0]; 
        message.minions[0].creature.maxhp = message.ints[2]; 
        message.minions[0].creature.dmg = message.ints[1];
        message.minions[0].creature.UpdateDisplay(); 

        if (message.trigger)
        {
            CreateSplash(message.minions[0].creature.gameObject, message.damage);
        }
        return null;
    }
    Coroutine UpdateHeroVisual(VisualInfo message)
    {

        Hero hero = message.isFriendly ? currHero : enemyHero;
        bool armIncrease = false;
        if (message.ints[5] > hero.armDisplay) armIncrease = true;
        if (message.isFriendly)
        {
            hero.UpdateText(message.ints[0], message.ints[4], message.ints[5]);
            deck.UpdateDisplay(message.ints[1]);
            mana.UpdateDisplay(message.ints[2], message.ints[3]);
        }
        else
        {
            hero.UpdateText(message.ints[0], message.ints[4], message.ints[5]);
            enemyDeck.UpdateDisplay(message.ints[1]);
            enemyMana.UpdateDisplay(message.ints[2], message.ints[3]);
        }

        hero.UpdateWeaponText(message.ints[6], message.ints[7]);

        if (message.trigger)
        {
            CreateSplash(hero.spriteRenderer.gameObject, message.damage);
        }
        if (armIncrease) return StartCoroutine(Wait(10));
        return null;
    }
    Coroutine UpdateCardVisual(VisualInfo message)
    {
        Card c = message.handCards[0].cardObject;
        if (c == null) return null;
        if (c.card.played) return null;
        c.UpdateManaCost();
        return null;
    }
    Coroutine AddSecretVisual(VisualInfo message)
    {
        Hero h = message.isFriendly ? currHero : enemyHero;
        //h.AddSecret(message.names[0]);
        h.OrderSecrets();
        return StartCoroutine(Wait(15));
    }
    Coroutine TriggerSecretVisual(VisualInfo message)
    {
        ShowEnemyPlay(message.secrets[0].card,-1,!message.isFriendly);
        return StartCoroutine(message.secrets[0].TriggerAnim());
    }
    Coroutine RemoveSecretVisual(VisualInfo message)
    {
        Debug.LogError("UNIMPLEMENTED REMOVESECRET");
        return StartCoroutine(Wait(15));
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
            if (message.health == -1)
            {
                return message.isFriendly ? currHero.TriggerTrigger() : enemyHero.TriggerTrigger();
            }
            return StartCoroutine(Wait(30));
        }

        if (message.minions[0].creature == null) return null;
        Creature m = message.minions[0].creature; 
        return m.TriggerTrigger();
    }

    Coroutine AddCardVisual(VisualInfo message)
    {
        //Hand hand = message.isFriendly ? currHand : enemyHand;
        //hand.AddCard(message.handCards[0], Hand.CardSource.Custom, message.vectors[0]);
        addChainQueue.Enqueue(message);
        bool going = adding;
        if (!going) StartCoroutine(AddChain());

        if (visualMessageQueue.Count > 0)
        {
            Server.MessageType t = visualMessageQueue.Peek().type;
            if (t != Server.MessageType.AddCard && t!=Server.MessageType.RemoveMinion)
            {
                return StartCoroutine(Wait(15));
            }
        }
        return null;
    }

    Queue<VisualInfo> addChainQueue = new Queue<VisualInfo>();
    bool adding = false;
    IEnumerator AddChain()
    {
        adding = true;
        yield return null; //One frame delay to fill up the destroy queue
        List<Hand> hands = new List<Hand>();
        while (addChainQueue.Count > 0)
        {
            VisualInfo message = addChainQueue.Dequeue();
            Hand hand = message.isFriendly ? currHand : enemyHand;
            if (!hands.Contains(hand)) hands.Add(hand);
            hand.AddCard(message.handCards[0], Hand.CardSource.Custom, message.vectors[0], true);
        }

        adding = false;
        foreach (var h in hands)
        {
            StartCoroutine(DelayedOrderCards(h));
        }
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

    Coroutine FatigueVisual(VisualInfo message)
    {
        Card.Cardname card = Card.Cardname.Fatigue;
        bool friendly = message.isFriendly;
        Card c = CreateCard();
        c.GetComponent<BoxCollider2D>().enabled = false;
        c.transform.parent = deck.transform.parent;
        c.Set(new HandCard(card, 0));
        c.UpdateCardText(message.damage);
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
    Coroutine AuraPlayerChangeVisual(VisualInfo message, bool remove)
    {
        bool state = !remove;
        Aura.Type type = (Aura.Type)message.ints[0];
        Hero hero = message.isFriendly ? currHero : enemyHero;
        switch (type)
        {
            case Aura.Type.Freeze:
                hero.FREEZE = state;
                break;
            case Aura.Type.Immune:
                hero.IMMUNE = state;
                break;
        }
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
