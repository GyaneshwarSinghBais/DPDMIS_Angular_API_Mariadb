using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPDMIS_Angular_API.MariadbDTO.DestinationDTO
{
    [Table("MASPROGRAM")]
    public class DmasprogramDTO
    {
        [Key]
        public int? PROGRAMID { get; set; }
        public string? PROGRAM { get; set; }
        public int? ORDERID { get; set; }
        public DateTime? ENTRYDT { get; set; }
        public DateTime? LASTUPDATEDT { get; set; }
        public string? ISACTIVE { get; set; }
    }
}
