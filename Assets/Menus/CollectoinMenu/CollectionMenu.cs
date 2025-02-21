using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using TMPro;

public class CollectionMenu : MonoBehaviour
{

    public GameObject listObject;

    public Board board;

    public List<Database.CardInfo> cardData = new();
    public List<Database.CardInfo> classData = new();
    public List<Database.CardInfo> neutralData = new();
    public List<CardPicker> pickers;
    public List<UIButton> deckButtons;
    public GameObject deckButtonAnchor;
    public GameObject cardPickerAnchor;
    public GameObject classPickerAnchor;
    public GameObject listAnchor;
    public SpriteRenderer cardpickerBG;
    public Card.Class currClass;
    public int currPage = 0;
    public string currName = "NEW DECK";
    public TMP_Text headerText;
    public TMP_InputField nameBox;

    public UIButton confirmButton;
    public UIButton deleteButton;
    public UIButton nextPageButton;
    public UIButton prevPageButton;

    state currState = state.DeckSelect;
    enum state
    {
        DeckSelect,
        NewSelect,
        DeckEdit,
    }
    public void Start()
    {
        //GetData();

        foreach (CardPicker p in pickers)
            p.cardObject.board = board;

        SetState(state.DeckSelect);
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow)||Input.GetKeyDown(KeyCode.D))
        {
            ChangePage(1);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow)||Input.GetKeyDown(KeyCode.A))
        {
            ChangePage(-1);
        }
    }
    void SetState(state x)
    {
        currState = x;

        cardPickerAnchor.transform.localScale = Vector3.zero;
        deckButtonAnchor.transform.localScale = Vector3.zero;
        confirmButton.transform.localScale = Vector3.zero;
        deleteButton.transform.localScale = Vector3.zero;
        classPickerAnchor.transform.localScale = Vector3.zero;
        nextPageButton.transform.localScale = Vector3.zero;
        prevPageButton.transform.localScale = Vector3.zero;
        prevPageButton.transform.localScale = Vector3.zero; 
        nameBox.transform.localScale = Vector3.zero;
        cardpickerBG.enabled = false;
        switch (x)
        {
            case state.DeckSelect:
                deckButtonAnchor.transform.localScale = Vector3.one;
                headerText.text = "CHOOSE DECK";
                InitDecks();
                break;
            case state.NewSelect:
                headerText.text = "CHOOSE CLASS";
                classPickerAnchor.transform.localScale = Vector3.one;
                break;
            case state.DeckEdit:
                cardPickerAnchor.transform.localScale = Vector3.one;
                confirmButton.transform.localScale = Vector3.one;
                deleteButton.transform.localScale = Vector3.one;
                nextPageButton.transform.localScale = Vector3.one;
                prevPageButton.transform.localScale = Vector3.one;
                nameBox.transform.localScale = Vector3.one;
                cardpickerBG.enabled = true; 
                headerText.text = "EDIT DECK";
                currPage = 0;
                ShowCards(0);
                CheckDeck();
                break;
        }
    }


    public void GetData()
    {
        cardData.Clear();
        neutralData.Clear();
        List<Card.Cardname> secretCards = new List<Card.Cardname>(){ Card.Cardname.Malygos,Card.Cardname.Blackwing_Technician,Card.Cardname.Blackwing_Corruptor};
        for (int i = 1; i < (int)Card.Cardname._COUNT; i++)
        {
            if (secretCards.Contains((Card.Cardname)i) && board.saveData.secret==false)
            {
                continue;
            }
            var c = Database.GetCardData((Card.Cardname)i);
            if (c == null) continue;
            if (c.TOKEN) continue;
            c.cardname = (Card.Cardname)i;
            cardData.Add(c);
        }

        cardData.Sort((x, y) => x.manaCost.CompareTo(y.manaCost));

        foreach (var c in cardData)
        {
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
            deckButtons[j].owner = this;
            deckButtons[j].text.transform.localPosition = new Vector3(0, -0.125f);
            deckButtons[j].GetComponent<BoxCollider2D>().enabled = true;
        }
    }   

    public void SaveDeck()
    {
        if (currDeckslot < board.saveData.decks.Count)
        {
            board.saveData.decks[currDeckslot].deckName = currName;
            board.saveData.decks[currDeckslot].cards = new List<Card.Cardname>(cards);
            board.saveData.decks[currDeckslot].classType = currClass;
        }
        else
        {
            board.saveData.decks.Add(new SaveManager.Decklist(currName, currClass, cards));
        }

        board.saveManager.SaveGame();
        Back();
    }
    public void NewDeck()
    {
        if (board.saveData.decks.Count >= 8) return;
        currDeckslot = board.saveData.decks.Count;
        SetState(state.NewSelect);
    }
    public void SelectNewClass(int x)
    {
        Card.Class newClass = (Card.Class)x;
        SetClass(newClass);
        nameBox.SetTextWithoutNotify("");
        currName = "NEW DECK";
        SetState(state.DeckEdit);
    }
    public void Back()
    {
        if (currState==state.DeckEdit||currState == state.NewSelect)
        {
            foreach (var x in listings)
                Destroy(x.Value.gameObject);
            listings.Clear();
            cards.Clear();

            SetState(state.DeckSelect);
        }
        else
        {
            board.mainmenu.InitDecks();
            Camera.main.transform.localPosition = new Vector3(-40, 0, -10);
        }
    }
    Dictionary<Card.Cardname, ListCard> listings = new();

    public void RemoveListing(Card.Cardname c)
    {
        if (cards.Contains(c) == false) return;
        if (listings.ContainsKey(c) == false) return;

        cards.Remove(c);

        ListCard listing = listings[c];
        listing.count--;

        if (listing.count == 0)
        {
            listings.Remove(c);
            Destroy(listing.gameObject);
            SortListings();
        }
        else
            listing.SetCount(listing.count);

        CheckDeck();
    }

    public void SortListings()
    {
        List<ListCard> list = new(listings.Values);
        list.Sort((x, y) => x.manaCost.CompareTo(y.manaCost));
        int i = 0;
        foreach (var x in list)
        {
            x.transform.localPosition = new Vector3(0, -0.875f * i++);
        }
        listAnchor.transform.localScale = Vector3.one;
        if (listings.Count >= 16)
            listAnchor.transform.localScale = Vector3.one * 0.95f;
        if (listings.Count > 17)
            listAnchor.transform.localScale = Vector3.one * 0.9f;
        if (listings.Count > 18)
            listAnchor.transform.localScale = Vector3.one * 0.85f;
        if (listings.Count > 19)
            listAnchor.transform.localScale = Vector3.one * 0.8f;
        if (listings.Count > 20)
            listAnchor.transform.localScale = Vector3.one * 0.775f;
        if (listings.Count > 21)
            listAnchor.transform.localScale = Vector3.one * 0.7f;
        if (listings.Count > 22)
            listAnchor.transform.localScale = Vector3.one * 0.68f;
        if (listings.Count > 25)
            listAnchor.transform.localScale = Vector3.one * 0.6f;
        if (listings.Count > 27)
            listAnchor.transform.localScale = Vector3.one * 0.55f;
    }
    public List<Card.Cardname> cards = new List<Card.Cardname>();
    public bool AddListing(Card.Cardname c)
    {
        if (cards.Count == 30) return false;

        if (listings.ContainsKey(c))
        {
            ListCard l = listings[c];
            if (l.legendary) return false;
            if (l.count >= 2) return false;
            l.count++;
            l.SetCount(l.count);
            cards.Add(c);
        }
        else
        {
            ListCard newListing = Instantiate(listObject).GetComponent<ListCard>();
            newListing.Set(Database.GetCardData(c));
            newListing.card = c;
            newListing.transform.parent = listAnchor.transform;
            newListing.transform.localScale = Vector3.one;
            newListing.transform.localPosition = new Vector3(0, -0.875f * listings.Count);
            newListing.menu = this;

            newListing.SetCount(1);
            cards.Add(c);
            listings.Add(c, newListing);
            SortListings();
        }
        CheckDeck();
        return true;
    }
    int currDeckslot = 0;
    public void SelectDeck(int x)
    {
        SaveManager.Decklist list = board.saveData.decks[x];
        currDeckslot = x;
        currClass = list.classType;
        currName = list.deckName;
        nameBox.SetTextWithoutNotify(currName);
        SetClass(list.classType);

        foreach(Card.Cardname c in list.cards)
        {
            AddListing(c);
        }

        SetState(state.DeckEdit);
    }
    public void DeleteDeck()
    {
        if (board.saveData.decks.Count<=currDeckslot)
        {
            Back();
            return;
        }
        board.saveData.decks.RemoveAt(currDeckslot);
        board.saveManager.SaveGame();
        Back();
    }

    public void SetName()
    {
        currName = nameBox.text; 
        if (board.saveData.decks.Count <= currDeckslot)
        {
            return;
        }

        board.saveData.decks[currDeckslot].deckName = currName;
        board.saveManager.SaveGame();
    }

    int maxPages = 0;
    public void ShowCards(int page)
    {
        float classPages = Mathf.Ceil(classData.Count / 8f);
        maxPages = (int)classPages + (int)Mathf.Ceil(neutralData.Count / 8f) - 1;

        nextPageButton.transform.localScale = page == maxPages? Vector3.zero : (nextPageButton.transform.localScale==Vector3.zero? Vector3.one:nextPageButton.transform.localScale);
        prevPageButton.transform.localScale = page == 0? Vector3.zero: (prevPageButton.transform.localScale == Vector3.zero ? Vector3.one : prevPageButton.transform.localScale);


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
        StartCoroutine(BounceCards());
    }
    IEnumerator BounceCards()
    {
        foreach(var x in pickers)
        {
            x.transform.localPosition += new Vector3(0, 0.1f);
        }
        yield return null;
        yield return null;
        foreach(var x in pickers)
        {
            x.transform.localPosition += new Vector3(0, -0.05f);
        }
        yield return null;
        yield return null;
        foreach(var x in pickers)
        {
            x.transform.localPosition += new Vector3(0, -0.05f);
        }
    }
    public void ChangePage(int i=0)
    {
        currPage = Mathf.Clamp(currPage + i, 0,maxPages);
        ShowCards(currPage);
    }

    void CheckDeck()
    {
        if (cards.Count==30)
        {
            confirmButton.locked = false;
            confirmButton.text.text = "CONFIRM";
            confirmButton.SetColor(Board.GetColor("4A5462"));
        }
        else
        {
            confirmButton.locked = true;
            confirmButton.text.text = $"{cards.Count}/30";
            confirmButton.SetColor(Board.GetColor("B4202A"));
        }
    }

}
