using System.ComponentModel.DataAnnotations;

namespace DPDMIS_Angular_API.DTO.IssueDTO
{
    public class WardIssueItemsDTO
    {
        [Key]
        public Int32 itemid { get; set; }
        public string? name { get; set; }
    }
}
