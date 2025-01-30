using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPDMIS_Angular_API.DTO.SourceDTO
{
    [Table("MASDISTRICTS")]
    public class SmasdistrictsDTO
    {
        [Key]
        public int? DISTRICTID { get; set; }
        public string? DISTRICTCODE { get; set; }
        public string? DISTRICTNAME { get; set; }
        public int? STATEID { get; set; }
        public int? DIVISIONID { get; set; }
        public int? NHMDISTRICTID { get; set; }
        public int? CENSUSDISTRICTID { get; set; }
        public int? WAREHOUSEID { get; set; }
        public int? VLFACILITYID { get; set; }
        public DateTime? ENTRY_DATE { get; set; }
        public int? NGDISTRICTID { get; set; }
        public int? NGSTATEID { get; set; }
    }
}
