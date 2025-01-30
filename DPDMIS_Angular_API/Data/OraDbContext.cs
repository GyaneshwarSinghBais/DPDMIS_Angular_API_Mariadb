using DPDMIS_Angular_API.DTO.AAMAdminDTO;
using DPDMIS_Angular_API.DTO.CGMSCStockDTO;
using DPDMIS_Angular_API.DTO.FacilityDTO;
using DPDMIS_Angular_API.DTO.IndentDTO;
using DPDMIS_Angular_API.DTO.IssueDTO;
using DPDMIS_Angular_API.DTO.LoginDTO;
using DPDMIS_Angular_API.DTO.ReceiptDTO;
using DPDMIS_Angular_API.DTO.SourceDTO;
using DPDMIS_Angular_API.MariadbDTO.DestinationDTO;
using DPDMIS_Angular_API.Models;
using Microsoft.EntityFrameworkCore;

namespace DPDMIS_Angular_API.Data
{
    public class OraDbContext : DbContext
    {
        public OraDbContext(DbContextOptions<OraDbContext> options) : base(options)
        {
        }

        //public DbSet<UsruserModel> Usruser { get; set; }
        //public DbSet<GetfacreceiptitemidDTO> GetfacreceiptitemidDbSet { get; set; }
        //public DbSet<GetInwardNoDTO> GetInwardNoDbSet { get; set; }
        //public DbSet<GetFacilityReceiptIdDTO> GetFacilityReceiptIdDbSet { get; set; }
        //public DbSet<GetFacilityReceiptItemIdDTO> GetFacilityReceiptItemIdDbSet { get; set; }
        //public DbSet<GetItemCodeDTO> GetItemCodeDbSet { get; set; }
        //public DbSet<GetFacilityCodeDTO> GetFacilityCodeDbSet { get; set; }
        //public DbSet<GetHSccDTO> GetHSccDbSet { get; set; }
        //public DbSet<GetWHsSerialNoDTO> GetWHsSerialNoDbSet { get; set; }
        //public DbSet<GetFacReceiptIdDTO> GetFacReceiptIdDbSet { get; set; }
        //public DbSet<GenWhIndentNoDTO> GenWhIndentNoDbSet { get; set; }
        //public DbSet<GenrateReceiptIssueNoDTO> GenrateReceiptIssueNoDbSet { get; set; }
        //public DbSet<IndentDataDTO> IndentDataDbSet { get; set; }
        public DbSet<masAccYearSettingsModel> masAccYearSettingsDbSet { get; set; }
        //public DbSet<getCMHOfacfromDistDTO> getCMHOfacfromDistDbSet { get; set; }
        //public DbSet<FacilityInfoDTO> FacilityInfoDbSet { get; set; }
        //public DbSet<ExtractReceiptItemsDTO> ExtractReceiptItemsDbSet { get; set; }
        //public DbSet<MasItemsDTO> MasItemsDbSet { get; set; }
        //public DbSet<tbFacilityReceiptItemsDTO> tbFacilityReceiptItemsDbSet1 { get; set; }
        //public DbSet<ProgressRecDTO> ProgressRecDbSet { get; set; }
        //public DbSet<GetAbsrQtyDTO> GetAbsrQtyDbSet { get; set; }
        //public DbSet<GetFileStorageLocationDTO> GetFileStorageLocationDbSet { get; set; }
        //public DbSet<GetOpeningStocksRptDTO> GetOpeningStocksRptDbSet { get; set; }
        //public DbSet<GetFacreceiptidForOpeningStockDTO> GetFacreceiptidForOpeningStockDbSet { get; set; }
        //public DbSet<GetHeaderInfoDTO> GetHeaderInfoDbSet { get; set; }
        //public DbSet<getFacilityOpeningsDTO> getFacilityOpeningsDbSet { get; set; }
        //public DbSet<getIndentProgramDTO> getIndentProgramDbSet { get; set; }
        //public DbSet<IncompleteWardIssueDTO> IncompleteWardIssueDTOs { get; set; }
        //public DbSet<tbGenIndent> tbGenIndentDbSet { get; set; }
        //public DbSet<FacMonthIndentDTO> FacMonthIndentDbSet { get; set; }
        //public DbSet<CategoryDTO> CategoryDTODbSet { get; set; }
        //public DbSet<IndentItemsFromWardDTO> IndentItemsFromWardDbSet { get; set; }
        //public DbSet<IndentAlertNewDTO> IndentAlertNewDbSet { get; set; }
        //public DbSet<MasCgmscNocItems> mascgmscnocitemsDbSet { get; set; }
        //public DbSet<SavedFacIndentItemsDTO> SavedFacIndentItemsDbSet { get; set; }
        //public DbSet<ReceiptMasterWHDTO> ReceiptMasterDbSet { get; set; }
        //public DbSet<ReceiptMasterDTO> ReceiptMasterDTODbSet { get; set; }
        //public DbSet<tbFacilityReceiptsModel> tbFacilityReceiptsDbSet { get; set; }
        //public DbSet<SHCitemlistDTO> SHCitemlistDbSet { get; set; }
        //public DbSet<FacilityInfoAamDTO> FacilityInfoAamDbSet { get; set; }
        //public DbSet<VeriftyOtpDTO> VeriftyOtpDbSet { get; set; }
        //public DbSet<ReceiptItemsDDL> ReceiptItemsDDLDbSet { get; set; }
        //public DbSet<MasRackDTO> MasRackDbSet { get; set; }
        //public DbSet<ReceiptDetailsDTO> ReceiptDetailsDbSet { get; set; }
        //public DbSet<tbFacilityReceiptItemsModel> tbFacilityReceiptItemsDbSet { get; set; }
        //public DbSet<tbFacilityReceiptBatchesModel> tbFacilityReceiptBatchesDbSet { get; set; }
        //public DbSet<ReceipItemBatchesDTO> ReceipItemBatchesDbSet { get; set; }
        //public DbSet<FacilityWardDTO> FacilityWardDTOs { get; set; }
        //public DbSet<tbFacilityGenIssue> tbFacilityGenIssueDbSet { get; set; }
        //public DbSet<WardIssueItemsDTO> WardIssueItemsDbSet { get; set; }
        //public DbSet<ItemStockDTO> ItemStockDBSet { get; set; }
        //public DbSet<tbFacilityIssueItems> tbFacilityIssueItems { get; set; }
        //public DbSet<getIssueItemIdDTO> getIssueItemIdDbSet { get; set; }
        //public DbSet<getBatchesDTO> getBatchesDbSet { get; set; }
        //public DbSet<tbFacilityOutwardsModel> tbFacilityOutwardsDbSet { get; set; }
        //public DbSet<LastIssueDTO> LastIssueDbSet { get; set; }
        //public DbSet<getIndentTimlineDTO> getIndentTimlineDbSet { get; set; }
        //public DbSet<FacilityIssueCurrentStockDTO> FacilityIssueCurrentStockDbSet { get; set; }
        //public DbSet<getFacilityIssueBatchesDTO> getFacilityIssueBatchesDbSet { get; set; }
        //public DbSet<GetOpningClosingItemDTO> GetOpningClosingItemDbSet { get; set; }
        //public DbSet<GetReceiptItemsDTO> GetReceiptItemsDbSet { get; set; }
        //public DbSet<GetBatchesOfReceiptDTO> GetBatchesOfReceiptDbSt { get; set; }
        //public DbSet<getReceiptVouchersDTO> getReceiptVouchersDbSet { get; set; }
        //public DbSet<getIssueVoucherPdfDTO> getIssueVoucherPdfDbSet { get; set; }
        //public DbSet<GetWHStockItemsDTO> getGetWHStockItemsDbSet { get; set; }
        //public DbSet<StockReportFacilityDTO> StockReportFacilityDTOs { get; set; }
        //public DbSet<ParentPHCidDTO> ParentPHCidDbSet { get; set; }
        //public DbSet<getSHCItemsDTO> getSHCItemsDbSet { get; set; }
        //public DbSet<getReceiptByIdDTO> getReceiptByIdDbSet { get; set; }
        //public DbSet<getDistrictAchivementStatusDTO> getDistrictAchivementStatusDbSet { get; set; }
        public DbSet<MASFACTRANSFERS> MASFACTRANSFERSDbSet { get; set; }
        public DbSet<getFacCodeForSHCIndentDTO> getFacCodeForSHCIndentDbSet { get; set; }
        //public DbSet<getBlockFACsDTO> getBlockFACsDbSet { get; set; }
        //public DbSet<LocationIDDTO> getLocationIDbSet { get; set; }
        //public DbSet<MasFacDemandItems> MasFacDemandItemsDbSet { get; set; }
        //public DbSet<OtherFacilityIndentDTO> OtherFacilityIndentDbSet { get; set; }
        //public DbSet<getOtherFacIssueDetailsDTO> getOtherFacIssueDetailsDbSet { get; set; }
        //public DbSet<OtherFacIndentDetailsDTO> OtherFacIndentDetailsDbSet { get; set; }
        //public DbSet<OPStockCheckDTO> OPStockCheckDBset { get; set; }
        //public DbSet<GetConsumptionDTO> GetConsumptionDbSet { get; set; }
        //public DbSet<PasswordForgetDTO> PasswordForgetDbset { get; set; }
        //public DbSet<UsruserModelConsultant> UsruserModelConsultantDbSet { get; set; }
        //public DbSet<getLastIssueDT_DTO> getLastIssueDT_DbSet { get; set; }
        //public DbSet<getFacilityItemWiseStockDTO> getFacilityItemWiseStockDbSet { get; set; }
        //public DbSet<getFacilityWiseIssueDTO> getFacilityWiseIssueDbSet { get; set; }
        //public DbSet<KPIdistWiseDTO> KPIdistWiseDbSet { get; set; }
        //public DbSet<KPIFacWiseDTO> KPIFacWiseDbSet { get; set; }
        public DbSet<OraMasFacDemandItemsDTO> OraMasFacDemandItemsDbSet { get; set; }
        public DbSet<ORAMASFACTRANSFERS_DTO> ORAMASFACTRANSFERS_DbSet { get; set; }
        public DbSet<ORAMASFACTRANSFERSINDENT_DTO> ORAMASFACTRANSFERSINDENT_DbSet { get; set; }
        









