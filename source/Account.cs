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
using System.Linq;
using System.Text;
using System.Data.SQLite;

namespace D3Database
{

    //  ///////////////////////////////////////////////////////////////////////////////////////////
    //  @Class: Account
    //  ///////////////////////////////////////////////////////////////////////////////////////////
    public class Account
    {
        //  All getters and setters.
        public int Id { get; private set; }
        public string Name { get; private set; }
        public int Gold { get; set; }
        public int Gender { get; private set; }

        //  Constructors.
        public Account(string name, int gold, int gender)
        {
            Id = -1;
            Name = name;
            Gold = gold;
            Gender = gender;
        }

        //  ///////////////////////////////////////////////////////////////////////////////////////
        //  @Function: Save
        //
        //  @Description: Stores all account related information into the databse.
        //  @Parameter: -
        //  @Return: Returns true, false if storage failed.
        //  //////////////////////////////////////////////////////////////////////////////////////
        public bool Save()
        {
            try
            {
                SQLiteCommand command = new SQLiteCommand(string.Format(
                    "UPDATE account SET gold='{1}' WHERE account_id='{0}'", 
                    Id,
                    Gold), 
                    Database.Instance.Connection);
                command.ExecuteNonQuery();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to save hero exception: {0}", e.Message);
                return false;
            }
        }

        //  ///////////////////////////////////////////////////////////////////////////////////////
        //  @Function: Create
        //
        //  @Description: Creates a new account in the database.
        //  @Parameter:
        //      +Password: A Password.              
        //  @Return: Returns false, if the account already exists, else true.
        //  //////////////////////////////////////////////////////////////////////////////////////
        public bool Create(string password)
        {
            if (Id != -1)
                return false;
            if (CheckIfAccountExists(Name))
                return false;
            string md5Password = GetMD5Hash(password);
            SQLiteCommand command = new SQLiteCommand(string.Format(
                "INSERT INTO account (account_name, password, gold, gender) VALUES('{0}','{1}','{2}','{3}')",
                Name, 
                md5Password,
                Gold, 
                Gender), 
                Database.Instance.Connection);
            int affectedRows = command.ExecuteNonQuery();
            if (affectedRows == 0)
                return false;
            Id = Database.Instance.GetLastInsertId();
            return true;
        }

        //  ///////////////////////////////////////////////////////////////////////////////////////
        //  @Function: CheckifAccountExists
        //
        //  @Description: Checks, if a given account already exists in the database.
        //  @Parameter:
        //      +account_name: A valid account name.
        //  @Return: Returns the accounts name.
        //  //////////////////////////////////////////////////////////////////////////////////////
        public static bool CheckIfAccountExists(string account_name)
        {
            SQLiteCommand command = new SQLiteCommand(string.Format(
                "SELECT account_id FROM account WHERE account_name='{0}'", 
                account_name),
                Database.Instance.Connection);
            SQLiteDataReader reader = command.ExecuteReader();
            return reader.HasRows;
        }

        //  ///////////////////////////////////////////////////////////////////////////////////////
        //  @Function: CheckifAccounExists
        //
        //  @Description: Checks, if a given account already exists in the database.
        //  @Parameter:
        //      +account_id: A valid account id.
        //  @Return: Returns account id.
        //  //////////////////////////////////////////////////////////////////////////////////////
        public static bool CheckIfAccountExists(int account_id)
        {
            SQLiteCommand command = new SQLiteCommand(string.Format(
                "SELECT account_id FROM account WHERE account_id='{0}'",
                account_id), 
                Database.Instance.Connection);
            SQLiteDataReader reader = command.ExecuteReader();
            return reader.HasRows;
        }

        //  ///////////////////////////////////////////////////////////////////////////////////////
        //  @Procedure: Load
        //
        //  @Description: Loads an account from the database.
        //  @Parameter:
        //      +id: Account id.
        //      +account: Loaded account data.
        //  @Return: True, if data available, false if not.
        //  //////////////////////////////////////////////////////////////////////////////////////
        public static bool Load(int id, out Account account)
        {
            account = null;
            SQLiteCommand command = new SQLiteCommand(string.Format(
                "SELECT account_id, account_name, gold, gender FROM account WHERE account_id='{0}'",
                id), 
                Database.Instance.Connection);
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
            return false;
        }

