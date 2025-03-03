using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tooltip : MonoBehaviour
{
    public Card card;
    public bool anim = false;
    Coroutine hider = null;
    public GameObject targetContainer;
    public Card targetCard;
    public SpriteRenderer targetHero;
    public SpriteRenderer attackHero;
    public SpriteRenderer arrow;
    public Sprite attackSprite;
    public Sprite arrowSprite;
    public void Set(Card.Cardname name, int delay=-1,Card.Cardname tar = Card.Cardname.Cardback,Hero tarHero=null,Hero attHero=null, bool attack=false)
    {
        if (attHero == null)
        {
            attackHero.transform.localScale = Vector3.zero;
            card.transform.localScale = Vector3.one * 2;
            card.SetSortingOrder(delay != -1 ? 11 : 12);
            if (!anim) transform.localScale = Vector3.one;
            card.Set(new HandCard(name, 1));
        }
        else
        {
            attackHero.transform.localScale = Vector3.one*2;
            card.transform.localScale = Vector3.zero;
            attackHero.sprite = attHero.spriteRenderer.sprite;
        }
        if (delay != -1)
        {
            if (hider!=null)
            {
                StopCoroutine(hider);
                hider = null;
            }
            
            hider = StartCoroutine(disappear(delay));
        }
        if (attack)
        {
            arrow.sprite = attackSprite;
        }
        else
            arrow.sprite = arrowSprite;

        if (tarHero!=null)
        {
            SetTarget(tarHero);
        }
        else if (tar != Card.Cardname.Cardback)
        {
            SetTarget(tar);
        }
        else
        {
            targetContainer.transform.localScale = Vector3.zero;
        }
    }

    public void SetTarget(Card.Cardname target)
    {
        targetContainer.transform.localScale = Vector3.one;

        targetCard.transform.localScale = Vector3.one*2;
        targetHero.transform.localScale = Vector3.zero;

        targetCard.SetSortingOrder(12);
        targetCard.Set(new HandCard(target, 1));
    }
    public void SetTarget(Hero target)
    {
        targetContainer.transform.localScale = Vector3.one;

        targetCard.transform.localScale = Vector3.zero;
        targetHero.transform.localScale = Vector3.one*2;

        targetHero.sprite = target.spriteRenderer.sprite;
    }

    public void Hide()
    {
        transform.localScale = Vector3.zero;
        targetContainer.transform.localScale = Vector3.zero;
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
