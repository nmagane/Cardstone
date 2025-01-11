using System;
using System.Collections.Generic;
using Riptide;
using System.Reflection;
using UnityEngine;

public partial class Board
{
    [Serializable]
    public class MinionBoard
    {
        public List<Minion> minions;

        public Board board;
        public Dictionary<Minion, Creature> minionObjects = new Dictionary<Minion, Creature>();
        public bool server = false;
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
            if (Count() == 0)
            {
                minions.Add(new Minion(c, 0));
            }
            else if (Count() != 0 && ind>= Count())
            {
                minions.Add(new Minion(c, Count()));
            }
            else minions.Insert(ind, new Minion(c, ind));

            if (server)
            {
                OrderInds();
                return;
            }

            Creature creature = Instantiate(board.minionObject).GetComponent<Creature>();
            creature.board = board;
            creature.Set(minions[ind]);
            minionObjects.Add(minions[ind], creature);
            creature.transform.parent = board.transform;

            OrderInds();
        }
        public void RemoveAt(int x)
        {
            if (!server)
            {
                Creature c = minionObjects[minions[x]];
                minions.Remove(minions[x]);
                Destroy(c.gameObject);
            }
            minions.RemoveAt(x);
            OrderInds();
        }

        public void Remove(Minion c)
        {
            if (!server)
            {
                Creature co = minionObjects[c];
                minionObjects.Remove(c);
                Destroy(co.gameObject);
            }
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
            if (server) return;

            i = 0;
            foreach (var kvp in minionObjects)
            {
                Creature c = kvp.Value;
                c.transform.localPosition = new Vector3(-9 + 4.5f * (i++), this==board.currMinions?-2.75f:3, 0);
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
