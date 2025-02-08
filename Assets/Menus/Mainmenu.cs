using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Mainmenu : MonoBehaviour
{
    public Board board;
    public TMP_InputField textbox;
    public void Start()
    {

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

}
