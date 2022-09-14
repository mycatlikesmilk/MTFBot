using Exiled.API.Features;

namespace MTFBotPlugin
{
    public class MTFPlugin : Plugin<Configuration>
    {
        public override void OnEnabled()
        {

        }
    }

    public class Configuration : Exiled.API.Interfaces.IConfig
    {
        public bool IsEnabled { get; set; }
    }
}