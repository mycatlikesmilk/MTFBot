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
            Log.WriteLine("Запуск бота");
            Global.DiscordClient = new DiscordSocketClient();

            Global.DiscordClient.Ready += OnReady;
            Global.DiscordClient.SlashCommandExecuted += OnSlashCommand; 

            await Global.DiscordClient.LoginAsync(TokenType.Bot, Global.BotToken);
            await Global.DiscordClient.StartAsync();
            await Global.DiscordClient.SetStatusAsync(UserStatus.Online);
            Log.WriteLine("Бот запущен");
        }

        public static async Task Stop()
        {
            Log.WriteLine("Остановка работы бота");
            if (Global.DiscordClient != null)
            {
                await Global.DiscordClient.SetStatusAsync(UserStatus.Offline);
                await Global.DiscordClient.LogoutAsync();
                await Global.DiscordClient.StopAsync();
            }
            Log.WriteLine("Бот остановлен");
        }

        private static async Task OnReady()
        {
            await DiscordHandler.Initialize();
        }

        private static async Task OnSlashCommand(SocketSlashCommand arg)
        {
            await DiscordHandler.ExecuteCommand(arg.CommandName, arg);
        }
    }
}
