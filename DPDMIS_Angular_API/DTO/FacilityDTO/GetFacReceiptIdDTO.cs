using System.ComponentModel.DataAnnotations;

namespace DPDMIS_Angular_API.DTO.FacilityDTO
{
    public class GetFacReceiptIdDTO
    {
        [Key]
        //public string FACRECEIPTID { get; set; }
        public Int64 FACRECEIPTID { get; set; }
    }
}
