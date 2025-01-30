using System.ComponentModel.DataAnnotations;

namespace DPDMIS_Angular_API.DTO.AAMAdminDTO
{
    public class getLastIssueDT_DTO
    {
        [Key]
        public Int64 FACILITYID { get; set; }
        public string? DISTRICTNAME { get; set; }
        public string? LOCATIONNAME { get; set; }
        public string? PARENTFAC { get; set; }
        public string? FACILITYNAME { get; set; }
        public Int64? NOSITEMS { get; set; }
        public Int64? STKAVIL { get; set; }
        public Int64? STOCKOUT { get; set; }
        public Int64? DISTRICTID { get; set; }
    
    }
 
}
