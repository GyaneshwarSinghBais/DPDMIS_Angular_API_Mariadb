using System.ComponentModel.DataAnnotations;

namespace DPDMIS_Angular_API.DTO.CGMSCStockDTO
{
    public class getDistrictAchivementStatusDTO
    {
        [Key]
        public Int64 DISTRICTID { get; set; }
        public String? DISTRICTNAME { get; set; }
        public Int32? TARGET { get; set; }
        public Int32? ACHIVEMENT { get; set; }
        public Decimal? PER { get; set; }
    }
   
}
