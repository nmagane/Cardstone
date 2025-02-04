
using UnityEngine;
public partial class Board
{
    public void CheckHighlights()
    {
        UnhighlightAll();

        if (currTurn == false) return;

        if (targetMode == TargetMode.None)
        {
            HighlightActions();
        }
        else
        {
            HighlightTargets();
        }
    }

    public void UnhighlightAll()
    {
        foreach (var c in currHand)
        {
            currHand.cardObjects[c].Unhighlight();
        }

        foreach (var m in currMinions)
        {
            currMinions.minionObjects[m].Unhighlight();
        }

        heroPower.Unhighlight();
    }


    public void HighlightActions()
    {
        foreach (var c in currHand)
        {
            if (c.manaCost > currMana) continue;
            if (c.SPELL && c.TARGETED && ValidTargetsAvailable(c.eligibleTargets) == false) continue;
            if (c.MINION && currMinions.Count() >= 7) continue;
            currHand.cardObjects[c].Highlight();
        }

        foreach (var m in currMinions)
        {
            if (m.canAttack == false) continue;
            currMinions.minionObjects[m].Highlight();
        }

        if (heroPower.enabled)
        {
            if (heroPower.manaCost <= currMana)
            {
                heroPower.Highlight();
            }
        }

        //todo: WEAPON SWING
    }
    public void HighlightTargets()
    {

    }

}
