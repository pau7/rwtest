using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(RWProject.Startup))]
namespace RWProject
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
