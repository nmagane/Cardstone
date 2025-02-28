using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeaderboardEntry : MonoBehaviour
{

    public LeaderboardsMenu menu;
    public TMP_Text nameText;
    public TMP_Text statsText;
    ulong playerID = 0;
    public void Set(string name, int index, int wins, int losses, ulong id)
    {
        playerID = id;
        nameText.text = $"[{index}] {name}";
        int winrate = (int)(100*(float)(wins) / (wins + losses));
        string color = (winrate >= 50) ? "#D6F264" : "#F5A097";
        statsText.text = $"<color=#D6F264>{wins.ToString("00")} W</color> - <color=#F5A097>{losses.ToString("00")} L</color>    <color={color}>{winrate.ToString("00")}%</color>";
    }

    public void Expand()
    {
        menu.ExpandPlayerStats(playerID);
    }
}
