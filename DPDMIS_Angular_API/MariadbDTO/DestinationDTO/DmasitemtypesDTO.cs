using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPDMIS_Angular_API.MariadbDTO.DestinationDTO
{
    [Table("MASITEMTYPES")]
    public class DmasitemtypesDTO
    {
        [Key]
        public int? ITEMTYPEID { get; set; }
        public string? ITEMTYPECODE { get; set; }
        public string? ITEMTYPENAME { get; set; }
        public string? TYPEPREFIX { get; set; }
        public int? QCDAYSLAB { get; set; }
        public int? POITEMTYPEID { get; set; }
        public DateTime? ENTRYDT { get; set; }
    }
}
