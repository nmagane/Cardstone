
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
        foreach (var c in currHand.cardObjects)
        {
            c.Value.Unhighlight();
        }

        foreach (var m in currMinions.minionObjects)
        {
            m.Value.Unhighlight();
        }
        foreach (var m in enemyMinions.minionObjects)
        {
            m.Value.Unhighlight();
        }

        heroPower.Unhighlight();
        currHero.Unhighlight();
        enemyHero.Unhighlight();
    }


    public void HighlightActions()
    {
        foreach (var c in currHand.cardObjects)
        {
            if (c.Value.card.manaCost > currMana) continue;
            if (c.Value.card.SPELL && c.Value.card.TARGETED && ValidTargetsAvailable(c.Value.card.eligibleTargets) == false) continue;
            if (c.Value.card.MINION && currMinions.Count() >= 7) continue;
            if (c.Value.card.played) continue;
            c.Value.Highlight();
        }

        foreach (var m in currMinions.minionObjects)
        {
            if (m.Value.minion.canAttack == false) continue;
            if (m.Value.minion.DEAD == true) continue;
            m.Value.Highlight();
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
        switch (targetMode)
        {
            case TargetMode.Attack:
            case TargetMode.Weapon:
                HighlightAttack();
                break;

            case TargetMode.Battlecry:
            case TargetMode.Spell:
            case TargetMode.HeroPower:
                HighlightSpell();
                break;
        }
    }
    void HighlightAttack()
    {
        bool taunter = false;
        foreach (Minion m in enemyMinions)
        {
            if (m.HasAura(Aura.Type.Taunt)) taunter = true;
        }
        if (taunter)
        {
            foreach (Minion m in enemyMinions)
            {
                if (m.HasAura(Aura.Type.Stealth)) continue;

                if (m.HasAura(Aura.Type.Taunt))
                {
                    m.creature.Highlight(true);

                    //bounce taunters (experimental)
                    animationManager.LerpZoom(enemyMinions.minionObjects[m].gameObject, enemyMinions.minionObjects[m].transform.localScale, 5, 0.2f);
                }
            }
        }
        else
        {
            enemyHero.Highlight(true);
            HighlightEnemyMinions();
        }
    }
    void HighlightSpell()
    {
        switch (eligibleTargets)
        {
            case EligibleTargets.AllCharacters:
                HighlightEnemyHero();
                HighlightEnemyMinions();
                HighlightFriendlyHero();
                HighlightFriendlyMinions();
                break;
            case EligibleTargets.EnemyCharacters:
                HighlightEnemyHero();
                HighlightEnemyMinions();
                break;
            case EligibleTargets.FriendlyCharacters:
                HighlightFriendlyHero();
                HighlightFriendlyMinions();
                break;

            case EligibleTargets.EnemyMinions:
                HighlightEnemyMinions();
                break;
            case EligibleTargets.FriendlyMinions:
                HighlightFriendlyMinions();
                break;
            case EligibleTargets.AllMinions:
                HighlightFriendlyMinions();
                HighlightEnemyMinions();
                break;

            case EligibleTargets.EnemyHero:
                HighlightEnemyHero();
                break;
            case EligibleTargets.FriendlyHero:
                HighlightFriendlyHero();
                break;
            case EligibleTargets.AllHeroes:
                HighlightEnemyHero();
                HighlightFriendlyHero();
                break;
            default:
                Debug.LogError("NO HIGHLIGHT IMPLEMENTED: " + eligibleTargets);
                break;
        }
    }

    void HighlightEnemyHero()
    {
        enemyHero.Highlight(true);
    }
    void HighlightEnemyMinions()
    {
        foreach (Minion m in enemyMinions)
        {
            if (m.HasAura(Aura.Type.Stealth)) continue;
            m.creature.Highlight(true);
        }
    }
    void HighlightFriendlyHero()
    {
        currHero.Highlight(true);
    }
    void HighlightFriendlyMinions()
    {
        foreach (var m in currMinions)
        {
            m.creature.Highlight(true);
        }
    }
}
