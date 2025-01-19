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
        UpdateDisplay();
    }

    public void SetMax(int x)
    {
        max = x;
        UpdateDisplay();
    }
    public void SetCurrent(int x)
    {
        curr = x;
        UpdateDisplay();
    }

    public void Gain(int x)
    {
        curr += x;
        UpdateDisplay();
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

    public void UpdateDisplay()
    {
        text.text = "MANA:" + curr + "/" + max;
    }
    void Update()
    {
        
    }
}
