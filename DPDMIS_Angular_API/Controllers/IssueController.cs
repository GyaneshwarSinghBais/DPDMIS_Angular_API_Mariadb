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

namespace DPDMIS_Angular_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IssueController : ControllerBase
    {
        private readonly MariaDbContext _context;
        public IssueController(MariaDbContext context)
        {
            _context = context;
        }

        [HttpGet("getIncompleteWardIssue")]
        public async Task<ActionResult<IEnumerable<IncompleteWardIssueDTO>>> getIncompleteWardIssue(string faclityId,string status,string wardid)
        {
            string whstatus = "";
            if (status != "0")
            {
                if (status == "I")
                {
                    whstatus = " and a.status = 'IN' ";
                }
                else
                {
                    whstatus = " and a.status = 'C' ";
                }
            }

            string whFac = "";
            if (wardid == "0")
            {
                whFac = " and b.FacilityID=0 ";
            }
            else
            {
                whFac = " and b.FacilityID=" + wardid + @" ";
            }

            string qry = @"  Select a.WardID,b.WardName,
            a.IssueNo,a.IssueDate,a.WRequestDate,a.WRequestBy, a.Status,a.IssueID, 0 as indentid
             from tbFacilityIssues a
             Inner Join masFacilityWards b on (b.WardID=a.WardID) "+ whFac + @"
             Inner Join masFacilities fac on (fac.FacilityID=a.FacilityID)    
             Inner Join masAccYearSettings m1 on (a.IssueDate Between m1.StartDate and m1.EndDate)
             Where 1=1 and a.FacilityID=" + faclityId + @"   "+ whstatus + @"
             Order By a.IssueDate desc ";

            var context = new IncompleteWardIssueDTO();

            var myList = _context.IncompleteWardIssueDTOs
            .FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList();

            return myList;
        }

        [HttpGet("getFacilityWards")]
        public async Task<ActionResult<IEnumerable<FacilityWardDTO>>> getFacilityWards(string wardid)
        {
            string whFac = "";
            if (wardid == "0")
            {
                whFac = " and a.FacilityID=0 ";
            }
            else
            {
                whFac = " and a.FacilityID="+ wardid + @" ";
            }
            string qry = @"   Select a.WardID, a.WardName
                 from masFacilityWards a
                 Where a.IsActive=1 "+ whFac + @"  Order By a.WardName ";

            //var context = new FacilityWardDTO();

            var myList = _context.FacilityWardDTOs
            .FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList();

            return myList;
        }

        [HttpPost("postIssueNo")] //mariadb
        public async Task<ActionResult<IEnumerable<IncompleteWardIssueDTO>>> postWardIssue(string facid, tbFacilityGenIssue objeIssue)
        {
            try
            {
                // Initialize and populate issue object
                FacOperations ob = new FacOperations(_context);
                string validIssueDate = ob.FormatDate(objeIssue.ISSUEDATE);
                objeIssue.ISSUEDATE = validIssueDate;
                objeIssue.ISSUEDDATE = validIssueDate;
                objeIssue.WREQUESTDATE = validIssueDate;
                objeIssue.ISSUETYPE = "NO";
                objeIssue.ISUSEAPP = "Y";

                // Generate issue number and save the issue
                string issueno = getGeneratedIssueNo(facid);
                objeIssue.ISSUENO = issueno;

                _context.tbFacilityGenIssueDbSet.Add(objeIssue);
                await _context.SaveChangesAsync();

                // Convert facility ID to long
                Int64 ffcid = Convert.ToInt64(facid);

                // Directly call the query logic to fetch details
                string qry = @"
            SELECT 
                a.WardID AS WARDID, 
                b.WardName AS WARDNAME, 
                a.IssueNo AS ISSUENO, 
                a.IssueDate AS ISSUEDATE, 
                a.WRequestDate AS WREQUESTDATE, 
                a.WRequestBy AS WREQUESTBY, 
                a.STATUS, 
                a.IssueID AS ISSUEID, 
                a.INDENTID
            FROM tbFacilityIssues a
            INNER JOIN masFacilityWards b ON b.WardID = a.WardID
            INNER JOIN masFacilities fac ON fac.FacilityID = a.FacilityID
            INNER JOIN masAccYearSettings m1 ON a.IssueDate BETWEEN m1.StartDate AND m1.EndDate
            WHERE a.FacilityID = @facid 
              AND ifnull(a.status,'IN') = 'IN' 
              AND a.IssueNo = @Issueno
            ORDER BY a.IssueDate";

                var parameters = new MySqlParameter[]
                {
            new MySqlParameter("@facid", ffcid),
            new MySqlParameter("@Issueno", issueno)
                };

                var issueDetails = await _context.IncompleteWardIssueDTOs
                    .FromSqlRaw(qry, parameters)
                    .ToListAsync();

                return Ok(issueDetails);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("getGeneratedIssueNo")]
        public string getGeneratedIssueNo(string facId)
        {

            FacOperations op = new FacOperations(_context);
            string getNo = op.FacAutoGenerateNumbers(facId, false, "NO");
            return getNo;

        }

        [HttpGet("getLastIssueDT")]
        public async Task<ActionResult<IEnumerable<LastIssueDTO>>> getLastIssueDT(Int64 facid)
        {
            string qry = @" select max(ISSUEDATE) as ISSUEDATE from tbFacilityIssues where FACILITYID="+ facid + @" and status='C' ";
            var myList = _context.LastIssueDbSet
           .FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList();

            return myList;

        }

        [HttpGet("getIssueDetails")]
        public async Task<ActionResult<IEnumerable<IncompleteWardIssueDTO>>> getIssueDetails(string Issueno, long facid)
        {
            string qry = @"
        SELECT 
            a.WardID AS WARDID, 
            b.WardName AS WARDNAME, 
            a.IssueNo AS ISSUENO, 
            a.IssueDate AS ISSUEDATE, 
            a.WRequestDate AS WREQUESTDATE, 
            a.WRequestBy AS WREQUESTBY, 
            a.STATUS, 
            a.IssueID AS ISSUEID, 
            a.INDENTID
        FROM tbFacilityIssues a
        INNER JOIN masFacilityWards b ON b.WardID = a.WardID
        INNER JOIN masFacilities fac ON fac.FacilityID = a.FacilityID
        INNER JOIN masAccYearSettings m1 ON a.IssueDate BETWEEN m1.StartDate AND m1.EndDate
        WHERE a.FacilityID = @facid 
          AND ifnull(a.status,'IN') = 'IN' 
          AND a.IssueNo = @Issueno
        ORDER BY a.IssueDate";

            var parameters = new MySqlParameter[]
            {
        new MySqlParameter("@facid", facid),
        new MySqlParameter("@Issueno", Issueno)
            };

            // Fetch data and convert to list
            var myList = await _context.IncompleteWardIssueDTOs
                .FromSqlRaw(qry, parameters)
                .ToListAsync();

            return Ok(myList);
        }



        //    [HttpGet("getIssueDetails")]
        //private async Task<ActionResult<IEnumerable<IncompleteWardIssueDTO>>> getIssueDetails(string Issueno, Int64 facid)
        //{
        //    string qry = @"SELECT a.WardID WARDID, b.WardName WARDNAME, a.IssueNo as ISSUENO, a.IssueDate as ISSUEDATE, a.WRequestDate as WREQUESTDATE, a.WRequestBy as WREQUESTBY, a.STATUS, a.IssueID as ISSUEID,a.INDENTID 
        //                   FROM tbFacilityIssues a
        //                   INNER JOIN masFacilityWards b ON b.WardID = a.WardID
        //                   INNER JOIN masFacilities fac ON fac.FacilityID = b.FacilityID
        //                   INNER JOIN masAccYearSettings m1 ON a.IssueDate BETWEEN m1.StartDate AND m1.EndDate
        //                   WHERE a.FacilityID = :facid AND a.status = 'IN' AND a.IssueNo = :Issueno
        //                   ORDER BY a.IssueDate";



        //    OracleParameter[] parameters = new OracleParameter[]
        //    {
        //        new OracleParameter(":facid", OracleDbType.Int64, facid, ParameterDirection.Input),
        //        new OracleParameter(":Issueno", OracleDbType.Varchar2, Issueno, ParameterDirection.Input)
        //    };

        //    var myList = _context.IncompleteWardIssueDTOs
        //        .FromSqlRaw(qry, parameters)
        //        .ToList();

        //    return myList;
        //}

        [HttpGet("getWardIssueItems")]
        public async Task<ActionResult<IEnumerable<WardIssueItemsDTO>>> getWardIssueItems(string faclityId, string issueId)
        {
            string qry = @"   select mi.ITEMCODE ||'-' || mi.itemname || '-' || to_char( (case when (b.qastatus ='1' or  mi.qctest='N') then (sum(nvl(b.absrqty,0)) - sum(nvl(iq.issueqty,0))) end) ) as name              
                         , mi.itemid
                         from tbfacilityreceiptbatches b   
                         inner join tbfacilityreceiptitems i on b.facreceiptitemid=i.facreceiptitemid 
                         inner join tbfacilityreceipts t on t.facreceiptid=i.facreceiptid  
                         inner join vmasitems mi on mi.itemid=i.itemid 
                         inner join masfacilities f  on f.facilityid=t.facilityid 
                         left outer join 
                         (  
                           select  fs.facilityid,fsi.itemid,ftbo.inwno,sum(nvl(ftbo.issueqty,0)) issueqty   
                             from tbfacilityissues fs 
                           inner join tbfacilityissueitems fsi on fsi.issueid=fs.issueid 
                           inner join tbfacilityoutwards ftbo on ftbo.issueitemid=fsi.issueitemid 
                           where 1=1  and fs.facilityid=" + faclityId + @"          
                           group by fsi.itemid,fs.facilityid,ftbo.inwno                     
                         ) iq on b.inwno = Iq.inwno and iq.itemid=i.itemid and iq.facilityid=t.facilityid                 
                         Where  T.Status = 'C'  And (b.Whissueblock = 0 or b.Whissueblock is null) and b.expdate>curdate() 
                        and t.facilityid= " + faclityId + @" and mi.itemid not in (select distinct itemid from tbfacilityissueitems where issueid = " + issueId + @")
                        and (   (case when (b.qastatus ='1' or  mi.qctest='N') then (nvl(b.absrqty,0) - nvl(iq.issueqty,0)) end)) > 0
                        group by  mi.ITEMCODE, mi.itemid,b.qastatus,mi.qctest,mi.itemname
                        order by  mi.itemname ";

            // var context = new WardIssueItemsDTO();

            var myList = _context.WardIssueItemsDbSet
            .FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList();

            return myList;
        }

        [HttpGet("getItemStock")] //checking purpose
        public async Task<ActionResult<IEnumerable<ItemStockDTO>>> getItemStock(string faclityId, string itemid, Int64 indentid = 0)
        {

            string qry = @" select  to_char( (case when (b.qastatus ='1' or  mi.qctest='N') then (sum(nvl(b.absrqty,0)) - sum(nvl(iq.issueqty,0))) end) ) as stock,mi.itemid,
        nvl(mi.multiple,1) as multiple,nvl(INDENTqty,0) as INDENTQTY
                         from tbfacilityreceiptbatches b   
                         inner join tbfacilityreceiptitems i on b.facreceiptitemid=i.facreceiptitemid 
                         inner join tbfacilityreceipts t on t.facreceiptid=i.facreceiptid  
                         inner join vmasitems mi on mi.itemid=i.itemid 
                         inner join masfacilities f  on f.facilityid=t.facilityid 
                         left outer join 
                         (  
                           select  fs.facilityid,fsi.itemid,ftbo.inwno,sum(nvl(ftbo.issueqty,0)) issueqty   
                             from tbfacilityissues fs 
                           inner join tbfacilityissueitems fsi on fsi.issueid=fs.issueid 
                           inner join tbfacilityoutwards ftbo on ftbo.issueitemid=fsi.issueitemid 
                           where 1=1  and fs.facilityid=" + faclityId + @"        
                           group by fsi.itemid,fs.facilityid,ftbo.inwno                     
                         ) iq on b.inwno = Iq.inwno and iq.itemid=i.itemid and iq.facilityid=t.facilityid  
                            LEFT OUTER JOIN (
                                                 select b.BookedQty AS INDENTqty ,B.ItemID 
                            from maswardindents a  
                            inner join maswardindentitems b on a.nocid =b.nocid   
                            inner join masfacilitywards f on f.wardid=a.wardid 
                            left outer join tbfacilityissues d on d.indentid=a.NocID and IssueType='NO'  
                            left outer join tbfacilityissueitems c on c.ItemID=b.ItemID and c.issueid=d.issueid   
                            where a.status='C' and b.bookedflag in ('B','C')  and b.BookedQty >0  and  a.NOCID = " + indentid + @"
                            AND B.ITEMID=" + itemid + @" 
                         )  idt on idt.itemid = mi.itemid
                         Where  T.Status = 'C'  And (b.Whissueblock = 0 or b.Whissueblock is null) and b.expdate>curdate() 
                        and t.facilityid= " + faclityId + @"   and mi.itemid = " + itemid + @"
                        and (   (case when (b.qastatus ='1' or  mi.qctest='N') then (nvl(b.absrqty,0) - nvl(iq.issueqty,0)) end)) > 0
                        group by   mi.itemid,b.qastatus,mi.qctest,mi.multiple,INDENTqty ";

            var context = new ItemStockDTO();

            var myList = _context.ItemStockDBSet
            .FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList();

            return myList;
        }

       
       
        [HttpPost("ChangpasswordbyID")]
        public IActionResult ChangpasswordbyID(string userid, string newpass)
        {
            FacOperations ObjFacOp = new FacOperations(_context);

            ObjFacOp.changpassword(userid, newpass, out string message);
            if (message == "Password Successfully Updated")
            {


                return Ok();

            }
            else
            {
                return BadRequest("Invalid credentials."); }


        }

        [HttpPost("postWardIssue")]
        public IActionResult postWardIssue(tbFacilityIssueItems ObjtbFacilityIssueItems, string facid)
        {

            try
            {
                Int64 issueitemid = 0;
                if (ObjtbFacilityIssueItems.ISSUEITEMID == 0)
                {
                    _context.tbFacilityIssueItems.Add(ObjtbFacilityIssueItems);
                    _context.SaveChanges();
                     issueitemid = getIssueItemid(ObjtbFacilityIssueItems.ISSUEID, ObjtbFacilityIssueItems.ITEMID);
                }
                else
                {
                    issueitemid = ObjtbFacilityIssueItems.ISSUEITEMID;

                }
             
               
                UpdateFacilityAllotQtyOP(facid, ObjtbFacilityIssueItems.ITEMID, ObjtbFacilityIssueItems.ISSUEQTY, issueitemid);
                return Ok("Success");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("getIssueItemid")] //checking purpose
        private Int64 getIssueItemid(Int32 issueId, Int32 itemid)
        {
            string qry = @" Select IssueItemID,ISSUEID,ItemID from tbFacilityIssueItems where IssueID=" + issueId + " and ItemID=" + itemid;


            //var context = new getIssueItemIdDTO();

            var myList = _context.getIssueItemIdDbSet
            .FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList();

            if (myList.Count > 0)
            {
                int issueItemID = myList[0].ISSUEITEMID; // Assuming IssueItemID is an integer
                return Convert.ToInt64(issueItemID.ToString());
            }
            else
            {
                return 0; // Or any other appropriate indication
            }
        }

        [HttpGet("UpdateFacilityAllotQtyOP")]
        private string UpdateFacilityAllotQtyOP(string FacilityID, Int64 ItemID, Int64 IssueQty, Int64 IssueItemID)
        {
            string WhereConditions = " and (rb.ExpDate >= curdate() or nvl(rb.ExpDate,curdate()) >= curdate())";


            deleteIssueItemIdIfExist(IssueItemID);

            Int64 TotAllotQty = IssueQty;

            string qry = @" select distinct rb.FACReceiptItemid FacReceiptItemID,rb.BatchNo,rb.MfgDate,rb.ExpDate,nvl(rb.AbsRqty,0) AbsRQty,nvl(x.issueQty,0) AllotQty , nvl(rb.AbsRqty,0)-nvl(x.issueQty,0) avlQty
         , rb.Inwno ,nvl(rb.whinwno,0) whinwno
          from tbFacilityReceiptBatches rb  
          inner join tbfacilityreceiptitems i on rb.FACreceiptitemid=i.FACreceiptitemid  
          inner join tbfacilityreceipts r on r.FACreceiptid=i.FACreceiptid 
          left outer join (
            select   '' BatchNo,'' MfgDate,'' ExpDate,sum(nvl(a.IssueQty,0)) issueQty, 0 AbsRQty,  
          a.Inwno   
          from  tbfacilityoutwards a
          inner join tbfacilityissueitems tbi on tbi.issueitemid =a.issueitemid   
          inner join tbfacilityissues tb on tb.issueid=tbi.issueid    
          Where  tb.FacilityID=" + FacilityID + @"   and tbi.itemid =" + ItemID + @"  group by a.Inwno    
          ) x on x.inwno=rb.inwno
          where rb.qastatus=1 and r.status='C' and nvl(rb.whissueblock,0) in (0) and r.FacilityID=" + FacilityID + @"   and i.ItemID= " + ItemID + @"    
           and (rb.ExpDate >= curdate() or nvl(rb.ExpDate,curdate()) >= curdate())  and (nvl(rb.AbsRqty,0)-nvl(x.issueQty,0))>0
           and Round(rb.ExpDate-curdate(),0) >= 15 order by expdate";


            //var context = new getBatchesDTO();

            var myList = _context.getBatchesDbSet
            .FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList();


            // Assuming IssueItemID is an integer
            int issueItemID = 0; // Default value if not found

            // Loop through myList to find the desired object
            foreach (var item in myList)
            {

                Int64 dblCurAllot = 0;
                Int64 availableQty = Convert.ToInt64(item.avlQty.ToString());

                Int64 issueQty = 0;
                string mIssueItemID = IssueItemID.ToString();
                string mItemID = ItemID.ToString();
                string mFacReceiptItemID = item.FacReceiptItemID.ToString();
                string mInWno = item.Inwno.ToString();
                string mWhInWno = item.whinwno.ToString();
                //string mIssueQty = dblCurAllot.ToString();
                string mIssued = "0";
                string strSQL = "";

                if (TotAllotQty <= availableQty)
                {
                    issueQty = TotAllotQty;
                    TotAllotQty -= issueQty;
                   

                    #region Save Data
                    strSQL = "Insert into tbFacilityOutwards (IssueItemID,ItemID,FacReceiptItemID,IssueQty,Issued,Inwno,whinwno) values (" +
                        mIssueItemID + "," + mItemID + "," + mFacReceiptItemID + "," + issueQty + "," + mIssued + "," + mInWno + ",'" + mWhInWno + "' )";

                    Int32 result3 = _context.Database.ExecuteSqlRaw(strSQL);


               
                    #endregion
                    tbFacilityOutwardsModel obj = new tbFacilityOutwardsModel();
                    obj.ISSUEITEMID = Convert.ToInt64(mIssueItemID);
                    obj.ITEMID = Convert.ToInt64(mItemID);
                    obj.FACRECEIPTITEMID = Convert.ToInt64(mFacReceiptItemID);
                    obj.ISSUEQTY = Convert.ToInt64(issueQty);
                    obj.ISSUED = Convert.ToInt64(mIssued);
                    obj.INWNO = Convert.ToInt64(mInWno);
                    obj.WHINWNO = Convert.ToInt64(mWhInWno);

                    posttboOutWards(obj);

                    break;


                }

                else
                {
                    issueQty = availableQty;
                    TotAllotQty -= issueQty;
                    #region Save Data
                    strSQL = "Insert into tbFacilityOutwards (IssueItemID,ItemID,FacReceiptItemID,IssueQty,Issued,Inwno,whinwno) values (" +
                        mIssueItemID + "," + mItemID + "," + mFacReceiptItemID + "," + issueQty + "," + mIssued + "," + mInWno + ",'" + mWhInWno + "')";

                    Int32 result3 = _context.Database.ExecuteSqlRaw(strSQL);
                    #endregion

                    tbFacilityOutwardsModel obj = new tbFacilityOutwardsModel();
                    obj.ISSUEITEMID = Convert.ToInt64(mIssueItemID);
                    obj.ITEMID = Convert.ToInt64(mItemID);
                    obj.FACRECEIPTITEMID = Convert.ToInt64(mFacReceiptItemID);
                    obj.ISSUEQTY = Convert.ToInt64(issueQty);
                    obj.ISSUED = Convert.ToInt64(mIssued);
                    obj.INWNO = Convert.ToInt64(mInWno);
                    obj.WHINWNO = Convert.ToInt64(mWhInWno);
                    if (TotAllotQty == 0)
                    {
                        break;
                    }
                    else
                    {
                        posttboOutWards(obj);
                        continue;


                    }


                }

            }

            return "Test";
        }

        [HttpGet("deleteIssueItemIdIfExist")]
        public void deleteIssueItemIdIfExist(Int64 IssueItemID)
        {
            string qry = "Delete from tbFacilityOutwards where IssueItemID=" + IssueItemID;

            var context = new getIssueItemIdDTO();

            _context.Database.ExecuteSqlRaw(qry);

            // _context.getIssueItemIdDbSet
            //.FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList();


        }

        [HttpPost("posttboOutWards")]
        public void posttboOutWards(tbFacilityOutwardsModel ObjtbFacilityOutwardsModel)
        {
            try
            {
                _context.tbFacilityOutwardsDbSet.Add(ObjtbFacilityOutwardsModel);
                _context.SaveChanges();

            }
            catch (Exception ex)
            {

            }
        }

        [HttpPut("completeWardIssues")]
        public IActionResult completeWardIssues(Int64 IssueID)
        {
            string strQuery1 = "Update tbFacilityIssues set Status='C',IssuedDate=curdate() Where IssueID=" + IssueID;
            Int32 result1 = _context.Database.ExecuteSqlRaw(strQuery1);

            string strQuery2 = @"Update tbFacilityOutwards set Status='C', Issued=1
                                      Where IssueItemID in (Select IssueItemID from tbFacilityIssueItems Where IssueID=" + IssueID + ")";
            Int32 result2 = _context.Database.ExecuteSqlRaw(strQuery2);

            string strQuery3 = "Update tbFacilityIssueItems set Status='C',Issued=1 Where IssueID=" + IssueID;
            Int32 result3 = _context.Database.ExecuteSqlRaw(strQuery3);

            Int32 totResult = result1 + result2 + result3;

            if (totResult != 0)
            {
                return Ok("Successfully Updated");
            }
            else
            {
                return BadRequest("Some Error Occuered May be all 3 queries are not excecuted successfully.");
            }

        }



        [HttpDelete("deleteWardIssues")] //mariadb
        public IActionResult deleteWardIssues(Int64 IssueID)
        {
            string strQuery1 = "Delete from tbFacilityIssues  Where IssueID=" + IssueID;
            Int32 result1 = _context.Database.ExecuteSqlRaw(strQuery1);

            string strQuery2 = @"Delete from tbFacilityOutwards Where IssueItemID in (Select IssueItemID from tbFacilityIssueItems Where IssueID=" + IssueID + ")";
            Int32 result2 = _context.Database.ExecuteSqlRaw(strQuery2);

            string strQuery3 = "Delete from tbFacilityIssueItems Where IssueID=" + IssueID;
            Int32 result3 = _context.Database.ExecuteSqlRaw(strQuery3);

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

        [HttpGet("FacilityIssueCurrectstock")] //mariadb
        public async Task<ActionResult<IEnumerable<FacilityIssueCurrentStockDTO>>> FacilityIssueCurrectstock(string faclityId, string itemid, string catname, string issueid)
        {
            FacOperations op = new FacOperations(_context);
            Int32 hodid = op.geFACHOID(faclityId);
            string whcatid = "";
            if (catname == "D")
            {
                if (hodid == 7)
                {
                    whcatid = " and c.categoryid in (58,59,60,61)";
                }
                else
                {
                    whcatid = " and c.categoryid in (52)";
                }
            }
            else if (catname == "C")
            {

                whcatid = " and c.categoryid not in (52,58,59,60,61)";

            }
            else
            {

            }

            string whclause = "";
            if (itemid == null)
            {

            }
            else if (itemid == "")
            {

            }
            else if (itemid == "0")
            {

            }
            else
            {
                whclause = " and mi.itemid=" + itemid;
            }
            string qry = @"   select c.categoryname,mi.ITEMCODE,ty.itemtypename,mi.itemname,mi.strength1,  
                 (case when (b.qastatus ='1' or  mi.qctest='N') then (sum(nvl(b.absrqty,0)) - sum(nvl(iq.issueqty,0))) end) ReadyForIssue                 
                 ,t.facilityid, mi.itemid,c.categoryid, case when mi.ISEDL2021='Y' then 'EDL' else 'Non EDL' end as EDLType
   ,nvl(fiss.facissueqty,0) as facissueqty,nvl(fiss.issueitemid,0) as  issueitemid
                 from tbfacilityreceiptbatches b   
                 inner join tbfacilityreceiptitems i on b.facreceiptitemid=i.facreceiptitemid 
                 inner join tbfacilityreceipts t on t.facreceiptid=i.facreceiptid  
                 inner join masitems mi on mi.itemid=i.itemid 
                 left outer join masitemcategories c on c.categoryid=mi.categoryid
                 left outer join masitemtypes ty on ty.itemtypeid=mi.itemtypeid
                 inner join masfacilities f  on f.facilityid=t.facilityid 
                 left outer join 
                 (  
                   select  fs.facilityid,fsi.itemid,ftbo.inwno,sum(nvl(ftbo.issueqty,0)) issueqty   
                     from tbfacilityissues fs 
                   inner join tbfacilityissueitems fsi on fsi.issueid=fs.issueid 
                   inner join tbfacilityoutwards ftbo on ftbo.issueitemid=fsi.issueitemid 
                   where fs.status = 'C'  and fs.facilityid=" + faclityId + @"          
                   group by fsi.itemid,fs.facilityid,ftbo.inwno                     
                 ) iq on b.inwno = Iq.inwno and iq.itemid=i.itemid and iq.facilityid=t.facilityid 
left outer join
                 (
                 select tb.ISSUEID,tbi.issueitemid,tbi.itemid,sum(tbo.issueqty) as facissueqty from tbfacilityissues tb
                            inner join tbfacilityissueitems tbi on tbi.ISSUEID=tb.ISSUEID
                            inner join tbfacilityoutwards tbo on tbo.issueitemid=tbi.issueitemid
                            where tb.ISSUEID=" + issueid + @" and facilityid=" + faclityId + @"
                            group by tb.ISSUEID,tbi.issueitemid,tbi.itemid
                 )fiss on fiss.itemid=mi.itemid
                 Where 1=1 " + whclause + @" and  T.Status = 'C'  And (b.Whissueblock = 0 or b.Whissueblock is null) and b.expdate>curdate() 
                and t.facilityid= " + faclityId + @"  " + whcatid + @"
                and (   (case when (b.qastatus ='1' or  mi.qctest='N') then (nvl(b.absrqty,0) - nvl(iq.issueqty,0)) end)) > 0
                group by  fiss.facissueqty,fiss.issueitemid,  mi.ITEMCODE, t.facilityid, mi.itemid,b.qastatus,mi.qctest,mi.itemname,mi.strength1,c.categoryname,c.categoryid,itemtypename,mi.ISEDL2021
                order by c.categoryid, mi.itemname ";

            // var context = new StockReportFacilityDTO();

            var myList = _context.FacilityIssueCurrentStockDbSet
            .FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList();

            return myList;
        }



        [HttpGet("getFacilityIssueBatches")]
        public async Task<ActionResult<IEnumerable<getFacilityIssueBatchesDTO>>> getFacilityIssueBatches(string faclityIssueItemId)
        {
            string qry = @"   select rb.inwno,
                             rb.batchno,  rb.mfgdate , rb.expdate,
                             nvl(tbo.issueqty,0) as facissueqty 
                            from tbfacilityissues tb
                            inner join tbfacilityissueitems tbi on tbi.ISSUEID=tb.ISSUEID
                            inner join tbfacilityoutwards tbo on tbo.issueitemid=tbi.issueitemid
                            inner join tbfacilityreceiptbatches rb on rb.inwno=tbo.inwno
                            where   tbi.ISSUEITEMID=" + faclityIssueItemId + " ";

           

            var myList = _context.getFacilityIssueBatchesDbSet
            .FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList();

            return myList;
        }


        [HttpGet("getIssueVoucherPdf")]
        public async Task<ActionResult<IEnumerable<getIssueVoucherPdfDTO>>> getIssueVoucherPdf(string issueId)
        {
            string qry = @"     SELECT 
    rb.inwno,
    rb.batchno,
    DATE_FORMAT(rb.mfgdate, '%d-%m-%Y') AS mfgdate,
    DATE_FORMAT(rb.expdate, '%d-%m-%Y') AS expdate,
    IFNULL(tbo.issueqty, 0) AS facissueqty,
    m.itemcode,
    m.itemname,
    m.strength1,
    tb.ISSUENO,
    DATE_FORMAT(tb.ISSUEDDATE, '%d-%m-%Y') AS IssueDT,
    tbw.wardname,
    CASE 
        WHEN tb.status = 'C' THEN 'Complete'
        ELSE 'Incomplete'
    END AS status
FROM 
    tbfacilityissues tb
LEFT JOIN 
    masfacilitywards tbw ON tbw.wardid = tb.wardid
INNER JOIN 
    tbfacilityissueitems tbi ON tbi.ISSUEID = tb.ISSUEID
INNER JOIN 
    masitems m ON m.itemid = tbi.itemid
INNER JOIN 
    tbfacilityoutwards tbo ON tbo.issueitemid = tbi.issueitemid
INNER JOIN 
    tbfacilityreceiptbatches rb ON rb.inwno = tbo.inwno
WHERE 
    tb.ISSUEID = "+ issueId + @";
 ";



            var myList = _context.getIssueVoucherPdfDbSet
            .FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList();

            return myList;
        }
    }
}
