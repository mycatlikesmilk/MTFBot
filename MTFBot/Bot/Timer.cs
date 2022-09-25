using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTFBot.Bot
{
    internal class Timer
    {
        public static void DoTimer(object token)
        {
            var cancellationToken = (CancellationToken)token;

            while (true)
            {
                if (cancellationToken.IsCancellationRequested) return;

                Coroutines.MuteChecker.Do();
                Thread.Sleep(1000);
            }
        }
    }
}
