using System.ComponentModel.DataAnnotations;

namespace DPDMIS_Angular_API.DTO.CGMSCStockDTO
{
    public class GetReceiptItemsDTO
    {
        [Key]
        public Int64 ITEMID { get; set; }
        public String? NAME { get; set; }
      
    }
}
