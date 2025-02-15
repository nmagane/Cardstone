using System;
using System.Collections.Generic;
using Riptide;
using System.Reflection;
using UnityEngine;
using Mirror.BouncyCastle.Bcpg;
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
            newMinion = new Minion(c, 0, this, playOrder);
            minions.Add(newMinion);
        }
        else if (Count() != 0 && ind >= Count())
        {
            newMinion = new Minion(c, Count(), this, playOrder);
            minions.Add(newMinion);
        }
        else
        {
            newMinion = new Minion(c, ind, this, playOrder);
            minions.Insert(ind, newMinion);
        }

        OrderInds();

        if (server)
        {
            return newMinion;
        }

        //AddCreature(minions[ind]);

        return newMinion;
    }
    public enum MinionSource
    {
        Play,
        Summon,
    }
    public void AddCreature(Minion m, MinionSource source=MinionSource.Play)
    {
        Creature creature = board.CreateCreature();
        creature.board = board;
        creature.Set(m);
        minionObjects.Add(m, creature);
        m.creature = creature;
        creature.transform.parent = board.gameAnchor.transform;
        creature.source = source;

        
        foreach (Creature c in minionObjects.Values)
        {
            c.order = c.minion.index;
        }

        OrderCreatures();
    }

    public void RemoveCreature(Minion m, bool noAnim=false, bool removal=false)
    {
        Creature c = minionObjects[m];
        minionObjects.Remove(m);
        if (removal) board.animationManager.RemoveMinion(c,10);
        else board.animationManager.DeathAnim(c);

        if (noAnim) return;
        OrderCreatures();
    }

    public Minion RemoveAt(int x)
    {
        Minion m = minions[x];
        minions.RemoveAt(x);
        OrderInds();

        if (!server)
        {
            m.creature.GetComponent<BoxCollider2D>().enabled = false;
            m.creature.Unhighlight();
        }
        return m;
    }
    public void Remove(Minion c)
    {
        if (!server)
        {
            Creature co = minionObjects[c];
            minionObjects.Remove(c);
            board.animationManager.DeathAnim(co);
        }
        minions.Remove(c);
        OrderInds();
    }

    public static float _dist = 4.7f;
    public float dist => _dist + Mathf.Max(0, 7 - (minionObjects.Count+(previewing?1:0))) * 0.3f;
    public void OrderInds()
    {
        int i = 0;
        foreach (var c in minions)
        {
            c.index = i++;
            c.previewIndex = -1;
        }
        if (server) return;
        //OrderCreatures();
    }

    public void OrderCreatures()
    {
        previewing = false;

        List<Creature> creatures = new();
        foreach (var kvp in minionObjects)
        {
            creatures.Add(kvp.Value);
        }

        //creatures.Sort((x, y) => x.minion.index.CompareTo(y.minion.index));
        creatures.Sort((x, y) => x.order.CompareTo(y.order));

        if (this == board.currMinions)
        {
            foreach (var kvp in board.prePlayMinions)
            {
                if (creatures.Contains(kvp.Value.creature)) creatures.Remove(kvp.Value.creature);
                if (creatures.Count == 0 || kvp.Key >= creatures.Count) creatures.Add(kvp.Value.creature);
                else creatures.Insert(kvp.Key, kvp.Value.creature);
            }
        }
        //creatures.Sort((x, y) => x.minion.index.CompareTo(y.minion.index));
        //todo: change count or whatever
        float count = creatures.Count;
        float offset = -((count - 1) / 2f * dist);

        int i = 0;
        foreach (Creature c in creatures)
        {
            //todo: sort the minionobjects by kvp.key.index then have it be dist*i++ below,
            //so it doesnt move ahead of itself?
            c.order = i;
            Vector3 targetPos = new Vector3(offset + dist * i++, this == board.currMinions ? -2.25f : 2.5f, 0);

            if (c.init == false && c.index == currPreview && previewMinion != null)
            {
                //battlecry prev
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
                DropCreature(c, targetPos,10);
                c.init = true;
            }
            else MoveCreature(c, targetPos);
        }

        if (previewMinion != null)
        {
            board.DestroyObject(previewMinion);
            previewMinion = null;
        }
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
        if (currPreview>=7) currPreview = minionObjects.Count;
        float offset = -((count - 1) / 2f * dist);
        foreach (var kvp in minionObjects)
        {
            Creature c = kvp.Value;
            int ind = kvp.Key.index;
            if (ind >= gapIndex) ind++;
            kvp.Key.previewIndex = ind;
            kvp.Value.order = ind;
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
        OrderCreatures();
    }

    public Vector3 SpawnPreviewMinion(Card.Cardname card, int pos)
    {
        Creature creature = board.CreateCreature();
        if (pos >= 7) pos = minionObjects.Count;
        creature.board = board;
        Minion prev = new Minion(card, pos, this);
        creature.Set(prev);
        previewMinion = creature;
        creature.transform.parent = board.gameAnchor.transform;
        creature.preview = true;
        float count = minionObjects.Count + 1;
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
        float bounce = 0.1f;

        if (Vector3.Distance(c.transform.localPosition,location)< 0.2f)
        {
            bounce = 0;
        }

        board.animationManager.LerpTo(c, location, 5, bounce);
    }
    public void DropCreature(Creature c, Vector3 location, int delay = 10)
    {
        c.transform.localPosition = location + new Vector3(0, 3);
        c.shadow.elevation = 2;
        if (delay>0)
        {
            c.floatEnabled = false;
            c.transform.localScale = Vector3.zero;
            board.animationManager.DelayedDrop(c, location, delay, this);
            return;
        }
        //if (board.playerID == 100) Debug.Log("Dropping minion " + c.minion.card);
        c.floatEnabled = true;
        c.transform.localScale = Vector3.one * 1.15f;
        int F = 10;
        board.animationManager.LerpTo(c, location, F);
        board.animationManager.DropMinion(c, F);
        board.animationManager.LerpZoom(c.gameObject, Vector3.one, F,0);

        c.boardPos = location;
    }
 

}
