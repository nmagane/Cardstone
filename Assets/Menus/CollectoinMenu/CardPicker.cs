using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardPicker : MonoBehaviour
{
    public Card cardObject;
    public CollectionMenu menu;

    public Card.Cardname card => cardObject.card.card;
    public void Set(Database.CardInfo c)
    {
        transform.localScale = Vector3.one * 1.4f;
        cardObject.noHover = true;
        cardObject.noReturn = true;
        cardObject.shadow.elevation = 0.1f;
        cardObject.GetComponent<BoxCollider2D>().enabled = false;
        cardObject.transform.localPosition = Vector3.zero;
        cardObject.Set(new HandCard(c.cardname, 0));
    }


    private void OnMouseDown()
    {
        bool added = menu.AddListing(card);
        if (added)
            menu.board.animationManager.BounceZoom(this.gameObject, 0.1f);
        else
            StartCoroutine(Shake());
    }

    public void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (menu.cards.Contains(card))
            {
                menu.board.animationManager.BounceZoom(this.gameObject, -0.1f);
                menu.RemoveListing(card);
            }
        }
    }

    IEnumerator Shake()
    {
        this.transform.localPosition += new Vector3(0.1f, 0);
        yield return null;
        yield return null; 
        this.transform.localPosition += new Vector3(-0.2f, 0);
        yield return null;
        yield return null;
        this.transform.localPosition += new Vector3(0.1f, 0);
    }

}
