using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Deck : MonoBehaviour
{
    int count = 30;
    public TMP_Text countText;
    public SpriteRenderer bg;
    public SpriteRenderer cardIcon;
    public TMP_Text fatigueText;
    public GameObject fatigueFrame;

    public void Set(int x)
    {
        count = x;
    }
    public void UpdateDisplay(int cards=-1)
    {
        int x = cards == -1 ? count : cards;
        countText.text = x.ToString();
        if (count == 0)
        {
            cardIcon.enabled = false;
            countText.color = Board.GetColor("FA6A0A");
            bg.color = Board.GetColor("B4202A");

            if (fatigueFrame.transform.localScale == Vector3.zero)
            {
                Board.Instance.animationManager.LerpZoom(fatigueFrame, Vector3.one * 0.67f, 5, 0.1f);
            }
        }
        else if (count<=5)
        {
            cardIcon.enabled = true;
            countText.color = Board.GetColor("FFD541");
            bg.color = Board.GetColor("D4A135");
        }
        else
        {
            cardIcon.enabled = true;
            countText.color = Board.GetColor("59C135");
            bg.color = Board.GetColor("1A7A3E");
        }
        
    }

    public void SetFatigue(int x)
    {
        if (fatigueFrame.transform.localScale==Vector3.zero)
        {
            Board.Instance.animationManager.LerpZoom(fatigueFrame, Vector3.one* 0.67f, 5, 0.1f);
        }

        fatigueText.text = x.ToString();
    }

    //TODO: DECK TRACKER ON HOVER
}
