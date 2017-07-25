using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(LocalAuthorization.Startup))]
namespace LocalAuthorization
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
