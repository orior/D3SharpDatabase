using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;

namespace D3Database
{
    public class Hero
    {
        #region Properties
        
        /// <summary>
        /// The database id of this hero.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// The database id of the account that this hero is associated with.
        /// </summary>
        public int AccountId { get; private set; }

        /// <summary>
        /// The name of this hero
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The class of this hero (1 = Wizard, 2 = Witch Doctor, 3 = Demon Hunter, 4 = Monk, 5 = Barbarian)
        /// </summary>
        public int HeroClass { get; private set; }

        /// <summary>
        /// The gender for this hero (1 = male, 2 = female).
        /// </summary>
        public int Gender { get; set; }

        /// <summary>
        /// The current total experience points for this hero.
        /// </summary>
        public int Experience { get; set; }

        /// <summary>
        /// The current level of this hero.
        /// </summary>
        public int Level { get; set; }

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Hero name</param>
        /// <param name="heroClass">Hero class (1 = Wizard, 2 = Witch Doctor, 3 = Demon Hunter, 4 = Monk, 5 = Barbarian)</param>
        /// <param name="gender">Gender (1 = male, 2 = female)</param>
        /// <param name="experience">Experience points</param>
        /// <param name="level">Hero level</param>
        public Hero(int accountId, string name, int heroClass, int gender, int experience, int level)
        {
            Id = -1;
            AccountId = accountId;
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
                // initialize SQL statement
                SQLiteCommand command = new SQLiteCommand(string.Format("UPDATE hero SET experience='{0}', level='{1}' WHERE hero_id='{2}'", Experience, Level, Id), Database.Instance.Connection);
                // execute SQL (update appropriate hero row with new data)
                command.ExecuteNonQuery();

                return true;
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Failed to save hero exception: ");
                Console.ResetColor();
                Console.WriteLine(e.Message);
                return false;
            }
        }

        /// <summary>
        /// Saves the current hero as a new record in the database.
        /// </summary>
        /// <returns>True on success, otherwise false.</returns>
        public bool Create()
        {
            // check that id is the default (id is set by the database)
            if (Id != -1)
            {
                return false;
            }

            // check that an account with the specified account id exists
            if (!Account.CheckIfAccountExists(AccountId))
            {
                return false;
            }

            // check that a hero with this name does not already exist
            if (CheckIfHeroExists(AccountId, Name))
            {
                return false;
            }

            // initialize SQL statement
            SQLiteCommand command = new SQLiteCommand(string.Format("INSERT INTO hero (account_id, name, hero_class_id, hero_gender_id, experience, level) VALUES('{0}','{1}','{2}','{3}', '{4}', '{5}')", AccountId, Name, HeroClass, Gender, Experience, Level), Database.Instance.Connection);

            // execute SQL (create new record in database)
            int affectedRows = command.ExecuteNonQuery();

            // check that command succeeded
            if (affectedRows == 0)
            {
                return false;
            }

            // set instance id to the id assigned by the database
            Id = Database.Instance.GetLastInsertId();            
            return true;
        }

        /// <summary>
        /// Queries the database to determine if a hero with the specified name and account id exists.
        /// </summary>
        /// <param name="account_id">The id of the account associated with the hero.</param>
        /// <param name="hero_name">The name of the hero.</param>
        /// <returns>True if a hero with the specified name and account id exists in the database, otherwise false.</returns>
        private bool CheckIfHeroExists(int account_id, string hero_name)
        {
            SQLiteCommand command = new SQLiteCommand(string.Format("SELECT hero_id FROM hero WHERE account_id='{0}' AND name='{1}'", account_id, hero_name), Database.Instance.Connection);
            SQLiteDataReader reader = command.ExecuteReader();
            return reader.HasRows;
        }

        /// <summary>
        /// Retrieves data for a specific hero from the database.
        /// </summary>
        /// <param name="id">The hero_id of the desired hero.</param>
        /// <param name="hero">Reference to an existing Hero instance where data will be stored on success.</param>
        /// <returns>True on success, otherwise false.</returns>
        public static bool Load(int id, out Hero hero)
        {
            // clear hero instance
            hero = null;

            // initialize SQL statement
            SQLiteCommand command = new SQLiteCommand(string.Format("SELECT hero_id, account_id, name, hero_class_id, hero_gender_id, experience, level FROM hero WHERE hero.hero_id='{0}'", id), Database.Instance.Connection);

            // execute SQL (retrieve hero data for the hero whose hero_id matches the input id
            SQLiteDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    // load data into hero instance
                    hero = new Hero(reader.GetInt32(1), reader.GetString(2), reader.GetInt32(3), reader.GetInt32(4), reader.GetInt32(5), reader.GetInt32(6));
                    // set hero id (not set by constructor)
                    hero.Id = reader.GetInt32(0);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return String.Format("Id: {0}, Name: {1}, HeroClass: {2}, Gender: {3}, Exp: {4}, Level: {5}", Id, Name, HeroClass, Gender, Experience, Level);
        }
    }
}
