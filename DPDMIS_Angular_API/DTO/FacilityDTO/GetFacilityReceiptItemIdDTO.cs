using System.ComponentModel.DataAnnotations;

namespace DPDMIS_Angular_API.DTO.FacilityDTO
{
    public class GetFacilityReceiptItemIdDTO
    {
        [Key]
        public long FACILITYID { get; set; }
    }
}
