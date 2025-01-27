
using UnityEngine;
[System.Serializable]
public class HandCard
{
    public int index = 0;
    public int manaCost = 1;
    public int health = 3;
    public int damage = 1;
    public Card.Cardname card;
    public Board.EligibleTargets eligibleTargets = Board.EligibleTargets.AllCharacters;

    public bool SPELL = false;
    public bool MINION = false;
    public bool SECRET = false;
    public bool WEAPON = false;

    public bool TARGETED = false;
    public bool COMBO = false;
    public bool BATTLECRY = false;

    public bool played = false;

    public void Set(Card.Cardname name, int ind)
    {
        card = name;
        index = ind;

        if (name == Card.Cardname.Cardback) return;

        Database.CardInfo cardInfo = Database.GetCardData(name);

        manaCost = cardInfo.manaCost;
        health = cardInfo.health;
        damage = cardInfo.damage;

        SPELL = cardInfo.SPELL;
        MINION = cardInfo.MINION;
        SECRET = cardInfo.SECRET;
        WEAPON = cardInfo.WEAPON;

        TARGETED = cardInfo.TARGETED;
        BATTLECRY = cardInfo.BATTLECRY;
        COMBO = cardInfo.COMBO;

        eligibleTargets = cardInfo.eligibleTargets;
    }

    public HandCard(Card.Cardname name, int ind)
    {
        Set(name, ind);
    }
    public override string ToString()
    {
        return card.ToString();
    }
}
