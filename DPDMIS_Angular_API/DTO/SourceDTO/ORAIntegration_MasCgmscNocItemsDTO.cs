﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPDMIS_Angular_API.DTO.SourceDTO
{
    [Table("MASCGMSCNOCITEMS")]
    public class ORAIntegration_MasCgmscNocItemsDTO
    {
        [Key]
        public Int64 SR { get; set; }
        public Int64? ITEMID { get; set; }
        public Int64? NOCID { get; set; }
        public Int64? REQUESTEDQTY { get; set; }
        public Int64? WHSTOCK { get; set; }
        public String? CGMSCLREMARKS { get; set; }
        public String? STATUS { get; set; }
        public Int64? APPROVEDQTY { get; set; }
        public Int64? BOOKEDQTY { get; set; }
        public String? BOOKEDFLAG { get; set; }
        public Int64? STOCKINHAND { get; set; }
    }
}
