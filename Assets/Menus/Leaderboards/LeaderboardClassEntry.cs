using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeaderboardClassEntry : MonoBehaviour
{
    public TMP_Text text;
    public TMP_Text classNameText;
    public void Set(Server.LeaderboardClass data)
    {
        int wins = data.wins;
        int losses = data.losses;
        int winrate = (int)(100 * (float)(wins) / (wins + losses));
        if (wins + losses == 0) winrate = 0;
        string color = (winrate >= 50) ? "#D6F264" : "#F5A097";
        classNameText.text = data.className.ToString()+":";
        text.text = $"{(wins+losses).ToString("00")}      <color=#D6F264>{wins.ToString("00")}</color> - <color=#F5A097>{losses.ToString("00")}</color>     <color={color}>{winrate.ToString("00")}%</color>";
    }
    public void SetMatchup(Card.Class opponent, int wins, int losses)
    {
        int winrate = (int)(100 * (float)(wins) / (wins + losses));
        if (wins + losses == 0) winrate = 0;
        string color = (winrate >= 50) ? "#D6F264" : "#F5A097";
        classNameText.text = "vs. "+opponent.ToString()+":";
        text.text = $"{(wins+losses).ToString("00")}      <color=#D6F264>{wins.ToString("00")}</color> - <color=#F5A097>{losses.ToString("00")}</color>     <color={color}>{winrate.ToString("00")}%</color>";
    }
}
