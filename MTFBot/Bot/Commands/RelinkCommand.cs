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
    internal class RelinkCommand : BaseCommand
    {
        public override string Name => "relink";

        public override async Task RegisterCommand(SocketGuild guild)
        {
            await guild.CreateApplicationCommandAsync(new SlashCommandBuilder()
                .WithName(Name)
                .WithDescription("Перепривязывает steamid к определенному пользователю дискорда")
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("discord")
                    .WithDescription("Пользователь в дискорде")
                    .WithType(ApplicationCommandOptionType.User)
                    .WithRequired(true)
                )
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("steamid")
                    .WithDescription("Steamid игрока")
                    .WithType(ApplicationCommandOptionType.Integer)
                    .WithRequired(true)
                ).Build());
        }

        public override async Task Execute(SocketSlashCommand command, SocketGuild guild)
        {
            var triggeredUser = command.GetTriggeredUser();
            if (!CheckRoles(guild.GetUser(triggeredUser.Id)))
                await command.RespondAsync(text: "У вас недостаточно прав для выполнения этой команды");

            var user = command.GetCommandValue<SocketUser>("discord", null);
            var steamid = (ulong)command.GetCommandValue<long>("steamid", 0);

            var dbuser = Database.Context.Users.FirstOrDefault(x => x.DiscordId == user.Id);
            if (dbuser == null)
            {
                dbuser = Database.Context.Users.Add(new User(user.Id, steamid)).Entity;
                await Database.Context.SaveChangesAsync();
                await command.RespondAsync(text: $"Пользователь <@{user.Id}> не найден. Связь создана ({steamid})");
                return;
            }

            dbuser.SteamId = steamid;
            await Database.Context.SaveChangesAsync();
            await command.RespondAsync(text: $"Перепривязка успешно выполнена (<@{user.Id}> {steamid})");
        }

        private bool CheckRoles(SocketGuildUser user)
        {
            return (user.HasRole(Global.Roles.Administration) ||
                    user.HasRole(Global.Roles.Moderation) ||
                    user.HasRole(Global.Roles.WhitelistAllower));
        }
    }
}
