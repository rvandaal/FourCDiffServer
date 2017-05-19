using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Microsoft.Owin.Testing;
using FourCDiffServer.Owin;
using Owin;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Net.Http;
using System.Data.Entity;
using System.Transactions;

namespace FourCDiffServer.IntegrationTests {
    [TestClass]
    public class FourCDiffServerTests {

        // Using OWIN: https://docs.microsoft.com/en-us/aspnet/web-api/overview/hosting-aspnet-web-api/use-owin-to-self-host-web-api
        // 
        // NOTE: i couldn't get this test to work. I left the code for you to see how far i came with it.
        // When self-hosting (with OWIN) works, i could set a breakpoint in the GetDiffItemAsync method in the controller,
        // and it would hit the breakpoint but not connect to the database correctly. It would return an InternalServerError. 
        // This controller works with a FourCDiffServerContext that is connected to the live database. I don't know how to use a testdatabase. 
        // I updated the app.config and inserted the connection string for a testdatabase, but i'm not sure how and when this database
        // should be created. Should i use the migrations as configured in the FourCDiffServer project?

        class OwinTestConf {
            public void Configuration(IAppBuilder app) {
                app.Use(typeof(LogMiddleware));
                HttpConfiguration config = new HttpConfiguration();
                config.Services.Replace(typeof(IAssembliesResolver), new TestWebApiResolver());
                config.MapHttpAttributeRoutes();
                app.UseWebApi(config);
            }
        }

        protected DbContext DbContext;
        protected TransactionScope TransactionScope;

        [TestInitialize]
        public void Initialize() {
            // If i comment out the initialize and cleanup, the GetAsync will return an InternalServerError.
            DbContext = new DbContext(TestInit.TestDatabaseName);
            DbContext.Database.CreateIfNotExists();
            TransactionScope = new TransactionScope(TransactionScopeOption.RequiresNew);
        }

        [TestCleanup]
        public void CleanUp() {
            TransactionScope.Dispose();
        }

        [TestMethod]
        public async Task TestMethod1() {

            using(var server = TestServer.Create<OwinTestConf>()) {
                using(var client = new HttpClient(server.Handler)) {
                    var response = await client.GetAsync("http://testserver/v1/diff/1");
                    Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
                }
            }

            
            // old code, not using OWIN:
            //using (WebApp.Start<Startup>(url: BaseURL)) {
            //    // Create HttpCient and make a request to api/values 
            //    HttpClient client = new HttpClient();

            //    var response = await client.GetAsync(BaseURL + "v1/diff/1");
            //    Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);

            //    //response = await client.PutAsync(
            //    //    BaseURL + "v1/diff/1/left", 
            //    //    new StringContent("{\"Data\":\"AAAAAA==\"}", Encoding.UTF8, "application/json")
            //    //);
            //    //Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            //    //Assert.AreEqual("AAAAAA==", response.Content.ReadAsStringAsync().Result);
            //}



        }
    }
}
