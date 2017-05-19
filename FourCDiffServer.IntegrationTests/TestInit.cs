using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Entity;

namespace FourCDiffServer.IntegrationTests {
    [TestClass]
    public static class TestInit {
        public const string TestDatabaseName = "TestDatabase";

        [AssemblyCleanup]
        public static void AssemblyCleanup() {
            Database.Delete(TestDatabaseName);
        }

    }
}
