using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;

namespace D3Database
{
    public class AccountBanner
    {
        public int Id { get; private set; }
        public string BackgroundColor { get; private set; }
        public string Banner { get; private set; }
        public int Pattern { get; private set; }
        public string PatternColor { get; private set; }
        public int Placement { get; private set; }
        public int SignilAccent { get; private set; }
        public int SignilMain { get; private set; }
        public string SignilColor { get; private set; }
        public bool UseSignilVariant { get; private set; }

        public AccountBanner(int Id, string BackgroundColor, string Banner, int Pattern, string PatternColor, int Placement, int SignilAccent,int SignilMain, string SignilColor, bool UsesignilVariant)
        {
            this.Id = Id;
            this.BackgroundColor = BackgroundColor;
            this.Banner = Banner;
            this.Pattern = Pattern;
            this.Placement = Placement;
            this.SignilAccent = SignilAccent;
            this.SignilColor = SignilColor;
            this.SignilMain = SignilMain;
            this.UseSignilVariant = UseSignilVariant;

        }

        public bool Save()
        {
            try
            {
                SQLiteCommand command = new SQLiteCommand(string.Format("UPDATE account_banner SET background_color='{1}', banner='{2}', pattern='{3}', pattern_color='{4}', placement='{5}', signil_accent='{6}', signil_main='{7}', signil_color='{8}', use_signil_variant='{9}' WHERE account_id='{0}'", Id,BackgroundColor,Banner,Pattern,PatternColor,Placement,SignilAccent,SignilMain,SignilColor,UseSignilVariant), Database.Instance.Connection);
                command.ExecuteNonQuery();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to save Account Banner exception: {0}", e.Message);
                return false;
            }
        }

        public bool Create(string password)
        {
            SQLiteCommand command = new SQLiteCommand(string.Format("INSERT INTO account_banner (account_id, background_color, banner, pattern, pattern_color, placement, signil_accent, signil_main, signil_color, use_signil_variant) VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}')", Id, BackgroundColor, Banner, Pattern, PatternColor, Placement, SignilAccent, SignilMain, SignilColor, UseSignilVariant), Database.Instance.Connection);
            int affectedRows = command.ExecuteNonQuery();
            if (affectedRows == 0)
                return false;
            Id = Database.Instance.GetLastInsertId();
            return true;
        }


        public static bool Load(int id, out AccountBanner accountbanner)
        {
            accountbanner = null;
            SQLiteCommand command = new SQLiteCommand(string.Format("SELECT * FROM account_banner WHERE account_id='{0}'", id), Database.Instance.Connection);
            SQLiteDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    accountbanner = new AccountBanner(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetInt32(3), reader.GetString(4), reader.GetInt32(5), reader.GetInt32(6), reader.GetInt32(7), reader.GetString(8), reader.GetBoolean(9));
                    return true;
                }
            }
            return false;
        }

        

        public override string ToString()
        {
            return String.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}", Id, BackgroundColor,Banner,Pattern,PatternColor,Placement,SignilAccent,SignilMain,SignilColor,UseSignilVariant);
        }

        
    }
}
