
using System.ComponentModel.DataAnnotations;

namespace DPDMIS_Angular_API.DTO.CGMSCStockDTO
{
    public class StockReportFacilityDTO
    {
        [Key]
        public int ItemId { get; set; }
        public string? CategoryName { get; set; }
        public string? ItemCode { get; set; }
        public string? itemtypename { get; set; }
        public string? ItemName { get; set; }
        public string? Strength1 { get; set; }
        public Int64? ReadyForIssue { get; set; }

        public int FacilityId { get; set; }

        public int CategoryId { get; set; }
        public string? EDLType { get; set; }
    }
}
