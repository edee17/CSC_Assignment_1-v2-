using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Task1.Startup))]
namespace Task1
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
