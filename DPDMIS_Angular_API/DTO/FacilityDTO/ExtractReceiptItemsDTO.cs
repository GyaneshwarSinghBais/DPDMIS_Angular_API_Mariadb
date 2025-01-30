using System.ComponentModel.DataAnnotations;

namespace DPDMIS_Angular_API.DTO.FacilityDTO
{
    public class ExtractReceiptItemsDTO
    {
        [Key]
        public long INWNO { get; set; }
        public long? ITEMID { get; set; }
        public string? BATCHNO { get; set; }
        public string? EXPDATE { get; set; }
        public long? ISSUEBATCHQTY { get; set; }
        public long? INDENTITEMID { get; set; }
        public long? FACRECEIPTID { get; set; }
        public long? FACRECEIPTITEMID { get; set; }
        public string? MFGDATE { get; set; }
        public long? PONOID { get; set; }
        public int? QASTATUS { get; set; }
        public string? WHISSUEBLOCK { get; set; }
    }
}
