using DPDMIS_Angular_API.DTO.CGMSCStockDTO;

namespace DPDMIS_Angular_API.DTO.ReceiptDTO
{
    public class GetDataDTO
    {
        public List<GetFacreceiptidForOpeningStockDTO> GetFacreceiptidForOpeningStockDTO { get; set; }
        public List<BarcodeReceiptDto> BarcodeReceiptDto { get; set; }
    }
}
