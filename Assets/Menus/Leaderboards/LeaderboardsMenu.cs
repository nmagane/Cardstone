using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeaderboardsMenu : MonoBehaviour
{
    public GameObject entryObj;

    List<LeaderboardEntry> entries = new();
    public void ClearList()
    {
        foreach (var x in entries)
            Destroy(x.gameObject);
        entries.Clear();
    }

    public GameObject statsScreen;
    public GameObject expandedScreen;

    public void DisplayStatsScreen(string s)
    {
        ClearList();
        Server.LeaderboardStatView stats = JsonUtility.FromJson<Server.LeaderboardStatView>(s);
        
        for (int i=0;i<stats.playerIDs.Count;i++)
        {
            Debug.Log($"{stats.names[i]} ({stats.playerIDs[i]}): {stats.wins[i]}-{stats.losses[i]} ({(float)stats.wins[i] / (stats.losses[i] + stats.wins[i]) * 100}%)");
            LeaderboardEntry entry = Instantiate(entryObj).GetComponent<LeaderboardEntry>();
            entry.menu = this;
            entry.Set(stats.names[i], i, stats.wins[i], stats.losses[i], stats.playerIDs[i]);
            entry.transform.parent = statsScreen.transform;
            entry.transform.localPosition = new Vector3(0, 7 - i * 1.75f);
        }
    }

    public void ExpandPlayerStats(ulong playerID)
    {
        Board.Instance.RequestPlayerStats(playerID);
        statsScreen.transform.localScale = Vector3.zero;
        expandedScreen.transform.localScale = Vector3.one;
    }

    public List<LeaderboardClassEntry> classEntries;
    public TMP_Text profileName;
    public TMP_Text profileTotalStats;
    public Server.LeaderboardPlayer currData = null;
    public void ShowPlayerProfile(string playerJson)
    {
        Server.LeaderboardPlayer player = JsonUtility.FromJson<Server.LeaderboardPlayer>(playerJson);
        currData = player;
        profileName.text = player.playerName;
        int wins = player.wins;
        int losses = player.losses;
        int winrate = (int)(100 * (float)(wins) / (wins + losses));
        if (wins + losses == 0) winrate = 0;
        string color = (winrate >= 50) ? "#D6F264" : "#F5A097";
        profileTotalStats.text = $"Total Games:  {(wins + losses).ToString("00")}           <color=#D6F264>{wins.ToString("00")} W</color> - <color=#F5A097>{losses.ToString("00")} L</color>       <color={color}>{winrate.ToString("00")}%</color>";

        for (int i=1;i<player.classStats.Count;i++)
        {
            if (player.classStats[i].className>Card.Class.Druid)
            {
                Debug.LogWarning("Remaining classes unimplemented.");
                break;
            }
            classEntries[i].Set(player.classStats[i]);
        }
    }

    public void ExpandClassStats(int c)
    {

    }
}
