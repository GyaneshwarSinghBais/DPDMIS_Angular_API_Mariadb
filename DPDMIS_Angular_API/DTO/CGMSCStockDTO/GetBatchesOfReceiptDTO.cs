namespace DPDMIS_Angular_API.DTO.CGMSCStockDTO
{
    public class GetBatchesOfReceiptDTO
    {      
        public Int64 ITEMID { get; set; }
        public String? NAME { get; set; }
        public String? FACRECEIPTITEMID { get; set; }
        public String? BATCHNO { get; set; }
        public String? BATCHQTY { get; set; }
        public String? MFGDATE { get; set; }
        public String? EXPDATE { get; set; }
    }
    
}
