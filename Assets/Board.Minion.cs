using System;
using Unity.Collections.LowLevel.Unsafe;

public partial class Board
{
    [Serializable]
    public class Minion
    {
        public Card.Cardname card;

        public int index;

        public int damage;
        public int health;

        public bool canAttack = false;

        public bool TAUNT = false;
        public bool STEALTH = false;
        public bool WINDFURY = false;

        public void Set(Card.Cardname name, int ind)
        {
            card = name;
            index = ind;
            //manaCost
        }
        public Minion(Card.Cardname c, int ind)
        {
            card = c;
            health = 3;
            damage = 1;
            index = ind;
        }
        public override string ToString()
        {
            return card.ToString();
        }
        
    }

}
