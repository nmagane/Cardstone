using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretDisplay : MonoBehaviour
{
    public Card.Cardname card;
    public Hero hero;
    public Board board => hero.board;
    public SpriteRenderer icon;
    public Sprite[] secretIcons;
    public bool init = false;
    public void Set(Card.Cardname c)
    {
        Database.CardInfo cardInfo = Database.GetCardData(c);
        card = c;
        icon.sprite = secretIcons[(int)cardInfo.classType];
    }

    private void OnMouseExit()
    {
        hoverTimer = 0;
        hero.board.HideHoverTip();
    }

    int hoverTimer = 0;
    private void OnMouseOver()
    {
        if (hoverTimer < 30)
        {
            hoverTimer++;
            if (hoverTimer == 30)
                hero.board.ShowHoverTip(gameObject,card);
        }
    }
}
