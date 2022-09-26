using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using MTFBot.Extensions;

namespace MTFBot.Bot.Commands
{
    internal abstract class BaseCommand
    {
        public abstract string Name { get; }
        public abstract Task RegisterCommand(SocketGuild guild);

        public abstract Task Execute(SocketSlashCommand command, SocketGuild guild);

        public abstract Global.Roles[] Permissions { get; }

        protected const string NoAccessResponse = "У вас недостаточно прав для выполнения этой команды";

        protected bool HasAccess(SocketSlashCommand command, SocketGuild guild)
        {
            var user = guild.GetUser(command.GetTriggeredUser().Id);

            bool accesDenied = true;

            foreach (var role in Permissions)
            {
                if (user.HasRole(role))
                    accesDenied = false;
            }

            if (accesDenied)
                command.RespondAsync(text: NoAccessResponse);

            return !accesDenied;
        }
    }
}
