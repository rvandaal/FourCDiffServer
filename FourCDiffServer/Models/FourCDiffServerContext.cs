using System.Data.Entity;
using FourCDiffServer.Controllers;

namespace FourCDiffServer.Models {
    /// <summary>
    /// Custom <see cref="DbContext"/> that handles communication between the <see cref="DiffItemsController"/> and the database.
    /// </summary>
    public class FourCDiffServerContext : DbContext, IFourCDiffServerContext {

        public FourCDiffServerContext() : base("name=FourCDiffServerContext") {
        }

        public DbSet<DiffItem> DiffItems { get; set; }

        public void MarkAsAdded(DiffItem item) {
            Entry(item).State = EntityState.Added;
        }
        public void MarkAsModified(DiffItem item) {
            Entry(item).State = EntityState.Modified;
        }
    }
}
