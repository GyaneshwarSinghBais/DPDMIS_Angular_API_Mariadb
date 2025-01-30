using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPDMIS_Angular_API.DTO.SourceDTO
{
    [Table("MASITEMCATEGORIES")]
    public class SmasitemcategoriesDTO
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
