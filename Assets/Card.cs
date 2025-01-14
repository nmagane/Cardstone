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
    public SpriteRenderer icon;
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
    }
    void Start()
    {
        
    }

    public void Set(Board.HandCard c)
    {
        card = c;
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
        OP = transform.localPosition;
        offset = this.transform.position - GetMousePos();
    }

    private void OnMouseDrag()
    {
        transform.position = DragPos();
    }

    private void OnMouseUp()
    {
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

            int position = -1;
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
