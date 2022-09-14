using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace MTFBot.Bot
{
    internal static class Core
    {
        public static async Task Start()
        {
            Global.DiscordClient.Ready += OnReady;
            Global.DiscordClient.SlashCommandExecuted += OnSlashCommand; 

            await Global.DiscordClient.LoginAsync(TokenType.Bot, Global.BotToken);
            await Global.DiscordClient.StartAsync();
        }

        public static async Task Stop()
        {
            await Global.DiscordClient.LogoutAsync();
            await Global.DiscordClient.StopAsync();
        }

        private static async Task OnReady()
        {
            await DiscordHandler.Initialize();
        }

        private static async Task OnSlashCommand(SocketSlashCommand arg)
        {
            await DiscordHandler.ExecuteCommand(arg.CommandName, arg.Data);
        }
    }
}
