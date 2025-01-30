using System.ComponentModel.DataAnnotations;

namespace DPDMIS_Angular_API.DTO.FacilityDTO
{
    public class tbFacilityReceiptItemsDTO
    {
        [Key]
        public long FACRECEIPTITEMID { get; set; }
        public long? ABSRQTY { get; set; }
    }
}
