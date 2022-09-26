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
    internal class UserCommand : BaseCommand
    {
        public override string Name => "user";

        public override Global.Roles[] Permissions => Array.Empty<Global.Roles>();

        public override async Task RegisterCommand(SocketGuild guild)
        {
            await guild.CreateApplicationCommandAsync(new SlashCommandBuilder()
                .WithName(Name)
                .WithDescription("Получить информацию об игроке")
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("discord")
                    .WithDescription("Получить информацию об игроке, используя аккаунт дискорда")
                    .WithType(ApplicationCommandOptionType.SubCommand)
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("discord")
                        .WithDescription("Пользователь дискорда")
                        .WithType(ApplicationCommandOptionType.User)
                        .WithRequired(true)))
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("steamid")
                    .WithDescription("Получить информацию об игроке, используя SteamID")
                    .WithType(ApplicationCommandOptionType.SubCommand)
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("steamid")
                        .WithDescription("SteamID игрока")
                        .WithType(ApplicationCommandOptionType.Integer)
                        .WithRequired(true)))
                .Build());
        }

        public override async Task Execute(SocketSlashCommand command, SocketGuild guild)
        {
            var subcommand = command.GetSubcommand();
            var user = subcommand.GetCommandValue<SocketUser>("discord", null);
            var steamid = (ulong)subcommand.GetCommandValue<long>("steamid", 0);

            switch (subcommand.Name)
            {
                case "discord":
                    {
                        var dbuser = Database.Context.Users.FirstOrDefault(x => x.DiscordId == user.Id.ToString());
                        if (dbuser == null)
                        {
                            await command.RespondAsync(text: "Пользователь не найден в базе данных");
                            return;
                        }

                        await command.RespondAsync(embed: new EmbedBuilder()
                            .WithTitle($"Информация об игроке")
                            .WithFields(new EmbedFieldBuilder[]
                            {
                                new EmbedFieldBuilder()
                                    .WithName("Discord")
                                    .WithValue($"<@{dbuser.DiscordId}>"),
                                new EmbedFieldBuilder()
                                    .WithName("SteamID")
                                    .WithValue(dbuser.SteamId),
                                new EmbedFieldBuilder()
                                    .WithName("Дата привязывания")
                                    .WithValue(dbuser.LinkDate),
                                new EmbedFieldBuilder()
                                    .WithName("Состояние Whitelist")
                                    .WithValue(dbuser.WhitelistState.ToString())
                            })
                            .Build());
                    }
                    break;
                case "steamid":
                    {
                        var dbuser = Database.Context.Users.FirstOrDefault(x => x.SteamId == steamid.ToString());
                        if (dbuser == null)
                        {
                            await command.RespondAsync(text: "Пользователь не найден в базе данных");
                            return;
                        }

                        await command.RespondAsync(embed: new EmbedBuilder()
                            .WithTitle($"Информация об игроке <@{dbuser.DiscordId}>")
                            .WithFields(new EmbedFieldBuilder[]
                            {
                                new EmbedFieldBuilder()
                                    .WithName("SteamID")
                                    .WithValue(dbuser.SteamId),
                                new EmbedFieldBuilder()
                                    .WithName("Дата привязывания")
                                    .WithValue(dbuser.LinkDate),
                                new EmbedFieldBuilder()
                                    .WithName("Состояние Whitelist")
                                    .WithValue(dbuser.WhitelistState.ToString())
                            })
                            .Build());
                    }
                    break;
            }
        }
    }
}
