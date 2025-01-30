using System.ComponentModel.DataAnnotations;

namespace DPDMIS_Angular_API.DTO.ReceiptDTO
{
    public class getOtherFacIssueDetailsDTO
    {
        [Key]
        public Int32 ISSUEID { get; set; }
        public string? FACILITYNAME { get; set; }
        public string? INDENTNO { get; set; }
        public string? INDENTDATE { get; set; }
        public string? ISSUENO { get; set; }
        public string? ISSUEDDATE { get; set; }
        public string? NOS { get; set; }
        public string? FACINDENTID { get; set; }
        public string? FACRECEIPTID { get; set; }
        public string? FACRECEIPTNO { get; set; }
        public string? FACRECEIPTDATE { get; set; }
        public string? STATUS { get; set; }
        public Int32? facilityid { get; set; }
        
    }

   
}