        //  ///////////////////////////////////////////////////////////////////////////////////////
        //  @Function: Authorize
        //
        //  @Description: Auth's a current account.
        //  @Parameter:
        //      +account_name: A valid account name.
        //      +password: Password to the spezific account.
        //      +account: Login to this account.
        //  @Return: False if account_name doesnt exist, else loads the accont data.
        //  //////////////////////////////////////////////////////////////////////////////////////
        public static bool Authorize(string account_name, string password, out Account account)
        {
            account = null;
            string md5Password = GetMD5Hash(password);
            SQLiteCommand command = new SQLiteCommand(string.Format(
                "SELECT account_id FROM account WHERE account_name='{0}' AND password='{1}'",
                account_name, 
                md5Password), 
                Database.Instance.Connection);
            SQLiteDataReader reader = command.ExecuteReader();
            if (!reader.HasRows)
                return false;
            reader.Read();
            int account_id = reader.GetInt32(0);
            return Load(account_id, out account);
        }

        //  ///////////////////////////////////////////////////////////////////////////////////////
        //  @Function: GetHeroes
        //
        //  @Description: Lists all heros of an account.
        //  @Return: Returns a list with all heroes in.
        //  //////////////////////////////////////////////////////////////////////////////////////
        public List<Hero> GetHeroes()
        {
            var heroList = new List<Hero>();
            SQLiteCommand command = new SQLiteCommand(string.Format(
                "SELECT hero_id FROM hero WHERE account_id='{0}'", 
                Id), 
                Database.Instance.Connection);
            SQLiteDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    var hero_id = reader.GetInt32(0);
                    Hero hero;
                    if (!Hero.Load(hero_id, out hero))
                        Console.WriteLine("Failed to load hero with id: {0}", hero_id);
                    else
                        heroList.Add(hero);

                }
            }
            return heroList;
        }

        //  ///////////////////////////////////////////////////////////////////////////////////////
        //  @Function: GetHero
        //
        //  @Description: Gets hero related data by id out of the database.
        //  @Parameter:
        //      +heroId: Spezific hero id.
        //  @Return: If hero exists, returns the hero data, else returns null.
        //  //////////////////////////////////////////////////////////////////////////////////////
        public Hero GetHero(int heroId)
        {
            var heroList = new List<Hero>();
            SQLiteCommand command = new SQLiteCommand(string.Format(
                "SELECT hero_id FROM hero WHERE account_id='{0}' AND hero_id='{1}'", 
                Id, 
                heroId), 
                Database.Instance.Connection);
            SQLiteDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                Hero hero;
                if (!Hero.Load(heroId, out hero))
                    return null;
                else
                    return hero;
            }
            return null;
        }

        //  ///////////////////////////////////////////////////////////////////////////////////////
        //  @Function: GetBanners
        //
        //  @Description: Shows all banners of an account.
        //  @Return: Returns a list of banner data.
        //  //////////////////////////////////////////////////////////////////////////////////////
        public List<AccountBanner> GetBanners()
        {
            var accountBannerList = new List<AccountBanner>();
            SQLiteCommand command = new SQLiteCommand(string.Format("SELECT account_banner_id FROM account_banner WHERE account_id='{0}'", Id), Database.Instance.Connection);
            SQLiteDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    var account_banner_id = reader.GetInt32(0);
                    AccountBanner accountBanner;
                    if (!AccountBanner.Load(account_banner_id, out accountBanner))
                        Console.WriteLine("Failed to load account banner with id: {0}", account_banner_id);
                    else
                        accountBannerList.Add(accountBanner);

                }
            }
            return accountBannerList;
        }

        //  ///////////////////////////////////////////////////////////////////////////////////////
        //  @Function: ToString
        //
        //  @Description: Returns the string representation of Id, Name, Gold, Gender.
        //  @Return: Returns a string.
        //  //////////////////////////////////////////////////////////////////////////////////////
        public override string ToString()
        {
            return String.Format("{0}\t{1}\t{2}\t{3}", Id, Name, Gold, Gender);
        }

        //  ///////////////////////////////////////////////////////////////////////////////////////
        //  @Function: GetMD5Hash
        //
        //  @Description: Creates an MD5 Hash for a given string.
        //  @Parameter:
        //      +string: A string.
        //  @Return: Returns a MD5 password.
        //  //////////////////////////////////////////////////////////////////////////////////////
        private static string GetMD5Hash(string input)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider x = 
                new System.Security.Cryptography.MD5CryptoServiceProvider();
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
