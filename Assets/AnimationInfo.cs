using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnimationInfo
{
    public Card.Cardname card;

    public bool sourceIsHero;
    public bool sourceIsFriendly;
    public int sourceIndex;

    public bool targetIsHero;
    public bool targetIsFriendly;
    public int targetIndex;
    public AnimationInfo()
    {

    }
    public AnimationInfo(Card.Cardname card, Player player, CastInfo target)
    {
        if (target.isHero)
        {
            var a = new AnimationInfo(card, player, target.targetPlayer);
        }
        else
        {
            var a = new AnimationInfo(card, player, target.targetMinion);
        }
    }
    public AnimationInfo(Card.Cardname card, Player player, Minion source, CastInfo target)
    {
        if (target.isHero)
        {
            var a = new AnimationInfo(card, player, source, target.targetPlayer);
        }
        else
        {
            var a = new AnimationInfo(card, player, source, target.targetMinion);
        }
    }
    public AnimationInfo(Card.Cardname card, Player player, Minion source, Minion target)
    {
        this.card = card;

        sourceIsHero = false;
        sourceIndex = source.index;
        sourceIsFriendly = player.board.Contains(source);

        targetIsHero = false;
        targetIndex = target.index;
        targetIsFriendly = player.board.Contains(target);

        SendAnim(player);
    }
    public AnimationInfo(Card.Cardname card, Player player, Minion source, Player target)
    {
        this.card = card;

        sourceIsHero = false;
        sourceIndex = source.index;
        sourceIsFriendly = player.board.Contains(source);

        targetIsHero = true;
        targetIsFriendly = player == target;

        SendAnim(player);
    }
    public AnimationInfo(Card.Cardname card, Player player, Minion target)
    {
        this.card = card;

        sourceIsHero = true;
        sourceIsFriendly = true;

        targetIsHero = false;
        targetIndex = target.index;
        targetIsFriendly = player.board.Contains(target);

        SendAnim(player);
    }
    public AnimationInfo(Card.Cardname card, Player player, Player target)
    {
        this.card = card; this.card = card;

        sourceIsHero = true;
        sourceIsFriendly = true;

        targetIsHero = true;
        targetIsFriendly = player == target;

        SendAnim(player);
    }
    public AnimationInfo(Card.Cardname card, Player player)
    {
        this.card = card;
        sourceIsHero = true;
        sourceIsFriendly = true;

        targetIsHero = true;
        targetIsFriendly = false;

        SendAnim(player);
    }

    public void SendAnim(Player player)
    {
        player.match.server.ConfirmAnimation(player.match, player, this);
    }

}
