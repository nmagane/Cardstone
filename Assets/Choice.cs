using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Choice : MonoBehaviour
{
    public int index;
    public Card display;
    public Card owner;

    public void Set(int index, Card.Cardname displayCard, Card owner)
    {
        this.owner = owner;
        this.index = index;
        display.Set(new HandCard(displayCard,0));
        display.board = owner.board;
        display.UpdateCardText();
    }


}
