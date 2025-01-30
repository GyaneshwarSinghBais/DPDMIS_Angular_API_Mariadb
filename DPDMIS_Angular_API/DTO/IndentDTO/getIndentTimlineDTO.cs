using System.ComponentModel.DataAnnotations;

namespace DPDMIS_Angular_API.DTO.IndentDTO
{
    public class getIndentTimlineDTO
    {
        [Key]

        public string LOCATIONNAME { get; set; } // l.locationname
        public string FACILITYNAME { get; set; } // f.facilityname
        public string CONTACTPERSONNAME { get; set; } // f.contactpersonname
        public string PHONE1 { get; set; } // f.phone1
        public int PHC_ID { get; set; } // f.phc_id
        public string PHCNAME { get; set; } // f2.facilityname AS phcname
        public DateTime? FIRSTINDENTOPENING { get; set; } // f.indentduration
        public DateTime? FIRSTINDENTCLOSING { get; set; } // DATE_ADD(f.indentduration, interval 7 DAY)
        public DateTime? LASTINDENTDATE { get; set; } // i.lastindentdate
        public int FACILITYID { get; set; } // f.facilityid
        public DateTime? INDENTOPENINGDATE { get; set; } // i.indentOpeningDate
        public DateTime? INDENTCLOSINGDATE { get; set; } // i.indentOpeningDate + 15
        public int REGULAROPENINGCASE { get; set; } // Computed RegularOpeningCase logic

        //public Int64 FACILITYID { get; set; }
        //public string? LOCATIONNAME { get; set; }
        //public string? FACILITYNAME { get; set; }
        //public string? CONTACTPERSONNAME { get; set; }
        //public string? PHONE1 { get; set; }
        //public Int64? PHC_ID { get; set; }
        //public string? PHCNAME { get; set; }
        ////public string? SYSDATE { get; set; }
        //public DateTime? FIRSTINDENTOPENING { get; set; }
        //public string? FIRSTINDENTCLOSING { get; set; }
        //public DateTime? LASTINDENTDATE { get; set; }      
        //public DateTime? INDENTOPENINGDATE { get; set; }
        //public DateTime? INDENTCLOSINGDATE { get; set; }
        //public int? REGULAROPENINGCASE { get; set; }
    }

  
}
