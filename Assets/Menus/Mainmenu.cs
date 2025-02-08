using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Mainmenu : MonoBehaviour
{
    public Board board;
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
        //if (board.mirror)  TODO: dont queue if not connected?
        SetPlayerID();

        textbox.enabled = false;
        board.StartMatchmaking();
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
}
