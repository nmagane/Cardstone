using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
public partial class Server
{
    public static void RefreshAttackCharge(Minion m)
    {
        Aura windfury = m.FindAura(Aura.Type.Windfury);
        if (windfury != null)
            windfury.trigger = true;

        m.SICKNESS = false;
        m.canAttack = true;
    }
    public static void RefreshAttackCharge(Player p)
    {
        Aura windfury = p.FindAura(Aura.Type.Windfury);
        if (windfury != null)
            windfury.trigger = true;

        p.canAttack = true;
    }

    public void ConsumeAttackCharge(Minion m)
    {
        m.SICKNESS = false;
        if (m.STEALTH)
        {
            RemoveAura(m.player.match, m, m.FindAura(Aura.Type.Stealth));
        }

        Aura windfury = m.FindAura(Aura.Type.Windfury);
        if (windfury != null)
        {  
            if (windfury.trigger)
            {
                windfury.trigger = false;
                return;
            }
        }

        m.canAttack = false;
    }

    public static void ConsumeAttackCharge(Player p)
    {
        Aura windfury = p.FindAura(Aura.Type.Windfury);
        if (windfury != null)
        {
            if (windfury.trigger)
            {
                windfury.trigger = false;
                return;
            }
        }
        p.canAttack = false;
    }
    public void DamageMinionsAOE()
    {
        //TODO: AOE EFFECTS DONT ACTIVATE TRIGGERS UNTIL ALL UNITS TAKE DAMAGE

    }

    public void HealMinion(Match match, Minion minion, int heal)
    {
        bool healed = false;
        if (minion.health<minion.maxHealth)
        {
            healed = true;
        }
        minion.health = Mathf.Min(minion.health+heal,minion.maxHealth);
        if (healed)
        {
            match.TriggerMinion(Trigger.Type.OnHealGiven, minion);
            match.AddTrigger(Trigger.Type.OnMinionHealed, null, minion);
        }
        //this is for the client to know if its not an aura change
        match.healedMinions.Add(minion);
    }
    public void HealFace(Match match, Player target, int heal)
    {
        bool healed = false;
        if (target.health < target.maxHealth)
            healed = true;

        target.health = Mathf.Min(target.health + heal, target.maxHealth);

        if (healed)
        {
            match.AddTrigger(Trigger.Type.OnFaceHealed, null, target);
        }

        //this is for the client to know if its not an aura change
        match.healedPlayers.Add(target);
    }

    public void DamageMinion(Match match, Minion minion, int damage, Player source)
    {
        if (minion.HasAura(Aura.Type.Shield))
        {
            match.server.RemoveAura(match,minion,minion.FindAura(Aura.Type.Shield));
            return;
        }

        if (minion.HasAura(Aura.Type.Immune)) return;

        minion.health -= damage;

        match.TriggerMinion(Trigger.Type.OnDamageTaken,minion);
        match.AddTrigger(Trigger.Type.OnMinionDamage, null, minion);

        //this is for the client to know if its not an aura change
        match.damagedMinions.Add(minion);
    }


    public void DamageFace(Match match, Player target, int damage, Player source)
    {
        if (target.health + target.armor <= damage)
        {
            match.AddTrigger(Trigger.Type.OnLethalFaceDamage, null, source);
            CastInfo c = new CastInfo();
            match.ResolveTriggerQueue(ref c);
        }

        if (target.HasAura(Aura.Type.Immune)) return;

        int startingArmor = target.armor;
        target.armor = Mathf.Max(0, target.armor - damage);
        damage -= startingArmor;
        if (damage>0)
            target.health -= damage;

        match.AddTrigger(Trigger.Type.OnFaceDamage, null, source);

        //this is for the client to know if its not an aura change
        match.damagedPlayers.Add(target);
    }
    
    public void FatiguePlayer(Match match, Player target)
    {
        target.fatigue++;

        DamageFace(match, target, target.fatigue, target);
    }
    public void SetHealth(Match match, Minion minion, int value)
    {
        minion.RemoveTemporaryAuras(Aura.Type.Health);

        int externalHealth = 0;
        foreach (Aura a in minion.auras)
        {
            if (a.foreignSource && a.type == Aura.Type.Health && a.sourceAura != null)
            {
                externalHealth += a.value;
            }
        }

        int diff = value - (minion.maxHealth - externalHealth);
        //Debug.Log($"hp {diff}={value}-({minion.maxHealth}-{externalHealth})");
        match.server.AddAura(match, minion, new Aura(Aura.Type.Health,diff));
        minion.health = minion.maxHealth;
    }

    public void SetDamage(Match match, Minion minion, int value)
    {
        minion.RemoveTemporaryAuras(Aura.Type.Damage);

        int externalDamage = 0;
        foreach (Aura a in minion.auras)
        {
            if (a.foreignSource && a.type ==Aura.Type.Damage && a.sourceAura!=null)
            {
                //if (a.sourceAura.minion != minion)
                externalDamage += a.value;
            }
        }

        int diff = value - (minion.damage-externalDamage);
        //Debug.Log($"atk {diff}={value}-({minion.damage}-{externalDamage})");
        match.server.AddAura(match, minion, new Aura(Aura.Type.Damage,diff));
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

                ConsumeAttackCharge(action.player);
                DamageFace(match, targetPlayer, action.player.damage, action.player);
                return true;
            }

            //Minion to face
            //Check for failed attacks
            if (attack.attacker.DEAD) 
                return false;

