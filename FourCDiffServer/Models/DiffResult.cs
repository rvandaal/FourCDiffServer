using System.Collections.Generic;

namespace FourCDiffServer.Models {
    public class DiffResult {
        public string DiffResultType { get; set; }
        public IEnumerable<Diff> Diffs { get; set; }
    }
}