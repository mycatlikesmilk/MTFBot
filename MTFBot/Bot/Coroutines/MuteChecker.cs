using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTFBot.DB;

namespace MTFBot.Bot.Coroutines
{
    internal static class MuteChecker
    {
        private static List<DB.DiscordMute> Mutes { get; set; }

        public static void UpdateMutes()
        {
            Mutes = Database.Context.DiscordMutes.ToList();
        }

        public static void Do()
        {
            if (!Global.Ready) return;

            if (Mutes == null)
                UpdateMutes();

            var mutes = 0;
            foreach (var muteUser in Mutes)
            {
                if (muteUser.MutedTo > DateTime.Now) continue;

                Database.Context.DiscordMutes.Remove(muteUser);
                DiscordHandler.RemoveRole(muteUser.DiscordId, Global.Roles.Muted).GetAwaiter().GetResult();
                Log.WriteLine($"Мьют пользователя {muteUser.DiscordId} закончился");
                mutes++;
            }
            if (mutes == 0) return;

            Database.Context.SaveChanges();
            UpdateMutes();
        }
    }
}
