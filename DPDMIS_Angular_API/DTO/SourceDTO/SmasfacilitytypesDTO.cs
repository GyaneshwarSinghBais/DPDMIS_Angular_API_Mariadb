using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPDMIS_Angular_API.DTO.SourceDTO
{
    [Table("MASFACILITYTYPES")]
    public class SmasfacilitytypesDTO
    {
        [Key]
        public int? FACILITYTYPEID { get; set; }
        public string? FACILITYTYPECODE { get; set; }
        public string? FACILITYTYPEDESC { get; set; }
        public int? DIRECTORATEID { get; set; }
        public int? ELDCAT { get; set; }
        public decimal? ORDERQ { get; set; }
        public int? ORDERD { get; set; }
        public decimal? ORDERDP { get; set; }
        public string? UNIQUSTATS { get; set; }
        public int? EDL_ELIGIBILITY { get; set; }
        public int? HODID { get; set; }
        public int? EDLINDENT { get; set; }
        public DateTime? ENTRY_DATE { get; set; }
    }
}
