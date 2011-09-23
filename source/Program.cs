using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text.RegularExpressions;

namespace D3Database
{
    class Program
    {
        /// <summary>
        /// Represents the account of the currently signed in user. Null if no user is signed in.
        /// </summary>
        static Account currentAccount = null;

        /// <summary>
        /// Program entry point.
        /// </summary>
        static void Main(string[] args)
        {
            Console.Title = "D3SharpDatabase CLI";
            try
            {
                string dbFile = "d3sharp_fixed20110918_2.sqlite";
                Database.Instance.Connect(dbFile);
                if (Database.Instance.Connection.State != System.Data.ConnectionState.Open)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("Failed to open connection to: ");
                    Console.WriteLine(dbFile);
                    Console.ResetColor();
                    Console.WriteLine("Press any key to exit.");
                    Console.ReadKey();
                    return;
                }

                Console.WriteLine("Connected to {0}", dbFile);
                PrintHelp();

                while (true)
                {
                    if (currentAccount != null)
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(currentAccount.Name);
                        Console.ResetColor();
                    }

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
                        case "clear":
                            Console.Clear();
                            break;
                        default:
                            Console.WriteLine("Unknown command");
                            PrintHelp();
                            break;
                    }
                }
            }
            //catch (Exception e)
            //{
            //    Console.ForegroundColor = ConsoleColor.Red;
            //    Console.WriteLine("Error:");
            //    Console.ResetColor();
            //    Console.WriteLine(e.Message);
            //    Console.WriteLine(e.StackTrace);
            //    Console.ReadLine();
            //}
            finally
            {
                try { Database.Instance.Connection.Close(); }
                catch { }
            }
        }

        /// <summary>
        /// Outputs a list of acceptable commands.
        /// </summary>
        static void PrintHelp()
        {
            Console.Write("Commands: ");
            Console.Write("exit, login, logout, list accounts, create account, create hero, list heroes, hero level up, clear" + Environment.NewLine);
        }

        /// <summary>
        /// Handles the "create account" command which attempts to create a new account in the database.
        /// </summary>
        static void CommandCreateAccount()
        {
            // prompt for account name
            Console.Write("Name: ");

            // read input
            string name = Console.ReadLine();

            // check that name is valid
            while (string.IsNullOrEmpty(name))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid name.");
                Console.ResetColor();
                Console.Write("Name: ");
                name = Console.ReadLine();
            }

            string password;
            string passwordConfirm;

            // prompt for password
            Console.Write("Password: ");

            // hide password while typing
            Console.ForegroundColor = ConsoleColor.Black;

            password = Console.ReadLine();

            // check that password is valid
            while (string.IsNullOrEmpty(password))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid password.");
                Console.ResetColor();
                Console.Write("Password: ");
                // hide password while typing
                Console.ForegroundColor = ConsoleColor.Black;
                password = Console.ReadLine();
            }

            Console.ResetColor();

            // prompt for password again
            Console.Write("Confirm Password: ");

            // hide password while typing
            Console.ForegroundColor = ConsoleColor.Black;

            passwordConfirm = Console.ReadLine();

            // check that passwords match
            while (passwordConfirm != password)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Passwords don't match.");
                Console.ResetColor();

                // prompt for password
                Console.Write("Password: ");

                // hide password while typing
                Console.ForegroundColor = ConsoleColor.Black;

                password = Console.ReadLine();

                // check that password is valid
                while (string.IsNullOrEmpty(password))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid password.");
                    Console.ResetColor();
                    Console.Write("Password: ");
                    // hide password while typing
                    Console.ForegroundColor = ConsoleColor.Black;
                    password = Console.ReadLine();
                }

                Console.ResetColor();

                // prompt for password again
                Console.Write("Confirm Password: ");

                // hide password while typing
                Console.ForegroundColor = ConsoleColor.Black;

                passwordConfirm = Console.ReadLine();

                Console.ResetColor();
            }
            
            // prompt for gold amount
            Console.Write("Gold: ");
            
            // read input
            string goldString = Console.ReadLine();
            int gold;

            // check that gold is numeric
            while (int.TryParse(goldString, out gold))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid gold. Please choose a number.");
                Console.ResetColor();
                Console.Write("Gold: ");
                goldString = Console.ReadLine();
            }

            // prompt for gender
            Console.Write("Gender (male/female): ");

            int gender = 0;

            // check that gender is valid (1 or 2)
            while (gender == 0)
            {
                switch (Console.ReadLine().ToLower().Trim())
                {
                    case "male":
                    case "m":
                        gender = 1;
                        break;
                    case "female":
                    case "f":
                        gender = 2;
                        break;
                    default:
                        // red for errors
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Invalid gender. Please choose male or female.");
                        // reset colour
                        Console.ResetColor();
                        Console.Write("Gender (male/female): ");
                        break;
                }
            }            

            // initialize account
            Account account = new Account(name, gold, gender);

            // save account to database
            if (account.Create(password))
            {
                Console.Write("Account ");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(name);
                Console.ResetColor();
                Console.Write(" created.");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("An account with that name already exists");
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Handles the "list accounts" command which lists all accounts in the database.
        /// </summary>
        static void CommandListAccounts()
        {
            // initialize SQL
            SQLiteCommand command = new SQLiteCommand(string.Format("SELECT account_id, account_name FROM account ORDER BY account_id ASC"), Database.Instance.Connection);
            // execute SQL (retrieve all accounts from database)
            SQLiteDataReader reader = command.ExecuteReader();

            // accounts found
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    // <id>: <account_name>
                    Console.WriteLine("{0}: {1}", reader.GetInt32(0), reader.GetString(1));
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("No accounts found in the database.");
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Handles the "login" command which attempts to log in as a specific user.
        /// </summary>
        static void CommandLogin()
        {
            // prompt for name
            Console.Write("Name: ");
            string name = Console.ReadLine().Trim();
            // prompt for password
            Console.Write("Password: ");

            // hide password while typing
            Console.ForegroundColor = ConsoleColor.Black;
            string password = Console.ReadLine();
            Console.ResetColor();

            // attempt log in
            if (Account.Authorize(name, password, out currentAccount))
            {
                Console.Write("Logged in as ");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(name);
                Console.ResetColor();
                Console.WriteLine(".");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Failed to log in.");
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Handles the "logout" command which logs out the currently logged in user.
        /// </summary>
        static void CommandLogout()
        {
            // check that a user is even logged in
            if (currentAccount != null)
            {
                currentAccount = null;
                Console.WriteLine("Logged out.");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Already logged out.");
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Handles the "list heroes" command which lists all heroes in the database associated with the currently logged in account.
        /// </summary>
        static void CommandListHeroes()
        {
            // check that the user is logged in
            if (currentAccount == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Not logged in.");
                Console.ResetColor();
                return; // exit
            }

            // get heroes for this account
            List<Hero> heroes = currentAccount.GetHeroes();

            // check that there are one or more heroes
            if (heroes.Count > 0)
            {
                // iterate through collection
                foreach (Hero hero in heroes)
                {
                    string classStr;
                    ConsoleColor classColour;

                    switch (hero.HeroClass)
                    {
                        case 1:
                            classStr = "Wizard";
                            classColour = ConsoleColor.Cyan;
                            break;
                        case 2:
                            classStr = "Witch Doctor";
                            classColour = ConsoleColor.Green;
                            break;
                        case 3:
                            classStr = "Demon Hunter";
                            classColour = ConsoleColor.Magenta;
                            break;
                        case 4:
                            classStr = "Monk";
                            classColour = ConsoleColor.Yellow;
                            break;
                        case 5:
                            classStr = "Barbarian";
                            classColour = ConsoleColor.Red;
                            break;
                        default:
                            classStr = "Unknown";
                            classColour = ConsoleColor.Gray;
                            break;
                    }

                    string genderStr;

                    switch (hero.Gender)
                    {
                        case 1:
                            genderStr = "male";
                            break;
                        case 2:
                            genderStr = "female";
                            break;
                        default:
                            genderStr = "unknown";
                            break;
                    }

                    Console.Write(hero.Id.ToString().PadLeft(3, '0'));
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write(" | ");
                    Console.ResetColor();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(hero.Name.PadRight(12));
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write(" | ");
                    Console.ResetColor();
                    Console.Write("level " + hero.Level.ToString().PadLeft(3, '0'));
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write(" | ");
                    Console.ResetColor();
                    Console.Write(genderStr.PadRight(6));
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write(" | ");
                    Console.ForegroundColor = classColour;
                    Console.Write(classStr.PadRight(12));
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write(" | ");
                    Console.ResetColor();
                    Console.WriteLine(" " + hero.Experience.ToString() + " XP");
                }

                // NO MORE HEROES ANYMOOOORE
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("No heroes associated with this account.");
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Handles the "create hero" command which creates a hero under the currently logged in account and saves it to the database.
        /// </summary>
        static void CommandCreateHero()
        {
            // check that user is logged in
            if (currentAccount == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Not logged in.");
                Console.ResetColor();
                return; // exit
            }

            // prompt for hero name
            Console.Write("Name: ");

            // read input
            string name = Console.ReadLine();

            // check that name doesn't contain numbers
            while (Regex.IsMatch(name, @"\d"))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid name. Names cannot contain numbers.");
                Console.ResetColor();
                Console.Write("Name: ");
                name = Console.ReadLine();
            }

            while (name.Length < 3 || name.Length > 12)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid name. Names must be between 3 and 12 characters inclusive.");
                Console.ResetColor();
                Console.Write("Name: ");
                name = Console.ReadLine();
            }

            // prompt for hero class
            Console.WriteLine("Hero Class:\n1: Wizard\n2: Witch Doctor\n3: Demon Hunter\n4: Monk\n5: Barbarian");

            // read input
            string heroClassString = Console.ReadLine();
            int heroClass;

            // check that input is a valid class id
            while (!int.TryParse(heroClassString, out heroClass) || heroClass > 5 || heroClass < 1)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid class. Please choose a number.");
                Console.ResetColor();
                Console.WriteLine("Hero Class:\n1: Wizard\n2: Witch Doctor\n3: Demon Hunter\n4: Monk\n5: Barbarian");
                heroClassString = Console.ReadLine();
            }
            
            // prompt for gender
            Console.Write("Gender (male/female): ");

            int gender = 0;

            // check that gender is valid (1 or 2)
            while (gender == 0)
            {
                switch (Console.ReadLine().ToLower().Trim())
                {
                    case "male":
                    case "m":
                        gender = 1;
                        break;
                    case "female":
                    case "f":
                        gender = 2;
                        break;
                    default:
                        // red for errors
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Invalid gender. Please choose male or female.");
                        // reset colour
                        Console.ResetColor();
                        Console.Write("Gender (male/female): ");
                        break;
                }
            }

            // prompt for hero level
            Console.Write("Level: ");

            // read input
            string levelString = Console.ReadLine();
            int level;

            // check that input is numeric
            while (!int.TryParse(levelString, out level))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid level. Please choose a number.");
                Console.ResetColor();
                Console.Write("Level: ");
                levelString = Console.ReadLine();
            }

            // prompt for hero experience
            Console.Write("Experience: ");

            // read input
            string experienceString = Console.ReadLine();
            int experience;

            // check that input is numeric
            while (!int.TryParse(experienceString, out experience))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid experience. Please choose a number.");
                Console.ResetColor();
                Console.Write("Experience: ");
                experienceString = Console.ReadLine();
            }

            // initialize hero object
            Hero hero = new Hero(currentAccount.Id, name, heroClass, gender, experience, level);

            // create hero
            if (hero.Create())
            {
                Console.Write("Hero ");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(name);
                Console.ResetColor();
                Console.WriteLine(" created.");
            }
            else // hero already exists
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Hero with that name already exists");
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Handles the "create banner" command which creates a banner for the currently logged in account.
        /// </summary>
        static void CommandCreateBanner()
        {
            // check that user is logged in
            if (currentAccount == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Not logged in");
                Console.ResetColor();
                return; // exit
            }

            // prompt for background colour
            Console.Write("Background Colour: ");

            // read input
            string backgroundColorString = Console.ReadLine();
            int backgroundColor;

            // check that background colour is numeric
            while (int.TryParse(backgroundColorString, out backgroundColor))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid background color. Please choose a number.");
                Console.ResetColor();
                Console.Write("Background Colour: ");
                backgroundColorString = Console.ReadLine();
            }

            // prompt for banner
            Console.Write("Banner: ");
            
            // read input
            string bannerString = Console.ReadLine();
            int banner;

            // check that banner is numeric
            while (int.TryParse(bannerString, out banner))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid banner. Please choose a number.");
                Console.ResetColor();
                Console.Write("Banner: ");
                bannerString = Console.ReadLine();
            }

            // prompt for pattern
            Console.Write("Pattern: ");

            // read input
            string patternString = Console.ReadLine();
            int pattern;

            // check that pattern is numeric
            while (int.TryParse(patternString, out pattern))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid pattern. Please choose a number.");
                Console.ResetColor();
                Console.Write("Pattern: ");
                patternString = Console.ReadLine();
            }

            // prompt for pattern colour
            Console.Write("Pattern Colour: ");

            // read input
            string patternColorString = Console.ReadLine();
            int patternColor;

            // check that pattern colour is numeric
            while (int.TryParse(patternColorString, out patternColor))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid pattern colour. Please choose a number.");
                Console.ResetColor();
                Console.Write("Pattern Colour: ");
                patternColorString = Console.ReadLine();
            }

            // prompt for placement
            Console.Write("Placement: ");

            // read input
            string placementString = Console.ReadLine();
            int placement;

            // check that placement is numeric
            while (int.TryParse(placementString, out placement))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid placement. Please choose a number.");
                Console.ResetColor();
                Console.Write("Placement: ");
                placementString = Console.ReadLine();
            }

            // prompt for sigil
            Console.Write("Sigil: ");

            // read input
            string sigilMainString = Console.ReadLine();
            int sigilMain;

            // check that sigil is numeric
            while (int.TryParse(sigilMainString, out sigilMain))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid sigil. Please choose a number.");
                Console.ResetColor();
                Console.Write("Sigil: ");
                sigilMainString = Console.ReadLine();
            }

            // prompt for sigil accent
            Console.Write("Sigil Accent: ");

            // read input
            string sigilAccentString = Console.ReadLine();
            int sigilAccent;

            // check that sigil accent is numeric
            while (int.TryParse(sigilAccentString, out sigilAccent))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid sigil accent. Please choose a number.");
                Console.ResetColor();
                Console.Write("Sigil Accent: ");
                sigilAccentString = Console.ReadLine();
            }

            // prompt for sigil colour
            Console.Write("Sigil Colour: ");

            // read input
            string sigilColorString = Console.ReadLine();
            int sigilColor;

            // check that sigil colour is numeric
            while (int.TryParse(sigilColorString, out sigilColor))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid sigil colour. Please choose a number.");
                Console.ResetColor();
                Console.Write("Sigil Colour: ");
                sigilColorString = Console.ReadLine();
            }

            // prompt for sigil variant
            Console.Write("Use Sigil Variant (yes/no): ");

            // read input
            string useSigilVariantString = Console.ReadLine();
            bool useSigilVariant = false;
            bool useSigilVariantSet = false;

            while (!useSigilVariantSet)
            {
                switch (useSigilVariantString.ToLower())
                {
                    case "yes":
                    case "y":
                        useSigilVariant = true;
                        useSigilVariantSet = true;
                        break;
                    case "no":
                    case "n":
                        useSigilVariant = false;
                        useSigilVariantSet = true;
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Invalid sigil colour. Please choose a number.");
                        Console.ResetColor();
                        Console.Write("Use Sigil Variant (yes/no): ");
                        useSigilVariantString = Console.ReadLine();
                        break;
                }
            }

            // initialize account banner
            AccountBanner accountBanner = new AccountBanner(currentAccount.Id, backgroundColor, banner, pattern, patternColor, placement, sigilAccent, sigilMain, sigilColor, useSigilVariant);

            // save account banner to database
            if (accountBanner.Create())
            {
                Console.WriteLine("Banner created");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Failed to create banner.");
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Handles the "list banners" command which lists all the banners associated with the currently logged in account.
        /// </summary>
        static void CommandListBanners()
        {
            // check that user is logged in
            if (currentAccount == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Not logged in.");
                Console.ResetColor();
                return; // exit
            }

            // load banners
            List<AccountBanner> banners = currentAccount.GetBanners();

            // check that one or more banners exist for this account
            if (banners.Count > 0)
            {
                // iterate through collection
                foreach (AccountBanner banner in banners)
                {
                    Console.WriteLine(banner);
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("No banners associated with this account.");
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Handles the "hero level up" command which increments a hero's level.
        /// </summary>
        static void CommandHeroLevelUp()
        {
            // check that user is logged in
            if (currentAccount == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Not logged in");
                Console.ResetColor();
                return; // exit
            }

            // prompt for hero id
            Console.Write("Hero id: ");

            // read input
            string heroIdString = Console.ReadLine();
            int heroId;

            // check input validity
            if (!int.TryParse(heroIdString, out heroId) || heroId == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid hero id.");
                Console.ResetColor();
                return; // exit
            }

            // initialize hero object
            Hero hero;

            // check if hero exists
            if (!Hero.Load(heroId, out hero))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("No hero with the specified id exists.");
                Console.ResetColor();
                return; // exit
            }

            // check if hero account_id matches the logged in account
            if (hero.AccountId != currentAccount.Id)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Specified hero is not associated with this account.");
                Console.ResetColor();
                return; // exit
            }

            // increment hero level
            hero.Level++;

            // save hero to database
            hero.Save();

            // output result
            Console.Write("Hero ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(hero.Name);
            Console.ResetColor();
            Console.Write(" is now level ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(hero.Level);
            Console.ResetColor();
            Console.WriteLine(".");
        }
    }
}
