using System.ComponentModel.DataAnnotations;
using System.ServiceModel.Channels;

namespace DPDMIS_Angular_API.DTO.CGMSCStockDTO
{
    public class GetWHStockItemsDTO
    {

       [Key]
        public Int64 ITEMID { get; set; }
        public String? itemname { get; set; }
        public String? itemcode { get; set; }
        public String? itemtypename { get; set; }
        public String? unitcount { get; set; }
        public String? strength1 { get; set; }

        public Int64 ReadySTK { get; set; }
        public Int64 UqcSTK { get; set; }
    }
}
