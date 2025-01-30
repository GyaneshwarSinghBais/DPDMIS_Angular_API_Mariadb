using DPDMIS_Angular_API.Data;
using DPDMIS_Angular_API.DTO.SourceDTO;
using DPDMIS_Angular_API.MariadbDTO.DestinationDTO;
using DPDMIS_Angular_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Runtime.CompilerServices;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DPDMIS_Angular_API.Services.IntegrationService
{
    public class DataTransferService
    {
        private readonly OraDbContext _oracleContext;
        private readonly MariaDbContext _mariaContext;

        public DataTransferService(OraDbContext oracleContext, MariaDbContext mariaContext)
        {
            _oracleContext = oracleContext;
            _mariaContext = mariaContext;
        }

        public async Task TransferDataAsync_masitems()
        {
            // Step 1: Fetch data from Oracle

            //var oracleData = await _oracleContext.SmasitemsDbSet.ToListAsync();

            var oracleData = await _oracleContext.SmasitemsDbSet
    .Where(item => item.SHC == "Y")
    .ToListAsync();


            // Step 2: Map/Transform data (if needed)
            var mariaData = oracleData.Select(item => new DmasitemsDTO
            {
                ITEMID = item.ITEMID,
                ITEMCODE = item.ITEMCODE,
                ITEMNAME = item.ITEMNAME,
                ITEMTYPEID = item.ITEMTYPEID,
                CATEGORYID = item.CATEGORYID,
                GROUPID = item.GROUPID,
                UNIT = item.UNIT,
                STOCKSTATUSID = item.STOCKSTATUSID,
                VENID = item.VENID,
                STRENGTH1 = item.STRENGTH1,
                STRENGTH2 = item.STRENGTH2,
                PACKING1 = item.PACKING1,
                PACKING2 = item.PACKING2,
                PACKCARTON = item.PACKCARTON,
                PACKINGQTY = item.PACKINGQTY,
                QTYIND = item.QTYIND,
                QCTEST = item.QCTEST,
                TESTQTY = item.TESTQTY,
                EXPINDICATID = item.EXPINDICATID,
                ACTIVEFLAG = item.ACTIVEFLAG,
                RATE = item.RATE,
                QCMINQTY = item.QCMINQTY,
                MINLVL = item.MINLVL,
                MAXLVL = item.MAXLVL,
                REORDLVL = item.REORDLVL,
                EMERLVL = item.EMERLVL,
                REF_REQ = item.REF_REQ,
                SUBGROUPID = item.SUBGROUPID,
                HIGHLIGHTNULLBATCHES = item.HIGHLIGHTNULLBATCHES,
                UNSPSCID = item.UNSPSCID,
                ECRIID = item.ECRIID,
                DGSDID = item.DGSDID,
                GS1PRODUCTCODE = item.GS1PRODUCTCODE,
                ITEMCLASSIFICATION = item.ITEMCLASSIFICATION,
                REFITEMID = item.REFITEMID,
                BRANDNAME = item.BRANDNAME,
                ISEDL = item.ISEDL,
                SHELFLIFEINMONTHS = item.SHELFLIFEINMONTHS,
                SHELFLIFEINPERCENT = item.SHELFLIFEINPERCENT,
                PHARMACOPOEIA = item.PHARMACOPOEIA,
                COMPOSITION = item.COMPOSITION,
                UNITCOUNT = item.UNITCOUNT,
                MULTIPLE = item.MULTIPLE,
                QCTESTINGESTDAY = item.QCTESTINGESTDAY,
                EDLCAT = item.EDLCAT,
                ITEMNAME_HINDI = item.ITEMNAME_HINDI,
                STRENGTH_HINDI = item.STRENGTH_HINDI,
                TREAT_ID = item.TREAT_ID,
                HSNCODE = item.HSNCODE,
                GSTRATE = item.GSTRATE,
                ROWNO = item.ROWNO,
                EDLITEMID = item.EDLITEMID,
                ISMITANINDRUG = item.ISMITANINDRUG,
                GDT_ENTRY_DATE = item.GDT_ENTRY_DATE,
                QCTESTBK = item.QCTESTBK,
                TESTQTYBK = item.TESTQTYBK,
                PRIORITYD = item.PRIORITYD,
                PRIORITY100 = item.PRIORITY100,
                APRXRATE = item.APRXRATE,
                UHCSHC = item.UHCSHC,
                UHCDH = item.UHCDH,
                UHCCHC = item.UHCCHC,
                UHCPHC_HWC = item.UHCPHC_HWC,
                UHCEDL = item.UHCEDL,
                RCEXTANDABLE = item.RCEXTANDABLE,
                ISPSU = item.ISPSU,
                PSURATE = item.PSURATE,
                ISOTHERCORP = item.ISOTHERCORP,
                SHCGMSC_MIN = item.SHCGMSC_MIN,
                DHSCGMSC = item.DHSCGMSC,
                HWC = item.HWC,
                DHSFM = item.DHSFM,
                ISMITANIN = item.ISMITANIN,
                ISIMPD = item.ISIMPD,
                IMPTYPE = item.IMPTYPE,
                BPRIORITY = item.BPRIORITY,
                SECMAM = item.SECMAM,
                MINBUFFQTY = item.MINBUFFQTY,
                MAXBUFFQTY = item.MAXBUFFQTY,
                ISEDL2019 = item.ISEDL2019,
                OLDEDLCAT = item.OLDEDLCAT,
                DHSBUFFERSTOCK = item.DHSBUFFERSTOCK,
                ISNEDL = item.ISNEDL,
                OLDDHFM = item.OLDDHFM,
                OLDDHSBUFFERSTOCK = item.OLDDHSBUFFERSTOCK,
                RATESOURCH = item.RATESOURCH,
                ISFREEZED = item.ISFREEZED,
                PROGRAMDRU = item.PROGRAMDRU,
                REMARKS = item.REMARKS,
                ISDVDMSMAPPED = item.ISDVDMSMAPPED,
                ISHIDE = item.ISHIDE,
                PITEMCODE = item.PITEMCODE,
                COVIDFLAG = item.COVIDFLAG,
                COVIDD = item.COVIDD,
                COVIDORDER = item.COVIDORDER,
                OLDDHFM2 = item.OLDDHFM2,
                SPTENDER = item.SPTENDER,
                CITEMCODE = item.CITEMCODE,
                COVIDC_POFLAG = item.COVIDC_POFLAG,
                LITEMCODE = item.LITEMCODE,
                ISCOVIDINDENT = item.ISCOVIDINDENT,
                TESTID = item.TESTID,
                ISCOVIDINDFLAG = item.ISCOVIDINDFLAG,
                PCODE = item.PCODE,
                ISNONEDL = item.ISNONEDL,
                ISQCCHECK = item.ISQCCHECK,
                TARGET1 = item.TARGET1,
                CSNO = item.CSNO,
                AIREF = item.AIREF,
                ONLYCOVIDAI = item.ONLYCOVIDAI,
                ISCONEDL2013 = item.ISCONEDL2013,
                ISEDL2021 = item.ISEDL2021,
                BUFFERSTOCK21 = item.BUFFERSTOCK21,
                EDLCATUPTO2019 = item.EDLCATUPTO2019,
                EDL2021REMARK = item.EDL2021REMARK,
                ISGOVD = item.ISGOVD,
                TESTCOMOFORREPORT = item.TESTCOMOFORREPORT,
                VIRLABCREPOR = item.VIRLABCREPOR,
                PERLABPERTEST = item.PERLABPERTEST,
                BRODID = item.BRODID,
                NABLREQ = item.NABLREQ,
                NABLENTRYDT = item.NABLENTRYDT,
                EMSITEMID = item.EMSITEMID,
                EMSMACHINEID = item.EMSMACHINEID,
                PMACHINEID = item.PMACHINEID,
                EMSITEMCODE = item.EMSITEMCODE,
                LASTUPDATEDT = item.LASTUPDATEDT,
                PITEMID = item.PITEMID,
                ISBANNED = item.ISBANNED,
                ITPRENTRYDT = item.ITPRENTRYDT,
                ITPRREMARKS = item.ITPRREMARKS,
                ISFREEZ_ITPR = item.ISFREEZ_ITPR,
                ITPR_OTP = item.ITPR_OTP,
                RCVALIDCODE = item.RCVALIDCODE,
                ISSTERILITY = item.ISSTERILITY,
                STERILITYENTRYDT = item.STERILITYENTRYDT,
                SUBCATID = item.SUBCATID,
                AYUSHAIORDER = item.AYUSHAIORDER,
                INSERTEDBY = item.INSERTEDBY,
                ISRFORLP = item.ISRFORLP,
                ISDEDL = item.ISDEDL,
                ISWHREPORT = item.ISWHREPORT,
                MAKE = item.MAKE,
                MODEL = item.MODEL,
                MMID = item.MMID,
                DISPENSETYPEID = item.DISPENSETYPEID,
                ISMCITEMS = item.ISMCITEMS,
                MATERIAL_ID = item.MATERIAL_ID,
                CTRCALB = item.CTRCALB,
                TESTUNITONREAG = item.TESTUNITONREAG,
                SHC = item.SHC
                // Map other fields as needed
            }).ToList();

            // Step 3: Upsert data into MariaDB
            foreach (var record in mariaData)
            {
                var existingRecord = await _mariaContext.DmasitemsDbSet
                    .FirstOrDefaultAsync(x => x.ITEMID == record.ITEMID);

                if (existingRecord == null)
                {
                    // Insert new record
                    await _mariaContext.DmasitemsDbSet.AddAsync(record);
                }
                else
                {
                    // Update existing record
                    existingRecord.ITEMCODE = record.ITEMCODE;
                    existingRecord.ITEMID = record.ITEMID;
                    existingRecord.ITEMCODE = record.ITEMCODE;
                    existingRecord.ITEMNAME = record.ITEMNAME;
                    existingRecord.ITEMTYPEID = record.ITEMTYPEID;
                    existingRecord.CATEGORYID = record.CATEGORYID;
                    existingRecord.GROUPID = record.GROUPID;
                    existingRecord.UNIT = record.UNIT;
                    existingRecord.STOCKSTATUSID = record.STOCKSTATUSID;
                    existingRecord.VENID = record.VENID;
                    existingRecord.STRENGTH1 = record.STRENGTH1;
                    existingRecord.STRENGTH2 = record.STRENGTH2;
                    existingRecord.PACKING1 = record.PACKING1;
                    existingRecord.PACKING2 = record.PACKING2;
                    existingRecord.PACKCARTON = record.PACKCARTON;
                    existingRecord.PACKINGQTY = record.PACKINGQTY;
                    existingRecord.QTYIND = record.QTYIND;
                    existingRecord.QCTEST = record.QCTEST;
                    existingRecord.TESTQTY = record.TESTQTY;
                    existingRecord.EXPINDICATID = record.EXPINDICATID;
                    existingRecord.ACTIVEFLAG = record.ACTIVEFLAG;
                    existingRecord.RATE = record.RATE;
                    existingRecord.QCMINQTY = record.QCMINQTY;
                    existingRecord.MINLVL = record.MINLVL;
                    existingRecord.MAXLVL = record.MAXLVL;
                    existingRecord.REORDLVL = record.REORDLVL;
                    existingRecord.EMERLVL = record.EMERLVL;
                    existingRecord.REF_REQ = record.REF_REQ;
                    existingRecord.SUBGROUPID = record.SUBGROUPID;
                    existingRecord.HIGHLIGHTNULLBATCHES = record.HIGHLIGHTNULLBATCHES;
                    existingRecord.UNSPSCID = record.UNSPSCID;
                    existingRecord.ECRIID = record.ECRIID;
                    existingRecord.DGSDID = record.DGSDID;
                    existingRecord.GS1PRODUCTCODE = record.GS1PRODUCTCODE;
                    existingRecord.ITEMCLASSIFICATION = record.ITEMCLASSIFICATION;
                    existingRecord.REFITEMID = record.REFITEMID;
                    existingRecord.BRANDNAME = record.BRANDNAME;
                    existingRecord.ISEDL = record.ISEDL;
                    existingRecord.SHELFLIFEINMONTHS = record.SHELFLIFEINMONTHS;
                    existingRecord.SHELFLIFEINPERCENT = record.SHELFLIFEINPERCENT;
                    existingRecord.PHARMACOPOEIA = record.PHARMACOPOEIA;
                    existingRecord.COMPOSITION = record.COMPOSITION;
                    existingRecord.UNITCOUNT = record.UNITCOUNT;
                    existingRecord.MULTIPLE = record.MULTIPLE;
                    existingRecord.QCTESTINGESTDAY = record.QCTESTINGESTDAY;
                    existingRecord.EDLCAT = record.EDLCAT;
                    existingRecord.ITEMNAME_HINDI = record.ITEMNAME_HINDI;
                    existingRecord.STRENGTH_HINDI = record.STRENGTH_HINDI;
                    existingRecord.TREAT_ID = record.TREAT_ID;
                    existingRecord.HSNCODE = record.HSNCODE;
                    existingRecord.GSTRATE = record.GSTRATE;
                    existingRecord.ROWNO = record.ROWNO;
                    existingRecord.EDLITEMID = record.EDLITEMID;
                    existingRecord.ISMITANINDRUG = record.ISMITANINDRUG;
                    existingRecord.GDT_ENTRY_DATE = record.GDT_ENTRY_DATE;
                    existingRecord.QCTESTBK = record.QCTESTBK;
                    existingRecord.TESTQTYBK = record.TESTQTYBK;
                    existingRecord.PRIORITYD = record.PRIORITYD;
                    existingRecord.PRIORITY100 = record.PRIORITY100;
                    existingRecord.APRXRATE = record.APRXRATE;
                    existingRecord.UHCSHC = record.UHCSHC;
                    existingRecord.UHCDH = record.UHCDH;
                    existingRecord.UHCCHC = record.UHCCHC;
                    existingRecord.UHCPHC_HWC = record.UHCPHC_HWC;
                    existingRecord.UHCEDL = record.UHCEDL;
                    existingRecord.RCEXTANDABLE = record.RCEXTANDABLE;
                    existingRecord.ISPSU = record.ISPSU;
                    existingRecord.PSURATE = record.PSURATE;
                    existingRecord.ISOTHERCORP = record.ISOTHERCORP;
                    existingRecord.SHCGMSC_MIN = record.SHCGMSC_MIN;
                    existingRecord.DHSCGMSC = record.DHSCGMSC;
                    existingRecord.HWC = record.HWC;
                    existingRecord.DHSFM = record.DHSFM;
                    existingRecord.ISMITANIN = record.ISMITANIN;
                    existingRecord.ISIMPD = record.ISIMPD;
                    existingRecord.IMPTYPE = record.IMPTYPE;
                    existingRecord.BPRIORITY = record.BPRIORITY;
                    existingRecord.SECMAM = record.SECMAM;
                    existingRecord.MINBUFFQTY = record.MINBUFFQTY;
                    existingRecord.MAXBUFFQTY = record.MAXBUFFQTY;
                    existingRecord.ISEDL2019 = record.ISEDL2019;
                    existingRecord.OLDEDLCAT = record.OLDEDLCAT;
                    existingRecord.DHSBUFFERSTOCK = record.DHSBUFFERSTOCK;
                    existingRecord.ISNEDL = record.ISNEDL;
                    existingRecord.OLDDHFM = record.OLDDHFM;
                    existingRecord.OLDDHSBUFFERSTOCK = record.OLDDHSBUFFERSTOCK;
                    existingRecord.RATESOURCH = record.RATESOURCH;
                    existingRecord.ISFREEZED = record.ISFREEZED;
                    existingRecord.PROGRAMDRU = record.PROGRAMDRU;
                    existingRecord.REMARKS = record.REMARKS;
                    existingRecord.ISDVDMSMAPPED = record.ISDVDMSMAPPED;
                    existingRecord.ISHIDE = record.ISHIDE;
                    existingRecord.PITEMCODE = record.PITEMCODE;
                    existingRecord.COVIDFLAG = record.COVIDFLAG;
                    existingRecord.COVIDD = record.COVIDD;
                    existingRecord.COVIDORDER = record.COVIDORDER;
                    existingRecord.OLDDHFM2 = record.OLDDHFM2;
                    existingRecord.SPTENDER = record.SPTENDER;
                    existingRecord.CITEMCODE = record.CITEMCODE;
                    existingRecord.COVIDC_POFLAG = record.COVIDC_POFLAG;
                    existingRecord.LITEMCODE = record.LITEMCODE;
                    existingRecord.ISCOVIDINDENT = record.ISCOVIDINDENT;
                    existingRecord.TESTID = record.TESTID;
                    existingRecord.ISCOVIDINDFLAG = record.ISCOVIDINDFLAG;
                    existingRecord.PCODE = record.PCODE;
                    existingRecord.ISNONEDL = record.ISNONEDL;
                    existingRecord.ISQCCHECK = record.ISQCCHECK;
                    existingRecord.TARGET1 = record.TARGET1;
                    existingRecord.CSNO = record.CSNO;
                    existingRecord.AIREF = record.AIREF;
                    existingRecord.ONLYCOVIDAI = record.ONLYCOVIDAI;
                    existingRecord.ISCONEDL2013 = record.ISCONEDL2013;
                    existingRecord.ISEDL2021 = record.ISEDL2021;
                    existingRecord.BUFFERSTOCK21 = record.BUFFERSTOCK21;
                    existingRecord.EDLCATUPTO2019 = record.EDLCATUPTO2019;
                    existingRecord.EDL2021REMARK = record.EDL2021REMARK;
                    existingRecord.ISGOVD = record.ISGOVD;
                    existingRecord.TESTCOMOFORREPORT = record.TESTCOMOFORREPORT;
                    existingRecord.VIRLABCREPOR = record.VIRLABCREPOR;
                    existingRecord.PERLABPERTEST = record.PERLABPERTEST;
                    existingRecord.BRODID = record.BRODID;
                    existingRecord.NABLREQ = record.NABLREQ;
                    existingRecord.NABLENTRYDT = record.NABLENTRYDT;
                    existingRecord.EMSITEMID = record.EMSITEMID;
                    existingRecord.EMSMACHINEID = record.EMSMACHINEID;
                    existingRecord.PMACHINEID = record.PMACHINEID;
                    existingRecord.EMSITEMCODE = record.EMSITEMCODE;
                    existingRecord.LASTUPDATEDT = record.LASTUPDATEDT;
                    existingRecord.PITEMID = record.PITEMID;
                    existingRecord.ISBANNED = record.ISBANNED;
                    existingRecord.ITPRENTRYDT = record.ITPRENTRYDT;
                    existingRecord.ITPRREMARKS = record.ITPRREMARKS;
                    existingRecord.ISFREEZ_ITPR = record.ISFREEZ_ITPR;
                    existingRecord.ITPR_OTP = record.ITPR_OTP;
                    existingRecord.RCVALIDCODE = record.RCVALIDCODE;
                    existingRecord.ISSTERILITY = record.ISSTERILITY;
                    existingRecord.STERILITYENTRYDT = record.STERILITYENTRYDT;
                    existingRecord.SUBCATID = record.SUBCATID;
                    existingRecord.AYUSHAIORDER = record.AYUSHAIORDER;
                    existingRecord.INSERTEDBY = record.INSERTEDBY;
                    existingRecord.ISRFORLP = record.ISRFORLP;
                    existingRecord.ISDEDL = record.ISDEDL;
                    existingRecord.ISWHREPORT = record.ISWHREPORT;
                    existingRecord.MAKE = record.MAKE;
                    existingRecord.MODEL = record.MODEL;
                    existingRecord.MMID = record.MMID;
                    existingRecord.DISPENSETYPEID = record.DISPENSETYPEID;
                    existingRecord.ISMCITEMS = record.ISMCITEMS;
                    existingRecord.MATERIAL_ID = record.MATERIAL_ID;
                    existingRecord.CTRCALB = record.CTRCALB;
                    existingRecord.TESTUNITONREAG = record.TESTUNITONREAG;
                    existingRecord.SHC = record.SHC;

                    // Update other fields as needed

                }
            }

            // Step 4: Save changes
            await _mariaContext.SaveChangesAsync();
        }

        public async Task TransferDataAsync_masdistricts()
        {
            // Step 1: Fetch data from Oracle
            var oracleData = await _oracleContext.SmasdistrictsDbSet.ToListAsync();

            // Step 2: Map/Transform data (if needed)
            var mariaData = oracleData.Select(item => new DmasdistrictsDTO
            {
                DISTRICTID = item.DISTRICTID,
                DISTRICTCODE = item.DISTRICTCODE,
                DISTRICTNAME = item.DISTRICTNAME,
                STATEID = item.STATEID,
                DIVISIONID = item.DIVISIONID,
                NHMDISTRICTID = item.NHMDISTRICTID,
                CENSUSDISTRICTID = item.CENSUSDISTRICTID,
                WAREHOUSEID = item.WAREHOUSEID,
                VLFACILITYID = item.VLFACILITYID,
                ENTRY_DATE = item.ENTRY_DATE,
                NGDISTRICTID = item.NGDISTRICTID,
                NGSTATEID = item.NGSTATEID
                // Map other fields as needed
            }).ToList();

            // Step 3: Upsert data into MariaDB
            foreach (var record in mariaData)
            {
                var existingRecord = await _mariaContext.DmasdistrictsDbSet
                    .FirstOrDefaultAsync(x => x.DISTRICTID == record.DISTRICTID);

                if (existingRecord == null)
                {
                    // Insert new record
                    await _mariaContext.DmasdistrictsDbSet.AddAsync(record);
                }
                else
                {
                    // Update existing record
                    existingRecord.DISTRICTID = record.DISTRICTID;
                    existingRecord.DISTRICTCODE = record.DISTRICTCODE;
                    existingRecord.DISTRICTNAME = record.DISTRICTNAME;
                    existingRecord.STATEID = record.STATEID;
                    existingRecord.DIVISIONID = record.DIVISIONID;
                    existingRecord.NHMDISTRICTID = record.NHMDISTRICTID;
                    existingRecord.CENSUSDISTRICTID = record.CENSUSDISTRICTID;
                    existingRecord.WAREHOUSEID = record.WAREHOUSEID;
                    existingRecord.VLFACILITYID = record.VLFACILITYID;
                    existingRecord.ENTRY_DATE = record.ENTRY_DATE;
                    existingRecord.NGDISTRICTID = record.NGDISTRICTID;
                    existingRecord.NGSTATEID = record.NGSTATEID;

                    // Update other fields as needed

                }
            }

            // Step 4: Save changes
            await _mariaContext.SaveChangesAsync();
        }

        public async Task TransferDataAsync_masitemtypes()
        {
            // Step 1: Fetch data from Oracle
            var oracleData = await _oracleContext.SmasitemtypesDbSet.ToListAsync();

            // Step 2: Map/Transform data (if needed)
            var mariaData = oracleData.Select(item => new DmasitemtypesDTO
            {
                ITEMTYPEID = item.ITEMTYPEID,
                ITEMTYPECODE = item.ITEMTYPECODE,
                ITEMTYPENAME = item.ITEMTYPENAME,
                TYPEPREFIX = item.TYPEPREFIX,
                QCDAYSLAB = item.QCDAYSLAB,
                POITEMTYPEID = item.POITEMTYPEID,
                ENTRYDT = item.ENTRYDT,

                // Map other fields as needed
            }).ToList();

            // Step 3: Upsert data into MariaDB
            foreach (var record in mariaData)
            {
                var existingRecord = await _mariaContext.DmasitemtypesDbSet
                    .FirstOrDefaultAsync(x => x.ITEMTYPEID == record.ITEMTYPEID);

                if (existingRecord == null)
                {
                    // Insert new record
                    await _mariaContext.DmasitemtypesDbSet.AddAsync(record);
                }
                else
                {
                    // Update existing record
                    existingRecord.ITEMTYPEID = record.ITEMTYPEID;
                    existingRecord.ITEMTYPECODE = record.ITEMTYPECODE;
                    existingRecord.ITEMTYPENAME = record.ITEMTYPENAME;
                    existingRecord.TYPEPREFIX = record.TYPEPREFIX;
                    existingRecord.QCDAYSLAB = record.QCDAYSLAB;
                    existingRecord.POITEMTYPEID = record.POITEMTYPEID;
                    existingRecord.ENTRYDT = record.ENTRYDT;

                    // Update other fields as needed

                }
            }

            // Step 4: Save changes
            await _mariaContext.SaveChangesAsync();
        }

        public async Task TransferDataAsync_masitemcategories()
        {
            // Step 1: Fetch data from Oracle
            var oracleData = await _oracleContext.SmasitemcategoriesDbSet.ToListAsync();

            // Step 2: Map/Transform data (if needed)
            var mariaData = oracleData.Select(item => new DmasitemcategoriesDTO
            {
                CATEGORYID = item.CATEGORYID,
                CATEGORYCODE = item.CATEGORYCODE,
                CATEGORYNAME = item.CATEGORYNAME,
                CATEGORYPREFIX = item.CATEGORYPREFIX,
                SUPPLYORDERCODE = item.SUPPLYORDERCODE,
                ENTRY_DATE = item.ENTRY_DATE,
                MCID = item.MCID,


                // Map other fields as needed
            }).ToList();

            // Step 3: Upsert data into MariaDB
            foreach (var record in mariaData)
            {
                var existingRecord = await _mariaContext.DmasitemcategoriesDbSet
                    .FirstOrDefaultAsync(x => x.CATEGORYID == record.CATEGORYID);

                if (existingRecord == null)
                {
                    // Insert new record
                    await _mariaContext.DmasitemcategoriesDbSet.AddAsync(record);
                }
                else
                {
                    // Update existing record
                    existingRecord.CATEGORYID = record.CATEGORYID;
                    existingRecord.CATEGORYCODE = record.CATEGORYCODE;
                    existingRecord.CATEGORYNAME = record.CATEGORYNAME;
                    existingRecord.CATEGORYPREFIX = record.CATEGORYPREFIX;
                    existingRecord.SUPPLYORDERCODE = record.SUPPLYORDERCODE;
                    existingRecord.ENTRY_DATE = record.ENTRY_DATE;
                    existingRecord.MCID = record.MCID;


                    // Update other fields as needed

                }
            }

            // Step 4: Save changes
            await _mariaContext.SaveChangesAsync();
        }

        public async Task TransferDataAsync_masfacilitytypes()
        {
            // Step 1: Fetch data from Oracle
            var oracleData = await _oracleContext.SmasfacilitytypesDbSet.ToListAsync();

            // Step 2: Map/Transform data (if needed)
            var mariaData = oracleData.Select(item => new DmasfacilitytypesDTO
            {
                FACILITYTYPEID = item.FACILITYTYPEID,
                FACILITYTYPECODE = item.FACILITYTYPECODE,
                FACILITYTYPEDESC = item.FACILITYTYPEDESC,
                DIRECTORATEID = item.DIRECTORATEID,
                ELDCAT = item.ELDCAT,
                ORDERQ = item.ORDERQ,
                ORDERD = item.ORDERD,
                ORDERDP = item.ORDERDP,
                UNIQUSTATS = item.UNIQUSTATS,
                EDL_ELIGIBILITY = item.EDL_ELIGIBILITY,
                HODID = item.HODID,
                EDLINDENT = item.EDLINDENT,
                ENTRY_DATE = item.ENTRY_DATE,

                // Map other fields as needed
            }).ToList();

            // Step 3: Upsert data into MariaDB
            foreach (var record in mariaData)
            {
                var existingRecord = await _mariaContext.DmasfacilitytypesDbSet
                    .FirstOrDefaultAsync(x => x.FACILITYTYPEID == record.FACILITYTYPEID);

                if (existingRecord == null)
                {
                    // Insert new record
                    await _mariaContext.DmasfacilitytypesDbSet.AddAsync(record);
                }
                else
                {
                    // Update existing record
                    existingRecord.FACILITYTYPEID = record.FACILITYTYPEID;
                    existingRecord.FACILITYTYPECODE = record.FACILITYTYPECODE;
                    existingRecord.FACILITYTYPEDESC = record.FACILITYTYPEDESC;
                    existingRecord.DIRECTORATEID = record.DIRECTORATEID;
                    existingRecord.ELDCAT = record.ELDCAT;
                    existingRecord.ORDERQ = record.ORDERQ;
                    existingRecord.ORDERD = record.ORDERD;
                    existingRecord.ORDERDP = record.ORDERDP;
                    existingRecord.UNIQUSTATS = record.UNIQUSTATS;
                    existingRecord.EDL_ELIGIBILITY = record.EDL_ELIGIBILITY;
                    existingRecord.HODID = record.HODID;
                    existingRecord.EDLINDENT = record.EDLINDENT;
                    existingRecord.ENTRY_DATE = record.ENTRY_DATE;



                    // Update other fields as needed

                }
            }

            // Step 4: Save changes
            await _mariaContext.SaveChangesAsync();
        }

        public async Task TransferDataAsync_masfacilities()
        {
            // Step 1: Fetch data from Oracle
            var oracleData = await _oracleContext.SmasfacilitiesDbSet.ToListAsync();

            // Step 2: Map/Transform data (if needed)
            var mariaData = oracleData.Select(item => new DmasfacilitiesDTO
            {
                FACILITYID = (int?)item.FACILITYID,
                FACILITYCODE = item.FACILITYCODE,
                FACILITYNAME = item.FACILITYNAME,
                ADDRESS1 = item.ADDRESS1,
                ADDRESS2 = item.ADDRESS2,
                ADDRESS3 = item.ADDRESS3,
                CITY = item.CITY,
                ZIP = item.ZIP,
                PHONE1 = item.PHONE1,
                PHONE2 = item.PHONE2,
                FAX = item.FAX,
                EMAIL = item.EMAIL,
                ISACTIVE = Convert.ToBoolean(item.ISACTIVE),
                STATEID = (int?)item.STATEID,
                DISTRICTID = (int?)item.DISTRICTID,
                LOCATIONID = (int?)item.LOCATIONID,
                FACILITYTYPEID = (int?)item.FACILITYTYPEID,
                REMARKS = item.REMARKS,
                CONTACTPERSONNAME = item.CONTACTPERSONNAME,
                NHM_HCID = item.NHM_HCID,
                NHM_DISTRICTID = item.NHM_DISTRICTID,
                TYPE = item.TYPE,
                FACILITYNAME_HINDI = item.FACILITYNAME_HINDI,
                DHSSTORE = item.DHSSTORE,
                CHCID = item.CHCID,
                CHCFACILITYID = (int?)item.CHCFACILITYID,
                GDT_ENTRY_DATE = item.GDT_ENTRY_DATE,
                LONGITUDE = item.LONGITUDE,
                LATITUDE = item.LATITUDE,
                UHCFAC = item.UHCFAC,
                ISBUFFERFREEZE = item.ISBUFFERFREEZE,
                ISBUFFERCONSFREEZ = item.ISBUFFERCONSFREEZ,
                PPHCID = (int?)item.PPHCID,
                SHC_ID = (int?)item.SHC_ID,
                PHC_ID = (int?)item.PHC_ID,
                ISCOVIDFAC = item.ISCOVIDFAC,
                LATLONGREMARKS = item.LATLONGREMARKS,
                INCHARGENAME = item.INCHARGENAME,
                HC_IDMAPPED = item.HC_IDMAPPED,
                OPDATE = item.OPDATE,
                ISUPDATED = item.ISUPDATED,
                ITEMINDENTMC_H = item.ITEMINDENTMC_H,
                NINNO = (int?)item.NINNO,
                ISUPDATEDNINFROMGOI_REC_DATA = item.ISUPDATEDNINFROMGOI_REC_DATA,
                AYFACTYPEID = (int?)item.AYFACTYPEID,
                ISSHCMAPPED = item.ISSHCMAPPED,
                OLDDISTRICTID = (int?)item.OLDDISTRICTID,
                ISDRIVERADDEDPOSITION = item.ISDRIVERADDEDPOSITION,
                POSITIONENTRYBYDRIVER = item.POSITIONENTRYBYDRIVER,
                ISWHFREEZLAT = item.ISWHFREEZLAT,
                NGHF_ID = (int?)item.NGHF_ID,
                ISNGWORKING = item.ISNGWORKING,
                IS_AAM = item.IS_AAM,
                INDENTDURATION = item.INDENTDURATION,


                // Map other fields as needed
            }).ToList();

            // Step 3: Upsert data into MariaDB
            foreach (var record in mariaData)
            {
                var existingRecord = await _mariaContext.DmasfacilitiesDbSet
                    .FirstOrDefaultAsync(x => x.FACILITYID == record.FACILITYID);

                if (existingRecord == null)
                {
                    // Insert new record
                    await _mariaContext.DmasfacilitiesDbSet.AddAsync(record);
                }
                else
                {
                    // Update existing record
                    existingRecord.FACILITYID = record.FACILITYID;
                    existingRecord.FACILITYCODE = record.FACILITYCODE;
                    existingRecord.FACILITYNAME = record.FACILITYNAME;
                    existingRecord.ADDRESS1 = record.ADDRESS1;
                    existingRecord.ADDRESS2 = record.ADDRESS2;
                    existingRecord.ADDRESS3 = record.ADDRESS3;
                    existingRecord.CITY = record.CITY;
                    existingRecord.ZIP = record.ZIP;
                    existingRecord.PHONE1 = record.PHONE1;
                    existingRecord.PHONE2 = record.PHONE2;
                    existingRecord.FAX = record.FAX;
                    existingRecord.EMAIL = record.EMAIL;
                    existingRecord.ISACTIVE = record.ISACTIVE;
                    existingRecord.STATEID = record.STATEID;
                    existingRecord.DISTRICTID = record.DISTRICTID;
                    existingRecord.LOCATIONID = record.LOCATIONID;
                    existingRecord.FACILITYTYPEID = record.FACILITYTYPEID;
                    existingRecord.REMARKS = record.REMARKS;
                    existingRecord.CONTACTPERSONNAME = record.CONTACTPERSONNAME;
                    existingRecord.NHM_HCID = record.NHM_HCID;
                    existingRecord.NHM_DISTRICTID = record.NHM_DISTRICTID;
                    existingRecord.TYPE = record.TYPE;
                    existingRecord.FACILITYNAME_HINDI = record.FACILITYNAME_HINDI;
                    existingRecord.DHSSTORE = record.DHSSTORE;
                    existingRecord.CHCID = record.CHCID;
                    existingRecord.CHCFACILITYID = record.CHCFACILITYID;
                    existingRecord.GDT_ENTRY_DATE = record.GDT_ENTRY_DATE;
                    existingRecord.LONGITUDE = record.LONGITUDE;
                    existingRecord.LATITUDE = record.LATITUDE;
                    existingRecord.UHCFAC = record.UHCFAC;
                    existingRecord.ISBUFFERFREEZE = record.ISBUFFERFREEZE;
                    existingRecord.ISBUFFERCONSFREEZ = record.ISBUFFERCONSFREEZ;
                    existingRecord.PPHCID = record.PPHCID;
                    existingRecord.SHC_ID = record.SHC_ID;
                    existingRecord.PHC_ID = record.PHC_ID;
                    existingRecord.ISCOVIDFAC = record.ISCOVIDFAC;
                    existingRecord.LATLONGREMARKS = record.LATLONGREMARKS;
                    existingRecord.INCHARGENAME = record.INCHARGENAME;
                    existingRecord.HC_IDMAPPED = record.HC_IDMAPPED;
                    existingRecord.OPDATE = record.OPDATE;
                    existingRecord.ISUPDATED = record.ISUPDATED;
                    existingRecord.ITEMINDENTMC_H = record.ITEMINDENTMC_H;
                    existingRecord.NINNO = record.NINNO;
                    existingRecord.ISUPDATEDNINFROMGOI_REC_DATA = record.ISUPDATEDNINFROMGOI_REC_DATA;
                    existingRecord.AYFACTYPEID = record.AYFACTYPEID;
                    existingRecord.ISSHCMAPPED = record.ISSHCMAPPED;
                    existingRecord.OLDDISTRICTID = record.OLDDISTRICTID;
                    existingRecord.ISDRIVERADDEDPOSITION = record.ISDRIVERADDEDPOSITION;
                    existingRecord.POSITIONENTRYBYDRIVER = record.POSITIONENTRYBYDRIVER;
                    existingRecord.ISWHFREEZLAT = record.ISWHFREEZLAT;
                    existingRecord.NGHF_ID = record.NGHF_ID;
                    existingRecord.ISNGWORKING = record.ISNGWORKING;
                    existingRecord.IS_AAM = record.IS_AAM;
                    existingRecord.INDENTDURATION = record.INDENTDURATION;




                    // Update other fields as needed

                }
            }

            // Step 4: Save changes
            await _mariaContext.SaveChangesAsync();
        }

        public async Task TransferDataAsync_usrusers()
        {
            // Step 1: Fetch data from Oracle
            var oracleData = await _oracleContext.SusrusersDbSet.ToListAsync();

            // Step 2: Map/Transform data (if needed)
            var mariaData = oracleData.Select(item => new DusrusersDTO
            {
                USERID = item.USERID,
                EMAILID = item.EMAILID,
                PWD = item.PWD,
                FIRSTNAME = item.FIRSTNAME,
                LASTNAME = item.LASTNAME,
                ROLEID = item.ROLEID,
                USERTYPE = item.USERTYPE,
                DISPLAYNAME = item.DISPLAYNAME,
                STATUS = item.STATUS,
                ISDELETED = Convert.ToInt32(item.ISDELETED),
                IPADDRESS = item.IPADDRESS,
                CREATEDDATE = item.CREATEDDATE,
                SOURCEID = item.SOURCEID,
                SCHEMEID = item.SCHEMEID,
                STATEID = item.STATEID,
                DISTRICTID = item.DISTRICTID,
                WAREHOUSEID = item.WAREHOUSEID,
                FACILITYID = item.FACILITYID,
                COUNTRYID = item.COUNTRYID,
                SUPPLIERID = item.SUPPLIERID,
                PSAID = item.PSAID,
                DIVISIONID = item.DIVISIONID,
                ISLOCALSUPPLIERS = Convert.ToInt32(item.ISLOCALSUPPLIERS),
                PASSWORDRESETKEY = item.PASSWORDRESETKEY,
                RESETPASSWORD = Convert.ToInt32(item.RESETPASSWORD),
                LPSUPPLIERID = item.LPSUPPLIERID,
                LABID = item.LABID,
                ORDERID = Convert.ToDouble(item.ORDERID),
                OTP = item.OTP,
                OTPUPDATEDT = item.OTPUPDATEDT,
                DEPMOBILE = Convert.ToInt64(item.DEPMOBILE),
                DEPEMAIL = item.DEPEMAIL,
                ISFILE = item.ISFILE,
                ISPAYMENT = item.ISPAYMENT,
                HORDER = Convert.ToInt32(item.HORDER),
                HORDER1 = Convert.ToInt32(item.HORDER1),
                ADMINTYPE = item.ADMINTYPE,
                ORDERDP = Convert.ToInt32(item.ORDERDP),
                ISRESETPWD = item.ISRESETPWD,
                MACADDRESS = item.MACADDRESS,
                ISMRCUSER = item.ISMRCUSER,
                LASTPWDCHANGEDATE = item.LASTPWDCHANGEDATE,
                ISOTPUSER = item.ISOTPUSER,
                APPROLE = item.APPROLE,
                ISCOURIERPICK = item.ISCOURIERPICK,
                CPWD = item.CPWD,
                SUBWHID = item.SUBWHID,
                ISMACUSER = item.ISMACUSER

                // Map other fields as needed
            }).ToList();

            // Step 3: Upsert data into MariaDB
            foreach (var record in mariaData)
            {
                var existingRecord = await _mariaContext.DusrusersDbSet
                    .FirstOrDefaultAsync(x => x.USERID == record.USERID);

                if (existingRecord == null)
                {
                    // Insert new record
                    await _mariaContext.DusrusersDbSet.AddAsync(record);
                }
                else
                {
                    // Update existing record
                    existingRecord.USERID = record.USERID;
                    existingRecord.EMAILID = record.EMAILID;
                    existingRecord.PWD = record.PWD;
                    existingRecord.FIRSTNAME = record.FIRSTNAME;
                    existingRecord.LASTNAME = record.LASTNAME;
                    existingRecord.ROLEID = record.ROLEID;
                    existingRecord.USERTYPE = record.USERTYPE;
                    existingRecord.DISPLAYNAME = record.DISPLAYNAME;
                    existingRecord.STATUS = record.STATUS;
                    existingRecord.ISDELETED = record.ISDELETED;
                    existingRecord.IPADDRESS = record.IPADDRESS;
                    existingRecord.CREATEDDATE = record.CREATEDDATE;
                    existingRecord.SOURCEID = record.SOURCEID;
                    existingRecord.SCHEMEID = record.SCHEMEID;
                    existingRecord.STATEID = record.STATEID;
                    existingRecord.DISTRICTID = record.DISTRICTID;
                    existingRecord.WAREHOUSEID = record.WAREHOUSEID;
                    existingRecord.FACILITYID = record.FACILITYID;
                    existingRecord.COUNTRYID = record.COUNTRYID;
                    existingRecord.SUPPLIERID = record.SUPPLIERID;
                    existingRecord.PSAID = record.PSAID;
                    existingRecord.DIVISIONID = record.DIVISIONID;
                    existingRecord.ISLOCALSUPPLIERS = record.ISLOCALSUPPLIERS;
                    existingRecord.PASSWORDRESETKEY = record.PASSWORDRESETKEY;
                    existingRecord.RESETPASSWORD = record.RESETPASSWORD;
                    existingRecord.LPSUPPLIERID = record.LPSUPPLIERID;
                    existingRecord.LABID = record.LABID;
                    existingRecord.ORDERID = record.ORDERID;
                    existingRecord.OTP = record.OTP;
                    existingRecord.OTPUPDATEDT = record.OTPUPDATEDT;
                    existingRecord.DEPMOBILE = record.DEPMOBILE;
                    existingRecord.DEPEMAIL = record.DEPEMAIL;
                    existingRecord.ISFILE = record.ISFILE;
                    existingRecord.ISPAYMENT = record.ISPAYMENT;
                    existingRecord.HORDER = record.HORDER;
                    existingRecord.HORDER1 = record.HORDER1;
                    existingRecord.ADMINTYPE = record.ADMINTYPE;
                    existingRecord.ORDERDP = record.ORDERDP;
                    existingRecord.ISRESETPWD = record.ISRESETPWD;
                    existingRecord.MACADDRESS = record.MACADDRESS;
                    existingRecord.ISMRCUSER = record.ISMRCUSER;
                    existingRecord.LASTPWDCHANGEDATE = record.LASTPWDCHANGEDATE;
                    existingRecord.ISOTPUSER = record.ISOTPUSER;
                    existingRecord.APPROLE = record.APPROLE;
                    existingRecord.ISCOURIERPICK = record.ISCOURIERPICK;
                    existingRecord.CPWD = record.CPWD;
                    existingRecord.SUBWHID = record.SUBWHID;
                    existingRecord.ISMACUSER = record.ISMACUSER;




                    // Update other fields as needed

                }
            }

            // Step 4: Save changes
            await _mariaContext.SaveChangesAsync();
        }

        public async Task TransferDataAsync_masfacilitywh()
        {
            // Step 1: Fetch data from Oracle
            var oracleData = await _oracleContext.SmasfacilitywhDbSet.ToListAsync();

            // Step 2: Map/Transform data (if needed)
            var mariaData = oracleData.Select(item => new DmasfacilitywhDTO
            {
                FACILITYWHNO = item.FACILITYWHNO,
                WAREHOUSEID = item.WAREHOUSEID,
                FACILITYID = item.FACILITYID,
                STATUS = item.STATUS ?? "A", // Use default if null
                ENTRY_DATE = item.ENTRY_DATE ?? DateTime.Now, // Use default if null
                REMARKS = item.REMARKS

                // Map other fields as needed
            }).ToList();

            // Step 3: Upsert data into MariaDB
            foreach (var record in mariaData)
            {
                var existingRecord = await _mariaContext.DmasfacilitywhDbSet
                    .FirstOrDefaultAsync(x => x.FACILITYWHNO == record.FACILITYWHNO);

                if (existingRecord == null)
                {
                    // Insert new record
                    await _mariaContext.DmasfacilitywhDbSet.AddAsync(record);
                }
                else
                {
                    // Update existing record
                    existingRecord.FACILITYWHNO = record.FACILITYWHNO;
                    existingRecord.WAREHOUSEID = record.WAREHOUSEID;
                    existingRecord.FACILITYID = record.FACILITYID;
                    existingRecord.STATUS = record.STATUS ?? existingRecord.STATUS; // Retain existing if null
                    existingRecord.ENTRY_DATE = record.ENTRY_DATE ?? existingRecord.ENTRY_DATE; // Retain existing if null
                    existingRecord.REMARKS = record.REMARKS ?? existingRecord.REMARKS; // Retain existing if null

                    // Update other fields as needed

                }
            }

            // Step 4: Save changes
            await _mariaContext.SaveChangesAsync();
        }

        public async Task TransferDataAsync_maswarehouses()
        {
            // Step 1: Fetch data from Oracle
            var oracleData = await _oracleContext.SmaswarehousesDbSet.ToListAsync();

            // Step 2: Map/Transform data (if needed)
            var mariaData = oracleData.Select(item => new DmaswarehousesDTO
            {
                WAREHOUSEID = item.WAREHOUSEID,
                WAREHOUSECODE = item.WAREHOUSECODE,
                WAREHOUSENAME = item.WAREHOUSENAME,
                WAREHOUSETYPEID = item.WAREHOUSETYPEID,
                ADDRESS1 = item.ADDRESS1,
                ADDRESS2 = item.ADDRESS2,
                ADDRESS3 = item.ADDRESS3,
                CITY = item.CITY,
                ZIP = item.ZIP,
                PHONE1 = item.PHONE1,
                PHONE2 = item.PHONE2,
                FAX = item.FAX,
                EMAIL = item.EMAIL,
                ISACTIVE = item.ISACTIVE, // Assuming ISACTIVE is stored as 1 or 0
                ISGOVERNMENT = item.ISGOVERNMENT, // Assuming ISGOVERNMENT is stored as 1 or 0
                STATEID = item.STATEID,
                DISTRICTID = item.DISTRICTID,
                LOCATIONID = item.LOCATIONID,
                REMARKS = item.REMARKS,
                CONTACTPERSONNAME = item.CONTACTPERSONNAME,
                AMID = item.AMID,
                SOID = item.SOID,
                PARENTID = item.PARENTID,
                GDT_ENTRY_DATE = item.GDT_ENTRY_DATE,
                FACILITYTYPEID = item.FACILITYTYPEID,
                FACILITYTYPE = item.FACILITYTYPE,
                POPER = item.POPER,
                ENTRY_DATE = item.ENTRY_DATE,
                ZONEID = item.ZONEID,
                LATITUDE = item.LATITUDE,
                LONGITUDE = item.LONGITUDE,
                ISANPRACTIVE = item.ISANPRACTIVE

                // Map other fields as needed
            }).ToList();

            // Step 3: Upsert data into MariaDB
            foreach (var record in mariaData)
            {
                var existingRecord = await _mariaContext.DmaswarehousesDbSet
                    .FirstOrDefaultAsync(x => x.WAREHOUSEID == record.WAREHOUSEID);

                if (existingRecord == null)
                {
                    // Insert new record
                    await _mariaContext.DmaswarehousesDbSet.AddAsync(record);
                }
                else
                {
                    // Update existing record
                    existingRecord.WAREHOUSEID = record.WAREHOUSEID;
                    existingRecord.WAREHOUSECODE = record.WAREHOUSECODE;
                    existingRecord.WAREHOUSENAME = record.WAREHOUSENAME;
                    existingRecord.WAREHOUSETYPEID = record.WAREHOUSETYPEID;
                    existingRecord.ADDRESS1 = record.ADDRESS1;
                    existingRecord.ADDRESS2 = record.ADDRESS2;
                    existingRecord.ADDRESS3 = record.ADDRESS3;
                    existingRecord.CITY = record.CITY;
                    existingRecord.ZIP = record.ZIP;
                    existingRecord.PHONE1 = record.PHONE1;
                    existingRecord.PHONE2 = record.PHONE2;
                    existingRecord.FAX = record.FAX;
                    existingRecord.EMAIL = record.EMAIL;
                    existingRecord.ISACTIVE = record.ISACTIVE;
                    existingRecord.ISGOVERNMENT = record.ISGOVERNMENT;
                    existingRecord.STATEID = record.STATEID;
                    existingRecord.DISTRICTID = record.DISTRICTID;
                    existingRecord.LOCATIONID = record.LOCATIONID;
                    existingRecord.REMARKS = record.REMARKS;
                    existingRecord.CONTACTPERSONNAME = record.CONTACTPERSONNAME;
                    existingRecord.AMID = record.AMID;
                    existingRecord.SOID = record.SOID;
                    existingRecord.PARENTID = record.PARENTID;
                    existingRecord.GDT_ENTRY_DATE = record.GDT_ENTRY_DATE;
                    existingRecord.FACILITYTYPEID = record.FACILITYTYPEID;
                    existingRecord.FACILITYTYPE = record.FACILITYTYPE;
                    existingRecord.POPER = record.POPER;
                    existingRecord.ENTRY_DATE = record.ENTRY_DATE;
                    existingRecord.ZONEID = record.ZONEID;
                    existingRecord.LATITUDE = record.LATITUDE;
                    existingRecord.LONGITUDE = record.LONGITUDE;
                    existingRecord.ISANPRACTIVE = record.ISANPRACTIVE;


                    // Update other fields as needed

                }
            }

            // Step 4: Save changes
            await _mariaContext.SaveChangesAsync();
        }

        public async Task TransferDataAsync_masitemmaincategory()
        {
            // Step 1: Fetch data from Oracle
            var oracleData = await _oracleContext.SmasitemmaincategoryDbSet.ToListAsync();

            // Step 2: Map/Transform data (if needed)
            var mariaData = oracleData.Select(item => new DmasitemmaincategoryDTO
            {
                MCID = item.MCID,
                MCATEGORY = item.MCATEGORY,


                // Map other fields as needed
            }).ToList();

            // Step 3: Upsert data into MariaDB
            foreach (var record in mariaData)
            {
                var existingRecord = await _mariaContext.DmasitemmaincategoryDbSet
                    .FirstOrDefaultAsync(x => x.MCID == record.MCID);

                if (existingRecord == null)
                {
                    // Insert new record
                    await _mariaContext.DmasitemmaincategoryDbSet.AddAsync(record);
                }
                else
                {
                    // Update existing record
                    existingRecord.MCID = record.MCID;
                    existingRecord.MCATEGORY = record.MCATEGORY;

                    // Update other fields as needed

                }
            }

            // Step 4: Save changes
            await _mariaContext.SaveChangesAsync();
        }

        public async Task TransferDataAsync_masitemgroups()
        {
            // Step 1: Fetch data from Oracle
            var oracleData = await _oracleContext.SmasitemgroupsDbSet.ToListAsync();

            // Step 2: Map/Transform data (if needed)
            var mariaData = oracleData.Select(item => new DmasitemgroupsDTO
            {
                GROUPID = item.GROUPID,
                GROUPCODE = item.GROUPCODE,
                GROUPNAME = item.GROUPNAME,
                REFITEMGROUPID = item.REFITEMGROUPID,
                ENTRY_DATE = item.ENTRY_DATE,


                // Map other fields as needed
            }).ToList();

            // Step 3: Upsert data into MariaDB
            foreach (var record in mariaData)
            {
                var existingRecord = await _mariaContext.DmasitemgroupsDbSet
                    .FirstOrDefaultAsync(x => x.GROUPID == record.GROUPID);

                if (existingRecord == null)
                {
                    // Insert new record
                    await _mariaContext.DmasitemgroupsDbSet.AddAsync(record);
                }
                else
                {
                    // Update existing record
                    existingRecord.GROUPID = record.GROUPID;
                    existingRecord.GROUPCODE = record.GROUPCODE;
                    existingRecord.GROUPNAME = record.GROUPNAME;
                    existingRecord.REFITEMGROUPID = record.REFITEMGROUPID;
                    existingRecord.ENTRY_DATE = record.ENTRY_DATE;

                    // Update other fields as needed

                }
            }

            // Step 4: Save changes
            await _mariaContext.SaveChangesAsync();
        }

        public async Task TransferDataAsync_masfacilitywards()
        {

           
            // Step 1: Fetch data from Oracle
            var oracleData = await _oracleContext.SmasfacilitywardsDbSet
                 .Where(item => item.FACILITYID == 0).ToListAsync();

            // Step 2: Map/Transform data (if needed)
            var mariaData = oracleData.Select(item => new DmasfacilitywardsDTO
            {
                WARDID = item.WARDID,
                FACILITYID = item.FACILITYID,
                WARDCODE = item.WARDCODE,
                WARDNAME = item.WARDNAME,
                ISACTIVE = item.ISACTIVE,
                MCWARDID = item.MCWARDID,
                PWD = item.PWD,
                ROLE = item.ROLE,
                ROLEID = item.ROLEID,
                ISDH = item.ISDH,
                ISVISIBLE = item.ISVISIBLE,
                ENTRY_DATE = item.ENTRY_DATE,
                APPROLE = item.APPROLE


                // Map other fields as needed
            }).ToList();

            // Step 3: Upsert data into MariaDB
            foreach (var record in mariaData)
            {
                var existingRecord = await _mariaContext.DmasfacilitywardsDbSet
                    .FirstOrDefaultAsync(x => x.WARDID == record.WARDID);

                if (existingRecord == null)
                {
                    // Insert new record
                    await _mariaContext.DmasfacilitywardsDbSet.AddAsync(record);
                }
                else
                {
                    // Update existing record
                    existingRecord.WARDID = record.WARDID;
                    existingRecord.FACILITYID = record.FACILITYID;
                    existingRecord.WARDCODE = record.WARDCODE;
                    existingRecord.WARDNAME = record.WARDNAME;
                    existingRecord.ISACTIVE = record.ISACTIVE;
                    existingRecord.MCWARDID = record.MCWARDID;
                    existingRecord.PWD = record.PWD;
                    existingRecord.ROLE = record.ROLE;
                    existingRecord.ROLEID = record.ROLEID;
                    existingRecord.ISDH = record.ISDH;
                    existingRecord.ISVISIBLE = record.ISVISIBLE;
                    existingRecord.ENTRY_DATE = record.ENTRY_DATE;
                    existingRecord.APPROLE = record.APPROLE;


                    // Update other fields as needed

                }
            }

            // Step 4: Save changes
            await _mariaContext.SaveChangesAsync();
        }

        public async Task TransferDataAsync_masprogram()
        {


            // Step 1: Fetch data from Oracle
            var oracleData = await _oracleContext.SmasprogramDbSet.ToListAsync();

            // Step 2: Map/Transform data (if needed)
            var mariaData = oracleData.Select(item => new DmasprogramDTO
            {
                PROGRAMID = item.PROGRAMID,
                PROGRAM = item.PROGRAM,
                ORDERID = item.ORDERID,
                ENTRYDT = item.ENTRYDT,
                LASTUPDATEDT = item.LASTUPDATEDT,
                ISACTIVE = item.ISACTIVE,

                // Map other fields as needed
            }).ToList();

            // Step 3: Upsert data into MariaDB
            foreach (var record in mariaData)
            {
                var existingRecord = await _mariaContext.DmasprogramDbSet
                    .FirstOrDefaultAsync(x => x.PROGRAMID == record.PROGRAMID);

                if (existingRecord == null)
                {
                    // Insert new record
                    await _mariaContext.DmasprogramDbSet.AddAsync(record);
                }
                else
                {
                    // Update existing record
                    existingRecord.PROGRAMID = record.PROGRAMID;
                    existingRecord.PROGRAM = record.PROGRAM;
                    existingRecord.ORDERID = record.ORDERID;
                    existingRecord.ENTRYDT = record.ENTRYDT;
                    existingRecord.LASTUPDATEDT = record.LASTUPDATEDT;
                    existingRecord.ISACTIVE = record.ISACTIVE;



                    // Update other fields as needed

                }
            }

            // Step 4: Save changes
            await _mariaContext.SaveChangesAsync();
        }

        public async Task TransferDataAsync_masaccyearsettings()
        {


            // Step 1: Fetch data from Oracle
            var oracleData = await _oracleContext.SmasaccyearsettingsDbSet.ToListAsync();

            // Step 2: Map/Transform data (if needed)
            var mariaData = oracleData.Select(item => new DmasaccyearsettingsDTO
            {
                ACCYRSETID = item.ACCYRSETID,
                ACCYEAR = item.ACCYEAR,
                STARTDATE = item.STARTDATE,
                ENDDATE = item.ENDDATE,
                SHACCYEAR = item.SHACCYEAR,
                YEARORDER = item.YEARORDER,
                ENTRYDT = item.ENTRYDT,
                LASTUPDATEDT = item.LASTUPDATEDT

                // Map other fields as needed
            }).ToList();

            // Step 3: Upsert data into MariaDB
            foreach (var record in mariaData)
            {
                var existingRecord = await _mariaContext.DmasaccyearsettingsDbSet
                    .FirstOrDefaultAsync(x => x.ACCYRSETID == record.ACCYRSETID);

                if (existingRecord == null)
                {
                    // Insert new record
                    await _mariaContext.DmasaccyearsettingsDbSet.AddAsync(record);
                }
                else
                {
                    // Update existing record
                    existingRecord.ACCYRSETID = record.ACCYRSETID;
                    existingRecord.ACCYEAR = record.ACCYEAR;
                    existingRecord.STARTDATE = record.STARTDATE;
                    existingRecord.ENDDATE = record.ENDDATE;
                    existingRecord.SHACCYEAR = record.SHACCYEAR;
                    existingRecord.YEARORDER = record.YEARORDER;
                    existingRecord.ENTRYDT = record.ENTRYDT;
                    existingRecord.LASTUPDATEDT = record.LASTUPDATEDT;




                    // Update other fields as needed

                }
            }

            // Step 4: Save changes
            await _mariaContext.SaveChangesAsync();
        }

        public async Task TransferDataAsync_maslocations()
        {


            // Step 1: Fetch data from Oracle
            var oracleData = await _oracleContext.SmaslocationsDbSet.ToListAsync();

            // Step 2: Map/Transform data (if needed)
            var mariaData = oracleData.Select(item => new DmaslocationsDTO
            {
                STATEID = item.STATEID,
                DISTRICTID = item.DISTRICTID,
                LOCATIONID = item.LOCATIONID,
                LOCATIONCODE = item.LOCATIONCODE,
                LOCATIONNAME = item.LOCATIONNAME,

                // Map other fields as needed
            }).ToList();

            // Step 3: Upsert data into MariaDB
            foreach (var record in mariaData)
            {
                var existingRecord = await _mariaContext.DmaslocationsDbSet
                    .FirstOrDefaultAsync(x => x.LOCATIONID == record.LOCATIONID);

                if (existingRecord == null)
                {
                    // Insert new record
                    await _mariaContext.DmaslocationsDbSet.AddAsync(record);
                }
                else
                {
                    // Update existing record
                    existingRecord.STATEID = record.STATEID;
                    existingRecord.DISTRICTID = record.DISTRICTID;
                    existingRecord.LOCATIONID = record.LOCATIONID;
                    existingRecord.LOCATIONCODE = record.LOCATIONCODE;
                    existingRecord.LOCATIONNAME = record.LOCATIONNAME;

                    // Update other fields as needed

                }
            }

            // Step 4: Save changes
            await _mariaContext.SaveChangesAsync();
        }

        public async Task TransferDataAsync_usrroles()
        {


            // Step 1: Fetch data from Oracle
            var oracleData = await _oracleContext.SusrrolesDbSet.ToListAsync();

            // Step 2: Map/Transform data (if needed)
            var mariaData = oracleData.Select(item => new DusrrolesDTO
            {
                ROLEID = item.ROLEID,
                ROLECODE = item.ROLECODE,
                ROLENAME = item.ROLENAME,
                USERTYPE = item.USERTYPE,
                ALLOWCHANGEPWD = item.ALLOWCHANGEPWD, // Convert TINYINT(1) to bool
                ISHOUSER = item.ISHOUSER,
                ENTRYDATE = item.ENTRYDATE

                // Map other fields as needed
            }).ToList();

            // Step 3: Upsert data into MariaDB
            foreach (var record in mariaData)
            {
                var existingRecord = await _mariaContext.DusrrolesDbSet
                    .FirstOrDefaultAsync(x => x.ROLEID == record.ROLEID);

                if (existingRecord == null)
                {
                    // Insert new record
                    await _mariaContext.DusrrolesDbSet.AddAsync(record);
                }
                else
                {
                    // Update existing record
                    existingRecord.ROLEID = record.ROLEID;
                    existingRecord.ROLECODE = record.ROLECODE;
                    existingRecord.ROLENAME = record.ROLENAME;
                    existingRecord.USERTYPE = record.USERTYPE;
                    existingRecord.ALLOWCHANGEPWD = record.ALLOWCHANGEPWD;
                    existingRecord.ISHOUSER = record.ISHOUSER;
                    existingRecord.ENTRYDATE = record.ENTRYDATE;

                    // Update other fields as needed

                }
            }

            // Step 4: Save changes
            await _mariaContext.SaveChangesAsync();
        }

        public async Task TransferDataAsync_masedl()
        {


            // Step 1: Fetch data from Oracle
            var oracleData = await _oracleContext.SmasedlDbSet.ToListAsync();

            // Step 2: Map/Transform data (if needed)
            var mariaData = oracleData.Select(item => new DmasedlDTO
            {
                EDLCAT = item.EDLCAT,
                EDLNAME = item.EDLNAME,
                EDL = item.EDL

                // Map other fields as needed
            }).ToList();

            // Step 3: Upsert data into MariaDB
            foreach (var record in mariaData)
            {
                var existingRecord = await _mariaContext.DmasedlDbSet
                    .FirstOrDefaultAsync(x => x.EDL == record.EDL);

                if (existingRecord == null)
                {
                    // Insert new record
                    await _mariaContext.DmasedlDbSet.AddAsync(record);
                }
                else
                {
                    // Update existing record
                    existingRecord.EDLCAT = record.EDLCAT;
                    existingRecord.EDLNAME = record.EDLNAME;
                    existingRecord.EDL = record.EDL;


                    // Update other fields as needed

                }
            }

            // Step 4: Save changes
            await _mariaContext.SaveChangesAsync();
        }

        public async Task TransferDataAsync_masfacheaderfooter()
        {


            // Step 1: Fetch data from Oracle
            var oracleData = await _oracleContext.SmasfacheaderfooterDbSet.ToListAsync();

            // Step 2: Map/Transform data (if needed)
            var mariaData = oracleData.Select(item => new DmasfacheaderfooterDTO
            {
                USERID = item.USERID,
                HEADER1 = item.HEADER1,
                HEADER2 = item.HEADER2,
                HEADER3 = item.HEADER3,
                FOOTER1 = item.FOOTER1,
                FOOTER2 = item.FOOTER2,
                FOOTER3 = item.FOOTER3,
                DRMOBILE = item.DRMOBILE,
                DRNAME = item.DRNAME,
                EMAIL = item.EMAIL

                // Map other fields as needed
            }).ToList();

            // Step 3: Upsert data into MariaDB
            foreach (var record in mariaData)
            {
                var existingRecord = await _mariaContext.DmasfacheaderfooterDbSet
                    .FirstOrDefaultAsync(x => x.USERID == record.USERID);

                if (existingRecord == null)
                {
                    // Insert new record
                    await _mariaContext.DmasfacheaderfooterDbSet.AddAsync(record);
                }
                else
                {
                    // Update existing record
                    existingRecord.USERID = record.USERID;
                    existingRecord.HEADER1 = record.HEADER1;
                    existingRecord.HEADER2 = record.HEADER2;
                    existingRecord.HEADER3 = record.HEADER3;
                    existingRecord.FOOTER1 = record.FOOTER1;
                    existingRecord.FOOTER2 = record.FOOTER2;
                    existingRecord.FOOTER3 = record.FOOTER3;
                    existingRecord.DRMOBILE = record.DRMOBILE;
                    existingRecord.DRNAME = record.DRNAME;
                    existingRecord.EMAIL = record.EMAIL;



                    // Update other fields as needed

                }
            }

            // Step 4: Save changes
            await _mariaContext.SaveChangesAsync();
        }

        public async Task TransferDataAsync_Freeze_MASCGMSCNOC(Int64 nocid)
        {

            string qry = "  update MASCGMSCNOC set NOCDATE=curdate(), STATUS = 'C',ENTRY_DATE=curdate() where nocid = " + nocid;
            _mariaContext.Database.ExecuteSqlRaw(qry);



            string query = "SELECT * FROM MASCGMSCNOC WHERE NOCID = @nocid";
            var mariaData = await _mariaContext.MDB_mascgmscnocDbSet.FromSqlRaw(query, new MySqlParameter("@nocid", nocid)).ToListAsync();

            // Step 2: Map/Transform data (if needed)
            var oracleData = mariaData.Select(item => new ORA_mascgmscnocDTO
            {
                NOCID = (int?) item.NOCID,
                NOCDATE = Convert.ToDateTime(item.NOCDATE),
                FACILITYID = (int?)item.FACILITYID,
                ACCYRSETID = (int?)item.ACCYRSETID,
                STATUS = item.STATUS,
                NOCNUMBER = item.NOCNUMBER,
                AUTO_NCCODE =Convert.ToInt16(item.AUTO_NCCODE),               
                PROGRAMID = (int?) item.PROGRAMID,                
                ENTRY_DATE = item.ENTRY_DATE,               
                ISUSEAPP = item.ISUSEAPP,               
                MNOCID = nocid               
            }).ToList();

            // Step 3: Upsert data into Oracle
            foreach (var record in oracleData)
            {               
                    // Insert new record
                    await _oracleContext.ORA_mascgmscnocDbSet.AddAsync(record);
                    await _oracleContext.SaveChangesAsync();
            }


            var valuenocid = await GetNOCIDByMNOCIDAsync(nocid);

            if (!valuenocid.HasValue)           
            {
                return; //valuenocid not found
            }



            string query1 = "SELECT * FROM MASCGMSCNOCITEMS WHERE NOCID = @nocid";
            var mariaData1 = await _mariaContext.Integration_MasCgmscNocItemsDbSet.FromSqlRaw(query1, new MySqlParameter("@nocid", nocid)).ToListAsync();

            // Step 2: Map/Transform data (if needed)
            var oracleData1 = mariaData1.Select(item => new ORAIntegration_MasCgmscNocItemsDTO
            {
                SR = item.SR,
                ITEMID = item.ITEMID,
                NOCID = valuenocid,
                REQUESTEDQTY = item.REQUESTEDQTY,
                WHSTOCK = item.WHSTOCK,
                CGMSCLREMARKS = item.CGMSCLREMARKS,
                STATUS = item.STATUS,
                APPROVEDQTY = item.APPROVEDQTY,
                BOOKEDQTY = item.BOOKEDQTY,
                BOOKEDFLAG = item.BOOKEDFLAG,
                STOCKINHAND = item.STOCKINHAND
            }).ToList();

            // Step 3: Upsert data into MariaDB
            foreach (var record in oracleData1)
            {
                // Insert new record
                await _oracleContext.ORAIntegration_MasCgmscNocItemsDbSet.AddAsync(record);
                await _oracleContext.SaveChangesAsync();
            }


            

        }

        public async Task<Int64?> GetNOCIDByMNOCIDAsync(Int64 mnocid)
        {
            var query = "SELECT NOCID FROM MASCGMSCNOC WHERE MNOCID = :mnocid";

            // Ensure the DbConnection remains open
            var connection = _oracleContext.Database.GetDbConnection();
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    await connection.OpenAsync();
                }

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    command.Parameters.Add(new OracleParameter("mnocid", mnocid));

                    var result = await command.ExecuteScalarAsync();

                    return result == DBNull.Value ? (Int64?)null : Convert.ToInt64(result);
                }
            }
            finally
            {
                // Explicitly close the connection if it was opened here
                if (connection.State == ConnectionState.Open)
                {
                    await connection.CloseAsync();
                }
            }
        }

        //public async Task<Int64?> GetNOCIDByMNOCIDAsync(Int64 mnocid)
        //{
        //    var nocid = await _oracleContext.ValueMascgmscnocDbSet
        //        .Where(item => item.MNOCID == mnocid)  // Filtering by MNOCID
        //        .Select(item => item.MNOCID)           // Selecting only the NOCID
        //        .FirstOrDefaultAsync();               // Returns the first matching or default if not found

        //    return nocid;
        //}
    }

}
