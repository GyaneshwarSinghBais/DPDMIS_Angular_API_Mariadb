using System.ComponentModel.DataAnnotations;

namespace DPDMIS_Angular_API.DTO.AAMAdminDTO
{
    public class getFacilityItemWiseStockDTO
    {
       
        public string CATEGORYNAME { get; set; }
        public string? ITEMCODE { get; set; }
        public string? ITEMTYPENAME { get; set; }
        public string? ITEMNAME { get; set; }
        public string? STRENGTH1 { get; set; }
        public Int64? READYFORISSUE { get; set; }
        public Int64? FACILITYID { get; set; }
        public Int64? ITEMID { get; set; }
        public Int64? CATEGORYID { get; set; }
        public string? EDLTYPE { get; set; }
        public string? FACILITYNAME { get; set; }
       
        public string? DISTRICTNAME { get; set; }
        public string? LOCATIONNAME { get; set; }
        public Int64? DISTRICTID { get; set; }
        public Int64? LOCATIONID { get; set; }
    }
   
}
