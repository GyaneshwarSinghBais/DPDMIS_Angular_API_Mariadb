using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPDMIS_Angular_API.DTO.SourceDTO
{
    [Table("MASACCYEARSETTINGS")]
    public class SmasaccyearsettingsDTO
    {
        [Key]
        public int? ACCYRSETID { get; set; }
        public string? ACCYEAR { get; set; }
        public DateTime? STARTDATE { get; set; }
        public DateTime? ENDDATE { get; set; }
        public string? SHACCYEAR { get; set; }
        public int? YEARORDER { get; set; }
        public DateTime? ENTRYDT { get; set; }
        public DateTime? LASTUPDATEDT { get; set; }
    }
}
