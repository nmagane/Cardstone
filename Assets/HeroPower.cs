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

    public Card.Cardname ability;
    public int manaCost = 2;

    public new bool enabled = true;
    public bool TARGETED = false;

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
        if (board.currMana < manaCost)
        {
            Debug.Log("TODO: ERROR NO MANA HERO POWER");
            return;
        }
        board.CastHeroPower(ability, -1, true, true);
    }
}
