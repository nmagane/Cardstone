using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using System.Linq;
using System.IO;
using UnityEditor;

public class SaveManager : MonoBehaviour
{
    public Board board;
    [System.Serializable]
    public class Decklist
    {
        public string deckName = "Deck";
        public Card.Class classType = Card.Class.Warlock;
        public List<Card.Cardname> cards = new List<Card.Cardname>();

        public Decklist(string name, Card.Class hero, List<Card.Cardname> list)
        {
            deckName = name;
            classType = hero;
            cards = new List<Card.Cardname>(list); 
        }
    }

    [System.Serializable]
    public class GameSave
    {
        public string playerName = "";
        public List<Decklist> decks = new List<Decklist>();
        public int selectedDeck = 0;
        public GameSave()
        {
            playerName = "";
        }
    }

    public GameSave saveData;


    public void LoadGame()
    {
        string saveDir = GetSaveDir();

        string dir = saveDir + "/save.json";
        string videoDir = saveDir + "/video.prefs";
        if (File.Exists(dir))
        {
            try //try to load json
            {
                string jsonText = File.ReadAllText(dir);
                saveData = JsonUtility.FromJson<GameSave>(jsonText);
            }
            catch //save corrupt, cant load json
            {
                Debug.Log("Error loading game save");
                FileStream file = File.Create(dir);
                file.Close();
                saveData = new GameSave();

                string jsonText = JsonUtility.ToJson(saveData);

                File.WriteAllText(dir, jsonText);
            }
        }
        else //no save present, create new one
        {
            FileStream file = File.Create(dir);
            file.Close();
            saveData = new GameSave();

            string jsonText = JsonUtility.ToJson(saveData);
            File.WriteAllText(dir, jsonText);
        }

        if (saveData.decks.Count==0)
        {
            saveData.decks.Add(new Decklist("ZOO", Card.Class.Warlock, Database.Zoo_Lock));
            saveData.decks.Add(new Decklist("OIL", Card.Class.Rogue, Database.Oil_Rogue));
            saveData.decks.Add(new Decklist("PATRON", Card.Class.Warrior, Database.Patron_Warrior));
            saveData.decks.Add(new Decklist("FREEZE", Card.Class.Mage, Database.Freeze_Mage));
            saveData.decks.Add(new Decklist("MALYGOS", Card.Class.Warlock, Database.Malygos_Lock));
            saveData.decks.Add(new Decklist("COMBO", Card.Class.Druid, Database.Combo_Druid));
        }

        List<Decklist> decklists = new List<Decklist>(saveData.decks);

        foreach(Decklist list in decklists)
        {
            if (CheckValidDeck(list) == false)
            {
                Debug.LogError(list.deckName+" invalid");
                saveData.decks.Remove(list);
            }
        }

        SelectDeck(saveData.selectedDeck);
        board.playerName = saveData.playerName;
    }

    public void SelectDeck(int index)
    {
        saveData.selectedDeck = Mathf.Clamp(index, 0, saveData.decks.Count - 1);
        board.currDecklist = saveData.decks[saveData.selectedDeck];
        SaveGame();
    }

    public void SaveGame()
    {
        string saveDir = GetSaveDir();

        string dir = saveDir + "/save.json";
        string jsonText = JsonUtility.ToJson(saveData);
#if (UNITY_WEBGL)
        PlayerPrefs.SetString("save", jsonText);
#endif
        //Debug.Log("Saved+ "+jsonText);
        File.WriteAllText(dir, jsonText);

    }

    public string GetSaveDir()
    {
        string saveDir;

        saveDir = Application.dataPath + "/Userdata/";

#if UNITY_EDITOR
        if (board.playerID == 101) saveDir = Application.dataPath + "/Userdata101/";
#endif

        if (!Directory.Exists(saveDir))
        {
            Directory.CreateDirectory(saveDir);
        }
        return saveDir;
    }


    public static bool CheckValidDeck(Decklist deck)
    {
        Card.Class hero = deck.classType;
        Dictionary<Card.Cardname, int> cardCount = new Dictionary<Card.Cardname, int>();
        if (deck.cards.Count != 30) return false;
        foreach (Card.Cardname c in deck.cards)
        {
            Database.CardInfo card = Database.GetCardData(c);

            if (cardCount.ContainsKey(c) == false) cardCount.Add(c, 1);
            else cardCount[c]++;

            if (card.classType != Card.Class.Neutral && card.classType != hero)
            {
                Debug.LogError("classwrong " + card.name);
                return false;
            }
            if (card.TOKEN)
            {
                Debug.LogError("token "+card);
                return false;
            }
            if (card.LEGENDARY && cardCount[c] > 1)
            {

                Debug.LogError("legendoubl " + card);
                return false;
            }
            if (card.LEGENDARY == false && cardCount[c] > 2)
            {

                Debug.LogError("tripl " + card);
                return false;
            }
        }
        return true;
    }


    void Awake()
    {
        LoadGame();
    }


}