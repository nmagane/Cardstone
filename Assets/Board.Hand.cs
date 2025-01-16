using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class Board
{
    [Serializable]
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
        public void Add(Card.Cardname x,int ind=-1, CardSource source = CardSource.Deck)
        {
            int index = ind == -1 ? Count() : ind;
            cards.Add(new HandCard(x,index));

            if (server)
            {
                OrderInds();
                return;
            }

            Card c = Instantiate(board.cardObject).GetComponent<Card>();
            c.board = board;
            c.Set(cards[index]);
            cardObjects.Add(cards[index], c);
            c.transform.parent = board.transform;
            OrderInds();

        }
        public void RemoveAt(int x)
        {
            if (!server)
            {
                Card c = cardObjects[cards[x]];
                cardObjects.Remove(cards[x]);
                Destroy(c.gameObject);
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
                Destroy(co.gameObject);
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

            if (mulliganMode!=MulliganState.Done)
            {
                float count = cardObjects.Count;
                float dist = 5;
                Vector3 offset = new Vector3(-((count-1)/2f * dist), 0);
                foreach (var kvp in cardObjects)
                {
                    Card c = kvp.Value;
                    c.transform.localScale = Vector3.one*1.5f;
                    c.transform.localPosition = offset +new Vector3(dist * (c.card.index), 0, 0);
                }
                return;
            }

            
            foreach (var kvp in cardObjects)
            {
                Card c = kvp.Value;
                c.transform.localScale = Vector3.one;
                c.transform.localPosition = new Vector3(-15+4*(c.card.index),enemyHand?10:-10,0);
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

    public static bool RNG(float percent) //Two point precision. RNG(XX.XXf)
    {
        percent *= 100;
        return (UnityEngine.Random.Range(0, 10000) <= percent);
    }
    public static T RandElem<T>(List<T> L)
    {
        return L[UnityEngine.Random.Range(0, L.Count)];
    }
    public static KeyValuePair<T, Q> RandElem<T, Q>(Dictionary<T, Q> L)
    {
        List<T> L2 = new List<T>(L.Keys);

        T K2 = RandElem(L2);
        Q V2 = L[K2];

        KeyValuePair<T, Q> kvp = new KeyValuePair<T, Q>(K2, V2);

        return kvp;
    }
    public static T RandElem<T>(T[] L)
    {
        return L[UnityEngine.Random.Range(0, L.Length)];
    }
    public static List<T> Shuffle<T>(List<T> l)
    {
        return l.OrderBy(x => UnityEngine.Random.value).ToList();
    }
    public static T[] Shuffle<T>(T[] l)
    {
        return l.OrderBy(x => UnityEngine.Random.value).ToArray();
    }
        
}
