using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPDMIS_Angular_API.Models
{
    [Table("MASCGMSCNOC")]
    public class tbGenIndent
    {
        //    [Key]
        //    public int NOCID { get; set; } // Primary key, assumed non-nullable
        //    public DateTime? NOCDATE { get; set; } // Nullable DateTime (as NOCDATE is nullable in the DB)
        //    public int? FACILITYID { get; set; } // Nullable int (since it's nullable in DB)
        //    public int? ACCYRSETID { get; set; } // Nullable int (since it's nullable in DB)
        //    public string? STATUS { get; set; } // Nullable string (since STATUS is nullable in DB)
        //    public string? NOCNUMBER { get; set; } // Nullable string (since NOCNUMBER is nullable in DB)
        //    public int? AUTO_NCCODE { get; set; } // Nullable int
        //    public string? DISPATCHNO { get; set; } // Nullable string
        //    public DateTime? DISPATCHDATE { get; set; } // Nullable DateTime
        //    public string? REMARKS { get; set; } // Nullable string
        //    public string? FILEPATH { get; set; } // Nullable string
        //    public string? FILENAME { get; set; } // Nullable string
        //    public string? DISPATCHNOGM { get; set; } // Nullable string
        //    public DateTime? DISPATCHDATEGM { get; set; } // Nullable DateTime
        //    public int? PROGRAMID { get; set; } // Nullable int
        //    public string? NOCSTATUS { get; set; } // Nullable string
        //    public int? BUDGETID { get; set; } // Nullable int
        //    public string? IWHISCOMPLETED { get; set; } // Nullable string
        //    public string? INDENTTYPE { get; set; } // Nullable string
        //    public string? FILEPATHPUSH { get; set; } // Nullable string
        //    public string? FILENAMEPUSH { get; set; } // Nullable string
        //    public int? NICINDENTID { get; set; } // Nullable int
        //    public string? NICINDENTNO { get; set; } // Nullable string
        //    public string? EXT { get; set; } // Nullable string
        //    public string? ISNOS { get; set; } // Nullable string
        //    public DateTime? ENTRY_DATE { get; set; } // Nullable DateTime
        //    public string? ISREQFUND { get; set; } // Nullable string
        //    public string? ISREAGENT { get; set; } // Nullable string
        //    public string? ISUSEAPP { get; set; } // Nullable string
        //    public string? NGPO_ID { get; set; } // Nullable string
        //    public string? NGINDENT_ID { get; set; } // Nullable string
        //    public string? NGINDENT_NO { get; set; } // Nullable string
        //    public string? NGHF_ID { get; set; } // Nullable string
        //    public DateTime? NGINDENT_GENERATION_DATE { get; set; } // Nullable DateTime
        //    public int? MMID { get; set; } // Nullable int


        [Key]
        public Int64 NOCID { get; set; }
        [Required]
        public Int64? PROGRAMID { get; set; }
        public Int64? ACCYRSETID { get; set; }
        public Int64? FACILITYID { get; set; }

        public string? NOCNUMBER { get; set; }
        public string? NOCDATE { get; set; }
        public string? STATUS { get; set; }

        public string? AUTO_NCCODE { get; set; }

        public string? ISUSEAPP { get; set; }
    }
}
