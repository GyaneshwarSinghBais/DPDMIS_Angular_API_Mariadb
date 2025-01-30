using System.ComponentModel.DataAnnotations;

namespace DPDMIS_Angular_API.DTO.CGMSCStockDTO
{
    public class getFacilityOpeningsDTO
    {       
        public Int64 receiptid { get; set; }
        public String? receiptno { get; set; }
        public String? receiptdate { get; set; }
        public String? facilityname { get; set; }
        public String? status { get; set; }
    }
}
