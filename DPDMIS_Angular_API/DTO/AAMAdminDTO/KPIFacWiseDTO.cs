
using System.ComponentModel.DataAnnotations;

namespace DPDMIS_Angular_API.DTO.AAMAdminDTO
{
    public class KPIFacWiseDTO
    {
        [Key]
        public Int64 USERID { get; set; }

        public string? DISTRICTNAME { get; set; }
        public string? LOCATIONNAME { get; set; }
        public string? FACILITYNAME { get; set; }
        public string? PARENTFAC { get; set; }
        public string? OPSTOCK { get; set; }
        public Int64? INDENTDURATION { get; set; }
        public Int64? NOSINDENTTOWH { get; set; }
        public Int64? INDENTTOOTHERFAC { get; set; }
        public string? LASTCONSUMPTIONENTRY { get; set; }
        public Int64? LOCATIONID { get; set; }
        public Int64? FACILITYID { get; set; }
      //  public Int64? FACILITYID_1 { get; set; }
        public Int64? DISTRICTID { get; set; }




    }
}
