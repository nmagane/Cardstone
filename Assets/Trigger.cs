using UnityEngine;

public class Trigger
{
    public static Type GetPhaseTrigger(Match.Phase phase)
    {
        if ((int)phase >= (int)Type._PHASELIMIT || phase.ToString() != ((Type)(int)phase).ToString())
        {
            Debug.LogError("NO TRIGGER FOUND FOR REQUESTED PHASE");
        }
        return (Type)(int)(phase);
    }
    public enum Type
    {
        NONE,

        OnPlayCard,
        AfterPlayCard,

        OnPlayMinion,
        OnSummonMinion, //Tokens
        AfterPlayMinion,
        AfterSummonMinion,

        OnPlaySpell,
        AfterPlaySpell,

        BeforeAttack,
        AfterAttack,

        BeforeAttackFace,
        AfterAttackFace,

        StartTurn,
        EndTurn,

        OnDrawCard,
        OnDiscardCard,
        OnMillCard,
        OnFatigue,

        AfterHeroPower,

        OnPlayWeapon,
        AfterPlayWeapon,

        OnEquipWeapon,
        AfterEquipWeapon,

        BeforeSwingMinion,
        AfterSwingMinion,

        BeforeSwingFace,
        AfterSwingFace,

        //==============
        _PHASELIMIT,
        //=============SPECIAL EVENTS
        OnDamageTaken,
        OnMinionDamage,

        OnFaceDamage,

        OnHealGiven,
        OnMinionHealed,

        OnFaceHealed,

        Deathrattle,
        OnMinionDeath,

        OnWeaponDeath, 

        OnLethalFaceDamage,
    }

    public enum Ability
    {
        KnifeJuggler,
        AcolyteOfPain,
        YoungPriestess,
        HarvestGolem,
        Emperor_Thaurissan,
        Loatheb,
        Millhouse,
        Preparation_Cast,
        Archmage_Antonidas,
        Ice_Block,
        Noble_Sacrifice,
        Ice_Barrier
    }
    public enum Side
    {
        Friendly,
        Enemy,
        Both,
    }
    public int playOrder = 0;
    public Type type;
    public Ability ability;
    public Side side;
    public Minion minion;
    public Secret secret;

    public bool CheckTrigger(Type t, Side s, CastInfo spell)
    {
        if (type != t) return false;

        //DEAD MINIONS CAN'T TRIGGER
        if (minion.DEAD && t != Type.Deathrattle) return false;
        switch (t)
        {
            case Type.OnPlayCard:
            case Type.OnPlaySpell:
            case Type.OnPlayMinion:
            case Type.OnSummonMinion:
            case Type.AfterPlayCard:
            case Type.AfterPlayMinion:
            case Type.AfterSummonMinion:
            case Type.AfterPlaySpell:
            case Type.OnMinionDeath:
                //Does not trigger on self
                if (minion == spell.minion) return false;
                break;
            case Type.Deathrattle:
                if (minion == spell.minion) return true;
                else return false;
                /*
            case Type.OnDamageTaken:
                if (minion == spell.minion) return true;
                break;
                */

        }
        return (side == Side.Both || s == side);
    }
    public void ActivateTrigger(Match match, ref CastInfo spell)
    {
        switch (ability)
        {
            case Ability.KnifeJuggler:
                TriggerEffects.KnifeJuggler(match, minion);
                break;
            case Ability.AcolyteOfPain:
                TriggerEffects.AcolyteOfPain(match, minion);
                break;
            case Ability.YoungPriestess:
                TriggerEffects.YoungPriestess(match, minion);
                break;
            case Ability.HarvestGolem:
                TriggerEffects.HarvestGolem(match, minion);
                break;
            case Ability.Emperor_Thaurissan:
                TriggerEffects.Emperor_Thaurissan(match, minion);
                break;
            case Ability.Loatheb:
                TriggerEffects.Loatheb(match, minion, this);
                break;
            case Ability.Millhouse:
                TriggerEffects.Millhouse(match, minion, this);
                break;
            case Ability.Preparation_Cast:
                TriggerEffects.Prep_Cast(match, minion, this);
                break;
            case Ability.Archmage_Antonidas:
                TriggerEffects.Archmage_Antonidas(match, minion, this);
                break;
            case Ability.Ice_Barrier:
                TriggerEffects.Ice_Barrier(match, minion, this);
                break;
            case Ability.Ice_Block:
                TriggerEffects.Ice_Block(match, minion, this);
                break;
            case Ability.Noble_Sacrifice:
                TriggerEffects.Noble_Sacrifice(match, this, spell);
                break;
        }

    }

    public Trigger(Type typ, Side s, Ability abil, Minion owner, int order = 0)
    {
        type = typ;
        side = s;
        ability = abil;
        minion = owner;
        playOrder = order;
    }
}