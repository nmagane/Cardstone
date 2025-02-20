using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardPicker : MonoBehaviour
{
    public Card cardObject;
    public CollectionMenu menu;
    public Card.Cardname card => cardObject.card.card;
    public void Set(Card.Cardname c)
    {
        cardObject.Set(new HandCard(c, 0));
    }


    private void OnMouseDown()
    {
        //addcard
    }

}
