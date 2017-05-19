using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FourCDiffServer.Models {
    public class DiffItem {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string LeftSide { get; set; }
        public string RightSide { get; set; }
    }
}