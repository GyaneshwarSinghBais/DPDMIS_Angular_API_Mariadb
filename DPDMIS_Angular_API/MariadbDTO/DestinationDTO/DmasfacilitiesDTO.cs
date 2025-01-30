using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPDMIS_Angular_API.MariadbDTO.DestinationDTO
{
    [Table("MASFACILITIES")]
    public class DmasfacilitiesDTO
    {
        [Key]
        public int? FACILITYID { get; set; }
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
        public bool? ISACTIVE { get; set; } // TINYINT(1) -> bool
        public int? STATEID { get; set; }
        public int? DISTRICTID { get; set; }
        public int? LOCATIONID { get; set; }
        public int? FACILITYTYPEID { get; set; }
        public string? REMARKS { get; set; }
        public string? CONTACTPERSONNAME { get; set; }
        public string? NHM_HCID { get; set; }
        public long? NHM_DISTRICTID { get; set; } // BIGINT(20)
        public long? TYPE { get; set; } // BIGINT(20)
        public string? FACILITYNAME_HINDI { get; set; }
        public string? DHSSTORE { get; set; }
        public string? CHCID { get; set; }
        public int? CHCFACILITYID { get; set; }
        public DateTime? GDT_ENTRY_DATE { get; set; } // DATETIME
        public string? LONGITUDE { get; set; }
        public string? LATITUDE { get; set; }
        public string? UHCFAC { get; set; }
        public string? ISBUFFERFREEZE { get; set; }
        public string? ISBUFFERCONSFREEZ { get; set; }
        public int? PPHCID { get; set; }
        public int? SHC_ID { get; set; }
        public int? PHC_ID { get; set; }
        public string? ISCOVIDFAC { get; set; }
        public string? LATLONGREMARKS { get; set; }
        public string? INCHARGENAME { get; set; }
        public string? HC_IDMAPPED { get; set; }
        public DateTime? OPDATE { get; set; } // DATETIME
        public string? ISUPDATED { get; set; }
        public string? ITEMINDENTMC_H { get; set; }
        public int? NINNO { get; set; }
        public string? ISUPDATEDNINFROMGOI_REC_DATA { get; set; }
        public int? AYFACTYPEID { get; set; }
        public string? ISSHCMAPPED { get; set; }
        public int? OLDDISTRICTID { get; set; }
        public string? ISDRIVERADDEDPOSITION { get; set; }
        public DateTime? POSITIONENTRYBYDRIVER { get; set; } // DATETIME
        public string? ISWHFREEZLAT { get; set; }
        public int? NGHF_ID { get; set; }
        public string? ISNGWORKING { get; set; }
        public string? IS_AAM { get; set; }
        public DateTime? INDENTDURATION { get; set; } // DATETIME
    }
}
