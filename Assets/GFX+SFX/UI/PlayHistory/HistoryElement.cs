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
    public Card.Cardname card;
    public void Set(Card.Cardname card)
    {
        this.card = card;
        Database.CardInfo info = Database.GetCardData(card);
        background.sprite = board.minionObject.GetComponent<Creature>().frameSprites[(int)info.classType];
        icon.sprite = board.cardObject.GetComponent<Card>().cardSprites[(int)card];
    }
}
