using Microsoft.EntityFrameworkCore;
using MTFBot.Bot;
using MTFBot.DB;

namespace MTFBot
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += OnExit;
            Database.Start();
            await Database.Context.Database.MigrateAsync();
#if SQL
            
#else

            Global.Setup();

            await MTFBot.Bot.Core.Start();
            await Task.Delay(-1);
#endif
        }

        private static async void OnExit(object sender, EventArgs e)
        {
            Global.TimerCancellationToken.Cancel();
            Database.Stop();
            await MTFBot.Bot.Core.Stop();
        }
    }
}