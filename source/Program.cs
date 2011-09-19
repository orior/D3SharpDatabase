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
                string dbFile = "d3sharp_fixed20110918_2.sqlite";
                Database.Instance.Connect(dbFile);
                if (Database.Instance.Connection.State != System.Data.ConnectionState.Open)
                {
                    Console.WriteLine("Failed to open connection to: {0}", dbFile);
                    Console.ReadLine();
                    return;
                }
                Console.WriteLine("Connected to {0}", dbFile);
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
                        case "create banner":
                            CommandCreateBanner();
                            break;
                        case "list banners":
                            CommandListBanners();
                            break;
                        case "hero level up":
                            CommandHeroLevelUp();
                            break;
                        default:
                            Console.WriteLine("Unknown command");
                            PrintHelp();
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("We got a crash!");
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                Console.ReadLine();
            }
            finally
            {
                try { Database.Instance.Connection.Close(); } catch { }
            }
        }

        static void PrintHelp()
        {
            Console.Write("Commands: ");
            Console.Write("exit, login, logout, list accounts, create account, create hero, list heroes, hero level up" + Environment.NewLine);
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
            else
                Console.WriteLine("No accounts");
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
            var heroes = currentAccount.GetHeroes();
            if (heroes.Count > 0)
            {
                foreach (var hero in heroes)
                {
                    Console.WriteLine(hero);
                }
            }
            else
                Console.WriteLine("No heroes");
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

        static void CommandCreateBanner()
        {
            if (currentAccount == null)
            {
                Console.WriteLine("not logged in");
                return;
            }
            Console.Write("BackgroundColor: ");
            var backgroundColorString = Console.ReadLine();
            Console.Write("Banner: ");
            var bannerString = Console.ReadLine();
            Console.Write("Pattern: ");
            var patternString = Console.ReadLine();
            Console.Write("PatternColor: ");
            var patternColorString = Console.ReadLine();
            Console.Write("Placement: ");
            var placementString = Console.ReadLine();
            Console.Write("SigilAccent: ");
            var sigilAccentString = Console.ReadLine();
            Console.Write("SigilMain: ");
            var sigilMainString = Console.ReadLine();
            Console.Write("SigilColor: ");
            var sigilColorString = Console.ReadLine();

            int backgroundColor;
            int.TryParse(backgroundColorString, out backgroundColor);
            int banner;
            int.TryParse(bannerString, out banner);
            int pattern;
            int.TryParse(patternString, out pattern);
            int patternColor;
            int.TryParse(patternColorString, out patternColor);
            int placement;
            int.TryParse(placementString, out placement);
            int sigilAccent;
            int.TryParse(sigilAccentString, out sigilAccent);
            int sigilMain;
            int.TryParse(sigilMainString, out sigilMain);
            int sigilColor;
            int.TryParse(sigilColorString, out sigilColor);

            Console.Write("UseSigilVariant (yes/no): ");
            var useSigilVariant = Console.ReadLine() == "yes";

            var accountBanner = new AccountBanner(currentAccount.Id, backgroundColor, banner, pattern, patternColor, placement, sigilAccent, sigilMain, sigilColor, useSigilVariant);
            if(accountBanner.Create())
                Console.WriteLine("Banner created");
            else
                Console.WriteLine("Failed to create banner");
        }

        static void CommandListBanners()
        {
            if (currentAccount == null)
            {
                Console.WriteLine("not logged in");
                return;
            }

            var banners = currentAccount.GetBanners();
            if (banners.Count > 0)
            {
                foreach (var banner in banners)
                {
                    Console.WriteLine(banner);
                }
            }
            else
                Console.WriteLine("No account banners");
        }

        static void CommandHeroLevelUp()
        {
            if (currentAccount == null)
            {
                Console.WriteLine("not logged in");
                return;
            }
            Console.Write("Hero id: ");
            var heroIdString = Console.ReadLine();
            int heroId;
            int.TryParse(heroIdString, out heroId);
            if (heroId == 0 && heroIdString != "0")
            {
                Console.WriteLine("Invalid hero id");
                return;
            }

            var hero = currentAccount.GetHero(heroId);
            if (hero == null)
                Console.WriteLine("Could not find hero on current account");
            hero.Level += 1;
            hero.Save();
            Console.WriteLine("Hero {0} is now level {1}", hero.Name, hero.Level);
        }
    }
}
