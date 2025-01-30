using System.ComponentModel.DataAnnotations;

namespace DPDMIS_Angular_API.DTO.FacilityDTO
{
    public class GetFacilityReceiptIdDTO
    {
        [Key]
        public long FACRECEIPTID { get; set; }
    }
}
