using System.ComponentModel.DataAnnotations;

namespace DPDMIS_Angular_API.OraDTO.OraFacilityDTO
{
    public class IndentDataDTO
    {
        [Key]
        public long NOCID { get; set; }
        public string? REQUESTEDDATE { get; set; }
        public string? WARDID { get; set; }
    }
}
