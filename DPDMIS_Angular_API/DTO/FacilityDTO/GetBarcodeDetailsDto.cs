namespace DPDMIS_Angular_API.DTO.FacilityDTO
{
    public class GetBarcodeDetailsDto
    {
        public string? INDENTID { get; set; }
        public string? INDENTNO { get; set; }
        public string? INDENTDATE { get; set; }

        public string? ITEMCODE { get; set; }
        public string? ITEMNAME { get; set; }

        public string? BATCHNO { get; set; }
        public string? MFGDATE { get; set; }
        public string? EXPDATE { get; set; }

        public string? ISSUEQTY { get; set; }
        public string? BARCODEID { get; set; }
    }
}
