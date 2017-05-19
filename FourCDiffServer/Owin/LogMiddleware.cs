using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace FourCDiffServer.Owin {
    /// <summary>
    /// Custom <see cref="OwinMiddleware"/> that operates between the web server and web application.
    /// </summary>
    /// <remarks>
    /// This was added to this sample project to provide for extra logging.
    /// </remarks>
    public class LogMiddleware : OwinMiddleware {
        public LogMiddleware(OwinMiddleware next) : base(next) {
        }

        public async override Task Invoke(IOwinContext context) {
            Debug.WriteLine("Begin execution request : {0} {1}", context.Request.Method, context.Request.Uri);
            await Next.Invoke(context);
            Debug.WriteLine("End execution request : {0} {1}", context.Request.Method, context.Request.Uri);
        }
    }
}