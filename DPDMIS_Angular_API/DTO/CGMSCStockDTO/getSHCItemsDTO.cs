

using System.ComponentModel.DataAnnotations;

namespace DPDMIS_Angular_API.DTO.CGMSCStockDTO
{
    public class getSHCItemsDTO
    {
        [Key]
        public Int64 ITEMID { get; set; }
        public String ITEMCODE { get; set; }
     
    }
}
