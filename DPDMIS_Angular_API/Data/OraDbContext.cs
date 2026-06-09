//using DPDMIS_Angular_API.DTO.AAMAdminDTO;
//using DPDMIS_Angular_API.DTO.CGMSCStockDTO;
//using DPDMIS_Angular_API.DTO.FacilityDTO;
//using DPDMIS_Angular_API.DTO.IndentDTO;
//using DPDMIS_Angular_API.DTO.IssueDTO;
//using DPDMIS_Angular_API.DTO.LoginDTO;
//using DPDMIS_Angular_API.DTO.ReceiptDTO;
//using DPDMIS_Angular_API.DTO.SourceDTO;
//using DPDMIS_Angular_API.MariadbDTO.DestinationDTO;
//using DPDMIS_Angular_API.Models;
using DPDMIS_Angular_API.OraDTO.OraCGMSCStockDTO;
using DPDMIS_Angular_API.OraDTO.OraFacilityDTO;
using DPDMIS_Angular_API.OraDTO.OraIndentDTO;
using DPDMIS_Angular_API.OraDTO.OraIssueDTO;
using DPDMIS_Angular_API.OraDTO.OraModels;
using DPDMIS_Angular_API.OraDTO.OraReceiptDTO;
using Microsoft.EntityFrameworkCore;

namespace DPDMIS_Angular_API.Data
{
    public class OraDbContext : DbContext
    {
        public OraDbContext(DbContextOptions<OraDbContext> options) : base(options)
        {
        }     
        public DbSet<masAccYearSettingsModel> masAccYearSettingsDbSet { get; set; }
      
        public DbSet<OraMASFACTRANSFERS> OraMASFACTRANSFERSDbSet { get; set; }
        public DbSet<getFacCodeForSHCIndentDTO> getFacCodeForSHCIndentDbSet { get; set; }
     
        //public DbSet<OraMasFacDemandItemsDTO> OraMasFacDemandItemsDbSet { get; set; }
        //public DbSet<ORAMASFACTRANSFERS_DTO> ORAMASFACTRANSFERS_DbSet { get; set; }
        //public DbSet<ORAMASFACTRANSFERSINDENT_DTO> ORAMASFACTRANSFERSINDENT_DbSet { get; set; }

        public DbSet<OraBarcodeReceiptDto> OraBarcodeReceiptDtoDbSet { get; set; }

        ////Data Transfer DTO's
        //public DbSet<SmasitemsDTO> SmasitemsDbSet { get; set; }
        //public DbSet<SmasdistrictsDTO> SmasdistrictsDbSet { get; set; }
        //public DbSet<SmasitemtypesDTO> SmasitemtypesDbSet { get; set; }
        //public DbSet<SmasitemcategoriesDTO> SmasitemcategoriesDbSet { get; set; }
        //public DbSet<SmasfacilitytypesDTO> SmasfacilitytypesDbSet { get; set; }
        //public DbSet<SmasfacilitiesDTO> SmasfacilitiesDbSet { get; set; }
        //public DbSet<SusrusersDTO> SusrusersDbSet { get; set; }
        //public DbSet<SmasfacilitywhDTO> SmasfacilitywhDbSet { get; set; }
        //public DbSet<SmaswarehousesDTO> SmaswarehousesDbSet { get; set; }
        //public DbSet<SmasitemmaincategoryDTO> SmasitemmaincategoryDbSet { get; set; }
        //public DbSet<SmasitemgroupsDTO> SmasitemgroupsDbSet { get; set; }
        //public DbSet<SmasfacilitywardsDTO> SmasfacilitywardsDbSet { get; set; }
        //public DbSet<SmasprogramDTO> SmasprogramDbSet { get; set; }
        //public DbSet<SmasaccyearsettingsDTO> SmasaccyearsettingsDbSet { get; set; }
        //public DbSet<SmaslocationsDTO> SmaslocationsDbSet { get; set; }
        //public DbSet<SusrrolesDTO> SusrrolesDbSet { get; set; }
        //public DbSet<SmasedlDTO> SmasedlDbSet { get; set; }
        //public DbSet<SmasfacheaderfooterDTO> SmasfacheaderfooterDbSet { get; set; }
        //public DbSet<ORA_mascgmscnocDTO> ORA_mascgmscnocDbSet { get; set; }
        //public DbSet<ORAIntegration_MasCgmscNocItemsDTO> ORAIntegration_MasCgmscNocItemsDbSet { get; set; }
        //public DbSet<ValueMascgmscnocDTO> ValueMascgmscnocDbSet { get; set; }
        //public DbSet<BarcodeReceiptDto> BarcodeReceiptDto { get; set; }
        //public DbSet<GetBarcodeDetailsDto> GetBarcodeDetailsDbSet { get; set; }