            ConsumeAttackCharge(attack.attacker);
            DamageFace(match, targetPlayer, attack.attacker.damage,attack.attacker.player);
            return true;
        }

        //Attack on minion

        //Check for failed attacked
        if (attack.weaponSwing && attack.target.DEAD)
        {
            return false;
        }
        else if (!attack.weaponSwing)
        {
            if (attack.attacker.DEAD || attack.target.DEAD)
            {
                return false;
            }
        }

        //Successful attack
        if (attack.weaponSwing)
        {
            //Face to Minion
            ConsumeAttackCharge(action.player);
            DamageMinion(match, attack.target, action.player.damage, action.player);
            DamageFace(match, action.player, attack.target.damage, attack.target.player);
            return true;
        }
        //Minion to minion
        ConsumeAttackCharge(attack.attacker);
        DamageMinion(match, attack.target, attack.attacker.damage, attack.attacker.player);
        DamageMinion(match, attack.attacker, attack.target.damage, attack.target.player);
        return true;
    }
    public void CastSpell(CastInfo spell)
    {
        //TODO: check for target survival/validity after prespell phase. fizzle if invalid.

        switch(spell.card.card)
        {
            case Card.Cardname.Coin:
                Coin(spell);
                break;
            case Card.Cardname.Ping:
                Ping(spell);
                break;
            case Card.Cardname.Arcane_Explosion:
                Arcane_Explosion(spell);
                break;
            case Card.Cardname.Shattered_Sun_Cleric:
                ShatteredSunCleric(spell);
                break;
            case Card.Cardname.Defender_of_Argus:
                Defender_of_Argus(spell);
                break;
            case Card.Cardname.Abusive_Sergeant:
            case Card.Cardname.Dark_Iron_Dwarf:
                Abusive_Sergeant(spell);
                break;
            case Card.Cardname.Ironbeak_Owl:
                SilenceMinion(spell);
                break;
            case Card.Cardname.Voodoo_Doctor:
                HealTarget(2,spell);
                break;
            case Card.Cardname.Soulfire:
                Soulfire(spell);
                break;
            case Card.Cardname.Doomguard:
                Discard(spell, 2);
                break;
            case Card.Cardname.Lifetap:
                Lifetap(spell);
                break;
            case Card.Cardname.Flame_Imp:
                Flame_Imp(spell);
                break;
            case Card.Cardname.Loatheb:
                Loatheb(spell);
                break;
            case Card.Cardname.Preparation:
                Preparation(spell);
                break;
            case Card.Cardname.Millhouse_Manastorm:
                Millhouse_Manastorm(spell);
                break;
            case Card.Cardname.Hunters_Mark:
                Hunters_Mark(spell);
                break;
            case Card.Cardname.Crazed_Alchemist:
                Crazed_Alchemist(spell);
                break;
            case Card.Cardname.Heroic_Strike:
                Heroic_Strike(spell);
                break;
            case Card.Cardname.Deadly_Poison:
                Deadly_Poison(spell);
                break;
            case Card.Cardname.Blade_Flurry:
                Blade_Flurry(spell);
                break;
            case Card.Cardname.Armor_Up:
                Armor_Up(spell);
                break;
            case Card.Cardname.SI7_Agent:
                SI7_Agent(spell);
                break;
            case Card.Cardname.Eviscerate:
                Eviscerate(spell);
                break;
            case Card.Cardname.Sap:
                Sap(spell);
                break;
                
            case Card.Cardname.Frostbolt:
                Frostbolt(spell);
                break;

            case Card.Cardname.Dagger_Mastery:
                Dagger_Mastery(spell);
                break;
            case Card.Cardname.Backstab:
                Backstab(spell);
                break;

            case Card.Cardname.Fan_of_Knives:
                Fan_of_Knives(spell);
                break;

            case Card.Cardname.Earthen_Ring_Farseer:
                Earthen_Ring_Farseer(spell);
                break;

            case Card.Cardname.Tinkers_Oil:
                Tinkers_Oil(spell);
                break;

            case Card.Cardname.Antique_Healbot:
                Antique_Healbot(spell);
                break;
            case Card.Cardname.Azure_Drake:
                Azure_Drake(spell);
                break;

            case Card.Cardname.Sprint:
                Sprint(spell);
                break;

            case Card.Cardname.Dr_Boom:
                Dr_Boom(spell);
                break;

            case Card.Cardname.Inner_Rage:
                Inner_Rage(spell);
                break;

            case Card.Cardname.Execute:
                Execute(spell);
                break;
                
            case Card.Cardname.Whirlwind:
                Whirlwind(spell);
                break;

            case Card.Cardname.Battle_Rage:
                Battle_Rage(spell);
                break;

            case Card.Cardname.Slam:
                Slam(spell);
                break;

            case Card.Cardname.Cruel_Taskmaster:
                Cruel_Taskmaster(spell);
                break;

            case Card.Cardname.Gnomish_Inventor:
                Gnomish_Inventor(spell);
                break;
                
            case Card.Cardname.Ice_Lance:
                Ice_Lance(spell);
                break;
            case Card.Cardname.Frost_Nova:
                Frost_Nova(spell);
                break;
            case Card.Cardname.Arcane_Intellect:
                Arcane_Intellect(spell);
                break;
            case Card.Cardname.Fireball:
                Fireball(spell);
                break;
            case Card.Cardname.Flamestrike:
                Flamestrike(spell);
                break;
            case Card.Cardname.Blizzard:
                Blizzard(spell);
                break;
            case Card.Cardname.Pyroblast:
                Pyroblast(spell);
                break;
            case Card.Cardname.Alexstrasza:
                Alexstrasza(spell);
                break;

            default:
                Debug.LogError("MISSING SPELL " + spell.card.card);
                break;
        }
    }

}
