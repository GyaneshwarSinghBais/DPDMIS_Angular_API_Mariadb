using System.ComponentModel.DataAnnotations;

namespace DPDMIS_Angular_API.DTO.IndentDTO
{
    public class IncompleteWardIssueDTO
    {      

        public Int32 WARDID { get; set; }
        public string? WARDNAME { get; set; }
        public string? ISSUENO { get; set; }
        public DateTime? ISSUEDATE { get; set; }
        public DateTime? WREQUESTDATE { get; set; }
        public string? WREQUESTBY { get; set; }
        public string? STATUS { get; set; }
        public Int64? INDENTID { get; set; }
        [Key]
        public Int64? ISSUEID { get; set; }
    }
}
