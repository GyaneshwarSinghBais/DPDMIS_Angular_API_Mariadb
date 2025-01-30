namespace DPDMIS_Angular_API.DTO.ReceiptDTO
{
    public class getReceiptByIdDTO
    {

        public Int64? ITEMID { get; set; }
        public String? ITEMCODE { get; set; }
        public String? ITEMNAME { get; set; }
        public String? STRENGTH { get; set; }
        
        public String? BATCHNO { get; set; }
        public DateTime? MFGDATE { get; set; }
        public DateTime? EXPDATE { get; set; }
        //public String? ISSUEWH { get; set; }
        //public String? INDENTID { get; set; }
        public Int32? FACRECEIPTID { get; set; }
        //public String? ABSRQTY { get; set; }
        public Double? BATRCHRECEIPTQTY { get; set; }
        //public String? LOCATIONNO { get; set; }
        //public String? STOCKLOCATION { get; set; }
        public Int32? FACRECEIPTITEMID { get; set; }
        public Int32? INWNO { get; set; }
        public String? RSTATUS { get; set; }
        //public String? RISTATUS { get; set; }
    }
    //ITEMID, ITEMCODE, ITEMNAME, BATCHNO, MFGDATE, EXPDATE, BATRCHRECEIPTQTY, FACRECEIPTITEMID, INWNO, RSTATUS, FACRECEIPTID
}
