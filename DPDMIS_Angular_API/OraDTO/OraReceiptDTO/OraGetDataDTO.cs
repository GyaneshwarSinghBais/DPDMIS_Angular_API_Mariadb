using DPDMIS_Angular_API.OraDTO.OraCGMSCStockDTO;

namespace DPDMIS_Angular_API.OraDTO.OraReceiptDTO
{
    public class OraGetDataDTO
    {
        public List<OraGetFacreceiptidForOpeningStockDTO> OraGetFacreceiptidForOpeningStockDTO { get; set; }
        public List<OraBarcodeReceiptDto> OraBarcodeReceiptDto { get; set; }
    }
}