        //Data Transfer DTO's
        public DbSet<SmasitemsDTO> SmasitemsDbSet { get; set; }
        public DbSet<SmasdistrictsDTO> SmasdistrictsDbSet { get; set; }
        public DbSet<SmasitemtypesDTO> SmasitemtypesDbSet { get; set; }
        public DbSet<SmasitemcategoriesDTO> SmasitemcategoriesDbSet { get; set; }
        public DbSet<SmasfacilitytypesDTO> SmasfacilitytypesDbSet { get; set; }
        public DbSet<SmasfacilitiesDTO> SmasfacilitiesDbSet { get; set; }
        public DbSet<SusrusersDTO> SusrusersDbSet { get; set; }
        public DbSet<SmasfacilitywhDTO> SmasfacilitywhDbSet { get; set; }
        public DbSet<SmaswarehousesDTO> SmaswarehousesDbSet { get; set; }
        public DbSet<SmasitemmaincategoryDTO> SmasitemmaincategoryDbSet { get; set; }
        public DbSet<SmasitemgroupsDTO> SmasitemgroupsDbSet { get; set; }
        public DbSet<SmasfacilitywardsDTO> SmasfacilitywardsDbSet { get; set; }
        public DbSet<SmasprogramDTO> SmasprogramDbSet { get; set; }
        public DbSet<SmasaccyearsettingsDTO> SmasaccyearsettingsDbSet { get; set; }
        public DbSet<SmaslocationsDTO> SmaslocationsDbSet { get; set; }
        public DbSet<SusrrolesDTO> SusrrolesDbSet { get; set; }
        public DbSet<SmasedlDTO> SmasedlDbSet { get; set; }
        public DbSet<SmasfacheaderfooterDTO> SmasfacheaderfooterDbSet { get; set; }
        public DbSet<ORA_mascgmscnocDTO> ORA_mascgmscnocDbSet { get; set; }
        public DbSet<ORAIntegration_MasCgmscNocItemsDTO> ORAIntegration_MasCgmscNocItemsDbSet { get; set; }
        public DbSet<ValueMascgmscnocDTO> ValueMascgmscnocDbSet { get; set; }
        






























        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<GetAbsrQtyDTO>().HasNoKey();
            modelBuilder.Entity<GetFacreceiptidForOpeningStockDTO>().HasNoKey();
            modelBuilder.Entity<getFacilityOpeningsDTO>().HasNoKey();
            modelBuilder.Entity<ReceipItemBatchesDTO>().HasNoKey();
            modelBuilder.Entity<LastIssueDTO>().HasNoKey();
            modelBuilder.Entity<GetOpningClosingItemDTO>().HasNoKey();
            modelBuilder.Entity<GetBatchesOfReceiptDTO>().HasNoKey();
            modelBuilder.Entity<getReceiptVouchersDTO>().HasNoKey();
            modelBuilder.Entity<getReceiptByIdDTO>().HasNoKey();
            modelBuilder.Entity<getFacilityItemWiseStockDTO>().HasNoKey();
            


            //  modelBuilder.Entity<MasCgmscNocItems>().ToTable("MasCgmscNocItems");
        }




    }
}
