using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace MTFBot.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    internal class RoleIDAttribute : Attribute
    {
        public string RoleName { private set; get; }

        public RoleIDAttribute(string RoleName)
        {
            this.RoleName = RoleName;
        }

        public static ulong GetRoleId(Global.Roles role)
        {
            try
            {
                var type = role.GetType();
                var name = Enum.GetName(type, role);
                
                var description = type.GetField(name).GetCustomAttributes(false).OfType<RoleIDAttribute>().SingleOrDefault().RoleName;
                
                return (ulong)Global.Configuration["Server"]["Roles"][description].Value<ulong>();
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex.Message, Log.LogLevel.FATAL);
                throw ex;
            }
        }
        
        //string description = Enumerations.GetEnumDescription((MyEnum)value);
    }
}
