using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public partial class Board : MonoBehaviour
{
    public AnimationManager animationManager;
    public SaveManager saveManager;
    public SaveManager.GameSave saveData => saveManager.saveData;
    public Mainmenu mainmenu;
    public GameObject gameAnchor;   
    public UIButton mulliganButton;
    public UIButton endTurnButton;
    public GameObject waitingEnemyMulliganMessage;
    public SpriteRenderer secretPopup;

    public Card hoveredCard = null;
    public BoardSide hoveredSide = null;
    public Creature hoveredMinion = null;
    public Hero hoveredHero = null;

    public BoardSide friendlySide;
    public BoardSide enemySide;

    public GameObject cardObject;
    public GameObject minionObject;
    public GameObject UISprite;
    public GameObject splashObject;

    public ulong playerID = 100;
    public SaveManager.Decklist currDecklist;
    public string playerName = "Player";
    public ulong currentMatchID;

    public Hand currHand = new Hand();
    public Hand enemyHand = new Hand();
    public bool currTurn = false;
    public MinionBoard currMinions;
    public MinionBoard enemyMinions;
    public Hero currHero;
    public Hero enemyHero;

    public ManaBar mana;
    public ManaBar enemyMana;

    public HeroPower heroPower;
    public HeroPower enemyHeroPower;

    public Deck deck;
    public Deck enemyDeck;
    public int currMana => mana.curr;
    public bool combo => currHero.combo;
    public int spellpower => currHero.spellpower;

    public TMP_Text gameoverText;
    public TMP_Text playerNameText;
    public TMP_Text enemyNameText;
    public SpriteRenderer mulliganBG;


    public Card CreateCard()
    {
        Card c = Instantiate(cardObject).GetComponent<Card>();
        return c;
    }
    public Splash CreateSplash(GameObject obj,int value)
    {
        if (value == 0) return null;
        Splash splash = Instantiate(splashObject).GetComponent<Splash>();
        Splash.Type type = value < 0 ? Splash.Type.Damage : Splash.Type.Heal;
        splash.Set(type, value, obj.gameObject);
        return splash;
    }
    public Creature CreateCreature()
    {
        Creature c = Instantiate(minionObject).GetComponent<Creature>();
        return c;
    }
    public void DestroyObject(MonoBehaviour o)
    {
        Destroy(o.gameObject);
    }
    public NetworkHandler mirror;

    //===================
    
    private static Board _instance;
    private static Board _instanceTest;

    public static Board Instance { get { return _instance; } }
    public static Board InstanceTest { get { return _instanceTest; } }


    private void Awake()
    {
        if (playerID == 101)
        {
            if (_instanceTest != null && _instanceTest != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instanceTest = this;
            }
        }
        else
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
            }
        }
    }
    
    //==================
    void Start()
    {
        Application.targetFrameRate = 60;
#if (UNITY_EDITOR == false)
        //playerID = (ulong)Random.Range(-1000000, 1000000);
#endif

        
        if (mirror.StartClient() ==false)
        {
            ConfirmConnection();
        }
            
        currHand.board = this;
        currHand.server = false;
    }

    public int matchMessageOrder = 0;
    public int messageReceivedOrder = 0;
    List<(Server.MessageType, Server.CustomMessage, int)> messageQue = new();

    public void OnMessageReceived(Server.CustomMessage message)
    {
        Server.MessageType messageID = message.type;
        int count = message.order;
        ReceiveMessage(message.type, message, count);

    }
    public void ReceiveMessage(Server.MessageType type, Server.CustomMessage message, int order)
    {
        if (order == messageReceivedOrder)
        {
            messageReceivedOrder++;
            ParseMessage(message);
        }
        else
        {
            messageQue.Add((type,message, order));
        }

        for (int i = 0; i < messageQue.Count; i++)
        {
            var v = messageQue[i];
            if (v.Item3 == messageReceivedOrder)
            {
                ReceiveMessage(v.Item1, v.Item2, v.Item3);
                messageQue.Remove(v);
                break;
            }
        }
    }
    public Coroutine ParseMessage(Server.CustomMessage message)
    {
        Server.MessageType messageID = message.type;
        Coroutine animation = null;
        switch (messageID)
        {
            case Server.MessageType._TEST:
                break;

            case Server.MessageType.EndGame:
                Match.Result endgameResult = (Match.Result)message.GetInt();
                EndGame(endgameResult);
                break;
            case Server.MessageType.StartSequence:
                disableInput = true;
                break;
            case Server.MessageType.EndSequence:
                disableInput = false;
                break;

            case Server.MessageType.ConfirmMatch:
                ulong matchID = message.GetULong();
                string matchEnemyName = message.GetString();
                Card.Class allyClass = (Card.Class)message.GetInt();
                Card.Class enemyClass = (Card.Class)message.GetInt();
                InitGame(matchID, matchEnemyName,allyClass,enemyClass);
                break;
            case Server.MessageType.DrawHand:
                List<ushort> hand = message.GetUShorts();
                int enemyCardCount = message.GetInt();
                InitHand(hand, enemyCardCount);
                break;
            case Server.MessageType.ConfirmMulligan:
                List<ushort> mulliganNewHand = message.GetUShorts();
                ConfirmMulligan(mulliganNewHand);
                break;
            case Server.MessageType.EnemyMulligan:
                List<int> enemyMulligan = message.GetInts();
                ConfirmEnemyMulligan(enemyMulligan);
                break;
            case Server.MessageType.AddCard:
                bool addFriendly = message.GetBool();
                Card.Cardname addCard = (Card.Cardname)message.GetInt();
                bool addSourceFriendly = message.GetBool();
                int addSourceIndex = message.GetInt();
                int addCostChange = message.GetInt();
                AddCard(addFriendly, addCard, addSourceFriendly, addSourceIndex, addCostChange);
                break;
            case Server.MessageType.DrawCards:
                int draw = message.GetInt();
                DrawCard((Card.Cardname)draw); 
                break;
            case Server.MessageType.DrawEnemy:
                int enemyDraws = message.GetInt();
                DrawEnemy(enemyDraws);
                break;
            case Server.MessageType.StartGame:
                bool isTurn = message.GetBool();
                StartGame(isTurn);
                break;
            case Server.MessageType.StartTurn:
                bool startAllyTurn = message.GetBool();

                int startMaxMana = message.GetInt();
                int startCurrMana = message.GetInt();
                int startMessageCount = message.GetInt();
                StartTurn(startAllyTurn,startMaxMana,startCurrMana,startMessageCount);
                break;
            case Server.MessageType.EndTurn:
                EndTurn();
                break;
            case Server.MessageType.PlayCard:
                bool playedFriendlySide = message.GetBool();
                int playedIndex = message.GetInt();
                int playedManaCost = message.GetInt();
                int playedCard = message.GetInt();
                int playedPos = message.GetInt();

                ConfirmPlayCard(playedFriendlySide, playedIndex, playedManaCost, (Card.Cardname)playedCard,playedPos);
                break;
            case Server.MessageType.SummonMinion:
                bool summonedFriendlySide = message.GetBool();
                int summonedMinion = message.GetInt();
                int summonedPos = message.GetInt();
                int summonedSource = message.GetInt();
                SummonMinion(summonedFriendlySide,(Card.Cardname)summonedMinion,summonedPos, (MinionBoard.MinionSource)summonedSource);
                break;
            case Server.MessageType.UpdateMinion:
                int updateMinionHP = message.GetInt();
                int updateMinionMax = message.GetInt();
                int updateMinionDmg = message.GetInt();
                int updateMinionInd = message.GetInt();
                bool minionUpdateFriendly = message.GetBool();
                bool UpdateMinionDamaged = message.GetBool();
                bool UpdateMinionHealed = message.GetBool();

                UpdateMinion(updateMinionHP,updateMinionMax,updateMinionDmg,updateMinionInd, minionUpdateFriendly, UpdateMinionDamaged, UpdateMinionHealed);
                break;
            case Server.MessageType.UpdateCard:
                int updateCardInd = message.GetInt();
                int updateCardCost = message.GetInt();
                bool updateCardTargeted = message.GetBool();

                UpdateCard(updateCardInd, updateCardCost, updateCardTargeted);
                break;
            case Server.MessageType.ConfirmPreAttackMinion:
                bool preMinionAllyAttack = message.GetBool();
                int preMinionAttackerIndex = message.GetInt();
                int preMinionTargetIndex = message.GetInt();
                animation = ConfirmPreAttackMinion(preMinionAllyAttack, preMinionAttackerIndex,preMinionTargetIndex);
                break;
            case Server.MessageType.ConfirmAttackMinion:
                bool minionAllyAttack = message.GetBool();
                int minionAttackerIndex = message.GetInt();
                int minionTargetIndex = message.GetInt();
                bool minionCanAttack = message.GetBool();
                bool minionFriendlyFire = message.GetBool();
                animation = ConfirmAttackMinion(minionAllyAttack, minionAttackerIndex, minionTargetIndex, minionCanAttack, minionFriendlyFire);
                break;
            case Server.MessageType.ConfirmPreAttackFace:
                bool preFaceAllyAttack = message.GetBool();
                int preFaceAttackerIndex = message.GetInt();
                animation = ConfirmPreAttackFace(preFaceAllyAttack, preFaceAttackerIndex);
                break;
            case Server.MessageType.ConfirmAttackFace:
                bool faceAllyAttack = message.GetBool();
                int faceAttackerIndex = message.GetInt();
                bool faceAttackCanAttack = message.GetBool();
                bool faceAttackFriendlyFire = message.GetBool();
                animation = ConfirmAttackFace(faceAllyAttack, faceAttackerIndex, faceAttackCanAttack, faceAttackFriendlyFire);
                break;

            case Server.MessageType.ConfirmPreSwingMinion:
                bool preSwingMinionAlly = message.GetBool();
                int preSwingMinionTargetIndex = message.GetInt();
                ConfirmPreSwingMinion(preSwingMinionAlly, preSwingMinionTargetIndex);
                break;
            case Server.MessageType.ConfirmSwingMinion:
                bool swingMinionAlly = message.GetBool();
                int swingMinionTargetIndex = message.GetInt();
                bool swingMinionCanAttack = message.GetBool();
                bool swingMinionFriendlyFire = message.GetBool();
                ConfirmSwingMinion(swingMinionAlly,swingMinionTargetIndex,swingMinionCanAttack,swingMinionFriendlyFire);
                break;

            case Server.MessageType.ConfirmPreSwingFace:
                bool preSwingFaceFriendly = message.GetBool();
                ConfirmPreSwingFace(preSwingFaceFriendly);
                break;
            case Server.MessageType.ConfirmSwingFace:
                bool swingFaceFriendly = message.GetBool();
                bool swingFaceCanAttack = message.GetBool();
                bool swingFaceFriendlyFire = message.GetBool();
                ConfirmSwingFace(swingFaceFriendly, swingFaceCanAttack, swingFaceFriendlyFire);
                break;
            case Server.MessageType.DestroyMinion:
            case Server.MessageType.RemoveMinion:
                int DestroyInd = message.GetInt();
                bool DestroyFriendly = message.GetBool();
                DestroyMinion(DestroyInd, DestroyFriendly,message.type==Server.MessageType.RemoveMinion);
                break;
            case Server.MessageType.UpdateHero:
                int UpdateHeroHP = message.GetInt();
                bool UpdateHeroFriendly = message.GetBool();
                int UpdateHeroDeckCount = message.GetInt();
                int UpdateHeroCurrMana = message.GetInt();
                int UpdateHeroMaxMana = message.GetInt();
                int UpdateHeroDamage = message.GetInt();
                int UpdateHeroArmor = message.GetInt();
                int UpdateHeroWeaponDamage = message.GetInt();
                int UpdateHeroWeaponDurability = message.GetInt();
                int UpdateHeroSpellpower = message.GetInt();
                bool UpdateHeroDamaged = message.GetBool();
                bool UpdateHeroHealed = message.GetBool();
                UpdateHero(UpdateHeroHP,UpdateHeroFriendly, UpdateHeroDeckCount,UpdateHeroCurrMana,UpdateHeroMaxMana, UpdateHeroDamage, UpdateHeroArmor,UpdateHeroWeaponDamage,UpdateHeroWeaponDurability,UpdateHeroSpellpower,UpdateHeroDamaged, UpdateHeroHealed);
                break;
            case Server.MessageType.AddAuraPlayer:
            case Server.MessageType.RemoveAuraPlayer:
                bool playerAuraChange = message.GetBool();
                Aura.Type playerAuraType = (Aura.Type)message.GetInt();
                AuraPlayerChange(playerAuraChange, playerAuraType, messageID==Server.MessageType.RemoveAuraPlayer);
                break;
            case Server.MessageType.AddAura:
            case Server.MessageType.RemoveAura:
                int addAuraMinionIndex = message.GetInt();
                bool addAuraFriendly = message.GetBool();
                ushort addAuraType = message.GetUShort();
                int addAuraValue = message.GetInt();
                bool addAuraTemp = message.GetBool();
                bool addAuraForeign = message.GetBool();
                Card.Cardname addAuraSourceCard = (Card.Cardname)message.GetInt();
                if (messageID==Server.MessageType.RemoveAura)
                {
                    RemoveAura(addAuraMinionIndex, addAuraFriendly, addAuraType, addAuraValue, addAuraTemp, addAuraForeign, addAuraSourceCard);
                    break;
                }
                AddAura(addAuraMinionIndex, addAuraFriendly, addAuraType, addAuraValue, addAuraTemp, addAuraForeign, addAuraSourceCard);
                break;
            case Server.MessageType.AddTrigger:
            case Server.MessageType.RemoveTrigger:
                int addTriggerMinionIndex = message.GetInt();
                bool addTriggerFriendly = message.GetBool();
                ushort addTriggerType = message.GetUShort();
                ushort addTriggerSide = message.GetUShort();
                ushort addTriggerAbility = message.GetUShort();
                if (messageID == Server.MessageType.RemoveTrigger)
                {
                    RemoveTrigger(addTriggerMinionIndex, addTriggerFriendly, addTriggerType, addTriggerSide, addTriggerAbility);
                    break;
                }
                AddTrigger(addTriggerMinionIndex, addTriggerFriendly, addTriggerType, addTriggerSide, addTriggerAbility);
                break;
            case Server.MessageType.DiscardCard:
                bool discardFriendly = message.GetBool();
                int discardCardInd = message.GetInt();
                int discardCardName = message.GetInt();
                int discardCardCost = message.GetInt();
                DiscardCard(discardFriendly, discardCardInd, (Card.Cardname)discardCardName, discardCardCost);
                break;
            case Server.MessageType.MillCard:
                bool millFriendly = message.GetBool();
                int millCardName = message.GetInt();
                MillCard(millFriendly, (Card.Cardname)millCardName);
                break;
            case Server.MessageType.ConfirmHeroPower:
                bool heroPowerFriendly = message.GetBool();
                ConfirmHeroPower(heroPowerFriendly);
                break;
            case Server.MessageType.EquipWeapon:
                bool weaponFriendly = message.GetBool();
                Card.Cardname weaponCard = (Card.Cardname)message.GetInt();
                EquipWeapon(weaponFriendly, weaponCard);
                break;
            case Server.MessageType.DestroyWeapon:
                bool destroyWeaponFriendly = message.GetBool();
                DestroyWeapon(destroyWeaponFriendly);
                break;
            case Server.MessageType.AddSecret:
                bool addSecretFriendly = message.GetBool();
                Card.Cardname addSecretCard = (Card.Cardname)message.GetInt();
                ConfirmAddSecret(addSecretFriendly, addSecretCard);
                break;
            case Server.MessageType.RemoveSecret:
            case Server.MessageType.TriggerSecret:
                bool removeSecretFriendly = message.GetBool();
                int removeSecretIndex = message.GetInt();
                Card.Cardname removeSecretCard = (Card.Cardname)message.GetInt();
                if (message.type == Server.MessageType.TriggerSecret)
                    ConfirmTriggerSecret(removeSecretFriendly, removeSecretIndex, removeSecretCard);
                else
                    ConfirmRemoveSecret(removeSecretFriendly, removeSecretIndex, removeSecretCard);
                break;
            case Server.MessageType.ConfirmBattlecry: 
            case Server.MessageType.ConfirmTrigger:
                bool battlecryFriendly = message.GetBool();
                int battlecryIndex = message.GetInt();
                if (messageID == Server.MessageType.ConfirmTrigger)
                {
                    bool triggerDeathrattle = message.GetBool();
                    bool triggerWeapon = message.GetBool();
                    animation = ConfirmTrigger(battlecryFriendly, battlecryIndex,triggerDeathrattle,triggerWeapon);
                }
                else
                    animation = ConfirmBattlecry(battlecryFriendly, battlecryIndex);
                break;
            case Server.MessageType.ConfirmAnimation: //TODO ANIM QUEUE
                bool animationFriendly = message.GetBool();
                string animJson = message.GetString();
                AnimationInfo animInfo = JsonUtility.FromJson<AnimationInfo>(animJson);
                ConfirmAnimation(animInfo, animationFriendly);
                break;
            default:
                Debug.LogError("UNKNOWN MESSAGE TYPE");
                break;
        }
        return animation;
    }

    public void InitHand(List<ushort> hand, int enemyCards=4)
    {
        if (hand.Contains((ushort)Card.Cardname.Coin))
            currHand.coinHand = true;
        foreach (var c in hand)
        {
            HandCard handcardPlayer = currHand.Add(((Card.Cardname)c),-1);
            currHand.AddCard(handcardPlayer, Hand.CardSource.Start);
        }
        //currHand = hand;
        string s = "";

        foreach (var c in hand)
        {
            s += ((Card.Cardname)c).ToString()+" ";
        }

        enemyHand.enemyHand = true;
        enemyHand.board = this;
        enemyHand.mulliganMode = Hand.MulliganState.Done;
        enemyHand.server = false;
        for (int i = 0; i < enemyCards; i++)
        {
            HandCard handcardEnemy = enemyHand.Add(Card.Cardname.Cardback, -1);
            enemyHand.AddCard(handcardEnemy, Hand.CardSource.Start);
        }

        //Debug.Log(playerID+" Hand: " + s);
        currHand.mulliganMode = Hand.MulliganState.None;
        mulliganButton.transform.localScale = Vector3.one;

        deck.Set(30 - currHand.Count()+(currHand.coinHand?1:0));
        enemyDeck.Set(30 - enemyCards+(currHand.coinHand?0:1));
    }


    public void EndTurn()
    {
        if (!currTurn) return;
        currTurn = false;
        currHero.combo = false;
        currHero.DisableWeapon();

        endTurnButton.SetColor(GetColor("4A5462"));

        CheckHighlights();
    }
    public void StartTurn(bool allyTurn, int maxMana, int currMana, int messageCount)
    {
        matchMessageOrder = messageCount;

        VisualInfo anim =new();
        anim.type = Server.MessageType.StartTurn;
        anim.isFriendly = allyTurn;
        anim.ints.Add(currMana);
        anim.ints.Add(maxMana);

        QueueAnimation(anim);

        CheckHighlights();
    }
    public void DrawCard(Card.Cardname card)
    {
        HandCard c = currHand.Add(card);

        //=========ANIM
        VisualInfo anim = new VisualInfo();
        anim.type = Server.MessageType.DrawCards;
        anim.handCards.Add(c);
        anim.isFriendly = true;

        QueueAnimation(anim);
    }
    public void DrawEnemy(int x)
    {
        List<HandCard> cards = new List<HandCard>();
        for (int i = 0; i < x; i++)
            cards.Add(enemyHand.Add(Card.Cardname.Cardback));


        //=========ANIM
        VisualInfo anim = new VisualInfo();
        anim.type = Server.MessageType.DrawEnemy;
        anim.handCards.AddRange(cards);

        QueueAnimation(anim);
    }

    public void DestroyMinion(int ind, bool friendlySide, bool removal=false)
    {
        Minion m = null;

        if (friendlySide)
        {
            m = currMinions.RemoveAt(ind);
        }
        else
        {
            m = enemyMinions.RemoveAt(ind);
        }
        m.DEAD = true;
        CheckHighlights();

        //===========ANIM
        VisualInfo anim = new VisualInfo();
        anim.type = removal? Server.MessageType.RemoveMinion:Server.MessageType.DestroyMinion;
        anim.minions.Add(m);
        anim.isFriendly = friendlySide;

        QueueAnimation(anim);

    }

    public void UpdateCard(int index, int updatedManaCost, bool updateTargeted)
    {
        currHand[index].manaCost = updatedManaCost;
        currHand[index].TARGETED = updateTargeted;

        //===================
        VisualInfo anim = new VisualInfo();
        anim.type = Server.MessageType.UpdateCard;
        anim.handCards.Add(currHand[index]);

        QueueAnimation(anim);

    }

    public void UpdateMinion(int updatedHealth,int updatedMax, int updatedDamage,int index,bool friendly, bool damaged, bool healed)
    {
        Minion minion = friendly ? currMinions[index] : enemyMinions[index];

        int diff = updatedHealth - minion.health;
        minion.health = updatedHealth;
        minion.maxHealth = updatedMax;
        minion.damage = updatedDamage;

        VisualInfo anim = new VisualInfo();
        anim.ints.Add(minion.health);
        anim.ints.Add(minion.damage);
        anim.ints.Add(minion.maxHealth);
        anim.type = Server.MessageType.UpdateMinion;
        anim.minions.Add(minion);
        if (damaged || healed)
        {
            anim.trigger = true;
            anim.damage=diff;
        }
        QueueAnimation(anim);
        //todo: splash damage/heal
        
    }
    public void UpdateHero(int hp, bool friendly, int deckCount, int currMana,int maxMana, int damage, int armor,int weaponDamage, int weaponDurability, int spellpower, bool damaged, bool healed)
    {
        int diff = 0;

        if (friendly)
        {
            diff = hp - currHero.health;

            currHero.SetHealth(hp);
            deck.Set(deckCount);
            mana.SetCurrent(currMana);
            mana.SetMax(maxMana);
            currHero.damage = damage;
            currHero.armor = armor;
            if (spellpower!=currHero.spellpower)
            {
                currHero.spellpower = spellpower;
                foreach(Card c in currHand.cardObjects.Values)
                {
                    c.UpdateCardText();
                }
            }

            if (currHero.weapon!=null)
            {
                currHero.weapon.damage = weaponDamage;
                currHero.weapon.durability = weaponDurability;
            }
        }
        else
        {
            diff = hp - enemyHero.health;

            enemyHero.SetHealth(hp);
            enemyDeck.Set(deckCount);
            enemyMana.SetCurrent(currMana);
            enemyMana.SetMax(maxMana);
            enemyHero.damage = damage;
            enemyHero.armor = armor;
            enemyHero.spellpower = spellpower;

            if (enemyHero.weapon != null)
            {
                enemyHero.weapon.damage = weaponDamage;
                enemyHero.weapon.durability = weaponDurability;
            }
        }

        VisualInfo anim = new VisualInfo();
        anim.type = Server.MessageType.UpdateHero;
        anim.isFriendly = friendly;
        anim.ints.Add(hp);
        anim.ints.Add(deckCount);
        anim.ints.Add(currMana);
        anim.ints.Add(maxMana);
        anim.ints.Add(damage);
        anim.ints.Add(armor);
        anim.ints.Add(weaponDamage);
        anim.ints.Add(weaponDurability);
        if (damaged || healed)
        {
            anim.trigger = true;
            anim.damage=diff;
        }
        QueueAnimation(anim);
        CheckHighlights();
    }
    public void AddAura(int minionIndex,bool friendly, ushort auraType, int value, bool temp, bool foreign,Card.Cardname source)
    {
        Minion target = friendly? currMinions[minionIndex] : enemyMinions[minionIndex];
        
        target.AddAura(new Aura((Aura.Type)auraType, value, temp, foreign,null, source));


        VisualInfo anim = new VisualInfo();
        anim.type = Server.MessageType.AddAura;
        anim.minions.Add(target);
        QueueAnimation(anim);
        
    }
    public void RemoveAura(int minionIndex,bool friendly, ushort auraType, int value, bool temp, bool foreign,Card.Cardname source)
    {
        Minion target = friendly? currMinions[minionIndex] : enemyMinions[minionIndex];
        
        target.RemoveMatchingAura(new Aura((Aura.Type)auraType, value, temp, foreign, null, source));

        VisualInfo anim = new VisualInfo();
        anim.type = Server.MessageType.RemoveAura;
        anim.minions.Add(target);
        QueueAnimation(anim);
    }
    public void RemoveTrigger(int minionIndex, bool friendly, ushort type, ushort side, ushort ability)
    {
        Minion target = friendly ? currMinions[minionIndex] : enemyMinions[minionIndex];
        target.RemoveMatchingTrigger(new Trigger((Trigger.Type)type, (Trigger.Side)side, (Trigger.Ability)ability,target));

        VisualInfo anim = new VisualInfo();
        anim.type = Server.MessageType.RemoveTrigger;
        anim.minions.Add(target);
        QueueAnimation(anim);
    }
    public void AddTrigger(int minionIndex, bool friendly, ushort type, ushort side, ushort ability)
    {
        Minion target = friendly ? currMinions[minionIndex] : enemyMinions[minionIndex];
        target.AddTrigger((Trigger.Type)type, (Trigger.Side)side, (Trigger.Ability)ability);

        VisualInfo anim = new VisualInfo();
        anim.type = Server.MessageType.AddTrigger;
        anim.minions.Add(target);
        QueueAnimation(anim);
    }

    public void DiscardCard(bool friendly, int ind, Card.Cardname card, int manaCost)
    {
        Hand hand = friendly ? currHand : enemyHand;
        HandCard c = hand[ind];
        hand.RemoveAt(ind);

        VisualInfo anim = new VisualInfo();
        anim.type = Server.MessageType.DiscardCard;
        anim.handCards.Add(c);
        anim.names.Add(card);
        anim.ints.Add(manaCost);
        anim.isFriendly = friendly;

        QueueAnimation(anim);

    }
    public void MillCard(bool friendly, Card.Cardname card)
    {
        VisualInfo anim = new();
        anim.type = Server.MessageType.MillCard;
        anim.isFriendly = friendly;
        anim.names.Add(card);
        QueueAnimation(anim);
    }


    public bool IsFriendly(Minion m)
    {
        if (currMinions.Contains(m)) return true;
        return false;
    }    

    public bool IsFriendly(Hero h)
    {
        return h == currHero;
    }
    void Update()
    {
#if (UNITY_EDITOR)
        if (Input.GetKeyDown(KeyCode.Q) && playerID!=101)
        {
            StartMatchmaking(new SaveManager.Decklist("zoo",Card.Class.Warlock,Database.Zoo_Lock));
        }
        if (Input.GetKeyDown(KeyCode.A) && playerID!=101)
        {
            SubmitMulligan();
        }
        if (Input.GetKeyDown(KeyCode.Z) && playerID!=101)
        {
            SubmitEndTurn();
        }
        //===============================
        if (Input.GetKeyDown(KeyCode.W) && playerID==101)
        {
            StartMatchmaking(new SaveManager.Decklist("patron", Card.Class.Warrior, Database.Patron_Warrior));
        }
        if (Input.GetKeyDown(KeyCode.S) && playerID==101)
        {
            SubmitMulligan();
        }
        if (Input.GetKeyDown(KeyCode.X) && playerID==101)
        {
            SubmitEndTurn();
        }
        //===============================
        if (Input.GetKey(KeyCode.BackQuote))
        {
            RestartScene();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Camera.main.transform.localPosition = new Vector3(0,0,-10);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Camera.main.transform.localPosition = new Vector3(50,0,-10);
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            currHand.OrderInds();
        }
#endif //END TESTING HOTKEYS
    }
    void Tester(Server.CustomMessage m)
    {
    }

    //todo: make UImanager monobehavior/class
    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public GameObject menu;
    public void ToggleMenu()
    {
        menu.transform.localScale = menu.transform.localScale == Vector3.zero ? Vector3.one : Vector3.zero;
        if (menu.transform.localScale == Vector3.one)
        {
            StartCoroutine(menuCancel());
        }
    }
    IEnumerator menuCancel()
    {
        menu.transform.localPosition += new Vector3(0, 0.1f);
        yield return null;
        menu.transform.localPosition += new Vector3(0, 0.1f);
        yield return null;
        menu.transform.localPosition += new Vector3(0, -0.1f);
        yield return null;
        menu.transform.localPosition += new Vector3(0, -0.1f);
        yield return null;

        while (true)
        {
            if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1) || Input.GetMouseButtonUp(2) || Input.GetMouseButtonDown(1))
            {
                menu.transform.localScale = Vector3.zero;
                break;
            }
            yield return null;
        }

    }
}
