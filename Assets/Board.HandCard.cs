using System;

public partial class Board
{
    [Serializable]
    public class HandCard
    {
        public int index = 0;
        public int manaCost = 1;
        public Card.Cardname card;
        public void Set(Card.Cardname name, int ind)
        {
            card = name;
            index = ind;
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
