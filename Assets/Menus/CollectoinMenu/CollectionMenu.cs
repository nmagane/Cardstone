using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class CollectionMenu : MonoBehaviour
{
    public Board board;

    public List<Database.CardInfo> cardData = new();
    public List<Database.CardInfo> classData = new();
    public List<Database.CardInfo> neutralData = new();
    public List<CardPicker> pickers;
    public List<UIButton> deckButtons;
    public GameObject deckButtonAnchor;
    public GameObject cardPickerAnchor;
    public SpriteRenderer cardpickerBG;
    public Card.Class currClass;
    public int currPage = 0;

    enum state
    {
        DeckSelect,
        NewSelect,
        DeckEdit,
    }
    public void Start()
    {
        GetData();
        InitDecks();

        foreach (CardPicker p in pickers)
            p.cardObject.board = board;

        SetState(state.DeckSelect);
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ChangePage(1);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ChangePage(-1);
        }
    }
    void SetState(state x)
    {
        
        switch (x)
        {
            case state.DeckSelect:
                cardPickerAnchor.transform.localScale = Vector3.zero;
                deckButtonAnchor.transform.localScale = Vector3.one;
                cardpickerBG.enabled = false;
                break;
            case state.NewSelect:
                cardPickerAnchor.transform.localScale = Vector3.one;
                deckButtonAnchor.transform.localScale = Vector3.zero;
                cardpickerBG.enabled = false;
                break;
            case state.DeckEdit:
                cardPickerAnchor.transform.localScale = Vector3.one;
                deckButtonAnchor.transform.localScale = Vector3.zero;
                cardpickerBG.enabled = true;
                break;
        }
    }


    public void GetData()
    {
        for (int i = 1; i < (int)Card.Cardname._COUNT; i++)
        {
            var c = Database.GetCardData((Card.Cardname)i);
            if (c == null) continue;
            if (c.TOKEN) continue;
            c.cardname = (Card.Cardname)i;
            cardData.Add(c);
        }

        cardData.Sort((x, y) => x.manaCost.CompareTo(y.manaCost));

        foreach (var c in cardData)
        {
            Debug.Log(c.manaCost);
            if (c.classType == Card.Class.Neutral) neutralData.Add(c);
        }
    }

    public void SetClass(Card.Class hero)
    {
        classData.Clear();
        currClass = hero;

        foreach (var c in cardData)
            if (c.classType == hero) classData.Add(c);
    }

    public Sprite[] deckSprites => board.mainmenu.deckSprites;

    public void InitDecks()
    {
        int i = 0;
        foreach (SaveManager.Decklist list in board.saveData.decks)
        {
            deckButtons[i].icon.sprite = deckSprites[(int)list.classType];
            deckButtons[i].text.text = list.deckName; 
            deckButtons[i].f = UIButton.func.EditDeck;
            deckButtons[i].owner = this;
            deckButtons[i].GetComponent<BoxCollider2D>().enabled = true;
            deckButtons[i].text.transform.localPosition = new Vector3(0, -1.6f);

            i++;
            if (i > 7) break;
        }
        for (int j = i; j < 8; j++)
        {
            deckButtons[j].icon.sprite = null;
            deckButtons[j].text.text = "NEW";
            deckButtons[j].f = UIButton.func.NewDeck; 
            deckButtons[i].owner = this;
            deckButtons[j].text.transform.localPosition = new Vector3(0, -0.125f);
            deckButtons[j].GetComponent<BoxCollider2D>().enabled = true;
        }
    }   


    public void NewDeck()
    {

    }

    public void SelectDeck(int x)
    {
        SaveManager.Decklist list = board.saveData.decks[x];
        currClass = list.classType;
        SetClass(list.classType);


        currPage = 0;
        ShowCards(currPage);
        SetState(state.DeckEdit);
    }

    public void ShowCards(int page)
    {
        float classPages = Mathf.Ceil(classData.Count / 8f);
        int start = page * 8;
        List<Database.CardInfo> cards;
        if (start < classData.Count)
        {
            cards = classData;
        }
        else
        {
            start = (page - (int)classPages) * 8;
            cards = neutralData;
        }

        int ind = 0;
        for (int i=start;i<start+8;i++)
        {
            if (i >= cards.Count)
            {
                for (int x = ind;x<8;x++)
                {
                    pickers[x].transform.localScale = Vector3.zero;
                }
                break;
            }
            pickers[ind].Set(cards[i]);
            ind++;
        }
    }
    public void ChangePage(int i=0)
    {
        currPage = Mathf.Max(currPage + i, 0);
        ShowCards(currPage);
    }

}
