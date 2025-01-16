using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Device;

public class Card : MonoBehaviour
{
    public Board board;
    public Board.HandCard card;

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
        F0,
        F1,
        F2,
        F3,
        F4,
        F5,
        F6,
        F7,
        F8,
        F9,
        F10,
        F11,
        F12,
        F13,
        F14,
        F15,
        Cardback,
    }
    void Start()
    {
        
    }

    public void Set(Board.HandCard c)
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
        name.text = c.card.ToString();

    }
    void Update()
    {
        
    }
    public Vector3 GetMousePos()
    {
        return (Camera.main.ScreenToWorldPoint(Input.mousePosition) - new Vector3(0, 0, Camera.main.ScreenToWorldPoint(Input.mousePosition).z));
    }
    public Vector3 DragPos()
    {

        return GetMousePos() + offset;
    }

    Vector3 offset;
    Vector3 OP = new Vector3();
    private void OnMouseDown()
    {
        if (card.card == Cardname.Cardback) return;
        if (board.currHand.mulliganMode==Board.Hand.MulliganState.None)
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
            return;
        }
        OP = transform.localPosition;
        offset = this.transform.position - GetMousePos();
    }

    private void OnMouseDrag()
    {
        if (card.card == Cardname.Cardback) return;
        if (board.currHand.mulliganMode!=Board.Hand.MulliganState.Done)
        {
            return;
        }
        transform.position = DragPos();
    }

    private void OnMouseUp()
    {

        if (card.card == Cardname.Cardback) return;
        if (board.currHand.mulliganMode!=Board.Hand.MulliganState.Done)
        {
            return;
        }
        if (board.currTurn == false)
        {
            //ERROR: NOT YOUR TURN
            transform.localPosition = OP;
            return;
        }


        if (card.manaCost>board.currMana)
        {
            //ERROR: NOT ENOUGH MANA
            transform.localPosition = OP;
            return;
        }

        if ((card.SPELL || card.SECRET || card.WEAPON) && card.TARGETED == false)
        {
            //UNTARGETED NON-MINION
        }

        if ((card.SPELL || card.SECRET || card.WEAPON) && card.TARGETED == true)
        {
            //TARGETED NON-MINION
        }


        if (board.hoveredSide == board.enemySide)
        {
            transform.localPosition = OP;
            return;
        }

        if (card.MINION && card.TARGETED==false)
        {
            //SIMPLE MINION SUMMON
            if (board.currMinions.Count() >= 7)
            {
                transform.localPosition = OP;
                return;
            }

            int position = -1   ;
            board.PlayCard(card, -1, position);
            return;
        }

        if (card.MINION && card.TARGETED==true)
        {
            //MINION WITH TARGET ABILITY
            //place temporary minion and start targetining effect
            if (board.currMinions.Count() >= 7)
            {
                transform.localPosition = OP;
                return;
            }
        }


        transform.localPosition = OP;
        return;
    }


}
