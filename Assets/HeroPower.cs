using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HeroPower : MonoBehaviour
{
    public Board board;
    public SpriteRenderer icon;
    public TMP_Text manaText;

    public Sprite activeSprite;
    public Sprite disabledSprite;

    public int manaCost = 2;

    public new bool enabled = true;
    public bool TARGETED = false;

    HandCard card;

    public void Set(Card.Cardname heroPower)
    {
        card = new HandCard(heroPower, 0);
    }

    public void Enable()
    {
        enabled = true;
        icon.sprite = activeSprite;
        manaText.transform.localScale = Vector3.one*0.5f;
    }
    public void Disable()
    {
        enabled = false;
        icon.sprite = disabledSprite;
        manaText.transform.localScale = Vector3.zero;
    }
    private void OnMouseDown()
    {
        if (enabled == false || transform.position.y>0) return;
        if (board.currTurn == false) return; 
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
