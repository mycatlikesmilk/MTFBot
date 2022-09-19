using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace MTFBot.Bot
{
    internal static class SlashCommandParser
    {
        public static SocketSlashCommandDataOption GetSubcommand(this SocketSlashCommand command)
        {
            return command.Data.Options.FirstOrDefault(x => x.Type == ApplicationCommandOptionType.SubCommand);
        }

        public static SocketSlashCommandDataOption GetSubCommand(this SocketSlashCommandDataOption command)
        {
            return command.Options.First(x => x.Type == ApplicationCommandOptionType.SubCommand);
        }

        public static T GetCommandValue<T>(this SocketSlashCommand command, string parameter, T defaultValue)
        {
            try
            {
                return (T)command.Data.Options.FirstOrDefault(x => x.Name == parameter).Value;
            }
            catch
            {
                return (T)defaultValue;
            }
        }

        public static T GetCommandValue<T>(this SocketSlashCommandDataOption command, string parameter, T defaultValue)
        {
            try
            {
                return (T)command.Options.FirstOrDefault(x => x.Name == parameter).Value;
            }
            catch
            {
                return (T)defaultValue;
            }
        }

        public static SocketUser GetTriggeredUser(this SocketSlashCommand command)
        {
            return command.User;
        }
    }
}
