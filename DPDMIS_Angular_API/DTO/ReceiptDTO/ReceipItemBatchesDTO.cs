using System.ComponentModel.DataAnnotations;

namespace DPDMIS_Angular_API.DTO.ReceiptDTO
{
    public class ReceipItemBatchesDTO
    {
    
       
        public Int64 INWNO { get; set; }
        public Int64? SR { get; set; }
        public Int64? ITEMID { get; set; }

        public Int64? MULTIPLE { get; set; }
        public Int64? UNITCOUNT { get; set; }
        public Int64? INDENTQTY { get; set; }
        public Int64? ISSUEWH { get; set; }

        public string? ITEMCODE { get; set; }
        public string? ITEMTYPENAME { get; set; }

        public string? ITEMNAME { get; set; }
        public string? STRENGTH1 { get; set; }
        public string? BATCHNO { get; set; }
        public string? MFGDATE { get; set; }
        public string? EXPDATE { get; set; }
      


        public Int64? rqty { get; set; }
       public Int64? PONOID { get; set; }
        

    }
}
