using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Security.Cryptography;

namespace D3Database
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string account_name = "diabloIII@emu.db";
                string password = "pass";
                string hero_name = "diabloIII"; // todo: random name

                Database.Instance.Connect();
                if (Database.Instance.Connection.State != System.Data.ConnectionState.Open)
                {
                    Console.WriteLine("Failed to open Database.Instance.Connection");
                    Console.ReadLine();
                    return;
                }
                Console.WriteLine("Connected to db.");

                Account account;
                if (!Account.Authorize(account_name, password, out account))
                {
                    Console.WriteLine("Failed to authorize account: {0}", account_name);
                    account = new Account(account_name, 0, 1);
                    if (!account.Create(password))
                    {
                        Console.WriteLine("Failed to create account: {0}", account_name);
                        Console.ReadLine();
                        return;
                    }
                    else
                        Console.WriteLine("Created account: {0}", account_name);
                }
                Console.WriteLine("Authorized.");
                Console.WriteLine("\r\n\r\nCharacter Data before Gold Insert:\r\n\r\nId\tName\t\t\tGold\tGender");
                Console.WriteLine(account.ToString());
                Console.WriteLine("\r\n\r\nCharacter Data after 10 Gold was Inserted:\r\n\r\nId\tName\t\t\tGold\tGender");
                account.Gold += 10;
                account.Save();
                Console.WriteLine(account.ToString());

                Hero heroNew = new Hero(hero_name, 2, 1, 2, 1);
                if (!heroNew.Create(account.Id))
                    Console.WriteLine("Failed to create hero! Already exists !?!\r\n");
                else
                    Console.WriteLine("Hero created.");

                List<Hero> heroList = account.GetHeroes();
                Console.WriteLine("{0} Hero(es): ", heroList.Count);
                Console.WriteLine("id\tname\t\tclass\tgender\texp\tlevel");
                foreach (var hero in heroList)
                {
                    Console.WriteLine(hero);
                    hero.Experience += 10;
                    hero.Level += 1;
                    hero.Save();
                }

                Console.ReadLine();
            }
            finally
            {
                try { Database.Instance.Connection.Close(); } catch { }
            }
        }
    }
}
