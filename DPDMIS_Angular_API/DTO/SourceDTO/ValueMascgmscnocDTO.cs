using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPDMIS_Angular_API.DTO.SourceDTO
{
  //  [Table("MASCGMSCNOCITEMS")]
    public class ValueMascgmscnocDTO
    {
        [Key]
        public Int64 MNOCID { get; set; }
    }
}
