using System.Collections.Generic;
using System.IO;
using Mirror.BouncyCastle.Utilities;
using UnityEngine;
public partial class Server
{
    [System.Serializable]
    public class LeaderboardClass
    {
        public Card.Class className;
        public int wins;
        public int losses;
        public List<int> matchupWins;
        public List<int> matchupLosses;

        public LeaderboardClass(Card.Class hero)
        {
            className = hero;
            wins = 0;
            losses = 0;

            for (int i = 0; i < 10; i++)
            {
                matchupWins.Add(0);
                matchupLosses.Add(0);
            }
        }
    }
    [System.Serializable]
    public class LeaderboardPlayer
    {
        public ulong playerID;
        public int wins;
        public int losses;
        public List<LeaderboardClass> classStats;

        public LeaderboardPlayer(ulong id)
        {
            playerID = id;
            wins = 0;
            losses = 0;
            for (int i=0;i<10;i++)
            {
                classStats.Add(new LeaderboardClass((Card.Class)i));
            }
        }
    }
    public void RecordGame(PlayerConnection winner, PlayerConnection loser)
    {
        UpdatePlayerStats(winner, loser, true);
        UpdatePlayerStats(loser, winner, false);
    }
    public void UpdatePlayerStats(PlayerConnection playerInfo, PlayerConnection opponentInfo, bool win)
    {
        LeaderboardPlayer player = GetPlayerData(playerInfo.playerID);

        if (win)
        {
            player.wins++;
            player.classStats[(int)playerInfo.classType].wins++;
            player.classStats[(int)playerInfo.classType].matchupWins[(int)opponentInfo.classType]++;
        }
        else
        {
            player.losses++;
            player.classStats[(int)playerInfo.classType].losses++;
            player.classStats[(int)playerInfo.classType].matchupLosses[(int)opponentInfo.classType]++;
        }

        SavePlayerData(player);
    }

    public LeaderboardPlayer GetPlayerData(ulong playerID)
    {
        string saveDir = GetSaveDir();
        string dir = saveDir + "/"+playerID+".json";
        LeaderboardPlayer playerData = null;
        if (File.Exists(dir))
        {
            try //try to load json
            {
                string jsonText = File.ReadAllText(dir);
                playerData = JsonUtility.FromJson<LeaderboardPlayer>(jsonText);
            }
            catch //save corrupt, cant load json
            {
                Debug.Log("Error loading game save");
                FileStream file = File.Create(dir);
                file.Close();
                playerData = new LeaderboardPlayer(playerID);

                string jsonText = JsonUtility.ToJson(playerData);

                File.WriteAllText(dir, jsonText);
            }
        }
        else //no save present, create new one
        {
            FileStream file = File.Create(dir);
            file.Close();
            playerData = new LeaderboardPlayer(playerID);

            string jsonText = JsonUtility.ToJson(playerData);
            File.WriteAllText(dir, jsonText);
        }

        return playerData;
    }

    public string GetSaveDir()
    {
        string saveDir;

        saveDir = Application.dataPath + "/Leaderboards/";

        if (!Directory.Exists(saveDir))
        {
            Directory.CreateDirectory(saveDir);
        }
        return saveDir;
    }

    public void SavePlayerData(LeaderboardPlayer playerData)
    {
        string saveDir = GetSaveDir();

        string dir = saveDir + "/" + playerData.playerID + ".json";
        string jsonText = JsonUtility.ToJson(playerData);

        File.WriteAllText(dir, jsonText);
    }
}
