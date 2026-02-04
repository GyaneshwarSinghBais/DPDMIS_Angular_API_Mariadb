namespace DPDMIS_Angular_API.DTO.ReceiptDTO
{
    public class BarcodeReceiptDto
    {

        public string? BARCODEID { get; set; }
        public string? FACRECEIPTID { get; set; }
        public string? FACILITYID { get; set; }
        public string? INDENTID { get; set; }
        public string? WAREHOUSEID { get; set; }

        public string? FACRECEIPTNO { get; set; }
        public string? FACRECEIPTDATE { get; set; }

        public string? ISUSEAPP { get; set; }
        public string? STATUS { get; set; }
        public string? FACRECEIPTTYPE { get; set; }

        public string? ISSUEID { get; set; }
        public string? TOFACILITYID { get; set; }
    }
}
