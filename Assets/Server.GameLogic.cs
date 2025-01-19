
using UnityEngine;

public partial class Server
{
    public void ConsumeAttackCharge(Board.Minion m)
    {
        if (m.WINDFURY) m.WINDFURY = false;
        else m.canAttack = false;
    }
    
    public void DamageMinion(Match match, Board.Minion minion, int damage)
    {
        minion.health -= damage;
        UpdateMinion(match, minion);

        //TODO: trigger ON DAMAGE (acolyte)
        //todo: triggier MINION DAMAGE (frothing)
        if (minion.health<=0)
        {
            DestroyMinion(match, minion);
        }
    }
    public void DamageFace(Match match, Player target, int damage)
    {
        target.health -= damage;
        UpdateHero(match,target);

        //TODO: trigger ON DAMAGE FACE

        if (target.health<=0)
        {
            //TODO: GAME END
            Debug.Log("Game over");
        }
    }


}
