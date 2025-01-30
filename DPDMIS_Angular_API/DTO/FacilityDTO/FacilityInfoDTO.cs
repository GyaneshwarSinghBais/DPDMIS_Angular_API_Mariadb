using System.ComponentModel.DataAnnotations;

namespace DPDMIS_Angular_API.DTO.FacilityDTO
{
    public class FacilityInfoDTO
    {
        [Key]
        public long FACILITYID { get; set; }
        public string? FACILITYCODE { get; set; }
        public string? FACILITYNAME { get; set; }
        public string? FOOTER1 { get; set; }
        public string? FOOTER2 { get; set; }
        public string? FOOTER3 { get; set; }
        public string? EMAIL { get; set; }
        public long? DISTRICTID { get; set; }
        public string? DISTRICTNAME { get; set; }
        public string? FACILITYTYPECODE { get; set; }
        public string? FACILITYTYPEDESC { get; set; }
        public string? WAREHOUSENAME { get; set; }
        public string? WHEMAIL { get; set; }
        public string? WHCONTACT { get; set; }
        public long? ELDCAT { get; set; }
        public long? WAREHOUSEID { get; set; }
        public long? FACILITYTYPEID { get; set; }
        public int? HODID { get; set; }
        public int? CMHOFACILITY { get; set; }
    }
}
