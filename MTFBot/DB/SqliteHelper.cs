using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using MTFBot.Exceptions;
using MTFBot.Model;

namespace MTFBot.DB
{
    internal static class SqliteHelper
    {
        private static SqliteConnection connection;

        public static void Open()
        {
            var builder = new SqliteConnectionStringBuilder()
            {
                DataSource = "DB.db"
            };
            connection = new SqliteConnection(builder.ToString());
            connection.Open();
            CreateTables();
        }

        public static void Close()
        {
            connection.Close();
            connection = null;
        }

        public static DataTable Execute(string sqlCommand)
        {
            var cmd = new SqliteCommand(sqlCommand, connection);

            DataTable table = new DataTable();
            table.Load(cmd.ExecuteReader());

            return table;
        }

        // single-use scripts
        public static void CreateTables()
        {
            var userTable = Execute("SELECT name FROM sqlite_master WHERE type='table' and name = 'Users'");
            if (userTable.Rows.Count == 0)
                CreateUserTable();
        }

        public static void CreateUserTable()
        {
            Execute(@"
CREATE TABLE Users(
    discord_id integer not null primary key unique,
    steam_id integer not null,
    link_date text,
    whitelist_state integer
)");

            // TODO: add restriction reason
        }

        public static void AddUser(string discordId, string steamId)
        {
            CheckIfUserInDB(discordId);

            Execute($"INSERT INTO Users(discord_id, steam_id, link_date, whitelist_state, whitelist_restrict_reason) VALUES ({discordId}, {steamId}, '{DateTime.Now:yyyy.MM.dd}', {0})");
        }

        public static void UpdateUserLink(string discordId, string steamId)
        {
            CheckIfUserInDB(discordId);

            Execute($"UPDATE Users SET steam_id = {steamId} WHERE discord_id = {discordId}");
        }

        public static void UpdateWhitelistState(string discordId, WhitelistState state, string steamid = null)
        {
            var user = Execute($"SELECT * FROM Users WHERE discord_id = {discordId}");

            if (user.Rows.Count == 0)
                AddUser(discordId, steamid);
            
            Execute($"UPDATE Users SET whitelist_state = {(int)state} WHERE discord_id = {discordId}");
        }

        public static User GetUserInfo(string discordId) => User.Parse(Execute($"SELECT * FROM Users WHERE discord_id = {discordId}"));

        public enum WhitelistState
        {
            NoWhitelist = 0,
            Whitelist = 1,
            WhitelistResticted = 2
        }

        // private

        private static void CheckIfUserInDB(string discordId)
        {
            var user = Execute($"SELECT * FROM Users WHERE discord_id = {discordId}");
            if (user.Rows.Count != 0)
                throw new UserAlreadyInDatabaseException();
        }
    }
}
