using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;

namespace D3SharpDatabase
{
    //  TODO 
    //  Implementation of hero_details and SQL_Lite functions.
    public class HeroDetails 
    {
        public int Id { get; private set; }
        public int Title_Id { get; set; }
        public int BlockAmount { get; set; }
        public int BlockChance { get; set; }
        public int DodgeChance { get; set; }
        public int DamageReduction { get; set; }
        public int AttackDamageBonus { get; set; }
        public int PrecisionCritBonus{ get; set; }
        public int DefenseDamageReduction { get; set; }
        public int VitalityLife { get; set; }
        public int Armor { get; set; }
        public int Attack { get; set; }
        public int Precision { get; set; }
        public int Defense { get; set; }
        public int Vitality { get; set; }
        public int DamageIncrease { get; set; }
        public int AttacksPerSecond  { get; set; }
        public int CritDamageBonus { get; set; }
        public int CritChanceBonus { get; set; }
        public int CastingSpeed { get; set; }
        public int LifePerKill { get; set; }
        public int LifePerHit { get; set; }
        public int MovementSpeed { get; set; }
        public int GoldFind { get; set; }
        public int Life { get; set; }
        public int LifePerSecond { get; set; }
        public int LifeSteal { get; set; }
        public int MagicFind { get; set; }
        public int FuryGain { get; set; }
        public int SpiritGain { get; set; }
        public int ManaGain { get; set; }
        public int ArcanumGain { get; set; }
        public int HatredGain { get; set; }
        public int DisciplineGain { get; set; }
        public int MaxMana { get; set; }
        public int MaxArcanum { get; set; }
        public int MaxHatred { get; set; }
        public int MaxFury { get; set; }
        public int MaxDiscipline { get; set; }
        public int MaxSpirit { get; set; }

        public HeroDetails (int title_id, int blockamount, 
                             int blockchance, int dodgechance, 
                             int damagereduction, int attackbonusdamage, 
                             int precisioncritbonus, 
                             int defensedamagereduction, int vitalitylife,
                             int armor, int attack, int precision,
                             int defense, int vitality,
                             int damageincrease, int attackspersecond,
                             int critdamagebonus, int critchancebonus,
                             int castingspeed, int lifeperkill,
                             int lifeperhit, int movementspeed,
                             int goldfind, int life, int lifepersecond, 
                             int lifesteal, int magicfind, int furygain, 
                             int spiritgain, int managain, 
                             int arcanumgain, int hatredgain, 
                             int disciplinegain, int maxmana, 
                             int maxarcanum, int maxhatred,
                             int maxfury, int maxdiscipline,
                             int maxspirit)
        {
            Id = -1;
            Title_Id = title_id;
            BlockAmount = blockamount;
            BlockChance = blockchance;
            DodgeChance = dodgechance;
            DamageReduction = damagereduction;
            AttackDamageBonus = attackbonusdamage;
            PrecisionCritBonus = precisioncritbonus;
            DefenseDamageReduction = defensedamagereduction;
            VitalityLife = vitalitylife;
            Armor = armor;
            Attack = attack;
            Precision = precision;
            Defense = defense;
            Vitality = vitality;
            DamageIncrease = damageincrease;
            AttacksPerSecond = attackspersecond;
            CritDamageBonus = critdamagebonus;
            CritChanceBonus =critchancebonus;
            CastingSpeed = castingspeed;
            LifePerKill = lifeperkill;
            LifePerHit = lifeperhit;
            MovementSpeed = movementspeed;
            GoldFind = goldfind;
            Life = life;
            LifePerSecond = lifepersecond;
            LifeSteal = lifesteal;
            MagicFind = magicfind;
            FuryGain = furygain;
            SpiritGain = spiritgain;
            ManaGain = managain;
            ArcanumGain = arcanumgain;
            HatredGain = hatredgain;
            DisciplineGain = disciplinegain;
            MaxMana = maxmana;
            MaxArcanum = maxarcanum;
            MaxHatred = maxhatred;
            MaxFury = maxfury;
            MaxDiscipline = maxdiscipline;
            MaxSpirit = maxspirit;
        }

        //  TODO
        //  Implement save function
        public bool Save ( )
        {
            try
            {
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to save hero_details exception: {0}", e.Message);
                return false;
            }
        }

        //  TODO
        //  Implement create function
        public bool Create ()
        {
            return true;
        }

        //  TODO
        //  Implement CheckIfDetailsExists function
        private bool CheckIfDetailsExists()
        {
            return true;
        }

        public bool Load()
        {
            return true;
        }

        //  TODO
        //  Implement function
        public override string ToString()
        {
            return true
        }
    }
}
