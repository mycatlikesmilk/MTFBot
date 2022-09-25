using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTFBot.DB
{
    public class DiscordMute
    {
        [Key]
        [Required]
        public ulong DiscordId { get; set; }
        public DateTime MutedTo { get; set; }
        public string Reason { get; set; }
    }
}
