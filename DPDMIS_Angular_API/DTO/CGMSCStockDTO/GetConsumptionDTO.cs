

using System.ComponentModel.DataAnnotations;

namespace DPDMIS_Angular_API.DTO.CGMSCStockDTO
{
    public class GetConsumptionDTO
    {

        [Key]
        public Int64 ID { get; set; }
        public Int64? INWNO { get; set; }
     
        public Int64? ITEMID { get; set; }
        public String? ITEMCODE { get; set; }
        public DateTime? ISSUEDATE { get; set; }
        public String? STRENGTH1 { get; set; }
        public String? ITEMNAME { get; set; }
        public String? BATCHNO { get; set; }
        public DateTime? MFGDATE { get; set; }
        public DateTime? EXPDATE { get; set; }
        public decimal? ISSUEQTY { get; set; }
        public Int64? WARDID { get; set; }
        public String? WARDNAME { get; set; }
    }
}
