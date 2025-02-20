using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionMenu : MonoBehaviour
{
    public List<Database.CardInfo> cardData;
    public List<Database.CardInfo> classData;
    public List<Database.CardInfo> neutralData;
    public List<CardPicker> pickers;

    

    public Card.Class currClass;

    public void GetData()
    {
        for (int i = 0; i < (int)Card.Cardname._COUNT; i++)
        {
            var c = Database.GetCardData((Card.Cardname)i);
            if (c == null) continue;
            if (c.TOKEN) continue;
            cardData.Add(c);
        }

        cardData.Sort((x, y) => y.manaCost.CompareTo(x.manaCost));

        foreach (var c in cardData)
            if (c.classType == Card.Class.Neutral) neutralData.Add(c);
    }

    public void SetClass(Card.Class hero)
    {
        classData.Clear();
        currClass = hero;

        foreach (var c in cardData)
            if (c.classType == hero) classData.Add(c);
    }

}
