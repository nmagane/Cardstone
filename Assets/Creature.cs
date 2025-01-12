using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Creature : MonoBehaviour
{
    public TMP_Text testname;
    public TMP_Text heatlh, damage;
    public SpriteRenderer spriteRenderer;

    public Board board;

    public void Set(Board.Minion c)
    {
        testname.text = c.card.ToString();
        heatlh.text = c.health.ToString();
        damage.text = c.damage.ToString();
    }

    void Start()
    {
        
    }

    void Update()
    {
        
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
            //board.AttackMinion(board.attacker, this);
        }
        board.StartTargeting(this.gameObject);
    }
}
