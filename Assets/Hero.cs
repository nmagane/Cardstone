using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    public Board board;
    public SpriteRenderer spriteRenderer;
    public int health = 30;
    public int maxHealth = 30;

    public enum Class
    {
        Mage,
        Warrior,
        Warlock,
        Rogue,
        Druid,
        Hunter,
        Priest,
        Shaman,
        Paladin
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
