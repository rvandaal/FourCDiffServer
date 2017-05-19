using FourCDiffServer.Controllers;
using System.Web.Http;
using System.Web.Http.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FourCDiffServer.Models;
using System.Linq;
using System.Threading.Tasks;

namespace FourCDiffServer.Tests {
    /// <summary>
    /// Tests the <see cref="DiffItemsController"/> class.
    /// </summary>
    [TestClass]
    public class DiffItemsControllerTests {

        DiffItemsController controller;

        [TestInitialize]
        public void Initialize() {
            controller = new DiffItemsController(new TestFourCDiffServerContext());
        }

        [TestCleanup]
        public void CleanUp() {
            controller.Dispose();
        }

        [TestMethod]
        public async Task TestGetWithoutInput() {
            IHttpActionResult getResult = await controller.GetDiffItemAsync(1) as NotFoundResult;
            Assert.IsNotNull(getResult);
        }

        [TestMethod]
        public async Task TestPutLeftSide() {
            CreatedNegotiatedContentResult<DiffInput> putResult =
                await controller.PutLeftSideAsync(1, new DiffInput { Data = "AAAAAA==" }) as CreatedNegotiatedContentResult<DiffInput>;
            Assert.IsNotNull(putResult);
            Assert.AreEqual("AAAAAA==", putResult.Content.Data);
        }

        [TestMethod]
        public async Task TestGetWithOnlyLeftSideInput() {
            var putResult = await controller.PutLeftSideAsync(1, new DiffInput { Data = "AAAAAA==" });
            IHttpActionResult getResult = await controller.GetDiffItemAsync(1) as NotFoundResult;
            Assert.IsNotNull(getResult);
        }

        [TestMethod]
        public async Task TestPutRightSide() {
            CreatedNegotiatedContentResult<DiffInput> putResult =
                await controller.PutRightSideAsync(1, new DiffInput { Data = "AAAAAA==" }) as CreatedNegotiatedContentResult<DiffInput>;
            Assert.IsNotNull(putResult);
            Assert.AreEqual("AAAAAA==", putResult.Content.Data);
        }

        [TestMethod]
        public async Task TestGetWithEqualInput() {
            var putResult = await controller.PutLeftSideAsync(1, new DiffInput { Data = "AAAAAA==" });
            var putResult2 = await controller.PutRightSideAsync(1, new DiffInput { Data = "AAAAAA==" });

            OkNegotiatedContentResult<DiffResult> getResult = await controller.GetDiffItemAsync(1) as OkNegotiatedContentResult<DiffResult>;
            Assert.AreEqual("Equals", getResult.Content.DiffResultType);
        }

        [TestMethod]
        public async Task TestGetWithEqualInputLengthButUnequalContent() {
            var putResult = await controller.PutLeftSideAsync(1, new DiffInput { Data = "AAAAAA==" });
            var putResult2 = await controller.PutRightSideAsync(1, new DiffInput { Data = "ABAQAB==" });

            OkNegotiatedContentResult<DiffResult> getResult = await controller.GetDiffItemAsync(1) as OkNegotiatedContentResult<DiffResult>;
            Assert.AreEqual("ContentsDoNotMatch", getResult.Content.DiffResultType);
            var list = getResult.Content.Diffs.ToList();
            Assert.AreEqual(3, list.Count);
            Assert.AreEqual(1, list[0].Offset);
            Assert.AreEqual(1, list[0].Length);
            Assert.AreEqual(3, list[1].Offset);
            Assert.AreEqual(1, list[1].Length);
            Assert.AreEqual(5, list[2].Offset);
            Assert.AreEqual(1, list[2].Length);

            putResult = await controller.PutRightSideAsync(1, new DiffInput { Data = "AASDFA*_" });

            getResult = await controller.GetDiffItemAsync(1) as OkNegotiatedContentResult<DiffResult>;
            Assert.AreEqual("ContentsDoNotMatch", getResult.Content.DiffResultType);
            list = getResult.Content.Diffs.ToList();
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual(2, list[0].Offset);
            Assert.AreEqual(3, list[0].Length);
            Assert.AreEqual(6, list[1].Offset);
            Assert.AreEqual(2, list[1].Length);
        }

        [TestMethod]
        public async Task TestGetWithUnequalInputLength() {
            var putResult = await controller.PutLeftSideAsync(1, new DiffInput { Data = "AAA=" });
            var putResult2 = await controller.PutRightSideAsync(1, new DiffInput { Data = "AAAAAA==" });

            OkNegotiatedContentResult<DiffResult> getResult = await controller.GetDiffItemAsync(1) as OkNegotiatedContentResult<DiffResult>;
            Assert.AreEqual("SizesDoNotMatch", getResult.Content.DiffResultType);
        }

        [TestMethod]
        public async Task TestPutWithInvalidInput() {
            object putResult = await controller.PutLeftSideAsync(1, new DiffInput { Data = null });
            Assert.IsNotNull(putResult);
        }

        [TestMethod]
        public async Task TestOtherIds() {
            CreatedNegotiatedContentResult<DiffInput> putResult =
                await controller.PutLeftSideAsync(86, new DiffInput { Data = "AAAAAA==" }) as CreatedNegotiatedContentResult<DiffInput>;
            Assert.IsNotNull(putResult);
            Assert.AreEqual("AAAAAA==", putResult.Content.Data);
            CreatedNegotiatedContentResult<DiffInput> putResult2 =
                await controller.PutRightSideAsync(86, new DiffInput { Data = "ABCADA==" }) as CreatedNegotiatedContentResult<DiffInput>;
            Assert.IsNotNull(putResult2);
            Assert.AreEqual("ABCADA==", putResult2.Content.Data);

            OkNegotiatedContentResult<DiffResult> getResult = await controller.GetDiffItemAsync(86) as OkNegotiatedContentResult<DiffResult>;
            Assert.AreEqual("ContentsDoNotMatch", getResult.Content.DiffResultType);
            var list = getResult.Content.Diffs.ToList();
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual(1, list[0].Offset);
            Assert.AreEqual(2, list[0].Length);
            Assert.AreEqual(4, list[1].Offset);
            Assert.AreEqual(1, list[1].Length);
        }
    }
}
