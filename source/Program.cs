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
        static Account currentAccount = null;
        static void Main(string[] args)
        {
            try
            {
                Database.Instance.Connect();
                if (Database.Instance.Connection.State != System.Data.ConnectionState.Open)
                {
                    Console.WriteLine("Failed to open connection");
                    Console.ReadLine();
                    return;
                }
                Console.WriteLine("Connected to db");
                Console.WriteLine();
                PrintHelp();

                while (true)
                {
                    if (currentAccount != null)
                        Console.Write(currentAccount.Name);
                    Console.Write(">");
                    var command = Console.ReadLine();
                    switch (command)
                    {
                        case "exit":
                            return;
                            break;
                        case "list accounts":
                            CommandListAccounts();
                            break;
                        case "create account":
                            CommandCreateAccount();
                            break;
                        case "login":
                            CommandLogin();
                            break;
                        case "logout":
                            CommandLogout();
                            break;
                        case "list heroes":
                            CommandListHeroes();
                            break;
                        case "create hero":
                            CommandCreateHero();
                            break;
                        default:
                            Console.WriteLine("Unknown command");
                            PrintHelp();
                            break;
                    }
                }
            }
            finally
            {
                try { Database.Instance.Connection.Close(); } catch { }
            }
        }

        static void PrintHelp()
        {
            Console.WriteLine("Commands: ");
            Console.WriteLine("exit, login, logout, list accounts, create account, create hero, list heroes");
        }

        static void CommandCreateAccount()
        {
            Console.Write("Name: ");
            var name = Console.ReadLine();
            Console.Write("Password: ");
            var password = Console.ReadLine();
            Console.Write("Gold: ");
            var goldString = Console.ReadLine();
            Console.Write("Gender (male/female): ");
            var gender = Console.ReadLine() == "male" ? 1 : 2;
            int gold;
            int.TryParse(goldString, out gold);

            var account = new Account(name, gold, gender);
            if (account.Create(password))
                Console.WriteLine("Account {0} created", name);
            else
                Console.WriteLine("Account already exists");
        }

        static void CommandListAccounts()
        {
            SQLiteCommand command = new SQLiteCommand(string.Format("SELECT account_id, account_name FROM account ORDER BY account_id ASC"), Database.Instance.Connection);
            SQLiteDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    var account_id = reader.GetInt32(0);
                    var account_name = reader.GetString(1);
                    Console.WriteLine("{0}: {1}", account_id, account_name);
                }
            }
        }

        static void CommandLogin()
        {
            Console.Write("Name: ");
            var name = Console.ReadLine();
            Console.Write("Password: ");
            var password = Console.ReadLine();

            if (Account.Authorize(name, password, out currentAccount))
            {
                Console.WriteLine("Logged in as {0}", name);
            }
            else
            {
                Console.WriteLine("Failed to login");
            }
        }

        static void CommandLogout()
        {
            currentAccount = null;
            Console.WriteLine("Logged out");
        }

        static void CommandListHeroes()
        {
            if (currentAccount == null)
            {
                Console.WriteLine("not logged in");
                return;
            }
            foreach (var hero in currentAccount.GetHeroes())
            {
                Console.WriteLine(hero);
            }
        }

        static void CommandCreateHero()
        {
            if (currentAccount == null)
            {
                Console.WriteLine("not logged in");
                return;
            }
            Console.Write("Name: ");
            var name = Console.ReadLine();
            Console.Write("Hero class 1-5: ");
            var heroClassString = Console.ReadLine();
            Console.Write("Gender (male/female): ");
            var gender = Console.ReadLine() == "male" ? 1 : 2;
            Console.Write("Level: ");
            var levelString = Console.ReadLine();
            Console.Write("Experience: ");
            var experienceString = Console.ReadLine();
            int heroClass;
            int.TryParse(heroClassString, out heroClass);

            int level;
            int.TryParse(levelString, out level);
            int experience;
            int.TryParse(experienceString, out experience);

            var hero = new Hero(name, heroClass, gender, experience, level);
            if (hero.Create(currentAccount.Id))
                Console.WriteLine("Hero {0} created", name);
            else
                Console.WriteLine("Hero already exists");
        }
    }
}
