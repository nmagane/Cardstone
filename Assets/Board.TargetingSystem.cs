using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using System.Linq;
using System.Net;

public partial class Board
{
    public static Color GetColor(string hex)
    {
        if (hex.StartsWith("#"))
        {
            hex = hex.Substring(1);
        }
        if (hex.Length != 6)
        {
            return Color.white;
        }
        float r = int.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber) / 255f;
        float g = int.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber) / 255f;
        float b = int.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber) / 255f;

        return new Color(r, g, b);
    }
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
    public int playingChoice = -1;

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

        Weapon,

        DamagedMinions,
        HealthyMinions,
        MechMinions,
        Big_Game_Hunter,

        None,
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
        if (!friendly && minion.STEALTH)
        {
            Debug.Log("Invalid target");
            return;
        }
        switch (targetMode)
        {
            case TargetMode.Battlecry:
                PlayCard(targetingCard, friendly? minion.index : minion.index, currMinions.previewMinion.index, IsFriendly(minion),false,playingChoice);
                break;
            case TargetMode.Attack:
                AttackMinion(targetingMinion, minion);
                break;
            case TargetMode.Spell:
                PlayCard(targetingCard, minion.index, -1, friendly,false,playingChoice);
                break;
            case TargetMode.HeroPower:
                CastHeroPower(targetingCard.card, minion.index, friendly,false);
                break;
            case TargetMode.Weapon:
                SwingMinion(targetingHero,friendly, minion);
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
        if (hero.IMMUNE)
        {
            //immune target
            Debug.Log("Invalid target");
            return;
        }
        bool friendly = IsFriendly(hero);
        switch (targetMode)
        {
            case TargetMode.Battlecry:
                PlayCard(targetingCard, -1, currMinions.previewMinion.index, friendly,true,playingChoice);
                break;
            case TargetMode.Attack:
                AttackFace(targetingMinion, hero);
                break;
            case TargetMode.Spell:
                PlayCard(targetingCard, -1, -1, friendly, true,playingChoice);
                break;
            case TargetMode.HeroPower:
                CastHeroPower(targetingCard.card, -1, friendly, true);
                break;
            case TargetMode.Weapon:
                SwingFace(targetingHero,hero);
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
    public void StartTargetingSwing(Hero source)
    {
        targeting = true;
        targetMode = TargetMode.Weapon;
        eligibleTargets = EligibleTargets.EnemyCharacters;
        targetingHero = source;

        animationManager.LiftHero(source);

        StartTargetingAnim(source);
    }

    public void StartTargetingCard(HandCard source, MonoBehaviour customPos=null, int choice=-1, EligibleTargets targetOverride=EligibleTargets.None)
    {
        targeting = true;
        targetMode = TargetMode.Spell;
        eligibleTargets = source.eligibleTargets;
        targetingCard = source;
        playingChoice = choice;

        if (targetOverride != EligibleTargets.None)
        {
            eligibleTargets = targetOverride;
        }

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
            if (targetingHero != null)
            {
                animationManager.CancelLiftHero(targetingHero);
            }
            foreach (var x in currHand.cardObjects.Values)
            {
                x.alpha = 1;
            }
        }
        targeting = false;
        targetMode = TargetMode.None;
        eligibleTargets = EligibleTargets.AllCharacters;
        
        targetingMinion = null;
        targetingCard = null;
        targetingHero = null;
        dragTargeting = false;
        playingChoice = -1;

        if (cancel) CheckHighlights();
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
        if (eligibleTargets == EligibleTargets.HealthyMinions)
        {
            if (m.health == m.maxHealth) return true;
            else return false;
        }
        if (eligibleTargets == EligibleTargets.DamagedMinions)
        {
            if (m.health < m.maxHealth) return true;
            else return false;
        }

        if (eligibleTargets == EligibleTargets.Big_Game_Hunter)
        {
            if (m.damage >= 7) return true;
            else return false;
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
        if (eligibleTargets.ToString().Contains("Minions") || eligibleTargets == EligibleTargets.Big_Game_Hunter)
            return false;
        return false;
    }

    public bool ValidTargetsAvailable(EligibleTargets targets)
    {
        switch (targets)
        {
            case EligibleTargets.Weapon:
                return (currHero.weapon != null);
            case EligibleTargets.FriendlyMinions:
                return currMinions.Count() > 0;
            case EligibleTargets.EnemyMinions:
                return enemyMinions.Count() > 0;
            case EligibleTargets.AllMinions:
                return (currMinions.Count() + enemyMinions.Count()) > 0;
            case EligibleTargets.HealthyMinions:
                foreach(Minion m in currMinions)
                {
                    if (m.health == m.maxHealth) return true;
                }
                foreach(Minion m in enemyMinions)
                {
                    if (m.health == m.maxHealth) return true;
                }
                return false;
            case EligibleTargets.DamagedMinions:
                foreach(Minion m in currMinions)
                {
                    if (m.health < m.maxHealth) return true;
                }
                foreach(Minion m in enemyMinions)
                {
                    if (m.health < m.maxHealth) return true;
                }
                return false;
            case EligibleTargets.Big_Game_Hunter:
                foreach(Minion m in currMinions)
                {
                    if (m.damage >= 7) return true;
                }
                foreach(Minion m in enemyMinions)
                {
                    if (m.damage >= 7) return true;
                }
                return false;
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
        foreach (SecretDisplay s in enemyHero.secrets)
        {
            s.GetComponent<BoxCollider2D>().enabled = false;
        }

        CheckHighlights();
        StartCoroutine(_animActive(source.gameObject));
    }
    public void EndTargetingAnim()
    {
        HideSkulls();
        foreach (SecretDisplay s in enemyHero.secrets)
        {
            s.GetComponent<BoxCollider2D>().enabled = true;
        }

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
                Arrow[i].transform.position = Vector3.Lerp(pos.transform.position, mPos, (i+0.7f) / ((float)Arrow.Count));
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
                    //Arrow[i].transform.position = Vector3.Lerp(pos.transform.position, mPos, (i + 1) / ((float)Arrow.Count)
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


    //TODO: factor in spell power for skull
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
        if (targetMode == TargetMode.Weapon)
        {
            if (targetingHero.damage >= target.minion.health)
            {
                if (target.minion.HasAura(Aura.Type.Shield) == false)
                    target.ShowSkull();
            }
            
            if (target.minion.damage >= targetingHero.health + targetingHero.armor)
            {
                //if (targetingMinion.HasAura(Aura.Type.Immune)==false)
                targetingHero.ShowSkull();
            }
        }

        if (targetMode == TargetMode.Spell || targetMode == TargetMode.Battlecry || targetMode == TargetMode.HeroPower)
        {
            Database.CardInfo c = Database.GetCardData(targetingCard.card);

            int d = playingChoice==1? c.comboSpellDamage : c.spellDamage;
            if (targetingCard.card == Card.Cardname.Ice_Lance && !target.minion.HasAura(Aura.Type.Freeze))
            {
                return;
            }
            if (c.COMBO && c.comboSpellDamage > 0 && currHero.combo) d = c.comboSpellDamage;
            if (c.SPELL && targetingCard.card != heroPower.card.card && c.spellDamage>0) d += currHero.spellpower;
            if (d >= target.minion.health)
            {
                if (target.minion.HasAura(Aura.Type.Shield) == false)
                    target.ShowSkull();
            }
        }
    }
    public void ShowSkulls(Hero target)
    {
        if (targetMode == TargetMode.Attack)
        {
            if (targetingMinion.damage >= target.health + target.armor)
            {
                  target.ShowSkull();
            }
        }
        if (targetMode == TargetMode.Weapon)
        {
            if (targetingHero.damage >= target.health + target.armor)
            {
                  target.ShowSkull();
            }
        }
        if (targetMode == TargetMode.Spell || targetMode == TargetMode.Battlecry || targetMode == TargetMode.HeroPower)
        {
            if (targetingCard.card == Card.Cardname.Ice_Lance && !target.FREEZE)
            {
                return;
            }
            Database.CardInfo c = Database.GetCardData(targetingCard.card);
            int d = c.spellDamage;
            if (c.COMBO && c.comboSpellDamage > 0 && currHero.combo) d = c.comboSpellDamage;
            if (c.SPELL && targetingCard.card != heroPower.card.card && c.spellDamage > 0) d += currHero.spellpower;

            if (d >= target.health + target.armor)
            {
                 target.ShowSkull();
            }
        }
    }
    public void HideSkulls()
    {
        foreach (Creature c in currMinions.minionObjects.Values)
            c.HideSkull();
        foreach (Creature c in enemyMinions.minionObjects.Values)
            c.HideSkull();
        currHero.HideSkull();
        enemyHero.HideSkull();
    }
}
