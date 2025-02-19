using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Splash : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public TMP_Text text;
    public Sprite damageSprite;
    public Sprite healSprite;

    public enum Type
    {
        Damage,
        Heal,
    }
    public void Set(Type t, int value, GameObject obj)
    {
        if (t==Type.Damage && Mathf.Abs(value)>=8)
        {
            ShakeScreen(0.2f,-0.1f,3);
            ShakeScreen(-0.1f,0.2f,5);
        }
        string s = "";
        switch(t)
        {
            case Type.Damage:
                spriteRenderer.sprite = damageSprite;
                s += "-";
                break;
            case Type.Heal:
                spriteRenderer.sprite = healSprite;
                s += "+";
                break;
        }
        s += Mathf.Abs(value);
        text.text = s;
        transform.position = obj.transform.position;
        StartCoroutine(follower(obj));
        StartCoroutine(fader());
    }

    IEnumerator follower(GameObject obj)
    {
        while (obj!=null)
        {
            transform.position = obj.transform.position;
            yield return null;
        }
    }
    IEnumerator fader()
    {
        float bumpTime = 10;
        for (int i=0;i<bumpTime;i++)
        {
            transform.localScale += Vector3.one * 0.25f / bumpTime;
            yield return AnimationManager.Wait(1);
        }
        yield return AnimationManager.Wait(5);
        float fadeTime = 25;
        for (int i=0;i< fadeTime; i++)
        {
            spriteRenderer.color += new Color(0, 0, 0, -1 / fadeTime);
            text.color += new Color(0, 0, 0, -1 / fadeTime);

            yield return AnimationManager.Wait(1);
        }

        Destroy(this.gameObject);
    }
    public void ShakeScreen(float x, float y, int frames = 3)
    {
        StartCoroutine(screenshaker(x, y, frames));
    }
    IEnumerator screenshaker(float x, float y, int frames = 3)
    {
        Board.Instance.transform.localPosition += new Vector3(x, y);
        for (int i = 0; i < frames; i++)
        {
            yield return null;
        }
        Board.Instance.transform.localPosition -= new Vector3(x, y);
    }
}
