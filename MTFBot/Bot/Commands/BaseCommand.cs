using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace MTFBot.Bot.Commands
{
    internal abstract class BaseCommand
    {
        public abstract string Name { get; }

        public abstract Task RegisterCommand(SocketGuild guild);

        public abstract Task Execute(SocketSlashCommand command, SocketGuild guild);

        protected void StartExecute(SocketSlashCommand command)
        {
            Log.WriteLine($"Начало выполнения команды {this.GetType().Name}");
        }

        protected void EndExecute(SocketSlashCommand command)
        {
            Log.WriteLine($"Команда {this.GetType().Name} выполнена успешно", Log.LogLevel.SUCCES);
        }
    }
}
