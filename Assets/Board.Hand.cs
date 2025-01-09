using System;
using System.Collections.Generic;
using System.Linq;

public partial class Board
{
    [Serializable]
    public class Hand
    {
        //public List<Card.Cardname> cards;
        public List<HandCard> cards;
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
        public void Add(Card.Cardname x,int ind=-1)
        {
            cards.Add(new HandCard(x,ind==-1? Count():ind));
            OrderInds();
        }
        public void RemoveAt(int x)
        {
            cards.RemoveAt(x);
            OrderInds();
        }

        public void Remove(HandCard c)
        {
            cards.Remove(c);
            OrderInds();
        }

        public void OrderInds()
        {
            int i = 0;
            foreach (var c in cards)
            {
                c.index = i++;
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
