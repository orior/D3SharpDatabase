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
                string account_name = "orior3";
                string password = "poop";
                Database.Instance.Connect();
                if (Database.Instance.Connection.State != System.Data.ConnectionState.Open)
                {
                    Console.WriteLine("Failed to open Database.Instance.Connection");
                    Console.ReadLine();
                    return;
                }
                Console.WriteLine("Connected to db");

                Account account;
                if (!Account.Authorize(account_name, password, out account))
                {
                    Console.WriteLine("Failed to authorize account");
                    account = new Account(account_name, 0, 1);
                    if (!account.Create(password))
                    {
                        Console.WriteLine("Failed to create account");
                        Console.ReadLine();
                        return;
                    }
                    else
                        Console.WriteLine("Created account");
                }
                Console.WriteLine("Authorized");
                Console.WriteLine(account.ToString());
                Console.WriteLine("10 gold given and saved");
                account.Gold += 10;
                account.Save();
                Console.WriteLine(account.ToString());

                Hero heroNew = new Hero("orior", 2, 1, 2, 1);
                if (!heroNew.Create(account.Id))
                    Console.WriteLine("Failed to create hero");
                else
                    Console.WriteLine("Hero created");

                List<Hero> heroList = account.GetHeroes();
                Console.WriteLine("{0} Heroes: ", heroList.Count);
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
