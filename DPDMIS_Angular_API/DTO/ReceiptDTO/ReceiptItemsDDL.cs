using System.ComponentModel.DataAnnotations;

namespace DPDMIS_Angular_API.DTO.ReceiptDTO
{
    public class ReceiptItemsDDL
    {
        [Key]
        public Int32 INWNO { get; set; }
        public string? NAME { get; set; }
    }
}
