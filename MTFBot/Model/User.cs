using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTFBot.DB;

namespace MTFBot.Model
{
    internal class User
    {
        public long DiscordId { get; set; }
        public long SteamId { get; set; }
        public DateTime LinkDate { get; set; }
        public SqliteHelper.WhitelistState WhitelistState { get; set; }

        public static User Parse(DataTable data)
        {
            var user = new User();
            user.DiscordId = (long)data.Rows[0][0];
            user.SteamId = (long)data.Rows[0][1];
            user.LinkDate = DateTime.Parse((string)data.Rows[0][2]);
            user.WhitelistState = (SqliteHelper.WhitelistState)(long)data.Rows[0][3];
            return user;
        }
    }
}
