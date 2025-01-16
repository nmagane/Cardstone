public partial class Board
{
    public bool targeting = false;
    public TargetMode targetMode = TargetMode.None;
    public EligibleTargets eligibleTargets = EligibleTargets.AllCharacters;
    //public int targetSourceIndex = 0;
    //public int targetIndex = 0;
    public Minion targetingMinion = null;
    public HandCard targetingCard = null;
    public bool dragTargeting = false;

    public enum TargetMode
    {
        None,
        Attack,
        Spell,
        HeroPower,
        Weapon,
    }
    public enum EligibleTargets
    {
        AllCharacters,

        EnemyCharacters,
        EnemyMinions,

        FriendlyCharacters,
        FriendlyMinions,
    }

    public void TargetMinion(Minion minion)
    {
        switch (targetMode)
        {
            case TargetMode.Attack:
                AttackMinion(targetingMinion, minion);
                break;
            case TargetMode.Spell:
                break;
            case TargetMode.HeroPower:
                break;
            case TargetMode.Weapon:
                break;
            case TargetMode.None:
                EndTargeting();
                break;
        }
    }

    public void StartTargetingAttack(Minion source)
    {
        targeting = true;
        targetMode = TargetMode.Attack;
        eligibleTargets = EligibleTargets.EnemyCharacters;
        targetingMinion = source;
        //TODO: target anim arrows
    }
    public void EndTargeting()
    {
        targeting = false;
        targetMode = TargetMode.None;
        eligibleTargets = EligibleTargets.AllCharacters;
        //targetSourceIndex = 0;
        //targetIndex = 0;
        targetingMinion = null;
        targetingCard = null;
        dragTargeting = false;
    }

    public bool CheckTargetEligibility(Minion m)
    {
        if (eligibleTargets == EligibleTargets.AllCharacters)
        {
            return true;
        }
        if (eligibleTargets == EligibleTargets.EnemyMinions || eligibleTargets == EligibleTargets.EnemyCharacters)
        {
            if (IsFriendly(m)) return false;
            else return true;
        }
        if (eligibleTargets == EligibleTargets.FriendlyMinions || eligibleTargets == EligibleTargets.FriendlyCharacters)
        {
            if (IsFriendly(m)) return true;
            else return false;
        }
        return true;
    }

}
