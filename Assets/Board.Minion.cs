using System;
using System.Collections.Generic;
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
        public int maxHealth;

        public int baseHealth;
        public int baseDamage;

        public bool canAttack = false;

        public bool TAUNT = false;
        public bool STEALTH = false;
        public bool WINDFURY = false;

        public List<Aura> auras = new List<Aura>();

        public int previewIndex = -1;

        public void Set(Card.Cardname name, int ind)
        {
            //transform into another minion
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

            maxHealth = health;
            baseHealth = maxHealth;
            baseDamage = damage;
        }
        public override string ToString()
        {
            return card.ToString();
        }
        
        public void AddAura(Aura a)
        {
            a.minion = this;
            a.InitAura();
            auras.Add(a);
            
        }
        public bool HasAura(Aura.Type t)
        {
            return (FindAura(t) != null);
        }
        public Aura FindAura(Aura.Type t)
        {
            foreach (Aura a in auras)
            {
                if (a.type == t) return a;
            }
            return null;
        }

        public class Aura
        {
            public enum Type
            {
                Health,
                Damage,
                Taunt,
                Shield,
                Stealth,
                Windfury,
                NoAttack,
                
            }

            public Type type = Type.Health;
            public bool temporary = false;
            public bool trigger = false;
            public bool stackable = false;
            public int value = 0;
            public Minion minion;
            
            public void InitAura()
            {
                switch (type)
                {
                    case Type.Health:
                        minion.health += value;
                        break;
                    case Type.Damage:
                        minion.damage += value;
                        break;
                    case Type.NoAttack:
                        minion.canAttack = false;
                        break;
                }
            }


            public Aura(Type t,int val, bool temp=false)
            {
                type = t;
                value = val;
                temporary = temp;

                switch (type)
                {
                    case Type.Health:
                        stackable = true;
                        break;
                    case Type.Damage:
                        stackable = true;
                        break;
                }
            }
        }

    }

}
