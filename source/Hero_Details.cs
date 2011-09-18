using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;

namespace D3Database
{

    public class Hero_Details 
    {
        public int HeroDetailId {get;set;}
        public int HeroId { get; private set; }
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

        public Hero_Details (int herodetailid, int heroid, int title_id, int blackamount, 
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
            HeroDetailId = -1;
            HeroId = heroid;
            Title_Id = title_id;
            BlockAmount = blackamount;
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



        public bool Save()
        {
            //stub todo
            try
            {
                SQLiteCommand command = new SQLiteCommand(string.Format("UPDATE hero_details SET background_color='{1}', banner='{2}', pattern='{3}', pattern_color='{4}', placement='{5}', signil_accent='{6}', signil_main='{7}', signil_color='{8}', use_signil_variant='{9}' WHERE hero_detail_id='{0}'", HeroDetailId,BackgroundColor,Banner,Pattern,PatternColor,Placement,SignilAccent,SignilMain,SignilColor,UseSignilVariant), Database.Instance.Connection);
                command.ExecuteNonQuery();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to save Hero Details exception: {0}", e.Message);
                return false;
            }
        }

        public bool Create()
        {
            // might not work HeroDetailId isnt autoincrement in the db  i think?
            SQLiteCommand command = new SQLiteCommand(string.Format("INSERT INTO hero_details (hero_id, title_id, BlockAmount, BlockChance, DamageReduction, AttackDamageBonus, PrecisionCritBonus, DefenseDamageReduction, VitalityLife, Armor, Attack, Precision, Defense, Vitality, DamageIncrease, AttacksPerSecond, CritDamageBonus, CritChanceBonus, CastingSpeed, LifePerKill, MovementSpeed, GoldFind, Life, LifePerSecond, LifeSteal, MagicFind, FuryGain, SpiritGain, ManaGain, ArcanumGain, HatredGain, DisciplineGain, MaxMana, MaxArcanum, MaxHatred, MaxFury, MaxDiscipline, MaxSpirit) VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}','{31}','{32}','{33}','{34}','{35}','{36}','{37}','{38}','{39}')", HeroId,Title_Id, BlockAmount, BlockChance, DamageReduction, AttackDamageBonus, PrecisionCritBonus, DefenseDamageReduction, VitalityLife, Armor, Attack, Precision, Defense, Vitality, DamageIncrease, AttacksPerSecond, CritDamageBonus, CritChanceBonus, CastingSpeed, LifePerKill, MovementSpeed, GoldFind, Life, LifePerSecond, LifeSteal, MagicFind, FuryGain, SpiritGain, ManaGain, ArcanumGain, HatredGain, DisciplineGain, MaxMana, MaxArcanum, MaxHatred, MaxFury, MaxDiscipline, MaxSpirit), Database.Instance.Connection);
            int affectedRows = command.ExecuteNonQuery();
            if (affectedRows == 0)
                return false;
            HeroDetailId = Database.Instance.GetLastInsertId();
            return true;
        }


        public static bool Load(int heroid, out Hero_Details herodetails)
        {

            herodetails = null;
            try {
            SQLiteCommand command = new SQLiteCommand(string.Format("SELECT * FROM hero_details WHERE hero_detail_id='{0}'", heroid), Database.Instance.Connection);
            SQLiteDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    herodetails = new Hero_Details(reader.GetInt32(0),reader.GetInt32(1),reader.GetInt32(2),reader.GetInt32(3),reader.GetInt32(4),
                    reader.GetInt32(5),reader.GetInt32(6),reader.GetInt32(7),reader.GetInt32(8),reader.GetInt32(9),reader.GetInt32(10),reader.GetInt32(11),
                    reader.GetInt32(12),reader.GetInt32(13),reader.GetInt32(14),reader.GetInt32(15),reader.GetInt32(16),reader.GetInt32(17),reader.GetInt32(18),
                    reader.GetInt32(19),reader.GetInt32(20),reader.GetInt32(21),reader.GetInt32(22),reader.GetInt32(23),reader.GetInt32(24),reader.GetInt32(25),
                    reader.GetInt32(26),reader.GetInt32(27),reader.GetInt32(28),reader.GetInt32(29),reader.GetInt32(30),reader.GetInt32(31),reader.GetInt32(32),
                    reader.GetInt32(33),reader.GetInt32(34),reader.GetInt32(35),reader.GetInt32(36),reader.GetInt32(37),reader.GetInt32(38),reader.GetInt32(39),reader.GetInt32(40));
                    return true;
                }
            }            
            }

            catch (Exception e)
            {
                Console.WriteLine("Failed to load Hero Details exception: {0}", e.Message);
                return false;
            }
            return false;
        }

        

        public override string ToString()
        {
            //stub todo
            return String.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}", Id, BackgroundColor,Banner,Pattern,PatternColor,Placement,SignilAccent,SignilMain,SignilColor,UseSignilVariant);
        }
    }
}

















 Details & Settings 
Help CodeViewer.org stay ad-free, and become better. 
 

Code Viewer ©2007; founder Daniel M. Story.
The content presented on this site is copyrighted material to its rightful author.

Powered By: GeSHi, mootools