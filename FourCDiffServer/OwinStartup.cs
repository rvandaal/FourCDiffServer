using Microsoft.Owin;
using Owin;
using FourCDiffServer.Owin;

[assembly: OwinStartup(typeof(FourCDiffServer.OwinStartup))]

namespace FourCDiffServer {
    public class OwinStartup {
        public void Configuration(IAppBuilder app) {
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
            app.Use(typeof(LogMiddleware));
        }
    }
}
