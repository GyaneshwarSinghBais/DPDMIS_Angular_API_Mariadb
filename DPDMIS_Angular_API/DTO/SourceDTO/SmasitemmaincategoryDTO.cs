using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPDMIS_Angular_API.DTO.SourceDTO
{
    [Table("MASITEMMAINCATEGORY")]
    public class SmasitemmaincategoryDTO
    {
        [Key]
        public int MCID { get; set; } // Primary Key, not nullable
        public string? MCATEGORY { get; set; } // Nullable
    }
}
