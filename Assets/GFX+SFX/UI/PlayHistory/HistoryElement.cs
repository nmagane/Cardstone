using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HistoryElement : MonoBehaviour
{
    public enum Type
    {
        Play,
        Attack,
        Trigger,
    }
    public Board board;
    public SpriteRenderer icon;
    public SpriteRenderer background;
    public SpriteRenderer frame;
    public SpriteRenderer attackIcon;
    public Sprite enemyFrame;
    public Card.Cardname card;
    public Card.Cardname targetCard=Card.Cardname.Cardback;
    public Hero targetHero = null;
    public Hero attackHero = null;
    public Type type;
    public void Set(Card.Cardname card, bool friendly, Type type,Card.Cardname tar = Card.Cardname.Cardback, Hero tarHero=null, Hero attHero=null)
    {
        this.card = card;
        if (!friendly) frame.sprite = enemyFrame;
        this.type = type;
        if (card != Card.Cardname.Cardback)
        {
            Database.CardInfo info = Database.GetCardData(card);
            background.sprite = board.minionObject.GetComponent<Creature>().frameSprites[(int)info.classType];
            icon.sprite = board.cardObject.GetComponent<Card>().cardSprites[(int)card];
        }
        else
        {
            if (attHero != null)
            {
                icon.sprite = attHero.spriteRenderer.sprite;
                icon.transform.localScale = Vector3.one * 0.5f;
            }
        }

        targetCard = tar;
        targetHero = tarHero;
        attackHero = attHero;

        switch (type)
        {
            case Type.Attack:
                attackIcon.transform.localScale = Vector3.one;
                break;
        }
    }

    int hoverTimer = 0;
    private void OnMouseOver()
    {
        if (hoverTimer < 30)
        {
            hoverTimer++;
            if (hoverTimer == 30)
                board.ShowHoverTip(gameObject,card, true,targetCard,targetHero,attackHero, type==Type.Attack);
        }
    }
    private void OnMouseExit()
    {
        hoverTimer = 0;
        board.HideHoverTip();
    }
}