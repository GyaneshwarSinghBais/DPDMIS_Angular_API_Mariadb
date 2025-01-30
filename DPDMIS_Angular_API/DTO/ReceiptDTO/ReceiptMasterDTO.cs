using System.ComponentModel.DataAnnotations;

namespace DPDMIS_Angular_API.DTO.ReceiptDTO
{
    public class ReceiptMasterDTO
    {
        [Key]
        public Int64 FACRECEIPTID { get; set; }
        public Int64 NOCID { get; set; }
        public Int64 INDENTID { get; set; }

        public string? REQNO { get; set; }
        public DateTime? REQDATE { get; set; }

        public string? WHISSUENO { get; set; }
        public DateTime? WHISSUEDT { get; set; }

        public string? FACRECEIPTNO { get; set; }
        public DateTime? FACRECEIPTDATE { get; set; }
        public string? STATUS { get; set; }
    }
}
