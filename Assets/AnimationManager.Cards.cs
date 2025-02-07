using System.Collections;
using UnityEngine;

public partial class AnimationManager
{
    public GameObject projectileObject;
    GameObject CreateProjectile()
    {
        GameObject g = Instantiate(projectileObject);
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

            default:
                Debug.LogWarning("Animatin Unimplemented? " + data.card);
                return null;
        }

    }

    IEnumerator KnifeJugglerAnim(AnimationData data)
    {
        GameObject p = CreateProjectile();
        p.transform.parent = board.gameAnchor.transform;

        Creature source = data.sourceMinion.creature;

        MonoBehaviour target;
        if (data.targetIsHero) target = data.targetHero;
        else target = data.targetMinion.creature;

        p.transform.localPosition = source.transform.localPosition;
        Vector3 targetPos = target.transform.localPosition;

        yield return LerpTo(p,targetPos,10);
        Destroy(p.gameObject);

        yield return Wait(5);
    }
}
