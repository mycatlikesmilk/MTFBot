using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTFBot.Enums;

namespace MTFBot.DB
{
    public class User
    {
        [Key]
        [Required]
        public string DiscordId { get; set; }
        [Required]
        public string SteamId { get; set; }
        public DateTime LinkDate { get; set; }
        public WhitelistState WhitelistState { get; set; }

        public User()
        {

        }

        public User(string discordId, string steamId)
        {
            DiscordId = discordId;
            SteamId = steamId;
            LinkDate = DateTime.Now;
            WhitelistState = WhitelistState.NoWhitelist;
        }
    }
}
