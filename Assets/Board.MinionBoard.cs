using System;
using System.Collections.Generic;
using UnityEngine;

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
        public void Add(Card.Cardname c, int ind = -1)
        {
            Debug.Log(Count() + " "+c+" "+ind);
            //minions.Add(new Minion(m, ind == -1 ? Count() : ind));
            if (Count() == 0)
            {
                minions.Add(new Minion(c, 0));
            }
            else if (Count() != 0 && ind>= Count())
            {
                minions.Add(new Minion(c, Count()));
            }
            else minions.Insert(ind, new Minion(c, ind));
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
