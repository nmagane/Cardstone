using System.Collections;
using TMPro;
using UnityEngine;

public class Card : MonoBehaviour
{
    public Board board;
    public HandCard card;

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
            name.color = new Color(name.color.r, name.color.b, name.color.g, _alpha);
            text.color = new Color(text.color.r, text.color.b, text.color.g, _alpha);
            manaCost.color = new Color(manaCost.color.r, manaCost.color.b, manaCost.color.g, _alpha);
            health.color = new Color(health.color.r, health.color.b, health.color.g, _alpha);
            damage.color = new Color(damage.color.r, damage.color.b, damage.color.g, _alpha);
            icon.color = new Color(icon.color.r, icon.color.b, icon.color.g, _alpha);
            back.color = new Color(back.color.r, back.color.b, back.color.g, _alpha);

        }
    }

    public new TMP_Text name;
    public TMP_Text text;
    public TMP_Text manaCost;
    public TMP_Text health;
    public TMP_Text damage;
    public SpriteRenderer icon;
    public SpriteRenderer back;
    public SpriteRenderer mulliganMark;
    public Sprite cardback;
    public Sprite spritePlaceholder;
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
    }
    void Start()
    {
        
    }

    public void Set(HandCard c)
    {
        card = c;
        if (c.card == Cardname.Cardback)
        {
            icon.sprite = cardback;
            name.text = "";
            text.text = "";
            manaCost.text = "";
            damage.text = "";
            health.text = "";
            return;
        }
        icon.sprite = spritePlaceholder;
        gameObject.name = c.card.ToString();
        Database.CardInfo cardInfo = Database.GetCardData(c.card);
        name.text = cardInfo.name;
        text.text = cardInfo.text;
        manaCost.text = c.manaCost.ToString(); ;
        if (c.MINION)
        {
            damage.text = c.damage.ToString();
            health.text = c.health.ToString();
        }
        if (c.SPELL)
        {
            damage.text = "";
            health.text = "";
        }
    }
    public void SetFlipped()
    {
        back.enabled = true;
        icon.transform.localEulerAngles = new Vector3(0, 90, 0);
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
    bool hidden = false;
    public void HideCard()
    {
        hidden = true;
        EndDrag();
        transform.localScale = Vector3.zero;
    }

    public void EndPlay()
    {
        if (hidden)
        {
            transform.localScale = Vector3.one;
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
        

        if ((card.SPELL || card.SECRET || card.WEAPON) && card.TARGETED == false)
        {
            //UNTARGETED NON-MINION
            board.PlayCard(card);
        }

        if ((card.SPELL || card.SECRET || card.WEAPON) && card.TARGETED == true)
        {
            //TARGETED NON-MINION
        }


        if (card.MINION && card.TARGETED == false)
        {
            //SIMPLE MINION SUMMON
            if (board.currMinions.Count() >= 7)
            {
                EndPlay();
                return;
            }

            int position = FindMinionPosition();
            board.PlayCard(card, -1, position);
            EndDrag();
            return;
        }

        if (card.MINION && card.TARGETED == true)
        {
            //MINION WITH TARGET ABILITY
            //place temporary minion and start targetining effect
            if (board.currMinions.Count() >= 7)
            {
                EndPlay();
                return;
            }

            int position = FindMinionPosition();
            //TODO: VALID TARGET EXISTS CHECK
            bool validTargetsExist = board.ValidTargetsAvailable(card.eligibleTargets);
            if (validTargetsExist)
            {
                //EndPreview();
                HideCard();
                board.StartMinionPreview(this, position);
            }
            else
                board.PlayCard(card, -1, position);
        }


    }

    int FindMinionPosition()
    {
        float mPos = this.transform.position.x;
        int ind = 0;
        
        foreach (var kvp in board.currMinions.minionObjects)
        {
            float x = kvp.Value.transform.position.x;
            if (mPos > x) ind++;
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

        if ((card.SPELL || card.SECRET || card.WEAPON) && card.TARGETED == true)
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
            HideCard();
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
    Vector3 OP = new Vector3();
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
        
        OP = transform.localPosition;
        offset = transform.position - GetMousePos();
        clickPos = GetMousePos();
        board.StartPlayingCard(this);

        transform.position = DragPos();
        StartDrag();
    }

    int dragCounter = 0;
    int dragTime = 5;
    private void OnMouseDrag()
    {
        if (card.card == Cardname.Cardback) return;
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
    Coroutine dragCoroutine = null;
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
        icon.transform.localScale = Vector3.one * 1.25f;
        //icon.transform.localPosition += new Vector3(0, -2);
        icon.transform.localEulerAngles = -handRot;
        yield return null;
        while (true)
        {

            transform.position = DragPos();

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

    public void SetSortingOrder(int x)
    {
        x = x * 10;
        icon.sortingOrder = x;
        back.sortingOrder = x;
        name.GetComponent<MeshRenderer>().sortingOrder = x + 1;
        text.GetComponent<MeshRenderer>().sortingOrder = x + 1;
        manaCost.GetComponent<MeshRenderer>().sortingOrder = x + 1;
        health.GetComponent<MeshRenderer>().sortingOrder = x + 1;
        damage.GetComponent<MeshRenderer>().sortingOrder = x + 1;
    }
    bool hov = false;
    public void ShowHover()
    {
        if (dragCoroutine != null) return;
        if (board.playingCard != null) return;
        if (noReturn) return;
        if (board.currHand.mulliganMode != Hand.MulliganState.Done) return;
        hov = true;
        icon.transform.localScale = Vector3.one * 1.5f;
        icon.transform.localEulerAngles = -handRot;
        board.animationManager.LerpTo(icon.gameObject, new Vector3(0,2), 5, 0);
    }
    public void HideHover()
    {
        if (dragCoroutine != null) return;
        if (board.playingCard != null) return;
        if (noReturn) return;
        if (board.currHand.mulliganMode != Hand.MulliganState.Done) return;
        if (!hov) return;
        hov = false;
        icon.transform.localScale = Vector3.one;
        icon.transform.localEulerAngles = Vector3.zero;
        board.animationManager.LerpTo(icon.gameObject, Vector3.zero, 5, 0);
    }
    public void ReturnToHand()
    {
        if (noReturn) return;
        if (board.currHand.cardObjects.ContainsKey(card) == false) return;
        board.animationManager.EndMovement(icon.gameObject);
        board.currHand.MoveCard(this, handPos, handRot);
        icon.transform.localScale = Vector3.one;
        icon.transform.localEulerAngles = Vector3.zero;
        icon.transform.localPosition = Vector3.zero;
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
                icon.transform.localEulerAngles = Vector3.Lerp(OP, Vector3.zero, (i + 1) / frames);
                yield return AnimationManager.Wait(1);
            }
        }
        else
        {
            for (float i = 0; i < frames; i++)
            {
                icon.transform.localEulerAngles = Vector3.Lerp(Vector3.zero, OP, (i + 1) / frames);
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
