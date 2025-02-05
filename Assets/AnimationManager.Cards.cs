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

    }
    public Coroutine StartAnimation(AnimationInfo c, bool isFriendly)
    {
        if (isFriendly == false)
        {
            c.sourceIsFriendly = !c.sourceIsFriendly;
            c.targetIsFriendly = !c.targetIsFriendly;
        }

        switch (c.card)
        {
            case Card.Cardname.Knife_Juggler:
                return StartCoroutine(KnifeJugglerAnim(c));
        }

        return null;
    }

    IEnumerator KnifeJugglerAnim(AnimationInfo anim)
    {
        GameObject p = CreateProjectile();
        p.transform.parent = board.gameAnchor.transform;

        Creature source = anim.sourceIsFriendly ? board.currMinions[anim.sourceIndex].creature : board.enemyMinions[anim.sourceIndex].creature;

        MonoBehaviour target;
        if (anim.targetIsHero) target = anim.targetIsFriendly ? board.currHero: board.enemyHero ;
        else target = anim.targetIsFriendly? board.currMinions[anim.targetIndex].creature : board.enemyMinions[anim.targetIndex].creature;

        p.transform.localPosition = source.transform.localPosition;
        Vector3 targetPos = target.transform.localPosition;

        yield return LerpTo(p,targetPos,10);

        Destroy(p.gameObject);
    }
}
