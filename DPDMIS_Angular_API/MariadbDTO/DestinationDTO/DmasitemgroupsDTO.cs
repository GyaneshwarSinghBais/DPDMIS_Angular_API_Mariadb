using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPDMIS_Angular_API.MariadbDTO.DestinationDTO
{
    [Table("MASITEMGROUPS")]
    public class DmasitemgroupsDTO
    {
        [Key]
        public int? GROUPID { get; set; } // Nullable
        public string? GROUPCODE { get; set; } // Nullable
        public string? GROUPNAME { get; set; } // Nullable
        public int? REFITEMGROUPID { get; set; } // Nullable
        public string? ENTRY_DATE { get; set; } // Nullable
    }
}
