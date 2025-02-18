using System.Collections;
using UnityEngine;

public partial class AnimationManager
{
    enum Effect
    {
        dagger,
        fireballSmall,
        fireballBig,

        lifetap,

        greenSmall,
        greenBig,

        frostSmall,
        frostBig,

        whirlwind,
    }
    public Sprite[] effectSprites;
    GameObject CreateEffect(Effect e)
    {
        GameObject g = Instantiate(board.UISprite);
        
        g.GetComponent<SpriteRenderer>().sortingLayerName = "creatureElevated";
        g.GetComponent<SpriteRenderer>().sortingOrder = 10;
        g.GetComponent<SpriteRenderer>().sprite = effectSprites[(int)e];

        g.transform.parent = board.currHero.transform.parent;
        g.transform.localScale = Vector3.one;

        return g;
    }

    public class AnimationData
    {
        public Card.Cardname card;
        public bool friendly = true;

        public bool sourceIsHero;
        public Minion sourceMinion = null;
        public Hero sourceHero = null;

        public bool targetIsHero;
        public Minion targetMinion = null;
        public Hero targetHero = null;
        public Vector3 sourcePos => GetSourcePos();
        public Vector3 targetPos => GetTargetPos();
        public MonoBehaviour GetTarget()
        {
            if (targetIsHero) return targetHero;
            else
            {
                if (targetMinion.creature == null) Debug.LogError("NO CREATURE FOUND FOR ANIM TARGET");
                return targetMinion.creature;
            }
        }
        public MonoBehaviour GetSource()
        {
            if (sourceIsHero) return sourceHero;
            else
            {
                if (sourceMinion.creature == null) Debug.LogError("NO CREATURE FOUND FOR ANIM SOURCE");
                return sourceMinion.creature;
            }
        }

        public Vector3 GetTargetPos()
        {
            MonoBehaviour obj = GetTarget();
            if (obj == null)
            {
                if (sourceIsHero == false)
                {
                    if (Board.Instance.currMinions.Contains(sourceMinion))
                    {
                        return Board.Instance.currHero.transform.localPosition;
                    }
                    else
                        return Board.Instance.enemyHero.transform.localPosition;
                }
            }
            return obj.transform.localPosition;
        }
        public Vector3 GetSourcePos()
        {
            MonoBehaviour obj = GetSource();
            if (obj == null)
            {
                if (sourceIsHero == false)
                {
                    if (Board.Instance.currMinions.Contains(sourceMinion))
                    {
                        return Board.Instance.currHero.transform.localPosition;
                    }
                    else
                        return Board.Instance.enemyHero.transform.localPosition;
                }
            }
            return obj.transform.localPosition;
        }
    }
    public Coroutine StartAnimation(AnimationData data)
    {
        switch (data.card)
        {
            case Card.Cardname.Knife_Juggler:
                return StartCoroutine(KnifeJugglerAnim(data));

            case Card.Cardname.Flame_Imp:
            case Card.Cardname.Ping:
                return StartCoroutine(Simple_Projectile(data,Effect.fireballSmall,12,5));

            case Card.Cardname.Soulfire:
            case Card.Cardname.Fireball:
                return StartCoroutine(SoulfireAnim(data));

            case Card.Cardname.Mortal_Coil:
                return StartCoroutine(Simple_Projectile(data, Effect.greenSmall, 12, 10));
            case Card.Cardname.Implosion:
                return StartCoroutine(Simple_Projectile(data, Effect.greenBig, 12, 12));

            case Card.Cardname.Ice_Lance:
                return StartCoroutine(Simple_Projectile(data, Effect.frostSmall, 12, 10));
            case Card.Cardname.Frostbolt:
                return StartCoroutine(Simple_Projectile(data, Effect.frostBig, 12, 12));

            case Card.Cardname.Lifetap:
                return StartCoroutine(Lifetap(data));

            case Card.Cardname.Whirlwind:
                return StartCoroutine(WhirlwindAnim(data));

            default:
                Debug.LogWarning("Animation Unimplemented? " + data.card);
                return null;
        }

    }

    IEnumerator KnifeJugglerAnim(AnimationData data)
    {
        GameObject p = CreateEffect(Effect.dagger);

        p.transform.localPosition = data.GetSourcePos();
        Vector3 targetPos = data.GetTargetPos();

        PointTo(p, targetPos,90);
        yield return LerpTo(p,targetPos,10);
        Destroy(p.gameObject);

    }
    IEnumerator Simple_Projectile(AnimationData data,Effect projectile, int zoomFrames=12, int travelFrames=12)
    {
        GameObject p = CreateEffect(projectile);


        p.transform.localPosition = data.sourcePos;
        Vector3 targetPos = data.targetPos;

        p.transform.localScale = Vector3.zero;
        Spin(p, 0.5f);
        yield return LerpZoom(p, Vector3.one, zoomFrames);
        yield return LerpTo(p,targetPos, travelFrames);
        Destroy(p.gameObject);

    }
    IEnumerator SoulfireAnim(AnimationData data)
    {
        GameObject p = CreateEffect(Effect.fireballBig);

        p.transform.localPosition = data.sourcePos;
        Vector3 targetPos = data.targetPos;

        p.transform.localScale = Vector3.zero;
        Spin(p, 0.5f);
        yield return LerpZoom(p, Vector3.one, 12);
        yield return LerpTo(p,targetPos,12);
        Destroy(p.gameObject);
    }

    IEnumerator Lifetap(AnimationData data)
    {
        GameObject p = CreateEffect(Effect.lifetap);
        p.transform.localPosition = data.sourcePos;
        p.transform.localScale = Vector3.zero;
        yield return LerpZoom(p, Vector3.one, 10);
        StartCoroutine(_fadeout(p,15));
    }
    IEnumerator WhirlwindAnim(AnimationData data)
    {
        GameObject p = CreateEffect(Effect.whirlwind);
        Spin(p, 20f);
        p.transform.localPosition = new Vector3(10,0);
        LerpTo(p, new Vector3(-10,0), 30);
        StartCoroutine(_fadeout(p,30));

        yield return Wait(10);
    }
}
