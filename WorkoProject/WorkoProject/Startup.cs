using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WorkoProject.Startup))]
namespace WorkoProject
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //ConfigureAuth(app);
        }
    }
}
