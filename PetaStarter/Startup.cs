using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Speedbird.Startup))]
namespace Speedbird
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
