using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    Dictionary<GameObject, Coroutine> activeLerps = new Dictionary<GameObject, Coroutine>();
    Dictionary<GameObject, Coroutine> activeZooms = new Dictionary<GameObject, Coroutine>();
    Dictionary<GameObject, Coroutine> activeRotates = new Dictionary<GameObject, Coroutine>();
    public static IEnumerator Wait(int x)
    {
        for (int i = 0; i < x; i++)
            yield return null;
    }
    public void MulliganAnim(Card c, HandCard newCard)
    {
        StartCoroutine(_mulliganAnim(c,newCard));
    }
    IEnumerator _mulliganAnim(Card c,HandCard newCard)
    {
        Vector3 OP = c.transform.localPosition;
        c.Flip();
        LerpZoom(c.gameObject, Vector3.one);
        yield return _lerpTo(c.gameObject, c.board.deck.transform.localPosition, 20);
        c.Flip();
        c.Set(newCard);
        LerpZoom(c.gameObject, Vector3.one*1.5f);
        yield return _lerpTo(c.gameObject, OP, 20);
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
    IEnumerator _drawAnim(GameObject obj, Vector3 tar1, Vector3 tar2, float f1, float f2, int delay,Vector3 rotation)
    {
        LerpZoom(obj, Vector3.one * 1.5f, f1);
        yield return _lerpTo(obj, tar1, f1);
        yield return Wait(delay);
        LerpZoom(obj, Vector3.one, f2);
        LerpRotate(obj, rotation, f2);
        yield return _lerpTo(obj, tar2, f2);
    }
    public void LerpTo(MonoBehaviour obj, Vector3 tar, int frameCount = 30, float bounce = 0)
    {
        LerpTo(obj.gameObject, tar, frameCount, bounce);
    }
    public void LerpTo(GameObject obj,Vector3 tar, int frameCount=30, float bounce = 0)
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
    public void LerpZoom(GameObject obj, Vector3 tar, float frameCount = 30, float bounce = 0)
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

    public void FadeCard(Card c, bool friendly, bool discard = false, Card.Cardname name = Card.Cardname.Coin)
    {
        if (discard)
        {
            if (friendly == false)
            {
                c.back.enabled = false;
                
                c.Set(new HandCard(name,0));
                LerpTo(c, c.transform.localPosition + new Vector3(0, -10), 120);
            }
            else
            {
                LerpTo(c, c.transform.localPosition + new Vector3(0, 10), 120);
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
}
