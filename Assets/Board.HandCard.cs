using System;

public partial class Board
{
    [Serializable]
    public class HandCard
    {
        public int index = 0;
        public int manaCost = 1;
        public int health = 3;
        public int damage = 1;
        public Card.Cardname card;
        public Board.EligibleTargets eligibleTargets = EligibleTargets.AllCharacters;

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
            MINION = true;
            //manaCost
            if (name==Card.Cardname.Ping)
            {
                MINION = false;
                SPELL = true;
                TARGETED = true;
            }
            if (name==Card.Cardname.ArcExplosion)
            {
                MINION = false;
                SPELL = true;
                TARGETED = false;
            }
            if (name==Card.Cardname.ShatteredSunCleric)
            {
                MINION = true;
                TARGETED = true;
                BATTLECRY = true;
                health = 2;
                damage = 3;
                eligibleTargets = EligibleTargets.FriendlyMinions;
            }
            if (name==Card.Cardname.Abusive || name==Card.Cardname.IronbeakOwl)
            {
                MINION = true;
                TARGETED = true;
                BATTLECRY = true;
                health = 2;
                damage = 3;
                eligibleTargets = EligibleTargets.AllMinions;
            }
            if (name==Card.Cardname.Argus)
            {
                MINION = true;
                TARGETED = false;
                BATTLECRY = true;
                health = 1;
                damage = 3;
            }
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
