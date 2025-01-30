using System.ComponentModel.DataAnnotations;

namespace DPDMIS_Angular_API.DTO.FacilityDTO
{
    public class GetWHsSerialNoDTO
    {
        [Key]
        public string WHSLNO { get; set; }
    }
}
