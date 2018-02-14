using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DeMonte.Startup))]
namespace DeMonte
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
