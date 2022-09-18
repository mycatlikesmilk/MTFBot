using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTFBot.Exceptions
{
    internal class UserAlreadyInDatabaseException : Exception
    {
        public override string Message => "Пользователь уже добавлен в базу данных";
    }
}
