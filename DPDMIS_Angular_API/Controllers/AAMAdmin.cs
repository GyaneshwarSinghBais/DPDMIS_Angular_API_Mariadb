using CgmscHO_API.Utility;
using DPDMIS_Angular_API.Data;
using DPDMIS_Angular_API.DTO.AAMAdminDTO;
using DPDMIS_Angular_API.DTO.IndentDTO;
using DPDMIS_Angular_API.DTO.IssueDTO;
using DPDMIS_Angular_API.DTO.ReceiptDTO;
using DPDMIS_Angular_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public AAMAdmin(MariaDbContext context)
        {
            _context = context;
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
                   where fs.status = 'C'   --and fs.facilityid="" + faclityId + @""          
                   group by fsi.itemid,fs.facilityid,ftbo.inwno                     
                 ) iq on b.inwno = Iq.inwno and iq.itemid=i.itemid and iq.facilityid=t.facilityid                 
                 Where 1=1 and f.is_aam = 'Y' and  T.Status = 'C'  And (b.Whissueblock = 0 or b.Whissueblock is null) and b.expdate>sysdate 
                               group by  t.facilityid, mi.itemid
                           
                           ) st on st.facilityid=f.facilityid and st.itemid=f.itemid
                           
         where 1=1 and f.districtid=" + distId + @" -- and f.facilityid=27952
         )
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
                 (case when (b.qastatus ='1' or  mi.qctest='N') then (sum(nvl(b.absrqty,0)) - sum(nvl(iq.issueqty,0))) end) ReadyForIssue                 
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
                 Where 1=1 and f.is_aam = 'Y' and  T.Status = 'C'  And (b.Whissueblock = 0 or b.Whissueblock is null) and b.expdate>sysdate 
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

    }


}
