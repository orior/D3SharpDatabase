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
        public int SigilAccent { get; private set; }
        public int SigilMain { get; private set; }
        public string SigilColor { get; private set; }
        public bool UseSigilVariant { get; private set; }

        public AccountBanner(int Id, string BackgroundColor, string Banner, int Pattern, string PatternColor, int Placement, int SigilAccent,int SigilMain, string SigilColor, bool UsesigilVariant)
        {
            this.Id = Id;
            this.BackgroundColor = BackgroundColor;
            this.Banner = Banner;
            this.Pattern = Pattern;
            this.Placement = Placement;
            this.SigilAccent = SigilAccent;
            this.SigilColor = SigilColor;
            this.SigilMain = SigilMain;
            this.UseSigilVariant = UseSigilVariant;

        }

        public bool Save()
        {
            try
            {
                SQLiteCommand command = new SQLiteCommand(string.Format("UPDATE account_banner SET background_color='{1}', banner='{2}', pattern='{3}', pattern_color='{4}', placement='{5}', sigil_accent='{6}', sigil_main='{7}', sigil_color='{8}', use_sigil_variant='{9}' WHERE account_id='{0}'", Id,BackgroundColor,Banner,Pattern,PatternColor,Placement,SigilAccent,SigilMain,SigilColor,UseSigilVariant), Database.Instance.Connection);
                command.ExecuteNonQuery();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("AccountBanner: Failed to save account banner! Exception: {0}", e.Message);
                return false;
            }
        }

        public bool Create()
        {
            SQLiteCommand command = new SQLiteCommand(string.Format("INSERT INTO account_banner (account_id, background_color, banner, pattern, pattern_color, placement, sigil_accent, sigil_main, sigil_color, use_sigil_variant) VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}')", Id, BackgroundColor, Banner, Pattern, PatternColor, Placement, SigilAccent, SigilMain, SigilColor, UseSigilVariant), Database.Instance.Connection);
            int affectedRows = command.ExecuteNonQuery();
            if (affectedRows == 0)
                return false;
            //Id = Database.Instance.GetLastInsertId();
            return true;
        }


        public static bool Load(int id, out AccountBanner accountbanner)
        {
            accountbanner = null;
            try
            {
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
            }

             catch (Exception e)
            {
                Console.WriteLine("AccountBanner: Failed to load account banner! Exception: {0}", e.Message);
                return false;
            }
            return false;
        }

        

        public override string ToString()
        {
            return String.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}", Id, BackgroundColor,Banner,Pattern,PatternColor,Placement,SigilAccent,SigilMain,SigilColor,UseSigilVariant);
        }

        
    }
}
