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
            
        
        return g;
    }
    [System.Serializable]
    public class AnimationInfo
    {
        public Card.Cardname card;

        public bool sourceIsHero;
        public bool sourceIsFriendly;
        public int sourceIndex;

        public int targetIndex;
        public bool targetIsFriendly;
        public bool targetIsHero;

        public void SetUntargeted()
        {
            sourceIsHero = true;
            sourceIsFriendly = true;

            targetIsHero = true;
            targetIsFriendly = false;
        }
    }

    public class AnimationData
    {
        public Card.Cardname card;

        public bool sourceIsHero;
        public Minion sourceMinion = null;
        public Hero sourceHero = null;

        public bool targetIsHero;
        public Minion targetMinion = null;
        public Hero targetHero = null;
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
        p.transform.parent = board.gameAnchor.transform;
        p.transform.localScale = Vector3.one * 1.25f;

        Creature source = data.sourceMinion.creature;

        MonoBehaviour target;
        if (data.targetIsHero) target = data.targetHero;
        else target = data.targetMinion.creature;

        p.transform.localPosition = source.transform.localPosition;
        Vector3 targetPos = target.transform.localPosition;

        PointTo(p, targetPos,90);
        yield return LerpTo(p,targetPos,10);
        Destroy(p.gameObject);

    }
    IEnumerator Flame_ImpAnim(AnimationData data)
    {
        GameObject p = CreateEffect(Effect.fireballSmall);
        p.transform.parent = board.gameAnchor.transform;

        Creature source = data.sourceMinion.creature;

        MonoBehaviour target;
        if (data.targetIsHero) target = data.targetHero;
        else target = data.targetMinion.creature;

        p.transform.localPosition = source.transform.localPosition;
        Vector3 targetPos = target.transform.localPosition;


        p.transform.localScale = Vector3.zero;
        Spin(p, 0.5f);
        yield return LerpZoom(p, Vector3.one * 1.25f, 12);
        yield return LerpTo(p,targetPos,5);
        Destroy(p.gameObject);

    }
    IEnumerator SoulfireAnim(AnimationData data)
    {
        GameObject p = CreateEffect(Effect.fireballBig);
        p.transform.parent = board.gameAnchor.transform;

        Hero source = data.sourceHero;

        MonoBehaviour target;
        if (data.targetIsHero) target = data.targetHero;
        else target = data.targetMinion.creature;

        p.transform.localPosition = source.transform.localPosition;
        Vector3 targetPos = target.transform.localPosition;

        p.transform.localScale = Vector3.zero;
        Spin(p, 0.5f);
        yield return LerpZoom(p, Vector3.one * 1.25f, 12);
        yield return LerpTo(p,targetPos,12);
        Destroy(p.gameObject);

    }

    IEnumerator Lifetap(AnimationData data)
    {
        GameObject p = CreateEffect(Effect.lifetap);
        p.transform.parent = board.gameAnchor.transform;
        Vector3 pos = data.targetHero == board.currHero ? board.currHero.transform.position : board.enemyHero.transform.position;
        p.transform.position = pos;
        p.transform.localScale = Vector3.zero;
        yield return LerpZoom(p, Vector3.one * 1.25f, 10);
        StartCoroutine(_fadeout(p,15));
    }
}
