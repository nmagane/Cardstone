using System.Collections;
using TMPro;
using UnityEngine;

public class Card : MonoBehaviour
{
    public Board board;
    public HandCard card;

    public new TMP_Text name;
    public TMP_Text text;
    public TMP_Text manaCost;
    public TMP_Text health;
    public TMP_Text damage;
    public SpriteRenderer icon;
    public SpriteRenderer mulliganMark;
    public Sprite cardback;
    public enum Cardname
    {
        //NONCARD (ENEMY HAND DISPLAY)
        Cardback,
        
        //NEUTRAL
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
        transform.localPosition = OP;
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
            //EndPlay();
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
            bool validTargetsExist = true;
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

        OP = transform.localPosition;
        offset = this.transform.position - GetMousePos();
        clickPos = GetMousePos();
        board.StartPlayingCard(this);
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
            transform.localPosition = OP;
            return;
        }

        transform.localPosition = OP;
        return;
    }

    public Vector3 DragPos()
    {
        return GetMousePos() + offset;
    }
    Coroutine dragCoroutine = null;
    void StartDrag()
    {
        if (dragCoroutine==null) StartCoroutine(dragger());
    }
    void EndDrag()
    {
        StopAllCoroutines();
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
        if (board.currTurn==false)
        {
            EndPlay();
            //TODO: Not your turn popup
            return;
        }

        if ((card.SPELL || card.SECRET || card.WEAPON) && card.TARGETED == true)
        {
            if (preview) return;
            //PlayCard();
            board.StartTargetingCard(card);
            //EndPlay();
            HideCard();
        }

        if ((card.MINION) == true)
        {
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

    public IEnumerator dragger()
    {
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
}
