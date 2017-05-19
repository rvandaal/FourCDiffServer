using System.Threading.Tasks;
using System.Data.Entity;

namespace FourCDiffServer.Models {
    public interface IFourCDiffServerContext {
        DbSet<DiffItem> DiffItems { get; set; }
        void MarkAsAdded(DiffItem item);
        void MarkAsModified(DiffItem item);
        int SaveChanges();
        Task<int> SaveChangesAsync();
        void Dispose();
    }
}
