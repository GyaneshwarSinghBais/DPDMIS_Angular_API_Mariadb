using System.ComponentModel.DataAnnotations;

namespace DPDMIS_Angular_API.DTO.FacilityDTO
{
    public class masAccYearSettingsModel
    {
        [Key]
        public int ACCYRSETID { get; set; }
        public string? ACCYEAR { get; set; }
        public DateTime? STARTDATE { get; set; }
        public DateTime? ENDDATE { get; set; }
        public string? SHACCYEAR { get; set; }
        public int YEARORDER { get; set; }
    }
}
