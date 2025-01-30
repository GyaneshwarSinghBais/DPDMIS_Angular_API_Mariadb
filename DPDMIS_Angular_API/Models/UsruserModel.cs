using System.ComponentModel.DataAnnotations;

namespace DPDMIS_Angular_API.Models
{
    public class UsruserModel
    {
        [Key]
        [Required]
        //public string? USERID { get; set; }
        //public string? EMAILID { get; set; }
        //public string? PWD { get; set; }
        //public string? FIRSTNAME { get; set; }
        //public string? USERTYPE { get; set; }
        //public string? DISTRICTID { get; set; }
        //public string? FACILITYID { get; set; }

        //public string? DEPMOBILE { get; set; }
        //public string? FOOTER3 { get; set; }
        //public string? FACILITYTYPEID { get; set; }
        //public string? FACTYPEID { get; set; }
        //public string? WHAIPERMISSION { get; set; }
        //public string? FACILITYTYPECODE { get; set; }
        //public string? FOOTER2 { get; set; }
        //public string? APPROLE { get; set; }
        //public string? PHONE1 { get; set; }
        //public string? RESETPASSWORD { get; set; }

        //public string? ROLENAME { get; set; }
        //public Int64? ROLEID { get; set; }

        public Int64? USERID { get; set; }
        public string? EMAILID { get; set; }
        public string? PWD { get; set; }
        public string? FIRSTNAME { get; set; }
        public string? USERTYPE { get; set; }
        public Int64? DISTRICTID { get; set; }
        public Int64? FACILITYID { get; set; }

        //public string? DEPMOBILE { get; set; }
        //public string? FOOTER3 { get; set; }
        public Int64? FACILITYTYPEID { get; set; }
        //public string? FACTYPEID { get; set; }
        //public string? WHAIPERMISSION { get; set; }
        //public string? FACILITYTYPECODE { get; set; }
        //public string? FOOTER2 { get; set; }
        //public string? APPROLE { get; set; }
        public string? PHONE1 { get; set; }
        //public Int16? RESETPASSWORD { get; set; }

        public Int64? roleid { get; set; }
        public string? ROLENAME { get; set; }
    }
}
