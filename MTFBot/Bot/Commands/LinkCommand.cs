using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using MTFBot.DB;
using MTFBot.Extensions;

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
                    .WithDescription("Привязать SteamID к дискорд-аккаунту")
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("steamid")
                        .WithDescription("Steamid игрока")
                        .WithType(ApplicationCommandOptionType.Integer)
                        .WithRequired(true))
                    .Build());
        }

        public override async Task Execute(SocketSlashCommand command, SocketGuild guild)
        {
            var triggeredUser = command.GetTriggeredUser();
            var steamid = (ulong)command.GetCommandValue<long>("steamid", 0);

            var dbUser = Database.Context.Users.FirstOrDefault(x => x.DiscordId == triggeredUser.Id);

            if (dbUser != null)
            {
                await command.RespondAsync($"SteamID уже привязан к этому аккаунту. Для перепривязки SteamID обратитесь к администрации");
                return;
            }

            Database.Context.Users.Add(new User(triggeredUser.Id, steamid));
            await Database.Context.SaveChangesAsync();

            await command.RespondAsync($"Пользователь <@{triggeredUser.Id}> успешно привязал свой steamid ({steamid})");
        }
    }
}
