using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;

namespace D3Database
{
    public static class ExperienceLevels
    {
        public static bool Load(int level, out int experience)
        {
            experience = -1;
            try {
                SQLiteCommand command = new SQLiteCommand(string.Format("SELECT experience FROM experience_levels WHERE level='{0}'", level), Database.Instance.Connection);
                SQLiteDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        experience = reader.GetInt32(0);
                        return true;
                    }
                }
            }
           
            catch (Exception e)
            {
                Console.WriteLine("ExperienceLevels: Failed to load experience levels! Exception: {0}", e.Message);
                return false;
            }
            return false;
        }



        
    }
}
