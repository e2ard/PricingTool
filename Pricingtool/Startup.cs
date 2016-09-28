using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(getLayout.Startup))]
namespace getLayout
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
