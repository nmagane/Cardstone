using UnityEngine;

public partial class Board
{
    public class Trigger
    {
        public static Type GetPhaseTrigger(Server.Match.Phase phase)
        {
            if ((int)phase >= (int)Type._PHASELIMIT || phase.ToString() != ((Type)(int)phase).ToString())
            {
                throw new System.Exception("NO TRIGGER FOUND FOR REQUESTED PHASE");
            }
            return (Type)(int)(phase);
        }
        public enum Type
        {
            //=============PHASES
            NONE,

            OnPlayCard,
            AfterPlayCard,

            OnPlayMinion,
            OnSummonMinion, //Tokens
            AfterPlayMinion,
            AfterSummonMinion,

            OnPlaySpell,
            AfterPlaySpell,

            BeforeAttack,
            AfterAttack,

            BeforeAttackFace,
            AfterAttackFace,

            StartTurn,
            EndTurn,
            CardDraw,

            //==============
            _PHASELIMIT,
            //=============SPECIAL EVENTS
            OnDamageTaken,
            OnMinionDamage,

            Deathrattle,
        }

        public enum Ability
        {
            KnifeJuggler,
            AcolyteOfPain,
        }
        public enum Side
        {
            Friendly,
            Enemy,
            Both,
        }
        public int playOrder = 0;
        public Type type;
        public Ability ability;
        public Side side;
        public Minion minion;

        public bool CheckTrigger(Type t,Side s, Server.CastInfo spell)
        {
            if (type != t) return false;

            switch(t)
            {
                case Type.OnPlayCard:
                case Type.OnPlaySpell:
                case Type.OnPlayMinion:
                case Type.OnSummonMinion:
                case Type.AfterPlayCard:
                case Type.AfterPlayMinion:
                case Type.AfterSummonMinion:
                case Type.AfterPlaySpell:
                    //Does not trigger on self
                    if (minion == spell.minion) return false;
                    break;
                    /*
                case Type.OnDamageTaken:
                    if (minion == spell.minion) return true;
                    break;
                    */
            }
            return (side == Side.Both || s == side);
        }
        public void ActivateTrigger(Server.Match match, ref Server.CastInfo spell)
        {
            Debug.Log("Trigger Activated");
            switch (ability)
            {
                case Ability.KnifeJuggler:
                    TriggerEffects.KnifeJuggler(match, minion);
                    break;
                case Ability.AcolyteOfPain:
                    TriggerEffects.AcolyteOfPain(match, minion);
                    break;
            }

        }

        public Trigger(Type typ,Side s, Ability abil, Minion owner, int order=0)
        {
            type = typ;
            side = s;
            ability = abil;
            minion = owner;
            playOrder = order;
        }
    }
}
