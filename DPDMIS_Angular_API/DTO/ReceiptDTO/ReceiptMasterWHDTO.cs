using System.ComponentModel.DataAnnotations;

namespace DPDMIS_Angular_API.DTO.ReceiptDTO
{
    public class ReceiptMasterWHDTO
    {


        [Key]

        public Int32? NOCID { get; set; }
        public DateTime? REQDATE { get; set; }
        public string? REQNO { get; set; }
        public int? NOSITEMSREQ { get; set; }
        public string? WHISSUENO { get; set; }
        public DateTime? WHISSUEDT { get; set; }
        public int? NOSITEMSISSUED { get; set; }
        public int? INDENTID { get; set; }
        public string? FACRECEIPTNO { get; set; }
        public DateTime? FACRECEIPTDATE { get; set; }
        public string? RSTATUS { get; set; }
        public int? FACRECEIPTID { get; set; }
        public int? FACILITYID { get; set; }
        public int? WAREHOUSEID { get; set; }

        //public Int64 NOCID { get; set; }
        //public string? REQDATE { get; set; }
        //public string? REQNO { get; set; }

        //public string? NOSITEMSREQ { get; set; }
        //public string? WHISSUENO { get; set; }
        //public string? WHISSUEDT { get; set; }
        //public string? FACRECEIPTNO { get; set; }
        //public string? FACRECEIPTDATE { get; set; }
        //public string? RSTATUS { get; set; }


        //public Int64? NOSITEMSISSUED { get; set; }
        //public Int64? INDENTID { get; set; }
        //public Int64? FACRECEIPTID { get; set; }
        //public Int64? FACILITYID { get; set; }
        //public Int64? WAREHOUSEID { get; set; }
    }
}
