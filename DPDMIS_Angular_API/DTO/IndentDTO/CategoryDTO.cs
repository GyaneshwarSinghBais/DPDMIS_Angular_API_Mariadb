using System.ComponentModel.DataAnnotations;

namespace DPDMIS_Angular_API.DTO.IndentDTO
{
    public class CategoryDTO
    {
        [Key]
        public Int32 CATEGORYID { get; set; }
        public string? CATEGORYNAME { get; set; }
    }
}
