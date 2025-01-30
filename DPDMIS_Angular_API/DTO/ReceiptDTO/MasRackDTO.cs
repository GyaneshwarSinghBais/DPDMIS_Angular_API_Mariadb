using System.ComponentModel.DataAnnotations;

namespace DPDMIS_Angular_API.DTO.ReceiptDTO
{
    public class MasRackDTO
    {
        [Key]
        public Int32 RACKID { get; set; }
        public string? LOCATIONNO { get; set; }
    }
}
