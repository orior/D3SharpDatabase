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

        public void Connect(string databaseFile)
        {
            Connection = new SQLiteConnection(string.Format("Data Source={0}", databaseFile));
            Connection.Open();
        }

        public int GetLastInsertId()
        {
            var command = new SQLiteCommand("SELECT last_insert_rowid()", Database.Instance.Connection);
            return Convert.ToInt32(command.ExecuteScalar());
        }

        public int Update(string table, List<SQLiteParameter> insertParameters, string whereSQL, List<SQLiteParameter> whereParameters)
        {
            string sql = "UPDATE {0} SET {1} WHERE {2}";

            var sbInsert = new StringBuilder();
            foreach(var parameter in insertParameters)
                sbInsert.AppendFormat("`{0}`={1},", parameter.ParameterName.Replace("@",""), parameter.ParameterName);
            sbInsert.Length -= 1;

            sql = string.Format(sql, table, sbInsert, whereSQL);

            var command = new SQLiteCommand(sql, Database.Instance.Connection);
            command.Parameters.AddRange(insertParameters.ToArray());
            command.Parameters.AddRange(whereParameters.ToArray());
            return command.ExecuteNonQuery();
        }
    }
}
