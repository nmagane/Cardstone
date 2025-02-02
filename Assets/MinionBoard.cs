using System;
using System.Collections.Generic;
using Riptide;
using System.Reflection;
using UnityEngine;
[Serializable]
public class MinionBoard
{
    public List<Minion> minions;

    public Board board;
    public Dictionary<Minion, Creature> minionObjects = new Dictionary<Minion, Creature>();
    public bool server = false;
    public Creature previewMinion = null;
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
    public Minion Add(Card.Cardname c, int ind = -1, int playOrder = 0)
    {
        Minion newMinion = null;
        if (ind == -1)
        {
            ind = Count();
        }
        if (Count() == 0)
        {
            newMinion = new Minion(c, 0, this);
            minions.Add(newMinion);
        }
        else if (Count() != 0 && ind >= Count())
        {
            newMinion = new Minion(c, Count(), this);
            minions.Add(newMinion);
        }
        else
        {
            newMinion = new Minion(c, ind, this);
            minions.Insert(ind, newMinion);
        }
        if (server)
        {
            OrderInds();
            return newMinion;
        }
        Creature creature = board.CreateCreature();
        creature.board = board;
        creature.Set(minions[ind]);
        minionObjects.Add(minions[ind], creature);
        creature.transform.parent = board.gameAnchor.transform;

        OrderInds();
        return newMinion;
    }
    public void RemoveAt(int x)
    {
        if (!server)
        {
            Creature c = minionObjects[minions[x]];
            minionObjects.Remove(minions[x]);
            board.DestroyObject(c);
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
            board.DestroyObject(co);
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
            c.previewIndex = -1;
        }
        if (server) return;

        i = 0;
        float count = minionObjects.Count;
        float dist = 4.7f;
        float offset = -((count - 1) / 2f * dist);
        foreach (var kvp in minionObjects)
        {
            Creature c = kvp.Value;
            Vector3 targetPos = new Vector3(offset + dist * (kvp.Key.index), this == board.currMinions ? -2.25f : 2.5f, 0);

            if (c.init==false && c.index == currPreview && previewMinion!=null)
            {
                if (previewMinion.minion.card == c.minion.card)
                {
                    c.transform.localPosition = targetPos;
                    c.shadow.elevation = 0;
                    c.transform.localScale = Vector3.one;
                    c.init = true;
                }
            }
            else if (c.init == false)
            {
                c.transform.localPosition = targetPos+new Vector3(0,3);
                c.shadow.elevation = 2;
                c.transform.localScale = Vector3.one * 1.15f;
                DropCreature(c, targetPos);
                c.init = true;
            }
            else MoveCreature(c, targetPos);
        }

        if (previewMinion != null)
        {
            board.DestroyObject(previewMinion);
            previewMinion = null;
        }
        previewing = false;
        currPreview = -1;
    }

    public bool previewing = false;
    public int currPreview = -1;
    public void PreviewGap(int gapIndex)
    {
        if (previewing)
        {
            if (currPreview == gapIndex)
                return;
        }
        previewing = true;
        currPreview = gapIndex;
        float count = minionObjects.Count + 1;
        float dist = 4.7f;
        float offset = -((count - 1) / 2f * dist);
        foreach (var kvp in minionObjects)
        {
            Creature c = kvp.Value;
            int ind = kvp.Key.index;
            if (ind >= gapIndex) ind++;
            kvp.Key.previewIndex = ind;
            //c.transform.localPosition 
            Vector3 targetPos = new Vector3(offset + dist * (ind), this == board.currMinions ? -2.25f : 2.5f, 0);
            MoveCreature(c, targetPos);
        }
    }
    public void EndPreview()
    {
        if (previewing == false)
            return;
        previewing = false;
        currPreview = -1;
        if (previewMinion != null)
        {
            board.DestroyObject(previewMinion);
            previewMinion = null;
        }
        OrderInds();
    }

    public Vector3 SpawnPreviewMinion(Card.Cardname card, int pos)
    {
        Creature creature = board.CreateCreature();
        creature.board = board;
        Minion prev = new Minion(card, pos, this);
        creature.Set(prev);
        previewMinion = creature;
        creature.transform.parent = board.gameAnchor.transform;
        creature.preview = true;
        float count = minionObjects.Count + 1;
        float dist = 4.7f;
        float offset = -((count - 1) / 2f * dist);


        Vector3 targetPos = new Vector3(offset + dist * (pos), this == board.currMinions ? -2.25f : 2.5f, 0);
        creature.transform.localPosition = targetPos + new Vector3(0, 3);
        creature.shadow.elevation = 2;
        creature.transform.localScale = Vector3.zero;
        DropCreature(creature, targetPos, 10);
        creature.init = true;

        return creature.transform.localPosition;

    }

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

    public void MoveCreature(Creature c, Vector3 location)
    {
        board.animationManager.LerpTo(c, location, 5, 0.1f);
    }
    public void DropCreature(Creature c, Vector3 location, int delay = 0)
    {
        if (delay>0)
        {
            board.animationManager.DelayedDrop(c, location, delay, this);
            return;
        }
        int F = 10;
        board.animationManager.LerpTo(c, location, F);
        board.animationManager.DropMinion(c, F);
        board.animationManager.LerpZoom(c.gameObject, Vector3.one, F,0);

        c.boardPos = location;
    }
 

}
