using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Creature : MonoBehaviour
{
    public TMP_Text testname;
    public TMP_Text heatlh, damage;
    public SpriteRenderer spriteRenderer;

    public Board board;

    public void Set(Board.Minion c)
    {
        testname.text = c.card.ToString();
        heatlh.text = c.health.ToString();
        damage.text = c.damage.ToString();
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
