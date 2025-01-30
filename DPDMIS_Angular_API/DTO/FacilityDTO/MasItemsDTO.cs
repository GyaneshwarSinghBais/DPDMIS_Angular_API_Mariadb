using System.ComponentModel.DataAnnotations;

namespace DPDMIS_Angular_API.DTO.FacilityDTO
{
    public class MasItemsDTO
    {
        [Key]
        public long ITEMID { get; set; }
        public string? NAME { get; set; }

        public string? ITEMCODE { get; set; }
        public string? ITEMNAME { get; set; }
        public string? STRENGTH1 { get; set; }
        public string? UNIT { get; set; }
        public long UNITCOUNT { get; set; }
        public long GROUPID { get; set; }
        public string? GROUPNAME { get; set; }
        public long ITEMTYPEID { get; set; }
        public string? ITEMTYPENAME { get; set; }
        public string? EDLCAT { get; set; }
        public string? EDL { get; set; }
        public string? EDLTYPE { get; set; }
    }
}
