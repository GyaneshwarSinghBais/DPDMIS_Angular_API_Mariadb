using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPDMIS_Angular_API.DTO.SourceDTO
{   
        [Table("MASFACHEADERFOOTER")]
        public class SmasfacheaderfooterDTO
        {
            [Key]
            public string? USERID { get; set; } // Maps to USERID
            public string? HEADER1 { get; set; } // Maps to HEADER1
            public string? HEADER2 { get; set; } // Maps to HEADER2
            public string? HEADER3 { get; set; } // Maps to HEADER3
            public string? FOOTER1 { get; set; } // Maps to FOOTER1
            public string? FOOTER2 { get; set; } // Maps to FOOTER2
            public string? FOOTER3 { get; set; } // Maps to FOOTER3
            public string? DRMOBILE { get; set; } // Maps to DRMOBILE
            public string? DRNAME { get; set; } // Maps to DRNAME
            public string? EMAIL { get; set; } // Maps to EMAIL
        }  
}
