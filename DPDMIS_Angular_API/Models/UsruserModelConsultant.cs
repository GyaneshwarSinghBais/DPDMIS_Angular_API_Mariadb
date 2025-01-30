using System.ComponentModel.DataAnnotations;

namespace DPDMIS_Angular_API.Models
{
    public class UsruserModelConsultant
    {
        [Key]
        [Required]
        public Int64? USERID { get; set; }
        public string? EMAILID { get; set; }
        public string? PWD { get; set; }    
        public string? USERTYPE { get; set; }
        public Int64? DISTRICTID { get; set; }
        public string? HEADER2 { get; set; }
        public string? WAREHOUSENAME { get; set; }
        public Int64? WAREHOUSEID { get; set; }
        public string? DISTRICTNAME { get; set; }
        public string? HEADER3 { get; set; }
        public string? FOOTER3 { get; set; }
        public string? FOOTER1 { get; set; }
        public string? EMAIL { get; set; }
        public string? FOOTER2 { get; set; }
        public string? ROLENAME { get; set; }
        public Int64? ROLEID { get; set; }
    }
 
}
