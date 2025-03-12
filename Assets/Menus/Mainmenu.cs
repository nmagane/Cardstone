using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Mainmenu : MonoBehaviour
{
    public Board board;
    public bool inQueue = false;
    public TMP_InputField textbox;
    public CollectionMenu editorMenu;
    public LeaderboardsMenu statsMenu;
    public TMP_Text playerCount;
    public UIButton findMatchButton;
    public UIButton collectionButton;
    public UIButton resetButton;
    public void Start()
    {
        findMatchButton.board = resetButton.board = board;
        textbox.SetTextWithoutNotify(board.playerName);
        SetDeck(board.saveData.selectedDeck);
        InitDecks();
        SetPlayerID();
    }

    public Sprite[] deckSprites;

    public void InitDecks()
    {
        board.playerName = board.saveData.playerName;
        int i = 0;
        foreach(SaveManager.Decklist list in board.saveData.decks)
        {
            deckButtons[i].icon.sprite = deckSprites[(int)list.classType];
            deckButtons[i].text.text = list.deckName;
            deckButtons[i].GetComponent<BoxCollider2D>().enabled = true;

            i++;
            if (i > 14) break;
        }
        for (int j=i; j<15; j++)
        {
            deckButtons[j].icon.sprite = null;
            deckButtons[j].text.text = "";
            deckButtons[j].GetComponent<BoxCollider2D>().enabled = false;
        }
        if (board.saveData.decks.Count == 0)
        {
            findMatchButton.transform.localScale = Vector3.zero;
        }
        else
        {
            findMatchButton.transform.localScale = Vector3.one;
        }
    }

    public void SetPlayerID()
    {
        if (PlayerPrefs.HasKey("playerID"))
        {
            board.playerID = ulong.Parse(PlayerPrefs.GetString("playerID"));
        }
        else
        {
            System.DateTime time = System.DateTime.Now;
            long t = time.ToFileTimeUtc();
            board.playerID = (ulong)t;
            PlayerPrefs.SetString("playerID", board.playerID.ToString());
        }
        board.playerName = textbox.text;
        board.saveData.playerName = textbox.text;

        if (textbox.text == "")
        {
            board.playerName = "Player";
        }
        board.saveManager.SaveGame();
    }

    public void StartMatchmaking()
    {
        if (inQueue)
        {
            inQueue = false;
            board.LeaveMatchmaking();
            findMatchButton.text.text = "FIND MATCH";
            StartCoroutine(findMatchButton.bigBouncer());
            textbox.enabled = true;

            board.RequestPlayerCount();
            return;
        }
        //if (board.mirror)  TODO: dont queue if not connected?
        SetPlayerID();
        inQueue = true;
        textbox.enabled = false;
        findMatchButton.text.text = "IN QUEUE";
        board.StartMatchmaking(board.currDecklist);

        board.RequestPlayerCount();
    }


    public GameObject disconnectedContainer;
    public GameObject connectedContainer;
    Coroutine countCoroutine = null;
    public void ConfirmConnection()
    {
        findMatchButton.transform.localScale = Vector3.one;
        textbox.transform.localScale = Vector3.one;
        connectedContainer.transform.localScale = Vector3.one;
        disconnectedContainer.transform.localScale = Vector3.zero;

        if (countCoroutine ==null) countCoroutine = StartCoroutine(countChecker());
        
    }

    public void ConfirmDisconnect()
    {
        findMatchButton.transform.localScale = Vector3.zero;
        textbox.transform.localScale = Vector3.zero;
        connectedContainer.transform.localScale = Vector3.zero;
        disconnectedContainer.transform.localScale = Vector3.one;

        if (countCoroutine != null)
        {
            StopCoroutine(countCoroutine);
            countCoroutine = null;
        }
    }


    public int selectedDeck = 0;
    public List<UIButton> deckButtons;
    public void SetDeck(int x)
    {
        if (inQueue)
        {
            StartMatchmaking();
        }
        foreach (UIButton b in deckButtons)
        {
            b.SetColor(new Color(0.2901961f, 0.3294118f, 0.3843138f));
        }
        selectedDeck = x;
        board.saveManager.SelectDeck(x);
        deckButtons[x].SetColor(new Color(0.1019608f, 0.4784314f, 0.2431373f));

    }

    internal void EditDecks()
    {
        editorMenu.GetData();
        editorMenu.InitDecks();
        Camera.main.transform.position = new Vector3(0, 40, -10);
    }
    internal void Stats()
    {
        statsMenu.GetStats();
        Camera.main.transform.position = new Vector3(-40, 40, -10);
    }

    public void SetPlayerCount(int x)
    {
        playerCount.text = $"PLAYERS IN GAME: {x}";
    }
    IEnumerator countChecker()
    {
        while (true)
        {
            board.RequestPlayerCount();
            yield return Board.Wait(120); //request update every 2 seconds
        }
    }
}
