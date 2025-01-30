using System.ComponentModel.DataAnnotations;

namespace DPDMIS_Angular_API.MariadbDTO
{
    public class GetAllFacilityTypesDTO
    {
        [Key]
        public int FACILITYTYPEID { get; set; } // Not nullable, as it's a primary key with AUTO_INCREMENT
        public string? FACILITYTYPECODE { get; set; } // VARCHAR(10), nullable
        public string? FACILITYTYPEDESC { get; set; } // VARCHAR(100), nullable, with a UNIQUE constraint
        public int? DIRECTORATEID { get; set; } // INT(10), nullable
        public int? ELDCAT { get; set; } // INT(10), nullable
        public byte? ORDERQ { get; set; } // TINYINT(2), nullable
        public int? ORDERD { get; set; } // INT(10), nullable
        public decimal? ORDERDP { get; set; } // DECIMAL(10,2), nullable
        public string? UNIQUSTATS { get; set; } // VARCHAR(50), nullable
        public int? EDL_ELIGIBILITY { get; set; } // INT(10), nullable
        public int? HODID { get; set; } // INT(10), nullable
        public int? EDLINDENT { get; set; } // INT(10), nullable
        public DateTime? ENTRY_DATE { get; set; } // DATETIME, nullable, defaults to CURRENT_TIMESTAMP
    }
}
