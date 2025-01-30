using System.ComponentModel.DataAnnotations;

namespace DPDMIS_Angular_API.DTO.AAMAdminDTO
{
    public class KPIdistWiseDTO
    {
        [Key]
        public Int64 DISTRICTID { get; set; }
        public string? DISTRICTNAME { get; set; }
        public Int64? TARGET { get; set; }
        public Int64? ACHIVEMENT { get; set; }
        public decimal? PER { get; set; }
        public Int64? OPSTOCK { get; set; }
       
    }
}
