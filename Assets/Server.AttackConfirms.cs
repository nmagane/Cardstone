using System.Diagnostics;
using Riptide;

public partial class Server
{
    public bool ValidAttackMinion(Match m, int attackerInd, int targetInd)
    {
        //TODO: reuse this function in board code
        Minion attacker = attackerInd <0 ? null:m.currPlayer.board[attackerInd];
        Minion target = m.enemyPlayer.board[targetInd];

        if (attacker!=null) if (attacker.canAttack == false) return false;
        if (attacker == null) if (m.currPlayer.canAttack == false) return false;
        if (target.HasAura(Aura.Type.Stealth)) return false;

        bool enemyTaunting = false;
        foreach (var minion in m.enemyPlayer.board)
        {
            if (minion.HasAura(Aura.Type.Taunt) && minion.HasAura(Aura.Type.Stealth)==false) { enemyTaunting = true; break; }
        }
        if (target.HasAura(Aura.Type.Taunt) == false && enemyTaunting) return false;

        return true;
    }
    public bool ValidAttackFace(Match match,Player attacker,Player defender, int attackerInd)
    {

        if (attackerInd >= 0) if (attacker.board[attackerInd].canAttack == false) return false;
        else if (match.currPlayer.canAttack == false) return false;

        foreach (var minion in defender.board)
        {
            if (minion.HasAura(Aura.Type.Taunt) && minion.HasAura(Aura.Type.Stealth)==false) 
            {
                return false;
            }
        }
        return true;
    }

    public void ConfirmAttackGeneral(CastInfo action, bool preattack=false)
    {
        Match match = action.match;
        AttackInfo attack = action.attack;

        if (attack.faceAttack)
        {
            if (attack.weaponSwing)
            {
                //Face to face
                ConfirmSwingFace(match, attack.friendlyFire, preattack, action.player.canAttack);
            }
            else
            {
                //Minion to face
                ConfirmAttackFace(match, attack.attacker.index, attack.friendlyFire,preattack, attack.attacker.canAttack);
            }
        }
        else
        {
            if (attack.weaponSwing)
            {
                //Face to minion
                ConfirmSwingMinion(match, attack.target.index, attack.friendlyFire, preattack, action.player.canAttack);
            }
            else
            {
                //Minion to minion
                ConfirmAttackMinion(match, attack.attacker.index, attack.target.index, attack.friendlyFire,preattack, attack.attacker.canAttack);
            }
        }
    }

    public void ConfirmAttackMinion(Match match, int attackerInd, int targetInd, bool friendlyFire, bool PREATTACK, bool canAttack)
    {
        MessageType phase = PREATTACK ? MessageType.ConfirmPreAttackMinion : MessageType.ConfirmAttackMinion;
        CustomMessage mOwner = CreateMessage(phase);
        CustomMessage mOpp = CreateMessage(phase);
        mOwner.AddBool(true);
        mOpp.AddBool(false);

        mOwner.AddInt(attackerInd);
        mOpp.AddInt(attackerInd);

        mOwner.AddInt(targetInd);
        mOpp.AddInt(targetInd);

        mOwner.AddBool(canAttack);
        mOpp.AddBool(canAttack);

        mOwner.AddBool(friendlyFire);
        mOpp.AddBool(friendlyFire);

        SendMessage(mOwner, match.currPlayer);
        SendMessage(mOpp, match.enemyPlayer);
    }
    public void ConfirmAttackFace(Match match, int attackerInd,bool friendlyFire, bool PREATTACK, bool canAttack)
    {
        MessageType phase = PREATTACK ? MessageType.ConfirmPreAttackFace : MessageType.ConfirmAttackFace;
        CustomMessage mOwner = CreateMessage(phase);
        CustomMessage mOpp = CreateMessage(phase);
        mOwner.AddBool(true);
        mOpp.AddBool(false);

        mOwner.AddInt(attackerInd);
        mOpp.AddInt(attackerInd);

        mOwner.AddBool(canAttack);
        mOpp.AddBool(canAttack);

        mOwner.AddBool(friendlyFire);
        mOpp.AddBool(friendlyFire);

        SendMessage(mOwner, match.currPlayer);
        SendMessage(mOpp, match.enemyPlayer);
    }
    public void ConfirmSwingMinion(Match match, int targetInd, bool friendlyFire, bool PREATTACK, bool canAttack)
    {
        MessageType phase = PREATTACK ? MessageType.ConfirmPreSwingMinion : MessageType.ConfirmSwingMinion;
        CustomMessage mOwner = CreateMessage(phase);
        CustomMessage mOpp = CreateMessage(phase);

        mOwner.AddBool(true);
        mOpp.AddBool(false);

        mOwner.AddInt(targetInd);
        mOpp.AddInt(targetInd);

        mOwner.AddBool(canAttack);
        mOpp.AddBool(canAttack);

        mOwner.AddBool(friendlyFire);
        mOpp.AddBool(friendlyFire);

        SendMessage(mOwner, match.currPlayer);
        SendMessage(mOpp, match.enemyPlayer);
    }
    public void ConfirmSwingFace(Match match, bool friendlyFire, bool PREATTACK, bool canAttack)
    {
        MessageType phase = PREATTACK ? MessageType.ConfirmPreSwingFace : MessageType.ConfirmSwingFace;
        CustomMessage mOwner = CreateMessage(phase);
        CustomMessage mOpp = CreateMessage(phase);

        mOwner.AddBool(true);
        mOpp.AddBool(false);

        mOwner.AddBool(canAttack);
        mOpp.AddBool(canAttack);

        mOwner.AddBool(friendlyFire);
        mOpp.AddBool(friendlyFire);
        SendMessage(mOwner, match.currPlayer);
        SendMessage(mOpp, match.enemyPlayer);
    }

    public void ConfirmHeroPower(CastInfo spell)
    {
        MessageType phase = MessageType.ConfirmHeroPower;
        CustomMessage mOwner = CreateMessage(phase);
        CustomMessage mOpp = CreateMessage(phase);
        mOwner.AddBool(true);
        mOpp.AddBool(false);

        mOwner.AddInt(spell.card.manaCost);
        mOpp.AddInt(spell.card.manaCost);

        SendMessage(mOwner, spell.match.currPlayer);
        SendMessage(mOpp, spell.match.enemyPlayer);
    }
}