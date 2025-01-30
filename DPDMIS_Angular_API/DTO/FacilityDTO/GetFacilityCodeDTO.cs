using System.ComponentModel.DataAnnotations;

namespace DPDMIS_Angular_API.DTO.FacilityDTO
{
    public class GetFacilityCodeDTO
    {
        [Key]
        public string FACILITYCODE { get; set; }
    }
}
