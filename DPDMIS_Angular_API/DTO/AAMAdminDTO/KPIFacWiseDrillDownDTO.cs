namespace DPDMIS_Angular_API.DTO.AAMAdminDTO
{
    public class KPIFacWiseDrillDownDTO
    {
        public int? DISTRICTID { get; set; }
        public string? DISTRICTNAME { get; set; }

        public int? FACILITYID { get; set; }
        public string? FACILITYNAME { get; set; }

        public int? NOSDRUGSOPSTOCK { get; set; }
        public int? NOSINDENT { get; set; }
        public int? NOSITEMSINDENTED { get; set; }

        public int? NOSRECEIPT { get; set; }
        public int? NOSITEMSRECEIPTS { get; set; }
    }
}
