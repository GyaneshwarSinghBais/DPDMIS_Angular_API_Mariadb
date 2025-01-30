using CgmscHO_API.Utility;
using DPDMIS_Angular_API.Data;
using DPDMIS_Angular_API.DTO.IndentDTO;
using DPDMIS_Angular_API.MariadbDTO.DestinationDTO;
using DPDMIS_Angular_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;

namespace DPDMIS_Angular_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IndentController : ControllerBase
    {
        private readonly MariaDbContext _context;

        public IndentController(MariaDbContext context)
        {
            _context = context;
        }



        [HttpGet("getMonthIndentProgram")]
        public async Task<ActionResult<IEnumerable<getIndentProgramDTO>>> getMonthIndentProgram()
        {
            string qry = @"select programid,program from masprogram where   programid in (1,4) and Isactive is null order by programid ";
            var myList = _context.getIndentProgramDbSet
            .FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList();
            return myList;
        }

        [HttpPost("postOtherFacilityIndent")]
        public async Task<ActionResult<IEnumerable<IncompleteWardIssueDTO>>> postOtherFacilityIndent(Int64 facid, string indentDt, Int64 programid, Int64 Target_facid) //programid = 1
        {
            //try
            //{
                FacOperations ob = new FacOperations(_context);
                MASFACTRANSFERS objSaveIndent = new MASFACTRANSFERS();
                string indentDate = ob.FormatDate(indentDt);

                string faccode = ob.getFacCodeForSHCIndent(facid);
                string yearcode = ob.getSHAccYear();
                string yearId = ob.getACCYRSETID();
                string autono = facid.ToString() + "/SH" + faccode + "/" + yearcode;
                //objeIssue.ISSUENO = issueno;
                objSaveIndent.INDENTDATE = Convert.ToDateTime(indentDate);
                objSaveIndent.INDENTNO = autono;
                objSaveIndent.FACILITYID = facid;
                objSaveIndent.FROMFACILITYID = Target_facid;
                objSaveIndent.ACCYRSETID = Convert.ToInt64(yearId);
                objSaveIndent.PROGRAMID = programid;
                objSaveIndent.STATUS = "I";
                objSaveIndent.AUTO_CODE = faccode;



                _context.MASFACTRANSFERSDbSet.Add(objSaveIndent);
                _context.SaveChanges();


                var myObj = GetFacDetailSHC(facid, autono);        
                return Ok(myObj);
            //}
            //catch (Exception ex)
            //{
            //    return BadRequest(ex.Message.ToString());
            //}
        }

        //[HttpGet("GetFacDetailSHC")]
        //public async Task<ActionResult<IEnumerable<MASFACTRANSFERS>>> GetFacDetailSHC(Int64 facid, string indentNo)
        //{
        //    string qry = @"SELECT INDENTID, INDENTNO, INDENTDATE, FACILITYID, FROMFACILITYID, DISPATCHNO, DISPATCHDATE, REMARKS, PROGRAMID, AUTO_CODE, ENTRYDATE, ACCYRSETID, STATUS 
        //           FROM MASFACTRANSFERS 
        //           WHERE INDENTNO = {0} AND FACILITYID = {1}";

        //    var results = await _context.MASFACTRANSFERSDbSet
        //        .FromSqlInterpolated(FormattableStringFactory.Create(qry, indentNo, facid))
        //        .ToListAsync();

        //    var dtoList = results.Select(item => new MASFACTRANSFERS
        //    {
        //        INDENTID = item.INDENTID,
        //        INDENTNO = item.INDENTNO,
        //        INDENTDATE = item.INDENTDATE,
        //        FACILITYID = item.FACILITYID,
        //        FROMFACILITYID = item.FROMFACILITYID,
        //        DISPATCHNO = item.DISPATCHNO,
        //        DISPATCHDATE = item.DISPATCHDATE,
        //        REMARKS = item.REMARKS,
        //        PROGRAMID = item.PROGRAMID,
        //        AUTO_CODE = item.AUTO_CODE,
        //        ENTRYDATE = item.ENTRYDATE,
        //        ACCYRSETID = item.ACCYRSETID,
        //        STATUS = item.STATUS
        //    }).ToList();

        //    return Ok(dtoList);
        //}


        [HttpGet("GetFacDetailSHC")]
        public async Task<ActionResult<IEnumerable<MASFACTRANSFERS_DTO>>> GetFacDetailSHC(Int64 facid, string indentNo)
        {
            string qry = @"  select INDENTID, INDENTNO, INDENTDATE, FACILITYID, FROMFACILITYID, DISPATCHNO, DISPATCHDATE, REMARKS, PROGRAMID, AUTO_CODE, ENTRYDATE, ACCYRSETID, STATUS,MTRANSFERID from MASFACTRANSFERS
        where INDENTNO = '" + indentNo + "' and FACILITYID = " + facid + " ";
            //(23413)
            var myList = _context.MASFACTRANSFERS_DbSet
            .FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList();

            return myList;
            //return Ok(JsonConvert.SerializeObject(myList));
        }


        [HttpPost("postWhIndentNo")]
        public async Task<ActionResult<IEnumerable<IncompleteWardIssueDTO>>> postWhIndentNo(Int64 facid, string indentDt, Int64 programid)
        {
            try
            {
                FacOperations ob = new FacOperations(_context);
                tbGenIndent objSaveIndent = new tbGenIndent();
                string indentDate = ob.FormatDate(indentDt);

                string faccode = ob.getFacCodeForIndent(facid);
                string yearcode = ob.getSHAccYear();
                string yearId = ob.getACCYRSETID();
                string autono = facid.ToString() + "/NC" + faccode + "/" + yearcode;
                //objeIssue.ISSUENO = issueno;
                objSaveIndent.NOCDATE = indentDate;
                objSaveIndent.NOCNUMBER = autono;
                objSaveIndent.FACILITYID = facid;
                objSaveIndent.PROGRAMID = programid;
                objSaveIndent.ACCYRSETID = Convert.ToInt64(yearId);
                objSaveIndent.ISUSEAPP = "Y";
                objSaveIndent.STATUS = "I";
                objSaveIndent.AUTO_NCCODE = faccode;


                _context.tbGenIndentDbSet.Add(objSaveIndent);
                _context.SaveChanges();


                var myObj = FacMonthIndentNo(facid.ToString(), autono);
                return Ok(myObj);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
        }

        [HttpGet("FacMonthIndent")] //mariadb

        public async Task<ActionResult<IEnumerable<FacMonthIndentDTO>>> FacMonthIndent(string facid, string istatus)
        {
            string whwarehouse = "";
            if (istatus == "I")
            {
                whwarehouse = "AND IFNULL(faci.STATUS, 'I') = 'I'";
            }
            if (istatus == "C")
            {
                whwarehouse = "AND IFNULL(faci.STATUS, 'I') = 'C'";
            }
            string qry = @"
        SELECT 
            faci.nocid, 
            faci.nocdate AS ReqDate, 
            faci.NOCNUMBER AS ReqNo, 
            IFNULL(nositemsReq, 0) AS nositemsReq, 
            tb.INDENTNO AS WHIssueNo, 
            tb.INDENTDATE AS WHIssueDT, 
            IFNULL(nositemsIssued, 0) AS NosIssued, 
            IFNULL(tb.indentid, 0) AS indentid, 
            fr.FACRECEIPTNO, 
            fr.FACRECEIPTDATE, 
            CASE 
                WHEN fr.STATUS = 'I' THEN 'IN' 
                ELSE IFNULL(fr.STATUS, 'IN') 
            END AS RStatus, 
            fr.FACRECEIPTID, 
            faci.FacilityID, 
            IFNULL(tb.warehouseid, 0) AS warehouseid, 
            IFNULL(faci.STATUS, 'I') AS IStatus
        FROM mascgmscnoc faci
        LEFT JOIN (
            SELECT 
                COUNT(DISTINCT ri.itemid) AS nositemsReq, 
                ri.nocid 
            FROM mascgmscnocitems ri
            WHERE ri.BOOKEDQTY > 0
            GROUP BY ri.nocid
        ) ri ON ri.nocid = faci.nocid
        LEFT JOIN tbIndents tb ON tb.NOCID = faci.nocid
        LEFT JOIN (
            SELECT 
                COUNT(DISTINCT tbi.itemid) AS nositemsIssued, 
                tbi.indentid 
            FROM tbIndentItems tbi
            GROUP BY tbi.indentid
        ) tbi ON tbi.indentid = tb.indentid
        LEFT JOIN tbFacilityReceipts fr 
            ON fr.FacilityID = faci.FacilityID 
            AND fr.IndentId = tb.IndentId
        WHERE faci.ACCYRSETID >= 544 
        AND faci.facilityid = " + facid + @"
        " + whwarehouse + @"
        ORDER BY faci.nocid DESC";
            //(23413)
            var myList = _context.FacMonthIndentDbSet
            .FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList();
            return myList;
        }


        [HttpGet("FacMonthIndentNo")]
        public async Task<ActionResult<IEnumerable<FacMonthIndentDTO>>> FacMonthIndentNo(string facid, string NOCNumber)
        {
            string qry = @"  select faci.nocid, faci.nocdate as ReqDate, faci.NOCNUMBER as ReqNo, nvl(nositemsReq,0) as nositemsReq, tb.INDENTNO WHIssueNo, tb.INDENTDATE as WHIssueDT, nvl(nositemsIssued,0) as NosIssued,nvl(tb.indentid,0) indentid
, fr.FACRECEIPTNO, fr.FACRECEIPTDATE,case when fr.STATUS='I' then 'IN' else  nvl(fr.STATUS,'IN') end as RStatus
, fr.FACRECEIPTID,faci.FacilityID,nvl(tb.warehouseid,0) warehouseid
,nvl(faci.STATUS,'I') as IStatus
from mascgmscnoc faci
left outer join
(
select count(distinct ri.itemid) nositemsReq, ri.nocid from mascgmscnocitems ri
where 1=1 and ri.BOOKEDQTY>0
group by ri.nocid
) ri on ri.nocid=faci.nocid
left outer join tbIndents tb on tb.NOCID=faci.nocid
left outer join
(
select count(distinct tbi.itemid) nositemsIssued, tbi.indentid from tbIndentItems tbi
group by tbi.indentid
) tbi on tbi.indentid=tb.indentid
left outer join tbFacilityReceipts fr on fr.FacilityID=faci.FacilityID and fr.IndentId= tb.IndentId

where 1=1 
and faci.ACCYRSETID>=544  and faci.facilityid =" + facid + @"
and faci.NOCNUMBER='" + NOCNumber + "' ";
            //(23413)
            var myList = _context.FacMonthIndentDbSet
            .FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList();
            return myList;
        }

        [HttpGet("getMainCategory")]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> getMainCategory(string faclityId)
        {

            FacOperations op = new FacOperations(_context);
            string qry = "";
            if (faclityId == "HOD")
            {
                qry = @"  select mcid as CATEGORYID, mcategory as CATEGORYNAME from masitemmaincategory where 1=1 ";
            }
            else if (faclityId != "HOD")
            {
                Int32 hodid = op.geFACHOID(faclityId);
                string whcatid = "";

                if (hodid == 7)
                {
                    qry = @"  select subcatid as CATEGORYID,CATNAME  as CATEGORYNAME from massubitemcategory ";

                }
                else
                {

                    qry = @"  select mcid as CATEGORYID, mcategory as CATEGORYNAME from masitemmaincategory where mcid not in (4)";
                }
            }
            else
            {
                qry = @"  select mcid as CATEGORYID, mcategory as CATEGORYNAME from masitemmaincategory where 1=1 ";
            }

            var context = new CategoryDTO();

            var myList = _context.CategoryDTODbSet
            .FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList();

            return myList;
        }

        [HttpGet("getFacMonthIndentItems")]
        public async Task<ActionResult<IEnumerable<IndentItemsFromWardDTO>>> getFacMonthIndentItems(string faclityId, string Mcatid, string indendid)
        {

            FacOperations f = new FacOperations(_context);
            Int64 WHIID = f.getWHID(faclityId);
            Int64 EDLCAt = f.geFACEDLCat(faclityId);
            Int32 hodid = f.geFACHOID(faclityId);
            string whmcatid = "";
            if (Mcatid != "0")
            {
                if (hodid == 7)
                {
                    whmcatid = " and msc.subcatid in (" + Mcatid + ") ";
                }
                else
                {
                    whmcatid = " and mc.MCID in (" + Mcatid + ")";
                }
            }

            string qry = @" select A.itemname || '-' || A.itemcode as name,ItemID
                  from
                 (
                 select e.edlcat, e.edl, w.WAREHOUSENAME, mi.ITEMCODE, b.inwno, mi.ITEMNAME, mi.strength1, mi.unit as SKU,
               (case when b.qastatus = '1' then(nvl(b.absrqty, 0) - nvl(iq.issueqty, 0)) else (case when mi.Qctest = 'N' then(nvl(b.absrqty, 0) - nvl(iq.issueqty, 0))  end ) end ) ReadyForIssue,  
                   case when mi.qctest = 'N' then 0 else (case when b.qastatus = 0 or b.qastatus = 3 then(nvl(b.absrqty, 0) - nvl(iq.issueqty, 0)) end) end Pending
, mi.unitcount,mi.itemid
                  from tbreceiptbatches b
                  inner join tbreceiptitems i on b.receiptitemid = i.receiptitemid
                  inner join tbreceipts t on t.receiptid = i.receiptid
                  inner join masitems mi on mi.itemid = i.itemid
                          left outer join masedl e on e.edlcat = mi.edlcat
                  inner join masitemcategories c on c.categoryid = mi.categoryid
                  inner join masitemmaincategory mc on mc.MCID = c.MCID
                  left outer join massubitemcategory msc on msc.categoryid = c.categoryid
                 inner join MASWAREHOUSES w  on w.warehouseid = t.warehouseid
                 left outer join
                (
                   select tb.warehouseid, tbi.itemid, tbo.inwno, sum(nvl(tbo.issueqty, 0)) issueqty
                   from tboutwards tbo, tbindentitems tbi, tbindents tb
                   where 1 = 1 and  tbo.indentitemid = tbi.indentitemid and tbi.indentid = tb.indentid and tb.status = 'C' and tb.notindpdmis is null and tbo.notindpdmis is null and tbi.notindpdmis is null
                   and tb.warehouseid = " + WHIID + @"
                   group by tbi.itemid, tb.warehouseid, tbo.inwno
                 ) iq on b.inwno = Iq.inwno and iq.itemid = i.itemid and iq.warehouseid = t.warehouseid
                 Where T.Status = 'C' and(b.ExpDate >= SysDate or nvl(b.ExpDate, SysDate) >= SysDate) And(b.Whissueblock = 0 or b.Whissueblock is null)
              and mc.MCID in (1) and nvl(mi.edlcat,0)<= " + EDLCAt + @"
                  and w.warehouseid = " + WHIID + @" and((case when b.qastatus = '1' then(nvl(b.absrqty, 0) - nvl(iq.issueqty, 0)) else (case when mi.Qctest = 'N' then(nvl(b.absrqty, 0) - nvl(iq.issueqty, 0))  end ) end ) )> 0
                 and t.notindpdmis is null and b.notindpdmis is null and i.notindpdmis is null
                 ) A
                where 1 = 1
                and A.itemid not in (select ci.itemid from mascgmscnoc c
inner join mascgmscnocitems ci on ci.nocid = c.nocid
where c.facilityid = " + faclityId + @" and ci.nocid = " + indendid + @")
                 group by WAREHOUSENAME, A.itemcode,A.ItemName, A.strength1,A.SKU ,A.unitcount,A.Itemid,A.edlcat,A.edl
                  having  sum(A.ReadyForIssue) > 0
                 order by A.ItemName ";
            var myList = _context.IndentItemsFromWardDbSet
           .FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList();
            return myList;
        }

        [HttpGet("FacIndentAlert")]

        public async Task<ActionResult<IEnumerable<IndentAlertNewDTO>>> FacIndentAlert(string facid, string mcatid, string isEDL, string itemid)
        {
            FacOperations op = new FacOperations(_context);
            Int64 WHIID = op.getWHID(facid);
            Int32 hodid = op.geFACHOID(facid);
            string yearid = op.getACCYRSETID();
            string whmcatid = "";
            if (mcatid != "0")
            {
                if (hodid == 7)
                {
                    whmcatid = " and msc.subcatid in (" + mcatid + ") ";
                }
                else
                {
                    whmcatid = " and mc.MCID in (" + mcatid + ")";
                }
            }
            string edlcaluse = "";

            if (isEDL == "Y")
            {
                edlcaluse = " and m.isedl2021 = 'Y' ";
            }
            if (isEDL == "N")
            {
                edlcaluse = " and (case when m.isedl2021 = 'Y' then 'EDL' else 'Non EDL' end)= 'Non EDL' ";
            }


            string whitemcase = "";
            if (itemid != "0")
            {
                whitemcase = " and itemid = " + itemid;
            }
            else
            {
                whitemcase = " and Remarks='Need To Be Indent to CGMSC'";
            }

            string qry = @" select itemid,MCID, MCATEGORY, itemcode, edlCatName, itemtypename, EDL, itemname, strength1, AIFacility, ApprovedAICMHO, FACReqFor3Month, facstock, FACReqFor3Month-facstock as RequiredQTY,WIssueQTYYear,WHReady,Remarks,ITEMTYPEID,edlcat,unitcount, nvl(ApprovedAICMHO,0)-nvl(WIssueQTYYear,0) as BalAI  from
                (
        select m.itemcode, ty.itemtypename, e.edl as edlCatName,case when m.isedl2021= 'Y' then 'EDL' else 'Non EDL' end as EDL, m.itemname, m.strength1, ai.facilityindentqty AIFacility, ai.cmhodistqty ApprovedAICMHO, nvl(round(ai.cmhodistqty/12,0)*3,0) as FACReqFor3Month 
        ,nvl(fs.facStock,0) as facstock,nvl(wi.WHIssueYear,0) as WIssueQTYYear,wh.Ready* nvl(m.unitcount,1) as WHReady,wh.warehouseid
        ,case when round(nvl(fs.facStock,0),0)< nvl(round(ai.cmhodistqty/12,0)*3,0) then 'Need To Be Indent to CGMSC' else 'Stock Availble For 3 Month' end as Remarks
        ,ty.ITEMTYPEID,e.edlcat, mc.MCID,mc.MCATEGORY,m.itemid,nvl(m.unitcount,1) as unitcount
        from masanualindent a
        inner join anualindent ai on ai.indentid=a.indentid
        inner join masitems m on m.itemid= ai.itemid
        inner join masitemcategories c on c.categoryid= m.categoryid
        inner join masitemmaincategory mc on mc.MCID= c.MCID
        left outer join massubitemcategory msc on msc.categoryid= c.categoryid
        left outer join masedl e on e.edlcat= m.edlcat
        left outer join masitemtypes ty on ty.ITEMTYPEID= m.ITEMTYPEID
        left outer join
             (
                         select mi.ITEMCODE, sum((case when b.qastatus = '1' then (nvl(b.absrqty,0) - nvl(iq.issueqty,0)) end)) facStock
                         from tbfacilityreceiptbatches b
                         inner join tbfacilityreceiptitems i on b.facreceiptitemid=i.facreceiptitemid
                         inner join tbfacilityreceipts t on t.facreceiptid= i.facreceiptid
                         inner join vmasitems mi on mi.itemid= i.itemid
                         inner join masfacilities f  on f.facilityid= t.facilityid
                         left outer join
                         (
                           select fs.facilityid, fsi.itemid, ftbo.inwno, sum(nvl(ftbo.issueqty,0)) issueqty
                             from tbfacilityissues fs
                           inner join tbfacilityissueitems fsi on fsi.issueid=fs.issueid
                           inner join tbfacilityoutwards ftbo on ftbo.issueitemid= fsi.issueitemid
                           where fs.status = 'C'  and fs.facilityid= " + facid + @"
                           group by fsi.itemid, fs.facilityid, ftbo.inwno
                         ) iq on b.inwno = Iq.inwno and iq.itemid=i.itemid and iq.facilityid=t.facilityid
                         Where  T.Status = 'C'  And(b.Whissueblock = 0 or b.Whissueblock is null) and b.expdate>sysdate
                        and t.facilityid= " + facid + @"
                        group by mi.ITEMCODE
                        having (sum((case when b.qastatus = '1' then (nvl(b.absrqty,0) - nvl(iq.issueqty,0)) end)))>0
        ) fs on  fs.ITEMCODE=m.itemcode


        left outer join
        (
          select t.warehouseid, i.itemid,
            sum(nvl(case when b.qastatus = '1' then (nvl(b.absrqty,0) - nvl(iq.issueqty,0)) else 
            (case when mi.Qctest ='N' and b.qastatus= 2 then 0 
            else case when mi.Qctest = 'N' then (nvl(b.absrqty,0)-nvl(iq.issueqty,0)) end end ) end,0)) Ready,
            sum(nvl(case when mi.qctest= 'N' then 0 else (case when b.qastatus = 0 or b.qastatus = 3 then (nvl(b.absrqty,0)) end) end,0))  Pending
            from tbreceiptbatches b
            inner join tbreceiptitems i on b.receiptitemid=i.receiptitemid
            inner join tbreceipts t on t.receiptid= i.receiptid
            inner join masitems mi on mi.itemid= i.itemid
            inner join masitemcategories c on c.categoryid= mi.categoryid
            left outer join
            (
            select tb.warehouseid, tbi.itemid, tbo.inwno, sum(nvl(tbo.issueqty,0)) issueqty
            from tboutwards tbo, tbindentitems tbi , tbindents tb
            where tbo.indentitemid=tbi.indentitemid and tbi.indentid= tb.indentid
            and tb.status = 'C' and tb.notindpdmis is null 
            and tbo.notindpdmis is null and tbi.notindpdmis is null    
            group by tbi.itemid, tb.warehouseid, tbo.inwno
            ) iq on b.inwno = Iq.inwno and iq.itemid=i.itemid and iq.warehouseid=t.warehouseid
            Where  T.Status = 'C'  And(b.ExpDate >= SysDate or nvl(b.ExpDate, SysDate) >= SysDate) and(b.Whissueblock = 0 or b.Whissueblock is null)
            and t.notindpdmis is null and b.notindpdmis is null  and i.notindpdmis is null
            and t.warehouseid= " + WHIID + @" 
            group by t.warehouseid, i.itemid
            having  (sum(nvl(case when b.qastatus = '1' then (nvl(b.absrqty,0) - nvl(iq.issueqty,0)) else 
            (case when mi.Qctest ='N' and b.qastatus= 2 then 0 
            else case when mi.Qctest = 'N' then (nvl(b.absrqty,0)-nvl(iq.issueqty,0)) end end ) end,0))>0)
        ) wh on wh.itemid=m.itemid

        left outer join
        (
        select tbi.itemid, sum(tbo.ISSUEQTY* nvl(m.unitcount,1)) as WHIssueYear from  tbIndents tb
        inner join   tbIndentItems tbi on tbi.indentid=tb.indentid
        inner join masitems m on m.itemid= tbi.itemid
        inner join masitemcategories c on c.categoryid= m.categoryid
        inner join tboutwards tbo on tbo.INDENTITEMID= tbi.INDENTITEMID
        inner Join masAccYearSettings yr on tb.IndentDate between yr.StartDate and yr.EndDate
        where tb.facilityid= " + facid + @" and tb.status= 'C' and tb.issuetype= 'NO'
        and yr.AccYrSetID = " + yearid + @"
        group by tbi.itemid, m.itemname, m.itemcode, m.unitcount
        ) wi on wi.itemid=m.itemid
        where 1=1 " + whmcatid + @" 
        and ai.cmhodistqty>0 and a.accyrsetid= " + yearid + @" and a.status= 'C' and a.facilityid= " + facid + @"
        and wh.Ready>0 " + edlcaluse + @"
        ) where 1=1 " + whitemcase + @"
        order  by itemname ";
            var myList = _context.IndentAlertNewDbSet
            .FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList();

            return myList;
        }

        //[HttpPost("postNOCitems")]
        //public IActionResult postNOCitems(MasCgmscNocItems objmascgmscnocitems)
        //{
        //    objmascgmscnocitems.CGMSCLREMARKS = "Indent Placed";
        //    objmascgmscnocitems.STATUS = "C";
        //    objmascgmscnocitems.BOOKEDFLAG = "B";
        //    objmascgmscnocitems.APPROVEDQTY = 0;
        //    objmascgmscnocitems.BOOKEDQTY = objmascgmscnocitems.REQUESTEDQTY;

        //    try
        //    {
        //        _context.mascgmscnocitemsDbSet.Add(objmascgmscnocitems);
        //        _context.SaveChanges();
        //        return Ok("Success");
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}


        [HttpPost("postNOCitems")]
        public IActionResult postNOCitems(MasCgmscNocItems objmascgmscnocitems, string SR)
        {
            objmascgmscnocitems.CGMSCLREMARKS = "Indent Placed";
            objmascgmscnocitems.STATUS = "C";
            objmascgmscnocitems.BOOKEDFLAG = "B";
            objmascgmscnocitems.APPROVEDQTY = 0;
            objmascgmscnocitems.BOOKEDQTY = objmascgmscnocitems.REQUESTEDQTY;

            try
            {
                if (SR == "0")
                {
                    // If SR is "0", add a new entry
                    _context.mascgmscnocitemsDbSet.Add(objmascgmscnocitems);
                }
                else
                {
                    // Convert SR to long for comparison
                    if (long.TryParse(SR, out long srValue))
                    {
                        // If SR is not "0", update the existing entry
                        var existingItem = _context.mascgmscnocitemsDbSet.FirstOrDefault(x => x.SR == srValue);
                        if (existingItem != null)
                        {
                            // Update the existing entry with new values
                            //existingItem.CGMSCLREMARKS = objmascgmscnocitems.CGMSCLREMARKS;
                            //existingItem.STATUS = objmascgmscnocitems.STATUS;
                            //existingItem.BOOKEDFLAG = objmascgmscnocitems.BOOKEDFLAG;
                            //existingItem.APPROVEDQTY = objmascgmscnocitems.APPROVEDQTY;
                            existingItem.BOOKEDQTY = objmascgmscnocitems.REQUESTEDQTY;
                            existingItem.REQUESTEDQTY = objmascgmscnocitems.REQUESTEDQTY; // Update as necessary

                            _context.Entry(existingItem).State = EntityState.Modified;
                        }
                        else
                        {
                            return NotFound("Item with the specified SR not found.");
                        }
                    }
                    else
                    {
                        return BadRequest("Invalid SR value.");
                    }
                }

                // Save changes to the database
                _context.SaveChanges();
                return Ok("Success");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("postOtherFacIndentitems")]
        public IActionResult postOtherFacIndentitems(MasFacDemandItems objMasFacDemandItems)
        {

            objMasFacDemandItems.STATUS = "N";
            objMasFacDemandItems.APPROVEDQTY = objMasFacDemandItems.REQUESTEDQTY;

            try
            {
                if (objMasFacDemandItems.INDENTITEMID == 0)
                {
                    _context.MasFacDemandItemsDbSet.Add(objMasFacDemandItems);
                }
                else
                {
                    // Convert SR to long for comparison
                    // If SR is not "0", update the existing entry

                    var existingItem = _context.MasFacDemandItemsDbSet.FirstOrDefault(x => x.INDENTITEMID == objMasFacDemandItems.INDENTITEMID);
                    if (existingItem != null)
                   //if(true)
                    {
                        existingItem.INDENTID = objMasFacDemandItems.INDENTID;
                        existingItem.ITEMID = objMasFacDemandItems.ITEMID;
                        existingItem.REQUESTEDQTY = objMasFacDemandItems.REQUESTEDQTY;
                        existingItem.FACSTOCK = objMasFacDemandItems.FACSTOCK;
                        existingItem.APPROVEDQTY = objMasFacDemandItems.APPROVEDQTY;
                        existingItem.STATUS = objMasFacDemandItems.STATUS;
                        existingItem.STOCKINHAND = objMasFacDemandItems.STOCKINHAND;

                    }
                    else
                    {
                        return NotFound("Item with the specified SR not found.");
                    }

                }

                // Save changes to the database
                _context.SaveChanges();
                return Ok("Success");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }




        [HttpGet("getSavedFacIndentItems")]
        public async Task<ActionResult<IEnumerable<SavedFacIndentItemsDTO>>> getSavedFacIndentItems(Int64 nocid)
        {
            //string qry = @"select SR,b.ItemID,mi.itemname,NVL(STOCKINHAND ,0) STOCKINHAND ,requestedqty*nvl(mi.unitcount,1) whindentQTY,
            //    a.status
            //    from MASCGMSCNOC a,MASCGMSCNOCITEMS b 
            //    inner join masitems mi on mi.ItemID=b.ItemID 
            //    where a.nocid=b.nocid and a.NOCID=" + nocid + @"  order by b.sr desc ";

            string qry = @"  select SR, b.ItemID,mi.itemname,NVL(STOCKINHAND, 0) STOCKINHAND ,requestedqty* nvl(mi.unitcount,1) whindentQTY,
                a.status,mi.itemcode,ty.itemtypename,mi.strength1,mc.MCATEGORY,mc.mcid
                from MASCGMSCNOC a, MASCGMSCNOCITEMS b
                inner join masitems mi on mi.ItemID = b.ItemID
                inner join masitemcategories c on c.categoryid = mi.categoryid
inner join masitemmaincategory mc on mc.MCID = c.MCID
                left outer join masitemtypes ty on ty.itemtypeid = mi.itemtypeid
                where a.nocid = b.nocid and a.NOCID = " + nocid + @"  order by mc.mcid,SR ";

            var myList = _context.SavedFacIndentItemsDbSet
            .FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList();
            return myList;

        }

        [HttpDelete("deleteCgmscNOCitemsALL")]
        public IActionResult deleteCgmscNOCitemsALL(Int64 nocid)
        {

            string qry = "  delete from MASCGMSCNOCITEMS where NOCID =  " + nocid;
            _context.Database.ExecuteSqlRaw(qry);

            string qry1 = "   delete from MASCGMSCNOC where NOCID = " + nocid;
            _context.Database.ExecuteSqlRaw(qry1);

            return Ok("Successfully Deleted Indent & Indented items");

        }

        [HttpPut("completemascgmscnoc")]
        public IActionResult completemascgmscnoc(Int64 nocid)
        {

            string qry = "  update MASCGMSCNOC set NOCDATE=curdate(), STATUS = 'C',ENTRY_DATE=curdate() where nocid = " + nocid;
            _context.Database.ExecuteSqlRaw(qry);


            return Ok("Successfully Update MASCGMSCNOC status C");

        }

        [HttpPut("completeOtherFacIndent")]
        public IActionResult completeOtherFacIndent(Int64 indentId)
        {

            string qry = "  update MASFACTRANSFERS set STATUS = 'C',ENTRYDATE=curdate() where INDENTID = " + indentId;
            _context.Database.ExecuteSqlRaw(qry);


            return Ok("Successfully Update MASFACTRANSFERS status C");

        }


        [HttpGet("getSHCitems")] //mariadb
        public async Task<ActionResult<IEnumerable<SHCitemlistDTO>>> getSHCitems(Int64 mcid, Int64 nocid)
        {
            string whMcid = "";
            string whNocid = "";
            string caseCond = "0";
            if (mcid != 0)
            {
                whMcid = " and mc.mcid = " + mcid + " ";
            }

            if (nocid != 0)
            {
                whNocid = " left outer join mascgmscnocitems ni on ni.itemid = m.itemid and ni.nocid = " + nocid + @" ";
                caseCond = " nvl(ni.BOOKEDQTY,0) *  nvl(m.unitcount,1)";
            }

            string qry = @" select nvl(ni.sr,0) sr, m.itemid, m.itemcode,ty.itemtypename,m.itemname,m.strength1, m.multiple, m.unitcount," + caseCond + @" as indentQty from masitems m 
inner join masitemcategories c on c.categoryid = m.categoryid
inner join masitemmaincategory mc on mc.MCID = c.MCID 
left outer join masitemtypes ty  on ty.itemtypeid = m.itemtypeid
" + whNocid + @"
where m.shc = 'Y' " + whMcid + @" and m.ISFREEZ_ITPR is NULL
order by  nvl(ni.BOOKEDQTY,0) desc,ty.itemtypename ";
            var myList = _context.SHCitemlistDbSet
            .FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList();
            return myList;
        }

        [HttpGet("getOtherFacIndentItems")]
        public async Task<ActionResult<IEnumerable<SHCitemlistDTO>>> getOtherFacIndentItems(Int64 mcid, Int64 indentid)
        {
            string whMcid = "";
            string whindentid = "";
            string caseCond = "0";
            if (mcid != 0)
            {
                whMcid = " and mc.mcid = " + mcid + " ";
            }

            if (indentid != 0)
            {
                whindentid = " left outer join MASFACDEMANDITEMS ni on ni.itemid = m.itemid and ni.INDENTID = " + indentid + @" ";
                caseCond = " nvl(ni.requestedqty,0) ";
            }

            string qry = @" select nvl(ni.INDENTITEMID,0) sr, m.itemid, m.itemcode,ty.itemtypename,m.itemname,m.strength1, m.multiple, m.unitcount," + caseCond + @" as indentQty from masitems m 
inner join masitemcategories c on c.categoryid = m.categoryid
inner join masitemmaincategory mc on mc.MCID = c.MCID 
left outer join masitemtypes ty  on ty.itemtypeid = m.itemtypeid
" + whindentid + @"
where m.shc = 'Y' " + whMcid + @" and m.ISFREEZ_ITPR is NULL
order by nvl(ni.requestedqty,0) desc, ty.itemtypename ";
            var myList = _context.SHCitemlistDbSet
            .FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList();
            return myList;
        }


        [HttpGet("getFacilityInfo")]
        public async Task<ActionResult<IEnumerable<FacilityInfoAamDTO>>> getFacilityInfo(Int64 facId, Int64 userid, Int64 whId, Int64 distId)
        {
            string whFacId = "";
            string whUserId = "";
            string whWarehouseId = "";
            string whDistrictId = "";

            if (facId != 0)
            {
                whFacId = " and f.facilityid =  " + facId + " ";
            }

            if (userid != 0)
            {
                whUserId = "  and u.userid = " + userid + " ";

            }

            if (whId != 0)
            {
                whWarehouseId = "  and fw.warehouseid = " + whId + "  ";

            }


            if (distId != 0)
            {
                whDistrictId = "  and d.districtid = " + distId + " ";

            }

            string qry = @" SELECT  
    d.districtname,
    l.locationname, 
    f.FACILITYNAME,
    IFNULL(p.FACILITYNAME, 'Not Linked') AS parentfac,
    f.LONGITUDE,
    f.LATITUDE,
    f.PHC_ID,
    f.PHONE1, 
    CASE 
        WHEN f.is_aam = 'Y' THEN 'AAM' 
        ELSE '-' 
    END AS mfacility,
    IFNULL(f.CONTACTPERSONNAME, 'Not Updated') AS CONTACTPERSONNAME,
    f.FACILITYCODE,
    f.FACILITYID,
    h.FOOTER1,
    h.FOOTER2,
    h.FOOTER3,
    h.EMAIL,
    d.districtid,
    ft.FACILITYTYPECODE,
    ft.FACILITYTYPEDESC,
    ft.ELDCAT,
    fw.warehouseid,
    w.WAREHOUSENAME,
    w.email AS whemail,
    w.phone1 AS whcontact,
    u.userid,
    u.emailid,
    f.indentduration
FROM 
    masfacilities f
INNER JOIN 
    usrusers u ON u.FACILITYID = f.facilityid
LEFT OUTER JOIN 
    MASFACHEADERFOOTER h ON h.userid = u.userid
INNER JOIN 
    masfacilitywh fw ON fw.facilityid = f.facilityid
INNER JOIN 
    maswarehouses w ON w.WAREHOUSEID = fw.WAREHOUSEID
INNER JOIN 
    masfacilitytypes ft ON ft.FACILITYTYPEID = f.FACILITYTYPEID
INNER JOIN 
    masdistricts d ON d.districtid = f.districtid
LEFT OUTER JOIN 
    maslocations l ON l.LOCATIONID = f.LOCATIONID
LEFT OUTER JOIN 
    masfacilities p ON p.facilityid = f.PHC_ID
WHERE 
    1 = 1 
    " + whFacId + @"
    " + whUserId + @"
    " + whWarehouseId + @"  
    " + whDistrictId + @";
  ";
            var myList = _context.FacilityInfoAamDbSet
            .FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList();
            return myList;
        }


        [HttpGet("sendSMS")]
        public IActionResult sendSMS(String mob, String otp)
        {
            FacOperations ObjFacOp = new FacOperations(_context);
            ObjFacOp.getLoginSMS(mob, otp);
            return Ok("Successfully Send");

        }

        [HttpGet("getIndentTimline")]  // MariaDB
        public async Task<ActionResult<IEnumerable<getIndentTimlineDTO>>> getIndentTimline(Int64 facId, Int64 distId)
        {
            String whFacId = "";
            String whFacId2 = "";
            String whDistId = "";

            if (facId != 0)
            {
                whFacId = "  and n.facilityid = " + facId + @"  ";
                whFacId2 = "  and f.facilityid = " + facId + @"  ";
            }

            if (distId != 0)
            {
                whDistId = "  and f.districtid = " + distId + "  ";
            }

            string qry = @"
    select 
        l.locationname,
        f.facilityname,
        f.contactpersonname,
        f.phone1,
        f.phc_id,
        f2.facilityname phcname,
        CAST(f.indentduration AS DATETIME) AS FirstIndentOpening,
        DATE_ADD(CAST(f.indentduration AS DATETIME), interval 7 DAY) AS FirstIndentClosing,
        i.lastindentdate,
        f.facilityid,
        i.indentOpeningDate,
        i.indentOpeningDate + INTERVAL 15 DAY AS indentClosingDate,
        CASE 
            WHEN CURDATE() <= DATE_ADD(CAST(f.indentduration AS DATETIME), INTERVAL 7 DAY) THEN 1
            ELSE (
                CASE 
                    WHEN CURDATE() BETWEEN IFNULL(i.indentOpeningDate, CAST(f.indentduration AS DATETIME)) 
                                        AND DATE_ADD(IFNULL(i.indentOpeningDate, CAST(f.indentduration AS DATETIME)), INTERVAL 7 DAY) 
                    THEN 1 ELSE 0 
                END
            ) 
        END AS RegularOpeningCase
    from masfacilities f 
    inner join masfacilities f2 on f2.facilityid = f.phc_id
    left outer join maslocations l on l.locationid = f.locationid
    left outer join (
        select 
            max(NOCDATE) as lastindentdate, 
            n.facilityid, 
            DATE_ADD(MAX(NOCDATE), INTERVAL 3 MONTH) AS indentOpeningDate 
        from mascgmscnoc n
        where n.status = 'C' " + whFacId + @"
        group by n.facilityid
    ) i on i.facilityid = f.facilityid 
    where 1=1 " + whFacId2 + @"  
    " + whDistId + @"
    and f.facilitytypeid = 377 and f.is_aam = 'Y'
    order by l.locationid
    ";

            var myList = await _context.getIndentTimlineDbSet
                .FromSqlInterpolated(FormattableStringFactory.Create(qry))
                .ToListAsync();
            return myList;
        }



        //        [HttpGet("getIndentTimline")]  //mariadb
        //        public async Task<ActionResult<IEnumerable<getIndentTimlineDTO>>> getIndentTimline(Int64 facId, Int64 distId)
        //        {
        //            String whFacId = "";
        //            String whFacId2 = "";
        //            String whDistId = "";

        //            if (facId != 0)
        //            {
        //                whFacId = "  and n.facilityid = " + facId + @"  ";
        //                whFacId2 = "  and f.facilityid = " + facId + @"  ";
        //            }

        //            if (distId != 0)
        //            {
        //                whDistId = "  and f.districtid = " + distId + "  ";
        //            }

        //            string qry = @"  select l.locationname,f.facilityname,f.contactpersonname,f.phone1,f.phc_id,f2.facilityname phcname,f.indentduration FirstIndentOpening,DATE_ADD( f.indentduration, interval 7 DAY) as FirstIndentClosing, i.lastindentdate,f.facilityid,i.indentOpeningDate,  i.indentOpeningDate+15 indentClosingDate 
        //,case when  CURDATE() <= DATE_ADD(f.indentduration, INTERVAL 7 DAY) then 1 else (case when  curdate() between ifnull(i.indentOpeningDate,f.indentduration) AND DATE_ADD( IFNULL(i.indentOpeningDate,f.indentduration), INTERVAL 7 DAY)  then 1 else 0 end ) end  as RegularOpeningCase 
        //from masfacilities f 
        //inner join masfacilities f2 on f2.facilityid = f.phc_id
        //  left outer join maslocations l on l.locationid = f.locationid
        //left outer join
        //(
        //select max(NOCDATE) as lastindentdate, n.facilityid, DATE_ADD(MAX(NOCDATE), INTERVAL 3 MONTH) AS indentOpeningDate from mascgmscnoc n

        //where n.status = 'C' " + whFacId + @"
        //group by n.facilityid
        //) i on i.facilityid = f.facilityid 
        //where 1=1 " + whFacId2 + @"  
        //" + whDistId + @"
        //and f.facilitytypeid = 377 and f.is_aam = 'Y'
        //order by l.locationid
        // ";
        //            var myList = _context.getIndentTimlineDbSet
        //            .FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList();
        //            return myList;
        //        }


        [HttpGet("getBlockFac")] //mariadb
        public async Task<ActionResult<IEnumerable<getBlockFACsDTO>>> getBlockFac(Int64 facid)
        {

            FacOperations ObjFacOp = new FacOperations(_context);
            Int64 LID = ObjFacOp.getLocationID(facid.ToString());

            //            string qry = @"  select facname, FACILITYID, forder
            //from
            //(
            //select case when f.FACILITYID= (select PHC_ID from masfacilities where FACILITYID= " + facid + @") then 'Parent:'|| f.FACILITYNAME else f.FACILITYNAME end facname,
            //f.FACILITYID,ft.orderdp,case when f.FACILITYID=(select PHC_ID from masfacilities where FACILITYID= " + facid + @") then 0
            //else ft.orderdp end forder
            //from masfacilities f
            //inner join masfacilitytypes ft on ft.FACILITYTYPEID = f.FACILITYTYPEID
            //inner join masdistricts d on d.districtid = f.districtid
            //inner join  maslocations l on l.LOCATIONID= f.LOCATIONID
            //left outer join masfacilities p on p.facilityid= f.PHC_ID
            //where 1=1 and ft.hodid= 2 and f.isactive= 1 and l.locationid= " + LID + @" and f.facilityid not in (" + facid + @")
            //)
            //order by forder ";

            string qry = @"  SELECT 
    facname, 
    FACILITYID, 
FACILITYTYPEID,
    forder
FROM
(
    SELECT 
        CASE 
            WHEN f.FACILITYID = (SELECT PHC_ID FROM masfacilities WHERE FACILITYID = " + facid + @") 
            THEN CONCAT('Parent:', f.FACILITYNAME) 
            ELSE f.FACILITYNAME 
        END AS facname,
        f.FACILITYID,
        f.FACILITYTYPEID,
        ft.orderdp,
        CASE 
            WHEN f.FACILITYID = (SELECT PHC_ID FROM masfacilities WHERE FACILITYID = " + facid + @") 
            THEN 0
            ELSE ft.orderdp 
        END AS forder
    FROM 
        masfacilities f
    INNER JOIN 
        masfacilitytypes ft ON ft.FACILITYTYPEID = f.FACILITYTYPEID
    INNER JOIN 
        masdistricts d ON d.districtid = f.districtid
    INNER JOIN  
        maslocations l ON l.LOCATIONID = f.LOCATIONID
    LEFT OUTER JOIN 
        masfacilities p ON p.facilityid = f.PHC_ID
    WHERE 
        1 = 1 
        AND ft.hodid = 2 
        AND f.isactive = 1 
        AND l.locationid = " + LID + @"
        AND f.facilityid NOT IN (" + facid + @")
) AS subquery
ORDER BY 
    forder;
 ";

            var myList = _context.getBlockFACsDbSet
            .FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList();
            return myList;
        }


        [HttpGet("OtherFacilityIndent")]

        public async Task<ActionResult<IEnumerable<OtherFacilityIndentDTO>>> OtherFacilityIndent(string facid)
        {

            string qry = @"  select m.INDENTNO,m.INDENTDATE,f.facilityname,f.facilityid, m.fromfacilityid,m.INDENTID,m.status,f.FACILITYTYPEID from MASFACTRANSFERS m 
inner join masfacilities f on f.facilityid = m.FROMFACILITYID
where m.facilityid = " + facid + @" ";

            var myList = _context.OtherFacilityIndentDbSet
            .FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList();
            return myList;
        }

        [HttpGet("OtherFacIndentDetails")]

        public async Task<ActionResult<IEnumerable<OtherFacIndentDetailsDTO>>> OtherFacIndentDetails(string indentId)
        {

            //            string qry = @" select nvl(ni.INDENTITEMID,0) sr, m.itemid, m.itemcode,ty.itemtypename,m.itemname,m.strength1, m.multiple, m.unitcount,requestedqty as indentQty,na.INDENTNO, na.indentdate,tof.facilityname as toFacility from masitems m 
            //inner join masitemcategories c on c.categoryid = m.categoryid
            //inner join masitemmaincategory mc on mc.MCID = c.MCID
            //left outer join masitemtypes ty  on ty.itemtypeid = m.itemtypeid
            //inner join MASFACDEMANDITEMS ni on ni.itemid = m.itemid and ni.INDENTID = " + indentId + @"
            //inner join MASFACTRANSFERS na on na.indentid = ni.indentid
            //inner join masfacilities tof on tof.facilityid = na.fromfacilityid
            //where m.shc = 'Y'  and m.ISFREEZ_ITPR is NULL
            //order by ty.itemtypename";

            string qry = @"SELECT IFNULL(ni.INDENTITEMID, 0) AS sr, 
                      m.itemid, 
                      m.itemcode, 
                      ty.itemtypename, 
                      m.itemname, 
                      m.strength1, 
                      m.multiple, 
                      m.unitcount, 
                      requestedqty AS indentQty, 
                      na.INDENTNO, 
                      na.indentdate, 
                      tof.facilityname AS toFacility 
               FROM masitems m
               INNER JOIN masitemcategories c ON c.categoryid = m.categoryid
               INNER JOIN masitemmaincategory mc ON mc.MCID = c.MCID
               LEFT OUTER JOIN masitemtypes ty ON ty.itemtypeid = m.itemtypeid
               INNER JOIN MASFACDEMANDITEMS ni ON ni.itemid = m.itemid AND ni.INDENTID = " + indentId + @"
               INNER JOIN MASFACTRANSFERS na ON na.indentid = ni.indentid
               INNER JOIN masfacilities tof ON tof.facilityid = na.fromfacilityid
               WHERE m.shc = 'Y' AND m.ISFREEZ_ITPR IS NULL
               ORDER BY ty.itemtypename";

            var myList = _context.OtherFacIndentDetailsDbSet
            .FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList();
            return myList;
        }


        [HttpDelete("deleteOtherFaceIdent")]
        public IActionResult deleteOtherFaceIdent(Int64 indentId)
        {

            string qry = " delete from MASFACDEMANDITEMS where indentid =  " + indentId;
            _context.Database.ExecuteSqlRaw(qry);

            string qry1 = "   delete from MASFACTRANSFERS where indentid =  " + indentId;
            _context.Database.ExecuteSqlRaw(qry1);

            return Ok("Successfully Deleted other facility Indent & Indented items");

        }
    }

}
