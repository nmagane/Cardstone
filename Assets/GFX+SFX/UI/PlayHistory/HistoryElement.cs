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
    public Sprite enemyFrame;
    public Card.Cardname card;
    public Type type;
    public List<Minion> targetMinions;
    public List<Hero> targetHeroes;
    public void Set(Card.Cardname card, bool friendly, Type type)
    {
        this.card = card;
        if (!friendly) frame.sprite = enemyFrame;
        this.type = type;

        Database.CardInfo info = Database.GetCardData(card);
        background.sprite = board.minionObject.GetComponent<Creature>().frameSprites[(int)info.classType];
        icon.sprite = board.cardObject.GetComponent<Card>().cardSprites[(int)card];

    }
}
