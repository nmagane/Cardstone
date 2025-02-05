using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tooltip : MonoBehaviour
{
    public Card card;
    public bool anim = false;
    public void Set(Card.Cardname name, int delay=-1)
    {
        card.SetSortingOrder(11);
        if (!anim) transform.localScale = Vector3.one;
        card.Set(new HandCard(name, 0));

        if (delay != -1)
            StartCoroutine(disappear(delay));

    }

    public void Hide()
    {
        transform.localScale = Vector3.zero;
    }

    IEnumerator disappear(int x)
    {
        yield return AnimationManager.Wait(x);
        Hide();
    }
    private void OnMouseDown()
    {
        transform.localScale = Vector3.zero;
    }
}
