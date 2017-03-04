using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ITWorkLogs.Startup))]
namespace ITWorkLogs
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
