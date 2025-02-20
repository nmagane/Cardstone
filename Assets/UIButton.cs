using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;
public class UIButton : MonoBehaviour
{
    public enum func
    {
        StartMatchmaking,
        SubmitMulligan,
        EndTurn,
        RestartScene,
        Concede,
        Menu,
        SelectDeck,
        EditDeck,
        NewDeck,
        ConfirmDeck,
        CollectionReturn,
        DeleteDeck,
        NewDeckClass,
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
    public bool locked = false;
    public bool bounce = false;

    public enum ColorPreset
    {
        grey,
        black,

        ActionAvailable,
        NoActions,

    }

    void Start()
    {
        SetColor(bg.color);
        buttonSize = GetComponent<BoxCollider2D>().size;
    }
    public void SetColor(ColorPreset c, bool bounce = false)
    {
        Color x = bg.color;
        switch (c)
        {
            case ColorPreset.grey:
                SetColor(new Color(0.37f, 0.37f, 0.37f));
                break;
            case ColorPreset.black:
                SetColor(new Color(0.23f, 0.23f, 0.23f));
                break;
            case ColorPreset.ActionAvailable:
                bg.color = Board.GetColor("BB7547");
                text.color = Board.GetColor("DBA463");
                break;
            case ColorPreset.NoActions:
                bg.color = Board.GetColor("1A7A3E");
                text.color = Board.GetColor("59C135");
                break;
        }
        if (bounce && bg.color != x)
            StartCoroutine(bigBouncer());
    }
    public void SetColor(Color c, bool bounce=false)
    {
        Color x = bg.color;

        float H, S, V;
        Color.RGBToHSV(c, out H, out S, out V);
        bg.color = c;
        //icon.color = Color.HSVToRGB(H, S, V + 0.25f);
        text.color = Color.HSVToRGB(H, S, V + 0.25f);

        if (bounce && bg.color != x)
            StartCoroutine(bigBouncer());
    }

    public void SubmitMulligan()
    {
        board.SubmitMulligan();
    }

    public void EndTurn()
    {
        board.SubmitEndTurn();
    }
    public void Concede()
    {
        board.SubmitConcede();
    }
    public void Menu()
    {
        board.ToggleMenu();
    }

    public void StartMatchmaking()
    {
        owner.GetComponent<Mainmenu>().StartMatchmaking();
        //Destroy(this.gameObject);
    }
    public void SelectDeck()
    {
        owner.GetComponent<Mainmenu>().SetDeck(data);
    }
    public void EditDeck()
    {
        owner.GetComponent<CollectionMenu>().SelectDeck(data);
    }
    public void NewDeck()
    {
        owner.GetComponent<CollectionMenu>().NewDeck();
    }
    public void ConfirmDeck()
    {
        owner.GetComponent<CollectionMenu>().SaveDeck();
    }
    public void CollectionReturn()
    {
        owner.GetComponent<CollectionMenu>().Back();
    }
    public void DeleteDeck()
    {
        owner.GetComponent<CollectionMenu>().DeleteDeck();
    }
    public void NewDeckClass()
    {
        owner.GetComponent<CollectionMenu>().SelectNewClass(data);
    }
    public void RestartScene()
    {
        board.RestartScene();
        Destroy(this.gameObject);
    }
    




    private void OnMouseEnter()
    {
        StartCoroutine(bouncer());
        
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
            StartCoroutine(bigBouncer(0.25f));
        }
        if (locked) return;

        switch (f)
        {
            case func.StartMatchmaking:
                StartMatchmaking();
                break;
            case func.EndTurn:
                EndTurn();
                break;
            case func.SubmitMulligan:
                SubmitMulligan();
                break;
            case func.RestartScene:
                RestartScene();
                break;
            case func.Concede:
                Concede();
                break;
            case func.Menu:
                Menu();
                break;
            case func.SelectDeck:
                SelectDeck();
                break;
            case func.EditDeck:
                EditDeck();
                break;
            case func.NewDeck:
                NewDeck();
                break;
            case func.ConfirmDeck:
                ConfirmDeck();
                break;
            case func.CollectionReturn:
                CollectionReturn();
                break;
            case func.DeleteDeck:
                DeleteDeck();
                break;
            case func.NewDeckClass:
                NewDeckClass();
                break;
            default:
                Debug.LogError("NO BUTTON FUNCTION");
                break;
        }
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
    public IEnumerator bigBouncer(float amp = 0.25f)
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