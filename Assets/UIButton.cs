using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;
public class UIButton : MonoBehaviour
{
    public enum func
    {
        SubmitMulligan,
        EndTurn,
    }
    public AudioClip[] sounds;
    public SpriteRenderer bg;
    public TMP_Text text;
    public SpriteRenderer icon;
    public func f;
    public MonoBehaviour owner;
    public Board board;
    public int data = 0;
    public float floatData = 0;
    public bool bounce = false;

    public enum ColorPreset
    {
        grey,
        black,
    }

    void Start()
    {
        SetColor(bg.color);
        buttonSize = GetComponent<BoxCollider2D>().size;
    }
    public void SetColor(ColorPreset c)
    {
        switch (c)
        {
            case ColorPreset.grey:
                SetColor(new Color(0.37f, 0.37f, 0.37f));
                break;
            case ColorPreset.black:
                SetColor(new Color(0.23f, 0.23f, 0.23f));
                break;
        }
    }
    public void SetColor(Color c)
    {
        float H, S, V;
        Color.RGBToHSV(c, out H, out S, out V);
        bg.color = c;
        icon.color = Color.HSVToRGB(H, S, V + 0.25f);
        text.color = Color.HSVToRGB(H, S, V + 0.25f);
    }

    public void SubmitMulligan()
    {
        board.SubmitMulligan();
    }

    public void EndTurn()
    {
        board.SubmitEndTurn();
    }







    private void OnMouseEnter()
    {
        if (bounce)
        {
            //Camera.main.GetComponentInChildren<AudioManager>().PlayUI_Randomized(sounds[0], 1, 1.2f);
            //Camera.main.GetComponentInChildren<AudioManager>().PlayUI_Randomized(sounds[0],1.7f,1.9f);
            StartCoroutine(bouncer());
        }

        switch (f)
        {
            default:
                break;
        }
    }
    private void OnMouseExit()
    {
        switch (f)
        {
            default:
                break;
        }
    }
    private void OnMouseOver()
    {
        if (Input.GetMouseButtonUp(1))
        {
            switch (f)
            {
                default:
                    break;
            }
        }
    }
    private void OnMouseUpAsButton()
    {
        if (bounce)
        {
            //Camera.main.GetComponentInChildren<AudioManager>().PlayUI_Randomized(sounds[0], 1.7f, 1.9f);
            StartCoroutine(bigBouncer(0.25f));
        }
        Invoke(f.ToString(), 0);
    }


    //bool anim = false;
    Vector2 buttonSize;
    IEnumerator bouncer(float amp = 0.15f)
    {
        //if (anim) yield break;
        //anim = true;
        //if (f == func.TutorialSelectDifficulty) amp /= 2f;
        this.transform.localScale += Vector3.one * amp;
        for (int i = 0; i < 8; i++)
        {
            this.transform.localScale += Vector3.one * -amp / 7;
            GetComponent<BoxCollider2D>().size = buttonSize / this.transform.localScale;
            yield return null;
        }
        yield return null;
        for (int i = 0; i < 1; i++)
        {
            this.transform.localScale += Vector3.one * amp / 7;

            GetComponent<BoxCollider2D>().size = buttonSize / this.transform.localScale;
            yield return null;
        }
        GetComponent<BoxCollider2D>().size = buttonSize;
        //anim = false;
    }
    IEnumerator bigBouncer(float amp = 0.25f)
    {
        //if (anim) yield break;
        //anim = true;
        this.transform.localScale += Vector3.one * amp;
        for (int i = 0; i < 8; i++)
        {
            this.transform.localScale += Vector3.one * -amp / 7;
            yield return null;
        }
        yield return null;
        for (int i = 0; i < 1; i++)
        {
            this.transform.localScale += Vector3.one * amp / 7;
            yield return null;
        }
        //anim = false;
    }

}