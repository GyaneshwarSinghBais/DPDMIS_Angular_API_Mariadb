using System.ComponentModel.DataAnnotations;

namespace DPDMIS_Angular_API.OraDTO.OraIssueDTO
{
    public class getIssueItemIdDTO
    {
        [Key]
        public Int32 ISSUEITEMID { get; set; }
        public Int32 ISSUEID { get; set; }

        public Int32 ITEMID { get; set; }
    }
}
