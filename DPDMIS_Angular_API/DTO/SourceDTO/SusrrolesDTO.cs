using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPDMIS_Angular_API.DTO.SourceDTO
{
    [Table("USRROLES")]
    public class SusrrolesDTO
    {
        [Key]
        public int ROLEID { get; set; } // Maps to ROLEID
        public string? ROLECODE { get; set; } // Maps to ROLECODE
        public string? ROLENAME { get; set; } // Maps to ROLENAME
        public string? USERTYPE { get; set; } // Maps to USERTYPE
        public bool ALLOWCHANGEPWD { get; set; } = true; // Maps to ALLOWCHANGEPWD with a default value
        public string? ISHOUSER { get; set; } // Maps to ISHOUSER
        public DateTime ENTRYDATE { get; set; } = DateTime.Now; // Maps to ENTRYDATE with a default value
    }
}
