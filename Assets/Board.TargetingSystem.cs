using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Board
{
    public bool targeting = false;
    public TargetMode targetMode = TargetMode.None;
    public EligibleTargets eligibleTargets = EligibleTargets.AllCharacters;
    //public int targetSourceIndex = 0;
    //public int targetIndex = 0;
    public Minion targetingMinion = null;
    public Hero targetingHero = null;
    public HandCard targetingCard = null;

    public Card playingCard = null;

    public bool dragTargeting = false;

    public enum TargetMode
    {
        None,
        Attack,
        Battlecry,
        Spell,
        HeroPower,
        Weapon,
    }
    public enum EligibleTargets
    {
        AllCharacters,
        AllMinions,
        AllHeroes,

        EnemyCharacters,
        EnemyMinions,

        FriendlyCharacters,
        FriendlyMinions,

        FriendlyHero,
        EnemyHero,
    }

    public void TargetMinion(Minion minion)
    {
        switch (targetMode)
        {
            case TargetMode.Attack:
                AttackMinion(targetingMinion, minion);
                break;
            case TargetMode.Spell:
                break;
            case TargetMode.HeroPower:
                break;
            case TargetMode.Weapon:
                break;
            case TargetMode.None:
                EndTargeting();
                break;
        }
    }
    public void TargetHero(Hero hero)
    {
        switch (targetMode)
        {
            case TargetMode.Attack:
                AttackFace(targetingMinion, hero);
                break;
            case TargetMode.Spell:
                break;
            case TargetMode.HeroPower:
                break;
            case TargetMode.Weapon:
                break;
            case TargetMode.None:
                EndTargeting();
                break;
        }
    }

    public void StartTargetingAttack(Minion source)
    {
        targeting = true;
        targetMode = TargetMode.Attack;
        eligibleTargets = EligibleTargets.EnemyCharacters;
        targetingMinion = source;

        StartTargetingAnim(currMinions.minionObjects[source]);
    }
    public void EndTargeting()
    {
        targeting = false;
        targetMode = TargetMode.None;
        eligibleTargets = EligibleTargets.AllCharacters;
        //targetSourceIndex = 0;
        //targetIndex = 0;
        targetingMinion = null;
        targetingCard = null;
        dragTargeting = false;

        EndTargetingAnim();
    }

    public bool CheckTargetEligibility(Minion m)
    {
        if (eligibleTargets == EligibleTargets.AllCharacters || eligibleTargets==EligibleTargets.AllMinions)
        {
            return true;
        }
        if (eligibleTargets == EligibleTargets.EnemyMinions || eligibleTargets == EligibleTargets.EnemyCharacters)
        {
            if (IsFriendly(m)) return false;
            else return true;
        }
        if (eligibleTargets == EligibleTargets.FriendlyMinions || eligibleTargets == EligibleTargets.FriendlyCharacters)
        {
            if (IsFriendly(m)) return true;
            else return false;
        }
        if (eligibleTargets == EligibleTargets.AllHeroes || eligibleTargets == EligibleTargets.FriendlyHero || eligibleTargets == EligibleTargets.EnemyHero)
        {
            return false;
        }
        return false;

    }
    public bool CheckTargetEligibility(Hero h)
    {
        if (eligibleTargets == EligibleTargets.AllCharacters)
        {
            return true;
        }
        if (eligibleTargets == EligibleTargets.EnemyCharacters)
        {
            if (IsFriendly(h)) return false;
            else return true;
        }
        if (eligibleTargets == EligibleTargets.FriendlyCharacters)
        {
            if (IsFriendly(h)) return true;
            else return false;
        }
        if (eligibleTargets == EligibleTargets.AllHeroes)
            return true;
        if (eligibleTargets == EligibleTargets.FriendlyHero)
            return IsFriendly(h);
        if (eligibleTargets == EligibleTargets.EnemyHero)
            return !IsFriendly(h);
        if (eligibleTargets == EligibleTargets.FriendlyMinions || eligibleTargets == EligibleTargets.EnemyMinions || eligibleTargets == EligibleTargets.AllMinions)
            return false;
        return false;
    }

    public void StartPlayingCard(Card c)
    {
        playingCard = c;
    }
    public void EndPlayingCard()
    {
        playingCard = null;
        dragTargeting = false;
    }

    public bool activeTargetingAnim = false;
    public Sprite[] ArrowSprites;

    public void StartTargetingAnim(MonoBehaviour source)
    {
        StartCoroutine(_animActive(source.transform.position));
    }
    public void EndTargetingAnim()
    {
        activeTargetingAnim = false;
    }

    public IEnumerator _animActive(Vector3 pos)
    {
        if (activeTargetingAnim) yield break;
        activeTargetingAnim = true;
        
        int DOT_COUNT = 5;
        List<GameObject> Arrow = new List<GameObject>();
        for (int i = 0; i < DOT_COUNT + 1; i++)
        {
            Arrow.Add(Instantiate(UISprite));
            Arrow[i].GetComponent<SpriteRenderer>().sprite = i == DOT_COUNT ? ArrowSprites[0] : ArrowSprites[1];
            if (i==DOT_COUNT) Arrow[i].GetComponent<SpriteRenderer>().sortingOrder = 1;
            Arrow[i].GetComponent<SpriteRenderer>().sortingLayerName = "ui0";
        }

        while (activeTargetingAnim)
        {
            Vector3 mPos = Card.GetMousePos();
            for (int i = 0; i < Arrow.Count; i++)
            {
                Arrow[i].transform.position = Vector3.Lerp(pos, mPos, (i+1) / ((float)Arrow.Count));
                if (i == Arrow.Count - 1)
                {
                    float x1 = Arrow[i - 1].transform.position.x; float x2 = Arrow[i - 2].transform.position.x;
                    float y1 = Arrow[i - 1].transform.position.y; float y2 = Arrow[i - 2].transform.position.y;
                    float angle = Mathf.Atan((y2 - y1) / (x2 - x1)) * 180 / Mathf.PI;
                    if (x2 < x1)
                    {
                        Arrow[i].GetComponent<SpriteRenderer>().flipX = true;
                        Arrow[i].GetComponent<SpriteRenderer>().flipY = true;
                    }
                    else
                    {
                        Arrow[i].GetComponent<SpriteRenderer>().flipX = false;
                        Arrow[i].GetComponent<SpriteRenderer>().flipY = false;
                    }
                    Arrow[i].transform.localEulerAngles = new Vector3(0, 0, angle);
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                EndTargeting();
            }
            if (Card.GetMousePos().y < -6)
            {
                EndTargeting();
            }
            yield return null;
        }

        foreach (GameObject g in Arrow) Destroy(g);
        activeTargetingAnim = false;

        yield return null;

    }
}
