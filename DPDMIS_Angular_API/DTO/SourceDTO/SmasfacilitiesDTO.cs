using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPDMIS_Angular_API.DTO.SourceDTO
{
    [Table("MASFACILITIES")]
    public class SmasfacilitiesDTO
    {
        [Key]
        public long? FACILITYID { get; set; }
        public string? FACILITYCODE { get; set; }
        public string? FACILITYNAME { get; set; }
        public string? ADDRESS1 { get; set; }
        public string? ADDRESS2 { get; set; }
        public string? ADDRESS3 { get; set; }
        public string? CITY { get; set; }
        public string? ZIP { get; set; }
        public string? PHONE1 { get; set; }
        public string? PHONE2 { get; set; }
        public string? FAX { get; set; }
        public string? EMAIL { get; set; }
        public int? ISACTIVE { get; set; }
        public long? STATEID { get; set; }
        public long? DISTRICTID { get; set; }
        public long? LOCATIONID { get; set; }
        public long? FACILITYTYPEID { get; set; }
        public string? REMARKS { get; set; }
        public string? CONTACTPERSONNAME { get; set; }
        public string? NHM_HCID { get; set; }
        public long? NHM_DISTRICTID { get; set; }
        public long? TYPE { get; set; }
        public string? FACILITYNAME_HINDI { get; set; }
        public string? DHSSTORE { get; set; }
        public string? CHCID { get; set; }
        public long? CHCFACILITYID { get; set; }
        public DateTime? GDT_ENTRY_DATE { get; set; }
        public string? LONGITUDE { get; set; }
        public string? LATITUDE { get; set; }
        public string? UHCFAC { get; set; }
        public string? ISBUFFERFREEZE { get; set; }
        public string? ISBUFFERCONSFREEZ { get; set; }
        public long? PPHCID { get; set; }
        public long? SHC_ID { get; set; }
        public long? PHC_ID { get; set; }
        public string? ISCOVIDFAC { get; set; }
        public string? LATLONGREMARKS { get; set; }
        public string? INCHARGENAME { get; set; }
        public string? HC_IDMAPPED { get; set; }
        public DateTime? OPDATE { get; set; }
        public string? ISUPDATED { get; set; }
        public string? ITEMINDENTMC_H { get; set; }
        public long? NINNO { get; set; }
        public string? ISUPDATEDNINFROMGOI_REC_DATA { get; set; }
        public long? AYFACTYPEID { get; set; }
        public string? ISSHCMAPPED { get; set; }
        public long? OLDDISTRICTID { get; set; }
        public string? ISDRIVERADDEDPOSITION { get; set; }
        public DateTime? POSITIONENTRYBYDRIVER { get; set; }
        public string? ISWHFREEZLAT { get; set; }
        public long? NGHF_ID { get; set; }
        public string? ISNGWORKING { get; set; }
        public string? IS_AAM { get; set; }
        public DateTime? INDENTDURATION { get; set; }
    }
}
