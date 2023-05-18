using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(HighchartSample.Startup))]
namespace HighchartSample
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
        }
    }
}
