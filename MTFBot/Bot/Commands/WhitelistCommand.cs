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
using MTFBot.Extensions;
using MTFBot.Model;

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
                            .WithType(ApplicationCommandOptionType.String)
                            .WithRequired(true))
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
            var cmdType = command.Data.Options.First().Name;

            if (!CheckRoles(guild.GetUser(command.User.Id)))
                await command.RespondAsync(text: "У вас недостаточно прав для выполнения этой команды");

            var user = (SocketUser)command.Data.Options.First().Options.First(x => x.Name == "discord").Value;

            switch (cmdType)
            {
                case "add":
                    var dbUser = SqliteHelper.GetUserInfo(user.Id.ToString());
                    var forced = (SocketSlashCommandDataOption)command.Data.Options.First().Options.FirstOrDefault(x => x.Name == "force");

                    if (dbUser.WhitelistState == SqliteHelper.WhitelistState.WhitelistResticted && (forced == null || !((bool)forced.Value)))
                        await command.RespondAsync($"Игроку <@{user.Id}> запрещено находиться в whitelist. Используйте force - true, чтобы добавить его, обходя это ограничение");

                    var steamid = (string)command.Data.Options.First().Options.First(x => x.Name == "steamid").Value;
                    await DiscordHandler.GrantRole(user, Global.Roles.WhitelistAllowed);
                    SqliteHelper.UpdateWhitelistState(user.Id.ToString(), SqliteHelper.WhitelistState.Whitelist, steamid);
                    await command.RespondAsync($"Пользователь <@{user.Id}> добавлен в whitelist");
                    break;
                case "remove":
                    var restriction = (SocketSlashCommandDataOption)command.Data.Options.First().Options.FirstOrDefault(x => x.Name == "restrict");

                    if (restriction == null || ((bool)restriction.Value))
                    {
                        await DiscordHandler.RemoveRole(user, Global.Roles.WhitelistAllowed);
                        await DiscordHandler.GrantRole(user, Global.Roles.WhitelistResticted);
                        SqliteHelper.UpdateWhitelistState(user.Id.ToString(), SqliteHelper.WhitelistState.WhitelistResticted);
                    }
                    else
                    {
                        await DiscordHandler.RemoveRole(user, Global.Roles.WhitelistAllowed);
                        SqliteHelper.UpdateWhitelistState(user.Id.ToString(), SqliteHelper.WhitelistState.NoWhitelist);
                    }
                    await command.RespondAsync($"Пользователь <@{user.Id}> выписан из whitelist");
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
