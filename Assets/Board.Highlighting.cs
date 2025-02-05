
using UnityEngine;
public partial class Board
{
    public Tooltip enemyPlayTip;
    public Tooltip hoverTip;
    public void ShowEnemyPlay(Card.Cardname c)
    {
        enemyPlayTip.Set(c,90);
    }

    public void ShowHoverTip(Creature c)
    {
        if (c.transform.localPosition.x<0)
        {
            hoverTip.transform.position = c.transform.position + new Vector3(6, 0);
        }
        else
            hoverTip.transform.position = c.transform.position + new Vector3(-6, 0);

        hoverTip.transform.localScale = Vector3.one * 0.3f;
        animationManager.LerpZoom(hoverTip.gameObject,Vector3.one, 5,0.1f);
        hoverTip.Set(c.minion.card);
        
    }
    public void ShowHoverTip(HeroPower p)
    {
        if (p.transform.position.y<0)
        {
            hoverTip.transform.position = p.transform.position + new Vector3(6, 6);
        }
        else
            hoverTip.transform.position = p.transform.position + new Vector3(6, -6);

        hoverTip.transform.localScale = Vector3.one * 0.3f;
        animationManager.LerpZoom(hoverTip.gameObject,Vector3.one, 5,0.1f);
        hoverTip.Set(p.card.card);
    }
    public void HideHoverTip()
    {
        hoverTip.Hide();
        hoverTip.transform.localPosition = new Vector3(0, 20);
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
