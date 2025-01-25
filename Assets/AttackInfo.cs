public class AttackInfo
{
    public Player player;
    public Minion attacker;
    public Minion target;
    public bool weaponSwing = false;
    public bool faceAttack = false;
    public bool friendlyFire = false;

    public AttackInfo(Player p, Minion atk, Minion tar, bool swing, bool face, bool friendly)
    {
        player = p;
        attacker = atk;
        target = tar;
        weaponSwing = swing;
        faceAttack = face;
        friendlyFire = friendly;
    }
}