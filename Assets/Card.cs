using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class Card : MonoBehaviour
{
    public Board board;
    public HandCard card;
    public DropShadow shadow;

    float _alpha = 1;
    public float alpha
    {
        get
        {
            return _alpha;
        }
        set
        {
            _alpha = value;
            name.color = new Color(name.color.r, name.color.g, name.color.b, _alpha);
            text.color = new Color(text.color.r, text.color.g, text.color.b, _alpha);
            highlight.color = new Color(highlight.color.r, highlight.color.g, highlight.color.b, _alpha);
            manaCost.color = new Color(manaCost.color.r, manaCost.color.g, manaCost.color.b, _alpha);
            health.color = new Color(health.color.r, health.color.g, health.color.b, _alpha);
            damage.color = new Color(damage.color.r, damage.color.g, damage.color.b, _alpha);
            frame.color = new Color(frame.color.r, frame.color.g, frame.color.b, _alpha);
            icon.color = new Color(frame.color.r, frame.color.g, frame.color.b, _alpha);
            back.color = new Color(back.color.r, back.color.g, back.color.b, _alpha);

        }
    }
    public Sprite[] cardSprites;

    public new TMP_Text name;
    public TMP_Text text;
    public TMP_Text manaCost;
    public TMP_Text health;
    public TMP_Text damage;
    public SpriteRenderer frame;
    public SpriteRenderer icon;
    public SpriteRenderer back;
    public SpriteRenderer mulliganMark;
    public SpriteRenderer highlight;
    public Sprite cardback;
    public Sprite highlightMinion;
    public Sprite highlightMinionSpecial;
    public Sprite highlightSpell;
    public Sprite highlightSpellSpecial;
    public bool init = false;
    public bool starter = false;
    public bool noReturn = false;
    public enum Cardname
    {
        //NONCARD (ENEMY HAND DISPLAY)
        Cardback,
        
        //NEUTRAL
        Coin,

        Abusive_Sergeant,
        Amani_Berserker,
        Argent_Squire,
        Dark_Iron_Dwarf,
        Dire_Wolf_Alpha,
        Defender_of_Argus,
        Harvest_Golem,
        Damaged_Golem,
        Ironbeak_Owl,
        Knife_Juggler,
        Shattered_Sun_Cleric,
        Shieldbearer,
        Young_Priestess,

        //WARLOCK
        Lifetap,

        Soulfire,
        Voidwalker,
        Flame_Imp,
        Doomguard,

        //Mage
        Ping,
        Arcane_Explosion,

        //UNIMPLEMENTED
        Voodoo_Doctor,

        Emperor_Thaurissan,
        Preparation,
        Loatheb,
        Mana_Wraith,
        Sorcerers_Apprentice,
        Stormwind_Champion, 
        Millhouse_Manastorm,
        Hunters_Mark,
        Crazed_Alchemist,
        Chillwind_Yeti,
        Dagger,
        Heroic_Strike,
        Deadly_Poison,
        Blade_Flurry,
        Armor_Up,
        SI7_Agent,
        Eviscerate,
        Archmage_Antonidas,
        Sap,
        Ogre_Magi,
        Ice_Barrier,

        Mage_Secret,
        Hunter_Secret,
        Paladin_Secret,

        Frostbolt,
        Southsea_Deckhand,
        Warsong_Commander,
        Grim_Patron,
        Mountain_Giant,
        Sea_Giant,
        Molten_Giant,

        Dagger_Mastery,
        Backstab,
        Bloodmage_Thalnos,
        Fan_of_Knives,
        Earthen_Ring_Farseer,
        Tinkers_Oil,
        Violet_Apprentice,
        Violet_Teacher
    }

    public enum Class
    {
        Neutral,

        Warlock,
        Mage,
        Rogue,
        Warrior,
        Druid,
        Priest,
        Shaman,
        Paladin,
        Hunter,
    }
    public enum Tribe
    {
        None,
        Demon,
        Dragon,
        Mech,
    }
    public Sprite[] minionCards;
    public Sprite[] spellCards;
    public Sprite[] weaponCards;

    void Awake()
    {
        shadow = frame.GetComponent<DropShadow>();
    }

    int spellDamage = 0;
    int comboDamage = 0;
    public void Set(HandCard c)
    {
        card = c;
        if (c.card == Cardname.Cardback)
        {
            frame.sprite = cardback;
            icon.sprite = null;
            name.text = "";
            text.text = "";
            manaCost.text = "";
            damage.text = "";
            health.text = "";
            SetElevated(false);
            return;
        }
        gameObject.name = c.card.ToString();
        icon.sprite = cardSprites[(int)c.card];
        Database.CardInfo cardInfo = Database.GetCardData(c.card);
        name.text = cardInfo.name;
        originalText = cardInfo.text;
        text.text = cardInfo.text;
        _manaCost = c.manaCost;
        spellDamage = cardInfo.spellDamage;
        comboDamage = cardInfo.comboSpellDamage;
        manaCost.text = c.manaCost.ToString();
        if (c.MINION || c.WEAPON)
        {
            damage.text = c.damage.ToString();
            health.text = c.health.ToString();
            if (c.WEAPON)
                frame.sprite = weaponCards[(int)cardInfo.classType];
            else frame.sprite = minionCards[(int)cardInfo.classType];
            highlight.sprite = highlightMinion;
            baseHighlight = highlightMinion;
            comboHighlight = highlightMinionSpecial;
        }
        if (c.SPELL || c.SECRET)
        {
            damage.text = "";
            health.text = "";
            frame.sprite = spellCards[(int)cardInfo.classType]; ;
            highlight.sprite = highlightSpell;
            baseHighlight = highlightSpell;
            comboHighlight = highlightSpellSpecial;
        }
        UpdateCardText();
    }
    int _manaCost = 0;

    string originalText="";
    public void UpdateCardText()
    {
        if (card.SPELL == false) return; //no need to update non-spells
        if (board == null) return;

        int sp = (transform.parent == board.enemyPlayTip.transform) ? board.enemyHero.spellpower : board.spellpower;
        string s = originalText;
        s = originalText.Replace("{0}", (sp>0?"(":"")+(spellDamage+sp).ToString() + (sp > 0 ? ")" : ""));
        s = s.Replace("{1}", (sp>0?"(":"")+(comboDamage+sp).ToString() + (sp > 0 ? ")" : ""));
        text.text = s;
    }

    public void UpdateManaCost(bool noAnim =false)
    {
        if (card.manaCost!=_manaCost && !noAnim)
        {
            StartCoroutine(Creature.txtBounce(manaCost));
        }
        _manaCost = card.manaCost;


        if (_manaCost < card.baseCost)
        {
            manaCost.color = Board.Instance.minionObject.GetComponent<Creature>().greenText;
        }
        else if (_manaCost > card.baseCost)
        {
            manaCost.color = Board.Instance.minionObject.GetComponent<Creature>().redText;
        }
        else
        {
            manaCost.color = Board.Instance.minionObject.GetComponent<Creature>().baseText;
        }
        alpha = alpha;
        manaCost.text = card.manaCost.ToString();
    }

    Sprite comboHighlight;
    Sprite baseHighlight;
    public void Highlight(bool specialEffect=false)
    {
        CheckAuras();
        if (special) specialEffect = true;
        if ((card.COMBO && board.combo)||specialEffect) highlight.sprite = comboHighlight;
        else highlight.sprite = baseHighlight;
        highlight.enabled = true;
    }
    public void Unhighlight()
    {
        highlight.enabled = false;
    }
    public void SetFlipped()
    {
        back.enabled = true;
        frame.transform.localEulerAngles = new Vector3(0, 90, 0);
    }
    public void Flip()
    {
        StartCoroutine(flipper());
    }
    void ToggleMulligan()
    {
        if (board.selectedMulligans.Contains(card.index))
        {

            mulliganMark.enabled = false;
            board.selectedMulligans.Remove(card.index);
        }
        else
        {
            mulliganMark.enabled = true;
            board.selectedMulligans.Add(card.index);
        }
    }
    public bool hidden = false;
    public void HideCard(Vector3 pos)
    {
        hidden = true;
        EndDrag();
        board.animationManager.PlayFade(this, pos, true);
    }
    public void ShowCard()
    {
        hidden = false;
        ReturnToHand();
        board.animationManager.Unfade(this);
    }

    public void EndPlay()
    {
        if (hidden)
        {
            ShowCard(); 
        }
        if (noReturn == false) ReturnToHand();
        EndDrag();
        board.EndPlayingCard();
        if (preview) EndPreview();
    }

    public void PlayCard()
    {
        if (transform.localPosition.y <= -6.5f)
        {
            EndPlay();
            return;
        }
        if (card.manaCost > board.currMana)
        {
            //ERROR: NOT ENOUGH MANA
            EndPlay();
            return;
        }
        if (card.MINION && board.currMinions.Count() >= 7)
        {
            EndPlay();
            return;
        }

        if (card.played)
        {
            EndPlay();
            return;
        }
        ////START CAST
        
        GetComponent<BoxCollider2D>().enabled = false;
        float f = frame.transform.localPosition.y;
        frame.transform.localPosition = Vector3.zero;
        transform.localPosition += new Vector3(0, f);



        if ((card.SPELL || card.SECRET || card.WEAPON) && (card.TARGETED || (card.COMBO && board.combo && card.COMBO_TARGETED)))
        {
            //TARGETED NON-MINION
            //THIS IS PLAYED THROUGH THE BOARD TARGETING SYSTEM SO THERE'S NOTHING HERE.
            return;
        }

        if ((card.SPELL || card.SECRET || card.WEAPON) && card.TARGETED == false)
        {
            //UNTARGETED NON-MINION
            board.PlayCard(card);
            return;
        }

        if (card.MINION && (card.TARGETED || (card.COMBO && board.combo && card.COMBO_TARGETED)))
        {
            //MINION WITH TARGET ABILITY or with COMBO TARGET (SI 7 agent)
            //place temporary minion and start targetining effect

            int position = FindMinionPosition();
            //TODO: VALID TARGET EXISTS CHECK
            bool validTargetsExist = board.ValidTargetsAvailable(card.eligibleTargets);
            if (validTargetsExist)
            {
                //EndPreview();
                Vector3 p = board.StartMinionPreview(this, position);
                HideCard(p);
            }
            else
            {
                board.PlayCard(card, -1, position);
            }
            return;
        }

        if (card.MINION && card.TARGETED == false)
        {
            //SIMPLE MINION SUMMON
            int position = FindMinionPosition();
            board.PlayCard(card, -1, position);
            EndDrag();
            return;
        }
    }

    int FindMinionPosition()
    {
        float mPos = this.transform.position.x;
        int ind = 0;
        
        foreach (var kvp in board.currMinions.minionObjects)
        {
            if (kvp.Key.DEAD) continue;
            float x = kvp.Value.transform.position.x;
            if (mPos > x) ind++;
        }
        if (ind >= board.currMinions.minionObjects.Count)
        {
            ind = 10;
        }
        //Debug.Log(ind);
        return ind;
    }
    bool preview = false;

    public void PreviewPlay()
    {
        if (board.currMana < card.manaCost)
        {
            EndPlay();
            //TODO: Not enough mana popup
            return;
        }
        if (board.currTurn == false)
        {
            EndPlay();
            //TODO: Not your turn popup
            return;
        }
        if (card.SECRET && board.currHero.HasSecret(card.card))
        {
            //EndPlay();
            return;
        }
        if (card.eligibleTargets==Board.EligibleTargets.Weapon && board.currHero.weapon==null)
        {
            EndPlay();
            return;
        }

        if ((card.SPELL || card.SECRET || card.WEAPON) && (card.TARGETED || (card.COMBO && board.combo && card.COMBO_TARGETED)))
        {
            if (preview) return;
            if (board.ValidTargetsAvailable(card.eligibleTargets) == false)
            {
                EndPlay();
                return;
            }
            //PlayCard();
            board.StartTargetingCard(card);
            //EndPlay();
            HideCard(this.transform.position);
        }

        if ((card.MINION) == true)
        {
            if (board.currMinions.Count() >= 7)
            {
                EndPlay();
                //TODO: "too many minions" error
                return;
            }
            board.currMinions.PreviewGap(FindMinionPosition());
        }

        preview = true;
    }

    public void EndPreview()
    {
        if (preview == false) return;
        preview = false;
        if ((card.SPELL || card.SECRET || card.WEAPON) && card.TARGETED == true)
        {
            //preview spell/target
        }
        if ((card.MINION) == true)
        {
            board.currMinions.EndPreview();
        }
    }
    
    public static Vector3 GetMousePos()
    {
        return (Camera.main.ScreenToWorldPoint(Input.mousePosition) - new Vector3(0, 0, Camera.main.ScreenToWorldPoint(Input.mousePosition).z));
    }


    Vector3 offset;
    Vector3 clickPos = new Vector3();
    private void OnMouseDown()
    {
        if (card.card == Cardname.Cardback) return;
        
        if (board.currHand.mulliganMode==Hand.MulliganState.None)
        {
            ToggleMulligan();
            return;
        }
        if (dragCoroutine != null|| board.playingCard == this) return;
        if (board.currHand.mulliganMode != Hand.MulliganState.Done) return;
        if (card.played) return;
        if (board.disableInput) return;
        
        offset = transform.position - GetMousePos();
        clickPos = GetMousePos();
        board.StartPlayingCard(this);

        transform.position = DragPos();
        StartDrag();
    }

    int dragCounter = 0;
    const int dragTime = 5;
    private void OnMouseDrag()
    {
        if (card.card == Cardname.Cardback) return;
        if (board.disableInput) return;
        if (board.currHand.mulliganMode!=Hand.MulliganState.Done)
        {
            return;
        }

        if (board.playingCard == this)
        {
            if (dragCounter < dragTime) dragCounter++;
            if (dragCounter >= dragTime)
            {
                if (Vector3.Distance(Card.GetMousePos(), clickPos) > 0.1f)
                {
                    board.dragTargeting = true;
                    //Debug.Log("drag");
                }
            }
        }
    }

    private void OnMouseUp()
    {

        if (card.card == Cardname.Cardback) return;
        if (board.currHand.mulliganMode!=Hand.MulliganState.Done)
        {
            return;
        }
        if (board.currTurn == false)
        {
            //ERROR: NOT YOUR TURN
            if (noReturn==false) ReturnToHand();
            return;
        }

        //if (noReturn == false) ReturnToHand();
        return;
    }
    private void OnMouseOver()
    {
        if (card.card == Cardname.Cardback) return;
        ShowHover();
    }
    private void OnMouseExit()
    {
        if (card.card == Cardname.Cardback) return;
        HideHover();
    }
    public Vector3 DragPos()
    {
        return GetMousePos() + offset;
    }
    public Coroutine dragCoroutine = null;
    void StartDrag()
    {
        //HideHover();
        if (dragCoroutine==null) StartCoroutine(dragger());
    }
    void EndDrag()
    {
        //if (noReturn == false) ReturnToHand();
        StopAllCoroutines();
    }

    public IEnumerator dragger()
    {
        frame.transform.localScale = Vector3.one * 1.25f;
        //icon.transform.localPosition += new Vector3(0, -2);
        frame.transform.localEulerAngles = Vector3.zero;
        transform.localEulerAngles = Vector3.zero;
        GetComponent<BoxCollider2D>().enabled = false;
        SetElevated(true);
        shadow.elevation = 1;
        yield return null;
        Vector3 last = DragPos();
        float angle = 0;
        while (true)
        {
            if (card.played) break;

            Vector3 pos = DragPos();
            float x = pos.x;
            float l = last.x;
            float diff = Mathf.Clamp(x - l, -2, 2);
            float target = diff / 2 * 30f;
            angle = Mathf.Lerp(angle, target, 0.25f);
            transform.localEulerAngles = new Vector3(0, 0, angle);
            transform.position = pos;
            last = pos;

            if (transform.localPosition.y >= -6.5f)
            {
                PreviewPlay();
            }
            if (transform.localPosition.y < -6.5f && preview==true)
            {
                EndPreview();
            }


            if (board.dragTargeting)
            {
                if (Input.GetMouseButtonUp(0))
                {
                    PlayCard();
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    PlayCard();
                }
            }
            if (Input.GetMouseButtonDown(1))
            {
                EndPlay();
            }

            yield return null;
        }
    }

    public Vector3 handPos=Vector3.zero;
    public Vector3 handRot=Vector3.zero;

    bool special = false;
    public void CheckAuras()
    {
        special = false;

        foreach (Aura a in card.auras)
        {
            switch (a.type)
            {
                case Aura.Type.HoldingDragonTargeted:
                case Aura.Type.HoldingDragon:
                    foreach (HandCard c in board.currHand)
                    {
                        if (c == card) continue;
                        if (c.tribe == Tribe.Dragon)
                        {
                            special = true;
                        }
                    }
                    break;
            }
        }
    }

    public void SetSortingOrder(int x)
    {
        x = x * 10;
        frame.sortingOrder = x;
        icon.sortingOrder = x+1;
        back.sortingOrder = x;
        highlight.sortingOrder = x;
        name.GetComponent<MeshRenderer>().sortingOrder = x + 1;
        text.GetComponent<MeshRenderer>().sortingOrder = x + 1;
        manaCost.GetComponent<MeshRenderer>().sortingOrder = x + 1;
        health.GetComponent<MeshRenderer>().sortingOrder = x + 1;
        damage.GetComponent<MeshRenderer>().sortingOrder = x + 1;
    }
    public void SetElevated(bool elevated)
    {
        string x = elevated ? "cardElevated" : "card";
        string s = elevated ? "shadowCardElevated" : "shadowCard";
        if (card.card == Cardname.Cardback && !elevated) s = "shadow";
        frame.sortingLayerName = x;
        icon.sortingLayerName = x;
        back.sortingLayerName = x;
        highlight.sortingLayerName = x;
        name.GetComponent<MeshRenderer>().sortingLayerName = x;
        text.GetComponent<MeshRenderer>().sortingLayerName = x;
        manaCost.GetComponent<MeshRenderer>().sortingLayerName = x;
        health.GetComponent<MeshRenderer>().sortingLayerName = x;
        damage.GetComponent<MeshRenderer>().sortingLayerName = x;

        shadow.sortingLayer = s;
    }
    bool hov = false;
    public bool noHover = false;
    public void ShowHover()
    {
        if (dragCoroutine != null) return;
        if (board.playingCard != null) return;
        if (noReturn) return;
        if (transform.localPosition.y > -6) return;
        if (noHover) return;
        if (board.currHand.mulliganMode != Hand.MulliganState.Done) return;
        hov = true;
        frame.transform.localScale = Vector3.one * 1.55f;
        frame.transform.localEulerAngles = -handRot;
        board.animationManager.LerpTo(frame.gameObject, new Vector3(0,(-10-transform.localPosition.y)+2.8f), 5, 0);

        SetElevated(true);
        shadow.elevation = 1f;
    }
    public void HideHover()
    {
        if (dragCoroutine != null) return;
        if (board.playingCard != null) return;
        if (noReturn) return;
        if (board.currHand.mulliganMode != Hand.MulliganState.Done) return;
        if (!hov) return;
        hov = false;
        frame.transform.localScale = Vector3.one;
        frame.transform.localEulerAngles = Vector3.zero;
        board.animationManager.LerpTo(frame.gameObject, Vector3.zero, 5, 0);
        shadow.elevation = 0.1f;

        SetElevated(false);
    }
    public void ReturnToHand()
    {
        if (noReturn) return;
        if (board.currHand.cardObjects.ContainsKey(card) == false) return;
        GetComponent<BoxCollider2D>().enabled = true;
        board.animationManager.EndMovement(frame.gameObject);
        board.currHand.MoveCard(this, handPos, handRot);
        frame.transform.localScale = Vector3.one;
        frame.transform.localEulerAngles = Vector3.zero;
        frame.transform.localPosition = Vector3.zero;
        SetElevated(false);
    }
    public void ElevateTo(float v,float f)
    {
        StartCoroutine(elevator(v, f));
    }
    IEnumerator elevator(float v,float frames)
    {
        float op = shadow.elevation;
        for (int i=0;i<frames;i++)
        {
            shadow.elevation = Mathf.Lerp(op, v, (i + 1) / frames);
            yield return null;
        }
    }
    IEnumerator flipper()
    {
        float frames = 5;
        Vector3 OP = new Vector3(0, 90, 0);
        if (back.enabled)
        {
            for (float i = 0; i < frames; i++)
            {
                back.transform.localEulerAngles = Vector3.Lerp(Vector3.zero, OP, (i + 1) / frames);
                yield return AnimationManager.Wait(1);
            }

            back.enabled = false;

            for (float i = 0; i < frames; i++)
            {
                frame.transform.localEulerAngles = Vector3.Lerp(OP, Vector3.zero, (i + 1) / frames);
                yield return AnimationManager.Wait(1);
            }
        }
        else
        {
            for (float i = 0; i < frames; i++)
            {
                frame.transform.localEulerAngles = Vector3.Lerp(Vector3.zero, OP, (i + 1) / frames);
                yield return AnimationManager.Wait(1);
            }

            back.enabled = true;

            for (float i = 0; i < frames; i++)
            {
                back.transform.localEulerAngles = Vector3.Lerp(OP, Vector3.zero, (i + 1) / frames);
                yield return AnimationManager.Wait(1);
            }
        }
    }
}
