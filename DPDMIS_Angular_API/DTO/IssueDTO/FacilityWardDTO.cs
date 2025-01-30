using System.ComponentModel.DataAnnotations;

namespace DPDMIS_Angular_API.DTO.IssueDTO
{
    public class FacilityWardDTO
    {
        [Key]
        public int WardID { get; set; }
        public string? WardName { get; set; }
    }
}
