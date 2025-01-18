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

            if (ind == -1)
            {
                ind = Count();
            }
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
                minionObjects.Remove(minions[x]);
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

            previewing = false;
            currPreview = -1;
            i = 0;
            float count = minionObjects.Count;
            float dist = 4.5f;
            float offset = -((count - 1) / 2f * dist);
            foreach (var kvp in minionObjects)
            {
                Creature c = kvp.Value;
                c.transform.localPosition = new Vector3(offset + dist * (kvp.Key.index), this==board.currMinions?-2.75f:3, 0);
            }
        }

        public bool previewing = false;
        public int currPreview = -1;
        public void PreviewGap(int gapIndex)
        {
            if (previewing)
            {
                if (currPreview==gapIndex)
                    return;
            }
            previewing = true;
            currPreview = gapIndex;
            float count = minionObjects.Count+1;
            float dist = 4.5f;
            float offset = -((count - 1) / 2f * dist);
            foreach (var kvp in minionObjects)
            {
                Creature c = kvp.Value;
                int ind = kvp.Key.index;
                if (ind >= gapIndex) ind++;
                c.transform.localPosition = new Vector3(offset + dist * (ind), this == board.currMinions ? -2.75f : 3, 0);
            }
        }
        public void EndPreview()
        {
            if (previewing == false)
                return;
            previewing = false;
            currPreview = -1;
            OrderInds();
        }
        //public void End

        public int Count()
        {
            return minions.Count;
        }
        public bool Contains(Minion m)
        {
            return minions.Contains(m);
        }
        public MinionBoard()
        {
            minions = new List<Minion>();
        }
    }

}
