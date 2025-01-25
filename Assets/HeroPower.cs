using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroPower : MonoBehaviour
{
    public Board board;
    public SpriteRenderer icon;

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
    }
    public void Disable()
    {
        enabled = false;
        icon.sprite = disabledSprite;
    }
    private void OnMouseDown()
    {
        
    }
}
