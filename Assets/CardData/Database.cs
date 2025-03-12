using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public partial class Database
{
    public class CardInfo
    {
        public string name="UNDEFINED CARD";
        public string text="UNDEFINED CARD";

        public Card.Cardname cardname = Card.Cardname.Cardback;
        public Card.Class classType = Card.Class.Neutral;
        public Card.Tribe tribe = Card.Tribe.None;

        public int manaCost=1;
        public int damage=1;
        public int health=1;
        public int spellDamage = 0;
        public int comboSpellDamage = 0;

        public bool SPELL = false;
        public bool MINION = false;
        public bool SECRET = false;
        public bool WEAPON = false;

        public bool BATTLECRY = false;
        public bool TARGETED = false;
        public bool COMBO = false;
        public bool COMBO_TARGETED = false; //for cards that are only targeted when combo is active (SI7)
        public bool CHOOSE = false;
        public List<Card.Cardname> choices = new();

        public bool LEGENDARY = false;
        public bool TOKEN = false;

        public Board.EligibleTargets eligibleTargets = Board.EligibleTargets.AllCharacters;

        public List<Aura.Type> auras = new List<Aura.Type>();
        public List<Aura.Type> cardAuras = new List<Aura.Type>();
        public List<(Trigger.Type, Trigger.Side, Trigger.Ability)> triggers = new List<(Trigger.Type, Trigger.Side, Trigger.Ability)>();
        public CardInfo()
        {

        }
    }
    public static CardInfo GetCardData(Card.Cardname card)
    {
        CardInfo info = GetCardDataInternal(card);
        info.cardname = card;
        return info;
    }
    static CardInfo GetCardDataInternal(Card.Cardname card)
    {

        CardInfo info = new CardInfo();
        switch (card)
        {
            case Card.Cardname.Coin:
                return Coin();
            case Card.Cardname.Argent_Squire:
                return Argent_Squire();

            case Card.Cardname.Knife_Juggler:
                return Knife_Juggler();

            case Card.Cardname.Amani_Berserker:
                return Amani_Berserker();

            case Card.Cardname.Harvest_Golem:
                return Harvest_Golem();

            case Card.Cardname.Damaged_Golem:
                return Damaged_Golem();

            case Card.Cardname.Young_Priestess:
                return Young_Priestess();

            case Card.Cardname.Shieldbearer:
                return Shieldbearer();

            case Card.Cardname.Defender_of_Argus:
                return Defender_of_Argus();

            case Card.Cardname.Dire_Wolf_Alpha:
                return Dire_Wolf_Alpha();

            case Card.Cardname.Shattered_Sun_Cleric:
                return Shattered_Sun_Cleric();

            case Card.Cardname.Abusive_Sergeant:
                return Abusive_Sergeant();

            case Card.Cardname.Dark_Iron_Dwarf:
                return Dark_Iron_Dwarf();

            case Card.Cardname.Ironbeak_Owl:
                return Ironbeak_Owl();

            case Card.Cardname.Flame_Imp:
                return Flame_Imp();

            case Card.Cardname.Voidwalker:
                return Voidwalker();

            case Card.Cardname.Soulfire:
                return Soulfire();

            case Card.Cardname.Doomguard:
                return Doomguard();

            case Card.Cardname.Lifetap:
                return Lifetap();

            case Card.Cardname.Ping:
                return Ping();

            case Card.Cardname.Emperor_Thaurissan:
                return Emperor_Thaurissan();

            case Card.Cardname.Mana_Wraith:
                return Mana_Wraith();
                
            case Card.Cardname.Loatheb:
                return Loatheb();

            case Card.Cardname.Stormwind_Champion:
                return Stormwind_Champion();

            case Card.Cardname.Preparation:
                return Preparation();

            case Card.Cardname.Millhouse_Manastorm:
                return Millhouse_Manastorm();

            case Card.Cardname.Hunters_Mark:
                return Hunters_Mark();

            case Card.Cardname.Crazed_Alchemist:
                return Crazed_Alchemist();

            case Card.Cardname.Chillwind_Yeti:
                return Chillwind_Yeti();

            case Card.Cardname.Dagger:
                return Dagger();

            case Card.Cardname.Heroic_Strike:
                return Heroic_Strike();
                
            case Card.Cardname.Deadly_Poison:
                return Deadly_Poison();

            case Card.Cardname.Blade_Flurry:
                return Blade_Flurry();

            case Card.Cardname.Armor_Up:
                return Armor_Up();

            case Card.Cardname.SI7_Agent:
                return SI7_Agent();
                
            case Card.Cardname.Eviscerate:
                return Eviscerate();

            case Card.Cardname.Archmage_Antonidas:
                return Archmage_Antonidas();

            case Card.Cardname.Sap:
                return Sap();

            case Card.Cardname.Ogre_Magi:
                return Ogre_Magi();
                
            case Card.Cardname.Ice_Barrier:
                return Ice_Barrier();

            case Card.Cardname.Mage_Secret:
                return Mage_Secret();

            case Card.Cardname.Hunter_Secret:
                return Hunter_Secret();

            case Card.Cardname.Paladin_Secret:
                return Paladin_Secret();

            case Card.Cardname.Frostbolt:
                return Frostbolt();

            case Card.Cardname.Southsea_Deckhand:
                return Southsea_Deckhand();

            case Card.Cardname.Warsong_Commander:
                return Warsong_Commander();

            case Card.Cardname.Grim_Patron:
                return Grim_Patron();

            case Card.Cardname.Mountain_Giant:
                return Mountain_Giant();
            case Card.Cardname.Sea_Giant:
                return Sea_Giant();
            case Card.Cardname.Molten_Giant:
                return Molten_Giant();

            case Card.Cardname.Dagger_Mastery:
                return Dagger_Mastery();
                
            case Card.Cardname.Backstab:
                return Backstab();
                
            case Card.Cardname.Bloodmage_Thalnos:
                return Bloodmage_Thalnos();

            case Card.Cardname.Fan_of_Knives:
                return Fan_of_Knives();

            case Card.Cardname.Earthen_Ring_Farseer:
                return Earthen_Ring_Farseer();

            case Card.Cardname.Tinkers_Oil:
                return Tinkers_Oil();
                
            case Card.Cardname.Violet_Teacher:
                return Violet_Teacher();
           case Card.Cardname.Violet_Apprentice:
                return Violet_Apprentice();

           case Card.Cardname.Antique_Healbot:
                return Antique_Healbot();

           case Card.Cardname.Azure_Drake:
                return Azure_Drake();

           case Card.Cardname.Coldlight_Oracle:
                return Coldlight_Oracle();

           case Card.Cardname.Youthful_Brewmaster:
                return Youthful_Brewmaster();

           case Card.Cardname.King_Mukla:
                return King_Mukla();

           case Card.Cardname.Bananas:
                return Bananas();

           case Card.Cardname.Leeroy_Jenkins:
                return Leeroy_Jenkins();

           case Card.Cardname.Whelp:
                return Whelp();

           case Card.Cardname.Sprint:
                return Sprint();

            case Card.Cardname.Dr_Boom:
                return Dr_Boom();
            case Card.Cardname.Boom_Bot:
                return Boom_Bot();

            case Card.Cardname.Inner_Rage:
                return Inner_Rage();

            case Card.Cardname.Execute:
                return Execute();

            case Card.Cardname.Whirlwind:
                return Whirlwind();

            case Card.Cardname.Fiery_War_Axe:
                return Fiery_War_Axe();

            case Card.Cardname.Battle_Rage:
                return Battle_Rage();

            case Card.Cardname.Slam:
                return Slam();
            
            case Card.Cardname.Armorsmith:
                return Armorsmith();
            
            case Card.Cardname.Cruel_Taskmaster:
                return Cruel_Taskmaster();

            case Card.Cardname.Unstable_Ghoul:
                return Unstable_Ghoul();
            
            case Card.Cardname.Acolyte_of_Pain:
                return Acolyte_of_Pain();
                
            case Card.Cardname.Frothing_Berserker:
                return Frothing_Berserker();
                
            case Card.Cardname.Deaths_Bite:
                return Deaths_Bite();
            
            case Card.Cardname.Gnomish_Inventor:
                return Gnomish_Inventor();
            
            case Card.Cardname.Dread_Corsair:
                return Dread_Corsair();

            case Card.Cardname.Ice_Lance:
                return Ice_Lance();

            case Card.Cardname.Doomsayer:
                return Doomsayer();

            case Card.Cardname.Frost_Nova:
                return Frost_Nova();

            case Card.Cardname.Arcane_Intellect:
                return Arcane_Intellect();

            case Card.Cardname.Loot_Hoarder:
                return Loot_Hoarder();

            case Card.Cardname.Ice_Block:
                return Ice_Block();
            
            case Card.Cardname.Fireball:
                return Fireball();

            case Card.Cardname.Flamestrike:
                return Flamestrike();

            case Card.Cardname.Blizzard:
                return Blizzard();

            case Card.Cardname.Pyroblast:
                return Pyroblast();

            case Card.Cardname.Alexstrasza:
                return Alexstrasza();

            case Card.Cardname.Mad_Scientist:
                return Mad_Scientist();

            case Card.Cardname.Arcane_Missiles:
                return Arcane_Missiles();

            case Card.Cardname.Arcane_Explosion:
                return Arcane_Explosion();

            case Card.Cardname.Mirror_Image:
                return Mirror_Image();

            case Card.Cardname.Mirror_Image_Token:
                return Mirror_Image_Token();

            case Card.Cardname.Cone_of_Cold:
                return Cone_of_Cold();

            case Card.Cardname.Mana_Wyrm:
                return Mana_Wyrm();

            case Card.Cardname.Ethereal_Arcanist:
                return Ethereal_Arcanist();
                
            case Card.Cardname.Mortal_Coil:
                return Mortal_Coil();

            case Card.Cardname.Power_Overwhelming:
                return Power_Overwhelming();

            case Card.Cardname.Imp_Gang_Boss:
                return Imp_Gang_Boss();

            case Card.Cardname.Imp:
                return Imp();
                
            case Card.Cardname.Implosion:
                return Implosion();


            case Card.Cardname.Haunted_Creeper:
                return Haunted_Creeper();
            case Card.Cardname.Spectral_Spider:
                return Spectral_Spider();

            case Card.Cardname.Nerubian_Egg:
                return Nerubian_Egg();
            case Card.Cardname.Nerubian:
                return Nerubian();

            case Card.Cardname.Voidcaller:
                return Voidcaller();

            case Card.Cardname.Darkbomb:
                return Darkbomb();
            case Card.Cardname.Hellfire:
                return Hellfire();
            case Card.Cardname.Shadowflame:
                return Shadowflame();
            case Card.Cardname.Siphon_Soul:
                return Siphon_Soul();
             
            case Card.Cardname.Zombie_Chow:
                return Zombie_Chow();
            case Card.Cardname.Blackwing_Technician:
                return Blackwing_Technician();
            case Card.Cardname.Blackwing_Corruptor:
                return Blackwing_Corruptor();
            case Card.Cardname.Twilight_Drake:
                return Twilight_Drake();
            case Card.Cardname.Big_Game_Hunter:
                return Big_Game_Hunter();
            case Card.Cardname.Sludge_Belcher:
                return Sludge_Belcher();
            case Card.Cardname.Putrid_Slime:
                return Putrid_Slime();
            case Card.Cardname.Malygos:
                return Malygos();
            case Card.Cardname.Sorcerers_Apprentice:
                return Sorcerers_Apprentice();
            case Card.Cardname.Core_Hound:
                return Core_Hound();
            case Card.Cardname.Sunfury_Protector:
                return Sunfury_Protector();
            case Card.Cardname.Acidic_Swamp_Ooze:
                return Acidic_Swamp_Ooze();
            case Card.Cardname.Ancient_Watcher:
                return Ancient_Watcher();
            case Card.Cardname.Kobold_Geomancer:
                return Kobold_Geomancer();
            case Card.Cardname.Novice_Engineer:
                return Novice_Engineer();

            case Card.Cardname.Edwin_VanCleef:
                return Edwin_VanCleef();
            case Card.Cardname.Shadowstep:
                return Shadowstep();
            case Card.Cardname.Shiv:
                return Shiv();
            case Card.Cardname.Gadgetzan_Auctioneer:
                return Gadgetzan_Auctioneer(); 
            case Card.Cardname.Shade_of_Naxxrammas:
                return Shade_of_Naxxrammas();  
            case Card.Cardname.Harrison_Jones:
                return Harrison_Jones();

            case Card.Cardname.Fatigue:
                return Fatigue();
                
            case Card.Cardname.Wrath:
                return Wrath();
            case Card.Cardname.Wrath_Big:
                return Wrath_Big(); 
            case Card.Cardname.Wrath_Small:
                return Wrath_Small();

            case Card.Cardname.Ancient_of_War:
                return Ancient_of_War();
            case Card.Cardname.Ancient_of_War_Attack:
                return Ancient_of_War_Attack();
            case Card.Cardname.Ancient_of_War_Taunt:
                return Ancient_of_War_Taunt();

            case Card.Cardname.Keeper_of_the_Grove:
                return Keeper_of_the_Grove();
            case Card.Cardname.Keeper_of_the_Grove_Damage:
                return Keeper_of_the_Grove_Damage();
            case Card.Cardname.Keeper_of_the_Grove_Silence:
                return Keeper_of_the_Grove_Silence();

            case Card.Cardname.Polymorph:
                return Polymorph();
            case Card.Cardname.Sheep:
                return Sheep();
                
            case Card.Cardname.Sylvanas_Windrunner:
                return Sylvanas_Windrunner();
            case Card.Cardname.Mind_Control_Tech:
                return Mind_Control_Tech();
                
            case Card.Cardname.Shapeshift:
                return Shapeshift();

            case Card.Cardname.Innervate:
                return Innervate();
            case Card.Cardname.Wild_Growth:
                return Wild_Growth();
            case Card.Cardname.Excess_Mana:
                return Excess_Mana();
            case Card.Cardname.Savage_Roar:
                return Savage_Roar();
                
            case Card.Cardname.Druid_of_the_Flame:
                return Druid_of_the_Flame();
            case Card.Cardname.Druid_of_the_Flame_Attack:
                return Druid_of_the_Flame_Attack();
            case Card.Cardname.Druid_of_the_Flame_Health:
                return Druid_of_the_Flame_Health();

            case Card.Cardname.Ancient_of_Lore:
                return Ancient_of_Lore();
            case Card.Cardname.Ancient_of_Lore_Draw:
                return Ancient_of_Lore_Draw();
            case Card.Cardname.Ancient_of_Lore_Heal:
                return Ancient_of_Lore_Heal();

            case Card.Cardname.Druid_of_the_Claw:
                return Druid_of_the_Claw();
            case Card.Cardname.Druid_of_the_Claw_Charge:
                return Druid_of_the_Claw_Charge();
            case Card.Cardname.Druid_of_the_Claw_Taunt:
                return Druid_of_the_Claw_Taunt();

            case Card.Cardname.Swipe:
                return Swipe();
            case Card.Cardname.Force_of_Nature:
                return Force_of_Nature();
            case Card.Cardname.Naturalize:
                return Naturalize();
            case Card.Cardname.Bite:
                return Bite();
            case Card.Cardname.Healing_Touch:
                return Healing_Touch();
            case Card.Cardname.Starfall:
                return Starfall();
            case Card.Cardname.Starfall_AoE:
                return Starfall_AoE();
            case Card.Cardname.Starfall_Single:
                return Starfall_Single();
            case Card.Cardname.Starfire:
                return Starfire();
            case Card.Cardname.Tree_of_Life:
                return Tree_of_Life();
            case Card.Cardname.Poison_Seeds:
                return Poison_Seeds();
            case Card.Cardname.Grove_Tender:
                return Grove_Tender();
            case Card.Cardname.Grove_Tender_Mana:
                return Grove_Tender_Mana();
            case Card.Cardname.Grove_Tender_Draw:
                return Grove_Tender_Draw();

            case Card.Cardname.Treant:
                return Treant();
            case Card.Cardname.Treant_Charge:
                return Treant_Charge();
            case Card.Cardname.Treant_Taunt:
                return Treant_Taunt();

            case Card.Cardname.Cenarius:
                return Cenarius();
            case Card.Cardname.Cenarius_Buff:
                return Cenarius_Buff();
            case Card.Cardname.Cenarius_Treants:
                return Cenarius_Treants();


            case Card.Cardname.Shield_Slam:
                return Shield_Slam();
            case Card.Cardname.Shield_Block:
                return Shield_Block();
            case Card.Cardname.Shieldmaiden:
                return Shieldmaiden();
            case Card.Cardname.Revenge:
                return Revenge();
            case Card.Cardname.Brawl:
                return Brawl();
            case Card.Cardname.Grommash_Hellscream:
                return Grommash_Hellscream();

            case Card.Cardname.Baron_Geddon:
                return Baron_Geddon();
            case Card.Cardname.Ragnaros:
                return Ragnaros();

            case Card.Cardname.Lord_Jaraxxus:
                return Lord_Jaraxxus();
            case Card.Cardname.Inferno:
                return Inferno();
            case Card.Cardname.Infernal:
                return Infernal();
            case Card.Cardname.Blood_Fury:
                return Blood_Fury();

            case Card.Cardname.Northshire_Cleric:
                return Northshire_Cleric();
            case Card.Cardname.Lesser_Heal:
                return Lesser_Heal();
            case Card.Cardname.Shadow_Word_Pain:
                return Shadow_Word_Pain();
            case Card.Cardname.Shadow_Word_Death:
                return Shadow_Word_Death();
            case Card.Cardname.Dark_Cultist:
                return Dark_Cultist();
            case Card.Cardname.Cabal_Shadow_Priest:
                return Cabal_Shadow_Priest();
            case Card.Cardname.Holy_Nova:
                return Holy_Nova();
            case Card.Cardname.Holy_Smite:
                return Holy_Smite();
            case Card.Cardname.Circle_of_Healing:
                return Circle_of_Healing();
            case Card.Cardname.Auchenai_Soulpriest:
                return Auchenai_Soulpriest();
            case Card.Cardname.Lightbomb:
                return Lightbomb();
            case Card.Cardname.Mind_Blast:
                return Mind_Blast();
            case Card.Cardname.Resurrect:
                return Resurrect();
            case Card.Cardname.Silence:
                return Silence();
            case Card.Cardname.Divine_Spirit:
                return Divine_Spirit();
            case Card.Cardname.Inner_Fire:
                return Inner_Fire();
            case Card.Cardname.Velens_Chosen:
                return Velens_Chosen();
            case Card.Cardname.Thoughtsteal:
                return Thoughtsteal();
            case Card.Cardname.Shadow_of_Nothing:
                return Shadow_of_Nothing();
            case Card.Cardname.Lightwarden:
                return Lightwarden();
            case Card.Cardname.Wild_Pyromancer:
                return Wild_Pyromancer();
            case Card.Cardname.Deathlord:
                return Deathlord();
            case Card.Cardname.Injured_Blademaster:
                return Injured_Blademaster();
            case Card.Cardname.Light_of_the_Naaru:
                return Light_of_the_Naaru();
            case Card.Cardname.Power_Word_Shield:
                return Power_Word_Shield();
                
            case Card.Cardname.Piloted_Shredder:
                return Piloted_Shredder();
            case Card.Cardname.Annoy_o_Tron:
                return Annoy_o_Tron();

            case Card.Cardname.Anodized_Robo_Cub:
                return Anodized_Robo_Cub();
            case Card.Cardname.Anodized_Robo_Cub_Attack:
                return Anodized_Robo_Cub_Attack();
            case Card.Cardname.Anodized_Robo_Cub_Health:
                return Anodized_Robo_Cub_Health();

            case Card.Cardname.Explosive_Sheep:
                return Explosive_Sheep();
            case Card.Cardname.Gilbin_Stalker:
                return Gilbin_Stalker();
            case Card.Cardname.Goblin_Auto_Barber:
                return Goblin_Auto_Barber();
            case Card.Cardname.Micro_Machine:
                return Micro_Machine();
            case Card.Cardname.Mechwarper:
                return Mechwarper();
            case Card.Cardname.One_eyed_Cheat:
                return One_eyed_Cheat();
            case Card.Cardname.Puddlestomper:
                return Puddlestomper();
            case Card.Cardname.Queen_of_Pain:
                return Queen_of_Pain();
            case Card.Cardname.Recombobulator:
                return Recombobulator();
            case Card.Cardname.Shadowboxer:
                return Shadowboxer();
            case Card.Cardname.Shielded_Minibot:
                return Shielded_Minibot();
            case Card.Cardname.Ships_Cannon:
                return Ships_Cannon();
            case Card.Cardname.Shrinkmeister:
                return Shrinkmeister();
            case Card.Cardname.Snowchugger:
                return Snowchugger();
            case Card.Cardname.Steamwheedle_Sniper:
                return Steamwheedle_Sniper();
            case Card.Cardname.Stonesplinter_Trogg:
                return Stonesplinter_Trogg();
            case Card.Cardname.Vitality_Totem:
                return Vitality_Totem();
            case Card.Cardname.Whirling_Zapomatic:
                return Whirling_Zapomatic();
            case Card.Cardname.Argent_Protector:
                return Argent_Protector();
            case Card.Cardname.Bloodfen_Raptor:
                return Bloodfen_Raptor();
            case Card.Cardname.Bloodsail_Raider:
                return Bloodsail_Raider();
            case Card.Cardname.Bluegill_Warrior:
                return Bluegill_Warrior();
            case Card.Cardname.Captains_Parrot:
                return Captains_Parrot();
            case Card.Cardname.Defias_Ringleader:
                return Defias_Ringleader();
            case Card.Cardname.Defias_Bandit:
                return Defias_Bandit();
            case Card.Cardname.Faerie_Dragon:
                return Faerie_Dragon();
            case Card.Cardname.Succubus:
                return Succubus();
            case Card.Cardname.Flametongue_Totem:
                return Flametongue_Totem();
            case Card.Cardname.Frostwolf_Grunt:
                return Frostwolf_Grunt();
            case Card.Cardname.Lightwell:
                return Lightwell();
            case Card.Cardname.Lorewalker_Cho:
                return Lorewalker_Cho();
            case Card.Cardname.Mad_Bomber:
                return Mad_Bomber();
            case Card.Cardname.Mana_Addict:
                return Mana_Addict();
            case Card.Cardname.Master_Swordsmith:
                return Master_Swordsmith();
            case Card.Cardname.Murloc_Tidehunter:
                return Murloc_Tidehunter();
            case Card.Cardname.Murloc_Scout:
                return Murloc_Scout();
            case Card.Cardname.Nat_Pagle:
                return Nat_Pagle();
            case Card.Cardname.Patient_Assassin:
                return Patient_Assassin();
            case Card.Cardname.Pint_Sized_Summoner:
                return Pint_Sized_Summoner();
            case Card.Cardname.River_Crocolisk:
                return River_Crocolisk();
            case Card.Cardname.Scavenging_Hyena:
                return Scavenging_Hyena();
            case Card.Cardname.Starving_Buzzard:
                return Starving_Buzzard();
            case Card.Cardname.Echoing_Ooze:
                return Echoing_Ooze();
            case Card.Cardname.Nerubar_Weblord:
                return Nerubar_Weblord();
                
            default:
                Debug.LogError("ERROR: UNDEFINED CARD: " + card);
                return null;
        }
    }
    public static Card.Cardname GetClassSecret(Card.Class c)
    {
        switch(c)
        {
            case Card.Class.Mage:
                return Card.Cardname.Mage_Secret;

            case Card.Class.Hunter:
                return Card.Cardname.Hunter_Secret;

            case Card.Class.Paladin:
                return Card.Cardname.Paladin_Secret;
            /*
        case Card.Class.Warrior:
            break;
        case Card.Class.Rogue:
            break;
        case Card.Class.Warlock:
            break;
        case Card.Class.Priest:
            break;
        case Card.Class.Druid:
            break;
        case Card.Class.Shaman:
            break;
        case Card.Class.Neutral:*/
            default:
                Debug.LogError("CLASS SECRET UNKNOWN");
                return Card.Cardname.Cardback;
        }
    }
    public static Card.Cardname GetClassHeroPower(Card.Class c)
    {
        switch(c)
        {
            case Card.Class.Mage:
                return Card.Cardname.Ping;

            case Card.Class.Warrior:
                return Card.Cardname.Armor_Up;

            case Card.Class.Rogue:
                return Card.Cardname.Dagger_Mastery;

            case Card.Class.Warlock:
                return Card.Cardname.Lifetap;

            case Card.Class.Druid:
                return Card.Cardname.Shapeshift;

            case Card.Class.Priest:
                return Card.Cardname.Lesser_Heal;
            /*
           case Card.Class.Hunter:
               break;
           case Card.Class.Paladin:
               break;
           case Card.Class.Shaman:
               break;
           case Card.Class.Neutral:*/
            default:
                Debug.LogError("CLASS SECRET UNKNOWN");
                return Card.Cardname.Cardback;
        }
    }
    static CardInfo Coin()
    { 
        CardInfo c = new();

        c.name = "The Coin";
        c.text = "Gain 1 mana this turn only.";
        c.manaCost = 0;
        c.SPELL = true;
        c.TARGETED = false;
        c.TOKEN = true;

        return c;
    }
    static CardInfo Fatigue()
    { 
        CardInfo c = new();

        c.name = "Fatigue";
        c.text = "Out of cards!\nTake {0} damage.";
        c.manaCost = 0;
        c.spellDamage = 1;
        c.SPELL = true;
        c.TOKEN = true;

        return c;
    }

    static CardInfo Argent_Squire()
    { 
        CardInfo c = new();

        c.name = "Argent Squire";
        c.text = "Divine Shield.";

        c.manaCost = 1;
        c.damage = 1;
        c.health = 1;

        c.MINION = true;
        c.auras.Add(Aura.Type.Shield);

        return c;
    }
    static CardInfo Shieldbearer()
    { 
        CardInfo c = new();

        c.name = "Shieldbearer";
        c.text = "Taunt.";

        c.manaCost = 1;
        c.damage = 0;
        c.health = 4;

        c.MINION = true;
        c.auras.Add(Aura.Type.Taunt);

        return c;
    }
    static CardInfo Amani_Berserker()
    { 
        CardInfo c = new();

        c.name = "Amani Berserker";
        c.text = "Enrage: +3 Attack.";

        c.manaCost = 2;
        c.damage = 2;
        c.health = 3;

        c.MINION = true;
        c.auras.Add(Aura.Type.Amani);

        return c;
    }
    static CardInfo Dire_Wolf_Alpha()
    { 
        CardInfo c = new();

        c.name = "Dire Wolf Alpha";
        c.text = "Adjacent minions have +1 attack.";

        c.manaCost = 2;
        c.damage = 2;
        c.health = 2;
        c.tribe = Card.Tribe.Beast;

        c.MINION = true;
        c.auras.Add(Aura.Type.DireWolfAlpha);

        return c;
    } 
    static CardInfo Abusive_Sergeant()
    { 
        CardInfo c = new();

        c.name = "Abusive Sergeant";
        c.text = "Battlecry: Give a minion +2 attack this turn.";

        c.manaCost = 1;
        c.damage = 2;
        c.health = 1;

        c.MINION = true;
        c.BATTLECRY = true;
        c.TARGETED = true;

        c.eligibleTargets = Board.EligibleTargets.AllMinions;

        return c;
    }
    static CardInfo Ironbeak_Owl()
    { 
        CardInfo c = new();

        c.name = "Ironbeak Owl";
        c.text = "Battlecry: Silence a minion.";

        c.manaCost = 2;
        c.damage = 2;
        c.health = 1;
        c.tribe = Card.Tribe.Beast;

        c.MINION = true;
        c.BATTLECRY = true;
        c.TARGETED = true;
        c.eligibleTargets = Board.EligibleTargets.AllMinions;

        return c;
    }
    static CardInfo Dark_Iron_Dwarf()
    { 
        CardInfo c = new();

        c.name = "Dark Iron Dwarf";
        c.text = "Battlecry: Give a minion +2 attack this turn.";

        c.manaCost = 4;
        c.damage = 4;
        c.health = 4;

        c.MINION = true;
        c.BATTLECRY = true;
        c.TARGETED = true;
        c.eligibleTargets = Board.EligibleTargets.AllMinions;

        return c;
    }
    static CardInfo Shattered_Sun_Cleric()
    { 
        CardInfo c = new();

        c.name = "Shattered Sun Cleric";
        c.text = "Battlecry: Give a friendly minion +1/+1.";

        c.manaCost = 3;
        c.damage = 3;
        c.health = 2;

        c.MINION = true;
        c.BATTLECRY = true;
        c.TARGETED = true;
        c.eligibleTargets = Board.EligibleTargets.FriendlyMinions;

        return c;
    }
    
    static CardInfo Defender_of_Argus()
    { 
        CardInfo c = new();

        c.name = "Defender of Argus";
        c.text = "Battlecry: Give adjacent minions +1/+1 and Taunt.";

        c.manaCost = 4;
        c.damage = 2;
        c.health = 3;

        c.MINION = true;
        c.BATTLECRY = true;
        c.TARGETED = false;

        return c;
    }
    static CardInfo Knife_Juggler()
    { 
        CardInfo c = new();

        c.name = "Knife Juggler";
        c.text = "After you summon a minion, deal 1 damage to a random enemy.";

        c.manaCost = 2;
        c.damage = 3;
        c.health = 2;

        c.MINION = true;

        c.triggers.Add((Trigger.Type.AfterPlayMinion, Trigger.Side.Friendly, Trigger.Ability.KnifeJuggler));
        c.triggers.Add((Trigger.Type.AfterSummonMinion, Trigger.Side.Friendly, Trigger.Ability.KnifeJuggler));

        return c;
    }
    static CardInfo Young_Priestess()
    { 
        CardInfo c = new();

        c.name = "Young Priestess";
        c.text = "End of Turn: Give another random friendly minion +1 health.";

        c.manaCost = 1;
        c.damage = 2;
        c.health = 1;

        c.MINION = true;

        c.triggers.Add((Trigger.Type.EndTurn, Trigger.Side.Friendly, Trigger.Ability.YoungPriestess));

        return c;
    }
    static CardInfo Harvest_Golem()
    { 
        CardInfo c = new();

        c.name = "Harvest Golem";
        c.text = "Deathrattle: Summon a 2/1 Damaged Golem.";

        c.manaCost = 3;
        c.damage = 2;
        c.health = 3;

        c.MINION = true;
        c.tribe = Card.Tribe.Mech;

        c.triggers.Add((Trigger.Type.Deathrattle, Trigger.Side.Friendly, Trigger.Ability.HarvestGolem));

        return c;
    }
    static CardInfo Damaged_Golem()
    { 
        CardInfo c = new();

        c.name = "Damaged Golem";
        c.text = "";

        c.manaCost = 1;
        c.damage = 2;
        c.health = 1;

        c.MINION = true;
        c.tribe = Card.Tribe.Mech;
        c.TOKEN = true;

        return c;
    }
    
    static CardInfo Emperor_Thaurissan()
    { 
        CardInfo c = new();

        c.name = "Emperor Thaurissan";
        c.text = "End of Turn: Reduce the cost of cards in your hand by 1.";

        c.manaCost = 6;
        c.damage = 5;
        c.health = 5;

        c.MINION = true;
        c.LEGENDARY = true;

        c.triggers.Add((Trigger.Type.EndTurn, Trigger.Side.Friendly, Trigger.Ability.Emperor_Thaurissan));
        return c;
    }
    static CardInfo Mana_Wraith()
    { 
        CardInfo c = new();

        c.name = "Mana Wraith";
        c.text = "ALL minions cost 1 more.";

        c.manaCost = 2;
        c.damage = 2;
        c.health = 2;

        c.MINION = true;

        c.auras.Add(Aura.Type.Mana_Wraith);
        return c;
    }
    
    static CardInfo Loatheb()
    { 
        CardInfo c = new();

        c.name = "Loatheb";
        c.text = "Battlecry: Enemy spells cost 5 more next turn.";

        c.manaCost = 5;
        c.damage = 5;
        c.health = 5;

        c.MINION = true;
        c.BATTLECRY = true;
        c.LEGENDARY = true;
        return c;
    }
    static CardInfo Stormwind_Champion()
    { 
        CardInfo c = new();

        c.name = "Stormwind Champion";
        c.text = "Ally minions have +1/+1";

        c.manaCost = 6;
        c.damage = 6;
        c.health = 6;

        c.MINION = true;
        c.auras.Add(Aura.Type.StormwindChampion);
        return c;
    }
    static CardInfo Millhouse_Manastorm()
    { 
        CardInfo c = new();

        c.name = "Millhouse Manastorm";
        c.text = "Battlecry: Enemy spells cost 0 next turn.";

        c.manaCost = 2;
        c.damage = 4;
        c.health = 4;

        c.MINION = true;
        c.BATTLECRY = true;
        c.LEGENDARY = true;
        return c;
    }
    static CardInfo Crazed_Alchemist()
    { 
        CardInfo c = new();

        c.name = "Crazed Alchemist";
        c.text = "Battlecry: Swap health and attack of target minion.";

        c.manaCost = 2;
        c.damage = 2;
        c.health = 2;

        c.eligibleTargets = Board.EligibleTargets.AllMinions;

        c.MINION = true;
        c.BATTLECRY = true;
        c.TARGETED = true;
        return c;
    }
    static CardInfo Chillwind_Yeti()
    { 
        CardInfo c = new();

        c.name = "Chillwind Yeti";
        c.text = "";

        c.manaCost = 4;
        c.damage = 4;
        c.health = 5;

        c.MINION = true;

        return c;
    }

    static CardInfo Ogre_Magi()
    { 
        CardInfo c = new();

        c.name = "Ogre Magi";
        c.text = "+1 Spellpower.";

        c.manaCost = 4;
        c.damage = 4;
        c.health = 4;

        c.MINION = true;

        c.auras.Add(Aura.Type.Spellpower);
        return c;
    }
    static CardInfo Southsea_Deckhand()
    { 
        CardInfo c = new();

        c.name = "Southsea Deckhand";
        c.text = "Has Charge while you have a weapon equipped.";

        c.manaCost = 1;
        c.damage = 2;
        c.health = 1;

        c.MINION = true;
        c.tribe = Card.Tribe.Pirate;

        c.auras.Add(Aura.Type.Southsea_Deckhand);
        return c;
    }
    static CardInfo Grim_Patron()
    { 
        CardInfo c = new();

        c.name = "Grim Patron";
        c.text = "Whenever this minion survives damage, summon a Grim Patron";

        c.manaCost = 5;
        c.damage = 3;
        c.health = 3;

        c.MINION = true;

        c.triggers.Add((Trigger.Type.OnDamageTaken, Trigger.Side.Both, Trigger.Ability.Grim_Patron));
        return c;
    }

    static CardInfo Mountain_Giant()
    { 
        CardInfo c = new();

        c.name = "Mountain Giant";
        c.text = "Costs 1 less for each other card in your hand.";

        c.manaCost = 12;
        c.damage = 8;
        c.health = 8;

        c.MINION = true;

        c.cardAuras.Add(Aura.Type.Mountain_Giant);
        return c;
    }
    static CardInfo Sea_Giant()
    { 
        CardInfo c = new();

        c.name = "Sea Giant";
        c.text = "Costs 1 less for each minion on the board.";

        c.manaCost = 10;
        c.damage = 8;
        c.health = 8;

        c.MINION = true;

        c.cardAuras.Add(Aura.Type.Sea_Giant);
        return c;
    }
    static CardInfo Molten_Giant()
    { 
        CardInfo c = new();

        c.name = "Molten Giant";
        c.text = "Costs 1 less for each health you're missing.";

        c.manaCost = 20;
        c.damage = 8;
        c.health = 8;

        c.MINION = true;

        c.cardAuras.Add(Aura.Type.Molten_Giant);
        return c;
    }
    private static CardInfo Bloodmage_Thalnos()
    {
        CardInfo c = new();

        c.name = "Bloodmage Thalnos";
        c.text = "+1 Spellpower.\nDeathrattle: Draw a card.";

        c.manaCost = 2;
        c.damage = 1;
        c.health = 1;

        c.MINION = true;
        c.LEGENDARY = true;

        c.auras.Add(Aura.Type.Spellpower);
        c.triggers.Add((Trigger.Type.Deathrattle, Trigger.Side.Both, Trigger.Ability.DrawCard));

        return c;
    }
    private static CardInfo Earthen_Ring_Farseer()
    {
        CardInfo c = new();

        c.name = "Earthen Ring Farseer";
        c.text = "Battlecry: Restore 3 health.";

        c.manaCost = 3;
        c.damage = 3;
        c.health = 3;

        c.MINION = true;
        c.BATTLECRY = true;
        c.TARGETED = true;

        c.eligibleTargets = Board.EligibleTargets.AllCharacters;


        return c;
    } 
    private static CardInfo Violet_Teacher()
    {
        CardInfo c = new();

        c.name = "Violet Teacher";
        c.text = "Whenever you cast a spell, summon a 1/1 Violet Apprentice";

        c.manaCost = 4;
        c.damage = 3;
        c.health = 5;

        c.MINION = true;

        c.triggers.Add((Trigger.Type.OnPlaySpell, Trigger.Side.Friendly, Trigger.Ability.Violet_Teacher));

        return c;
    }
    private static CardInfo Violet_Apprentice()
    {
        CardInfo c = new();

        c.name = "Violet Apprentice";
        c.text = "Ready to learn.";

        c.manaCost = 1;
        c.damage = 1;
        c.health = 1;

        c.MINION = true;
        c.TOKEN = true;

        return c;
    }
    
    private static CardInfo Antique_Healbot()
    {
        CardInfo c = new();

        c.name = "Antique Healbot";
        c.text = "Battlecry: Restore 8 health to your hero.";

        c.manaCost = 5;
        c.damage = 3;
        c.health = 3;
        c.tribe = Card.Tribe.Mech;

        c.MINION = true;
        c.BATTLECRY = true;

        return c;
    }
    private static CardInfo Azure_Drake()
    {
        CardInfo c = new();

        c.name = "Azure Drake";
        c.text = "+1 Spellpower.\nBattlecry: Draw a card.";

        c.manaCost = 5;
        c.damage = 4;
        c.health = 4;

        c.MINION = true;
        c.BATTLECRY = true;
        c.tribe = Card.Tribe.Dragon;

        c.auras.Add(Aura.Type.Spellpower);

        return c;
    }
    private static CardInfo Coldlight_Oracle()
    {
        CardInfo c = new();

        c.name = "Coldlight Oracle";
        c.text = "Battlecry: Each player draws 2 cards.";

        c.manaCost = 3;
        c.damage = 2;
        c.health = 2;

        c.MINION = true;
        c.BATTLECRY = true;
        c.tribe = Card.Tribe.Murloc;

        return c;
    }
    private static CardInfo Youthful_Brewmaster()
    {
        CardInfo c = new();

        c.name = "Youthful Brewmaster";
        c.text = "Battlecry: Return a friendly minion from the battlefield to your hand.";

        c.manaCost = 2;
        c.damage = 3;
        c.health = 2;

        c.MINION = true;
        c.BATTLECRY = true;
        c.TARGETED = true;
        c.eligibleTargets = Board.EligibleTargets.FriendlyMinions;

        return c;
    }
    private static CardInfo King_Mukla()
    {
        CardInfo c = new();

        c.name = "King Mukla";
        c.text = "Battlecry: Give your opponent two Bananas.";

        c.manaCost = 3;
        c.damage = 5;
        c.health = 5;

        c.MINION = true;
        c.BATTLECRY = true;
        c.LEGENDARY = true;
        c.tribe = Card.Tribe.Beast;

        return c;
    }
    private static CardInfo Bananas()
    {
        CardInfo c = new();

        c.name = "Bananas";
        c.text = "Give a minion +1/+1.";
        c.manaCost = 1;

        c.eligibleTargets = Board.EligibleTargets.AllMinions;

        c.TOKEN = true;
        c.SPELL = true;
        c.TARGETED = true;

        return c;
    }
    private static CardInfo Leeroy_Jenkins()
    {
        CardInfo c = new();

        c.name = "Leeroy Jenkins";
        c.text = "Charge.\nBattlecry: Summon two 1/1 Whelps for your opponent.";

        c.manaCost = 5;
        c.damage = 6;
        c.health = 2;

        c.MINION = true;
        c.BATTLECRY = true;
        c.LEGENDARY = true;

        c.auras.Add(Aura.Type.Charge);

        return c;
    }
    private static CardInfo Whelp()
    {
        CardInfo c = new();

        c.name = "Whelp";
        c.text = "";

        c.manaCost = 1;
        c.damage = 1;
        c.health = 1;
        c.tribe = Card.Tribe.Dragon;

        c.MINION = true;
        c.TOKEN = true;

        return c;
    }
    private static CardInfo Dr_Boom()
    {
        CardInfo c = new();

        c.name = "Dr. Boom";
        c.text = "Battlecry: Summon two 1/1 Boom Bots.\nWARNING: Bots may explode.";

        c.manaCost = 7;
        c.damage = 7;
        c.health = 7;

        c.MINION = true;
        c.BATTLECRY = true;
        c.LEGENDARY = true;

        return c;
    }
    private static CardInfo Boom_Bot()
    {
        CardInfo c = new();

        c.name = "Boom Bot";
        c.text = "Deathrattle:\nDeal 1-4 damage to a random enemy.";

        c.manaCost = 1;
        c.damage = 1;
        c.health = 1;

        c.MINION = true;
        c.TOKEN = true;

        c.triggers.Add((Trigger.Type.Deathrattle, Trigger.Side.Both, Trigger.Ability.Boom_Bot));

        return c;
    }
    private static CardInfo Unstable_Ghoul()
    {
        CardInfo c = new();

        c.name = "Unstable Ghoul";
        c.text = "Taunt.\nDeathrattle:\nDeal 1 damage to all minions.";

        c.manaCost = 2;
        c.damage = 1;
        c.health = 3;

        c.MINION = true;
        c.auras.Add(Aura.Type.Taunt);
        c.triggers.Add((Trigger.Type.Deathrattle, Trigger.Side.Both, Trigger.Ability.Unstable_Ghoul));

        return c;
    }

    private static CardInfo Acolyte_of_Pain()
    {
        CardInfo c = new();

        c.name = "Acolyte of Pain";
        c.text = "Whenever this minion takes damage, draw a card.";

        c.manaCost = 3;
        c.damage = 1;
        c.health = 3;

        c.MINION = true;

        c.triggers.Add((Trigger.Type.OnDamageTaken, Trigger.Side.Both, Trigger.Ability.Acolyte_of_Pain));

        return c;
    }
    private static CardInfo Gnomish_Inventor()
    {
        CardInfo c = new();

        c.name = "Gnomish Inventor";
        c.text = "Battlecry: Draw a card.";

        c.manaCost = 4;
        c.damage = 2;
        c.health = 4;

        c.MINION = true;
        c.BATTLECRY = true;

        return c;
    }
    private static CardInfo Dread_Corsair()
    {
        CardInfo c = new();

        c.name = "Dread Corsair";
        c.text = "Taunt.\nCosts 1 less per attack on your weapon.";

        c.manaCost = 4;
        c.damage = 3;
        c.health = 3;

        c.MINION = true;
        c.auras.Add(Aura.Type.Taunt);
        c.cardAuras.Add(Aura.Type.Dread_Corsair);
        c.tribe = Card.Tribe.Pirate;

        return c;
    }
    private static CardInfo Doomsayer()
    {
        CardInfo c = new();

        c.name = "Doomsayer";
        c.text = "At the start of your turn, destroy all minions.";

        c.manaCost = 2;
        c.damage = 0;
        c.health = 7;

        c.MINION = true;
        c.triggers.Add((Trigger.Type.StartTurn,Trigger.Side.Friendly,Trigger.Ability.Doomsayer));

        return c;
    }
    private static CardInfo Loot_Hoarder()
    {
        CardInfo c = new();

        c.name = "Loot Hoarder";
        c.text = "Deathrattle: Draw a card.";

        c.manaCost = 2;
        c.damage = 2;
        c.health = 1;

        c.MINION = true;
        c.triggers.Add((Trigger.Type.Deathrattle,Trigger.Side.Both,Trigger.Ability.DrawCard));

        return c;
    }
    private static CardInfo Alexstrasza()
    {
        CardInfo c = new();

        c.name = "Alexstrasza";
        c.text = "Battlecry:\nSet a hero's remaining health to 15.";

        c.eligibleTargets = Board.EligibleTargets.AllHeroes;
        c.tribe = Card.Tribe.Dragon;

        c.manaCost = 9;
        c.damage = 8;
        c.health = 8;

        c.MINION = true;
        c.TARGETED = true;
        c.BATTLECRY = true;
        c.LEGENDARY = true;

        return c;
    }
    private static CardInfo Mad_Scientist()
    {
        CardInfo c = new();

        c.name = "Mad Scientist";
        c.text = "Deathrattle: Put a secret from your deck into play.";

        c.manaCost = 2;
        c.damage = 2;
        c.health = 2;

        c.MINION = true;

        c.triggers.Add((Trigger.Type.Deathrattle, Trigger.Side.Both, Trigger.Ability.Mad_Scientist));
        return c;
    }
    private static CardInfo Haunted_Creeper()
    {
        CardInfo c = new();

        c.name = "Haunted Creeper";
        c.text = "Deathrattle: Summon two 1/1 Spectral Spiders.";

        c.manaCost = 2;
        c.damage = 1;
        c.health = 2;

        c.MINION = true;
        c.tribe = Card.Tribe.Beast;

        c.triggers.Add((Trigger.Type.Deathrattle, Trigger.Side.Both, Trigger.Ability.Haunted_Creeper));
        return c;
    }
    private static CardInfo Spectral_Spider()
    {
        CardInfo c = new();

        c.name = "Spectral Spider";
        c.text = "";

        c.manaCost = 1;
        c.damage = 1;
        c.health = 1;

        c.MINION = true;
        c.TOKEN = true;

        return c;
    }
    private static CardInfo Nerubian_Egg()
    {
        CardInfo c = new();

        c.name = "Nerubian Egg";
        c.text = "Deathrattle: Summon a 4/4 Nerubian.";

        c.manaCost = 2;
        c.damage = 0;
        c.health = 2;

        c.MINION = true;

        c.triggers.Add((Trigger.Type.Deathrattle, Trigger.Side.Both, Trigger.Ability.Nerubian_Egg));
        return c;
    }
    private static CardInfo Nerubian()
    {
        CardInfo c = new();

        c.name = "Nerubian";
        c.text = "";

        c.manaCost = 4;
        c.damage = 4;
        c.health = 4;

        c.MINION = true;
        c.TOKEN = true;

        return c;
    }

    private static CardInfo Zombie_Chow()
    {
        CardInfo c = new();

        c.name = "Zombie Chow";
        c.text = "Deathrattle: Restore 5 health to enemy hero.";

        c.manaCost = 1;
        c.damage = 2;
        c.health = 3;

        c.MINION = true;

        c.triggers.Add((Trigger.Type.Deathrattle, Trigger.Side.Both, Trigger.Ability.Zombie_Chow));
        return c;
    }

    private static CardInfo Blackwing_Technician()
    {
        CardInfo c = new();

        c.name = "Blackwing Technician";
        c.text = "Battlecry: If you're holding a dragon, gain +1/+1.";

        c.manaCost = 3;
        c.damage = 2;
        c.health = 4;

        c.MINION = true;
        c.BATTLECRY = true;

        c.cardAuras.Add(Aura.Type.HoldingDragon);
        return c;
    }
    
    private static CardInfo Twilight_Drake()
    {
        CardInfo c = new();

        c.name = "Twilight Drake";
        c.text = "Battlecry: Gain +1 Health for each card in your hand.";

        c.manaCost = 4;
        c.damage = 4;
        c.health = 1;

        c.MINION = true;
        c.BATTLECRY = true;

        c.tribe = Card.Tribe.Dragon;

        return c;
    }
    
    private static CardInfo Blackwing_Corruptor()
    {
        CardInfo c = new();

        c.name = "Blackwing Corruptor";
        c.text = "Battlecry: If you're holding a dragon, deal 3 damage.";

        c.manaCost = 5;
        c.damage = 5;
        c.health = 4;

        c.MINION = true;
        c.BATTLECRY = true;

        c.cardAuras.Add(Aura.Type.HoldingDragonTargeted);

        return c;
    }

    public static CardInfo Big_Game_Hunter()
    {
        CardInfo c = new();

        c.name = "Big Game Hunter";
        c.text = "Battlecry: Destroy a minion with 7 or more attack.";

        c.manaCost = 3;
        c.damage = 4;
        c.health = 2;

        c.MINION = true;
        c.BATTLECRY = true;
        c.TARGETED = true;
        c.eligibleTargets = Board.EligibleTargets.Big_Game_Hunter;

        return c;
    }
    
    private static CardInfo Sludge_Belcher()
    {
        CardInfo c = new();

        c.name = "Sludge Belcher";
        c.text = "Taunt.\nDeathrattle: Summon a 1/2 Slime with Taunt.";

        c.manaCost = 5;
        c.damage = 3;
        c.health = 5;

        c.MINION = true;

        c.auras.Add(Aura.Type.Taunt);

        c.triggers.Add((Trigger.Type.Deathrattle, Trigger.Side.Both, Trigger.Ability.Sludge_Belcher));

        return c;
    }
    private static CardInfo Putrid_Slime()
    {
        CardInfo c = new();

        c.name = "Putrid Slime";
        c.text = "Taunt";

        c.manaCost = 1;
        c.damage = 1;
        c.health = 2;

        c.MINION = true;
        c.TOKEN = true;

        c.auras.Add(Aura.Type.Taunt);

        return c;
    }
    private static CardInfo Core_Hound()
    {
        CardInfo c = new();

        c.name = "Core Hound";
        c.text = "";

        c.manaCost = 7;
        c.damage = 9;
        c.health = 5;
        c.tribe = Card.Tribe.Beast;

        c.MINION = true;

        return c;
    }
    private static CardInfo Malygos()
    {
        CardInfo c = new();

        c.name = "Malygos";
        c.text = "+5 Spellpower.";

        c.manaCost = 9;
        c.damage = 4;
        c.health = 12;

        c.MINION = true;
        c.tribe = Card.Tribe.Dragon;

        c.auras.Add(Aura.Type.Spellpower);
        c.auras.Add(Aura.Type.Spellpower);
        c.auras.Add(Aura.Type.Spellpower);
        c.auras.Add(Aura.Type.Spellpower);
        c.auras.Add(Aura.Type.Spellpower);

        c.LEGENDARY = true;
        return c;
    }

    private static CardInfo Sunfury_Protector()
    {
        CardInfo c = new();

        c.name = "Sunfury Protector";
        c.text = "Battlecry: Give adjacent minions Taunt.";

        c.manaCost = 2;
        c.damage = 2;
        c.health = 3;

        c.MINION = true;
        c.BATTLECRY = true;

        return c;
    }
    private static CardInfo Ancient_Watcher()
    {
        CardInfo c = new();

        c.name = "Ancient Watcher";
        c.text = "Can't attack.";

        c.manaCost = 2;
        c.damage = 4;
        c.health = 5;

        c.MINION = true;
        c.auras.Add(Aura.Type.NoAttack);

        return c;
    }
    private static CardInfo Acidic_Swamp_Ooze()
    {
        CardInfo c = new();

        c.name = "Acidic Swamp Ooze";
        c.text = "Battlecry: Destroy your opponent's weapon.";

        c.manaCost = 2;
        c.damage = 3;
        c.health = 2;

        c.MINION = true;
        c.BATTLECRY = true;

        return c;
    }
    private static CardInfo Novice_Engineer()
    {
        CardInfo c = new();

        c.name = "Novice Engineer";
        c.text = "Battlecry: Draw a card.";

        c.manaCost = 2;
        c.damage = 1;
        c.health = 1;

        c.MINION = true;
        c.BATTLECRY = true;

        return c;
    }
    private static CardInfo Kobold_Geomancer()
    {
        CardInfo c = new();

        c.name = "Kobold Geomancer";
        c.text = "+1 Spellpower.";

        c.manaCost = 2;
        c.damage = 2;
        c.health = 2;

        c.MINION = true;

        c.auras.Add(Aura.Type.Spellpower);

        return c;
    }
    private static CardInfo Gadgetzan_Auctioneer()
    {
        CardInfo c = new();

        c.name = "Gadgetzan Auctioneer";
        c.text = "Whenever you play a spell, draw a card.";

        c.manaCost = 6;
        c.damage = 4;
        c.health = 4;

        c.MINION = true;

        c.triggers.Add((Trigger.Type.OnPlaySpell, Trigger.Side.Friendly,Trigger.Ability.DrawCard));

        return c;
    }
    private static CardInfo Shade_of_Naxxrammas()
    {
        CardInfo c = new();

        c.name = "Shade of Naxxrammas";
        c.text = "Stealth.\nAt the start of your turn, gain +1/+1";

        c.manaCost = 3;
        c.damage = 2;
        c.health = 2;

        c.MINION = true;

        c.auras.Add(Aura.Type.Stealth);

        c.triggers.Add((Trigger.Type.StartTurn, Trigger.Side.Friendly,Trigger.Ability.Shade_of_Naxxrammas));

        return c;
    }
    
    private static CardInfo Harrison_Jones()
    {
        CardInfo c = new();

        c.name = "Harrison Jones";
        c.text = "Battlecry: Destroy enemy weapon. Draw cards equal to its durability.";

        c.manaCost = 5;
        c.damage = 5;
        c.health = 4;

        c.MINION = true;
        c.BATTLECRY = true;
        c.LEGENDARY = true;

        return c;
    }
    private static CardInfo Sylvanas_Windrunner()
    {
        CardInfo c = new();

        c.name = "Sylvanas Windrunner";
        c.text = "Deathrattle: Take control of a random enemy minion.";

        c.manaCost = 6;
        c.damage = 5;
        c.health = 5;

        c.MINION = true;
        c.LEGENDARY = true;

        c.triggers.Add((Trigger.Type.Deathrattle, Trigger.Side.Both, Trigger.Ability.Sylvanas_Windrunner));

        return c;
    }
    private static CardInfo Mind_Control_Tech()
    {
        CardInfo c = new();

        c.name = "Mind Control Tech";
        c.text = "Battlecry: If your opponent has 4+ minions, take control of a random one.";

        c.manaCost = 3;
        c.damage = 3;
        c.health = 3;

        c.MINION = true;
        c.BATTLECRY = true;

        return c;
    }
    private static CardInfo Baron_Geddon()
    {
        CardInfo c = new();

        c.name = "Baron Geddon";
        c.text = "End of Turn: Deal 2 damage to all other characters.";

        c.manaCost = 7;
        c.damage = 7;
        c.health = 5;

        c.MINION = true;
        c.LEGENDARY = true;

        c.triggers.Add((Trigger.Type.EndTurn, Trigger.Side.Friendly, Trigger.Ability.Baron_Geddon));

        return c;
    }
    private static CardInfo Ragnaros()
    {
        CardInfo c = new();

        c.name = "Ragnaros the Firelord";
        c.text = "Can't attack.\nEnd of Turn: Deal 8 damage to a random enemy.";

        c.manaCost = 8;
        c.damage = 8;
        c.health = 8;

        c.MINION = true;
        c.LEGENDARY = true;

        c.auras.Add(Aura.Type.NoAttack);
        c.triggers.Add((Trigger.Type.EndTurn, Trigger.Side.Friendly, Trigger.Ability.Ragnaros));

        return c;
    }
    private static CardInfo Lightwarden()
    {
        CardInfo c = new();

        c.name = "Lightwarden";
        c.text = "Whenever a character is healed, gain +2 attack";

        c.manaCost = 1;
        c.damage = 1;
        c.health = 2;

        c.MINION = true;

        c.triggers.Add((Trigger.Type.OnMinionHealed, Trigger.Side.Both, Trigger.Ability.Lightwarden));
        c.triggers.Add((Trigger.Type.OnFaceHealed, Trigger.Side.Both, Trigger.Ability.Lightwarden));

        return c;
    }
    private static CardInfo Wild_Pyromancer()
    {
        CardInfo c = new();

        c.name = "Wild Pyromancer";
        c.text = "After you play a spell, deal 1 damage to all minions";

        c.manaCost = 2;
        c.damage = 3;
        c.health = 2;

        c.MINION = true;

        c.triggers.Add((Trigger.Type.AfterPlaySpell, Trigger.Side.Friendly, Trigger.Ability.Unstable_Ghoul));

        return c;
    }
    private static CardInfo Deathlord()
    {
        CardInfo c = new();

        c.name = "Deathlord";
        c.text = "Taunt. Deathrattle: Your opponent puts a minion from their deck into play.";

        c.manaCost = 3;
        c.damage = 2;
        c.health = 8;

        c.MINION = true;

        c.auras.Add(Aura.Type.Taunt);

        c.triggers.Add((Trigger.Type.Deathrattle, Trigger.Side.Both, Trigger.Ability.Deathlord));

        return c;
    }

    private static CardInfo Injured_Blademaster()
    {
        CardInfo c = new();

        c.name = "Injured Blademaster";
        c.text = "Battlecry: Deals 4 damage to himself.";

        c.manaCost = 3;
        c.damage = 4;
        c.health = 7;

        c.MINION = true;
        c.BATTLECRY = true;

        return c;
    }

    private static CardInfo Piloted_Shredder()
    {
        CardInfo c = new();

        c.name = "Piloted Shredder";
        c.text = "Deathrattle: Summon a random\n2-cost minion.";

        c.manaCost = 4;
        c.damage = 4;
        c.health = 3;

        c.MINION = true;
        c.tribe = Card.Tribe.Mech;

        c.triggers.Add((Trigger.Type.Deathrattle, Trigger.Side.Both, Trigger.Ability.Piloted_Shredder));

        return c;
    }
    private static CardInfo Annoy_o_Tron()
    {
        CardInfo c = new();

        c.name = "Annoy-o-Tron";
        c.text = "Taunt.\nDivine Shield.";

        c.manaCost = 2;
        c.damage = 1;
        c.health = 2;

        c.MINION = true;
        c.tribe = Card.Tribe.Mech;

        c.auras.Add(Aura.Type.Taunt);
        c.auras.Add(Aura.Type.Shield);

        return c;
    }

    private static CardInfo Explosive_Sheep()
    {
        CardInfo c = new();

        c.name = "Explosive Sheep";
        c.text = "Deathrattle: Deal 2 damage to all minions.";

        c.manaCost = 2;
        c.damage = 1;
        c.health = 1;

        c.MINION = true;
        c.tribe = Card.Tribe.Mech;

        c.triggers.Add((Trigger.Type.Deathrattle, Trigger.Side.Both, Trigger.Ability.Explosive_Sheep));

        return c;
    }
    private static CardInfo Gilbin_Stalker()
    {
        CardInfo c = new();

        c.name = "Gilbin Stalker";
        c.text = "Stealth.";

        c.manaCost = 2;
        c.damage = 2;
        c.health = 3;

        c.MINION = true;
        c.auras.Add(Aura.Type.Stealth);

        return c;
    }

    private static CardInfo Micro_Machine()
    {
        CardInfo c = new();

        c.name = "Micro Machine";
        c.text = "At the start of each turn, gain +1 attack.";

        c.manaCost = 2;
        c.damage = 1;
        c.health = 2;

        c.MINION = true;
        c.tribe = Card.Tribe.Mech;

        c.triggers.Add((Trigger.Type.StartTurn, Trigger.Side.Both, Trigger.Ability.Frothing_Berserker));

        return c;
    }
    private static CardInfo Mechwarper()
    {
        CardInfo c = new();

        c.name = "Mechwarper";
        c.text = "Your mechs cost 1 less.";

        c.manaCost = 2;
        c.damage = 2;
        c.health = 3;

        c.MINION = true;
        c.tribe = Card.Tribe.Mech;
        c.auras.Add(Aura.Type.Mechwarper);

        return c;
    }

    private static CardInfo Puddlestomper()
    {
        CardInfo c = new();

        c.name = "Puddlestomper";
        c.text = "";

        c.manaCost = 2;
        c.damage = 3;
        c.health = 2;
        c.tribe = Card.Tribe.Murloc;

        c.MINION = true;

        return c;
    }

    private static CardInfo Recombobulator()
    {
        CardInfo c = new();

        c.name = "Recombobulator";
        c.text = "[EFFECT UNIMPLEMENTED]";

        c.manaCost = 2;
        c.damage = 3;
        c.health = 2;

        c.MINION = true;

        return c;
    }

    private static CardInfo Ships_Cannon()
    {
        CardInfo c = new();

        c.name = "Ship's Cannon";
        c.text = "Whenever you summon a pirate, deal 2 damage to a random enemy";

        c.manaCost = 2;
        c.damage = 2;
        c.health = 3;

        c.MINION = true;
        c.triggers.Add((Trigger.Type.AfterPlayMinion, Trigger.Side.Friendly, Trigger.Ability.Ships_Cannon));
        c.triggers.Add((Trigger.Type.AfterSummonMinion, Trigger.Side.Friendly, Trigger.Ability.Ships_Cannon));

        return c;
    }

    private static CardInfo Stonesplinter_Trogg()
    {
        CardInfo c = new();

        c.name = "Stonesplinter Trogg";
        c.text = "Whenever your opponent cast a spell, gain +1 Attack.";

        c.manaCost = 2;
        c.damage = 2;
        c.health = 3;

        c.MINION = true;
        c.triggers.Add((Trigger.Type.OnPlaySpell, Trigger.Side.Enemy, Trigger.Ability.Frothing_Berserker));

        return c;
    }

    private static CardInfo Bloodfen_Raptor()
    {
        CardInfo c = new();

        c.name = "Bloodfen Raptor";
        c.text = "";

        c.manaCost = 2;
        c.damage = 3;
        c.health = 2;

        c.tribe = Card.Tribe.Beast;
        c.MINION = true;

        return c;
    }
    private static CardInfo Bloodsail_Raider()
    {
        CardInfo c = new();

        c.name = "Bloodsail Raider";
        c.text = "Battlecry: Gain attack equal to your weapon's.";

        c.manaCost = 2;
        c.damage = 2;
        c.health = 3;

        c.MINION = true;
        c.BATTLECRY = true;
        c.tribe = Card.Tribe.Pirate;

        return c;
    }
    private static CardInfo Bluegill_Warrior()
    {
        CardInfo c = new();

        c.name = "Bluegill Warrior";
        c.text = "Charge.";

        c.manaCost = 2;
        c.damage = 2;
        c.health = 1;
        c.tribe = Card.Tribe.Murloc;
        c.auras.Add(Aura.Type.Charge);

        c.MINION = true;

        return c;
    }
    private static CardInfo Captains_Parrot()
    {
        CardInfo c = new();

        c.name = "Captain's Parrot";
        c.text = "Battlecry: Draw a pirate from your deck.";

        c.manaCost = 2;
        c.damage = 1;
        c.health = 1;
        c.tribe = Card.Tribe.Beast;

        c.MINION = true;
        c.BATTLECRY = true;

        return c;
    }

    private static CardInfo Faerie_Dragon()
    {
        CardInfo c = new();

        c.name = "Faerie Dragon";
        c.text = "Cannot be targeted by spells or hero powers.";

        c.manaCost = 2;
        c.damage = 3;
        c.health = 2;

        c.tribe = Card.Tribe.Dragon;
        c.MINION = true;
        c.auras.Add(Aura.Type.Elusive);

        return c;
    }

    private static CardInfo Frostwolf_Grunt()
    {
        CardInfo c = new();

        c.name = "Frostwolf Grunt";
        c.text = "Taunt.";

        c.manaCost = 2;
        c.damage = 2;
        c.health = 2;

        c.MINION = true;

        c.auras.Add(Aura.Type.Taunt);
        return c;
    }

    private static CardInfo Lorewalker_Cho()
    {
        CardInfo c = new();

        c.name = "Lorewalker Cho";
        c.text = "Whenever a player casts a spell, add it to the other's hand.";

        c.manaCost = 2;
        c.damage = 0;
        c.health = 4;

        c.MINION = true;
        c.LEGENDARY = true;

        c.triggers.Add((Trigger.Type.OnPlaySpell, Trigger.Side.Both, Trigger.Ability.Lorewalker_Cho));

        return c;
    }
    private static CardInfo Mad_Bomber()
    {
        CardInfo c = new();

        c.name = "Mad Bomber";
        c.text = "Battlecry: Deal 1 damage to a random character. Repeat 3 times.";

        c.manaCost = 2;
        c.damage = 3;
        c.health = 2;

        c.MINION = true;
        c.BATTLECRY = true;

        return c;
    }
    private static CardInfo Mana_Addict()
    {
        CardInfo c = new();

        c.name = "Mana Addict";
        c.text = "Whenever you cast a spell, gain +2 attack this turn.";

        c.manaCost = 2;
        c.damage = 1;
        c.health = 3;

        c.MINION = true;
        c.triggers.Add((Trigger.Type.OnPlaySpell, Trigger.Side.Friendly, Trigger.Ability.Mana_Addict));

        return c;
    }
    private static CardInfo Master_Swordsmith()
    {
        CardInfo c = new();

        c.name = "Master Swordsmith";
        c.text = "End of your turn: Give a random friendly minion +1 Attack.";

        c.manaCost = 2;
        c.damage = 1;
        c.health = 3;

        c.MINION = true;
        c.triggers.Add((Trigger.Type.EndTurn, Trigger.Side.Friendly, Trigger.Ability.Master_Swordsmith));

        return c;
    }
    private static CardInfo Murloc_Tidehunter()
    {
        CardInfo c = new();

        c.name = "Murloc Tidehunter";
        c.text = "Battlecry: Summon a 1/1 Murloc Scout";

        c.manaCost = 2;
        c.damage = 2;
        c.health = 1;
        c.tribe = Card.Tribe.Murloc;

        c.MINION = true;
        c.BATTLECRY = true;

        return c;
    }
    private static CardInfo Murloc_Scout()
    {
        CardInfo c = new();

        c.name = "Murloc Scout";
        c.text = "";

        c.manaCost = 1;
        c.damage = 1;
        c.health = 1;
        c.tribe = Card.Tribe.Murloc;

        c.MINION = true;
        c.TOKEN = true;

        return c;
    }
    private static CardInfo Nat_Pagle()
    {
        CardInfo c = new();

        c.name = "Nat Pagle";
        c.text = "Start of turn: 50% chance to draw a card.";

        c.manaCost = 2;
        c.damage = 0;
        c.health = 4;

        c.MINION = true;
        c.LEGENDARY = true;

        c.triggers.Add((Trigger.Type.StartTurn, Trigger.Side.Friendly, Trigger.Ability.Nat_Pagle));

        return c;
    }

    private static CardInfo Pint_Sized_Summoner()
    {
        CardInfo c = new();

        c.name = "Pint-Sized Summoner";
        c.text = "The first minion you play each turn costs 1 less.";

        c.manaCost = 2;
        c.damage = 2;
        c.health = 2;

        c.MINION = true;
        c.auras.Add(Aura.Type.Pint_Sized_Summoner);
        c.triggers.Add((Trigger.Type.StartTurn, Trigger.Side.Friendly, Trigger.Ability.Pint_Sized_Summoner));
        c.triggers.Add((Trigger.Type.OnPlayMinion, Trigger.Side.Friendly, Trigger.Ability.Pint_Sized_Exhaust));

        return c;
    }
    private static CardInfo River_Crocolisk()
    {
        CardInfo c = new();

        c.name = "River Crocolisk";
        c.text = "";

        c.manaCost = 2;
        c.damage = 2;
        c.health = 3;
        c.tribe = Card.Tribe.Beast;
        c.MINION = true;

        return c;
    }
    private static CardInfo Scavenging_Hyena()
    {
        CardInfo c = new();

        c.name = "Scavenging Hyena";
        c.text = "Whenever a friendly beast dies, gain +2/+1";

        c.manaCost = 2;
        c.damage = 2;
        c.health = 2;

        c.classType = Card.Class.Hunter;
        c.tribe = Card.Tribe.Beast;

        c.MINION = true;
        c.triggers.Add((Trigger.Type.OnMinionDeath, Trigger.Side.Friendly, Trigger.Ability.Scavenging_Hyena));

        return c;
    }
    private static CardInfo Starving_Buzzard()
    {
        CardInfo c = new();

        c.name = "Starving Buzzard";
        c.text = "Whenever you summon a beast, draw a card.";

        c.manaCost = 2;
        c.damage = 2;
        c.health = 1;

        c.MINION = true;
        c.classType = Card.Class.Hunter;
        c.tribe = Card.Tribe.Beast;

        c.triggers.Add((Trigger.Type.OnPlayMinion, Trigger.Side.Friendly, Trigger.Ability.Starving_Buzzard));
        c.triggers.Add((Trigger.Type.AfterSummonMinion, Trigger.Side.Friendly, Trigger.Ability.Starving_Buzzard));

        return c;
    }
    private static CardInfo Echoing_Ooze()
    {
        CardInfo c = new();

        c.name = "Echoing Ooze";
        c.text = "Battlecry: Summon an exact copy of this minion at end of turn.";

        c.manaCost = 2;
        c.damage = 1;
        c.health = 2;

        c.MINION = true;
        c.BATTLECRY = true;

        return c;
    }
    private static CardInfo Nerubar_Weblord()
    {
        CardInfo c = new();

        c.name = "Nerub'ar Weblord";
        c.text = "All Battlecry minions cost 1 more.";

        c.manaCost = 2;
        c.damage = 1;
        c.health = 4;

        c.MINION = true;
        c.auras.Add(Aura.Type.Nerubar_Weblord);

        return c;
    }

}
