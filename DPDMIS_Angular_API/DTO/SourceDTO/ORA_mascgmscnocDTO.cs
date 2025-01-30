using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPDMIS_Angular_API.DTO.SourceDTO
{
    [Table("MASCGMSCNOC")]
    public class ORA_mascgmscnocDTO
    {
        [Key]
        public int? NOCID { get; set; }
        public DateTime? NOCDATE { get; set; }
        public int? FACILITYID { get; set; }
        public int? ACCYRSETID { get; set; }
        public string? STATUS { get; set; }
        public string? NOCNUMBER { get; set; }
        public int? AUTO_NCCODE { get; set; }
        public string? DISPATCHNO { get; set; }
        public DateTime? DISPATCHDATE { get; set; }
        public string? REMARKS { get; set; }
        public string? FILEPATH { get; set; }
        public string? FILENAME { get; set; }
        public string? DISPATCHNOGM { get; set; }
        public DateTime? DISPATCHDATEGM { get; set; }
        public int? PROGRAMID { get; set; }
        public string? NOCSTATUS { get; set; }
        public int? BUDGETID { get; set; }
        public string? IWHISCOMPLETED { get; set; }
        public string? INDENTTYPE { get; set; }
        public string? FILEPATHPUSH { get; set; }
        public string? FILENAMEPUSH { get; set; }
        public int? NICINDENTID { get; set; }
        public string? NICINDENTNO { get; set; }
        public string? EXT { get; set; }
        public string? ISNOS { get; set; }
        public DateTime? ENTRY_DATE { get; set; }
        public string? ISREQFUND { get; set; }
        public string? ISREAGENT { get; set; }
        public string? ISUSEAPP { get; set; }
        public string? NGPO_ID { get; set; }
        public string? NGINDENT_ID { get; set; }
        public string? NGINDENT_NO { get; set; }
        public string? NGHF_ID { get; set; }
        public DateTime? NGINDENT_GENERATION_DATE { get; set; }
        public int? MMID { get; set; }
        public decimal? MNOCID { get; set; }
    }
}
