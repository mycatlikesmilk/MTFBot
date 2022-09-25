using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using MTFBot.DB;
using MTFBot.Extensions;

namespace MTFBot.Bot.Commands
{
    internal class MuteCommand : BaseCommand
    {
        public override string Name => "mute";
        public override async Task RegisterCommand(SocketGuild guild)
        {
            await guild.CreateApplicationCommandAsync(new SlashCommandBuilder()
                .WithName(Name)
                .WithDescription("Запретить пользователю писать сообщения")
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("user")
                    .WithDescription("Пользователь дискорда")
                    .WithType(ApplicationCommandOptionType.User)
                    .WithRequired(true)
                )
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("duration")
                    .WithDescription("Время мьюта. Формат: 1s 2m 3h 4d 5w - 1 секунда, 2 минуты, 3 часа, 4 дня, 5 недель")
                    .WithType(ApplicationCommandOptionType.String)
                    .WithRequired(true)
                )
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("reason")
                    .WithDescription("Причина мьюта")
                    .WithType(ApplicationCommandOptionType.String)
                    .WithRequired(true)
                )
                .Build());
        }

        public override async Task Execute(SocketSlashCommand command, SocketGuild guild)
        {
            var triggeredUser = command.GetTriggeredUser();

            if (!CheckRoles(guild.GetUser(triggeredUser.Id)))
                await command.RespondAsync(text: "У вас недостаточно прав для выполнения этой команды");

            var discordUser = command.GetCommandValue<SocketUser>("user", null);
            var duration = command.GetCommandValue<string>("duration", null);
            var reason = command.GetCommandValue<string>("reason", null);

            var prolongationType = new Regex("\\D").Match(duration).Value;
            var prolongationValue = int.Parse(new Regex("\\d+").Match(duration).Value);

            TimeSpan prolongation;

            switch (prolongationType)
            {
                case "s":
                    prolongation = TimeSpan.FromSeconds(prolongationValue);
                    break;
                case "m":
                    prolongation = TimeSpan.FromMinutes(prolongationValue);
                    break;
                case "h":
                    prolongation = TimeSpan.FromHours(prolongationValue);
                    break;
                case "d":
                    prolongation = TimeSpan.FromDays(prolongationValue);
                    break;
                case "w":
                    prolongation = TimeSpan.FromDays(prolongationValue * 7);
                    break;
                default:
                    prolongation = TimeSpan.FromHours(1);
                    break;
            }

            DiscordMute mute = new DiscordMute()
            {
                DiscordId = discordUser.Id,
                MutedTo = DateTime.Now + prolongation,
                Reason = reason
            };
            Database.Context.DiscordMutes.Add(mute);
            await DiscordHandler.GrantRole(discordUser, Global.Roles.Muted);

            await command.RespondAsync(text: $"Пользователь <@{discordUser.Id}> получил мьют до {mute.MutedTo:yyyy.MM.dd HH:mm:ss}");
        }


        private bool CheckRoles(SocketGuildUser user)
        {
            return (user.HasRole(Global.Roles.Administration) ||
                    user.HasRole(Global.Roles.Moderation));
        }
    }
}
