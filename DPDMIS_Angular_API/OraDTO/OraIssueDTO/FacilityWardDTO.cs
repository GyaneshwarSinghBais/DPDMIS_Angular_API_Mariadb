using System.ComponentModel.DataAnnotations;

namespace DPDMIS_Angular_API.OraDTO.OraIssueDTO
{
    public class FacilityWardDTO
    {
        [Key]
        public int WardID { get; set; }
        public string? WardName { get; set; }
    }
}
