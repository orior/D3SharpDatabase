using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;

namespace D3Database
{
    public class HeroClasses
    {
        public int CharacterID { get; private set; }
        public string CharacterName { get; private set; }
        public int CharacterClass { get; private set; }
        public HeroClasses(int CharacterID,string CharacterName,int CharacterClass)
        {
            this.CharacterID = CharacterID;
            this.CharacterName = CharacterName;
            this.CharacterClass = CharacterClass;
        }

        public static bool Load(int characterid, out HeroClasses heroclass)
        {
            heroclass = null;
            try {
                // no table called hero_classes
                //SQLiteCommand command = new SQLiteCommand(string.Format("SELECT * FROM hero_classes WHERE character_id='{0}'", characterid), Database.Instance.Connection);
                // need info on this
                SQLiteCommand command = new SQLiteCommand(string.Format("SELECT * FROM hero WHERE hero_id='{0}", characterid), Database.Instance.Connection);
                SQLiteDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        heroclass = new HeroClasses(reader.GetInt32(0), reader.GetString(1), reader.GetInt32(2));
                        return true;
                    }
                }
            }
            
            catch (Exception e)
            {
                Console.WriteLine("Failed to load HeroClasses exception: {0}", e.Message);
                return false;
            }
            return false;
        }
    }
}
