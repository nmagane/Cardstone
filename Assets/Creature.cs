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

    public Board board;

    public Board.Minion minion;

    public void Set(Board.Minion c)
    {
        minion = c;
        testname.text = c.card.ToString();
        health.text = c.health.ToString();
        damage.text = c.damage.ToString();
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

    
    private void OnMouseOver()
    {
        //timer for tooltip to show up
    }
    private void OnMouseEnter()
    {
        board.hoveredMinion = this;
    }
    private void OnMouseExit()
    {
        board.hoveredMinion = null;
    }

    private void OnMouseDown()
    {
        if (board.targeting)
        {
            //check if this is a valid target
            board.AttackMinion(board.targetingMinion,this.minion);
            return;
        }
        if (IsFriendly() == false) return;
        board.StartTargetingAttack(minion);
    }
}
