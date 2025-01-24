using System;
using System.Collections.Generic;
using UnityEngine;

public partial class Board
{
    [Serializable]
    public partial class Minion
    {
        public Card.Cardname card;

        public int index;

        public int damage;
        public int health;
        public int maxHealth;

        public int baseHealth;
        public int baseDamage;

        public bool canAttack = false;

        public bool STEALTH = false;
        public bool WINDFURY = false;

        public bool DEAD = false;

        public List<Aura> auras = new List<Aura>();
        public List<Trigger> triggers = new List<Trigger>();

        public int previewIndex = -1;
        public int playOrder = 0;

        [System.NonSerialized]
        public MinionBoard board;

        public void Set(Card.Cardname name, int ind)
        {
            //transform into another minion
            card = name;
            index = ind;
            //manaCost
        }
        public Minion(Card.Cardname c, int ind,MinionBoard _board)
        {
            board = _board;
            card = c;
            health = 3;
            damage = 1;
            index = ind;

            maxHealth = health;
            baseHealth = maxHealth;
            baseDamage = damage;

            if (c==Card.Cardname.ShieldBearer)
            {
               AddAura(new Aura(Aura.Type.Taunt));
            }        
            if (c==Card.Cardname.SWChamp)
            {
               AddAura(new Aura(Aura.Type.StormwindChampion));
            }
            if (c==Card.Cardname.DireWolf)
            {
               AddAura(new Aura(Aura.Type.DireWolfAlpha));
            }
            if (c==Card.Cardname.KnifeJuggler)
            {
                AddTrigger(Trigger.Type.AfterSummonMinion, Trigger.Side.Friendly, Trigger.Ability.KnifeJuggler);
                AddTrigger(Trigger.Type.AfterPlayMinion, Trigger.Side.Friendly, Trigger.Ability.KnifeJuggler);
            }
            if (c==Card.Cardname.Acolyte)
            {
                AddTrigger(Trigger.Type.OnMinionDeath, Trigger.Side.Both, Trigger.Ability.AcolyteOfPain);
            }
            if (c==Card.Cardname.YoungPri)
            {
                AddTrigger(Trigger.Type.Deathrattle, Trigger.Side.Both, Trigger.Ability.YoungPriestess);
            }

            if (c==Card.Cardname.HarvestGolem)
            {
                AddTrigger(Trigger.Type.Deathrattle, Trigger.Side.Both, Trigger.Ability.HarvestGolem);
            }
        }
        public override string ToString()
        {
            return card.ToString();
        }
        
        public List<Trigger> CheckTriggers(Trigger.Type type, Trigger.Side side, Server.CastInfo spell)
        {
            List<Trigger> result = new List<Trigger>();
            foreach (Trigger t in triggers)
            {
                if (t.CheckTrigger(type,side,spell)) result.Add(t);
            }
            return result;
        }
        public void AddTrigger(Trigger.Type type, Trigger.Side side, Trigger.Ability ability)
        {
            Trigger t = new Trigger(type, side, ability, this, playOrder);
            triggers.Add(t);
        }
        public void RemoveTrigger(Trigger t)
        {
            triggers.Remove(t);
        }
        public void AddAura(Aura a)
        {
            Aura finder = FindAura(a.type);
            if (finder != null)
            {
                if (finder.stackable == false)
                    return;
            }

            finder = FindForeignAura(a);
            if (finder != null)
            {
                if (finder.foreignSource && finder.source == a.source)
                {
                    //Refresh and don't re-add.
                    finder.refreshed = true;
                    return;
                }
            }

            if (finder == null && a.foreignSource)
            {
                //First time application of foreign aura. Add and considered it refreshed.
                a.refreshed = true;
            }
            
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
        
        public Aura FindForeignAura(Aura a)
        {
            if (a.foreignSource == false) return null;

            foreach (Aura x in auras)
            {
                if (a.type == x.type && x.foreignSource && a.source==x.source) 
                    return x;
            }
            return null;
        }

        public void RemoveAura(Aura a)
        {
            auras.Remove(a);
            Debug.Log("removing aura " + a.type);
            switch(a.type)
            {
                case Aura.Type.Health:
                    maxHealth -= a.value;
                    if (health > maxHealth)
                        health = maxHealth;
                    break;
                case Aura.Type.Damage:
                    damage -= a.value;
                    break;
                case Aura.Type.Taunt:
                    if (board.server == false)
                    {
                        board.minionObjects[this].DisableTaunt();
                    }
                    break;
            }
        }

        public void RemoveAura(Aura.Type t)
        {
            while (FindAura(t)!=null)
            {
                RemoveAura(FindAura(t));
            }
        }

        public void RefreshForeignAuras()
        {
            List<Aura> removeList = new List<Aura>();
            foreach (var aura in auras)
            {
                if (aura.foreignSource && aura.refreshed == false)
                    removeList.Add(aura);
                else if (aura.foreignSource && aura.refreshed == true)
                {
                    aura.refreshed = false;
                }
            }
            foreach (var aura in removeList)
                RemoveAura(aura);
        }
        public void RemoveTemporaryAuras()
        {
            List<Aura> removeList = new List<Aura>();
            foreach (var aura in auras)
            {
                if (aura.temporary)
                    removeList.Add(aura);

            }
            foreach (var aura in removeList)
                RemoveAura(aura);
        }
        public class Aura
        {
            public enum Type
            {
                Health,
                Damage,
                Charge,
                Taunt,
                Shield,
                Stealth,
                Windfury,
                NoAttack,
                
                StormwindChampion,
                DireWolfAlpha,
            }

            public Type type = Type.Health;
            public bool temporary = false;
            public bool foreignSource = false;
            public bool trigger = false;
            public bool stackable = false;
            public ushort value = 0;
            public Minion minion;
            public Minion source;
            public bool refreshed = false;
            
            public void InitAura()
            {
                switch (type)
                {
                    case Type.Health:
                        minion.health += value;
                        minion.maxHealth += value;
                        break;
                    case Type.Damage:
                        minion.damage += value;
                        break;
                    case Type.NoAttack:
                        minion.canAttack = false;
                        break;
                    case Type.Taunt:
                        if (minion.board.server==false)
                        {
                            if (minion.board.minionObjects.ContainsKey(minion))
                            {
                                minion.board.minionObjects[minion].EnableTaunt();
                            }
                        }
                        break;
                }
            }

            public void ActivateAura(Server.Match match)
            {
                switch (type)
                {
                    case Type.StormwindChampion:
                        AuraEffects.StormwindChampion(match,this.minion);
                        break;
                    case Type.DireWolfAlpha:
                        AuraEffects.DireWolfAlpha(match,this.minion);
                        break;
                }
            }

            public Aura(Type t, ushort val=0, bool temp = false, bool foreign = false, Minion provider = null)
            {
                type = t;
                value = val;
                temporary = temp;
                foreignSource = foreign;
                source = provider;

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
