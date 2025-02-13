using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public partial class AnimationManager
{

    public void LiftMinion(Creature c)
    {
        c.floatEnabled = false;
        c.SetElevated(true);
        StartCoroutine(elevator(c, 1.05f, 0.6f, 5));
    }
    public void CancelLiftMinion(Creature c)
    {
        c.isElevated = false;
        StartCoroutine(elevator(c, 1.025f, 0.4f, 10,true));
    }

    public void LiftHero(Hero h)
    {
        h.shadowSpriteRenderer.enabled = true;
        h.SetElevated(true);
        StartCoroutine(elevatorHero(h, 1.05f, 0.6f, 5));
    }
    public void CancelLiftHero(Hero h)
    {
        h.isElevated = false;
        StartCoroutine(elevatorHero(h, 1, 0, 5,true));
    }


    int attackFrames = 12;
    public Coroutine PreAttackMinion(Creature c, Vector3 target)
    {
        if (c.isElevated == false) LiftMinion(c);
        c.boardPos = c.transform.localPosition;
        Vector3 dir = (target - c.transform.localPosition).normalized;
        float ang = Mathf.Atan2(c.boardPos.y - target.y, c.boardPos.x - target.x);
        float diff = 1.4375f * Mathf.Sin(ang);
        target = target + dir * diff*2;
        return StartCoroutine(preAttack(c,target));
    }
    IEnumerator preAttack(Creature c, Vector3 target)
    {
        yield return Wait(10);
        yield return LerpTo(c.gameObject, Vector3.Lerp(c.transform.localPosition, target, 0.25f), (int)(attackFrames * 0.25));
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
    public Coroutine PreSwing(Hero c, Vector3 target)
    {
        if (c.isElevated == false) LiftHero(c);
        target -= c.transform.localPosition;
        Vector3 dir = (target - c.spriteRenderer.transform.localPosition).normalized;
        float ang = Mathf.Atan2(0 - target.y, 0 - target.x);
        float diff = 1.4375f * Mathf.Sin(ang);
        target = target + dir * diff*2;
        return StartCoroutine(preSwing(c,target));
    }
    IEnumerator preSwing(Hero c, Vector3 target)
    {
        yield return Wait(10);
        yield return LerpTo(c.spriteRenderer.gameObject, Vector3.Lerp(c.spriteRenderer.transform.localPosition, target, 0.25f), (int)(attackFrames * 0.25));
    }
    public Coroutine ConfirmSwing(Hero c, Vector3 target)
    {
        return StartCoroutine(swingConfirm(c, target));
    }
    public IEnumerator swingConfirm(Hero c, Vector3 target)
    {

        target -= c.transform.localPosition;
        Vector3 dir = (target - c.spriteRenderer.transform.localPosition).normalized;
        float ang = Mathf.Atan2(0 - target.y, 0 - target.x);
        float diff = 1.4375f * Mathf.Sin(ang);
        target = target + dir * diff * 2;

        yield return _lerpAccel(c.spriteRenderer.gameObject, target, (int)(attackFrames* 0.75));
        CancelLiftHero(c);
        c.spriteRenderer.transform.localPosition = Vector3.Lerp(c.spriteRenderer.transform.localPosition, Vector3.zero, 0.075f);
        LerpTo(c.spriteRenderer.gameObject, Vector3.zero, 5);

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
        c.floatEnabled = returnToFloat;
        if (c == null) yield break;
        if (returnToFloat)
        {
            c.SetSortingOrder(c.minion.index);
            c.SetElevated(false);
        }
        else
        {
            c.SetSortingOrder(c.minion.index + 10);
        }
    }

    IEnumerator elevatorHero(Hero c, float scale, float v, float frames, bool end = false)
    {
        float op = c.shadowElevation;
        LerpZoom(c.spriteRenderer.gameObject, Vector3.one * scale, frames);
        for (int i = 0; i < frames; i++)
        {
            if (c == null) yield break;
            c.shadowElevation = Mathf.Lerp(op, v, (i + 1) / frames);
            yield return null;
        }

        if (c == null) yield break;

        if (end)
        {
            c.shadowSpriteRenderer.enabled = false;
            c.SetElevated(false);
        }
    }

    public void DeathAnim(Creature c)
    {
        c.floatEnabled = false;
        foreach (Trigger t in c.minion.triggers)
        {
            if (t.type==Trigger.Type.Deathrattle)
            {
                StartCoroutine(deathrattleAnim(c));
            }
        }
        StartCoroutine(_death(c));
    }
    IEnumerator deathrattleAnim(Creature c)
    {
        SpriteRenderer s = Instantiate(c.board.UISprite).GetComponent<SpriteRenderer>();
        s.sprite = c.deathrattleSprite.sprite;
        s.transform.parent = c.transform.parent;
        s.transform.localScale = Vector3.one*2;
        s.transform.localPosition = c.boardPos;
        s.sortingLayerName = c.spriteRenderer.sortingLayerName;
        s.sortingOrder = c.spriteRenderer.sortingOrder + 4;
        s.color = new Color(1, 1, 1, 0);
        yield return Wait(10);
        float f = 5;
        for (int i = 0; i < f; i++)
        {
            s.color += new Color(0, 0, 0, 1 / f);
            yield return Wait(1);
        }

        LerpZoom(s.gameObject, Vector3.one * 3, 80);
        f = 25;
        for (int i=0;i<f;i++)
        {
            s.color += new Color(0, 0, 0, -1 /f);
            yield return Wait(1);
        }
        Destroy(s.gameObject);
    }
    IEnumerator _death(Creature c)
    {
        //yield return Wait(5);
        yield return _lerpZoom(c.gameObject, Vector3.zero, 8);
        Destroy(c.gameObject);
    }


}
