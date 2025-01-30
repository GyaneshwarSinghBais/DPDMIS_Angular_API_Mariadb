using System.ComponentModel.DataAnnotations;

namespace DPDMIS_Angular_API.DTO.IssueDTO
{
    public class getFacilityIssueBatchesDTO
    {
        [Key]
        public Int32 INWNO { get; set; }
        public string? BATCHNO { get; set; }
        public DateTime? MFGDATE { get; set; }
        public DateTime? EXPDATE { get; set; }
        public Int64? FACISSUEQTY { get; set; }
    }

   
}
