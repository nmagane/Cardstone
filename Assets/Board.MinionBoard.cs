using System;
using System.Collections.Generic;

public partial class Board
{
    [Serializable]
    public class MinionBoard
    {
        public List<Minion> minions;
        public Minion this[int index]
        {
            get
            {
                return minions[index];
            }

            set
            {
                minions[index] = value;

            }
        }
        public List<Minion>.Enumerator GetEnumerator()
        {
            return minions.GetEnumerator();
        }
        public void Add(Card.Cardname m, int ind = -1)
        {
            minions.Add(new Minion(m, ind == -1 ? Count() : ind));
            OrderInds();
        }
        public void RemoveAt(int x)
        {
            minions.RemoveAt(x);
            OrderInds();
        }

        public void Remove(Minion c)
        {
            minions.Remove(c);
            OrderInds();
        }

        public void OrderInds()
        {
            int i = 0;
            foreach (var c in minions)
            {
                c.index = i++;
            }
        }
        public int Count()
        {
            return minions.Count;
        }
        public MinionBoard()
        {
            minions = new List<Minion>();
        }
    }

}
