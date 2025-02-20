using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardPicker : MonoBehaviour
{
    public Card cardObject;
    public CollectionMenu menu;

    public TMP_Text cardName;
    public TMP_Text manaCost;
    public TMP_Text count;

    public Card.Cardname card => cardObject.card.card;
    public void Set(Database.CardInfo c)
    {
        transform.localScale = Vector3.one * 1.4f;
        cardObject.noHover = true;
        cardObject.noReturn = true;
        cardObject.GetComponent<BoxCollider2D>().enabled = false;
        cardObject.transform.localPosition = Vector3.zero;
        cardObject.Set(new HandCard(c.cardname, 0));
    }


    private void OnMouseDown()
    {
        //addcard
    }

}
