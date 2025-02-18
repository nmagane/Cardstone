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
    }
    public GameObject[] effectObjects;
    GameObject CreateEffect(Effect e)
    {
        GameObject g = Instantiate(effectObjects[(int)e]);

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
                return StartCoroutine(Flame_ImpAnim(data));
            case Card.Cardname.Soulfire:
            case Card.Cardname.Fireball:
                return StartCoroutine(SoulfireAnim(data));
            case Card.Cardname.Lifetap:
                return StartCoroutine(Lifetap(data));

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
    IEnumerator Flame_ImpAnim(AnimationData data)
    {
        GameObject p = CreateEffect(Effect.fireballSmall);


        p.transform.localPosition = data.sourcePos;
        Vector3 targetPos = data.targetPos;

        p.transform.localScale = Vector3.zero;
        Spin(p, 0.5f);
        yield return LerpZoom(p, Vector3.one, 12);
        yield return LerpTo(p,targetPos,5);
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
}
