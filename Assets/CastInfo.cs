public class CastInfo
{
    public Match match;
    public Player player;
    public HandCard card;
    public int target;
    public int position;
    public bool isFriendly;
    public bool isHero;
    public AttackInfo attack = null;
    public Minion minion;
    public Weapon weapon;
    public int playOrder = 0;
    public bool combo => player.combo;
    public Minion targetMinion;
    public Player targetPlayer;

    public CastInfo(Match m, Player p, HandCard name, int t, int s, bool fri, bool hero)
    {
        match = m;
        player = p;
        card = name;
        target = t;
        isFriendly = fri;
        isHero = hero;
        position = s;

        if (isHero)
        {
            targetPlayer = isFriendly ? p : p.opponent;
            targetMinion = null;
        }
        else
        {
            if (target!=-1)
            {
                targetMinion = isFriendly ? p.board[target] : p.opponent.board[target];
                targetPlayer = null;
            }
        }
    }
    public CastInfo(Match m, AttackInfo a)
    {
        match = m;
        attack = a;
        player = a.player;
    }
    public CastInfo()
    {

    }

    public Minion GetTargetMinion()
    {
        return targetMinion;
    }
}
