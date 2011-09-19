using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;

namespace D3Database
{
    public class Account
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public int Gold { get; set; }
        public int Gender { get; private set; }

        private List<Hero> heroList;

        public Account(string name, int gold, int gender)
        {
            Id = -1;
            Name = name;
            Gold = gold;
            Gender = gender;
        }

        public bool Save()
        {
            try
            {
                SQLiteCommand command = new SQLiteCommand(string.Format("UPDATE account SET gold='{1}' WHERE account_id='{0}'", Id, Gold), Database.Instance.Connection);
                command.ExecuteNonQuery();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Account: Failed to Save hero! Exception: {0}", e.Message);
                return false;
            }
        }

        public bool Create(string password)
        {
            if (CheckIfAccountExists(Name))
                return false;
            string md5Password = GetMD5Hash(password);
            SQLiteCommand command = new SQLiteCommand(string.Format("INSERT INTO account (account_name, password, gold, gender) VALUES('{0}','{1}','{2}','{3}')", Name, md5Password, Gold, Gender), Database.Instance.Connection);
            int affectedRows = command.ExecuteNonQuery();
            if (affectedRows == 0)
                return false;
            Id = Database.Instance.GetLastInsertId();
            return true;
        }

        static bool CheckIfAccountExists(string account_name)
        {
            SQLiteCommand command = new SQLiteCommand(string.Format("SELECT account_id FROM account WHERE account_name='{0}'", account_name), Database.Instance.Connection);
            SQLiteDataReader reader = command.ExecuteReader();
            return reader.HasRows;
        }

        public static bool Load(int id, out Account account)
        {
            account = null;
            try {
                SQLiteCommand command = new SQLiteCommand(string.Format("SELECT account_id, account_name, gold, gender FROM account WHERE account_id='{0}'", id), Database.Instance.Connection);
                SQLiteDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var account_id = reader.GetInt32(0);
                        var account_name = reader.GetString(1);
                        var gold = reader.GetInt32(2);
                        var gender = reader.GetInt32(3);
                        account = new Account(account_name, gold, gender);
                        account.Id = account_id;
                        return true;
                    }
                }            
            }
            
             catch (Exception e)
            {
                Console.WriteLine("Account: Failed to load hero! Exception: {0}", e.Message);
                return false;
            }
            return false;
        }

        public static bool Authorize(string account_name, string password, out Account account)
        {
            account = null;
            string md5Password = GetMD5Hash(password);
            SQLiteCommand command = new SQLiteCommand(string.Format("SELECT account_id FROM account WHERE account_name='{0}' AND password='{1}'", account_name, md5Password), Database.Instance.Connection);
            SQLiteDataReader reader = command.ExecuteReader();
            if (!reader.HasRows)
                return false;
            reader.Read();
            int account_id = reader.GetInt32(0);
            return Load(account_id, out account);
        }

        public List<Hero> GetHeroes()
        {
            if (heroList != null)
                return heroList;
            heroList = new List<Hero>();
            SQLiteCommand command = new SQLiteCommand(string.Format("SELECT hero_id FROM hero WHERE account_id='{0}'", Id), Database.Instance.Connection);
            SQLiteDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    var hero_id = reader.GetInt32(0);
                    Hero hero;
                    if (!Hero.Load(hero_id, out hero))
                        Console.WriteLine("Account: Failed to load hero with id: {0}", hero_id);
                    else
                        heroList.Add(hero);

                }
            }
            return heroList;
        }

        public override string ToString()
        {
            return String.Format("{0}\t{1}\t{2}\t{3}\r\n\r\n", Id, Name, Gold, Gender);
        }

        private static string GetMD5Hash(string input)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] bs = System.Text.Encoding.UTF8.GetBytes(input);
            bs = x.ComputeHash(bs);
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            foreach (byte b in bs)
                s.Append(b.ToString("x2").ToLower());
            string password = s.ToString();
            return password;
        }    

    }
}
