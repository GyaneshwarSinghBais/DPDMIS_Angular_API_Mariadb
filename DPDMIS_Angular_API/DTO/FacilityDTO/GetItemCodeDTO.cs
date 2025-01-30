using System.ComponentModel.DataAnnotations;

namespace DPDMIS_Angular_API.DTO.FacilityDTO
{
    public class GetItemCodeDTO
    {
        [Key]
        public string ITEMCODE { get; set; }
    }
}
