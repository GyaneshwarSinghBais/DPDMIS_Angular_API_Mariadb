using System.ComponentModel.DataAnnotations;

namespace DPDMIS_Angular_API.MariadbDTO.DestinationDTO
{
    public class MASFACTRANSFERS_DTO 
    {
        [Key]
        public int INDENTID { get; set; }                // Primary Key
        public string? INDENTNO { get; set; }            // Can be null
        public DateTime? INDENTDATE { get; set; }        // Can be null
        public int? FACILITYID { get; set; }             // Can be null
        public int? FROMFACILITYID { get; set; }         // Can be null
        public string? DISPATCHNO { get; set; }          // Can be null
        public DateTime? DISPATCHDATE { get; set; }      // Can be null
        public string? REMARKS { get; set; }             // Can be null
        public int? PROGRAMID { get; set; }              // Can be null
        public int? AUTO_CODE { get; set; }              // Can be null
        public DateTime? ENTRYDATE { get; set; }         // Can be null
        public int? ACCYRSETID { get; set; }             // Can be null
        public string? STATUS { get; set; }              // Can be null
        public int? MTRANSFERID { get; set; }
        
    }
}
