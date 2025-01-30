using System.ComponentModel.DataAnnotations;

namespace DPDMIS_Angular_API.DTO.IndentDTO
{
    public class SHCitemlistDTO
    {
        [Key]
        public Int64 ITEMID { get; set; }
        public String? ITEMCODE { get; set; }
        public String? ITEMTYPENAME { get; set; }
        public String? ITEMNAME { get; set; }
        public String? STRENGTH1 { get; set; }
        public Int64? MULTIPLE { get; set; }
        public Int64? UNITCOUNT { get; set; }
        public Int64? INDENTQTY { get; set; }
        public Int64? SR { get; set; }
        

    }
}
