using System.ComponentModel.DataAnnotations;

namespace DPDMIS_Angular_API.DTO.FacilityDTO
{
    public class GenWhIndentNoDTO
    {
        [Key]
        public long FACILITYID { get; set; }
        public string? FACILITYCODE { get; set; }
    }
}