        // OraIssueController DTO DbSets
        public DbSet<IncompleteWardIssueDTO> IncompleteWardIssueDTOs { get; set; }
        public DbSet<FacilityWardDTO> FacilityWardDTOs { get; set; }
        public DbSet<tbFacilityGenIssue> tbFacilityGenIssueDbSet { get; set; }

        public DbSet<LastIssueDTO> LastIssueDbSet { get; set; }
        public DbSet<WardIssueItemsDTO> WardIssueItemsDbSet { get; set; }
        public DbSet<ItemStockDTO> ItemStockDBSet { get; set; }

        public DbSet<tbFacilityIssueItems> tbFacilityIssueItems { get; set; }

        public DbSet<getIssueItemIdDTO> getIssueItemIdDbSet { get; set; }
        public DbSet<getBatchesDTO> getBatchesDbSet { get; set; }

        public DbSet<tbFacilityOutwardsModel> tbFacilityOutwardsDbSet { get; set; }

        public DbSet<FacilityIssueCurrentStockDTO> FacilityIssueCurrentStockDbSet { get; set; }

        public DbSet<getFacilityIssueBatchesDTO> getFacilityIssueBatchesDbSet { get; set; }

        public DbSet<getIssueVoucherPdfDTO> getIssueVoucherPdfDbSet { get; set; }
        public DbSet<GenrateReceiptIssueNoDTO> GenrateReceiptIssueNoDbSet { get; set; }

        public DbSet<IndentDataDTO> IndentDataDbSet { get; set; }
        public DbSet<FacilityInfoDTO> FacilityInfoDbSet { get; set; }
        public DbSet<OraGetFacreceiptidForOpeningStockDTO> OraGetFacreceiptidForOpeningStockDbSet { get; set; }
        public DbSet<OraGetBarcodeDetailsDto> OraGetBarcodeDetailsDbSet { get; set; }
        public DbSet<OraOraMasFacDemandItemsDTO> OraOraMasFacDemandItemsDbSet { get; set; }
        


        // Testing Only
        // public DbSet<tbFacilityGenIssue> tbFacilityGenIssueDbSet { get; set; }
        // public DbSet<tbFacilityIssueItems> tbFacilityIssueItems { get; set; }
        // public DbSet<tbFacilityOutwardsModel> tbFacilityOutwardsDbSet { get; set; }






        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //modelBuilder.Entity<GetAbsrQtyDTO>().HasNoKey();
            //modelBuilder.Entity<GetFacreceiptidForOpeningStockDTO>().HasNoKey();
            //modelBuilder.Entity<getFacilityOpeningsDTO>().HasNoKey();
            //modelBuilder.Entity<ReceipItemBatchesDTO>().HasNoKey();
            modelBuilder.Entity<LastIssueDTO>().HasNoKey();
            //modelBuilder.Entity<GetOpningClosingItemDTO>().HasNoKey();
            //modelBuilder.Entity<GetBatchesOfReceiptDTO>().HasNoKey();
            //modelBuilder.Entity<getReceiptVouchersDTO>().HasNoKey();
            //modelBuilder.Entity<getReceiptByIdDTO>().HasNoKey();
            //modelBuilder.Entity<getFacilityItemWiseStockDTO>().HasNoKey();
            //modelBuilder.Entity<BarcodeReceiptDto>().HasNoKey();
            //modelBuilder.Entity<GetBarcodeDetailsDto>().HasNoKey();


            modelBuilder.Entity<IncompleteWardIssueDTO>().HasNoKey();
            modelBuilder.Entity<FacilityWardDTO>().HasNoKey();
            modelBuilder.Entity<LastIssueDTO>().HasNoKey();
            modelBuilder.Entity<WardIssueItemsDTO>().HasNoKey();
            modelBuilder.Entity<ItemStockDTO>().HasNoKey();
            modelBuilder.Entity<getIssueItemIdDTO>().HasNoKey();
            modelBuilder.Entity<getBatchesDTO>().HasNoKey();
            modelBuilder.Entity<FacilityIssueCurrentStockDTO>().HasNoKey();
            modelBuilder.Entity<getFacilityIssueBatchesDTO>().HasNoKey();
            modelBuilder.Entity<getIssueVoucherPdfDTO>().HasNoKey();
            modelBuilder.Entity<OraGetFacreceiptidForOpeningStockDTO>().HasNoKey();
            modelBuilder.Entity<OraGetBarcodeDetailsDto>().HasNoKey();

            // Uncomment only for testing if EF throws Primary Key errors

            // modelBuilder.Entity<tbFacilityGenIssue>().HasNoKey();
            // modelBuilder.Entity<tbFacilityIssueItems>().HasNoKey();
            // modelBuilder.Entity<tbFacilityOutwardsModel>().HasNoKey();



            //  modelBuilder.Entity<MasCgmscNocItems>().ToTable("MasCgmscNocItems");
        }




    }
}
