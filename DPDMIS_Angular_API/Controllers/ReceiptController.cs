using CgmscHO_API.Utility;
using DPDMIS_Angular_API.Data;
using DPDMIS_Angular_API.DTO.IndentDTO;
using DPDMIS_Angular_API.DTO.IssueDTO;
using DPDMIS_Angular_API.DTO.ReceiptDTO;
using DPDMIS_Angular_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Runtime.CompilerServices;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DPDMIS_Angular_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReceiptController : ControllerBase
    {
        private readonly MariaDbContext _context;
        public ReceiptController(MariaDbContext context)
        {
            _context = context;
        }


        [HttpGet("getReceiptMasterFromWH")]
        public async Task<ActionResult<IEnumerable<ReceiptMasterWHDTO>>> getReceiptMaster(string faclityId, string status)
        {
            string whStatus = "";
            if (status == "IN")
            {
                whStatus = " AND IFNULL(fr.STATUS, 'I') = 'I' ";
            }
            else if (status == "C")
            {
                whStatus = " and IFNULL(fr.STATUS,'I')= 'C' ";
            }

            string qry = @"
SELECT faci.nocid, 
       faci.nocdate AS ReqDate, 
       faci.NOCNUMBER AS ReqNo, 
       nositemsReq, 
       tb.INDENTNO AS WHIssueNo, 
       tb.INDENTDATE AS WHIssueDT, 
       nositemsIssued, 
       tb.indentid,
       fr.FACRECEIPTNO, 
       fr.FACRECEIPTDATE, 
       CASE 
           WHEN fr.STATUS = 'I' THEN 'IN' 
           ELSE IFNULL(fr.STATUS, 'IN') 
       END AS RStatus,
       fr.FACRECEIPTID, 
       faci.FacilityID, 
       tb.warehouseid
FROM mascgmscnoc faci
INNER JOIN (
    SELECT COUNT(DISTINCT ri.itemid) AS nositemsReq, 
           ri.nocid 
    FROM mascgmscnocitems ri
    WHERE 1 = 1 
      AND ri.BOOKEDQTY > 0
    GROUP BY ri.nocid
) ri ON ri.nocid = faci.nocid
INNER JOIN tbIndents tb ON tb.NOCID = faci.nocid
LEFT OUTER JOIN (
    SELECT COUNT(DISTINCT tbi.itemid) AS nositemsIssued, 
           tbi.indentid 
    FROM tbIndentItems tbi
    GROUP BY tbi.indentid
) tbi ON tbi.indentid = tb.indentid
LEFT OUTER JOIN tbFacilityReceipts fr ON fr.FacilityID = faci.FacilityID 
                                      AND fr.IndentId = tb.IndentId
WHERE 1 = 1 
  AND faci.ACCYRSETID >= 544 
  AND faci.facilityid = " + faclityId + @"
" + whStatus + @"
ORDER BY tb.INDENTDATE DESC;
";



            //            string qry = @"  select faci.nocid, faci.nocdate as ReqDate, faci.NOCNUMBER as ReqNo, nositemsReq, tb.INDENTNO WHIssueNo, tb.INDENTDATE as WHIssueDT, nositemsIssued, tb.indentid
            //, fr.FACRECEIPTNO, fr.FACRECEIPTDATE,case when fr.STATUS='I' then 'IN' else  nvl(fr.STATUS,'IN') end as RStatus
            //, fr.FACRECEIPTID,faci.FacilityID,tb.warehouseid
            //from mascgmscnoc faci
            //inner join
            //(
            //select count(distinct ri.itemid) nositemsReq, ri.nocid from mascgmscnocitems ri
            //where 1=1 and ri.BOOKEDQTY>0
            //group by ri.nocid
            //) ri on ri.nocid=faci.nocid
            //inner  join tbIndents tb on tb.NOCID=faci.nocid
            //left outer join
            //(
            //select count(distinct tbi.itemid) nositemsIssued, tbi.indentid from tbIndentItems tbi
            //group by tbi.indentid
            //) tbi on tbi.indentid=tb.indentid
            //left outer join tbFacilityReceipts fr on fr.FacilityID=faci.FacilityID and fr.IndentId= tb.IndentId

            //where 1=1 
            //and faci.ACCYRSETID>=544  and  faci.facilityid =" + faclityId + @"  

            //" + whStatus + @"
            //order by tb.INDENTDATE desc";
            //    --  and  faci.facilityid in (22480,23417)
            var context = new ReceiptMasterWHDTO();

            var myList = _context.ReceiptMasterDbSet
            .FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList();

            return myList;



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


        [HttpPost("postReceiptMaster")]
        public async Task<ActionResult<IEnumerable<ReceiptMasterDTO>>> postReceiptMaster(string facid, tbFacilityReceiptsModel objeReceipt)
        {

            try
            {
                FacOperations ob = new FacOperations(_context);
                string validRceceiptDate = ob.FormatDate(objeReceipt.FACRECEIPTDATE);
                objeReceipt.FACRECEIPTDATE = validRceceiptDate;
                objeReceipt.FACRECEIPTTYPE = "NO";
                objeReceipt.ISUSEAPP = "Y";
                //objeReceipt.STATUS = "I";

                //string DataIDate = objeIssue.ISSUEDATE;
                string receiptNo = getReceiptIssueNo(facid);
                objeReceipt.FACRECEIPTNO = receiptNo;

                _context.tbFacilityReceiptsDbSet.Add(objeReceipt);
                _context.SaveChanges();
                Int64 ffcid = Convert.ToInt64(facid);
                var myObj = getReceiptDetails(Convert.ToInt64(objeReceipt.INDENTID), ffcid);
                return Ok(myObj);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
        }

        [HttpGet("getReceiptIssueNo")]
        public string getReceiptIssueNo(string facId)
        {

            FacOperations op = new FacOperations(_context);
            string getNo = op.FacAutoGenerateNumbers(facId, true, "NO");
            return getNo;

        }

        [HttpPost("postReceiptMasterSP")]
        public async Task<ActionResult<IEnumerable<ReceiptMasterDTO>>> postReceiptMasterSP(Int64 tofacid,Int64 indentid,Int64 issueid,string facid, tbFacilityReceiptsModel objeReceipt)
        {

            try
            {
                FacOperations ob = new FacOperations(_context);
                string validRceceiptDate = ob.FormatDate(objeReceipt.FACRECEIPTDATE);
                objeReceipt.FACRECEIPTDATE = validRceceiptDate;
                objeReceipt.FACRECEIPTTYPE = "SP";
                objeReceipt.ISUSEAPP = "Y";
                objeReceipt.INDENTID = indentid;
                objeReceipt.TOFACILITYID = tofacid;

                //objeReceipt.STATUS = "I";

                //string DataIDate = objeIssue.ISSUEDATE;
                string receiptNo = getReceiptIssueNo(facid);
                objeReceipt.FACRECEIPTNO = receiptNo;
                objeReceipt.ISSUEID = issueid;
                _context.tbFacilityReceiptsDbSet.Add(objeReceipt);
                _context.SaveChanges();
                Int64 ffcid = Convert.ToInt64(facid);
               // var myObj = getReceiptDetails(Convert.ToInt64(objeReceipt.INDENTID), ffcid);
                var myObj = getOtherFacIssueDetails(ffcid.ToString(),"I", issueid, receiptNo);
                return Ok(myObj);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
        }

        [HttpGet("getReceiptIssueNoSP")]
        public string getReceiptIssueNoSP(string facId)
        {

            FacOperations op = new FacOperations(_context);
            string getNo = op.FacAutoGenerateNumbers(facId, true, "SP");
            return getNo;

        }


        [HttpGet("getOtherFacIssueDetails")]
        public async Task<ActionResult<IEnumerable<getOtherFacIssueDetailsDTO>>> getOtherFacIssueDetails(string faclityId,string status,Int64 issueid,string  recno)
        {
            string strStatus = "";
            string strIssueid = "";
            string strRecno = "";
            if (status == "I")
            {
                strStatus = " and nvl(tbr.status,'I') = 'I' ";
            }

             if (status == "C")
            {
                strStatus = " and nvl(tbr.status,'I') = 'C' ";
            }
            if (issueid != 0)
            {
                strIssueid = " and tbf.issueid= " + issueid;
            }
            if (recno != "0")
            {
                strRecno = " and tbr.FACRECEIPTNO= '" + recno+"'";
            }
            

            string qry = @" select f.facilityname,ind.INDENTNO,ind.INDENTDATE ,
ISSUENO,ISSUEDDATE,nos,tbf.FACINDENTID,tbf.issueid,tbr.FACRECEIPTID,tbr.FACRECEIPTNO,tbr.FACRECEIPTDATE,nvl(tbr.status,'I') as status,tbf.facilityid from tbfacilityissues tbf 
inner join masfacilities f on f.facilityid=tbf.facilityid
inner join MASFACTRANSFERS ind on ind.indentid=tbf.FACINDENTID
left outer join tbfacilityreceipts tbr on tbr.FACRECEIPTTYPE='SP' and tbr.issueid=tbf.issueid
inner join 
(
select count(distinct tbi.itemid) nos,issueid from  tbfacilityissueitems tbi
group by issueid
)tbi on tbi.issueid=tbf.issueid 
where  1=1 " + strStatus + @"  "+ strIssueid + @"  "+ strRecno + @"  and tbf.issuetype='SP' and tbf.TOFACILITYID=" + faclityId + @"
order by tbf.ISSUEID desc ";

            var context = new ReceiptItemsDDL();

            var myList = _context.getOtherFacIssueDetailsDbSet
            .FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList();

            return myList;


        }





        [HttpGet("getReceiptDetails")] 
        public async Task<ActionResult<IEnumerable<ReceiptMasterDTO>>> getReceiptDetails(Int64 indentId, Int64 facid)
        {


            string qry = @"select faci.nocid,faci.NOCNUMBER as ReqNo ,faci.nocdate as ReqDate, fr.IndentId,tb.INDENTNO WHIssueNo, tb.INDENTDATE as WHIssueDT, FacReceiptNo,FacReceiptDate,fr.FACRECEIPTID,fr.status from tbFacilityReceipts fr
inner join tbIndents tb on tb.IndentId=fr.IndentId
inner join mascgmscnoc  faci on faci.nocid=tb.NOCID
where fr.facilityid="+ facid + " and tb.IndentID="+ indentId + "";
             

            //_context.Database.ExecuteSqlRaw(query,
            // new MySqlParameter("Password", salthash1),
            // new MySqlParameter("LastPwdChangeDate", dt1),
            // new MySqlParameter("UserID", userId)



         //MySqlParameter[] parameters = new MySqlParameter[]
         //   {
         //       new MySqlParameter(":facid",  facid),
         //       new MySqlParameter(":indentid", indentId)
         //   };

            //var myList = _context.ReceiptMasterDTODbSet
            //    .FromSqlRaw(qry)
            //    .ToList();

            var myList = _context.ReceiptMasterDTODbSet
        .FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList();

            return myList;


        
        }


        [HttpGet("getReceiptItemsDDL")]
        public async Task<ActionResult<IEnumerable<ReceiptItemsDDL>>> getReceiptItemsDDL(string faclityId, string FACRECEIPTID, string IndentID)
        {
            string qry = @" select itemname||'-'||itemcode||'-Batch-'||batchno||':'||to_char(IssueWH) as name,InwNo 
from 
(
Select distinct m.itemid,m.itemcode,m.itemname,rb.batchno,rb.expdate,tbo.IssueQty*nvl(m.unitcount,1) as IssueWH
,tfr.IndentID,tfr.FACRECEIPTID,tfi.ABSRQTY,rfb.ABSRQTY as BatrchReceiptQTY
,nvl(rc.locationno,'-') locationno,rfb.stocklocation,tfi.facreceiptitemid,rb.InwNo,tfr.status Rstatus,tfi.status RIstatus
             from tbIndentItems tbi
             left outer Join tbOutwards tbo on (tbo.IndentItemID = tbi.IndentItemID) 
             left outer Join tbReceiptBatches rb on (rb.InwNo = tbo.InwNo) 
             Inner Join masItems m on (m.ItemID=tbi.ItemID)
             Inner Join tbIndents tb on (tb.Indentid=tbi.IndentId) 
            left outer join  tbFacilityReceipts tfr on (tfr.IndentID=tb.IndentID) and tfr.facilityid=" + faclityId + @"
            Left Outer Join tbFacilityReceiptItems tfi on  tfi.FacReceiptID=tfr.FacReceiptID and tfi.itemid=tbi.itemid    
            and tfi.INDENTITEMID=tbi.INDENTITEMID
            left outer join tbfacilityreceiptbatches rfb on rfb.facreceiptitemid=tfi.facreceiptitemid
            left outer join masracks rc on rc.rackid=rfb.stocklocation
             Where 1=1  and tb.FacilityID =" + faclityId + @"
             and tb.IndentID =" + IndentID + @" and tfr.FACRECEIPTID=" + FACRECEIPTID + @"  and tbi.ItemID not in (select itemid from tbFacilityReceiptItems where FACRECEIPTID=" + FACRECEIPTID + @" )
)order by itemname ";

            var context = new ReceiptItemsDDL();

            var myList = _context.ReceiptItemsDDLDbSet
            .FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList();

            return myList;


        }


        [HttpGet("getReceiptItemsDetail")]
        public async Task<ActionResult<IEnumerable<ReceiptItemsDDL>>> getReceiptItemsDetail(string faclityId, string FACRECEIPTID, string IndentID, string inwno)
        {
            string qry = @" select IssueWH  as name,InwNo 
from 
(
Select distinct m.itemid,m.itemcode,m.itemname,rb.batchno,rb.expdate,tbo.IssueQty*nvl(m.unitcount,1) as IssueWH
,tfr.IndentID,tfr.FACRECEIPTID,tfi.ABSRQTY,rfb.ABSRQTY as BatrchReceiptQTY
,nvl(rc.locationno,'-') locationno,rfb.stocklocation,tfi.facreceiptitemid,rb.InwNo,tfr.status Rstatus,tfi.status RIstatus
             from tbIndentItems tbi
             left outer Join tbOutwards tbo on (tbo.IndentItemID = tbi.IndentItemID) 
             left outer Join tbReceiptBatches rb on (rb.InwNo = tbo.InwNo) 
             Inner Join masItems m on (m.ItemID=tbi.ItemID)
             Inner Join tbIndents tb on (tb.Indentid=tbi.IndentId) 
            left outer join  tbFacilityReceipts tfr on (tfr.IndentID=tb.IndentID) and tfr.facilityid=" + faclityId + @"
            Left Outer Join tbFacilityReceiptItems tfi on  tfi.FacReceiptID=tfr.FacReceiptID and tfi.itemid=tbi.itemid    
            and tfi.INDENTITEMID=tbi.INDENTITEMID
            left outer join tbfacilityreceiptbatches rfb on rfb.facreceiptitemid=tfi.facreceiptitemid
            left outer join masracks rc on rc.rackid=rfb.stocklocation
             Where 1=1  and tb.FacilityID =" + faclityId + @"
             and rb.InwNo = " + inwno + @"
             and tb.IndentID =" + IndentID + @" and tfr.FACRECEIPTID=" + FACRECEIPTID + @"  and tbi.ItemID not in (select itemid from tbFacilityReceiptItems where FACRECEIPTID=" + FACRECEIPTID + @" )
) ";

            var context = new ReceiptItemsDDL();

            var myList = _context.ReceiptItemsDDLDbSet
            .FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList();

            return myList;


        }

        [HttpGet("getRacks")]
        public async Task<ActionResult<IEnumerable<MasRackDTO>>> getRacks(string WH_FACID)
        {
            string qry = @" select LOCATIONNO, RACKID from masracks r where r.WAREHOUSEID=" + WH_FACID + @"
and IsDeactiveated is null
order by LOCATIONNO ";

            var context = new MasRackDTO();

            var myList = _context.MasRackDbSet
            .FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList();

            return myList;


        }


        [HttpGet("getOpstockCheck")]
        public async Task<ActionResult<IEnumerable<OPStockCheckDTO>>> getOpstockCheck(string facid)
        {

            //string qry = @"select nvl(max(nvl(FACRECEIPTID,0)),0) as OpReceiptid  from tbFacilityReceipts 
            //                    tb where STATUS ='C' and facreceipttype='FC' and tb.facilityid=" + facid;


            string qry = @" select OpReceiptid, nvl(status, 0) as stauts from
                          (
                              select nvl(max(nvl(tb.FACRECEIPTID, 0)), 0) as OpReceiptid

                              from tbFacilityReceipts tb


                               where tb.STATUS = 'C' and facreceipttype = 'FC' and tb.facilityid = " + facid + @"
                               ) a
                                 left outer join
                                (
                               select case when status is null then '0' else case when status= 'I' then 'I' else 'C' end end as status  from tbFacilityReceipts tbr
                               where facreceipttype = 'FC' and tbr.facilityid =" + facid + @"
                                ) ts on 1 = 1 ";

            var context = new OPStockCheckDTO();

            var myList = _context.OPStockCheckDBset
            .FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList();

            return myList;


        }

        [HttpGet("getOtherFacilityIssue")]
        public async Task<ActionResult<IEnumerable<getOtherFacilityIssueDTO>>> getOtherFacilityIssue(Int64 tofacilityid)
        {
            string qry = @" Select distinct m.itemid,tbo.batchno,tbo.expdate,tbo.IssueQty as IssueBatchQty,tbo.InwNo
,tbi.issueitemid,tfr.FacReceiptID,tfi.FacReceiptItemID,tbo.MfgDate from tbfacilityissueitems tbi
             left outer Join tbfacilityoutwards tbo on (tbo.issueitemid = tbi.issueitemid) 
             
             Inner Join masItems m on (m.ItemID=tbi.ItemID)
             Inner Join tbfacilityissues tb on (tb.issueid=tbi.issueid)      
             left outer join  tbFacilityReceipts tfr on (tfr.IndentID=tb.facIndentID) 
            Left Outer Join tbFacilityReceiptItems tfi on  tfi.FacReceiptID=tfr.FacReceiptID and tfi.itemid=tbi.itemid       
				  and tfi.issueITEMID=tbi.issueITEMID
             Where 1=1  and tb.toFacilityID ="+ tofacilityid + @" ";


            var myList = _context.getOtherFacilityIssueDbSet
            .FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList();

            return myList;


        }



        [HttpGet("getReceiptDetailsWithType")]
        public async Task<ActionResult<IEnumerable<ReceiptDetailsDTO>>> getReceiptDetailsWithType(string facilityReceiptType, string facilityId, string facReceiptId)
        {
            string qry = @" select m.itemcode,m.itemname,m.strength1, rfb.batchno,rfb.ABSRQTY,rfb.EXPDATE,nvl(rc.locationno,'-') locationno,rfb.stocklocation 
,rfb.INWNO,tri.itemid,tr.FACRECEIPTID,tri.facreceiptitemid  from tbfacilityreceipts tr
inner join tbfacilityreceiptitems tri on tri.FACRECEIPTID=tr.FACRECEIPTID
inner join masitems m on m.itemid = tri.itemid
inner join  tbfacilityreceiptbatches rfb on rfb.facreceiptitemid=tri.facreceiptitemid
inner  join masracks rc on rc.rackid=rfb.stocklocation
where 1=1 and tr.FACRECEIPTTYPE='" + facilityReceiptType + "' and tr.facilityid=" + facilityId + " and tr.FACRECEIPTID=" + facReceiptId + " ";


            var myList = _context.ReceiptDetailsDbSet
            .FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList();

            return myList;


        }

        [HttpPut("completeReceipts")]
        public IActionResult completeReceipts(Int64 receiptId)
        {
            string strQuery1 = "Update tbFacilityReceipts set Status = 'C',FACRECEIPTDATE=curdate(),ENTRYDATE=curdate() Where FacReceiptID = " + receiptId;
            Int32 result1 = _context.Database.ExecuteSqlRaw(strQuery1);

            string strQuery2 = @"Update tbFacilityReceiptItems set Status = 'C' Where FacReceiptID =" + receiptId;
            Int32 result2 = _context.Database.ExecuteSqlRaw(strQuery2);



            Int32 totResult = result1 + result2;

            if (totResult != 0)
            {
                return Ok("Successfully Updated");
            }
            else
            {
                return BadRequest("Some Error Occuered May be all 3 queries are not excecuted successfully.");
            }

        }

        [HttpDelete("deleteReceipts")]
        public IActionResult deleteReceipts(Int64 receiptId)

        {
            string strQuery3 = "delete from tbfacilityreceiptbatches where FACReceiptitemid in (select facreceiptitemid from tbfacilityreceiptitems where facreceiptid=" + receiptId + ")";
            Int32 result3 = _context.Database.ExecuteSqlRaw(strQuery3);


            string strQuery1 = "Delete from tbFacilityReceiptItems Where FacReceiptID = " + receiptId;
            Int32 result1 = _context.Database.ExecuteSqlRaw(strQuery1);

            string strQuery2 = " Delete from tbFacilityReceipts where FacReceiptID =" + receiptId;
            Int32 result2 = _context.Database.ExecuteSqlRaw(strQuery2);

            Int32 totResult = result1 + result2 + result3;

            if (totResult != 0)
            {
                return Ok("Successfully Deleted");
            }
            else
            {
                return BadRequest("Some Error Occuered May be all 3 queries are not excecuted successfully.");
            }

        }


        [HttpDelete("deleteReceiptItems")]
        public IActionResult deleteReceiptItems(Int64 inwno, Int64 facReceiptItemId, Int64 itemid, Int64 receiptId, Int64 deletedBatchQty)
        {
            Int32 resultdel1 = 0;
            Int32 resultdel2 = 0;
            Int32 resultdel3 = 0;
            Int32 resultdel4 = 0;

            FacOperations ob = new FacOperations(_context);
            ob.isFacilityReceiptItemsExist(Convert.ToInt64(Convert.ToInt64(itemid)), receiptId, out Int64? batchQty1, out Int64? facilityReceiptItemId);
            Int64 FacReceiptitemid = 0;
            if (deletedBatchQty == batchQty1)
            {
                //Int64 total = Convert.ToInt64(batchQty1) + Convert.ToInt64(batchQty);
                //string strQuery1 = @"update tbFacilityReceiptItems set ABSRQTY= " + total + " where itemid=" + Convert.ToInt64(itemId) + "" +
                //    " and FACRECEIPTID=" + facReceiptId + " and  FACRECEIPTITEMID = " + facilityReceiptItemId + "";
                //Int32 result1 = _context.Database.ExecuteSqlRaw(strQuery1);
                //FacReceiptitemid = Convert.ToInt64(facilityReceiptItemId);
                string strQuery5 = "Delete from tbFacilityReceiptItems Where FACRECEIPTITEMID = " + facReceiptItemId;
                resultdel1 = _context.Database.ExecuteSqlRaw(strQuery5);
                string strQuery6 = " Delete from TBFACILITYRECEIPTBATCHES where FACRECEIPTITEMID =" + facReceiptItemId + @" and inwno = " + inwno;
                resultdel2 = _context.Database.ExecuteSqlRaw(strQuery5);


            }
            else
            {

                string strQuery5 = "Update  tbFacilityReceiptItems set ABSRQTY = " + (batchQty1 - deletedBatchQty) + " Where FACRECEIPTITEMID = " + facReceiptItemId;
                resultdel3 = _context.Database.ExecuteSqlRaw(strQuery5);
                string strQuery6 = " Delete from TBFACILITYRECEIPTBATCHES where FACRECEIPTITEMID =" + facReceiptItemId + @" and inwno = " + inwno;
                resultdel4 = _context.Database.ExecuteSqlRaw(strQuery5);
            }




            Int32 totResult = resultdel1 + resultdel2 + resultdel3 + resultdel4;

            if (totResult != 0)
            {
                return Ok("Successfully Deleted");
            }
            else
            {
                return BadRequest("Some Error Occuered May be all 3 queries are not excecuted successfully.");
            }

        }

        [HttpGet("getReceiptDetailsBatch")]
        public async Task<ActionResult<IEnumerable<ReceipItemBatchesDTO>>> getReceiptDetailsBatch(Int64 nocid, Int64 facid, Int64 indentid)
        {
            string qry = @"  SELECT 
    IFNULL(ni.sr, 0) AS sr, 
    m.itemid, 
    m.itemcode, 
    ty.itemtypename, 
    m.itemname, 
    m.strength1, 
    m.multiple, 
    m.unitcount, 
    IFNULL(ni.BOOKEDQTY, 0) * IFNULL(m.unitcount, 1) AS indentQty,
    IFNULL(tbo.ISSUEQTY, 0) * IFNULL(m.unitcount, 1) AS issueWH,
    IFNULL(tbo.batchno, '-') AS batchno,
    CASE 
        WHEN tbo.mfgdate IS NULL THEN '-' 
        ELSE tbo.mfgdate 
    END AS mfgdate,
    CASE 
        WHEN tbo.expdate IS NULL THEN '-' 
        ELSE tbo.expdate 
    END AS expdate,
    IFNULL(tbo.inwno, 0) AS inwno,
    IFNULL(r.ABSRQTY, 0) AS rqty,
    tbo.ponoid
FROM 
    masitems m
INNER JOIN 
    masitemcategories c ON c.categoryid = m.categoryid
INNER JOIN 
    masitemmaincategory mc ON mc.MCID = c.MCID
LEFT OUTER JOIN 
    masitemtypes ty ON ty.itemtypeid = m.itemtypeid
LEFT OUTER JOIN 
    mascgmscnocitems ni ON ni.itemid = m.itemid AND ni.nocid = " + nocid + @"
INNER JOIN 
    mascgmscnoc n ON n.nocid = ni.nocid
INNER JOIN 
    tbIndents tb ON tb.nocid = n.nocid
LEFT OUTER JOIN 
    tbIndentItems tbi ON tbi.indentid = tb.indentid AND tbi.itemid = ni.itemid
LEFT OUTER JOIN 
    tbOutwards tbo ON tbo.IndentItemID = tbi.IndentItemID AND tbo.itemid = m.itemid

  
LEFT OUTER JOIN (
    SELECT 
        tr.INDENTID, 
        tri.INDENTITEMID, 
        tri.itemid, 
        tbr.ABSRQTY, 
        tbr.whinwno
    FROM 
        tbfacilityreceipts tr
    INNER JOIN 
        tbfacilityreceiptitems tri ON tri.FACRECEIPTID = tr.FACRECEIPTID
    INNER JOIN 
        tbfacilityreceiptbatches tbr ON tbr.facreceiptitemid = tri.facreceiptitemid
    WHERE 
        tr.facreceipttype = 'NO' 
        AND tr.INDENTID = " + indentid + @"
        AND tr.facilityid = " + facid + @"
) r ON r.INDENTID = tb.indentid 
     
     AND r.itemid = m.itemid 
     AND r.whinwno = tbo.inwno
WHERE 
    1 = 1 
    AND IFNULL(ni.BOOKEDQTY, 0) > 0 
    AND tb.status = 'C' 
    AND n.status = 'C' 
    AND n.nocid = " + nocid + @"
ORDER BY 
    m.itemid;
 ";


//            string qry = @"  select nvl(ni.sr,0) sr, m.itemid, m.itemcode,ty.itemtypename,m.itemname,m.strength1, m.multiple, m.unitcount, nvl(ni.BOOKEDQTY,0) *  nvl(m.unitcount,1) as indentQty 
// ,nvl(tbo.ISSUEQTY,0)*nvl(m.unitcount,1) as issueWH,nvl(rb.batchno,'-') batchno , case when rb.mfgdate is null then '-' else to_char(rb.mfgdate,'dd-MM-yyyy') end as mfgdate,  case when rb.expdate is null then '-' else to_char(rb.expdate,'dd-MM-yyyy') end as expdate
//,nvl(tbo.inwno,0) as inwno
// ,nvl(r.ABSRQTY,0) as rqty
// from masitems m 
//inner join masitemcategories c on c.categoryid = m.categoryid
//inner join masitemmaincategory mc on mc.MCID = c.MCID 
//left outer join masitemtypes ty  on ty.itemtypeid = m.itemtypeid
//left outer join mascgmscnocitems ni on ni.itemid = m.itemid and ni.nocid = " + nocid + @"

//inner join mascgmscnoc n on n.nocid = ni.nocid  

//inner JOIN tbIndents tb on 1=1  and tb.nocid = n.nocid 
//left OUTER join tbIndentItems tbi on tbi.indentid = tb.indentid and tbi.itemid = ni.itemid
//left outer join tbOutwards tbo  on  tbo.IndentItemID = tbi.IndentItemID and tbo.itemid=m.itemid
//left outer join   tbReceiptBatches rb on (rb.InwNo = tbo.InwNo) 

//left outer join 
//(
//    Select tr.INDENTID,tri.INDENTITEMID,tri.itemid,tbr.ABSRQTY,tbr.whinwno from tbfacilityreceipts tr 
//    inner join tbfacilityreceiptitems tri on tri.FACRECEIPTID = tr.FACRECEIPTID 
//    inner join tbfacilityreceiptbatches tbr on tbr.facreceiptitemid = tri.facreceiptitemid
//    where tr.facreceipttype = 'NO' and tr.INDENTID=" + indentid + @" and tr.facilityid=" + facid + @"
//) r on r.INDENTID=tb.indentid and r.INDENTITEMID=tbi.INDENTITEMID and r.itemid=m.itemid and r.whinwno=tbo.inwno
//where 1=1 and nvl(ni.BOOKEDQTY,0) > 0 and tb.status = 'C' and n.status = 'C' and n.nocid=" + nocid + @"
//order by  m.itemid ";
            //(23413)
            var myList = _context.ReceipItemBatchesDbSet
            .FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList();
            return myList;
        }


        [HttpGet("getReceiptDetailsBatchSP")] //mariadb
        public async Task<ActionResult<IEnumerable<ReceipItemBatchesDTO>>> getReceiptDetailsBatchSP(Int64 issueid, Int64 facid, Int64 indentid)
        {

            string qry = @"
SELECT IFNULL(ni.INDENTITEMID, 0) AS sr, 
       m.itemid, 
       m.itemcode, 
       ty.itemtypename, 
       m.itemname, 
       m.strength1, 
       m.multiple, 
       m.unitcount,
       IFNULL(ni.REQUESTEDQTY, 0) AS indentQty,
       IFNULL(tbo.ISSUEQTY, 0) AS issueWH,
       IFNULL(rb.batchno, '-') AS batchno, 
       CASE WHEN rb.mfgdate IS NULL THEN '-' ELSE DATE_FORMAT(rb.mfgdate, '%d-%m-%Y') END AS mfgdate, 
       CASE WHEN rb.expdate IS NULL THEN '-' ELSE DATE_FORMAT(rb.expdate, '%d-%m-%Y') END AS expdate,
       IFNULL(tbo.inwno, 0) AS inwno,
       IFNULL(r.ABSRQTY, 0) AS rqty,
        tbo.PONOID
FROM masitems m
INNER JOIN masitemcategories c ON c.categoryid = m.categoryid
INNER JOIN masitemmaincategory mc ON mc.MCID = c.MCID
LEFT OUTER JOIN masitemtypes ty ON ty.itemtypeid = m.itemtypeid
INNER JOIN MASFACDEMANDITEMS ni ON ni.itemid = m.itemid AND ni.INDENTID = " + indentid + @"
INNER JOIN MASFACTRANSFERS tb ON 1 = 1 AND tb.indentid = ni.indentid
INNER JOIN tbfacilityissues tbf ON tbf.FACINDENTID = tb.indentid
INNER JOIN tbfacilityissueitems tbi ON tbi.ISSUEID = tbf.ISSUEID AND tbi.itemid = m.itemid
LEFT OUTER JOIN tbfacilityoutwards tbo ON tbo.ISSUEITEMID = tbi.ISSUEITEMID AND tbo.itemid = m.itemid
LEFT OUTER JOIN tbfacilityreceiptbatches rb ON rb.InwNo = tbo.INWNO
LEFT OUTER JOIN (
    SELECT tr.INDENTID,
           tri.INDENTITEMID,
           tri.itemid,
           SUM(tbr.ABSRQTY) AS ABSRQTY,
           tbr.batchno,
           tr.ISSUEID,
           tri.issueitemid
    FROM tbfacilityreceipts tr
    INNER JOIN tbfacilityreceiptitems tri ON tri.FACRECEIPTID = tr.FACRECEIPTID
    INNER JOIN tbfacilityreceiptbatches tbr ON tbr.facreceiptitemid = tri.facreceiptitemid
    WHERE tr.facreceipttype = 'SP' 
      AND tr.INDENTID = " + indentid + @"
      AND tr.facilityid = " + facid + @"
      AND tr.ISSUEID = " + issueid + @"
    GROUP BY tr.INDENTID, tri.INDENTITEMID, tri.itemid, tbr.batchno, tr.ISSUEID, tri.issueitemid
) r ON r.INDENTID = tb.indentid 
    AND r.indentid = tbf.FACINDENTID 
    AND r.itemid = m.itemid 
    AND r.batchno = rb.batchno
WHERE 1 = 1 
  AND tb.status = 'C' 
  AND tbf.status = 'C' 
  AND tbf.FACINDENTID = " + indentid + @"
ORDER BY m.itemid;
";


            //            string qry = @"   select nvl(ni.INDENTITEMID,0) sr, m.itemid, m.itemcode,ty.itemtypename,m.itemname,m.strength1, m.multiple, m.unitcount
            // , 
            // nvl(ni.REQUESTEDQTY,0)  as indentQty 
            // ,nvl(tbo.ISSUEQTY,0) issueWH,nvl(rb.batchno,'-') batchno , case when rb.mfgdate is null then '-' else to_char(rb.mfgdate,'dd-MM-yyyy') end as mfgdate,  case when rb.expdate is null then '-' else to_char(rb.expdate,'dd-MM-yyyy') end as expdate
            //,nvl(tbo.inwno,0) as inwno
            // ,nvl(r.ABSRQTY,0) as rqty
            // from masitems m 
            //inner join masitemcategories c on c.categoryid = m.categoryid
            //inner join masitemmaincategory mc on mc.MCID = c.MCID 
            //left outer join masitemtypes ty  on ty.itemtypeid = m.itemtypeid
            //inner JOIN MASFACDEMANDITEMS ni on ni.itemid = m.itemid and ni.INDENTID ="+ indentid + @"
            //inner JOIN MASFACTRANSFERS tb  on 1=1  and tb.indentid = ni.indentid 
            //inner join tbfacilityissues tbf on tbf.FACINDENTID=tb.indentid
            //inner join  tbfacilityissueitems tbi on tbi.ISSUEID = tbf.ISSUEID and tbi.itemid = m.itemid
            //left outer join tbfacilityoutwards tbo  on  tbo.ISSUEITEMID = tbi.ISSUEITEMID and tbo.itemid=m.itemid
            //left outer join   tbfacilityreceiptbatches rb on (rb.InwNo = tbo.INWNO) 

            //left outer join 
            //(
            // Select tr.INDENTID,tri.INDENTITEMID,tri.itemid,sum(tbr.ABSRQTY)ABSRQTY ,tbr.batchno,tr.ISSUEID,tri.issueitemid from tbfacilityreceipts tr 
            //    inner join tbfacilityreceiptitems tri on tri.FACRECEIPTID = tr.FACRECEIPTID 
            //    inner join tbfacilityreceiptbatches tbr on tbr.facreceiptitemid = tri.facreceiptitemid
            //    where tr.facreceipttype = 'SP' and tr.INDENTID=" + indentid + @" and tr.facilityid="+ facid + @" and tr.ISSUEID="+ issueid + @"
            //    group by  tr.INDENTID,tri.INDENTITEMID,tri.itemid,tbr.batchno,tr.ISSUEID,tri.issueitemid

            //) r on r.INDENTID=tb.indentid and r.indentid= tbf.FACINDENTID and r.itemid=m.itemid and r.batchno=rb.batchno
            //where 1=1 and tb.status = 'C' and tbf.status = 'C' and tbf.FACINDENTID =" + indentid + @"
            //order by  m.itemid ";
            //(23413)

            var myList = _context.ReceipItemBatchesDbSet
            .FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList();
            return myList;
        }

        [HttpPost("postReceiptItemsSP")]  //gyan
        public async Task<ActionResult<IEnumerable<IncompleteWardIssueDTO>>> postReceiptItemsSP( Int64 rackID, Int64 facid, Int64 facReceiptId, Int64 whinwno, tbFacilityReceiptItemsModel ObjRItems)  //indentid is nocid
        {
            //GetMonthName(string date_ddmmyyyy)

            try
            {
                FacOperations ob = new FacOperations(_context);

                ob.getWhIssuedItemDataSP(facid, whinwno, facReceiptId, out Int64? indentItemId, out Int64? itemId, out Int64? batchQty, out Int64? facReceiptItemid
                                        , out string? MfgDate, out Int64? ponoid, out Int32? qastatus, out string? whissueblock, out string? expiryDate, out string? batchno);
             
                ObjRItems.ISSUEITEMID = Convert.ToInt64(indentItemId);
                ObjRItems.ITEMID = Convert.ToInt64(itemId);
                ObjRItems.FACRECEIPTID = facReceiptId;
                ObjRItems.ABSRQTY = Convert.ToInt64(batchQty);

                ob.isFacilityReceiptItemsExist(Convert.ToInt64(Convert.ToInt64(itemId)), facReceiptId, out Int64? batchQty1, out Int64? facilityReceiptItemId);
                Int64 FacReceiptitemid = 0;
                if (facilityReceiptItemId != null)
                {
                    Int64 total = Convert.ToInt64(batchQty1) + Convert.ToInt64(batchQty);
                    string strQuery1 = @"update tbFacilityReceiptItems set ABSRQTY= " + total + " where itemid=" + Convert.ToInt64(itemId) + "" +
                        " and FACRECEIPTID=" + facReceiptId + " and  FACRECEIPTITEMID = " + facilityReceiptItemId + "";
                    Int32 result1 = _context.Database.ExecuteSqlRaw(strQuery1);
                    FacReceiptitemid = Convert.ToInt64(facilityReceiptItemId);
                }
                else
                {

                    _context.tbFacilityReceiptItemsDbSet.Add(ObjRItems);  //
                    _context.SaveChanges();
                    FacReceiptitemid = Convert.ToInt64(ObjRItems.FACRECEIPTITEMID);
                }



                tbFacilityReceiptBatchesModel objRBatches = new tbFacilityReceiptBatchesModel();

                objRBatches.MFGDATE = MfgDate;
                objRBatches.EXPDATE = expiryDate;
                objRBatches.WHISSUEBLOCK = whissueblock;
                objRBatches.BATCHNO = batchno;
                objRBatches.ITEMID = Convert.ToInt64(itemId);
                objRBatches.FACRECEIPTITEMID = Convert.ToInt64(FacReceiptitemid);
                objRBatches.ABSRQTY = Convert.ToInt64(batchQty);
                objRBatches.STOCKLOCATION = Convert.ToInt64(rackID);
                objRBatches.QASTATUS = Convert.ToInt32(qastatus);
                objRBatches.WHINWNO = Convert.ToInt64(whinwno);
                objRBatches.PONOID = Convert.ToInt64(ponoid);

                _context.tbFacilityReceiptBatchesDbSet.Add(objRBatches);
                _context.SaveChanges();


                return Ok(objRBatches);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
        }



        [HttpPost("postReceiptItems")]  //gyan
        public async Task<ActionResult<IEnumerable<IncompleteWardIssueDTO>>> postReceiptItems(Int64 indentId, Int64 mponoid, Int64 rackID, Int64 facid, Int64 facReceiptId, Int64 whinwno, tbFacilityReceiptItemsModel ObjRItems)  //indentid is nocid
        {
            //GetMonthName(string date_ddmmyyyy) Int64 indentId

            try
            {
                FacOperations ob = new FacOperations(_context);

                ob.getWhIssuedItemData( indentId,facid, whinwno, facReceiptId, out Int64? indentItemId, out Int64? itemId, out Int64? batchQty, out Int64? facReceiptItemid
                                        , out string? MfgDate, out Int64? ponoid, out Int32? qastatus, out string? whissueblock, out string? expiryDate, out string? batchno);
                ObjRItems.INDENTITEMID = Convert.ToInt64(indentItemId);
                ObjRItems.ITEMID = Convert.ToInt64(itemId);
                ObjRItems.FACRECEIPTID = facReceiptId;
                ObjRItems.ABSRQTY = Convert.ToInt64(batchQty);

                ob.isFacilityReceiptItemsExist(Convert.ToInt64(Convert.ToInt64(itemId)), facReceiptId, out Int64? batchQty1, out Int64? facilityReceiptItemId);
                Int64 FacReceiptitemid = 0;
                if (facilityReceiptItemId != null)
                {
                    Int64 total = Convert.ToInt64(batchQty1) + Convert.ToInt64(batchQty);
                    string strQuery1 = @"update tbFacilityReceiptItems set ABSRQTY= " + total + " where itemid=" + Convert.ToInt64(itemId) + "" +
                        " and FACRECEIPTID=" + facReceiptId + " and  FACRECEIPTITEMID = " + facilityReceiptItemId + "";
                    Int32 result1 = _context.Database.ExecuteSqlRaw(strQuery1);
                    FacReceiptitemid = Convert.ToInt64(facilityReceiptItemId);
                }
                else
                {

                    _context.tbFacilityReceiptItemsDbSet.Add(ObjRItems);  //
                    _context.SaveChanges();
                    FacReceiptitemid = Convert.ToInt64(ObjRItems.FACRECEIPTITEMID);
                }



                tbFacilityReceiptBatchesModel objRBatches = new tbFacilityReceiptBatchesModel();

                objRBatches.MFGDATE = MfgDate;
                objRBatches.EXPDATE = expiryDate;
                objRBatches.WHISSUEBLOCK = whissueblock;
                objRBatches.BATCHNO = batchno;
                objRBatches.ITEMID = Convert.ToInt64(itemId);
                objRBatches.FACRECEIPTITEMID = Convert.ToInt64(FacReceiptitemid);
                objRBatches.ABSRQTY = Convert.ToInt64(batchQty);
                objRBatches.STOCKLOCATION = Convert.ToInt64(rackID);
                objRBatches.QASTATUS = Convert.ToInt32(qastatus);
                objRBatches.WHINWNO = Convert.ToInt64(whinwno);
                objRBatches.PONOID = Convert.ToInt64(mponoid);

                _context.tbFacilityReceiptBatchesDbSet.Add(objRBatches);
                _context.SaveChanges();


                return Ok(objRBatches);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
        }


        [HttpGet("getReceiptVouchers")]
        public async Task<ActionResult<IEnumerable<getReceiptVouchersDTO>>> getReceiptVouchers(string facId, string receiptId)
        {
            string qry = @" 
SELECT DISTINCT 
    CAST(m.itemid AS CHAR) AS ITEMID,
    CAST(m.itemcode AS CHAR) AS ITEMCODE,
    CAST(m.itemname AS CHAR) AS ITEMNAME,
    CAST(tbo.batchno AS CHAR) AS BATCHNO,
    CAST(tbo.expdate AS CHAR) AS EXPDATE,
    CAST(tbo.IssueQty * IFNULL(m.unitcount,1) AS CHAR) AS ISSUEWH,
    CAST(tfr.IndentID AS CHAR) AS INDENTID,
    CAST(tfr.FACRECEIPTID AS CHAR) AS FACRECEIPTID,
    CAST(tfi.ABSRQTY AS CHAR) AS ABSRQTY,
    CAST(rfb.ABSRQTY AS CHAR) AS BATRCHRECEIPTQTY,
    CAST(IFNULL(rc.locationno, '-') AS CHAR) AS LOCATIONNO,
    CAST(rfb.stocklocation AS CHAR) AS STOCKLOCATION,
    CAST(tfi.facreceiptitemid AS CHAR) AS FACRECEIPTITEMID,
    CAST(tbo.InwNo AS CHAR) AS INWNO,
    CAST(tfr.status AS CHAR) AS RSTATUS,
    CAST(tfi.status AS CHAR) AS RISTATUS
FROM tbIndentItems tbi
             left outer Join tbOutwards tbo on (tbo.IndentItemID = tbi.IndentItemID) 
             #left outer Join tbReceiptBatches rb on (rb.InwNo = tbo.InwNo) 
             Inner Join masItems m on (m.ItemID=tbi.ItemID)
             Inner Join tbIndents tb on (tb.Indentid=tbi.IndentId) 
            left outer join  tbFacilityReceipts tfr on (tfr.IndentID=tb.IndentID) and tfr.facilityid=" + facId + @"
            Left Outer Join tbFacilityReceiptItems tfi on  tfi.FacReceiptID=tfr.FacReceiptID and tfi.itemid=tbi.itemid    
            and tfi.INDENTITEMID=tbi.INDENTITEMID
            left outer join tbfacilityreceiptbatches rfb on rfb.facreceiptitemid=tfi.facreceiptitemid
            left outer join masracks rc on rc.rackid=rfb.stocklocation
             Where 1=1  and tb.FacilityID ="+ facId + @"     
             and tfr.FACRECEIPTID="+ receiptId + @" 
             order by m.itemname ";


            var myList = _context.getReceiptVouchersDbSet
            .FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList();

            return myList;


        }

        [HttpGet("getReceiptById")]
        public async Task<ActionResult<IEnumerable<getReceiptByIdDTO>>> getReceiptById(Int64 facId, Int64 receiptId)
        {
            String whfacId = "";
            String whreceiptId = "";

            if (facId != 0)
            {
                whfacId = "  and tfr.FacilityID ="+ facId +" ";
            }


            if (receiptId != 0)
            {
                whreceiptId = "  and tfr.FACRECEIPTID="+ receiptId + " ";
            }

            string qry = @" Select distinct m.itemid,m.itemcode,m.itemname,m.STRENGTH1 as STRENGTH,rb.batchno,rb.mfgdate,rb.expdate,rb.ABSRQTY as BatrchReceiptQTY ,tfi.facreceiptitemid,rb.InwNo,tfr.status Rstatus,tfr.FACRECEIPTID
             from tbFacilityReceipts tfr
            Left Outer Join tbFacilityReceiptItems tfi on  tfi.FacReceiptID=tfr.FacReceiptID    
              left outer join tbfacilityreceiptbatches rb on rb.facreceiptitemid=tfi.facreceiptitemid  
             Inner Join masItems m on (m.ItemID=tfi.ItemID)     
             Where 1=1 " + whfacId + @"     
             "+ whreceiptId + @"
             order by m.itemname  ";
            var myList = _context.getReceiptByIdDbSet
            .FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList();

            return myList;


        }

    }


}
