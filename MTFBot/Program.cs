using MTFBot.Bot;

namespace MTFBot
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += OnExit;

            await MTFBot.Bot.Core.Start();
            await Task.Delay(-1);
        }

        private static async void OnExit(object sender, EventArgs e)
        {
            await MTFBot.Bot.Core.Stop();
        }
    }
}