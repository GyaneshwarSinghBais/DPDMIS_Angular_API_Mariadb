using System.ComponentModel.DataAnnotations;

namespace DPDMIS_Angular_API.DTO.IndentDTO
{
    public class IndentItemsFromWardDTO
    {
        [Key]
        public Int32 ITEMID { get; set; }
        public string? NAME { get; set; }

    }
}
