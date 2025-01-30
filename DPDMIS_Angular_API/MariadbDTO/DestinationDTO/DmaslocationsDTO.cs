using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPDMIS_Angular_API.MariadbDTO.DestinationDTO
{
    [Table("MASLOCATIONS")]
    public class DmaslocationsDTO
    {
        [Key]
        public int? LOCATIONID { get; set; }
        public int? STATEID { get; set; }
        public int? DISTRICTID { get; set; }       
        public string? LOCATIONCODE { get; set; }
        public string? LOCATIONNAME { get; set; }
    }
}
