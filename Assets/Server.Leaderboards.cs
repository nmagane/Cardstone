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
        public List<int> matchupWins = new();
        public List<int> matchupLosses = new();

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
        public string playerName;
        public int wins;
        public int losses;
        public List<LeaderboardClass> classStats = new();

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

    [System.Serializable]
    public class LeaderboardStatView
    {
        public List<ulong> playerIDs = new();
        public List<string> names = new();
        public List<int> wins = new();
        public List<int> losses = new();
    }

    public void RecordGame(PlayerConnection winner, PlayerConnection loser)
    {
        UpdatePlayerStats(winner, loser, true);
        UpdatePlayerStats(loser, winner, false);
    }
    public void UpdatePlayerStats(PlayerConnection playerInfo, PlayerConnection opponentInfo, bool win)
    {
        LeaderboardPlayer player = GetPlayerData(playerInfo.playerID);
        player.playerName = playerInfo.name;

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

    public void RequestStatsScreen(int clientID)
    {
        LeaderboardStatView stats = GetStatView();
        string s = JsonUtility.ToJson(stats);
        CustomMessage message = CreateMessage(MessageType.RequestStatsScreen);

        message.AddString(s);
        Debug.Log("send stats to player: "+s);
        SendMessageClientID(message, clientID);
    }

    public LeaderboardStatView GetStatView()
    {
        LeaderboardStatView stats = new LeaderboardStatView();
        string saveDir = GetSaveDir();

        string[] files = Directory.GetFiles(saveDir);

        foreach (string dir in files)
        {
            if (dir.Contains(".meta")) continue;
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
                    continue;
                }
            }
            else //no save present
            {
                continue;
            }
            stats.playerIDs.Add(playerData.playerID);
            stats.names.Add(playerData.playerName);
            stats.wins.Add(playerData.wins);
            stats.losses.Add(playerData.losses);
        }

        return stats;
    }
}
