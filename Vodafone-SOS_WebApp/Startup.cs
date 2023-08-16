using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Vodafone_SOS_WebApp.Startup))]
namespace Vodafone_SOS_WebApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
