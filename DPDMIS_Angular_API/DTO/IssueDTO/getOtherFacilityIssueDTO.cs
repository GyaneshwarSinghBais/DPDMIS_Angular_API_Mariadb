using System.ComponentModel.DataAnnotations;

namespace DPDMIS_Angular_API.DTO.IssueDTO
{
    public class getOtherFacilityIssueDTO
    {
        [Key]
        public Int64 ITEMID { get; set; }
        public string? BATCHNO { get; set; }
        public DateTime? EXPDATE { get; set; }
        public Int64? ISSUEBATCHQTY { get; set; }
        public Int64? INWNO { get; set; }
        public Int64? ISSUEITEMID { get; set; }
        public Int64? FACRECEIPTID { get; set; }
        public Int64? FACRECEIPTITEMID { get; set; }
        public DateTime? MFGDATE { get; set; }
    }
}
