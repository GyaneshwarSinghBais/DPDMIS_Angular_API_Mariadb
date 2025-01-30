using DPDMIS_Angular_API.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPDMIS_Angular_API.MariadbDTO.DestinationDTO
{
   // [Table("MASCGMSCNOC")]
    public class MDB_mascgmscnocDTO 
    {
        [Key]
        public Int64 NOCID { get; set; }

        public Int64? PROGRAMID { get; set; }
        public Int64? ACCYRSETID { get; set; }
        public Int64? FACILITYID { get; set; }

        public string? NOCNUMBER { get; set; }
        public DateTime? NOCDATE { get; set; }
        public DateTime? ENTRY_DATE { get; set; }
        
        public string? STATUS { get; set; }

        public Int32? AUTO_NCCODE { get; set; }

        public string? ISUSEAPP { get; set; }
    }
}
