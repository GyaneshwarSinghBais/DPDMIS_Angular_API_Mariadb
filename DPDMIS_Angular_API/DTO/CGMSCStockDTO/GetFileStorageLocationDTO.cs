using System.ComponentModel.DataAnnotations;

namespace DPDMIS_Angular_API.DTO.CGMSCStockDTO
{
    public class GetFileStorageLocationDTO
    {
        [Key]
        public Int64 RACKID { get; set; }
        public String? LOCATIONNO { get; set; }
    }
}
