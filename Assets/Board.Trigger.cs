using System.Diagnostics;

public partial class Board
{
    public class Trigger
    {
        public enum Type
        {
            //=============PHASES

            OnPlayCard,
            AfterPlayCard,

            OnPlayMinion,
            OnSummonMinion, //Tokens
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
            OnFriendlyMinionDamage,
            OnAnyMinionDamage,

            Deathrattle,
        }

        public static Type GetPhaseTrigger(Server.Match.Phase phase)
        {
            if ((int)phase>=(int)Type._PHASELIMIT)
            {
                throw new System.Exception("NO TRIGGER FOUND FOR REQUESTED PHASE");
            }
            return (Type)(int)(phase);
        }
        public int PlayOrder = 0;

    }
}
