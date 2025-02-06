using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ManaBar : MonoBehaviour
{
    public int max = 0;
    public int curr = 0;
    public TMP_Text text;
    void Start()
    {
        
    }
    public void IncreaseMax(int x)
    {
        max += x;
    }

    public void SetMax(int x)
    {
        max = x;
    }
    public void SetCurrent(int x)
    {
        curr = x;
    }

    public void Gain(int x)
    {
        curr += x;
    }
    public void Spend(int x)
    {
        curr -= x;
        UpdateDisplay();
    }
    public void Refill()
    {
        curr = max;
        UpdateDisplay();
    }

    public void UpdateDisplay(int c=-1,int m=-1)
    {
        int x = c == -1 ? curr : c;
        int y = m == -1 ? max : m;
        text.text = "MANA: " + x + "/" + y;
    }
    void Update()
    {
        
    }
}
