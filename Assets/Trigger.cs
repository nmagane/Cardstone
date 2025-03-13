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
        DrawCard,

        KnifeJuggler,
        Acolyte_of_Pain,
        YoungPriestess,
        HarvestGolem,
        Emperor_Thaurissan,
        Loatheb,
        Millhouse,
        Preparation_Cast,
        Archmage_Antonidas,
        Ice_Block,
        Noble_Sacrifice,
        Ice_Barrier,
        Warsong_Commander,
        Grim_Patron,
        Violet_Teacher,
        Boom_Bot,
        Armorsmith,
        Unstable_Ghoul,
        Frothing_Berserker,
        Doomsayer,
        Mad_Scientist,
        Power_Overwhelming,
        Imp_Gang_Boss,
        Haunted_Creeper,
        Nerubian_Egg,
        Voidcaller,

        Zombie_Chow,
        Sludge_Belcher,
        Shade_of_Naxxrammas,
        Sylvanas_Windrunner,
        Baron_Geddon,
        Ragnaros,
        Dark_Cultist,
        Lightwarden,
        Deathlord,
        Piloted_Shredder,
        Explosive_Sheep,
        One_Eyed_Cheat,
        Ships_Cannon,
        Master_Swordsmith,
        Mana_Addict,
        Lorewalker_Cho,
        Nat_Pagle,
        Scavenging_Hyena,
        Starving_Buzzard,
        Echoing_Ooze,
        Pint_Sized_Summoner,
        Pint_Sized_Exhaust,
        Mana_Wyrm,
        Ethereal_Arcanist,
        Lightwell,
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
    public bool invisible = false;
    public Player player => minion.player;
    public Match match => minion.player.match;

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
            case Type.AfterPlayCard:
            case Type.AfterPlayMinion:
            case Type.AfterSummonMinion:
            case Type.AfterPlaySpell:
            case Type.OnMinionDeath:
                //Does not trigger on self
                switch (ability)
                {
                    case Ability.Pint_Sized_Exhaust:
                        //this triggers on self play
                        break;
                    default:
                        if (minion == spell.minion) return false;
                        break;
                }
                switch (ability)
                {
                    case Ability.Scavenging_Hyena:
                        if (Database.GetCardData(spell.minion.card).tribe!=Card.Tribe.Beast) return false;
                        break;
                }
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
            case Ability.DrawCard:
                TriggerEffects.DrawCard(match, this, spell);
                break;
            case Ability.KnifeJuggler:
                TriggerEffects.KnifeJuggler(match, minion);
                break;
            case Ability.Acolyte_of_Pain:
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
            case Ability.Warsong_Commander:
                TriggerEffects.Warsong_Commander(match, this, spell);
                break;
            case Ability.Grim_Patron:
                TriggerEffects.Grim_Patron(match, this, spell);
                break;
            case Ability.Violet_Teacher:
                TriggerEffects.Violet_Teacher(match, this, spell);
                break;
            case Ability.Boom_Bot:
                TriggerEffects.Boom_Bot(match, this, spell);
                break;
            case Ability.Armorsmith:
                TriggerEffects.Armorsmith(match, this, spell);
                break;
            case Ability.Unstable_Ghoul:
                TriggerEffects.Unstable_Ghoul(match, this, spell);
                break;
            case Ability.Mana_Wyrm:
                TriggerEffects.Mana_Wyrm(match, this, spell);
                break;
            case Ability.Ethereal_Arcanist:
                TriggerEffects.Ethereal_Arcanist(match, this);
                break;
            case Ability.Frothing_Berserker:
                TriggerEffects.Frothing_Berserker(match, this, spell);
                break;
            case Ability.Doomsayer:
                TriggerEffects.Doomsayer(match, this, spell);
                break;
            case Ability.Mad_Scientist:
                TriggerEffects.Mad_Scientist(match, this, spell);
                break;
            case Ability.Power_Overwhelming:
                TriggerEffects.Power_Overwhelming(match, this, spell);
                break;
            case Ability.Imp_Gang_Boss:
                TriggerEffects.Imp_Gang_Boss(match, this, spell);
                break;
            case Ability.Haunted_Creeper:
                TriggerEffects.Haunted_Creeper(match, this, spell);
                break;
            case Ability.Nerubian_Egg:
                TriggerEffects.Nerubian_Egg(match, this);
                break;
            case Ability.Voidcaller:
                TriggerEffects.Voidcaller(match, this);
                break;

            case Ability.Zombie_Chow:
                TriggerEffects.Zombie_Chow(match, this);
                break;
            case Ability.Sludge_Belcher:
                TriggerEffects.Sludge_Belcher(match, this);
                break;
            case Ability.Shade_of_Naxxrammas:
                TriggerEffects.Shade_of_Naxxrammas(match, this);
                break;
            case Ability.Sylvanas_Windrunner:
                TriggerEffects.Sylvanas_Windrunner(match, this);
                break;
            case Ability.Baron_Geddon:
                TriggerEffects.Baron_Geddon(match, this);
                break;
            case Ability.Ragnaros:
                TriggerEffects.Ragnaros(match, this.minion);
                break;
            case Ability.Dark_Cultist:
                TriggerEffects.Dark_Cultist(match, this.minion);
                break;
            case Ability.Lightwarden:
                TriggerEffects.Lightwarden(match, this.minion);
                break;
            case Ability.Deathlord:
                TriggerEffects.Deathlord(match, this.minion);
                break;
            case Ability.Piloted_Shredder:
                TriggerEffects.Piloted_Shredder(match, this.minion);
                break;
            case Ability.Explosive_Sheep:
                TriggerEffects.Explosive_Sheep(match, this);
                break;
            case Ability.One_Eyed_Cheat:
                TriggerEffects.One_Eyed_Cheat(match, this,spell);
                break;
            case Ability.Ships_Cannon:
                TriggerEffects.Ships_Cannon(match, this, spell);
                break;
            case Ability.Master_Swordsmith:
                TriggerEffects.Master_Swordsmith(match, minion);
                break;
            case Ability.Mana_Addict:
                TriggerEffects.Mana_Addict(match, this);
                break;
            case Ability.Lorewalker_Cho:
                TriggerEffects.Lorewalker_Cho(match, this, spell);
                break;
            case Ability.Nat_Pagle:
                TriggerEffects.Nat_Pagle(match, this);
                break;
            case Ability.Scavenging_Hyena:
                TriggerEffects.Scavenging_Hyena(match, this, spell);
                break;
            case Ability.Starving_Buzzard:
                TriggerEffects.Starving_Buzzard(match, this, spell);
                break;
            case Ability.Echoing_Ooze:
                TriggerEffects.Echoing_Ooze(match, this);
                break;
            case Ability.Lightwell:
                TriggerEffects.Lightwell(match, this);
                break;
            case Ability.Pint_Sized_Exhaust:
                TriggerEffects.Pint_Sized_Exhaust(match, minion);
                break;
            case Ability.Pint_Sized_Summoner:
                TriggerEffects.Pint_Sized_Summoner(match, minion);
                break;
            default:
                Debug.LogError("MISSING TRIGGER ABILITY");
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
        switch (abil)
        {
            case Ability.Pint_Sized_Exhaust:
            case Ability.Nat_Pagle:
            case Ability.Starving_Buzzard:
            case Ability.Ships_Cannon:
            case Ability.One_Eyed_Cheat:
            case Ability.Warsong_Commander:
                invisible = true;
                break;
        }
    }
}