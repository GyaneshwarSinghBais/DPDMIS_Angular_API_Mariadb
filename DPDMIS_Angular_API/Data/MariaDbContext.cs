using CgmscHO_API.DTO;
using DPDMIS_Angular_API.DTO.AAMAdminDTO;
using DPDMIS_Angular_API.DTO.CGMSCStockDTO;
using DPDMIS_Angular_API.DTO.FacilityDTO;
using DPDMIS_Angular_API.DTO.IndentDTO;
using DPDMIS_Angular_API.DTO.IssueDTO;
using DPDMIS_Angular_API.DTO.LoginDTO;
using DPDMIS_Angular_API.DTO.ReceiptDTO;
using DPDMIS_Angular_API.DTO.SourceDTO;
using DPDMIS_Angular_API.MariadbDTO;
using DPDMIS_Angular_API.MariadbDTO.DestinationDTO;
using DPDMIS_Angular_API.Models;
using Microsoft.EntityFrameworkCore;

namespace DPDMIS_Angular_API.Data
{
    public class MariaDbContext : DbContext
    {
        public MariaDbContext(DbContextOptions<MariaDbContext> options) : base(options) { }

        //Oracle TO MariaDB Conversion DTO's

        public DbSet<UsruserModel> Usruser { get; set; }
        public DbSet<GetfacreceiptitemidDTO> GetfacreceiptitemidDbSet { get; set; }
        public DbSet<GetInwardNoDTO> GetInwardNoDbSet { get; set; }
        public DbSet<GetFacilityReceiptIdDTO> GetFacilityReceiptIdDbSet { get; set; }
        public DbSet<GetFacilityReceiptItemIdDTO> GetFacilityReceiptItemIdDbSet { get; set; }
        public DbSet<GetItemCodeDTO> GetItemCodeDbSet { get; set; }
        public DbSet<GetFacilityCodeDTO> GetFacilityCodeDbSet { get; set; }
        public DbSet<GetHSccDTO> GetHSccDbSet { get; set; }
        public DbSet<GetWHsSerialNoDTO> GetWHsSerialNoDbSet { get; set; }
        public DbSet<GetFacReceiptIdDTO> GetFacReceiptIdDbSet { get; set; }
        public DbSet<GenWhIndentNoDTO> GenWhIndentNoDbSet { get; set; }
        public DbSet<GenrateReceiptIssueNoDTO> GenrateReceiptIssueNoDbSet { get; set; }
        public DbSet<IndentDataDTO> IndentDataDbSet { get; set; }
        public DbSet<masAccYearSettingsModel> masAccYearSettingsDbSet { get; set; }
        public DbSet<getCMHOfacfromDistDTO> getCMHOfacfromDistDbSet { get; set; }
        public DbSet<FacilityInfoDTO> FacilityInfoDbSet { get; set; }
        public DbSet<ExtractReceiptItemsDTO> ExtractReceiptItemsDbSet { get; set; }
        public DbSet<MasItemsDTO> MasItemsDbSet { get; set; }
        public DbSet<tbFacilityReceiptItemsDTO> tbFacilityReceiptItemsDbSet1 { get; set; }
        public DbSet<ProgressRecDTO> ProgressRecDbSet { get; set; }
        public DbSet<GetAbsrQtyDTO> GetAbsrQtyDbSet { get; set; }
        public DbSet<GetFileStorageLocationDTO> GetFileStorageLocationDbSet { get; set; }
        public DbSet<GetOpeningStocksRptDTO> GetOpeningStocksRptDbSet { get; set; }
        public DbSet<GetFacreceiptidForOpeningStockDTO> GetFacreceiptidForOpeningStockDbSet { get; set; }
        public DbSet<GetHeaderInfoDTO> GetHeaderInfoDbSet { get; set; }
        public DbSet<getFacilityOpeningsDTO> getFacilityOpeningsDbSet { get; set; }
        public DbSet<getIndentProgramDTO> getIndentProgramDbSet { get; set; }
        public DbSet<IncompleteWardIssueDTO> IncompleteWardIssueDTOs { get; set; }
        public DbSet<tbGenIndent> tbGenIndentDbSet { get; set; }
        public DbSet<FacMonthIndentDTO> FacMonthIndentDbSet { get; set; }
        public DbSet<CategoryDTO> CategoryDTODbSet { get; set; }
        public DbSet<IndentItemsFromWardDTO> IndentItemsFromWardDbSet { get; set; }
        public DbSet<IndentAlertNewDTO> IndentAlertNewDbSet { get; set; }
        public DbSet<MasCgmscNocItems> mascgmscnocitemsDbSet { get; set; }
        public DbSet<SavedFacIndentItemsDTO> SavedFacIndentItemsDbSet { get; set; }
        public DbSet<ReceiptMasterWHDTO> ReceiptMasterDbSet { get; set; }
        public DbSet<ReceiptMasterDTO> ReceiptMasterDTODbSet { get; set; }
        public DbSet<tbFacilityReceiptsModel> tbFacilityReceiptsDbSet { get; set; }
        public DbSet<SHCitemlistDTO> SHCitemlistDbSet { get; set; }
        public DbSet<FacilityInfoAamDTO> FacilityInfoAamDbSet { get; set; }
        public DbSet<VeriftyOtpDTO> VeriftyOtpDbSet { get; set; }
        public DbSet<ReceiptItemsDDL> ReceiptItemsDDLDbSet { get; set; }
        public DbSet<MasRackDTO> MasRackDbSet { get; set; }
        public DbSet<ReceiptDetailsDTO> ReceiptDetailsDbSet { get; set; }
        public DbSet<tbFacilityReceiptItemsModel> tbFacilityReceiptItemsDbSet { get; set; }
        public DbSet<tbFacilityReceiptBatchesModel> tbFacilityReceiptBatchesDbSet { get; set; }
        public DbSet<ReceipItemBatchesDTO> ReceipItemBatchesDbSet { get; set; }
        public DbSet<FacilityWardDTO> FacilityWardDTOs { get; set; }
        public DbSet<tbFacilityGenIssue> tbFacilityGenIssueDbSet { get; set; }
        public DbSet<WardIssueItemsDTO> WardIssueItemsDbSet { get; set; }
        public DbSet<ItemStockDTO> ItemStockDBSet { get; set; }
        public DbSet<tbFacilityIssueItems> tbFacilityIssueItems { get; set; }
        public DbSet<getIssueItemIdDTO> getIssueItemIdDbSet { get; set; }
        public DbSet<getBatchesDTO> getBatchesDbSet { get; set; }
        public DbSet<tbFacilityOutwardsModel> tbFacilityOutwardsDbSet { get; set; }
        public DbSet<LastIssueDTO> LastIssueDbSet { get; set; }
        public DbSet<getIndentTimlineDTO> getIndentTimlineDbSet { get; set; }
        public DbSet<FacilityIssueCurrentStockDTO> FacilityIssueCurrentStockDbSet { get; set; }
        public DbSet<getFacilityIssueBatchesDTO> getFacilityIssueBatchesDbSet { get; set; }
        public DbSet<GetOpningClosingItemDTO> GetOpningClosingItemDbSet { get; set; }
        public DbSet<GetReceiptItemsDTO> GetReceiptItemsDbSet { get; set; }
        public DbSet<GetBatchesOfReceiptDTO> GetBatchesOfReceiptDbSt { get; set; }
        public DbSet<getReceiptVouchersDTO> getReceiptVouchersDbSet { get; set; }
        public DbSet<getIssueVoucherPdfDTO> getIssueVoucherPdfDbSet { get; set; }
        public DbSet<GetWHStockItemsDTO> getGetWHStockItemsDbSet { get; set; }
        public DbSet<StockReportFacilityDTO> StockReportFacilityDTOs { get; set; }
        public DbSet<ParentPHCidDTO> ParentPHCidDbSet { get; set; }
        public DbSet<getSHCItemsDTO> getSHCItemsDbSet { get; set; }
        public DbSet<getReceiptByIdDTO> getReceiptByIdDbSet { get; set; }
        public DbSet<getDistrictAchivementStatusDTO> getDistrictAchivementStatusDbSet { get; set; }
        public DbSet<MASFACTRANSFERS> MASFACTRANSFERSDbSet { get; set; }
        public DbSet<getFacCodeForSHCIndentDTO> getFacCodeForSHCIndentDbSet { get; set; }
        public DbSet<getBlockFACsDTO> getBlockFACsDbSet { get; set; }
        public DbSet<LocationIDDTO> getLocationIDbSet { get; set; }
        public DbSet<MasFacDemandItems> MasFacDemandItemsDbSet { get; set; }
        public DbSet<OtherFacilityIndentDTO> OtherFacilityIndentDbSet { get; set; }
        public DbSet<getOtherFacIssueDetailsDTO> getOtherFacIssueDetailsDbSet { get; set; }
        public DbSet<OtherFacIndentDetailsDTO> OtherFacIndentDetailsDbSet { get; set; }
        public DbSet<OPStockCheckDTO> OPStockCheckDBset { get; set; }
        public DbSet<GetConsumptionDTO> GetConsumptionDbSet { get; set; }
        public DbSet<PasswordForgetDTO> PasswordForgetDbset { get; set; }
        public DbSet<UsruserModelConsultant> UsruserModelConsultantDbSet { get; set; }
        public DbSet<getLastIssueDT_DTO> getLastIssueDT_DbSet { get; set; }
        public DbSet<getFacilityItemWiseStockDTO> getFacilityItemWiseStockDbSet { get; set; }
        public DbSet<getFacilityWiseIssueDTO> getFacilityWiseIssueDbSet { get; set; }
        public DbSet<KPIdistWiseDTO> KPIdistWiseDbSet { get; set; }
        public DbSet<KPIFacWiseDTO> KPIFacWiseDbSet { get; set; }
        public DbSet<NearExpBatchDTO> NearExpBatchDTODBSet { get; set; }
        public DbSet<getOtherFacilityIssueDTO> getOtherFacilityIssueDbSet { get; set; }
        public DbSet<DistrictWiseAamHealthPerformanceDTO> DistrictWiseAamHealthPerformanceDbSet { get; set; }
        public DbSet<KPIFacilityDetailDTO> KPIFacilityDetailDbSet { get; set; }










