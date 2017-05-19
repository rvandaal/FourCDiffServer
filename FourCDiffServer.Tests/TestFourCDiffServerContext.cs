using System.Data.Entity;
using FourCDiffServer.Controllers;
using FourCDiffServer.Models;
using System.Threading.Tasks;

namespace FourCDiffServer.Tests {
    /// <summary>
    /// Mockup class for the <see cref="FourCDiffServerContext"/> class.
    /// </summary>
    /// <remarks>
    /// An instance of this class should be injected in the <see cref="DiffItemsController"/>, in order to execute unittests.
    /// </remarks>
    public class TestFourCDiffServerContext : IFourCDiffServerContext {
        public TestFourCDiffServerContext() {
            this.DiffItems = new TestDiffItemDbSet();
        }

        public DbSet<DiffItem> DiffItems { get; set; }

        public int SaveChanges() {
            return 0;
        }
        public Task<int> SaveChangesAsync() {
            return Task.FromResult(0);
        }

        public void MarkAsAdded(DiffItem item) { }
        public void MarkAsModified(DiffItem item) { }
        public void Dispose() { }
    }
}