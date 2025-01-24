using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking.Types;

public class Creature : MonoBehaviour
{
    public TMP_Text testname;
    public TMP_Text health, damage;
    public SpriteRenderer spriteRenderer;
    public SpriteRenderer tauntSprite;

    public Board board;

    public Board.Minion minion;
    public int index => minion.index;
    public bool preview = false;
    public void Set(Board.Minion c)
    {
        minion = c;
        testname.text = c.card.ToString();
        health.text = c.health.ToString();
        damage.text = c.damage.ToString();
        if (minion.HasAura(Board.Minion.Aura.Type.Taunt))
            EnableTaunt();
    }
    public bool IsFriendly()
    {
        if (board.enemyMinions.Contains(minion))
            return false;
        else
            return true;
    }
    void Start()
    {
        
    }

    void Update()
    {
        damage.text = minion.damage.ToString();
        health.text = minion.health.ToString();
    }

    public void EnableTaunt()
    {
        tauntSprite.enabled = true;
    }
    public void DisableTaunt()
    {
        tauntSprite.enabled = false;
    }

    private void OnMouseOver()
    {
        if (preview) return;
        if (board.targeting && board.dragTargeting)
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (board.targetingMinion == minion)
                {
                    //cancel by releasing on self
                    board.EndTargeting();
                    return;
                }
                board.TargetMinion(minion);
            }
        }
        //TODO: timer for tooltip to show up
    }
    private void OnMouseEnter()
    {
        board.hoveredMinion = this;
    }
    private void OnMouseExit()
    {
        board.hoveredMinion = null;
    }

    int dragCounter = 0;
    int dragTime = 8;
    private void OnMouseDrag()
    {
        if (preview) return;
        if (board.currTurn == false) return;
        if (board.targetingMinion==minion)
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
    private void OnMouseUp()
    {
        if (preview) return; 
        if (board.currTurn == false) return;
        dragCounter = 0;
        if (board.dragTargeting && board.targetingMinion==minion)
        {
            //LETTING GO OVER NOTHING
            if (board.hoveredMinion==null && board.hoveredHero==null) 
                board.EndTargeting();
        }
    }

    Vector3 clickPos = Vector3.zero;
    private void OnMouseDown()
    {
        if (preview) return; 
        if (board.currTurn == false) return;
        if (board.targeting)
        {
            if (board.targetingMinion == minion)
            {
                //cancel by clicking on self
                board.EndTargeting();
                return;
            }

            board.TargetMinion(minion);
            return;
        }

        if (IsFriendly() == false) return;
        if (minion.canAttack == false) return;
        board.StartTargetingAttack(minion);
        clickPos = Card.GetMousePos();
    }
}
