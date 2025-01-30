using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPDMIS_Angular_API.DTO.SourceDTO
{
    [Table("MASFACILITYWARDS")]
    public class SmasfacilitywardsDTO
    {
        [Key]
        public int WARDID { get; set; }
        public int? FACILITYID { get; set; }
        public string? WARDCODE { get; set; }
        public string? WARDNAME { get; set; }
        public int? ISACTIVE { get; set; }
        public string? MCWARDID { get; set; }
        public string? PWD { get; set; }
        public int? ROLE { get; set; }
        public int? ROLEID { get; set; }
        public string? ISDH { get; set; }
        public string? ISVISIBLE { get; set; }
        public string? ENTRY_DATE { get; set; }
        public string? APPROLE { get; set; }
    }
}
