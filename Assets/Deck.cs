using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Deck : MonoBehaviour
{
    int count = 30;
    public TMP_Text countText;
    public void Set(int x)
    {
        count = x;
    }
    public void UpdateDisplay(int cards=-1)
    {
        int x = cards == -1 ? count : cards;
        countText.text = x.ToString();
    }
    //TODO: DECK TRACKER ON HOVER
}
