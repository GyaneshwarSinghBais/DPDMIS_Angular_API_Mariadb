using System.ComponentModel.DataAnnotations.Schema;

namespace DPDMIS_Angular_API.Models
{
    [Table("USRUSERS")]
    public class LoginModel
    {
        public string emailid { get; set; }
        public string pwd { get; set; }
    }
}
