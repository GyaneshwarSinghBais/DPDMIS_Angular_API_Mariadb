using System.ComponentModel.DataAnnotations;

namespace DPDMIS_Angular_API.DTO.IssueDTO
{
    public class ItemStockDTO
    {
        [Key]
        public Int64 itemid { get; set; }
        public string? stock { get; set; }
        public Int32 multiple { get; set; }
        public Int64? INDENTQTY { get; set; }
    }
}
