using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using MTFBot.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MTFBot
{
    internal static class Global
    {
        public static DiscordSocketClient DiscordClient;
        public static string BotToken;

        public static CancellationTokenSource TimerCancellationToken { get; set; }

        public static JToken Configuration { get; set; }

        public static void Setup()
        {
            try
            {
                Log.WriteLine("Чтение файла конфигурации");
                Configuration = JsonConvert.DeserializeObject<JToken>(File.ReadAllText(
#if DEBUG
                    "Config/debug.json"
#else
                (File.Exists("Config/release.json")
                    ? File.ReadAllText("Config/release.json")
                    : File.ReadAllText("Config/config.json"))
#endif
                ));

                Log.WriteLine("Чтение токена бота");
                BotToken = Configuration["Server"]["BotToken"].Value<string>();
            }
            catch (Exception e)
            {
                Log.WriteLine("Невозможно прочитать данные из файла config.json в папке Config", Log.LogLevel.FATAL);
                Environment.Exit(0);
            }
        }

        public enum Roles
        {
            [RoleID("Administration")]
            Administration,
            [RoleID("Moderation")]
            Moderation,
            [RoleID("WhitelistAllower")]
            WhitelistAllower,
            [RoleID("WhitelistAllowed")]
            WhitelistAllowed,
            [RoleID("WhitelistRestricted")]
            WhitelistResticted,
            [RoleID("Muted")]
            Muted
        }
    }
}
