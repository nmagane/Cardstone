using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;
using UnityEngine.Assertions.Must;

public static class AuraEffects
{
    public static void Spellpower(Match m, Minion minion, Aura source)
    {
        minion.player.AddAura(new Aura(Aura.Type.SP_PLAYER_BUFF, 1, false, true,source));
    }
    public static void StormwindChampion(Match match, Minion sourceMinion, Aura sourceAura)
    {
        Player owner = match.FindOwner(sourceMinion);
        foreach (var m in owner.board)
        {
            if (m == sourceMinion)
            {
                continue;
            }
            match.server.AddAura(match,m,new Aura(Aura.Type.Health, 2, false, true, sourceAura));
            match.server.AddAura(match,m,new Aura(Aura.Type.Damage, 2, false, true, sourceAura));
        }
    }
    public static void DireWolfAlpha(Match match, Minion sourceMinion, Aura sourceAura)
    {
        Player owner = match.FindOwner(sourceMinion);
        foreach (var m in owner.board)
        {
            if (m.index == sourceMinion.index - 1 || m.index == sourceMinion.index + 1)
                match.server.AddAura(match, m, new Aura(Aura.Type.Damage, 1, false, true, sourceAura));
        }
    }

    public static void Amani(Match match, Minion sourceMinion, Aura sourceAura)
    {
        match.server.AddAura(match, sourceMinion, new Aura(Aura.Type.Damage, 3, false, true, sourceAura));
    }

    public static void Mana_Wraith(Match match, Minion sourceMinion, Aura sourceAura)
    {
        Player owner = match.FindOwner(sourceMinion);
        foreach (Player p in match.players)
        {
            foreach (HandCard c in p.hand)
                if (c.MINION)
                    match.server.AddCardAura(match, c, new Aura(Aura.Type.Cost, 1, false, true, sourceAura));
        }    
    }

    public static void Loatheb(Match match, Minion sourceMinion, Aura sourceAura)
    {
        foreach (HandCard c in sourceMinion.player.hand)
            if (c.SPELL)
            {
                match.server.AddCardAura(match, c, new Aura(Aura.Type.Cost, 5, false, true, sourceAura));
            }
    }
    public static void Preparation(Match match, Minion sourceMinion, Aura sourceAura)
    {
        Player owner = match.FindOwner(sourceMinion);

        foreach (HandCard c in owner.hand)
            if (c.SPELL)
                match.server.AddCardAura(match, c, new Aura(Aura.Type.Cost, -3, false, true, sourceAura));
        
    }

    public static void Millhouse(Match match, Minion sourceMinion, Aura sourceAura)
    {
        Player owner = match.FindOwner(sourceMinion);

        foreach (HandCard c in owner.hand)
            if (c.MINION)
                match.server.AddCardAura(match, c, new Aura(Aura.Type.SetCost, 0, false, true, sourceAura));

    }

    internal static void Southsea_Deckhand(Match match, Minion minion, Aura aura)
    {
        if (minion.player.match.currPlayer != minion.player) minion.RemoveAura(aura);
        if (minion.player.weapon!=null && minion.SICKNESS)
        {
            match.server.AddAura(match, minion, new Aura(Aura.Type.Charge,0,false,true,aura));
        }
    }

    public static void HoldingDragonTargeted(Match match, HandCard card, Aura aura)
    {
        Player owner = match.FindOwner(card);
        if (owner == null) return;

        card.TARGETED = false;
        card.BATTLECRY = false;
        foreach(var c in owner.hand)
        {
            if (c == card) continue;
            if (c.tribe == Card.Tribe.Dragon)
            {
                card.TARGETED = true;
                card.BATTLECRY = true;
                return;
            }
        }
    }

    public static void Mountain_Giant(Match match, HandCard card, Aura aura)
    {
        Player owner = match.FindOwner(card);
        if (owner == null) return;

        int i = 0;
        foreach (var c in owner.hand)
        {
            if (c == card) continue;
            i++;
        }
        match.server.AddCardAura(match, card, new Aura(Aura.Type.Cost, -i, false, true, aura));
    } 
    public static void Sea_Giant(Match match, HandCard card, Aura aura)
    {
        Player owner = match.FindOwner(card);
        if (owner == null) return;

        int i = owner.board.Count() + owner.opponent.board.Count();

        match.server.AddCardAura(match, card, new Aura(Aura.Type.Cost, -i, false, true, aura));
    }
    public static void Molten_Giant(Match match, HandCard card, Aura aura)
    {
        Player owner = match.FindOwner(card);
        if (owner == null) return;

        int i = owner.maxHealth - owner.health;

        match.server.AddCardAura(match, card, new Aura(Aura.Type.Cost, -i, false, true, aura));
    }
    public static void Dread_Corsair(Match match, HandCard card, Aura aura)
    {
        Player owner = match.FindOwner(card);
        if (owner == null) return;
        if (owner.weapon == null) return;
        if (owner.weapon.damage <= 0) return;

        int i = owner.weapon.damage;
        match.server.AddCardAura(match, card, new Aura(Aura.Type.Cost, -i, false, true, aura));
    }
}
