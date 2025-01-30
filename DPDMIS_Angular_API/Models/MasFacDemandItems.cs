using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPDMIS_Angular_API.Models
{
    [Table("MASFACDEMANDITEMS")] 
    public class MasFacDemandItems
    {
        [Key]
        public Int64 INDENTITEMID { get; set; }
        public Int64? INDENTID { get; set; }        
        public Int64? ITEMID { get; set; }
        public Int64? REQUESTEDQTY { get; set; }
        public Int64? FACSTOCK { get; set; }
        public Int64? APPROVEDQTY { get; set; }
        public String? STATUS { get; set; }
        public Int64? STOCKINHAND { get; set; }
     
    }
   
}
