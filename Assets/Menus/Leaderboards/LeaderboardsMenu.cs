using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardsMenu : MonoBehaviour
{
    public void DisplayStatsScreen(string s)
    {
        Debug.Log(s);
        Server.LeaderboardStatView stats = JsonUtility.FromJson<Server.LeaderboardStatView>(s);
        
        for (int i=0;i<stats.playerIDs.Count;i++)
        {
            Debug.Log($"{stats.names[i]} ({stats.playerIDs[i]}): {stats.wins[i]}-{stats.losses[i]} ({(float)stats.wins[i] / (stats.losses[i] + stats.wins[i]) * 100}%)");
        }
    }
}
