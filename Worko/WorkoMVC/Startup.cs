using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WorkoMVC.Startup))]
namespace WorkoMVC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
