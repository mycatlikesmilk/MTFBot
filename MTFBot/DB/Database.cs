using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTFBot.DB
{
    public static class Database
    {
        public static UsersContext Context { get; private set; }

        public static void Start()
        {
            Context = new UsersContext();
        }

        public static void Stop()
        {
            Context.Dispose();
        }
    }
}
