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
        if (owner.board.ValidTargetsAvailable(display.card.eligibleTargets))
        {
            display.Highlight();
        }
        else
        {
            GetComponent<BoxCollider2D>().enabled = false;
        }
    }

    private void OnMouseDown()
    {
        if (owner!=null)
            owner.ChooseOption(index);
    }
}
