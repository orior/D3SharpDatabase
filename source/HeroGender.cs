using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
namespace D3Database
{
    public class HeroGender
    {
        public int GenderId { get; set; }
        public string CharacterName { get; set; }
        public int CharacterClass { get; set; }
        public HeroGender(int GenderID, string CharacterName, int CharacterClass)
        {
            this.GenderId = GenderId;
            this.CharacterClass = CharacterClass;
            this.CharacterName = CharacterName;
        }
        public static bool Load(int genderid, out HeroGender herogender)
        {
            herogender = null;
            try
            {
                SQLiteCommand command = new SQLiteCommand(string.Format("SELECT * FROM hero_gender WHERE gender_id='{0}'", genderid), Database.Instance.Connection);
                SQLiteDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        herogender = new HeroGender(reader.GetInt32(0), reader.GetString(1), reader.GetInt32(2));
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to load HeroGender exception: {0}", e.Message);
                return false;
            }

            return false;
        }




    }
}
