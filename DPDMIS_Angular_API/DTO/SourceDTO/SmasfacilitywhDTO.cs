using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPDMIS_Angular_API.DTO.SourceDTO
{
    [Table("MASFACILITYWH")]
    public class SmasfacilitywhDTO
    {
        [Key]
        public int? FACILITYWHNO { get; set; }
        public int? WAREHOUSEID { get; set; }
        public int? FACILITYID { get; set; }
        public string? STATUS { get; set; } = "A"; // Default value
        public DateTime? ENTRY_DATE { get; set; } = DateTime.Now; // Default value
        public string? REMARKS { get; set; }
    }
}
