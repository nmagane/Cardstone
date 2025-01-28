using System.Collections.Generic;
using System.Linq;
using Riptide;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Hand
{
    //public List<Card.Cardname> cards;
    public List<HandCard> cards;

    public bool server = false;
    //public List<Card> cardObjects;
    public Board board;
    public Dictionary<HandCard, Card> cardObjects = new Dictionary<HandCard, Card>();
    public enum MulliganState
    {
        None,
        Waiting,
        Done,
    }
    public MulliganState mulliganMode = MulliganState.None;
    public bool enemyHand = false;
    public bool coinHand = false;
    public HandCard this[int index]
    {
        get
        {
            return cards[index];
        }

        set
        {
            cards[index] = value;

        }
    }
    public List<HandCard>.Enumerator GetEnumerator()
    {
        return cards.GetEnumerator();
    }
    public enum CardSource
    {
        Deck,
        Board,
        EnemyDeck,
    }
    public HandCard Add(Card.Cardname x, int ind = -1, CardSource source = CardSource.Deck)
    {
        int index = ind == -1 ? Count() : ind;
        HandCard newCard = new HandCard(x, index);
        cards.Add(newCard);

        if (server)
        {
            OrderInds();
            return newCard;
        }

        Card c = board.CreateCard();
        c.board = board;
        c.Set(cards[index]);
        cardObjects.Add(cards[index], c);
        c.transform.parent = board.transform;
        OrderInds();

        return newCard;

    }
    public void RemoveAt(int x)
    {
        if (!server)
        {
            Card c = cardObjects[cards[x]];
            cardObjects.Remove(cards[x]);
            board.DestroyObject(c);
        }

        cards.RemoveAt(x);

        OrderInds();
    }

    public void Remove(HandCard c)
    {
        if (!server)
        {
            Card co = cardObjects[c];
            cardObjects.Remove(c);
            board.DestroyObject(co);
        }
        cards.Remove(c);
        OrderInds();
    }

    public void MulliganReplace(int index, Card.Cardname c)
    {
        cards[index].Set(c, index);
        cardObjects[cards[index]].Set(cards[index]);
        cardObjects[cards[index]].mulliganMark.enabled = false;
    }
    public void EndMulligan()
    {
        mulliganMode = MulliganState.Waiting;
        OrderInds();
    }
    public void ConfirmBothMulligans()
    {
        mulliganMode = MulliganState.Done;
        OrderInds();
    }
    public void OrderInds()
    {
        int i = 0;
        foreach (var c in cards)
        {
            c.index = i++;
        }
        if (server) return;

        if (mulliganMode != MulliganState.Done)
        {
            float count = cardObjects.Count;
            float dist = 5;

            if (coinHand)
            {
                count--;
                var coinKVP = cardObjects.ElementAt(cards.Count - 1);

                coinKVP.Value.transform.localPosition = Vector3.zero;
                coinKVP.Value.transform.localScale = Vector3.zero;
            }

            Vector3 offset = new Vector3(-((count - 1) / 2f * dist), 0);

            foreach (var kvp in cardObjects)
            {
                Card c = kvp.Value;
                if (coinHand && c.card.card == Card.Cardname.Coin) continue;
                c.transform.localScale = Vector3.one * 1.5f;
                c.transform.localPosition = offset + new Vector3(dist * (c.card.index), 0, 0);
            }

            return;
        }
        if (coinHand)
        {
            coinHand = false;
            var coinKVP = cardObjects.ElementAt(cards.Count - 1);
            coinKVP.Value.transform.localScale = Vector3.one;
        }
        float count2 = cardObjects.Count;
        float dist2 = 3f-0.15f*count2;
        float offset2 = -((count2 - 1) / 2f * dist2);

        //x2+y2=r2
        float radius = 40;
        Vector3 circleCenter = new Vector3(0, -radius);
        float centerInt = (count2-1) / 2;
        foreach (var kvp in cardObjects)
        {
            Card c = kvp.Value;
            c.transform.localScale = Vector3.one;
            float x = offset2 + dist2*c.card.index;
            float y = -10 - (radius-Mathf.Sqrt(radius*radius-x*x));
            float angle = 180*Mathf.Acos(x/radius)/Mathf.PI;
            angle = angle-90;
            float radAngle = angle * Mathf.PI / 180f;
            //y = y - Mathf.Abs(centerInt-c.card.index) * 0.5f * Mathf.Tan(Mathf.Abs(radAngle));
            //x = x - ( * Mathf.Tan(radAngle));
            c.transform.localPosition = new Vector3(x, enemyHand ? 10 : y, 0);
            c.transform.localEulerAngles = new Vector3(0, 0, angle);
        }
    }
    public int Count()
    {
        return cards.Count;
    }
    public Hand()
    {
        cards = new List<HandCard>();
    }



}
