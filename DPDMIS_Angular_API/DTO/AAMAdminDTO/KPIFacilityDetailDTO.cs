namespace DPDMIS_Angular_API.DTO.AAMAdminDTO
{
    public class KPIFacilityDetailDTO
    {
        public long? DISTRICTID { get; set; }
        public string? DISTRICTNAME { get; set; }

        public string? BLOCKNAME { get; set; }

        public long? FACILITYID { get; set; }
        public string? FACILITYNAME { get; set; }

        public string? EMAILID { get; set; }
        public string? PHONE1 { get; set; }
        public string? CONTACTPERSONNAME { get; set; }

        public string? PARENTFACILITY { get; set; }
        public string? WAREHOUSENAME { get; set; }

        public int? NOSDRUGSOPSTOCK { get; set; }
        public int? NOSINDENT { get; set; }
        public int? NOSITEMSINDENTED { get; set; }

        public int? NOSRECEIPT { get; set; }
        public int? NOSITEMSRECEIPTS { get; set; }
    }
}
