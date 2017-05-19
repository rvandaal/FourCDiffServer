using System.Web.Http;

namespace FourCDiffServer {
    /// <summary>
    /// Configures WebApi.
    /// </summary>
    public static class WebApiConfig {
        public static void Register(HttpConfiguration config) {
            config.MapHttpAttributeRoutes();
        }
    }
}
