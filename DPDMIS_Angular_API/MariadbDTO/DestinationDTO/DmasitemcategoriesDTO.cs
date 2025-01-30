using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPDMIS_Angular_API.MariadbDTO.DestinationDTO
{
    [Table("MASITEMCATEGORIES")]
    public class DmasitemcategoriesDTO
    {
        [Key]
        public int? CATEGORYID { get; set; }
        public string? CATEGORYCODE { get; set; }
        public string? CATEGORYNAME { get; set; }
        public string? CATEGORYPREFIX { get; set; }
        public string? SUPPLYORDERCODE { get; set; }
        public DateTime? ENTRY_DATE { get; set; }
        public int? MCID { get; set; }
    }
}
