using System;

public partial class Board
{
    [Serializable]
    public class HandCard
    {
        public int index = 0;
        public int manaCost = 1;
        public Card.Cardname card;

        public bool TARGETED = false;
        public bool SPELL = false;
        public bool MINION = false;
        public bool COMBO = false;

        public bool played = false;

        public void Set(Card.Cardname name, int ind)
        {
            card = name;
            index = ind;
            MINION = true;
            //manaCost
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

}
