//  ///////////////////////////////////////////////////////////////////////////////////////////////
//  @File: Account.cs
//
//  @Project: D3SharpDatabase
//  @Version: 1.0
//  @Created: 21.09.2011
//  @Author: D3Sharp Database Team 
//
//  @Environment: Windows 
//  @References:  SQLite
//
//  @Description: This file provides all account related database communication functions.
//                
//  @Usage: Database communication.
//  @Notes: You need SQLite installed to get this working.
//
//  @Modifications: - 21-September-2010 (TeTuBe)
//                
//  ///////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace D3Database
{
    public class Account
    {
        #region Properties
        
        /// <summary>
        /// The database ID for this account.
        /// </summary>
        public int Id { get; private set; }
        
        /// <summary>
        /// The name of this account.
        /// </summary>
        public string Name { get; private set; }
        
        /// <summary>
        /// The amount of gold associated with this account.
        /// </summary>
        public int Gold { get; set; }

        /// <summary>
        /// The gender of this account (1 = male, 2 = female)
        /// </summary>
        public int Gender { get; private set; }

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Name of the account.</param>
        /// <param name="gold">Amount of gold.</param>
        /// <param name="gender">Account gender.</param>
        public Account(string name, int gold, int gender)
        {
            Id = -1;
            Name = name;
            Gold = gold;
            Gender = gender;
        }

        /// <summary>
        /// Updates account information in the database.
        /// </summary>
        /// <returns>False on failure, otherwise true.</returns>
        public bool Save()
        {
            try
            {
                // initialize SQL statement
                SQLiteCommand command = new SQLiteCommand(string.Format("UPDATE account SET gold='{1}' WHERE account_id='{0}'", Id, Gold), Database.Instance.Connection);
                // execute SQL (update appropriate account row with new data)
                command.ExecuteNonQuery();
                return true;
            }
            catch (Exception e)
            {
                // red for errors
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Failed to save hero to database:");
                // reset colour
                Console.ResetColor();
                // output exception detail
                Console.WriteLine(e.Message);
                return false;
            }
        }

        /// <summary>
        /// Inserts account information in the database.
        /// </summary>
        /// <param name="password">Account password.</param>
        /// <returns>False if the account already exists, otherwise true.</returns>
        public bool Create(string password)
        {
            // false if default ID
            if (Id != -1)
            {
                return false;
            }
            // false if account exists in the database
            if (CheckIfAccountExists(Name))
            {
                return false;
            }

            // MD5 hash of account password
            string md5Password = GetMD5Hash(password);

            // initialize SQL statement
            SQLiteCommand command = new SQLiteCommand(string.Format("INSERT INTO account (account_name, password, gold, gender) VALUES('{0}','{1}','{2}','{3}')", Name, md5Password, Gold, Gender), Database.Instance.Connection);

            // execute SQL (insert account data into the database), return false if insertion failed
            if (command.ExecuteNonQuery() == 0)
            {
                return false;
            }

            // set instance ID to that of the inserted row
            Id = Database.Instance.GetLastInsertId();
            return true;
        }

        /// <summary>
        /// Checks if an account already exists in the database.
        /// </summary>
        /// <param name="account_name">Name of the account to check.</param>
        /// <returns>True if the account already exists, otherwise false.</returns>
        public static bool CheckIfAccountExists(string account_name)
        {
            // initialize SQL statement
            SQLiteCommand command = new SQLiteCommand(string.Format("SELECT account_id FROM account WHERE account_name='{0}'", account_name), Database.Instance.Connection);
            // execute SQL (select row where account_name matches input)
            SQLiteDataReader reader = command.ExecuteReader();
            return reader.HasRows;
        }

        /// <summary>
        /// Checks if an account already exists in the database.
        /// </summary>
        /// <param name="account_id">ID of account to check.</param>
        /// <returns>True if the account already exists, otherwise false.</returns>
        public static bool CheckIfAccountExists(int account_id)
        {
            // initialize SQL statement
            SQLiteCommand command = new SQLiteCommand(string.Format("SELECT account_id FROM account WHERE account_id='{0}'", account_id), Database.Instance.Connection);
            // execute SQL (select row where account_id matches input)
            SQLiteDataReader reader = command.ExecuteReader();
            return reader.HasRows;
        }

        /// <summary>
        /// Loads an account from the database.
        /// </summary>
        /// <param name="id">ID of the account to load.</param>
        /// <param name="account">Account data.</param>
        /// <returns>True on success, otherwise false.</returns>
        public static bool Load(int id, out Account account)
        {
            account = null;

            // initialize SQL statement
            SQLiteCommand command = new SQLiteCommand(string.Format("SELECT account_id, account_name, gold, gender FROM account WHERE account_id='{0}'", id), Database.Instance.Connection);
            // execute SQL (retrieve account data where account_id matches input)
            SQLiteDataReader reader = command.ExecuteReader();

            // continue if a row was returned
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    account = new Account(reader.GetString(1), reader.GetInt32(2), reader.GetInt32(3));
                    account.Id = reader.GetInt32(0);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Validates account credentials with those stored in the database.
        /// </summary>
        /// <param name="account_name">Account name.</param>
        /// <param name="password">Account password.</param>
        /// <param name="account">Reference to the account instance.</param>
        /// <returns>False if account name and password hash don't match anything in the database, otherwise true.</returns>
        public static bool Authorize(string account_name, string password, out Account account)
        {
            account = null;

            // MD5 hash of account password
            string md5Password = GetMD5Hash(password);

            // initialize SQL statement
            SQLiteCommand command = new SQLiteCommand(string.Format("SELECT account_id FROM account WHERE account_name='{0}' AND password='{1}'", account_name, md5Password), Database.Instance.Connection);
            // execute SQL (retrieve row with account name and hashed password that match input)
            SQLiteDataReader reader = command.ExecuteReader();

            // false if no rows were returned
            if (!reader.HasRows)
            {
                return false;
            }

            reader.Read();
            int account_id = reader.GetInt32(0);
            return Load(account_id, out account);
        }

        /// <summary>
        /// Returns a collection of heroes associated with this account from the database.
        /// </summary>
        /// <returns>List of heroes associated with this account.</returns>
        public List<Hero> GetHeroes()
        {
            // initialize collection
            List<Hero> heroList = new List<Hero>();
            
            // initialize SQL statement
            SQLiteCommand command = new SQLiteCommand(string.Format("SELECT hero_id FROM hero WHERE account_id='{0}'", Id), Database.Instance.Connection);
            // execute SQL (get hero ids from database where account_id matches if of this instance)
            SQLiteDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    int hero_id = reader.GetInt32(0);
                    Hero hero;
                    if (!Hero.Load(hero_id, out hero))
                    {
                        // red for errors
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Failed to load hero with id: {0}", hero_id);
                        // reset colour
                        Console.ResetColor();
                    }
                    else
                    {
                        // add hero data to collection
                        heroList.Add(hero);
                    }
                }
            }

            return heroList;
        }

        /// <summary>
        /// Returns a collection of banners associated with this account from the database.
        /// </summary>
        /// <returns>List of banners associated with this account</returns>
        public List<AccountBanner> GetBanners()
        {
            // initialize collection
            List<AccountBanner> accountBannerList = new List<AccountBanner>();

            // initialize SQL statement
            SQLiteCommand command = new SQLiteCommand(string.Format("SELECT account_banner_id FROM account_banner WHERE account_id='{0}'", Id), Database.Instance.Connection);
            // execute SQL (retrieve banner id from database where account_id matches id of this instance)
            SQLiteDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    int account_banner_id = reader.GetInt32(0);
                    AccountBanner accountBanner;
                    if (!AccountBanner.Load(account_banner_id, out accountBanner))
                    {
                        // red for errors
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Failed to load account banner with id: {0}", account_banner_id);
                        // reset colour
                        Console.ResetColor();
                    }
                    else
                    {
                        // add banner to collection
                        accountBannerList.Add(accountBanner);
                    }

                }
            }

            return accountBannerList;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return String.Format("{0}\t{1}\t{2}\t{3}", Id, Name, Gold, Gender);
        }

        /// <summary>
        /// Creates an MD5 hash of a string.
        /// </summary>
        /// <param name="input">The string to hash.</param>
        /// <returns>A cryptographic hash of the input string</returns>
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
