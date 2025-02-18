using UnityEngine;
using System.Collections.Generic;

public partial class Board
{
    public void ConfirmDisconnect()
    {
#if UNITY_EDITOR
        if (playerID == 101) return;
#endif
        mainmenu.ConfirmDisconnect();

        if (!gameStarted) return;
        gameoverText.text = "DISCONNECTED.";
        animationManager.LerpTo(this.gameObject, new Vector3(this.transform.position.x, 40), 10, 0.2f);
    }
    
    public void ConfirmConnection()
    {
#if UNITY_EDITOR
        if (playerID == 101)
        {
            Debug.Log("101 connected!");
            return;
        }
#endif
        mainmenu.ConfirmConnection();
    }

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
        //response: yes, concede message.
        if (UNORDERED == false && currTurn == false)
            return;
        message.order = matchMessageOrder;
#if UNITY_EDITOR
        message.clientID = playerID!=101? 100:(int)playerID;
#else
        message.clientID = (int)playerID;
#endif
        //client.Send(message);
        mirror.SendClient(message);
        if (UNORDERED == false)
            matchMessageOrder++;
    }

    bool gameStarted = false;
    public void InitGame(ulong matchID, string enemyName, Card.Class allyClass, Card.Class enemyClass)
    {
        Debug.Log("Player " + playerID + " entered game " + matchID);
        Camera.main.transform.position = new Vector3(0, 0, -10);
        gameStarted = true;
        //TODO: ENEMY CLASS COMMUNICATED IN MESSAGE

        playerNameText.text = playerName;
        enemyNameText.text = enemyName;

        currHero.Set(allyClass);
        enemyHero.Set(enemyClass);
        heroPower.Set(allyClass, Database.GetClassHeroPower(allyClass));
        enemyHeroPower.Set(enemyClass,Database.GetClassHeroPower(enemyClass));

        currentMatchID = matchID;
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
        mulliganButton.transform.localScale = Vector3.zero;

        VisualInfo anim = new VisualInfo();
        anim.type = Server.MessageType.ConfirmMulligan;
        QueueAnimation(anim);
    }
    List<int> enemyMulls;
    void ConfirmEnemyMulligan(List<int> inds)
    {
        enemyMulls = inds;
    }
    void StartGame(bool isTurn)
    {
        VisualInfo anim = new VisualInfo();
        anim.type = Server.MessageType.StartGame;
        anim.isFriendly = isTurn;
        QueueAnimation(anim);
    }

    void EndGame(Match.Result result)
    {
        disableInput = true;
        currTurn = false;
        CheckHighlights();

        VisualInfo anim = new VisualInfo();
        anim.type = Server.MessageType.EndGame;
        anim.ints.Add((int)result);
        QueueAnimation(anim);
    }
    public void AddCard(bool friendly, Card.Cardname card, bool sourceFriendly, int sourceIndex, int costChange)
    {
        Hand hand = friendly ? currHand : enemyHand;
        HandCard c = hand.Add(card);

        if (costChange != 0) c.manaCost += costChange;

        Vector3 pos = Vector3.zero;
        if (sourceIndex != -1)
        {
            MinionBoard board = sourceFriendly ? currMinions : enemyMinions;
            if (sourceIndex < board.Count())
            {
                if (board[sourceIndex].creature != null)
                    pos = board[sourceIndex].creature.transform.position;
            }
        }
        VisualInfo anim = new VisualInfo();
        anim.type = Server.MessageType.AddCard;
        anim.isFriendly = friendly;
        anim.handCards.Add(c);
        anim.vectors.Add(pos);
        QueueAnimation(anim);
    }

    public void ConfirmPlayPlayer(HandCard card, int pos)
    {
        //return;
        currHand.RemoveAt(card.index);
        currHand.RemoveCard(card,Hand.RemoveCardType.Play,card.card,pos,-1,card.WEAPON);
        currHero.combo = true;
        if (card.MINION)
        {
            int p = pos;

            if (pos >= 7) pos = currMinions.Count();
            Minion m = new Minion(card.card, pos, currMinions);
            prePlayMinions.Add(p, m);
            currMinions.AddCreature(m);
        }
        if (card.WEAPON)
        {
            preplayWep = true;
            currHero.EquipWeapon(card.card);
            currHero.DisplayWeapon();
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

    bool preplayWep = false;
    public void EquipWeapon(bool friendlySide, Card.Cardname card)
    {
        if (friendlySide)
        {
            if (currHero.weapon != null && preplayWep)
            {
                if (currHero.weapon.card == card)
                {

                    preplayWep = false;
                }//preplay confirmed correct? idk [TODO]
                    return;
            }
            else currHero.EquipWeapon(card);
        }
        else
        {
            enemyHero.EquipWeapon(card);
        }

        VisualInfo anim = new VisualInfo();
        anim.type = Server.MessageType.EquipWeapon;
        anim.isFriendly = friendlySide;
        QueueAnimation(anim);
    }
    public void DestroyWeapon(bool friendlySide)
    {
        if (friendlySide)
        {
            currHero.newWep = false;
            currHero.DestroyWeapon();
        }
        else
        {
            enemyHero.newWep = false;
            enemyHero.DestroyWeapon();
        }

        VisualInfo anim = new VisualInfo();
        anim.type = Server.MessageType.DestroyWeapon;
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
    public Coroutine ConfirmAttackMinion(bool allyAttack, int attackerIndex, int targetIndex, bool canAttack, bool friendlyFire)
    {
        
        Minion attacker = allyAttack ? currMinions[attackerIndex] : enemyMinions[attackerIndex];
        Creature atkCreature = allyAttack ? currMinions.minionObjects[attacker] : enemyMinions.minionObjects[attacker];

        Minion target = allyAttack && !friendlyFire ? enemyMinions[targetIndex] : currMinions[targetIndex];
        //Creature tarCreature = allyAttack && !friendlyFire ? enemyMinions.minionObjects[target] : currMinions.minionObjects[target];

        if (allyAttack)
        {
            attacker.canAttack = canAttack;
        }

        CheckHighlights();


        VisualInfo anim = new VisualInfo();
        anim.type = Server.MessageType.ConfirmAttackMinion;
        anim.creatures.Add(atkCreature);
        anim.minions.Add(target);
        //anim.vectors.Add(tarCreature.transform.localPosition);

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
    public Coroutine ConfirmAttackFace(bool allyAttack, int attackerIndex, bool canAttack, bool friendlyFire)
    {
        Minion attacker = allyAttack ? currMinions[attackerIndex] : enemyMinions[attackerIndex];
        Creature atkCreature = allyAttack ? currMinions.minionObjects[attacker] : enemyMinions.minionObjects[attacker];
        Hero tar = allyAttack && !friendlyFire ? enemyHero : currHero;
        if (allyAttack)
        {
            attacker.canAttack = canAttack;
        }

        VisualInfo anim = new VisualInfo();
        anim.type = Server.MessageType.ConfirmAttackFace;
        anim.creatures.Add(atkCreature);
        anim.vectors.Add(tar.transform.localPosition);

        QueueAnimation(anim);

        return null;
    }

    public void ConfirmPreSwingMinion(bool allyAttack, int targetIndex)
    {
        Hero attacker = allyAttack ? currHero : enemyHero;
        Minion target = allyAttack ? enemyMinions[targetIndex] : currMinions[targetIndex];

        Creature tarCreature = allyAttack ? enemyMinions.minionObjects[target] : currMinions.minionObjects[target];

        CheckHighlights();

        VisualInfo anim = new VisualInfo();
        anim.type = Server.MessageType.ConfirmPreSwingMinion;
        anim.isFriendly = allyAttack;
        anim.vectors.Add(tarCreature.transform.localPosition);

        QueueAnimation(anim);
    }
    public void ConfirmSwingMinion(bool allyAttack, int targetIndex,bool canAttack, bool friendlyFire=false)
    {
        Hero attacker = allyAttack ? currHero : enemyHero;

        Minion target = allyAttack && !friendlyFire ? enemyMinions[targetIndex] : currMinions[targetIndex];
        Creature tarCreature = allyAttack && !friendlyFire ? enemyMinions.minionObjects[target] : currMinions.minionObjects[target];
        
        if (allyAttack)
        {
            attacker.canAttack = canAttack;
        }
        CheckHighlights();

        VisualInfo anim = new VisualInfo();
        anim.type = Server.MessageType.ConfirmSwingMinion;
        anim.isFriendly = allyAttack;
        anim.vectors.Add(tarCreature.transform.localPosition);

        QueueAnimation(anim);
    }
    public void ConfirmPreSwingFace(bool allyAttack)
    {
        Hero attacker = allyAttack ? currHero : enemyHero;

        Hero tarHero = allyAttack ? enemyHero : currHero;

        CheckHighlights();

        VisualInfo anim = new VisualInfo();
        anim.type = Server.MessageType.ConfirmPreSwingFace;
        anim.isFriendly = allyAttack;
        anim.vectors.Add(tarHero.transform.localPosition);

        QueueAnimation(anim);
    }
    public void ConfirmSwingFace(bool allyAttack, bool canAttack, bool friendlyFire=false)
    {
        Hero attacker = allyAttack ? currHero : enemyHero;

        Hero tarHero = allyAttack && !friendlyFire ? enemyHero : currHero;

        if (allyAttack)
        {
            attacker.canAttack = canAttack;
        }
        CheckHighlights();

        VisualInfo anim = new VisualInfo();
        anim.type = Server.MessageType.ConfirmSwingFace;
        anim.isFriendly = allyAttack;
        anim.vectors.Add(tarHero.transform.localPosition);

        QueueAnimation(anim);
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
    public Coroutine ConfirmTrigger(bool friendly, int index, bool deathrattle, bool weapon)
    {
        if (weapon)
        {
            VisualInfo animWP = new();
            animWP.type = Server.MessageType.ConfirmTrigger;
            animWP.trigger = true;
            animWP.isFriendly = friendly;
            animWP.health = -1;

            QueueAnimation(animWP);
            return null;
        }
        if (deathrattle)
        {
            VisualInfo animDR = new();
            animDR.type = Server.MessageType.ConfirmTrigger;
            animDR.trigger = true;

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

    void ConfirmAddSecret(bool ally, Card.Cardname card)
    {
        Hero h = ally ? currHero : enemyHero;
        h.AddSecret(card, true);

        VisualInfo anim = new();
        anim.type = Server.MessageType.AddSecret;
        anim.isFriendly = ally;
        anim.names.Add(card);

        QueueAnimation(anim);
    }
    void ConfirmTriggerSecret(bool ally, int index, Card.Cardname card)
    {
        Hero h = ally ? currHero : enemyHero;
        if (ally & h.secrets[index].card != card) Debug.LogError("WRONG SECRET");
        SecretDisplay s = h.RemoveAt(index,true);
        s.Set(card);

        VisualInfo anim = new();
        anim.type = Server.MessageType.TriggerSecret;
        anim.isFriendly = ally;
        anim.secrets.Add(s);

        QueueAnimation(anim);
    }
    void ConfirmRemoveSecret(bool ally, int index, Card.Cardname card)
    {
        //TODO: REMOVE SECRET WITHOUT TRIGGERING
    }
    private void AuraPlayerChange(bool friendly, Aura.Type type, bool remove)
    {
        Hero h = friendly ? currHero : enemyHero;
        switch (type)
        {
            case Aura.Type.Freeze:
                h.FREEZE = !remove;
                break;
            case Aura.Type.Immune:
                h.IMMUNE = !remove;
                break;
        }
    }
    void ConfirmAnimation(AnimationInfo animInfo, bool friendly)
    {
        AnimationManager.AnimationData data = TranslateAnimationInfo(animInfo,friendly);

        //===============================
        VisualInfo anim = new();
        anim.type = Server.MessageType.ConfirmAnimation;
        anim.isFriendly = friendly;
        anim.anim = data;

        QueueAnimation(anim);
    }

    AnimationManager.AnimationData TranslateAnimationInfo(AnimationInfo info, bool friendly)
    {
        if (friendly == false)
        {
            info.sourceIsFriendly = !info.sourceIsFriendly;
            info.targetIsFriendly = !info.targetIsFriendly;
        }

        AnimationManager.AnimationData data = new();
        data.card = info.card;
        data.friendly = friendly;
        data.sourceIsHero = info.sourceIsHero;
        data.targetIsHero = info.targetIsHero;

        //============
        if (info.sourceIsHero)
        {
            data.sourceMinion = null;
            data.sourceHero = info.sourceIsFriendly ? currHero : enemyHero;
        }
        else
        {
            data.sourceHero = null;
            data.sourceMinion = info.sourceIsFriendly ? currMinions[info.sourceIndex] : enemyMinions[info.sourceIndex];
        }

        //=========
        if (info.targetIsHero)
        {
            data.targetMinion = null;
            data.targetHero = info.targetIsFriendly ? currHero : enemyHero;
        }
        else
        {
            data.targetHero = null;
            data.targetMinion = info.targetIsFriendly ? currMinions[info.targetIndex] : enemyMinions[info.targetIndex];
        }
        //========
        return data;
    }
}
