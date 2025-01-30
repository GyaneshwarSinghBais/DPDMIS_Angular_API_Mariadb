using System.ComponentModel.DataAnnotations;

namespace DPDMIS_Angular_API.DTO.IndentDTO
{
    public class getIndentProgramDTO
    {
        [Key]
        public int programid { get; set; }
        public string? program { get; set; }
    }
}
