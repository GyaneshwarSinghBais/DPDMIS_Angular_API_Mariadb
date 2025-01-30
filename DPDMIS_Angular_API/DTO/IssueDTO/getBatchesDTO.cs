using System.ComponentModel.DataAnnotations;

namespace DPDMIS_Angular_API.DTO.IssueDTO
{
    public class getBatchesDTO
    {

        [Key]
        public Int64 Inwno { get; set; }
        public Int64 FacReceiptItemID { get; set; }
        public string BatchNo { get; set; }

        public DateTime MfgDate { get; set; }
        public DateTime ExpDate { get; set; }
        public Int64 AbsRQty { get; set; }
        public Int64 AllotQty { get; set; }
        public Int64 avlQty { get; set; }
        public Int64 whinwno { get; set; }
    }
}
