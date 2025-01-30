using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPDMIS_Angular_API.MariadbDTO.DestinationDTO
{
    [Table("MASWAREHOUSES")]
    public class DmaswarehousesDTO
    {
        [Key]
        public int WAREHOUSEID { get; set; }
        public string? WAREHOUSECODE { get; set; }
        public string? WAREHOUSENAME { get; set; }
        public int? WAREHOUSETYPEID { get; set; }
        public string? ADDRESS1 { get; set; }
        public string? ADDRESS2 { get; set; }
        public string? ADDRESS3 { get; set; }
        public string? CITY { get; set; }
        public string? ZIP { get; set; }
        public string? PHONE1 { get; set; }
        public string? PHONE2 { get; set; }
        public string? FAX { get; set; }
        public string? EMAIL { get; set; }
        public bool ISACTIVE { get; set; } = true;
        public bool ISGOVERNMENT { get; set; } = true;
        public int? STATEID { get; set; }
        public int? DISTRICTID { get; set; }
        public int? LOCATIONID { get; set; }
        public string? REMARKS { get; set; }
        public string? CONTACTPERSONNAME { get; set; }
        public string? AMID { get; set; }
        public string? SOID { get; set; }
        public int? PARENTID { get; set; }
        public DateTime? GDT_ENTRY_DATE { get; set; }
        public int? FACILITYTYPEID { get; set; }
        public int? FACILITYTYPE { get; set; }
        public decimal? POPER { get; set; }
        public string? ENTRY_DATE { get; set; }
        public int? ZONEID { get; set; }
        public string? LATITUDE { get; set; }
        public string? LONGITUDE { get; set; }
        public string? ISANPRACTIVE { get; set; }
    }
}
