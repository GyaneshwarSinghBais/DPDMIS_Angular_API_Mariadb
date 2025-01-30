using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPDMIS_Angular_API.DTO.IndentDTO
{
    [Table("MASFACDEMANDITEMS")]
    public class OraMasFacDemandItemsDTO
    {
        [Key]
        public Int64 INDENTITEMID { get; set; }
        public Int64? INDENTID { get; set; }
        public Int64? ITEMID { get; set; }
        public Int64? REQUESTEDQTY { get; set; }
        public Int64? FACSTOCK { get; set; }
        public Int64? APPROVEDQTY { get; set; }
        public String? STATUS { get; set; }
        public Int64? STOCKINHAND { get; set; }
       // public Int64? MTRANSFERID { get; set; }
    }
}
