
using System.Collections.Generic;
using UnityEditor;
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

    class CastInfo
    {
        public Match match;
        public Player player;
        public Card.Cardname card;
        public int target;
        public bool isFriendly;
        public bool isHero;

        public CastInfo(Match m, Player p,Card.Cardname name,int t, bool fri, bool hero)
        {
            match = m;
            player = p;
            card = name;
            target = t;
            isFriendly = fri;
            isHero = hero;
        }
    }
    public void CastSpell(Match match, Player player, Card.Cardname card, int target,bool isFriendly, bool isHero)
    {
        CastInfo spell = new CastInfo(match,player,card,target,isFriendly,isHero);
        switch(spell.card)
        {
            case Card.Cardname.Ping:
                Ping(spell);
                break;
            case Card.Cardname.ArcExplosion:
                Arcane_Explosion(spell);
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
}