        //Data Transfer DTO's Start

        public DbSet<GetAllFacilityTypesDTO> GetAllFacilityTypesDbSet { get; set; } // Replace MariaDbEntity with your actual entity
        public DbSet<DmasitemsDTO> DmasitemsDbSet { get; set; }
        public DbSet<DmasdistrictsDTO> DmasdistrictsDbSet { get; set; }
        public DbSet<DmasitemtypesDTO> DmasitemtypesDbSet { get; set; }
        public DbSet<DmasitemcategoriesDTO> DmasitemcategoriesDbSet { get; set; }
        public DbSet<DmasfacilitytypesDTO> DmasfacilitytypesDbSet { get; set; }
        public DbSet<DmasfacilitiesDTO> DmasfacilitiesDbSet { get; set; }
        public DbSet<DusrusersDTO> DusrusersDbSet { get; set; }
        public DbSet<DmasfacilitywhDTO> DmasfacilitywhDbSet { get; set; }
        public DbSet<DmaswarehousesDTO> DmaswarehousesDbSet { get; set; }
        public DbSet<DmasitemmaincategoryDTO> DmasitemmaincategoryDbSet { get; set; }
        public DbSet<DmasitemgroupsDTO> DmasitemgroupsDbSet { get; set; }
        public DbSet<DmasfacilitywardsDTO> DmasfacilitywardsDbSet { get; set; }
        public DbSet<DmasprogramDTO> DmasprogramDbSet { get; set; }
        public DbSet<DmasaccyearsettingsDTO> DmasaccyearsettingsDbSet { get; set; }
        public DbSet<DmaslocationsDTO> DmaslocationsDbSet { get; set; }
        public DbSet<DusrrolesDTO> DusrrolesDbSet { get; set; }
        public DbSet<DmasedlDTO> DmasedlDbSet { get; set; }
        public DbSet<DmasfacheaderfooterDTO> DmasfacheaderfooterDbSet { get; set; }
        public DbSet<MDB_mascgmscnocDTO> MDB_mascgmscnocDbSet { get; set; }
        public DbSet<MariaIntegration_MasCgmscNocItemsDTO> Integration_MasCgmscNocItemsDbSet { get; set; }
        public DbSet<MASFACTRANSFERS_DTO> MASFACTRANSFERS_DbSet { get; set; }
        public DbSet<KPIFacWiseDrillDownDTO> KPIFacWiseDrillDownDbSet { get; set; }
        public DbSet<MasFacIssueBarcodeDto> MasFacIssueBarcodeDbSet { get; set; }

