using System.ComponentModel.DataAnnotations;

namespace DPDMIS_Angular_API.DTO.IndentDTO
{
    public class SavedFacIndentItemsDTO
    {
        [Key]
        public Int64 SR { get; set; }
        public Int64? ItemID { get; set; }
        public String? itemname { get; set; }
        public Int64? STOCKINHAND { get; set; }
        public Int64? whindentQTY { get; set; }
        public String? status { get; set; }

        public String? ITEMCODE { get; set; }
        public String? ITEMTYPENAME { get; set; }
        public String? STRENGTH1 { get; set; }
        public String? MCATEGORY { get; set; }
        public Int64? MCID { get; set; }


    }
}
