﻿using System.Collections.Generic;
public partial class Server
{
    public void Lesser_Heal(CastInfo spell)
    {

        var anim = new AnimationInfo(Card.Cardname.Lesser_Heal, spell.player, spell);
        HealTarget(2, spell);
    }
    public void Circle_of_Healing(CastInfo spell)
    {
        var anim = new AnimationInfo(Card.Cardname.Circle_of_Healing, spell.player);
        foreach (Minion m in spell.player.board)
        {
            Heal(m, 4, spell);
        }
        foreach (Minion m in spell.player.opponent.board)
        {
            Heal(m, 4, spell);
        }
    }
    public void Light_of_the_Naaru(CastInfo spell)
    {

        var anim = new AnimationInfo(Card.Cardname.Lesser_Heal, spell.player, spell);
        HealTarget(3, spell);
        spell.match.midPhase = true;
        spell.match.ResolveTriggerQueue(ref spell);
        spell.match.midPhase = false;

        bool summ = false;
        if (spell.isHero)
        {
            if (spell.targetPlayer.health < spell.targetPlayer.maxHealth)
                summ = true;
        }
        else if (spell.targetMinion.health < spell.targetMinion.maxHealth)
        {
            summ = true;
        }
        if (summ)
            spell.match.server.SummonToken(spell.match, spell.player, Card.Cardname.Lightwarden);
    }
    public void Power_Word_Shield(CastInfo spell)
    {
        if (spell.targetMinion == null) return;

        var anim = new AnimationInfo(Card.Cardname.Lesser_Heal, spell.player, spell);
        AddAura(spell.match, spell.targetMinion, new Aura(Aura.Type.Health, 2));
        Draw(spell.player);
    }
    public void Divine_Spirit(CastInfo spell)
    {
        if (spell.targetMinion == null) return;
        var anim = new AnimationInfo(Card.Cardname.Inner_Rage, spell.player, spell.targetMinion);
        AddAura(spell.match, spell.targetMinion, new Aura(Aura.Type.Health, spell.targetMinion.health));
    }
    public void Inner_Fire(CastInfo spell)
    {
        if (spell.targetMinion == null) return;
        var anim = new AnimationInfo(Card.Cardname.Inner_Rage, spell.player, spell.targetMinion);
        SetDamage(spell.match, spell.targetMinion, spell.targetMinion.health);
    }
    public void Mind_Blast(CastInfo spell)
    {
        var anim = new AnimationInfo(Card.Cardname.Blackwing_Corruptor, spell.player, spell.player.opponent);
        Damage(spell.player.opponent, 5, spell);
    }
    public void Velens_Chosen(CastInfo spell)
    {
        if (spell.targetMinion == null) return;

        var anim = new AnimationInfo(Card.Cardname.Lesser_Heal, spell.player, spell);
        AddAura(spell.match, spell.targetMinion, new Aura(Aura.Type.Damage, 2, cardname: Card.Cardname.Velens_Chosen));
        AddAura(spell.match, spell.targetMinion, new Aura(Aura.Type.Health, 4, cardname: Card.Cardname.Velens_Chosen));
        AddAura(spell.match, spell.targetMinion, new Aura(Aura.Type.Spellpower, 1, cardname: Card.Cardname.Velens_Chosen));
    }

    public void Holy_Smite(CastInfo spell)
    {
        var anim = new AnimationInfo(Card.Cardname.Keeper_of_the_Grove, spell.player, spell);
        DamageTarget(2, spell);
    }
    public void Holy_Nova(CastInfo spell)
    {
        var anim = new AnimationInfo(Card.Cardname.Holy_Nova, spell.player);
        Damage(spell.player.opponent,2,spell);
        foreach (Minion m in spell.player.opponent.board)
            Damage(m,2,spell);

        spell.match.midPhase = true;
        spell.match.ResolveTriggerQueue(ref spell);
        spell.match.midPhase = false;

        Heal(spell.player,2,spell);
        foreach (Minion m in spell.player.board)
            Heal(m,2,spell);
    }
    public void Lightbomb(CastInfo spell)
    {
        foreach (Minion m in spell.player.opponent.board)
        {
            var anim = new AnimationInfo(Card.Cardname.Darkbomb, spell.player,m);
            Damage(m, m.damage, spell);
        }
    }
    public void Thoughtsteal(CastInfo spell)
    {
        List<Card.Cardname> c = new List<Card.Cardname>(spell.player.opponent.deck); 
        if (c.Count == 0) return;

        c = Board.Shuffle(c);
        for (int i=0;i<2;i++)
        {
            AddCard(spell.match, spell.player, c[0]);
            c.RemoveAt(0);
            if (c.Count == 0) break;
        }

    }
    public void Shadow_Word_Death(CastInfo spell)
    {
        var anim = new AnimationInfo(Card.Cardname.Blackwing_Corruptor, spell.player, spell);
        spell.targetMinion.DEAD = true;
    }
    public void Shadow_Word_Pain(CastInfo spell)
    {
        var anim = new AnimationInfo(Card.Cardname.Blackwing_Corruptor, spell.player, spell);
        spell.targetMinion.DEAD = true;
    }
    public void Silence(CastInfo spell)
    {
        SilenceMinion(spell);
    }
    public void Cabal_Shadow_Priest(CastInfo spell)
    {
        if (spell.targetMinion == null) return;
        var anim = new AnimationInfo(Card.Cardname.Mortal_Coil, spell.player, spell.minion, spell.targetMinion);
        StealMinion(spell.match,spell.player,spell.targetMinion);
    }
    public void Resurrect(CastInfo spell)
    {
        if (spell.player.graveyard.Count == 0) return;
        Card.Cardname c = Board.RandElem(spell.player.graveyard);
        SummonToken(spell.match, spell.player, c);
    }
    private void Shrinkmeister(CastInfo spell)
    {
        AddAura(spell.match, spell.targetMinion, new Aura(Aura.Type.Damage, -3, true));
    }
}
