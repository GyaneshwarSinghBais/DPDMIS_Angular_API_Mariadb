using CgmscHO_API.Utility;
using DPDMIS_Angular_API.Data;
using DPDMIS_Angular_API.DTO.AAMAdminDTO;
using DPDMIS_Angular_API.DTO.IndentDTO;
using DPDMIS_Angular_API.DTO.IssueDTO;
using DPDMIS_Angular_API.DTO.ReceiptDTO;
using DPDMIS_Angular_API.MariadbDTO.DestinationDTO;
using DPDMIS_Angular_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Runtime.CompilerServices;

namespace DPDMIS_Angular_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AAMAdmin : ControllerBase
    {
        private readonly MariaDbContext _context;
        private readonly OraDbContext _oraContext;
        public AAMAdmin(MariaDbContext context, OraDbContext oraContext)
        {
            _context = context;
            _oraContext = oraContext;
        }

        [HttpGet("getUserDataForForgotPassword")]
        public ActionResult<string> getUserDataForForgotPassword(string useremaiid)
        {
            if (string.IsNullOrEmpty(useremaiid))
            {
                return BadRequest("Email/Mobile is required");
            }

            try
            {
                FacOperations op = new FacOperations(_context);
                string result = op.getForgotData(useremaiid);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }


        // [HttpGet("getUserDataForForgotPassword")]
        // public string getUserDataForForgotPassword(string useremaiid)
        // {

        //     FacOperations op = new FacOperations(_context);
        //     string getNo = op.getForgotData(useremaiid);
        //     return getNo;

        // }

        [HttpGet("getSHCstockOut")]
        public async Task<ActionResult<IEnumerable<getLastIssueDT_DTO>>> getSHCstockOut(Int64 distId)
        {
            string qry = @" 
                           
                           
                         
        select  districtname,locationname,parentfac ,facilityname,count(distinct itemid) as nositems, sum(stkavil) as stkavil,sum(stockout) as stockout,facilityid,districtid
    from 
    (
                select f.facilityname,f.facilityid,f.itemid, nvl(st.ReadyForIssue,0) as cstock , d.districtname,d.districtid,case when nvl(st.ReadyForIssue,0)>0 then 1 else 0 end as stkavil
                ,case when nvl(st.ReadyForIssue,0)>0 then 0 else 1 end as stockout
                ,f.PHC_ID,nvl(p.FACILITYNAME,'Not Linked') as parentfac,f.LOCATIONID,l.locationname
                from 
                (
                select f.facilityname,f.PHC_ID,f.LOCATIONID,f.facilityid,m.itemid,f.districtid from masfacilities f ,masitems m 
               where     1=1  and f.districtid = " + distId + @" and f.is_aam = 'Y' and m.SHC='Y'
                ) f
                            inner join masdistricts d on d.districtid = f.districtid
                           left outer join masfacilities p on p.facilityid=f.PHC_ID
                           left outer join maslocations l on l.LOCATIONID=f.LOCATIONID
 left outer join 
                           (
                           select   
                (sum(nvl(b.absrqty,0)) - sum(nvl(iq.issueqty,0)))  ReadyForIssue                 
                 ,t.facilityid, mi.itemid
                 from tbfacilityreceiptbatches b   
                 inner join tbfacilityreceiptitems i on b.facreceiptitemid=i.facreceiptitemid 
                 inner join tbfacilityreceipts t on t.facreceiptid=i.facreceiptid 
                 inner join masfacilities f on f.facilityid= t.facilityid
                 inner join vmasitems mi on mi.itemid=i.itemid 
                 
                          left outer join 
                 (  
                   select  fs.facilityid,fsi.itemid,ftbo.inwno,sum(nvl(ftbo.issueqty,0)) issueqty   
                     from tbfacilityissues fs 
                   inner join tbfacilityissueitems fsi on fsi.issueid=fs.issueid 
                   inner join tbfacilityoutwards ftbo on ftbo.issueitemid=fsi.issueitemid 
                   where fs.status = 'C'            
                   group by fsi.itemid,fs.facilityid,ftbo.inwno                     
                 ) iq on b.inwno = Iq.inwno and iq.itemid=i.itemid and iq.facilityid=t.facilityid                 
                 Where 1=1 and f.is_aam = 'Y' and  T.Status = 'C'  And (b.Whissueblock = 0 or b.Whissueblock is null) and b.expdate>sysdate() 
                               group by  t.facilityid, mi.itemid
                           
                           ) st on st.facilityid=f.facilityid and st.itemid=f.itemid
                           
         where 1=1 and f.districtid=" + distId + @" -- and f.facilityid=27952
         ) stkout
               group by districtname ,districtid,facilityname,facilityid,parentfac,locationname
               order by locationname,parentfac
                           
                           
                           
                     ";
            var myList = _context.getLastIssueDT_DbSet
           .FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList();

            return myList;

        }

        [HttpGet("getFacilityItemWiseStock")]
        public async Task<ActionResult<IEnumerable<getFacilityItemWiseStockDTO>>> getFacilityItemWiseStock(Int64 distId, Int64 facId, Int64 itemId)
        {

            string whDistId = "";
            string whfacId = "";
            string whitemId = "";

            if (distId != 0)
            {
                whDistId = " and d.districtid=" + distId + " ";
            }

            if (facId != 0)
            {
                whfacId = " and f.facilityid=" + facId + "  ";
            }

            if (itemId != 0)
            {
                whitemId = "  and mi.itemid=" + itemId + " ";
            }

            string qry = @" 
select c.categoryname,mi.ITEMCODE,ty.itemtypename,mi.itemname,mi.strength1,  
                 (case when (b.qastatus ='1' or  mi.qctest='N') then (sum(ifnull(b.absrqty,0)) - sum(nvl(iq.issueqty,0))) end) ReadyForIssue                 
                 ,t.facilityid, mi.itemid,c.categoryid, case when mi.ISEDL2021='Y' then 'EDL' else 'Non EDL' end as EDLType
                ,f.facilityname,d.districtname,l.locationname,d.districtid,l.locationid
                 from tbfacilityreceiptbatches b   
                 inner join tbfacilityreceiptitems i on b.facreceiptitemid=i.facreceiptitemid 
                 inner join tbfacilityreceipts t on t.facreceiptid=i.facreceiptid  
                 inner join vmasitems mi on mi.itemid=i.itemid 
                 left outer join masitemcategories c on c.categoryid=mi.categoryid
                 left outer join masitemtypes ty on ty.itemtypeid=mi.itemtypeid
                  inner join masfacilities f on f.facilityid=t.facilityid
    inner join masfacilitytypes ft on ft.FACILITYTYPEID = f.FACILITYTYPEID
      inner join masdistricts d on d.districtid = f.districtid
                            left outer join maslocations l on l.LOCATIONID=f.LOCATIONID
                            left outer join masfacilities p on p.facilityid=f.PHC_ID
                 
                 left outer join 
                 (  
                   select  fs.facilityid,fsi.itemid,ftbo.inwno,sum(nvl(ftbo.issueqty,0)) issueqty   
                     from tbfacilityissues fs 
                   inner join tbfacilityissueitems fsi on fsi.issueid=fs.issueid 
                   inner join tbfacilityoutwards ftbo on ftbo.issueitemid=fsi.issueitemid 
                   where fs.status = 'C'     
                   group by fsi.itemid,fs.facilityid,ftbo.inwno                     
                 ) iq on b.inwno = Iq.inwno and iq.itemid=i.itemid and iq.facilityid=t.facilityid                 
                 Where 1=1 and f.is_aam = 'Y' and  T.Status = 'C'  And (b.Whissueblock = 0 or b.Whissueblock is null) and b.expdate>sysdate() 
             " + whfacId + @"  
  " + whitemId + @"
" + whDistId + @"
                and (   (case when (b.qastatus ='1' or  mi.qctest='N') then (nvl(b.absrqty,0) - nvl(iq.issueqty,0)) end)) > 0
                group by  mi.ITEMCODE, t.facilityid, mi.itemid,b.qastatus,mi.qctest,mi.itemname,mi.strength1,c.categoryname,c.categoryid,itemtypename,
                mi.ISEDL2021,f.facilityname,d.districtname,l.locationname,d.districtid,l.locationid
                order by  mi.itemname
                           
                           
                           
                     ";
            var myList = _context.getFacilityItemWiseStockDbSet
           .FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList();

            return myList;

        }


        [HttpGet("getFacilityWiseIssue")]
        public async Task<ActionResult<IEnumerable<getFacilityWiseIssueDTO>>> getFacilityWiseIssue(Int64 distId, Int64 facId, Int64 itemId, string fromDate, string todate)
        {

            string whDistId = "";
            string whfacId = "";
            string whitemId = "";
            string whDateBetween = "";

            if (distId != 0)
            {
                whDistId = " and d.districtid=" + distId + " ";
            }

            if (facId != 0)
            {
                whfacId = " and fs.facilityid=" + facId + "  ";
            }

            if (itemId != 0)
            {
                whitemId = "  and vm.itemid=" + itemId + " ";
            }
            if (fromDate != "0" && todate != "0")
            {
                whDateBetween = " and fs.ISSUEDATE between   '" + fromDate + @"' and    '" + todate + @"'  ";
            }

            string qry = @"   
  select   row_number() over (order by fsi.itemid) as id, fsi.itemid,vm.itemcode,vm.itemname,vm.strength1,sum(nvl(ftbo.issueqty,0)) issueqty ,fs.ISSUEDATE 
  ,fs.WardID,b.WardName
  ,0 as inwno,0 as batchno,'' as mfgdate,'' as expdate
  ,f.facilityname,f.facilityid,d.districtname,l.locationname,d.districtid,l.locationid
    from tbfacilityissues fs 
    inner join masfacilities f on f.facilityid=fs.facilityid
    inner join masfacilitytypes ft on ft.FACILITYTYPEID = f.FACILITYTYPEID
      inner join masdistricts d on d.districtid = f.districtid
                            left outer join maslocations l on l.LOCATIONID=f.LOCATIONID
                            left outer join masfacilities p on p.facilityid=f.PHC_ID
 Inner Join masFacilityWards b on (b.WardID=fs.WardID)
  inner join tbfacilityissueitems fsi on fsi.issueid=fs.issueid 
  inner join vmasitems vm on vm.itemid=fsi.itemid
  inner join tbfacilityoutwards ftbo on ftbo.issueitemid=fsi.issueitemid 
  where fs.status = 'C'  
  " + whfacId + @"  
 " + whitemId + @" 
" + whDistId + @"
  and f.is_aam = 'Y'
and fs.ISSUETYPE='NO'    
" + whDateBetween + @" 
  group by fsi.itemid,fs.ISSUEDATE,vm.itemcode,vm.itemname,vm.strength1,fs.WardID,b.WardName,f.facilityname,f.facilityid
  ,d.districtname,l.locationname,d.districtid,l.locationid
  order by fs.ISSUEDATE                          
                           
                           
                     ";
            var myList = _context.getFacilityWiseIssueDbSet
           .FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList();

            return myList;

        }


        [HttpGet("KPIdistWise")]
        public async Task<ActionResult<IEnumerable<KPIdistWiseDTO>>> KPIdistWise()
        {
            string qry = @"     select districtname,count(FACILITYID) as target,sum(target)  as achivement,round(sum(target)/count(FACILITYID)*100,2) as per,sum(OpStock) as OpStock
,districtid
from 
(
select nvl(nosindent,0) as Nosindent,case when lastissueDt is null then 0 else 1 end as Consumptiondoing
,case when r.FACRECEIPTID is null then 0 else 1 end as receipt,
d.districtname,f.FACILITYID,d.districtid,u.userid
,case when (nvl(nosindent,0)+(case when lastissueDt is null then 0 else 1 end)+(case when r.FACRECEIPTID is null then 0 else 1 end)+nvl(oind.nosotherfacindent,0))>1 then 1 else 0 end as target
,case when r2.FACRECEIPTID is null then 0 else 1 end as OpStock
from masfacilities f
                            inner join usrusers u on u.FACILITYID = f.facilityid
                            inner join masfacilitytypes ft on ft.FACILITYTYPEID = f.FACILITYTYPEID
                            inner join masdistricts d on d.districtid = f.districtid
                            left outer join maslocations l on l.LOCATIONID=f.LOCATIONID
                            left outer join masfacilities p on p.facilityid=f.PHC_ID
                            left outer join 
                            (
                            select facilityid,count(nocid) as nosindent from mascgmscnoc faci
                            where faci.status='C' group by facilityid
                            ) ind on ind.facilityid=f.facilityid
                            left outer join 
                            (
                                select FACILITYID,count(indentid) nosotherfacindent from MASFACTRANSFERS ofi
                            where ofi.status='C' and ofi.FACILITYID=27952
                            group by FACILITYID
                             ) oind on oind.facilityid=f.facilityid
                            left outer join 
                            (
                                 select  max(tb.issueddate) as lastissueDt,tb.facilityid       from tbfacilityissues tb
                                where tb.status='C' 
                                group by tb.facilityid  
                            ) iss on iss.facilityid=f.facilityid
                            
       
                            
                            left outer join 
                            (
                            select max(FACRECEIPTID) FACRECEIPTID,tb.facilityid from tbFacilityReceipts tb where STATUS ='C'
                         group by tb.facilityid
                            )r on  r.facilityid=f.facilityid
                            
                                  left outer join 
                            (
                            select FACRECEIPTID,tb.facilityid from tbFacilityReceipts tb where STATUS ='C' and facreceipttype='FC'
        
                            )r2 on  r2.facilityid=f.facilityid
                                                      
                            where 1=1 and f.is_aam = 'Y'
                            ) group by districtname,districtid
                            order by districtname 
                            
                         
                           
                     ";
            var myList = _context.KPIdistWiseDbSet
           .FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList();

            return myList;

        }


        [HttpGet("KPIFacWise")]
        public async Task<ActionResult<IEnumerable<KPIFacWiseDTO>>> KPIFacWise(Int64 distId, Int64 facId)
        {

            string whDistId = "";
            string whfacId = "";


            if (distId != 0)
            {
                whDistId = " and d.districtid=" + distId + " ";
            }

            if (facId != 0)
            {
                whfacId = " and f.facilityid=" + facId + "  ";
            }



            string qry = @"    

select d.districtname,l.locationname,f.facilityname,p.facilityname as parentfac
,case when r.FACRECEIPTID is not null then 'Opening Stock Completed' else 'Opening Stock Pending' end as opstock,
f.INDENTDURATION,nvl(nosindent,0) as NosindenttoWH,
nvl(nosotherfacindent,0) as IndenttoOtherFAC,iss.lastissueDt as LastConsumptionEntry
,l.locationid,f.facilityid,d.districtid,u.userid
from masfacilities f
                            inner join usrusers u on u.FACILITYID = f.facilityid
                            inner join masfacilitytypes ft on ft.FACILITYTYPEID = f.FACILITYTYPEID
                            inner join masdistricts d on d.districtid = f.districtid
                            left outer join maslocations l on l.LOCATIONID=f.LOCATIONID
                            left outer join masfacilities p on p.facilityid=f.PHC_ID
                            left outer join 
                            (
                            select facilityid,count(nocid) as nosindent from mascgmscnoc faci
                            where faci.status='C' group by facilityid
                            ) ind on ind.facilityid=f.facilityid
                            left outer join 
                            (
                                select FACILITYID,count(indentid) nosotherfacindent from MASFACTRANSFERS ofi
                            where ofi.status='C' " + whfacId + @"
                            group by FACILITYID
                             ) oind on oind.facilityid=f.facilityid
                            left outer join 
                            (
                                 select  max(tb.issueddate) as lastissueDt,tb.facilityid       from tbfacilityissues tb
                                where tb.status='C' 
                                group by tb.facilityid  
                            ) iss on iss.facilityid=f.facilityid
                             left outer join 
                            (
                            select FACRECEIPTID,tb.facilityid from tbFacilityReceipts tb where STATUS ='C' and facreceipttype='FC'
        
                            )r on  r.facilityid=f.facilityid
                                                      
       
                            
                      
                            where 1=1 and f.is_aam = 'Y' " + whDistId + @"
                          order by p.facilityname
                            
                            
                            
                          
                             
                            
                       
                           
                     ";
            var myList = _context.KPIFacWiseDbSet
           .FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList();

            return myList;

        }


        [HttpGet("AAMHealthPerformance")]
        public async Task<ActionResult<IEnumerable<DistrictWiseAamHealthPerformanceDTO>>> DistrictWiseAamHealthPerformance()
        {

            string qry = @"  SELECT d.districtid,d.DISTRICTNAME,nooffac,IFNULL(nosop,0) AS NoofOP,IFNULL(nosIndenttowh,0) AS nosIndentWH,IFNULL(nosfacilityReceipt,0) AS nosfacilityReceipt
FROM 
(
select d.districtid,d.DISTRICTNAME,count(distinct f.facilityid) nooffac from masfacilities f
inner join maslocations l on l.locationid = f.locationid
inner join masdistricts d on d.districtid = f.districtid


where f.is_aam = 'Y' AND f.isactive = 1
GROUP BY d.districtid,d.DISTRICTNAME
) d


LEFT OUTER JOIN 
(
SELECT DISTRICTID,COUNT(facilityid) AS nosop
FROM 
(
SELECT d.DISTRICTID, f.facilityid, f.facilityname,t.FACRECEIPTNO,t.FACRECEIPTTYPE,t.FACRECEIPTDATE,COUNT(DISTINCT i.itemid) noofdrugs FROM tbfacilityreceiptbatches b
INNER JOIN tbfacilityreceiptitems i ON i.FACRECEIPTITEMID = b.FACRECEIPTITEMID
INNER JOIN tbfacilityreceipts t ON t.FACRECEIPTID = i.FACRECEIPTID
INNER JOIN masfacilities f ON f.facilityid = t.FACILITYID
INNER JOIN masdistricts d ON d.DISTRICTID = f.districtid 
WHERE t.`STATUS` = 'C' AND t.facreceipttype = 'FC'
group by d.DISTRICTNAME, f.facilityname,t.FACRECEIPTNO,t.FACRECEIPTTYPE,t.FACRECEIPTDATE 
) op 
group by op.DISTRICTID
) op ON op.DISTRICTID=d.districtid

LEFT OUTER JOIN 
(
SELECT id.districtid,COUNT(facilityid) AS nosIndenttowh
FROM 
(
SELECT f.districtid,n.NOCID,f.facilityid,COUNT(ni.ITEMID) noofitems 
FROM mascgmscnoc n
INNER JOIN mascgmscnocitems ni ON ni.NOCID = n.nocid
INNER JOIN masfacilities f ON f.facilityid = n.facilityid
WHERE f.facilitytypeid = 377 and n.status = 'C'
GROUP BY n.NOCID,f.facilityid ,f.districtid  
) id  GROUP BY id.districtid

) id ON id.districtid=d.DISTRICTID


LEFT OUTER JOIN 
(
SELECT districtid,COUNT(facilityid) AS nosfacilityReceipt
FROM 
(
SELECT f.districtid,f.facilityid FROM tbfacilityreceipts r
INNER JOIN masfacilities f ON f.facilityid = r.FACILITYID
WHERE r.FACRECEIPTTYPE='NO' AND r.`STATUS`='C'
) r 
GROUP BY districtid
) rec ON rec.districtid=d.DISTRICTID


ORDER BY d.DISTRICTNAME
                            
                       
                           
                     ";
            var myList = _context.DistrictWiseAamHealthPerformanceDbSet
           .FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList();

            return myList;

        }


        [HttpGet("KPIFacWiseDrillDown")]
        public async Task<ActionResult<IEnumerable<KPIFacWiseDrillDownDTO>>> KPIFacWiseDrillDown(Int64 distId)
        {

            string whDistId = "";
        


            if (distId != 0)
            {
                whDistId = " and d.districtid = " + distId + " ";
            }




            string qry = @" SELECT  
    d.districtid,
    d.DISTRICTNAME,
    f.facilityid,
    f.facilityname,

    IFNULL(op.noofdrugs, 0)        AS nosdrugsOpStock,
    IFNULL(id.nosindent, 0)        AS nosindent,
    IFNULL(id.NositemsIndented,0) AS NositemsIndented,
    IFNULL(rec.nosreceipt, 0)     AS nosreceipt,
    IFNULL(rec.nositemsreceipts,0) AS nositemsreceipts

FROM masfacilities f
INNER JOIN maslocations l 
    ON l.locationid = f.locationid
INNER JOIN masdistricts d 
    ON d.districtid = f.districtid

/* ---------- OPENING STOCK (FC) ---------- */
LEFT JOIN 
(
    SELECT  
        op.facilityid,
        COUNT(op.facilityid) AS noofdrugs
    FROM
    (
        SELECT  
            f.facilityid,
            t.FACRECEIPTNO,
            t.FACRECEIPTTYPE,
            t.FACRECEIPTDATE,
            COUNT(DISTINCT i.itemid) AS drugcount
        FROM tbfacilityreceiptbatches b
        INNER JOIN tbfacilityreceiptitems i 
            ON i.FACRECEIPTITEMID = b.FACRECEIPTITEMID
        INNER JOIN tbfacilityreceipts t 
            ON t.FACRECEIPTID = i.FACRECEIPTID
        INNER JOIN masfacilities f 
            ON f.facilityid = t.FACILITYID
        WHERE t.STATUS = 'C'
          AND t.facreceipttype = 'FC'
        GROUP BY  
            f.facilityid,
            t.FACRECEIPTNO,
            t.FACRECEIPTTYPE,
            t.FACRECEIPTDATE
    ) op
    GROUP BY op.facilityid
) op 
    ON op.facilityid = f.facilityid

/* ---------- INDENT ---------- */
LEFT JOIN 
(
    SELECT  
        id.facilityid,
        COUNT(id.NOCID) AS nosindent,
        SUM(id.noofitems) AS NositemsIndented
    FROM
    (
        SELECT  
            n.NOCID,
            f.facilityid,
            COUNT(ni.ITEMID) AS noofitems
        FROM mascgmscnoc n
        INNER JOIN mascgmscnocitems ni 
            ON ni.NOCID = n.nocid
        INNER JOIN masfacilities f 
            ON f.facilityid = n.facilityid
        WHERE n.status = 'C'
        GROUP BY  
            n.NOCID,
            f.facilityid
    ) id
    GROUP BY id.facilityid
) id 
    ON id.facilityid = f.facilityid

/* ---------- RECEIPT FROM WAREHOUSE ---------- */
LEFT JOIN 
(
    SELECT  
        r.facilityid,
        COUNT(DISTINCT r.FACRECEIPTID) AS nosreceipt,
        COUNT(DISTINCT i.ITEMID) AS nositemsreceipts
    FROM tbfacilityreceipts r
    INNER JOIN tbfacilityreceiptitems i 
        ON i.FACRECEIPTID = r.FACRECEIPTID
    INNER JOIN tbfacilityreceiptbatches b 
        ON b.FACRECEIPTITEMID = i.FACRECEIPTITEMID
    WHERE r.FACRECEIPTTYPE = 'NO'
      AND r.STATUS = 'C'
    GROUP BY r.facilityid
) rec 
    ON rec.facilityid = f.facilityid

/* ---------- FILTER ---------- */
WHERE 1=1"+ whDistId + @"
  AND f.is_aam = 'Y'
  AND f.isactive = 1

ORDER BY f.facilityname               
                     ";
            var myList = _context.KPIFacWiseDrillDownDbSet
           .FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList();

            return myList;

        }


        [HttpGet("KPIFacilityDetai")]
        public async Task<ActionResult<IEnumerable<KPIFacilityDetailDTO>>>
    KPIFacilityDetai(long distId)
        {
            string whDistId = "";

            if (distId != 0)
            {
                whDistId = " AND d.districtid = " + distId + " ";
            }

            string qry = @"  SELECT  d.districtid,d.DISTRICTNAME,l.LOCATIONNAME AS blockname,f.facilityid,f.facilityname,u.EMAILID, f.phone1,f.contactpersonname,
pf.facilityname AS parentFacility, w.WAREHOUSENAME,

ifnull(noofdrugs,0) AS nosdrugsOpStock,ifnull(nosindent,0) AS nosindent , ifnull(NositemsIndented,0) as NositemsIndented ,
IFNULL(nosreceipt,0) AS nosreceipt,ifnull(nositemsreceipts,0) AS nositemsreceipts
from masfacilities f
LEFT outer join maslocations l on l.locationid = f.locationid
inner join masdistricts d on d.districtid = f.districtid
INNER JOIN maswarehouses w ON w.WAREHOUSEID = d.WAREHOUSEID
INNER JOIN usrusers u ON u.FACILITYID = f.facilityid
LEFT outer  JOIN masfacilities pf ON pf.facilityid = f.phc_id

LEFT OUTER JOIN 
(
SELECT  f.facilityid,COUNT(DISTINCT i.itemid) noofdrugs FROM tbfacilityreceiptbatches b
INNER JOIN tbfacilityreceiptitems i ON i.FACRECEIPTITEMID = b.FACRECEIPTITEMID
INNER JOIN tbfacilityreceipts t ON t.FACRECEIPTID = i.FACRECEIPTID
INNER JOIN masfacilities f ON f.facilityid = t.FACILITYID
INNER JOIN masdistricts d ON d.DISTRICTID = f.districtid 
WHERE t.`STATUS` = 'C' AND t.facreceipttype = 'FC'
group by d.DISTRICTNAME, f.facilityname,t.FACRECEIPTNO,t.FACRECEIPTTYPE,t.FACRECEIPTDATE
) op ON op.facilityid=f.facilityid



LEFT OUTER JOIN 
(
SELECT facilityid,nosindent,noofitems AS NositemsIndented
FROM 
(
SELECT count(distinct n.NOCID) nosindent,f.facilityid,COUNT(ni.ITEMID) noofitems 
FROM mascgmscnoc n
INNER JOIN mascgmscnocitems ni ON ni.NOCID = n.nocid
INNER JOIN masfacilities f ON f.facilityid = n.facilityid
WHERE f.facilitytypeid = 377 and n.status = 'C'
GROUP BY n.NOCID,f.facilityid  
) id 
GROUP BY facilityid
) id ON id.facilityid=f.facilityid

LEFT OUTER JOIN 
(
SELECT r.facilityid,COUNT(DISTINCT i.itemid) nositemsreceipts,COUNT(DISTINCT r.FACRECEIPTID) AS nosreceipt FROM tbfacilityreceipts r
INNER JOIN masfacilities f ON f.facilityid = r.FACILITYID
INNER JOIN tbfacilityreceiptitems i ON i.FACRECEIPTID = r.FACRECEIPTID
INNER JOIN tbfacilityreceiptbatches b ON b.FACRECEIPTITEMID = i.FACRECEIPTITEMID
WHERE r.FACRECEIPTTYPE='NO' AND r.`STATUS`='C'
GROUP BY r.facilityid
)rec ON rec.facilityid=f.facilityid

WHERE 1=1 "+ whDistId + @"
and f.is_aam = 'Y' AND f.isactive = 1

ORDER BY l.LOCATIONNAME 

 ";

            var myList = _context.Set<KPIFacilityDetailDTO>()
                .FromSqlInterpolated(FormattableStringFactory.Create(qry))
                .AsNoTracking()
                .ToList();

            return myList;
        }


        [HttpPut("updateFacilityContact")]
        public async Task<IActionResult> UpdateFacilityContact(long facilityId, string contactPersonName, string phone1)
        {
            if (facilityId <= 0)
                return BadRequest("Valid facilityId is required.");

            contactPersonName ??= string.Empty;
            phone1 ??= string.Empty;

            // Local helper to read PHONE1 and CONTACTPERSONNAME from a DbContext without using entity mappings
            async Task<(string phone, string contact)> ReadPhoneAndContactAsync(DbContext ctx, long id)
            {
                var conn = ctx.Database.GetDbConnection();
                // don't dispose the connection supplied by DbContext
                if (conn.State != ConnectionState.Open)
                    await conn.OpenAsync();

                await using var cmd = conn.CreateCommand();

                // Detect Oracle provider (simple type/name check)
                bool isOracle = ctx.GetType().Name.IndexOf("Ora", StringComparison.OrdinalIgnoreCase) >= 0
                                || conn.GetType().Name.IndexOf("Oracle", StringComparison.OrdinalIgnoreCase) >= 0;

                string sqlParam = isOracle ? ":id" : "@id";
                cmd.CommandText = $"SELECT PHONE1, CONTACTPERSONNAME FROM MASFACILITIES WHERE FACILITYID = {sqlParam}";

                var param = cmd.CreateParameter();
                // For OracleParameter the Name should not include the ':' prefix
                param.ParameterName = isOracle ? "id" : "@id";
                param.Value = id;
                cmd.Parameters.Add(param);

                await using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    var phoneVal = reader.IsDBNull(0) ? string.Empty : reader.GetString(0);
                    var contactVal = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
                    return (phoneVal ?? string.Empty, contactVal ?? string.Empty);
                }

                return (string.Empty, string.Empty);
            }

            try
            {
                // read original values from MariaDB and Oracle using explicit SELECTs (no EF entity mapping)
                var origMaria = await ReadPhoneAndContactAsync(_context, facilityId);
                var origOra = await ReadPhoneAndContactAsync(_oraContext, facilityId);

                // Update MariaDB
                string mQuery = "UPDATE MASFACILITIES SET CONTACTPERSONNAME = @contact, PHONE1 = @phone WHERE FACILITYID = @id";
                await _context.Database.ExecuteSqlRawAsync(mQuery,
                    new MySqlParameter("@contact", contactPersonName),
                    new MySqlParameter("@phone", phone1),
                    new MySqlParameter("@id", facilityId)
                );

                // Update Oracle - if fails, revert MariaDB
                try
                {
                    string oQuery = "UPDATE MASFACILITIES SET CONTACTPERSONNAME = :contact, PHONE1 = :phone WHERE FACILITYID = :id";
                    await _oraContext.Database.ExecuteSqlRawAsync(oQuery,
                        new OracleParameter("contact", contactPersonName),
                        new OracleParameter("phone", phone1),
                        new OracleParameter("id", facilityId)
                    );
                }
                catch (Exception oraEx)
                {
                    // Attempt to revert MariaDB
                    try
                    {
                        string revertMQuery = "UPDATE MASFACILITIES SET CONTACTPERSONNAME = @contact, PHONE1 = @phone WHERE FACILITYID = @id";
                        await _context.Database.ExecuteSqlRawAsync(revertMQuery,
                            new MySqlParameter("@contact", origMaria.contact),
                            new MySqlParameter("@phone", origMaria.phone),
                            new MySqlParameter("@id", facilityId)
                        );
                    }
                    catch (Exception revertEx)
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, new
                        {
                            message = "Oracle update failed and MariaDB revert failed. Manual intervention required.",
                            oracleError = oraEx.Message,
                            revertMariaError = revertEx.Message
                        });
                    }

                    return StatusCode(StatusCodes.Status500InternalServerError, new
                    {
                        message = "Oracle update failed. MariaDB changes reverted.",
                        oracleError = oraEx.Message
                    });
                }

                return Ok(new { message = "Facility contact updated in both MariaDB and Oracle." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error: " + ex.Message);
            }
        }


    }


}
