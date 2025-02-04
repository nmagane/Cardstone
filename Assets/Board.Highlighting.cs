
using UnityEngine;
public partial class Board
{
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
    }
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

    public void HighlightActions()
    {
        foreach (var c in currHand)
        {
            if (c.manaCost > currMana) continue;
            if (c.SPELL && c.TARGETED && ValidTargetsAvailable(c.eligibleTargets) == false) continue;
            currHand.cardObjects[c].Highlight();
        }
        foreach (var m in currMinions)
        {
            if (m.canAttack == false) continue;
            currMinions.minionObjects[m].Highlight();
        }
        //todo: HERO POWER
        //todo: WEAPON SWING
    }
    public void HighlightTargets()
    {

    }

}
