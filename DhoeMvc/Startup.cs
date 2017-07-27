using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DhoeMvc.Startup))]
namespace DhoeMvc
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
