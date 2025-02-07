using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using System.Linq;

public partial class Board
{
    public static bool RNG(float percent) //Two point precision. RNG(XX.XXf)
    {
        percent *= 100;
        return (UnityEngine.Random.Range(0, 10000) <= percent);
    }
    public static T RandElem<T>(List<T> L)
    {
        return L[UnityEngine.Random.Range(0, L.Count)];
    }
    public static KeyValuePair<T, Q> RandElem<T, Q>(Dictionary<T, Q> L)
    {
        List<T> L2 = new List<T>(L.Keys);

        T K2 = RandElem(L2);
        Q V2 = L[K2];

        KeyValuePair<T, Q> kvp = new KeyValuePair<T, Q>(K2, V2);

        return kvp;
    }
    public static T RandElem<T>(T[] L)
    {
        return L[UnityEngine.Random.Range(0, L.Length)];
    }
    public static List<T> Shuffle<T>(List<T> l)
    {
        return l.OrderBy(x => UnityEngine.Random.value).ToList();
    }
    public static T[] Shuffle<T>(T[] l)
    {
        return l.OrderBy(x => UnityEngine.Random.value).ToArray();
    }

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

        DamagedMinion,
        HealthyMinion,
        Mech,
    }

    public void TargetMinion(Minion minion)
    {

        if (CheckTargetEligibility(minion) == false)
        {
            //invalid target todo:check these on server
            Debug.Log("Invalid target");
            return;
        }
        bool friendly = IsFriendly(minion);
        switch (targetMode)
        {
            case TargetMode.Battlecry:
                PlayCard(targetingCard, friendly? minion.previewIndex:minion.index, currMinions.previewMinion.index, IsFriendly(minion));
                break;
            case TargetMode.Attack:
                AttackMinion(targetingMinion, minion);
                break;
            case TargetMode.Spell:
                PlayCard(targetingCard, minion.index, -1, friendly);
                break;
            case TargetMode.HeroPower:
                CastHeroPower(targetingCard.card, minion.index, friendly,false);
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
        if (CheckTargetEligibility(hero) == false)
        {
            //invalid target todo:check these on server
            Debug.Log("Invalid target");
            return;
        }
        bool friendly = IsFriendly(hero);
        switch (targetMode)
        {
            case TargetMode.Battlecry:
                PlayCard(targetingCard, -1, currMinions.previewMinion.index, friendly,true);
                break;
            case TargetMode.Attack:
                AttackFace(targetingMinion, hero);
                break;
            case TargetMode.Spell:
                PlayCard(targetingCard, -1, -1, friendly, true);
                break;
            case TargetMode.HeroPower:
                CastHeroPower(targetingCard.card, -1, friendly, true);
                break;
            case TargetMode.Weapon:
                break;
            case TargetMode.None:
                EndTargeting();
                break;
        }

    }
    public Vector3 StartMinionPreview(Card card, int position)
    {
        Vector3 p = currMinions.SpawnPreviewMinion(card.card.card, position);
        playingCard = card;
        StartTargetingCard(card.card,currMinions.previewMinion);
        targetMode = TargetMode.Battlecry;
        return p;
    }
    public void StartTargetingAttack(Minion source)
    {
        targeting = true;
        targetMode = TargetMode.Attack;
        eligibleTargets = EligibleTargets.EnemyCharacters;
        targetingMinion = source;

        animationManager.LiftMinion(currMinions.minionObjects[source]);

        StartTargetingAnim(currMinions.minionObjects[source]);
    }

    public void StartTargetingCard(HandCard source, MonoBehaviour customPos=null)
    {
        targeting = true;
        targetMode = TargetMode.Spell;
        eligibleTargets = source.eligibleTargets;
        targetingCard = source;

        StartTargetingAnim(customPos!=null? customPos : currHero);
    }

    public void StartTargetingHeroPower(HandCard source)
    {
        targeting = true;
        targetMode = TargetMode.HeroPower;
        eligibleTargets = source.eligibleTargets;
        targetingCard = source;

        StartTargetingAnim(heroPower);
    }

    public void EndTargeting(bool cancel=false)
    {
        if (cancel)
        {
            if (playingCard != null)
            {
                playingCard.EndPlay();
            }
            if (targetingMinion != null)
            {
                if (currMinions.minionObjects.ContainsKey(targetingMinion)==true)
                    animationManager.CancelLiftMinion(currMinions.minionObjects[targetingMinion]);
            }
        }
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

    public bool ValidTargetsAvailable(EligibleTargets targets)
    {
        switch (targets)
        {
            case EligibleTargets.FriendlyMinions:
                return currMinions.Count() > 0;
            case EligibleTargets.EnemyMinions:
                return enemyMinions.Count() > 0;
            case EligibleTargets.AllMinions:
                return (currMinions.Count() + enemyMinions.Count()) > 0;
            default:
                return true;
        }
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
        CheckHighlights();
        StartCoroutine(_animActive(source.gameObject));
    }
    public void EndTargetingAnim()
    {
        HideSkulls();
        CheckHighlights();
        activeTargetingAnim = false;
    }

    public IEnumerator _animActive(GameObject pos)
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
                Arrow[i].transform.position = Vector3.Lerp(pos.transform.position, mPos, (i+1) / ((float)Arrow.Count));
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
                EndTargeting(true);
            }
            if (Card.GetMousePos().y < -7f)
            {
                EndTargeting(true);
            }
            yield return null;
        }

        foreach (GameObject g in Arrow) Destroy(g);
        activeTargetingAnim = false;

        yield return null;

    }


    public void ShowSkulls(Creature target)
    {
        if (targetMode == TargetMode.Attack)
        {
            if (targetingMinion.damage >= target.minion.health)
            {
                if (target.minion.HasAura(Aura.Type.Shield) == false)
                    target.ShowSkull();
            }
            if (target.minion.damage >= targetingMinion.health)
            {
                if (targetingMinion.HasAura(Aura.Type.Shield)==false)
                    targetingMinion.creature.ShowSkull();
            }
        }
        if (targetMode == TargetMode.Spell || targetMode == TargetMode.Battlecry || targetMode == TargetMode.HeroPower)
        {

        }
    }
    public void HideSkulls()
    {
        foreach (Creature c in currMinions.minionObjects.Values)
            c.HideSkull();
        foreach (Creature c in enemyMinions.minionObjects.Values)
            c.HideSkull();
    }
}
