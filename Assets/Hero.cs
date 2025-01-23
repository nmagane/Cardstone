using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Hero : MonoBehaviour
{
    public Board board;
    public SpriteRenderer spriteRenderer;
    public TMP_Text hpText;
    public int health = 30;
    public int maxHealth = 30;

    public enum Class
    {
        Mage,
        Warrior,
        Warlock,
        Rogue,
        Druid,
        Hunter,
        Priest,
        Shaman,
        Paladin
    }

    public void Damage(int x)
    {
        health -= x;
        UpdateText();
    }
    public void SetHealth(int x)
    {
        health = x;
        UpdateText();
    }

    private void UpdateText()
    {
        hpText.text = health.ToString();
    }
    void Update()
    {
        
    }


    private void OnMouseOver()
    {
        if (board.targeting && board.dragTargeting)
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (board.targetingHero == this)
                {
                    //cancel by releasing on self
                    board.EndTargeting();
                    return;
                }
                board.TargetHero(this);
            }
        }
    }

    private void OnMouseDown()
    {
        if (board.targeting)
        {
            if (board.targetingHero == this)
            {
                //cancel by clicking on self
                board.EndTargeting();
                return;
            }

            board.TargetHero(this);
            return;
        }
    }

    private void OnMouseEnter()
    {
        board.hoveredHero = this;
    }
    private void OnMouseExit()
    {
        board.hoveredHero = null;
    }
    public Hero()
    {

    }
}
