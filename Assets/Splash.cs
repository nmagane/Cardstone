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
    public void Set(Type t, int value, MonoBehaviour obj)
    {
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

    IEnumerator follower(MonoBehaviour obj)
    {
        while (obj!=null)
        {
            transform.position = obj.transform.position;
            yield return null;
        }
    }
    IEnumerator fader()
    {
        float bumpTime = 20;
        for (int i=0;i<bumpTime;i++)
        {
            transform.localScale += Vector3.one * 0.25f / bumpTime;
            yield return AnimationManager.Wait(1);
        }
        //yield return AnimationManager.Wait(10);
        float fadeTime = 40;
        for (int i=0;i< fadeTime; i++)
        {
            spriteRenderer.color += new Color(0, 0, 0, -1 / fadeTime);
            text.color += new Color(0, 0, 0, -1 / fadeTime);

            yield return AnimationManager.Wait(1);
        }

        Destroy(this.gameObject);
    }
}
