using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPDMIS_Angular_API.DTO.ReceiptDTO
{
    [Table("TBFACILITYRECEIPTITEMS")]
    public class tbFacilityReceiptItemsModel
    {
        [Key]
        public Int64 FACRECEIPTITEMID { get; set; }
        [Required]
        public Int64 FACRECEIPTID { get; set; }
        public Int64 ITEMID { get; set; }
        public Int64? INDENTITEMID { get; set; }
        public Int64? ABSRQTY { get; set; }
        public Int64? ISSUEITEMID { get; set; }
    

    }
}
