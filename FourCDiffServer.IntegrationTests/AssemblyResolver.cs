using System.Collections.Generic;
using System.Reflection;
using System.Web.Http.Dispatcher;

namespace FourCDiffServer.IntegrationTests {
    class TestWebApiResolver : DefaultAssembliesResolver {
        public override ICollection<Assembly> GetAssemblies() {
            return new List<Assembly> { typeof(FourCDiffServer.Controllers.DiffItemsController).Assembly };

        }
    }
}
