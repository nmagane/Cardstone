using System.Collections.Generic;
using UnityEngine;
public partial class Server
{
    public static void ConsumeAttackCharge(Board.Minion m)
    {
        if (m.WINDFURY) m.WINDFURY = false;
        else m.canAttack = false;
    }
    public void DamageMinionsAOE()
    {
        //TODO: AOE EFFECTS DONT ACTIVATE TRIGGERS UNTIL ALL UNITS TAKE DAMAGE
    }
    public void DamageMinion(Match match, Board.Minion minion, int damage)
    {
        minion.health -= damage;

        //TODO: trigger ON DAMAGE (acolyte)
        //todo: triggier MINION DAMAGE (frothing)
        match.TriggerMinion(Board.Trigger.Type.OnDamageTaken,minion);
        match.AddTrigger(Board.Trigger.Type.OnMinionDamage, null, minion);
        UpdateMinion(match, minion);
    }


    public void DamageFace(Match match, Player target, int damage)
    {
        target.health -= damage;
        //UpdateHero(match,target);

        UpdateHero(match, target);
        //TODO: trigger ON DAMAGE FACE
    }
    public class AttackInfo
    {
        public Player player;
        public Board.Minion attacker;
        public Board.Minion target;
        public bool weaponSwing = false;
        public bool faceAttack = false;
        public bool friendlyFire = false;

        public AttackInfo(Player p, Board.Minion atk, Board.Minion tar, bool swing, bool face, bool friendly)
        {
            player = p;
            attacker = atk;
            target = tar;
            weaponSwing = swing;
            faceAttack = face;
            friendlyFire = friendly;
        }
    }
    public class CastInfo
    {
        public Match match;
        public Player player;
        public Board.HandCard card;
        public int target;
        public int position;
        public bool isFriendly;
        public bool isHero;
        public AttackInfo attack=null;
        public Board.Minion minion;

        public CastInfo(Match m, Player p,Board.HandCard name,int t, int s, bool fri, bool hero)
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
    }

    public bool ExecuteAttack(ref CastInfo action)
    {
        bool success = ExecuteAttackLogic(ref action);
        if (success) ConfirmAttackGeneral(action);
        return success;
    }

    bool ExecuteAttackLogic(ref CastInfo action)
    {
        AttackInfo attack = action.attack;
        Match match = action.match;

        if (attack.faceAttack)
        {
            Player targetPlayer = attack.friendlyFire ? attack.player : attack.player.opponent;
            if (attack.weaponSwing)
            {
                //Face to face
                //Check for failed attack
                if (attack.player.health<=0)
                {
                    return false;
                }

                return true;
            }
            //Check for failed attacks
            if (attack.attacker.DEAD) 
                return false;

            //Minion to face
            ConsumeAttackCharge(attack.attacker);
            DamageFace(match, targetPlayer, attack.attacker.damage);
            return true;
        }

        //Check for failed attacked
        if (attack.weaponSwing && attack.target.DEAD)
        {
            return false;
        }
        else if (attack.attacker.DEAD || attack.target.DEAD)
        {
            return false;
        }

        //Successful attack
        if (attack.weaponSwing)
        {
            //Face to Minion
            return true;
        }
        //Minion to minion
        ConsumeAttackCharge(attack.attacker);
        DamageMinion(match, attack.target, attack.attacker.damage);
        DamageMinion(match, attack.attacker, attack.target.damage);
        return true;
    }
    public void CastSpell(CastInfo spell)
    {
        //TODO: check for target survival/validity after prespell phase. fizzle if invalid.
        switch(spell.card.card)
        {
            case Card.Cardname.Ping:
                Ping(spell);
                break;
            case Card.Cardname.ArcExplosion:
                Arcane_Explosion(spell);
                break;
            case Card.Cardname.ShatteredSunCleric:
                ShatteredSunCleric(spell);
                break;
            case Card.Cardname.Argus:
                Argus(spell);
                break;
            default:
                Debug.LogError("MISSING SPELL " + spell.card.card);
                break;
        }
    }

    void Ping(CastInfo spell)
    {
        int damage = 1;

        if (spell.isHero)
        {
            if (spell.isFriendly)
                DamageFace(spell.match, spell.player, damage);
            else
                DamageFace(spell.match, spell.match.Opponent(spell.player), damage);
            return;
        }

        if (spell.isFriendly)
            DamageMinion(spell.match, spell.player.board[spell.target], damage);
        else
            DamageMinion(spell.match, spell.match.Opponent(spell.player).board[spell.target], damage);

    }

    void Arcane_Explosion(CastInfo spell)
    {
        int damage = 1;

        Player opp = spell.match.Opponent(spell.player);
        List<Board.Minion> minions = new List<Board.Minion>();
        foreach (var m in opp.board)
        {
            minions.Add(m);
        }

        foreach (var m in minions)
            DamageMinion(spell.match, m, damage);
    }
    void ShatteredSunCleric(CastInfo spell)
    {
        Player p = spell.player;
        Match m = spell.match;
        //TODO: SILENCABLE AURAS

        p.board[spell.target].AddAura(new Board.Minion.Aura(Board.Minion.Aura.Type.Health, 1));
        p.board[spell.target].AddAura(new Board.Minion.Aura(Board.Minion.Aura.Type.Damage, 1));
    }
    void Argus(CastInfo spell)
    {
        Player p = spell.player;
        foreach(var m in spell.player.board)
        {
            if (m.index == spell.position-1 || m.index == spell.position+1)
            {
                m.AddAura(new Board.Minion.Aura(Board.Minion.Aura.Type.Taunt));
                m.AddAura(new Board.Minion.Aura(Board.Minion.Aura.Type.Health, 1));
                m.AddAura(new Board.Minion.Aura(Board.Minion.Aura.Type.Damage, 1));
            }
        }
    }
}
