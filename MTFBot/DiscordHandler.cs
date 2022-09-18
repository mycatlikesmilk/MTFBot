using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using MTFBot.Attributes;
using MTFBot.Bot.Commands;

namespace MTFBot
{
    internal static class DiscordHandler
    {
        private static SocketGuild Guild { get; set; }

        private static Dictionary<string, BaseCommand> Commands { get; set; }

        public static async Task Initialize()
        {
            Log.WriteLine("Инициализация данных о сервере");
            Commands = new Dictionary<string, BaseCommand>();
            Guild = Global.DiscordClient.Guilds.First();
            await RegisterCommands();
            Log.WriteLine("Инициализация данных о сервере завершена", Log.LogLevel.SUCCES);
        }

        public static async Task ExecuteCommand(string commandName, SocketSlashCommand command)
        {
            try
            {
                Log.WriteLine($"Запуск команды {commandName}");
                await Commands[commandName].Execute(command, Guild);
                Log.WriteLine($"Команда {commandName} выполнена успешно", Log.LogLevel.SUCCES);
            }
            catch (Exception ex)
            {
                Log.WriteLine($"Выполнение команды {command} остановлено", Log.LogLevel.FAIL);
                await command.RespondAsync(embed: new EmbedBuilder()
                    .WithTitle(ex.Message)
                    .WithFields(new EmbedFieldBuilder[]
                    {
                        new EmbedFieldBuilder()
                            .WithName("StackTrace")
                            .WithValue($"```\n{ex.StackTrace}\n```")
                    })
                    .WithTimestamp(DateTimeOffset.Now)
                    .Build());
            }
        }

        private static async Task RegisterCommands()
        {
            Log.WriteLine("Регистрация команд сервера");
            await Guild.DeleteApplicationCommandsAsync();

            var commandTypes = Assembly.GetAssembly(typeof(BaseCommand)).GetTypes()
                .Where(x => x.IsClass && !x.IsAbstract && x.IsAssignableTo(typeof(BaseCommand))).ToList();

            foreach (var cmd in commandTypes)
            {
                Log.WriteLine($"Регистрация команды {cmd.Name}");
                var instance = (BaseCommand)Activator.CreateInstance(cmd);
                await instance.RegisterCommand(Guild);
                Commands.Add(instance.Name, instance);
                Log.WriteLine($"Команда {cmd.Name} зарегистрирована", Log.LogLevel.SUCCES);
            }
        }

        public static async Task GrantRole(SocketUser user, Global.Roles role)
        {
            try
            {
                await Guild.GetUser(user.Id).AddRoleAsync(RoleIDAttribute.GetRoleId(role));
            }
            catch (Exception e)
            {

            }
        }

        public static async Task RemoveRole(SocketUser user, Global.Roles role)
        {
            await Guild.GetUser(user.Id).RemoveRoleAsync(RoleIDAttribute.GetRoleId(role));
        }
    }
}
