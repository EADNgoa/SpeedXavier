using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Speedbird.Startup))]
namespace Speedbird
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            Microsoft.ApplicationInsights.Extensibility.TelemetryConfiguration.Active.DisableTelemetry = true;
            ConfigureAuth(app);
        }
    }
}
