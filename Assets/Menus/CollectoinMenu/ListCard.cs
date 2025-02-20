using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ListCard : MonoBehaviour
{
    public CollectionMenu menu;

    public TMP_Text cardName;
    public TMP_Text manaCostText;
    public TMP_Text countText;

    public SpriteRenderer bg;
    public Card.Cardname card;
    public int manaCost = 0;
    public bool legendary = false;
    public int count = 1;
    public void Set(Database.CardInfo info)
    {
        card = info.cardname;
        manaCostText.text = info.manaCost.ToString();
        manaCost = info.manaCost;
        cardName.text = info.name;
        count = 1;
        legendary = info.LEGENDARY;
    }

    public void SetCount(int x)
    {
        count = x;
        if (legendary) countText.text = "*";
        else countText.text = "x"+x.ToString();
    }

    private void OnMouseDown()
    {
        menu.RemoveListing(card);
    }
}
