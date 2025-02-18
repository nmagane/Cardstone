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

    public UIButton findMatchButton;
    public UIButton resetButton;
    public void Awake()
    {
        findMatchButton.board = resetButton.board = board;
        if (PlayerPrefs.HasKey("name"))
        {
            string s = PlayerPrefs.GetString("name");
            textbox.SetTextWithoutNotify(s);
            SetPlayerID();
        }
        if (PlayerPrefs.HasKey("deck"))
        {
            int d = PlayerPrefs.GetInt("deck");
            SetDeck(d);
        }
        else
        {
            SetDeck(0);
        }
    }

    public void SetPlayerID()
    {
        System.DateTime time = System.DateTime.Now;
        long t = time.ToFileTimeUtc();
        board.playerID = (ulong)t;
        board.playerName = textbox.text;
        PlayerPrefs.SetString("name", board.playerName);

        if (textbox.text == "")
        {
            board.playerName = "Player";
        }
    }

    public void StartMatchmaking()
    {
        if (inQueue)
        {
            inQueue = false;
            board.LeaveMatchmaking();
            findMatchButton.text.text = "FIND MATCH";
            StartCoroutine(findMatchButton.bigBouncer());
            return;
        }
        //if (board.mirror)  TODO: dont queue if not connected?
        SetPlayerID();
        inQueue = true;
        textbox.enabled = false;
        findMatchButton.text.text = "IN QUEUE";
        board.StartMatchmaking(board.currDecklist,board.currClass);
    }


    public GameObject disconnectedContainer;
    public GameObject connectedContainer;
    public void ConfirmConnection()
    {
        findMatchButton.transform.localScale = Vector3.one;
        connectedContainer.transform.localScale = Vector3.one;
        disconnectedContainer.transform.localScale = Vector3.zero;
    }

    public void ConfirmDisconnect()
    {
        findMatchButton.transform.localScale = Vector3.zero;
        connectedContainer.transform.localScale = Vector3.zero;
        disconnectedContainer.transform.localScale = Vector3.one;
    }

    public enum PresetDeck
    {
        Zoo,
        Oil,
        Patron,
        Freeze,
    }
    public PresetDeck selectedDeck = PresetDeck.Zoo;
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
        PresetDeck d = (PresetDeck)x;
        selectedDeck = d;
        deckButtons[x].SetColor(new Color(0.1019608f, 0.4784314f, 0.2431373f));
        PlayerPrefs.SetInt("deck", x);
        switch (d)
        {
            case PresetDeck.Zoo:
                board.currDecklist = Database.Zoo_Lock;
                board.currClass = Card.Class.Warlock;
                break;
            case PresetDeck.Oil:
                board.currDecklist = Database.Oil_Rogue;
                board.currClass = Card.Class.Rogue;
                break;
            case PresetDeck.Patron:
                board.currDecklist = Database.Patron_Warrior;
                board.currClass = Card.Class.Warrior;
                break;
            case PresetDeck.Freeze:
                board.currDecklist = Database.Freeze_Mage;
                board.currClass = Card.Class.Mage;
                break;
        }
    }
}
