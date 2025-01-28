using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    Dictionary<MonoBehaviour, Coroutine> activeLerps = new Dictionary<MonoBehaviour, Coroutine>();
    IEnumerator Wait(int x)
    {
        for (int i = 0; i < x; i++)
            yield return null;
    }

    public void LerpTo(MonoBehaviour obj,Vector3 tar, int frameCount=30, float bounce = 0)
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
    IEnumerator _lerpTo(MonoBehaviour obj, Vector3 tar, float frameCount= 30, float bounce=0)
    {
        Vector3 OP = obj.transform.localPosition;
        Vector3 DP = tar;
        Vector3 dir = (DP - OP).normalized;
        DP += dir * bounce;
        for (int i = 0; i < frameCount; i++)
        {
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
                obj.transform.localPosition = Vector3.Lerp(OP, DP, (i + 1) / bframes);
                yield return Wait(1);
            }
        }
        if (activeLerps.ContainsKey(obj)) activeLerps.Remove(obj);
    }


}
