using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MovieBlog.Startup))]
namespace MovieBlog
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
