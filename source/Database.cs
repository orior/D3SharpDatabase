using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;

namespace D3Database
{
    public class Database
    {
        #region Singleton
        private static volatile Database instance;
        private static object syncRoot = new Object();

        private Database() { }

        public static Database Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new Database();
                    }
                }

                return instance;
            }
        }
        #endregion

        public SQLiteConnection Connection{get; private set;}

        public void Connect()
        {
            //Connection = new SQLiteConnection("Data Source=d3sharp.sqlite");
            // Connection = new SQLiteConnection("Data Source=d3db_fixed.db");
            Connection = new SQLiteConnection("Data Source=d3sharp_fixed.sqlite");
            Connection.Open();
        }

        public int GetLastInsertId()
        {
            SQLiteCommand command = new SQLiteCommand("SELECT last_insert_rowid()", Database.Instance.Connection);
            return Convert.ToInt32(command.ExecuteScalar());
        } 
    }
}
