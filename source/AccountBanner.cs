using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;

namespace D3Database
{
    public class AccountBanner
    {
        public int Id {get; private set;}
        public int AccountId { get; private set; }
        public int BackgroundColor { get; private set; }
        public int Banner { get; private set; }
        public int Pattern { get; private set; }
        public int PatternColor { get; private set; }
        public int Placement { get; private set; }
        public int SigilAccent { get; private set; }
        public int SigilMain { get; private set; }
        public int SigilColor { get; private set; }
        public bool UseSigilVariant { get; private set; }

        public AccountBanner(int AccountId, int BackgroundColor, int Banner, int Pattern, int PatternColor, int Placement, int SigilAccent, int SigilMain, int SigilColor, bool UsesigilVariant)
        {
            this.Id = -1;
            this.AccountId = AccountId;
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
                SQLiteCommand command = new SQLiteCommand(string.Format("UPDATE account_banner SET background_color='{1}', banner='{2}', pattern='{3}', pattern_color='{4}', placement='{5}', sigil_accent='{6}', sigil_main='{7}', sigil_color='{8}', use_sigil_variant='{9}' WHERE account_id='{0}'", AccountId,BackgroundColor,Banner,Pattern,PatternColor,Placement,SigilAccent,SigilMain,SigilColor,UseSigilVariant), Database.Instance.Connection);
                command.ExecuteNonQuery();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to save Account Banner exception: {0}", e.Message);
                return false;
            }
        }

        public bool Create()
        {
            if (Id != -1)
                return false;
            if (!Account.CheckIfAccountExists(AccountId))
                return false;
            SQLiteCommand command = new SQLiteCommand(string.Format("INSERT INTO account_banner (account_id, background_color, banner, pattern, pattern_color, placement, sigil_accent, sigil_main, sigil_color, use_sigil_variant) VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}')", AccountId, BackgroundColor, Banner, Pattern, PatternColor, Placement, SigilAccent, SigilMain, SigilColor, UseSigilVariant ? 1 : 0), Database.Instance.Connection);
            int affectedRows = command.ExecuteNonQuery();
            if (affectedRows == 0)
                return false;
            Id = Database.Instance.GetLastInsertId();
            return true;
        }


        public static bool Load(int id, out AccountBanner accountbanner)
        {
            accountbanner = null;
            SQLiteCommand command = new SQLiteCommand(string.Format("SELECT account_banner_id, account_id, background_color, banner, pattern, pattern_color, placement, sigil_accent, sigil_main, sigil_color, use_sigil_variant FROM account_banner WHERE account_banner_id='{0}'", id), Database.Instance.Connection);
            SQLiteDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    var account_banner_id = reader.GetInt32(0);
                    var account_id = reader.GetInt32(1);
                    var background_color = reader.GetInt32(2);
                    var banner = reader.GetInt32(3);
                    var pattern = reader.GetInt32(4);
                    var pattern_color = reader.GetInt32(5);
                    var placement = reader.GetInt32(6);
                    var sigil_accent = reader.GetInt32(7);
                    var sigil_main = reader.GetInt32(8);
                    var sigil_color = reader.GetInt32(9);
                    var use_sigil_variant = reader.GetBoolean(10);
                    accountbanner = new AccountBanner(account_id, background_color, banner, pattern, pattern_color, placement, sigil_accent, sigil_main, sigil_color, use_sigil_variant);
                    accountbanner.Id = account_banner_id;
                    return true;
                }
            }
            return false;
        }

        

        public override string ToString()
        {
            return String.Format("BannerId: {0}, AccId: {1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}", Id, AccountId, BackgroundColor,Banner,Pattern,PatternColor,Placement,SigilAccent,SigilMain,SigilColor,UseSigilVariant);
        }

        
    }
}
