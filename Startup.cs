using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(test1702.StartupOwin))]

namespace test1702
{
    public partial class StartupOwin
    {
        public void Configuration(IAppBuilder app)
        {
            //AuthStartup.ConfigureAuth(app);
        }
    }
}
