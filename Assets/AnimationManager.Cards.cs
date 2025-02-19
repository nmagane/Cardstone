using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class AnimationManager
{
    public enum Effect
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

        brownSmall,
        brownBig,

        pyroblast,

        smoke0,
        smoke1,
        smoke2,
        smoke3,

        slash0,

        boardFire,
        boardFrost,
        
        rage,
        flurry,

        boom,
        boomFrost,

        particle1,
        particle2,
        particleCross,

        potionGreen,
        potionBlue,
        potionLight,
        bomb,
    }
    public Sprite[] effectSprites;
    GameObject CreateEffect(Effect e)
    {
        GameObject g = Instantiate(board.UISprite);
        
        g.GetComponent<SpriteRenderer>().sortingLayerName = "creatureElevated";
        g.GetComponent<SpriteRenderer>().sortingOrder = 500;
        g.GetComponent<SpriteRenderer>().sprite = effectSprites[(int)e];

        GameObject x = new GameObject(e.ToString());

        x.transform.parent = board.gameAnchor.transform;
        g.transform.parent = x.transform;
        g.transform.localScale = Vector3.one * 1.25f;
        x.transform.localScale = Vector3.one;

        return x;
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
        public Vector3 originalSourcePos;
        public Vector3 originalTargetPos;
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
            return obj.transform.localPosition * obj.transform.parent.transform.localScale.x;
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
            return obj.transform.localPosition * obj.transform.parent.transform.localScale.x;
        }
    }
    public Coroutine StartAnimation(AnimationData data)
    {
        switch (data.card)
        {
            case Card.Cardname.Knife_Juggler:
            case Card.Cardname.Cruel_Taskmaster:
                return StartCoroutine(KnifeJugglerAnim(data));

            case Card.Cardname.Flame_Imp:
                return StartCoroutine(Simple_Projectile(data,Effect.fireballSmall,12,5));

            case Card.Cardname.Ping:
                return StartCoroutine(Ping(data));

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

            case Card.Cardname.Slam:
                return StartCoroutine(Simple_Projectile(data, Effect.brownBig, 12, 12));
            case Card.Cardname.Execute:
                return StartCoroutine(Simple_Projectile(data, Effect.brownSmall, 12, 12));

            case Card.Cardname.Pyroblast:
                return StartCoroutine(Simple_Projectile(data, Effect.pyroblast, 20, 20));

            case Card.Cardname.Lifetap:
                return StartCoroutine(Lifetap(data));

            case Card.Cardname.Whirlwind:
            case Card.Cardname.Deaths_Bite:
                return StartCoroutine(WhirlwindEffect(data,Effect.whirlwind));
            case Card.Cardname.Blade_Flurry:
                return StartCoroutine(WhirlwindEffect(data,Effect.flurry));

            case Card.Cardname.Backstab:
            case Card.Cardname.SI7_Agent:
            case Card.Cardname.Eviscerate:
                return StartCoroutine(SmokeTrailProjectile(data));

            case Card.Cardname.Inner_Rage:
                return StartCoroutine(RageEffect(data));

            case Card.Cardname.Unstable_Ghoul:
                return StartCoroutine(Boom(data,Effect.boom));

            case Card.Cardname.Frost_Nova:
                return StartCoroutine(Boom(data,Effect.boomFrost));
            case Card.Cardname.Flamestrike:
                return StartCoroutine(AoEEffect(data,Effect.boardFire));
            case Card.Cardname.Blizzard:
                return StartCoroutine(AoEEffect(data,Effect.boardFrost));

            case Card.Cardname.Deadly_Poison:
                return StartCoroutine(PotionProjectileWeapon(data,Effect.potionGreen));

            case Card.Cardname.Tinkers_Oil:
                return StartCoroutine(Tinkers_Oil(data));

            case Card.Cardname.Crazed_Alchemist:
                return StartCoroutine(PotionProjectile(data,Effect.potionBlue));

            case Card.Cardname.Boom_Bot:
                return StartCoroutine(PotionProjectile(data,Effect.bomb,20));

            case Card.Cardname.Fan_of_Knives:
                return StartCoroutine(FanOfKnives(data));

            case Card.Cardname.Alexstrasza:
                return StartCoroutine(AlexEffect(data));
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
    IEnumerator Ping(AnimationData data)
    {
        GameObject p = CreateEffect(Effect.fireballSmall);
        p.transform.position = data.GetSource() == board.currHero? board.heroPower.transform.position: board.enemyHeroPower.transform.position;

        p.transform.localScale = Vector3.zero;
        Spin(p, 0.5f);
        yield return LerpZoom(p, Vector3.one, 12);
        yield return LerpTo(p, data.GetTargetPos(), 10);
        Destroy(p.gameObject);
    }

    IEnumerator SmokeTrailProjectile(AnimationData data)
    {
        GameObject p = CreateEffect(Effect.fireballBig);

        p.transform.localPosition = data.GetSourcePos();
        Vector3 targetPos = data.GetTargetPos();
        p.transform.localScale = Vector3.zero;
        List<Effect> smokes = new List<Effect>() { Effect.smoke0, Effect.smoke1, Effect.smoke2, Effect.smoke3 };
        int f = 15;
        LerpTo(p, targetPos, f,0,true);
        for (int i=0;i<f;i++)
        {
            GameObject s = CreateEffect(Board.RandElem(smokes));
            s.transform.position = p.transform.position + new Vector3(Random.Range(-0.5f,0.5f), Random.Range(-0.1f, 0.1f));
            s.transform.localScale = Vector3.one*1.25f;
            LerpZoom(s, Vector3.zero, 10);
            Spin(s, Random.Range(-5, 5));
            SpriteFade(s, 10);
            yield return null;
        }

        yield return Wait(5);
        GameObject slash = CreateEffect(Effect.slash0);
        slash.transform.position = p.transform.position;
        slash.transform.localScale = Vector3.one * 1.25f;
        BounceZoom(slash, 0.1f);
        SpriteFade(slash, 8,5);
        Destroy(p.gameObject);
    }

    IEnumerator RageEffect(AnimationData data)
    {
        StartCoroutine(RageEffectInternal(data));
        yield return Wait(10);
    }
    IEnumerator RageEffectInternal(AnimationData data)
    {
        GameObject p = CreateEffect(Effect.rage);

        p.transform.localPosition = data.targetPos;
        SpriteRenderer s = p.GetComponentInChildren<SpriteRenderer>();
        var color = s.color;
        color.a = 0;
        s.color = color;
        for (int i = 0; i < 20; i++)
        {
            p.transform.localPosition = data.targetPos;
            p.transform.localScale = data.GetTarget().transform.localScale;

            if (i < 10) color.a += 1 / 20f;
            else color.a -= 1 / 20f;

            s.color = color;
            yield return null;
        }
        Destroy(p.gameObject);
    }
    
    IEnumerator WhirlwindEffect(AnimationData data, Effect effect)
    {
        GameObject p = CreateEffect(effect);

        p.transform.localPosition = data.sourcePos;
        p.transform.parent = data.GetSource().transform.parent;
        p.transform.localScale = Vector3.one;
        Spin(p, 10f);
        SpriteFade(p, 10, 10);
        yield return Wait(10);
    }
    
    IEnumerator Boom(AnimationData data, Effect effect)
    {
        GameObject p = CreateEffect(effect);

        p.transform.localPosition = data.sourcePos;
        LerpZoom(p, Vector3.one * 30, 30);
        SpriteFade(p, 15);
        yield return Wait(15);
    }

    IEnumerator AlexEffect(AnimationData data)
    {
        Vector3 sourcePos = data.sourcePos;
        Vector3 targetPos = data.targetPos;
        for (int i=0;i<10;i++)
        {
            Vector3 p = Vector3.Lerp(sourcePos, targetPos, i / 10F);
            for (int j = 0; j < 5; j++)
            {
                CreateParticle(p, Effect.fireballSmall,Board.GetColor("DF3E23"));
            }
            yield return Wait(1);
        }

        for (int j = 0; j < 20; j++)
        {
            CreateParticle(targetPos, Effect.fireballSmall, Board.GetColor("DF3E23"));
        }
        yield return Wait(5);
    }
    IEnumerator AoEEffect(AnimationData data, Effect effect)
    {
        StartCoroutine(BoardAoE(data, effect));
        yield return Wait(10);
    }
    IEnumerator BoardAoE(AnimationData data, Effect effect)
    {
        GameObject p = CreateEffect(effect);

        p.transform.localPosition = data.sourcePos;

        SpriteRenderer s = p.GetComponentInChildren<SpriteRenderer>();
        if (data.friendly) p.transform.localPosition = new Vector3(0, 2.1325f+1/16f);
        else p.transform.localPosition = new Vector3(0, -2.1325f-1/16f);

        BounceZoom(p, 0.1f);
        var color = s.color;
        color.a = 0;
        s.color = color;
        for (int i = 0; i < 20; i++)
        {
            if (i < 10) color.a += 1 / 20f;
            else color.a -= 1 / 20f;

            s.color = color;
            yield return null;
        }
        Destroy(p.gameObject);
    }

    IEnumerator PotionProjectile(AnimationData data, Effect effect, int frames = 10)
    {
        GameObject p = CreateEffect(effect);

        p.transform.localPosition = data.sourcePos;
        Vector3 targetPos = data.targetPos;
        Spin(p, targetPos.x < p.transform.localPosition.x ? 5 : -5);
        yield return LerpTo(p, targetPos, frames);
        Destroy(p.gameObject);
        for (int i=0;i<20;i++)
        {
            CreateParticle(targetPos, effect);
        }
    }
    IEnumerator PotionProjectileWeapon(AnimationData data, Effect effect)
    {
        GameObject p = CreateEffect(effect);
        p.transform.localPosition = data.sourcePos;
        
        Vector3 targetPos = data.friendly == false? board.enemyHero.weaponFrame.transform.localPosition : board.currHero.weaponFrame.transform.localPosition;
        Vector3 offset = data.friendly == false? board.enemyHero.transform.localPosition : board.currHero.transform.localPosition;
        targetPos += offset;
        targetPos += new Vector3(0, 0.5f);
        targetPos *= 1.25f;

        Spin(p, targetPos.x < p.transform.localPosition.x ? 5 : -5);
        yield return LerpTo(p, targetPos, 10);
        Destroy(p.gameObject);
        for (int i=0;i<20;i++)
        {
            CreateParticle(targetPos, effect);
        }
    }

    IEnumerator Tinkers_Oil(AnimationData data)
    {
        Hero h = data.friendly ? board.currHero : board.enemyHero;
        bool anim = false;
        if (h.weapon != null)
        {
            anim = true;
            StartCoroutine(PotionProjectileWeapon(data, Effect.potionLight));
        }
        if (data.GetTarget() != (data.friendly?board.enemyHero:board.currHero))
        {
            anim = true;
            StartCoroutine(PotionProjectile(data, Effect.potionLight));
        }
        yield return Wait(anim? 10:0);
    }

    IEnumerator FanOfKnives(AnimationData data)
    {
        MinionBoard b = data.friendly ? board.enemyMinions : board.currMinions;
        List<GameObject> objs = new List<GameObject>();
        foreach(var c in b.minionObjects.Values)
        {
            GameObject p = CreateEffect(Effect.dagger);

            p.transform.localPosition = data.GetSourcePos();
            Vector3 targetPos = c.transform.localPosition;

            PointTo(p, targetPos, 90);
            LerpTo(p, targetPos, 10);
            objs.Add(p);
        }
        
        yield return Wait(10);
        foreach (GameObject g in objs) Destroy(g);
    }

    public void CreateParticle(Vector3 pos, Effect effect, Color color = new Color())
    {
        Color c = color;
        switch (effect)
        {
            case Effect.potionGreen:
                c = Board.GetColor("14A02E");
                break;
            case Effect.potionBlue:
                c = Board.GetColor("20D6C7");
                break;
            case Effect.potionLight:
                c = Board.GetColor("92DCBA");
                break;
            case Effect.bomb:
                c = Board.GetColor("73172D");
                break;
        }
        Effect[] particles = { Effect.particle1, Effect.particle2, Effect.particleCross };
        GameObject p = CreateEffect(Board.RandElem(particles));

        p.GetComponentInChildren<SpriteRenderer>().color = c;
        p.transform.localPosition = pos;
        Spin(p, Random.Range(-5, 5));
        float angle = Random.Range(0, 360) * Mathf.PI/180f;
        Vector3 targetPos = pos + 4 * new Vector3(Mathf.Cos(angle), Mathf.Sin(angle));
        p.transform.localPosition = Vector3.Lerp(pos, targetPos, 0.15f);
        LerpTo(p, targetPos, 30);
        SpriteFade(p, 20);
    }

    public void CreateSpellpowerParticle(Vector3 pos, Effect effect, Color color = new Color())
    {
        Color c = color;
        Effect[] particles = { Effect.particle1, Effect.particle2, Effect.particleCross };
        GameObject p = CreateEffect(Board.RandElem(particles));

        p.GetComponentInChildren<SpriteRenderer>().color = c;
        p.transform.localPosition = pos;
        p.transform.localScale = Vector3.one * 0.5f;
        Spin(p, Random.Range(-5, 5));
        Vector3 targetPos = pos + 4 * new Vector3(0,1);
        p.transform.localPosition = Vector3.Lerp(pos, targetPos, 0.15f);
        LerpTo(p, targetPos, 45);
        SpriteFade(p, 20,5);
    }
}
