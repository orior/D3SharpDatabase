using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;

namespace D3Database
{
    class HeroTitles
    {
        public int TitleID { get; set; }
        public string TitleName { get; set; }
        public int Level { get; set; }
        public int Gender { get; set; }
        public string HeroTitle { get; set; }
        public HeroTitles(int TitleID, string TitleName, int Level, int Gender, string HeroTitle)
        {
            this.TitleID = TitleID;
            this.TitleName = TitleName;
            this.Level = Level;
            this.Gender = Gender;
            this.HeroTitle = HeroTitle;
        }
        public static bool Load(int titleid, out HeroTitles herotitle)
        {
            herotitle = null;
            try {
                SQLiteCommand command = new SQLiteCommand(string.Format("SELECT * FROM hero_titles WHERE title_id='{0}'", titleid), Database.Instance.Connection);
                SQLiteDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        herotitle = new HeroTitles(reader.GetInt32(0), reader.GetString(1), reader.GetInt32(2), reader.GetInt32(3), reader.GetString(4));
                        return true;
                    }
                }
              }
              catch (Exception e)
            {
                Console.WriteLine("Failed to load HeroTitles exception: {0}", e.Message);
                return false;
            }
            return false;
        }
    }
}



