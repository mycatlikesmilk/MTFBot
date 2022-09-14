using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MTFBot
{
    internal static class Global
    {
        public static DiscordSocketClient DiscordClient;
        public static string BotToken;

        static Global()
        {
            DiscordClient = new DiscordSocketClient();

            try
            {
                var data = JsonConvert.DeserializeObject<JToken>(File.ReadAllText(
#if DEBUG
                "Config/debug.json"
#else
                (File.Exists("Config/release.json")
                    ? File.ReadAllText("Config/release.json")
                    : File.ReadAllText("Config/config.json"))
#endif
            ));
                BotToken = data["Server"]["BotToken"].Value<string>();
            }
            catch (Exception e)
            {
                Console.WriteLine("Невозможно прочитать данные из файла config.json в папке Config");
                Environment.Exit(0);
            }
        }
    }
}