        public DbSet<BarcodeReceiptDto> BarcodeReceiptDtoDbSet { get; set; }






        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<getFacilityItemWiseStockDTO>().HasNoKey();
            modelBuilder.Entity<GetBatchesOfReceiptDTO>().HasNoKey();
            modelBuilder.Entity<UsruserModel>().HasNoKey();
            modelBuilder.Entity<GetfacreceiptitemidDTO>().HasNoKey();
            modelBuilder.Entity<GetInwardNoDTO>().HasNoKey();
            modelBuilder.Entity<GetFacilityReceiptIdDTO>().HasNoKey();
            modelBuilder.Entity<GetFacilityReceiptItemIdDTO>().HasNoKey();
            modelBuilder.Entity<GetItemCodeDTO>().HasNoKey();
            modelBuilder.Entity<GetFacilityCodeDTO>().HasNoKey();
            modelBuilder.Entity<GetHSccDTO>().HasNoKey();
            modelBuilder.Entity<GetWHsSerialNoDTO>().HasNoKey();
            modelBuilder.Entity<GetFacReceiptIdDTO>().HasNoKey();
            modelBuilder.Entity<GenWhIndentNoDTO>().HasNoKey();
            modelBuilder.Entity<GenrateReceiptIssueNoDTO>().HasNoKey();
            modelBuilder.Entity<IndentDataDTO>().HasNoKey();
            modelBuilder.Entity<masAccYearSettingsModel>().HasNoKey();
            modelBuilder.Entity<getCMHOfacfromDistDTO>().HasNoKey();
            modelBuilder.Entity<FacilityInfoDTO>().HasNoKey();
            modelBuilder.Entity<ExtractReceiptItemsDTO>().HasNoKey();
            modelBuilder.Entity<MasItemsDTO>().HasNoKey();
            modelBuilder.Entity<tbFacilityReceiptItemsDTO>().HasNoKey();
            modelBuilder.Entity<ProgressRecDTO>().HasNoKey();
            modelBuilder.Entity<GetAbsrQtyDTO>().HasNoKey();
            modelBuilder.Entity<GetFileStorageLocationDTO>().HasNoKey();
            modelBuilder.Entity<GetOpeningStocksRptDTO>().HasNoKey();
            modelBuilder.Entity<GetFacreceiptidForOpeningStockDTO>().HasNoKey();
            modelBuilder.Entity<GetHeaderInfoDTO>().HasNoKey();
            modelBuilder.Entity<getFacilityOpeningsDTO>().HasNoKey();
            modelBuilder.Entity<getIndentProgramDTO>().HasNoKey();
            modelBuilder.Entity<IncompleteWardIssueDTO>().HasNoKey();
            //modelBuilder.Entity<tbGenIndent>().HasNoKey();
            modelBuilder.Entity<FacMonthIndentDTO>().HasNoKey();
            modelBuilder.Entity<CategoryDTO>().HasNoKey();
            modelBuilder.Entity<IndentItemsFromWardDTO>().HasNoKey();
            modelBuilder.Entity<IndentAlertNewDTO>().HasNoKey();
            //modelBuilder.Entity<MasCgmscNocItems>().HasNoKey();
            modelBuilder.Entity<SavedFacIndentItemsDTO>().HasNoKey();
           // modelBuilder.Entity<ReceiptMasterWHDTO>().HasNoKey();
            modelBuilder.Entity<ReceiptMasterDTO>().HasNoKey();
           // modelBuilder.Entity<tbFacilityReceiptsModel>().HasNoKey();
            modelBuilder.Entity<SHCitemlistDTO>().HasNoKey();
            modelBuilder.Entity<FacilityInfoAamDTO>().HasNoKey();
            modelBuilder.Entity<VeriftyOtpDTO>().HasNoKey();
            modelBuilder.Entity<ReceiptItemsDDL>().HasNoKey();
            modelBuilder.Entity<MasRackDTO>().HasNoKey();
            modelBuilder.Entity<ReceiptDetailsDTO>().HasNoKey();
           // modelBuilder.Entity<tbFacilityReceiptItemsModel>().HasNoKey();
           // modelBuilder.Entity<tbFacilityReceiptBatchesModel>().HasNoKey();
            modelBuilder.Entity<ReceipItemBatchesDTO>().HasNoKey();
            modelBuilder.Entity<FacilityWardDTO>().HasNoKey();
           // modelBuilder.Entity<tbFacilityGenIssue>().HasNoKey();
            modelBuilder.Entity<WardIssueItemsDTO>().HasNoKey();
            modelBuilder.Entity<ItemStockDTO>().HasNoKey();
            //modelBuilder.Entity<tbFacilityIssueItems>().HasNoKey();
            modelBuilder.Entity<getIssueItemIdDTO>().HasNoKey();
            modelBuilder.Entity<getBatchesDTO>().HasNoKey();
            modelBuilder.Entity<tbFacilityOutwardsModel>().HasNoKey();
            modelBuilder.Entity<LastIssueDTO>().HasNoKey();
            modelBuilder.Entity<getIndentTimlineDTO>().HasNoKey();
            modelBuilder.Entity<FacilityIssueCurrentStockDTO>().HasNoKey();
            modelBuilder.Entity<getFacilityIssueBatchesDTO>().HasNoKey();
            modelBuilder.Entity<GetOpningClosingItemDTO>().HasNoKey();
            modelBuilder.Entity<GetReceiptItemsDTO>().HasNoKey();
            modelBuilder.Entity<GetBatchesOfReceiptDTO>().HasNoKey();
            modelBuilder.Entity<getReceiptVouchersDTO>().HasNoKey();
            modelBuilder.Entity<getIssueVoucherPdfDTO>().HasNoKey();
            modelBuilder.Entity<GetWHStockItemsDTO>().HasNoKey();
            modelBuilder.Entity<StockReportFacilityDTO>().HasNoKey();
            modelBuilder.Entity<ParentPHCidDTO>().HasNoKey();
            modelBuilder.Entity<getSHCItemsDTO>().HasNoKey();
            modelBuilder.Entity<getReceiptByIdDTO>().HasNoKey();
            modelBuilder.Entity<getDistrictAchivementStatusDTO>().HasNoKey();
            //modelBuilder.Entity<MASFACTRANSFERS>().HasNoKey();
            modelBuilder.Entity<getFacCodeForSHCIndentDTO>().HasNoKey();
            modelBuilder.Entity<getBlockFACsDTO>().HasNoKey();
            modelBuilder.Entity<LocationIDDTO>().HasNoKey();
            //modelBuilder.Entity<MasFacDemandItems>().HasNoKey();
            modelBuilder.Entity<OtherFacilityIndentDTO>().HasNoKey();
            modelBuilder.Entity<getOtherFacIssueDetailsDTO>().HasNoKey();
            modelBuilder.Entity<OtherFacIndentDetailsDTO>().HasNoKey();
            modelBuilder.Entity<OPStockCheckDTO>().HasNoKey();
            modelBuilder.Entity<GetConsumptionDTO>().HasNoKey();
            modelBuilder.Entity<PasswordForgetDTO>().HasNoKey();
            modelBuilder.Entity<UsruserModelConsultant>().HasNoKey();
            modelBuilder.Entity<getLastIssueDT_DTO>().HasNoKey();
            modelBuilder.Entity<getFacilityItemWiseStockDTO>().HasNoKey();
            modelBuilder.Entity<getFacilityWiseIssueDTO>().HasNoKey();
            modelBuilder.Entity<KPIdistWiseDTO>().HasNoKey();
            modelBuilder.Entity<KPIFacWiseDTO>().HasNoKey();
            modelBuilder.Entity<DistrictWiseAamHealthPerformanceDTO>().HasNoKey();
            modelBuilder.Entity<KPIFacWiseDrillDownDTO>().HasNoKey();
            modelBuilder.Entity<KPIFacilityDetailDTO>().HasNoKey();
            modelBuilder.Entity<MasFacIssueBarcodeDto>().HasNoKey();

            modelBuilder.Entity<BarcodeReceiptDto>().HasNoKey();
            //  modelBuilder.Entity<MASFACTRANSFERS_DTO>().HasKey();

            //  modelBuilder.Entity<MDB_mascgmscnocDTO>().HasBaseType<tbGenIndent>();

        }











    }
}
