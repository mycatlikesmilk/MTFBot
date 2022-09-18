using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace MTFBot.Bot.Commands
{
    internal class RelinkCommand : BaseCommand
    {
        public override string Name => "relink";

        public override async Task RegisterCommand(SocketGuild guild)
        {
            await guild.CreateApplicationCommandAsync(new SlashCommandBuilder()
                .WithName(Name)
                .WithDescription("Перепривязывает steamid к определенному пользователю дискорда")
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("user")
                    .WithDescription("Пользователь в дискорде")
                    .WithType(ApplicationCommandOptionType.User)
                    .WithRequired(true)
                )
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("steamid")
                    .WithDescription("Steamid игрока")
                    .WithType(ApplicationCommandOptionType.String)
                    .WithRequired(true)
                ).Build());
        }

        public override async Task Execute(SocketSlashCommand command, SocketGuild guild)
        {
            var triggeredUser = command.User;
            var user = (SocketUser)command.Data.Options.First(x => x.Name == "user").Value;
            var steamid = (string)command.Data.Options.First(x => x.Name == "steamid").Value;

            // TODO: relink in db
        }
    }
}
