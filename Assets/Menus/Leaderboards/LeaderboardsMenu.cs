using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeaderboardsMenu : MonoBehaviour
{
    public GameObject entryObj;
    public TMP_Text header;

    List<LeaderboardEntry> entries = new();
    public void ClearList()
    {
        foreach (var x in entries)
            Destroy(x.gameObject);
        entries.Clear();
    }

    public GameObject statsScreen;
    public GameObject expandedScreen;
    public void GetStats()
    {
        Board.Instance.RequestStatScreen();
    }
    public void DisplayStatsScreen(string s)
    {
        ClearList();

        currState = state.Stats;
        statsScreen.transform.localScale = Vector3.one;
        expandedScreen.transform.localScale = Vector3.zero;
        header.text = "LEADERBOARD";

        Server.LeaderboardStatView stats = JsonUtility.FromJson<Server.LeaderboardStatView>(s);
        
        for (int i=0;i<stats.playerIDs.Count;i++)
        {
            if (stats.wins[i] + stats.losses[i] == 0) continue;
            //Debug.Log($"{stats.names[i]} ({stats.playerIDs[i]}): {stats.wins[i]}-{stats.losses[i]} ({(float)stats.wins[i] / (stats.losses[i] + stats.wins[i]) * 100}%)");
            LeaderboardEntry entry = Instantiate(entryObj).GetComponent<LeaderboardEntry>();
            entry.menu = this;
            entry.Set(stats.names[i], i, stats.wins[i], stats.losses[i], stats.playerIDs[i]);
            entry.transform.parent = statsScreen.transform;
            entry.transform.localPosition = new Vector3(0, 7 - i * 1.75f);
            entries.Add(entry);
        }
    }

    public void ExpandPlayerStats(ulong playerID)
    {
        Board.Instance.RequestPlayerStats(playerID);
        statsScreen.transform.localScale = Vector3.zero;
        expandedScreen.transform.localScale = Vector3.one;
        header.text = "PROFILE";
    }

    public List<LeaderboardClassEntry> classEntries;
    public List<LeaderboardClassEntry> matchupEntries;
    public TMP_Text profileName;
    public TMP_Text profileTotalStats;
    public TMP_Text profileMatchupStats;
    public GameObject profileBG;
    public GameObject matchupsBG;
    public Server.LeaderboardPlayer currData = null;

    public void SelfProfile()
    {
        ExpandPlayerStats(Board.Instance.playerID);
    }

    public void ShowPlayerProfile(string playerJson)
    {
        currState = state.Profile;
        Server.LeaderboardPlayer player = JsonUtility.FromJson<Server.LeaderboardPlayer>(playerJson);
        currData = player;
        profileName.text = player.playerName;
        int wins = player.wins;
        int losses = player.losses;
        int winrate = (int)(100 * (float)(wins) / (wins + losses));
        if (wins + losses == 0) winrate = 0;
        string color = (winrate >= 50) ? "#D6F264" : "#F5A097";
        profileTotalStats.text = $"Total Games:  {(wins + losses).ToString("00")}           <color=#D6F264>{wins.ToString("00")}</color> - <color=#F5A097>{losses.ToString("00")}</color>       <color={color}>{winrate.ToString("00")}%</color>";

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
        currState = state.Matchups;
        matchupsBG.transform.localScale = Vector3.one;
        profileBG.transform.localScale = Vector3.zero;

        Card.Class className = (Card.Class)c;
        int wins = currData.classStats[c].wins;
        int losses = currData.classStats[c].losses;
        int winrate = (int)(100 * (float)(wins) / (wins + losses));
        if (wins + losses == 0) winrate = 0;
        string color = (winrate >= 50) ? "#D6F264" : "#F5A097";
        profileMatchupStats.text = $"{currData.classStats[c].className.ToString()} Games:  {(wins + losses).ToString("00")}           <color=#D6F264>{wins.ToString("00")}</color> - <color=#F5A097>{losses.ToString("00")}</color>       <color={color}>{winrate.ToString("00")}%</color>";

        for (int i=1; i<currData.classStats[c].matchupWins.Count; i++)
        {
            if (currData.classStats[i].className > Card.Class.Druid)
            {
                Debug.LogWarning("Remaining classes unimplemented.");
                break;
            }
            matchupEntries[i].SetMatchup((Card.Class)i, currData.classStats[c].matchupWins[i], currData.classStats[c].matchupLosses[i]) ;
        }
    }
    enum state
    {
        Stats,
        Profile,
        Matchups,
    }
    state currState = state.Stats;
    public void Return()
    {
        if (currState==state.Stats)
        {
            Camera.main.transform.localPosition = new Vector3(-40, 0, -10);
        }
        if (currState == state.Profile)
        {
            currState = state.Stats;
            statsScreen.transform.localScale = Vector3.one;
            expandedScreen.transform.localScale = Vector3.zero;
            header.text = "LEADERBOARD";
        }
        if (currState == state.Matchups)
        {
            currState = state.Profile;
            profileBG.transform.localScale = Vector3.one;
            matchupsBG.transform.localScale = Vector3.zero;
        }
    }
}
