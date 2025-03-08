using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HeroPower : MonoBehaviour
{
    public Board board;
    public SpriteRenderer icon;
    public SpriteRenderer frame;
    public TMP_Text manaText;

    public Sprite activeSprite;
    public Sprite disabledSprite;

    public SpriteRenderer highlight;

    public Sprite[] heroPowerSprites;
    public Sprite[] specialHeroPowerSprites;

    public int manaCost => card.manaCost;

    public new bool enabled = true;

    public HandCard card;

    public void Set(Card.Class classType,Card.Cardname heroPower)
    {
        card = new HandCard(heroPower, 0);
        icon.sprite = heroPowerSprites[(int)classType];
    }
    public void Set(Card.Cardname heroPower)
    {
        Enable();

        card = new HandCard(heroPower, 0);
        switch (heroPower)
        {
            case Card.Cardname.Inferno:
                icon.sprite = specialHeroPowerSprites[0];
                break;
        }
        board.animationManager.BounceZoom(this.gameObject, 0.1f);
    }

    public void Enable()
    {
        enabled = true;
        icon.enabled = true;
        frame.sprite = activeSprite;
        manaText.transform.localScale = Vector3.one;
    }
    public void Disable()
    {
        enabled = false;
        icon.enabled = false;
        frame.sprite = disabledSprite;
        manaText.transform.localScale = Vector3.zero;
    }

    public void Highlight()
    {
        if (enabled == false) return;
        highlight.enabled = true;
    }
    public void Unhighlight()
    {
        highlight.enabled = false;
    }

    int hoverTimer = 0;
    private void OnMouseOver()
    {
        if (board.currHand.mulliganMode != Hand.MulliganState.Done)
        {
            return;
        }
        if (hoverTimer < 30)
        {
            hoverTimer++;
            if (hoverTimer == 30)
                board.ShowHoverTip(this);
        }
    }
    private void OnMouseExit()
    {
        hoverTimer = 0;
        board.HideHoverTip();
    }
    private void OnMouseDown()
    {
        if (enabled == false || transform.position.y>0) return;
        if (board.currTurn == false) return;
        if (board.disableInput) return;
        if (board.targeting)
        {
            return;
        }
        if (board.currMana < manaCost)
        {
            Debug.Log("TODO: ERROR NO MANA HERO POWER");
            return;
        }
        if (card.TARGETED)
        {
            board.StartTargetingHeroPower(card);
        }
        else
        {
            switch (card.card)
            {
                default:
                    //TODO: target enemy for hunter hero power etc...
                    board.CastHeroPower(card.card, -1, true, true);
                    break;
            }
        }
        clickPos = Card.GetMousePos();
    }
    Vector3 clickPos = Vector3.zero;
    
    int dragCounter = 0;
    int dragTime = 5;
    private void OnMouseDrag()
    {
        if (enabled == false || transform.position.y > 0) return;
        if (board.disableInput) return;
        if (board.currHand.mulliganMode != Hand.MulliganState.Done)
        {
            return;
        }
        if (board.targetingCard == card)
        {
            if (dragCounter < dragTime) dragCounter++;
            if (dragCounter >= dragTime)
            {
                if (Vector3.Distance(Card.GetMousePos(), clickPos) > 0.2f)
                {
                    board.dragTargeting = true;
                    //Debug.Log("drag");
                }
            }
        }
    }
}
