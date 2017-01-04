using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SANParentBanking.Startup))]
namespace SANParentBanking
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
