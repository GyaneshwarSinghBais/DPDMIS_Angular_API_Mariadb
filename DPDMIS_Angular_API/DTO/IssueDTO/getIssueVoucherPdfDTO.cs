using System.ComponentModel.DataAnnotations;

namespace DPDMIS_Angular_API.DTO.IssueDTO
{
    public class getIssueVoucherPdfDTO
    {
        [Key]
        public Int32 INWNO { get; set; }
        public string? BATCHNO { get; set; }
        public string? MFGDATE { get; set; }
        public string? EXPDATE { get; set; }
        public Int64? FACISSUEQTY { get; set; }
        public string? ITEMCODE { get; set; }
        public string? ITEMNAME { get; set; }
        public string? STRENGTH1 { get; set; }
        public string? ISSUENO { get; set; }
        public string? ISSUEDT { get; set; }
        public string? WARDNAME { get; set; }
        public string? STATUS { get; set; }
    }
     
}
