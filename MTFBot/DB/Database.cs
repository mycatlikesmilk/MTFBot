using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTFBot.DB
{
    public static class Database
    {
        public static DatabaseContext Context { get; private set; }

        public static void Start()
        {
            Context = new DatabaseContext();
        }

        public static void Stop()
        {
            Context.Dispose();
        }
    }
}
