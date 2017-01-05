using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MemotraficoV2.Startup))]
namespace MemotraficoV2
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
