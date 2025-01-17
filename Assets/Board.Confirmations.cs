using UnityEngine;
using Riptide;

public partial class Board
{
    public void ConfirmAttackMinion(Message message)
    {
        bool allyAttack = message.GetBool();
        int attackerIndex = message.GetInt();
        int targetIndex = message.GetInt();
        
        Minion attacker = allyAttack ? currMinions[attackerIndex] : enemyMinions[attackerIndex];
        Minion target = allyAttack ? enemyMinions[targetIndex] : currMinions[targetIndex];

        if (allyAttack)
        {
            if (attacker.WINDFURY) attacker.WINDFURY = false;
            else attacker.canAttack = false;
        }

        Debug.Log(playerID + ": " + (allyAttack ? "ally " : "enemy ") + attacker.ToString() + " hits " + target.ToString());
        //TODO: attack animation
    }
}
