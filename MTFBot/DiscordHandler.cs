using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using MTFBot.Bot.Commands;

namespace MTFBot
{
    internal static class DiscordHandler
    {
        private static SocketGuild Guild { get; set; }

        private static Dictionary<string, BaseCommand> Commands { get; set; }

        public static async Task Initialize()
        {
            Guild = Global.DiscordClient.Guilds.First();
            await RegisterCommands();
        }

        public static async Task ExecuteCommand(string command, SocketSlashCommandData data)
        {
            await Commands[command].Execute(data);
        }

        private static async Task RegisterCommands()
        {
            var commandTypes = Assembly.GetAssembly(typeof(BaseCommand)).GetTypes()
                .Where(x => x.IsClass && !x.IsAbstract && x.IsAssignableFrom(typeof(BaseCommand))).ToList();

            foreach (var cmd in commandTypes)
            {
                var instance = (BaseCommand)Activator.CreateInstance(cmd);
                await instance.RegisterCommand(Guild);
                Commands.Add(instance.Name, instance);
            }
        }
    }
}
