using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using MTFBot.Attributes;
using MTFBot.DB;
using MTFBot.Enums;
using MTFBot.Extensions;

namespace MTFBot.Bot.Commands
{
    internal class WhitelistCommand : BaseCommand
    {
        public override string Name => "whitelist";

        public override async Task RegisterCommand(SocketGuild guild)
        {
            await guild.CreateApplicationCommandAsync(new SlashCommandBuilder()
                .WithName(Name)
                .WithDescription("Управление доступом в whitelist")
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("add")
                    .WithDescription("Добавить игрока в whitelist")
                    .WithType(ApplicationCommandOptionType.SubCommand)
                        .AddOption(new SlashCommandOptionBuilder()
                            .WithName("discord")
                            .WithDescription("Дискорд игрока")
                            .WithType(ApplicationCommandOptionType.User)
                            .WithRequired(true))
                        .AddOption(new SlashCommandOptionBuilder()
                            .WithName("steamid")
                            .WithDescription("Steamid игрока")
                            .WithType(ApplicationCommandOptionType.Integer))
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("force")
                        .WithDescription("Дать доступ не смотря на запрет")
                        .WithType(ApplicationCommandOptionType.Boolean)
                        .WithRequired(false)
                    )
                )
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("remove")
                    .WithDescription("Удаляет игрока из whitelist с возможностью указать запрет последующего добавления")
                    .WithType(ApplicationCommandOptionType.SubCommand)
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("discord")
                        .WithDescription("Дискорд игрока")
                        .WithType(ApplicationCommandOptionType.User)
                        .WithRequired(true))
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("reason")
                        .WithDescription("Причина выписывания")
                        .WithType(ApplicationCommandOptionType.String)
                        .WithRequired(true))
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("restrict")
                        .WithDescription("Запомнить, что игроку запрещен доступ в whitelist")
                        .WithType(ApplicationCommandOptionType.Boolean)
                        .WithRequired(false)))
                .Build());
        }

        public override async Task Execute(SocketSlashCommand command, SocketGuild guild)
        {
            var triggeredUser = command.GetTriggeredUser();

            if (!CheckRoles(guild.GetUser(triggeredUser.Id)))
                await command.RespondAsync(text: "У вас недостаточно прав для выполнения этой команды");

            var subcommand = command.GetSubcommand();

            var discordUser = subcommand.GetCommandValue<SocketUser>("discord", null);
            var steamId = (ulong)subcommand.GetCommandValue<long>("steamid", 0);
            var forced = subcommand.GetCommandValue<bool>("force", false);
            var restriction = subcommand.GetCommandValue<bool>("restrict", true);

            var dbUser = Database.Context.Users.FirstOrDefault(x => x.DiscordId == discordUser.Id);
            if (dbUser == null)
            {
                if (steamId == 0)
                {
                    await command.RespondAsync(text: $"Пользователь <@{discordUser.Id}> не найден в базе данных. Укажите SteamID явно, чтобы добавить в базу данных и выдать доступ в Whitelist");
                    return;
                }
                dbUser = Database.Context.Users.Add(new User(discordUser.Id, steamId)).Entity;
            }

            await Database.Context.SaveChangesAsync();

            switch (subcommand.Name)
            {
                case "add":
                    if (dbUser.WhitelistState == WhitelistState.WhitelistAllowed)
                        await command.RespondAsync(text: $"Игрок <@{discordUser.Id}> уже добавлен в Whitelist");

                    if (dbUser.WhitelistState == WhitelistState.WhitelistRestricted && !forced)
                        await command.RespondAsync(
                            $"Игроку <@{discordUser.Id}> запрещено находиться в Whitelist. Используйте force - true, чтобы добавить его, обходя это ограничение");

                    await DiscordHandler.GrantRole(discordUser, Global.Roles.WhitelistAllowed);
                    dbUser.WhitelistState = WhitelistState.WhitelistAllowed;
                    await Database.Context.SaveChangesAsync();

                    await command.RespondAsync(text: $"Пользователь <@{discordUser.Id}> добавлен в whitelist");
                    break;
                case "remove":
                    if (restriction)
                    {
                        await DiscordHandler.RemoveRole(discordUser.Id, Global.Roles.WhitelistAllowed);
                        await DiscordHandler.GrantRole(discordUser, Global.Roles.WhitelistResticted);
                        dbUser.WhitelistState = WhitelistState.WhitelistRestricted;
                        await Database.Context.SaveChangesAsync();
                    }
                    else
                    {
                        await DiscordHandler.RemoveRole(discordUser.Id, Global.Roles.WhitelistAllowed);
                        dbUser.WhitelistState = WhitelistState.NoWhitelist;
                        await Database.Context.SaveChangesAsync();
                    }

                    await command.RespondAsync(text: $"Пользователь <@{discordUser.Id}> выписан из whitelist");
                    break;
            }
        }

        private bool CheckRoles(SocketGuildUser user)
        {
            return (user.HasRole(Global.Roles.Administration) ||
                      user.HasRole(Global.Roles.Moderation) ||
                      user.HasRole(Global.Roles.WhitelistAllower));
        }
    }
}
