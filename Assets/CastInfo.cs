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
    public int playOrder = 0;

    public CastInfo(Match m, Player p, HandCard name, int t, int s, bool fri, bool hero)
    {
        match = m;
        player = p;
        card = name;
        target = t;
        isFriendly = fri;
        isHero = hero;
        position = s;
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
        return isFriendly ? player.board[target] : player.opponent.board[target];
    }
}
