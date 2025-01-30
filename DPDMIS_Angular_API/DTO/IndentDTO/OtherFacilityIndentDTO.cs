using System.ComponentModel.DataAnnotations;

namespace DPDMIS_Angular_API.DTO.IndentDTO
{
    public class OtherFacilityIndentDTO
    {
        [Key]
        public string? INDENTNO { get; set; }          // m.INDENTNO
        public DateTime? INDENTDATE { get; set; }      // m.INDENTDATE
        public string? FACILITYNAME { get; set; }      // f.facilityname
        public int? FACILITYID { get; set; }           // f.facilityid
        public int? FROMFACILITYID { get; set; }       // m.fromfacilityid
        public int? INDENTID { get; set; }             // m.INDENTID
        public string? STATUS { get; set; }            // m.status
        public int? FACILITYTYPEID { get; set; }
        
    }

    
}
