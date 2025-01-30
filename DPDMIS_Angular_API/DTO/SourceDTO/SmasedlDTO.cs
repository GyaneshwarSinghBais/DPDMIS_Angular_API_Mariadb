using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPDMIS_Angular_API.DTO.SourceDTO
{
    [Table("MASEDL")]
    public class SmasedlDTO
    {
        [Key]
        public int? EDLCAT { get; set; } // Maps to EDLCAT
        public string? EDLNAME { get; set; } // Maps to EDLNAME
        public string? EDL { get; set; } // Maps to EDL
    }
}
