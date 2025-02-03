using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public partial class AnimationManager
{

    public void LiftMinion(Creature c)
    {
        c.floating = false;
        StartCoroutine(elevator(c, 1.05f, 0.6f, 5));
    }
    public void CancelLiftMinion(Creature c)
    {
        StartCoroutine(elevator(c, 1.025f, 0.4f, 10,true));
    }

    int attackFrames = 12;
    public Coroutine PreAttackMinion(Creature c, Vector3 target)
    {
        c.boardPos = c.transform.localPosition;
        Vector3 dir = (target - c.transform.localPosition).normalized;
        float ang = Mathf.Atan2(c.boardPos.y - target.y, c.boardPos.x - target.x);
        float diff = 1.4375f * Mathf.Sin(ang);
        target = target + dir * diff*2;
        return LerpTo(c.gameObject, Vector3.Lerp(c.transform.localPosition, target, 0.25f), (int)(attackFrames*0.25));
    }
    public Coroutine ConfirmAttackMinion(Creature c, Vector3 target)
    {
        return StartCoroutine(attackConfirm(c, target));
    }
    public IEnumerator attackConfirm(Creature c, Vector3 target)
    {

        Vector3 dir = (target - c.transform.localPosition).normalized;
        float ang = Mathf.Atan2(c.boardPos.y - target.y, c.boardPos.x - target.x);
        float diff = 1.4375f * Mathf.Sin(ang);
        target = target + dir * diff*1.5f * ((target.y<c.transform.localPosition.y)? -1:1);

        yield return _lerpAccel(c.gameObject, target, (int)(attackFrames* 0.75));

        CancelLiftMinion(c);
        c.transform.localPosition = Vector3.Lerp(c.transform.localPosition, c.boardPos, 0.075f);
        LerpTo(c.gameObject, c.boardPos, 5);

    }
     float v0 = 0.075f;
     float accel = 0.075f;
    public IEnumerator _lerpAccel(GameObject obj, Vector3 target, float frames)
    { 
        float v = v0;

        Vector3 OP = obj.transform.localPosition;
        Vector3 DP = target;
        Vector3 dir = (DP - OP).normalized;
        bool done = false;
        for (int i = 0; i < frames; i++)
        {
            if (obj == null) break;
            obj.transform.localPosition = Vector3.Lerp(OP, DP, v+(i + 1) / frames);

            if (done) break;
            if (v + (i + 1) / frames > 1) done = true;
            v += accel;
            yield return Wait(1);
        }
        if (activeLerps.ContainsKey(obj)) activeLerps.Remove(obj);
    }
    IEnumerator elevator(Creature c, float scale,float v, float frames, bool returnToFloat = false)
    {
        float op = c.shadow.elevation;
        LerpZoom(c.gameObject, Vector3.one * scale, frames);
        for (int i = 0; i < frames; i++)
        {
            if (c == null) yield break;
            c.shadow.elevation = Mathf.Lerp(op, v, (i + 1) / frames);
            yield return null;
        }
        c.floating = returnToFloat;
        if (c == null) yield break;
        if (returnToFloat)
            c.SetSortingOrder(c.minion.index);
        else
            c.SetSortingOrder(c.minion.index + 10);
    }

    public void DeathAnim(Creature c)
    {
        c.floating = false;
        StartCoroutine(_death(c));
    }
    IEnumerator _death(Creature c)
    {
        //yield return Wait(5);
        yield return _lerpZoom(c.gameObject, Vector3.zero, 8);
        Destroy(c.gameObject);
    }


}
