public partial class Board
{
    public class Trigger
    {
        public enum Type
        {
            OnPlayCard,
            AfterPlayCard,

            OnPlayMinion,
            OnSummonMinion,
            AfterSummonMinion,

            OnPlaySpell,
            OnCastSpell,
            AfterSpell,

            BeforeAttack,
            Attack,
            AfterAttack,

            OnDamageTaken,
            OnFriendlyMinionDamage,
            OnAnyMinionDamage,
        }
        public int PlayOrder = 0;

    }
}
