using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;

namespace D3Database
{
    public class Hero
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public int HeroClass { get; private set; }
        public int Gender { get; set; }
        public int Experience { get; set; }
        public int Level { get; set; }

        public Hero(string name, int heroClass, int gender, int experience, int level)
        {
            Id = -1;
            Name = name;
            HeroClass = heroClass;
            Gender = gender;
            Experience = experience;
            Level = level;
        }

        public bool Save()
        {
            try
            {
                SQLiteCommand command = new SQLiteCommand(string.Format("UPDATE hero SET hero_experience='{1}', hero_level='{2}' WHERE hero_id='{0}'", Id, Experience, Level), Database.Instance.Connection);
                command.ExecuteNonQuery();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to save hero exception: {0}", e.Message);
                return false;
            }
        }

        public bool Create(int accountId)
        {
            if (CheckIfHeroExists(accountId, Name))
                return false;
            SQLiteCommand command = new SQLiteCommand(string.Format("INSERT INTO hero (account_id, hero_name, hero_class_id, hero_gender_id, hero_experience, hero_level) VALUES('{0}','{1}','{2}','{3}', '{4}', '{5}')", accountId, Name, HeroClass, Gender, Experience, Level), Database.Instance.Connection);
            int affectedRows = command.ExecuteNonQuery();
            if (affectedRows == 0)
                return false;
            Id = Database.Instance.GetLastInsertId();
            return true;
        }

        private bool CheckIfHeroExists(int account_id, string hero_name)
        {
            SQLiteCommand command = new SQLiteCommand(string.Format("SELECT hero_id FROM hero WHERE account_id='{0}' AND hero_name='{1}'", account_id, hero_name), Database.Instance.Connection);
            SQLiteDataReader reader = command.ExecuteReader();
            return reader.HasRows;
        }

        public static bool Load(int id, out Hero hero)
        {
            hero = null;
            try {
            SQLiteCommand command = new SQLiteCommand(string.Format("SELECT hero_id, hero_name, hero_class_id, hero_gender_id, hero_experience, hero_level FROM hero WHERE hero.hero_id='{0}'", id), Database.Instance.Connection);
            SQLiteDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    var hero_id = reader.GetInt32(0);
                    var name = reader.GetString(1);
                    var hero_class = reader.GetInt32(2);
                    var gender = reader.GetInt32(3);
                    var experience = reader.GetInt32(4);
                    var level = reader.GetInt32(5);
                    hero = new Hero(name, hero_class, gender, experience, level);
                    hero.Id = hero_id;
                    return true;
                }
            }
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to load Hero exception: {0}", e.Message);
                return false;
            }
            return false;
        }

        public override string ToString()
        {
            return String.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}", Id, Name, HeroClass, Gender, Experience, Level);
        }
    }
}
