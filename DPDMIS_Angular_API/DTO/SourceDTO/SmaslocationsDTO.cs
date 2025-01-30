using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPDMIS_Angular_API.DTO.SourceDTO
{
    [Table("MASLOCATIONS")]
    public class SmaslocationsDTO
    {
        [Key]
        public int? LOCATIONID { get; set; }
        public int? STATEID { get; set; }
        public int? DISTRICTID { get; set; }       
        public string? LOCATIONCODE { get; set; }
        public string? LOCATIONNAME { get; set; }
    }
}
