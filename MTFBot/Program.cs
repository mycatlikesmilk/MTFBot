using MTFBot.Bot;
using MTFBot.DB;

namespace MTFBot
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += OnExit;
            SqliteHelper.Open();
#if SQL
            
#else

            Global.Setup();

            await MTFBot.Bot.Core.Start();
            await Task.Delay(-1);
#endif
        }

        private static async void OnExit(object sender, EventArgs e)
        {
            await MTFBot.Bot.Core.Stop();
            SqliteHelper.Close();
            Log.StopLog();
        }
    }
}