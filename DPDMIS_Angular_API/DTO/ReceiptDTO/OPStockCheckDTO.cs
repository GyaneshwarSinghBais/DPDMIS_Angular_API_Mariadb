using System.ComponentModel.DataAnnotations;

namespace DPDMIS_Angular_API.DTO.ReceiptDTO
{
    public class OPStockCheckDTO
    {
        [Key]
        public Int32 OPRECEIPTID { get; set; }
        public string? STAUTS { get; set; }
        

    }
}
