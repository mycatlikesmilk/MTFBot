using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using MTFBot.DB;

namespace MTFBot.Bot.Commands
{
    internal class LinkCommand : BaseCommand
    {
        public override string Name => "link";

        public override async Task RegisterCommand(SocketGuild guild)
        {
            await guild.CreateApplicationCommandAsync(
                new SlashCommandBuilder()
                    .WithName(Name)
                    .WithDescription("Связать игрока с дискорд-аккаунтом")
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("steamid")
                        .WithDescription("Steamid игрока")
                        .WithType(ApplicationCommandOptionType.String)
                        .WithRequired(true))
                    .Build());
        }

        public override async Task Execute(SocketSlashCommand command, SocketGuild guild)
        {
            var triggeredUser = command.User;
            var steamid = (string)command.Data.Options.First().Value;

            

            await command.RespondAsync($"Пользователь <@{triggeredUser.Id}> успешно привязал свой steamid ({steamid})");
        }
    }
}
