using System.ComponentModel.DataAnnotations;

namespace DPDMIS_Angular_API.DTO.IndentDTO
{
    public class getBlockFACsDTO
    {
     
        [Key]
        public int FACILITYID { get; set; }
        public string? FACNAME { get; set; }
        public int? FORDER { get; set; }
        public int? FACILITYTYPEID { get; set; }
        
    }
}
