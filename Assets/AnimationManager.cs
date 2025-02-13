using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class AnimationManager : MonoBehaviour
{
    public Material shadowMat;
    public Board board;
    Dictionary<GameObject, Coroutine> activeLerps = new Dictionary<GameObject, Coroutine>();
    Dictionary<GameObject, Coroutine> activeZooms = new Dictionary<GameObject, Coroutine>();
    Dictionary<GameObject, Coroutine> activeRotates = new Dictionary<GameObject, Coroutine>();
    public static IEnumerator Wait(int x)
    {
        for (int i = 0; i < x; i++)
            yield return null;
    }
    public void MulliganEnemyAnim(Card c)
    {
        StartCoroutine(_mulliganAnimEnemy(c));
    }
    IEnumerator _mulliganAnimEnemy(Card c)
    {
        yield return LerpTo(c.gameObject, c.transform.localPosition + new Vector3(0, 2),8);
        yield return LerpTo(c.gameObject, c.transform.localPosition + new Vector3(0, -2),8);
    }
    public void MulliganAnim(Card c, HandCard newCard)
    {
        StartCoroutine(_mulliganAnim(c,newCard));
    }
    IEnumerator _mulliganAnim(Card c,HandCard newCard)
    {
        c.GetComponent<BoxCollider2D>().enabled = false;

        Vector3 OP = c.transform.localPosition;
        c.Flip();
        LerpZoom(c.gameObject, Vector3.one);
        yield return _lerpTo(c.gameObject, c.board.deck.transform.localPosition, 20);
        c.Flip();
        c.Set(newCard);
        LerpZoom(c.gameObject, Vector3.one*1.5f);
        yield return _lerpTo(c.gameObject, OP, 20);

        c.GetComponent<BoxCollider2D>().enabled = true;
    }
    public void DrawAnim(GameObject obj, Vector3 tar1, Vector3 tar2, float f1, float f2, int delay, Vector3 rotation)
    {
        if (activeLerps.ContainsKey(obj))
        {
            if (activeLerps[obj] == null)
                activeLerps.Remove(obj);
            else
            {
                Coroutine stopper = activeLerps[obj];
                activeLerps.Remove(obj);
                StopCoroutine(stopper);
            }
        }
        Coroutine c = StartCoroutine(_drawAnim(obj, tar1, tar2, f1,f2,delay,rotation));
        activeLerps.Add(obj, c);
    }
    public void MillAnim(Card c, bool friendly)
    {
        Vector3 t = friendly ? new Vector3(13, -5, 0) : new Vector3(13, 5, 0);
        StartCoroutine(_millAnim(c,t,10,20));
    }
    IEnumerator _millAnim(Card obj, Vector3 target, float frames, int delay)
    {
        LerpZoom(obj.gameObject, Vector3.one * 1.5f, frames);
        yield return _lerpTo(obj.gameObject, target, frames);
        yield return Wait(delay);
        StartCoroutine(_fadeCard(obj,20));
        LerpZoom(obj.gameObject, Vector3.one * 2, 60);


    }
    IEnumerator _drawAnim(GameObject obj, Vector3 tar1, Vector3 tar2, float f1, float f2, int delay,Vector3 rotation)
    {
        obj.GetComponent<Card>().GetComponent<BoxCollider2D>().enabled = false;
        LerpZoom(obj, Vector3.one * 1.5f, f1);
        yield return _lerpTo(obj, tar1, f1);
        yield return Wait(delay);
        obj.GetComponent<Card>().GetComponent<BoxCollider2D>().enabled = true;
        LerpZoom(obj, Vector3.one, f2);
        LerpRotate(obj, rotation, f2);
        yield return _lerpTo(obj, tar2, f2);
    }

    public void LerpTo(MonoBehaviour obj, Vector3 tar, int frameCount = 30, float bounce = 0)
    {
        LerpTo(obj.gameObject, tar, frameCount, bounce);
    }
    public Coroutine LerpTo(GameObject obj,Vector3 tar, int frameCount=30, float bounce = 0)
    {
        if (activeLerps.ContainsKey(obj))
        {
            if (activeLerps[obj] == null)
                activeLerps.Remove(obj);
            else
            {
                Coroutine stopper = activeLerps[obj];
                activeLerps.Remove(obj);
                StopCoroutine(stopper);
            }
        }
        Coroutine c = StartCoroutine(_lerpTo(obj, tar, frameCount, bounce));
        activeLerps.Add(obj, c);
        return c;
    }
    public void EndMovement(GameObject obj)
    {
        if (activeLerps.ContainsKey(obj))
        {
            StopCoroutine(activeLerps[obj]);
            activeLerps.Remove(obj);
        }
    }
    IEnumerator _lerpTo(GameObject obj, Vector3 tar, float frameCount= 30, float bounce=0)
    {
        Vector3 OP = obj.transform.localPosition;
        Vector3 DP = tar;
        Vector3 dir = (DP - OP).normalized;
        DP += dir * bounce;
        for (int i = 0; i < frameCount; i++)
        {
            if (obj == null) break;
            obj.transform.localPosition = Vector3.Lerp(OP, DP, (i + 1) / frameCount);
            yield return Wait(1);
        }

        if (bounce>0)
        {
            OP = obj.transform.localPosition;
            DP = tar;
            float bframes = 3;
            for (int i = 0; i < bframes; i++)
            {
                if (obj == null) break;
                obj.transform.localPosition = Vector3.Lerp(OP, DP, (i + 1) / bframes);
                yield return Wait(1);
            }
        }
        if (activeLerps.ContainsKey(obj)) activeLerps.Remove(obj);
    }
    public Coroutine LerpZoom(GameObject obj, Vector3 tar, float frameCount = 30, float bounce = 0)
    {
        if (activeZooms.ContainsKey(obj))
        {
            if (activeZooms[obj] == null)
                activeZooms.Remove(obj);
            else
            {
                Coroutine stopper = activeZooms[obj];
                activeZooms.Remove(obj);
                StopCoroutine(stopper);
            }
        }
        Coroutine c = StartCoroutine(_lerpZoom(obj, tar, frameCount, bounce));
        activeZooms.Add(obj, c);
        return c;
    }
    IEnumerator _lerpZoom(GameObject obj, Vector3 tar, float frameCount = 30, float bounce = 0)
    {
        Vector3 OP = obj.transform.localScale;
        Vector3 DP = tar;
        Vector3 dir = (DP - OP).normalized;
        DP += dir * bounce;
        for (int i = 0; i < frameCount; i++)
        {
            if (obj == null) break;
            obj.transform.localScale = Vector3.Lerp(OP, DP, (i + 1) / frameCount);
            yield return Wait(1);
        }

        if (bounce > 0)
        {
            OP = obj.transform.localScale;
            DP = tar;
            float bframes = 3;
            for (int i = 0; i < bframes; i++)
            {
                if (obj == null) break;
                obj.transform.localScale = Vector3.Lerp(OP, DP, (i + 1) / bframes);
                yield return Wait(1);
            }
        }
        if (activeZooms.ContainsKey(obj)) activeZooms.Remove(obj);
    }
    public void LerpRotate(GameObject obj, Vector3 tar, float frameCount = 30, float bounce = 0)
    {
        if (activeRotates.ContainsKey(obj))
        {
            if (activeRotates[obj] == null)
                activeRotates.Remove(obj);
            else
            {
                Coroutine stopper = activeRotates[obj];
                activeRotates.Remove(obj);
                StopCoroutine(stopper);
            }
        }
        Coroutine c = StartCoroutine(_lerpRotate(obj, tar, frameCount, bounce));
        activeRotates.Add(obj, c);
    }
    IEnumerator _lerpRotate(GameObject obj, Vector3 tar, float frameCount = 30, float bounce = 0)
    {
        Vector3 OP = obj.transform.localEulerAngles;
        Vector3 DP = tar;

        while (DP.z > 360)
        {
            DP -= new Vector3(0, 0, 360);
        }
        while (OP.z > 360)
        {
            OP -= new Vector3(0, 0, 360);
        }
        while (DP.z < 0)
        {
            DP += new Vector3(0, 0, 360);
        }
        while (OP.z < 0)
        {
            OP += new Vector3(0, 0, 360);
        }
        if (DP.z>(OP.z+180))
        {
            DP -= new Vector3(0,0,360);
        }
        if (OP.z>(DP.z+180))
        {
            OP -= new Vector3(0,0,360);
        }
        Vector3 dir = (DP - OP).normalized;
        DP += dir * bounce;
        for (int i = 0; i < frameCount; i++)
        {
            if (obj == null) break;
            obj.transform.localEulerAngles = Vector3.Lerp(OP, DP, (i + 1) / frameCount);
            yield return Wait(1);
        }

        if (bounce > 0)
        {
            OP = obj.transform.localEulerAngles;
            DP = tar;
            float bframes = 3;
            for (int i = 0; i < bframes; i++)
            {
                if (obj == null) break;
                obj.transform.localEulerAngles = Vector3.Lerp(OP, DP, (i + 1) / bframes);
                yield return Wait(1);
            }
        }
        if (activeRotates.ContainsKey(obj)) activeRotates.Remove(obj);
    }

    public void FadeCard(Card c, bool friendly, bool discard = false, Card.Cardname name = Card.Cardname.Coin, int cost=-1)
    {
        if (discard)
        {
            if (friendly == false)
            {
                c.back.enabled = false;
                
                c.Set(new HandCard(name,0));
                c.card.manaCost = cost;
                c.UpdateManaCost(true);
                LerpTo(c, c.transform.localPosition + new Vector3(0, -7), 120);
            }
            else
            {
                LerpTo(c, c.transform.localPosition + new Vector3(0, 7), 120);
            }
            StartCoroutine(_fadeCard(c,60,60));
            return;
        }

        if (friendly==false)
        {
            LerpTo(c, c.transform.localPosition + new Vector3(0, -6), 10);
        }

        StartCoroutine(_fadeCard(c,10));
    }
    IEnumerator _fadeCard(Card c,float f=10,int delay=0)
    {
        yield return Wait(delay);
        float frames = f;
        for (int i=0;i< frames; i++)
        {
            c.alpha -= 1/frames;
            yield return Wait(1);
        }
        Destroy(c.gameObject);
    }
    public void DropMinion(Creature c, float frames)
    {
        StartCoroutine(_dropper(c, frames));
    }
    public void BounceZoom(GameObject g, float bounce)
    {
        StartCoroutine(bounceZoom(g, bounce));
    }
    IEnumerator bounceZoom(GameObject g, float bounce)
    {
        for (int i = 0;i<3;i++)
        {
            if (g == null) yield break;
            g.transform.localScale += Vector3.one * bounce / 3f;
            yield return Wait(1);
        }
        for (int i = 0;i<3;i++)
        {
            if (g == null) yield break;
            g.transform.localScale += Vector3.one * -bounce / 3f;
            yield return Wait(1);
        }
    }
    public void DelayedDrop(Creature c, Vector3 loc, int delay, MinionBoard b)
    {
        StartCoroutine(_delayedDrop(c, loc, delay, b));
    }
    IEnumerator _delayedDrop(Creature c, Vector3 loc, int del, MinionBoard b)
    {
        yield return Wait(del);
        if (c == null) yield break;
        c.transform.localScale = Vector3.one * 1.15f;
        b.DropCreature(c, loc, 0);
    }
    public void DelayedDropWeapon(int delay, Hero b)
    {
        StartCoroutine(_delayedDropWep(delay, b));
    }
    IEnumerator _delayedDropWep(int delay,Hero b)
    {
        yield return Wait(delay);
        b.DropWeapon(0);
    }
    IEnumerator _dropper(Creature c, float f)
    {
        float e = c.shadow.elevation;
        for (float i = 0; i < f; i++)
        {
            c.shadow.elevation -= e / f;
            yield return Wait(1);
        }
    }

    Coroutine PlayFader = null;
    public void PlayFade(Card c, Vector3 location, bool hider=false)
    {
        LerpTo(c, location,10);
        LerpRotate(c.gameObject, Vector3.zero, 10);
        PlayFader = StartCoroutine(_fadePlay(c, 0.5f,10, 5,hider));
    }
    IEnumerator _fadePlay(Card c, float target,float f = 10, int delay = 0, bool hider=false)
    {
        yield return Wait(delay);
        float frames = f;
        float e = c.alpha;
        for (int i = 0; i < frames; i++)
        {
            if (c == null) yield break;
            c.alpha = Mathf.Lerp(e, target, (i + 1) / frames);
            yield return Wait(1);
        }

        if (c == null) yield break;
        if (hider)
        {
            c.alpha = 0;
        }
        else
        {
            Destroy(c.gameObject);
        }
    }
    public void Unfade(Card c)
    {
        if (PlayFader!=null)
        {
            StopCoroutine(PlayFader);
            PlayFader = null;
        }
        StartCoroutine(_unfadeCard(c));
    }
    IEnumerator _unfadeCard(Card c)
    {
        float x = c.alpha;
        for (int i = 0; i < 10;i++)
        {
            c.alpha = Mathf.Lerp(x, 1, (i + 1) / 10f);
            yield return Wait(1);
        }
    }

    public Coroutine Spin(GameObject g, float speed, int frames=0)
    {
        return StartCoroutine(_spinner(g, speed, frames));
    }
    IEnumerator _spinner(GameObject g, float speed, int frames)
    {
        if (frames==0)
        {
            while (true)
            {
                if (g == null) yield break;
                g.transform.localEulerAngles += new Vector3(0, 0, speed);
                yield return Wait(1);
            }
        }
        else
        {
            for (int i=0;i<frames;i++)
            {
                if (g == null) yield break;
                g.transform.localEulerAngles += new Vector3(0, 0, speed);
                yield return Wait(1);
            }
        }
    }

    public static void PointTo(GameObject g, Vector3 target, float ang=0)
    {
        float angle = 180 / Mathf.PI * Mathf.Atan2(g.transform.position.y - target.y, g.transform.localPosition.x - target.x);
        angle += ang;
        g.transform.localEulerAngles = new Vector3(0, 0, angle);
    }

    public void SpriteFade(GameObject g, int frames, bool destroy=true)
    {
        StartCoroutine(_fadeout(g, frames, destroy));
    }

    IEnumerator _fadeout(GameObject g, int frames, bool destroy=true)
    {
        SpriteRenderer s = g.GetComponent<SpriteRenderer>();
        for (int i=0;i<frames;i++)
        {
            s.color += new Color(0, 0, 0, -1f / frames);
            yield return Wait(1);
        }
        if (destroy) Destroy(s.gameObject);
    }
}
