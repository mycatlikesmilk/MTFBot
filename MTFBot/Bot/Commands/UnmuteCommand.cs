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
    internal class UnmuteCommand : BaseCommand
    {
        public override string Name => "unmute";

        public override Global.Roles[] Permissions => new[] { Global.Roles.Administration, Global.Roles.Moderation };

        public override async Task RegisterCommand(SocketGuild guild)
        {
            await guild.CreateApplicationCommandAsync(new SlashCommandBuilder()
                .WithName(Name)
                .WithDescription("Снять мьют с пользователя")
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("user")
                    .WithDescription("Пользователь в дискорде")
                    .WithType(ApplicationCommandOptionType.User)
                    .WithRequired(true))
                .Build());
        }

        public override async Task Execute(SocketSlashCommand command, SocketGuild guild)
        {
            if (!base.HasAccess(command, guild)) return;

            var user = command.GetCommandValue<SocketUser>("user", null);
            var dbuser = Database.Context.DiscordMutes.FirstOrDefault(x => x.DiscordId == user.Id);

            if (dbuser == null)
            {
                await DiscordHandler.RemoveRole(user.Id, Global.Roles.Muted);
                await command.RespondAsync(text: $"Пользователь <@{user.Id}> не замьючен");
                return;
            }

            Database.Context.DiscordMutes.Remove(dbuser);
            await Database.Context.SaveChangesAsync();

            await DiscordHandler.RemoveRole(user.Id, Global.Roles.Muted);
            await command.RespondAsync(text: $"Мьют с пользователя <@{user.Id}> снят");
        }

        private bool CheckRoles(SocketGuildUser user)
        {
            return (user.HasRole(Global.Roles.Administration) ||
                    user.HasRole(Global.Roles.Moderation));
        }
    }
}
