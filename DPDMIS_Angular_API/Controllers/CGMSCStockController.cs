using CgmscHO_API.DTO;
using CgmscHO_API.Utility;
using DPDMIS_Angular_API.Data;
using DPDMIS_Angular_API.DTO.CGMSCStockDTO;
using DPDMIS_Angular_API.DTO.IndentDTO;
using DPDMIS_Angular_API.DTO.IssueDTO;
using DPDMIS_Angular_API.DTO.ReceiptDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace DPDMIS_Angular_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CGMSCStockController : ControllerBase
    {
        //private readonly OraDbContext _context;
        private readonly MariaDbContext _context;

        public CGMSCStockController(MariaDbContext context)
        {
            _context = context;
        }


       // [Authorize(Roles = "FAC")]  //SUP,LAB,WHU,DOH,FAC,MOH
        [HttpGet("getFileStorageLocation")]
        public async Task<ActionResult<IEnumerable<GetFileStorageLocationDTO>>> getFileStorageLocation(string whid)
        {

            string qry = @"Select nvl(RackID,0) RackID,nvl(locationno,'Null') locationno from masracks where warehouseid=" + whid + " order by locationno  ";
            var myList = _context.GetFileStorageLocationDbSet
            .FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList();
            return myList;
        }

        [HttpGet("getOpeningStocksReport")]
        public async Task<ActionResult<IEnumerable<GetOpeningStocksRptDTO>>> getOpeningStocksReport(String itemid, Int64 usrFacilityID)
        {
            string whcluase = "";
            if (itemid != "" && itemid != "0")
            {
                whcluase = " and mi.itemid=" + itemid;
            }

            string qry = @"  select  row_number() over (order by bt.inwno) as id ,bt.inwno,nvl(i.absrqty,0) opening_qty,to_char(FACreceiptdate,'dd/MM/yyyy') as receiptdate ,r.facilityid,i.itemid,mi.itemcode, 
                   mi.unit,mi.strength1,mi.itemname,bt.batchno,to_char(bt.mfgdate,'dd/MM/yyyy') as mfgdate,to_char(bt.expdate,'dd/MM/yyyy') as expdate,bt.absrqty 
,      r.facreceiptid as  ReceiptID,i.FACreceiptitemid as ReceiptItemID              
from tbfacilityreceipts r 
                    inner join tbfacilityreceiptitems i on r.FACreceiptid=i.FACreceiptid 
                 inner join tbFacilityReceiptBatches bt on bt.FACreceiptitemid=i.FACreceiptitemid 
                   inner join masitems mi on mi.itemid=i.itemid  where 1=1 " + whcluase + @" and  FACreceipttype='FC' and r.facilityid=" + usrFacilityID + @"
              Order By mi.ItemID, bt.BatchNo  ";

            var myList = _context.GetOpeningStocksRptDbSet
            .FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList();
            return myList;
        }

        [HttpPut("SaveBatchRecord")]   //MariaDB
        public IActionResult SaveBatchRecord(Int64 mItemID, String mBatchNo, String mStockLocation, String mMfgDate, String mExpDate, String mAbsRQty, string facilityId, string receiptID)
        {
            String ErrMsg = "";


            master msterObj = new master(_context);

            string mShortQty = "0";
            string mReceiptQty = "";

            double dblAbsRQty = double.Parse(mAbsRQty);
            double dblShortQty = double.Parse(mShortQty);
            double dblReceiptQty = dblAbsRQty - dblShortQty;
            mReceiptQty = dblReceiptQty.ToString();

            try
            {
                if (mMfgDate != "")
                {
                    string[] mfgdate = mMfgDate.Replace('-', '/').Split(new char[] { '/' });
                    if (mfgdate.Length == 2)
                        mMfgDate = "01/" + mMfgDate;
                }
                if (mExpDate != "")
                {
                    string[] expdate = mExpDate.Replace('-', '/').Split(new char[] { '/' });
                    if (expdate.Length == 2)
                    {
                        int expYear = int.Parse(expdate[1]);
                        int expMon = int.Parse(expdate[0]);
                        mExpDate = DateTime.DaysInMonth(expYear, expMon).ToString() + "/" + mExpDate;
                    }
                }
            }
            catch
            {
                ErrMsg += "Format error. Invalid date.";
            }



            ErrMsg += master.CheckDate(ref mMfgDate, "Mfg date", false);
            ErrMsg += master.CheckDate(ref mExpDate, "Exp date", false);

            string mReceiptItemID = msterObj.getOPReceiptItemidID(receiptID, mItemID.ToString());

            if (mReceiptItemID == "0" || mReceiptItemID == "")
            {
                string strQuery1 = "Insert into tbfacilityreceiptitems (FACReceiptID, ItemID, AbsRQty) values (" + receiptID + ", " + mItemID + ",0)";
                _context.Database.ExecuteSqlRaw(strQuery1);

                mReceiptItemID = msterObj.getOPReceiptItemidID(receiptID, mItemID.ToString());

            }



            if (mReceiptItemID != "0" && mReceiptItemID != "")
            {
                if (msterObj.getSameBatchInItems(mBatchNo, mReceiptItemID, mItemID.ToString()))
                {
                    string strQuery = "Insert into tbFacilityReceiptBatches (FACReceiptItemID,ItemID,BatchNo,StockLocation,MfgDate,ExpDate,AbsRQty,ShortQty,qastatus)" +
                        " values (" + mReceiptItemID + ", " + mItemID + ", '" + mBatchNo + "', '" + mStockLocation + "', " + mMfgDate + ", " + mExpDate + ", " + mReceiptQty + ", " + mShortQty + ",'1')";

                    _context.Database.ExecuteSqlRaw(strQuery);

                    string strQuery2 = "update  tbfacilityreceiptitems set AbsRQty=(AbsRQty+" + mReceiptQty + ")  where FACReceiptItemID=" + mReceiptItemID;
                    _context.Database.ExecuteSqlRaw(strQuery2);

                    return Ok("Added Successfully");

                }
                else
                {
                    return BadRequest("Your Batch No is Duplicate for this Items");
                }

            }
            else
            {
                return BadRequest("mReceiptItemID is not null or zero");
            }

        }

        [HttpPut("FreezeOpeningStock")]
        public IActionResult FreezeOpeningStock(Int64 mReceiptID)
        {

            master msterObj = new master(_context);

            bool IsReceipt = msterObj.isFacilityReceipt(mReceiptID.ToString());

            if (!IsReceipt)
            {
                return BadRequest("All the entries must be done before complete freezing the opening stock.");
            }

            // check all total qty vs sum of batch qty      


            if (msterObj.checkstock(mReceiptID.ToString()) == "True")
            {
                string strQuery = " update tbfacilityreceipts set Status='C' where facreceiptid=" + mReceiptID;
                _context.Database.ExecuteSqlRaw(strQuery);
                return Ok("Freezed Successfully");
            }
            else
            {
                //msg 
                string msg = "";
                if (msterObj.checkstock(mReceiptID.ToString()) == "False")
                {
                    msg = "Please Enter Opening Quantity along with batches";
                }
                else
                {
                    msg = "Please Enter Remaining batch quantity of Drugcode " + msterObj.checkstock(mReceiptID.ToString());
                }
                return BadRequest(msg);

            }

        }

        [HttpPut("UpdateHeaderInfo")] //MariaDB
        public IActionResult UpdateHeaderInfo(Int64 usrFacilityID, Int64 receiptID, String receiptNo, String receiptDate)
        {
            master msterObj = new master(_context);

            string mSourceID = "716";
            string mFacilityID = usrFacilityID.ToString();
            string mReceiptID = receiptID.ToString();
            string mReceiptNo = receiptNo.ToUpper();
            if (mReceiptNo == "AG")

                mReceiptNo = msterObj.FacAutoGenerateNumbers(usrFacilityID.ToString(), true, "FC");  //string ID, bool IsWarehouse, bool IsReceipt, string mType
            string mReceiptDate = receiptDate.Trim();



            //#region Validate Data
            //string ErrMsg = "";
            //ErrMsg += GenFunctions.CheckDateGreaterThanToday(mReceiptDate, "Receipt date");
            //ErrMsg += GenFunctions.CheckDuplicate(mReceiptNo, "Receipt number", "tbfacilityreceipts", "FACReceiptNo", mReceiptID, "FACReceiptID", " and FacilityID = " + mFacilityID);
            //ErrMsg += GenFunctions.CheckStringForEmpty(ref mReceiptNo, "Receipt number", false);
            //ErrMsg += GenFunctions.CheckStringForEmpty(ref mSourceID, "Source", false);
            //ErrMsg += GenFunctions.CheckDate(ref mReceiptDate, "Receipt date", false);
            //if (ErrMsg != "")
            //{
            //    GenFunctions.AlertOnLoad(this.Page, ErrMsg);
            //    lblMsg.Text = ErrMsg;
            //    lblMsg.ForeColor = Color.Red;
            //    return;
            //}
            //else
            //    lblMsg.Text = "";
            //#endregion


            if (mReceiptNo == "")
            {
                return BadRequest();

            }


                if (mReceiptID == "" || mReceiptID == "0")
            {
                String ReceiptID = "";
                if (msterObj.checkAlreadyFC(usrFacilityID))
                {
                    //string strSQL = "Insert into tbFacilityReceipts (FacilityID,FacReceiptNo,FacReceiptDate,FACReceiptType)" +
                    //    " values (" + mFacilityID + ",'" +
                    //    mReceiptNo + "',TO_DATE('" + mReceiptDate + "', 'DD-MM-YYYY'),'FC')";

                    string strSQL = "INSERT INTO tbFacilityReceipts (FacilityID, FacReceiptNo, FacReceiptDate, FACReceiptType,Status) " +
                "VALUES (" + mFacilityID + ", '" + mReceiptNo + "', STR_TO_DATE('" + mReceiptDate + "', '%d-%m-%Y'), 'FC','I')";


                    _context.Database.ExecuteSqlRaw(strSQL);


                    //strSQL = "Select FACRECEIPTID from tbFacilityReceipts where FACReceiptType='FC'" +
                    //    " and FacReceiptNo='" + mReceiptNo + "' and FacReceiptDate=TO_DATE('" + mReceiptDate + @"', 'DD-MM-YYYY')";

                     strSQL = "SELECT FACRECEIPTID FROM tbFacilityReceipts " +
                "WHERE FACReceiptType = 'FC' " +
                "AND FacReceiptNo = '" + mReceiptNo + "' " +
                "AND FacReceiptDate = STR_TO_DATE('" + mReceiptDate + "', '%d-%m-%Y')";


                    var myList = _context.GetFacilityReceiptIdDbSet
                        .FromSqlInterpolated(FormattableStringFactory.Create(strSQL)).ToList();

                    // DataTable dtID = DBHelper.GetDataTable(strSQL);

                    if (myList.Count > 0)
                        ReceiptID = myList[0].FACRECEIPTID.ToString();
                    return Ok(ReceiptID);
                    //lblReceiptID.Text = dtID.Rows[0]["FacReceiptID"].ToString();
                    //lblMsg.Text = "Updated Successfully";
                    //lblMsg.ForeColor = Color.Green;
                    // PopulateHeaderInfo(RC4Engine.Decrypter(ReceiptID.Value, EncryptionKey));
                }
                else
                {
                    //string strSQL = "Update tbFacilityReceipts set FacReceiptNo=" + mReceiptNo + ",FacReceiptDate=" + mReceiptDate + " " +
                    //    " Where FacReceiptID = " + mReceiptID;
                    //_context.Database.ExecuteSqlRaw(strSQL);
                    return Ok("Already Generated Opening Stock No");
                    //lblMsg.Text = "Updated Successfully";
                    //lblMsg.ForeColor = Color.Green;
                    //PopulateHeaderInfo(RC4Engine.Decrypter(ReceiptID.Value, EncryptionKey));
                }
            }
           

            return BadRequest();

        }

        [HttpGet("GetFacreceiptid")]
        public async Task<ActionResult<IEnumerable<GetFacreceiptidForOpeningStockDTO>>> GetFacreceiptid(String usrFacilityID)
        {

            string qry = @" select facreceiptid from tbfacilityreceipts where facilityid=" + usrFacilityID + " and  FACreceipttype='FC'  ";

            var myList = _context.GetFacreceiptidForOpeningStockDbSet
            .FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList();
            return myList;
        }

        [HttpGet("GetHeaderInfo")]
        public async Task<ActionResult<IEnumerable<GetHeaderInfoDTO>>> GetHeaderInfo(String ReceiptID)
        {

            string qry = @" Select  facreceiptid,sourceid,schemeid,facilityid,warehouseid,indentid,issueid,facreceiptno,facreceiptdate,facreceipttype,facreceiptvalue,remarks,
    status,wardid,supplierid,reasonflag,isedl,tofacilityid,ponoid,stkregno,stkregdate,mrcnumber,mrcdate,sptype,splocationid,invoiceno,invoicedate,voucherstatus,isnicshare,compentrydt,
    receiptby,recbydate,receivedby,recbymobileno,entrydate,isuseapp from tbfacilityreceipts where FACReceiptID = " + ReceiptID;


            var myList = _context.GetHeaderInfoDbSet
            .FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList();
            return myList;
        }

        [HttpDelete("OpeningStockRowDelete")]
        public IActionResult OpeningStockRowDelete(Int64 mReceiptID, Int64 mReceiptItemidID, String minwno)
        {
            FacOperations facOperations = new FacOperations(_context);
            try
            {

                if (facOperations.GetBatchQTY(minwno, mReceiptItemidID, mReceiptID))
                {
                    return Ok("Deleted Successfully");

                    //lblMsg.Text = "Deleted Successfully";
                    //lblMsg.ForeColor = Color.Green;
                    //populategrid("");
                    //var message = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize("Deleted Successfully");
                    //var script = string.Format("alert({0});", message);
                    //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "", script, true);
                    //return;
                }
                else
                {
                    return Ok("Not Deleted,Contact CGMSC-IT Team");
                    //lblMsg.Text = "Not Deleted,Contact CGMSC-IT Team";
                    //lblMsg.ForeColor = Color.Green;
                    //populategrid("");
                    //var message = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize("Not Deleted,Contact CGMSC-IT Team");
                    //var script = string.Format("alert({0});", message);
                    //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "", script, true);
                    //return;
                }
            }
            catch
            {
                return Ok("Please delete Batches first.");

                //string ErrMsg = "Please delete Batches first.";
                //GenFunctions.AlertOnLoad(this.Page, ErrMsg);
                //lblMsg.Text = ErrMsg;
                //lblMsg.ForeColor = Color.Red;
                //lblMsg.Text = "Delete not allowed. References found.";
                //lblMsg.ForeColor = Color.Red;
            }
            //finally
            //{

            //}

        }

        [HttpGet("getFacilityOpenings")]
        public async Task<ActionResult<IEnumerable<getFacilityOpeningsDTO>>> getFacilityOpenings(string facId)
        {

            string qry = @"  select FACreceiptid receiptid,FACreceiptno receiptno,FACreceiptdate receiptdate,facilityname,status from tbfacilityreceipts a, masfacilities b where a.facilityid=b.facilityid and a.facilityid is not null  and FACreceipttype='FC'   and a.facilityid="+ facId + " ";
            var myList = _context.getFacilityOpeningsDbSet
            .FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList();
            return myList;
        }

        [HttpGet("ConsumptionReport")] //mariadb
        public async Task<ActionResult<IEnumerable<GetConsumptionDTO>>> ConsumptionReport(string stktype,Int64 facId, Int64 itemId, String fromDate, String todate)

        {

            string whitemid = "";
            if (itemId != 0)
            {
                 whitemid = " and fsi.itemid="+ itemId;
            }


                string qry = "";
            if (stktype == "I")
            {
                qry = @"  select   row_number() over (order by fsi.itemid) as id, fsi.itemid,vm.itemcode,vm.itemname,vm.strength1,sum(nvl(ftbo.issueqty,0)) issueqty ,fs.ISSUEDATE  ,fs.WardID,b.WardName
  ,0 as inwno,0 as batchno,'' as mfgdate,'' as expdate
    from tbfacilityissues fs 
 Inner Join masFacilityWards b on (b.WardID=fs.WardID)
  inner join tbfacilityissueitems fsi on fsi.issueid=fs.issueid 
  inner join masitems vm on vm.itemid=fsi.itemid
  inner join tbfacilityoutwards ftbo on ftbo.issueitemid=fsi.issueitemid 
  where fs.status = 'C' "+ whitemid + @"  and fs.facilityid="+ facId + @"  and fs.ISSUETYPE='NO' and fs.ISSUEDATE between   '"+ fromDate + @"' and    '"+ todate + @"'  
  group by fsi.itemid,fs.ISSUEDATE,vm.itemcode,vm.itemname,vm.strength1,fs.WardID,b.WardName
  order by fs.ISSUEDATE ";
            }
            else
            {
                qry = @" select row_number() over(order by fsi.itemid) as id, fsi.itemid,vm.itemcode,vm.itemname,vm.strength1,
    sum(nvl(ftbo.issueqty, 0)) issueqty ,fs.ISSUEDATE  ,fs.WardID,b.WardName
    ,ftbo.inwno,rb.batchno,rb.mfgdate,rb.expdate
    from tbfacilityissues fs
 Inner Join masFacilityWards b on(b.WardID = fs.WardID)
  inner join tbfacilityissueitems fsi on fsi.issueid = fs.issueid
  inner join masitems vm on vm.itemid = fsi.itemid
  inner join tbfacilityoutwards ftbo on ftbo.issueitemid = fsi.issueitemid
   inner join tbfacilityreceiptbatches rb on rb.inwno = ftbo.inwno
  where fs.status = 'C'  "+ whitemid + @" and fs.facilityid = " + facId + @"  and fs.ISSUETYPE = 'NO' and fs.ISSUEDATE between   '"+ fromDate + @"' and    '"+ todate + @"' 
  group by fsi.itemid,fs.ISSUEDATE,vm.itemcode,vm.itemname,vm.strength1,fs.WardID,b.WardName,ftbo.inwno,rb.batchno,rb.mfgdate,rb.expdate
  order by fs.ISSUEDATE,vm.itemname";


            }
            var myList = _context.GetConsumptionDbSet
           .FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList();
            return myList;
        }



            [HttpGet("GetOpningClosingItem")]
        public async Task<ActionResult<IEnumerable<GetOpningClosingItemDTO>>> GetOpningClosingItem(Int64 facId,Int64 itemId, String fromDate)
        {

            string qry = @"  
 select
 dt,Type1,Type,TranDate,QTY,TranType,Place,TRANNO,receiptitemid
 from 
 (
    select  STR_TO_DATE('" + fromDate + @"', '%d-%m-%Y') AS dt ,0 as Type1,'OP' as Type,to_char('" + fromDate + @"') as  TranDate,sum((case when b.qastatus = '1' then (nvl(b.absrqty,0) - nvl(iq.issueqty,0)) end)) QTY,'OP' as TranType,'Opening Stock' as Place,'Opening Balance' TRANNO,0 as receiptitemid
   from tbfacilityreceiptbatches b
   inner join tbfacilityreceiptitems i on b.facreceiptitemid=i.facreceiptitemid
   inner join tbfacilityreceipts t on t.facreceiptid= i.facreceiptid
   inner join masitems mi on mi.itemid= i.itemid
   left outer join
   (
     select fs.facilityid, fsi.itemid, ftbo.inwno, sum(nvl(ftbo.issueqty,0)) issueqty
       from tbfacilityissues fs
     inner join tbfacilityissueitems fsi on fsi.issueid=fs.issueid
     inner join tbfacilityoutwards ftbo on ftbo.issueitemid= fsi.issueitemid
     where fs.status = 'C'  and fs.facilityid= " + facId + @"  and fs.issuedate<'"+ fromDate + @"'
     and fsi.itemid="+ itemId + @"
     group by fsi.itemid, fs.facilityid, ftbo.inwno
   ) iq on b.inwno = Iq.inwno and iq.itemid=i.itemid and iq.facilityid=t.facilityid
   Where  T.Status = 'C'  And(b.Whissueblock = 0 or b.Whissueblock is null) and b.expdate>curdate()
  and t.facilityid= " + facId + @" and i.itemid="+ itemId + @" and t.FACRECEIPTDATE<'"+ fromDate + @"'
  having (sum((case when b.qastatus = '1' then (nvl(b.absrqty,0) - nvl(iq.issueqty,0)) end)))>0
 union all
 
 select fs.issuedate dt,2 as Type1, 'Issue' as Type,to_char(fs.issuedate,'dd-MM-yyyy') as TranDate,sum(nvl(ftbo.issueqty,0)) QTY,fs.issuetype as TranType,case when wr.wardname is null then f2.facilityname else wr.wardname end as Place,fs.ISSUENO as TRANNO,0 as receiptitemid
   from tbfacilityissues fs
left outer join  masfacilitywards wr on wr.wardid=fs.wardid
left outer join masfacilities f2 on f2.facilityid=fs.tofacilityid
 inner join tbfacilityissueitems fsi on fsi.issueid=fs.issueid
 inner join tbfacilityoutwards ftbo on ftbo.issueitemid= fsi.issueitemid
 where fs.status = 'C'  and fs.facilityid=" + facId + @" and fsi.itemid="+ itemId + @"
 and fs.issuedate>='"+ fromDate + @"'
 group by fs.issuedate,fs.issuetype,wr.wardname,f2.facilityname,fs.ISSUENO
 
 union all   


 select  t.FACRECEIPTDATE as dt,1 as Type1 ,Case when t.FACRECEIPTTYPE = 'FC' then 'Opening Stock' else 'Receipt' end as Type,to_char(t.FACRECEIPTDATE,'dd-MM-yyyy') as TranDate, sum((case when b.qastatus = '1' then (nvl(b.absrqty,0)) else 0 end)) QTY,t.FACRECEIPTTYPE as  TranType,
    Case when t.FACRECEIPTTYPE = 'FC' then 'Opening' else  case when  f2.facilityname is  null then 'WH:'||w.warehousename else f2.facilityname end end as  Place, t.FACRECEIPTNO AS TRANNO,i.facreceiptitemid as receiptitemid 
   from tbfacilityreceiptbatches b
   inner join tbfacilityreceiptitems i on b.facreceiptitemid=i.facreceiptitemid
      inner join masitems mi on mi.itemid= i.itemid
   inner join tbfacilityreceipts t on t.facreceiptid= i.facreceiptid
   left outer join maswarehouses w on w.warehouseid=t.warehouseid
left outer join masfacilities f2 on f2.facilityid=t.TOFACILITYID
     Where  T.Status = 'C'  And(b.Whissueblock = 0 or b.Whissueblock is null) and b.expdate>curdate()
   and t.facilityid= " + facId + @" and i.itemid="+ itemId + @" and t.FACRECEIPTDATE>='"+ fromDate + @"' 
 group by t.FACRECEIPTDATE,t.FACRECEIPTTYPE,w.warehousename,f2.facilityname,t.FACRECEIPTNO,i.facreceiptitemid ,t.status


 ) ab
order by dt,Type1
 
 

 
  ";

            var myList = _context.GetOpningClosingItemDbSet
            .FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList();
            return myList;
        }


        [HttpGet("GetReceiptItems")]
        public async Task<ActionResult<IEnumerable<GetReceiptItemsDTO>>> GetReceiptItems(string facId)
        {

            string qry = @"  select itemid ,name from
 (
select distinct  mi.itemid, CONCAT(mi.itemcode,' ', mi.itemname,' ', mi.strength1)   as name
 from tbfacilityreceiptitems i 
      inner join masitems mi on mi.itemid= i.itemid
      inner join tbfacilityreceipts t on t.facreceiptid= i.facreceiptid
      Where  T.Status = 'C'  
      and t.facilityid= " + facId + @" 
  ) a
  order by name ";
            var myList = _context.GetReceiptItemsDbSet
            .FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList();
            return myList;
        }

        [HttpGet("GetBatchesOfReceipt")]
        public async Task<ActionResult<IEnumerable<GetBatchesOfReceiptDTO>>> GetBatchesOfReceipt(string facReceiptItemId)
        {

            string qry = @" select mi.itemid,mi.itemname||mi.strength1 as name,i.facreceiptitemid,b.batchno,sum(b.ABSRQTY) batchqty,b.mfgdate,b.expdate
   from tbfacilityreceiptbatches b
   inner join tbfacilityreceiptitems i on b.facreceiptitemid=i.facreceiptitemid
      inner join vmasitems mi on mi.itemid= i.itemid
   inner join tbfacilityreceipts t on t.facreceiptid= i.facreceiptid
     Where  T.Status = 'C' 
  and i.facreceiptitemid= "+ facReceiptItemId + @" 
 group by mi.itemid,mi.itemname,mi.strength1,i.facreceiptitemid,b.batchno,b.mfgdate,b.expdate ";
            var myList = _context.GetBatchesOfReceiptDbSt
            .FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList();
            return myList;
        }


        [HttpGet("GetWHStockAAM")]
        public async Task<ActionResult<IEnumerable<GetWHStockItemsDTO>>> GetWHStockAAM(string facid)
        {
            FacOperations f = new FacOperations(_context);
            Int64 whid = f.getWHID(facid);
            string qry = @" select  m.itemid, m.itemcode,ty.itemtypename,m.itemname,m.strength1, m.multiple, m.unitcount,nvl(ReadyForIssue,0)*nvl(m.unitcount,1) as ReadySTK,nvl(Pending,0)*nvl(m.unitcount,1) as UqcSTK  from masitems m 
inner join masitemcategories c on c.categoryid = m.categoryid
inner join masitemmaincategory mc on mc.MCID = c.MCID 
left outer join masitemtypes ty  on ty.itemtypeid = m.itemtypeid
left outer join 
(

 select A.itemid,
 (case when sum( A.ReadyForIssue)>0 then sum( A.ReadyForIssue) else 0 end) as ReadyForIssue,(case when sum(nvl(Pending,0)) >0 then sum(nvl(Pending,0)) else 0 end) Pending 
                  from 
                 (  
                 select mi.itemid,  (case when b.qastatus ='1' then (nvl(b.absrqty,0) - nvl(iq.issueqty,0)) else (case when mi.Qctest ='N' then (nvl(b.absrqty,0) - nvl(iq.issueqty,0) )  end ) end ) ReadyForIssue,  
                   case when  mi.qctest='N' then 0 else (case when b.qastatus = 0 or b.qastatus = 3 then (nvl(b.absrqty,0)- nvl(iq.issueqty,0)) end) end  Pending  
             
                  from tbreceiptbatches b  
                  inner join tbreceiptitems i on b.receiptitemid=i.receiptitemid 
                  inner join tbreceipts t on t.receiptid=i.receiptid 
                  inner join masitems mi on mi.itemid=i.itemid 
                 inner join MASWAREHOUSES w  on w.warehouseid=t.warehouseid  
                 left outer join 
                  (  
                   select  tb.warehouseid,tbi.itemid,tbo.inwno,sum(nvl(tbo.issueqty,0)) issueqty   
                   from tboutwards tbo, tbindentitems tbi , tbindents tb 
                   where  tbo.indentitemid=tbi.indentitemid and tbi.indentid=tb.indentid and tb.status = 'C' and tb.notindpdmis is null and tbo.notindpdmis is null and tbi.notindpdmis is null 
                   group by tbi.itemid,tb.warehouseid,tbo.inwno  
                 ) iq on b.inwno = Iq.inwno and iq.itemid=i.itemid and iq.warehouseid=t.warehouseid 
                 Where  1=1 and w.warehouseid= "+ whid + @" and T.Status = 'C' and (b.ExpDate >= SysDate or nvl(b.ExpDate,SysDate) >= SysDate) And (b.Whissueblock = 0 or b.Whissueblock is null)  
                 and t.notindpdmis is null and b.notindpdmis is null  and i.notindpdmis is null 
                 ) A group by A.itemid 
) st on st.itemid=m.itemid
where m.shc = 'Y'  and m.ISFREEZ_ITPR is NULL
order by ty.itemtypename, m.itemname";
            var myList = _context.getGetWHStockItemsDbSet
            .FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList();
            return myList;
        }



        [HttpGet("getNearExpStock")]
        public async Task<ActionResult<IEnumerable<NearExpBatchDTO>>> getNearExpBatchStock(string faclityId, string criteria)
        {
            //string faclityId1 = "23378";
            //string catidID1 = "52";
            //string criteria1 = "60";
            string whclause = ""; string whcatid = "";
            if (criteria != "0")
            {
                whclause = " and Round(b.EXPDATE-curdate())<=" + criteria;
            }


            string qry = @" select mi.ITEMCODE,mi.itemname as ITEMNAME,mi.strength1,b.BATCHNO,b.EXPDATE, (case when b.qastatus ='1' then (nvl(b.absrqty,0) - nvl(iq.issueqty,0)) end)  as FACSTOCK
   ,case when Round(b.EXPDATE-curdate())<30 then 'Under 1 Month'
   else case when Round(b.EXPDATE-curdate())>30 and Round(b.EXPDATE-curdate())<=60 then 'Under 2 Month'
   else case when Round(b.EXPDATE-curdate())>60 and Round(b.EXPDATE-curdate())<=90 then 'Under 3 Month'
    else case when Round(b.EXPDATE-curdate())>90 and Round(b.EXPDATE-curdate())<=120 then 'Under 4 Month'
        else case when Round(b.EXPDATE-curdate())>120 and Round(b.EXPDATE-curdate())<=150 then 'Under 5 Month'
        else 'Under 6 Month' 
        end end end end end as EXPTIMELINE,b.inwno as INWNO
                 from tbfacilityreceiptbatches b   
                 inner join tbfacilityreceiptitems i on b.facreceiptitemid=i.facreceiptitemid 
                 inner join tbfacilityreceipts t on t.facreceiptid=i.facreceiptid  
                 inner join masitems mi on mi.itemid=i.itemid 
                 inner join masfacilities f  on f.facilityid=t.facilityid 
                 left outer join 
                 (  
                   select  fs.facilityid,fsi.itemid,ftbo.inwno,sum(nvl(ftbo.issueqty,0)) issueqty   
                     from tbfacilityissues fs 
                   inner join tbfacilityissueitems fsi on fsi.issueid=fs.issueid 
                   inner join tbfacilityoutwards ftbo on ftbo.issueitemid=fsi.issueitemid 
                   where fs.status = 'C'  and fs.facilityid= " + faclityId + @"         
                   group by fsi.itemid,fs.facilityid,ftbo.inwno                     
                 ) iq on b.inwno = Iq.inwno and iq.itemid=i.itemid and iq.facilityid=t.facilityid                 
                 Where  T.Status = 'C'  And (b.Whissueblock = 0 or b.Whissueblock is null) and b.expdate>curdate()
                and f.facilityid= " + faclityId + @" 
                and (case when b.qastatus ='1' then (nvl(b.absrqty,0) - nvl(iq.issueqty,0)) end)>0
                and Round(b.EXPDATE-curdate())<180
                " + whclause + @"
  order by b.EXPDATE ";

            var context = new NearExpBatchDTO();

            var myList = _context.NearExpBatchDTODBSet
            .FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList();

            return myList;
        }

        [HttpGet("stockReportParent")]
        public async Task<ActionResult<IEnumerable<StockReportFacilityDTO>>> stockReportParent(string faclityId, string itemid, string catname)
        {
            FacOperations f = new FacOperations(_context);
            Int64 phcId = f.getPHCid(faclityId);
            string whcatid = "";
            if (catname == "D")
            {
                if (phcId == 7)
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
                 from tbfacilityreceiptbatches b   
                 inner join tbfacilityreceiptitems i on b.facreceiptitemid=i.facreceiptitemid 
                 inner join tbfacilityreceipts t on t.facreceiptid=i.facreceiptid  
                 inner join masitems mi on mi.itemid=i.itemid and mi.shc = 'Y' 
                 left outer join masitemcategories c on c.categoryid=mi.categoryid
                 left outer join masitemtypes ty on ty.itemtypeid=mi.itemtypeid
                 inner join masfacilities f  on f.facilityid=t.facilityid 
                 left outer join 
                 (  
                   select  fs.facilityid,fsi.itemid,ftbo.inwno,sum(nvl(ftbo.issueqty,0)) issueqty   
                     from tbfacilityissues fs 
                   inner join tbfacilityissueitems fsi on fsi.issueid=fs.issueid 
           inner join masitems mi on mi.itemid=fsi.itemid and mi.shc = 'Y' 
                   inner join tbfacilityoutwards ftbo on ftbo.issueitemid=fsi.issueitemid 
                   where fs.status = 'C'  and fs.facilityid=" + phcId + @"          
                   group by fsi.itemid,fs.facilityid,ftbo.inwno                     
                 ) iq on b.inwno = Iq.inwno and iq.itemid=i.itemid and iq.facilityid=t.facilityid                 
                 Where 1=1 " + whclause + @" and  T.Status = 'C'  And (b.Whissueblock = 0 or b.Whissueblock is null) and b.expdate>sysdate 
                and f.facilityid= " + phcId + @"  " + whcatid + @"
                and (   (case when (b.qastatus ='1' or  mi.qctest='N') then (nvl(b.absrqty,0) - nvl(iq.issueqty,0)) end)) > 0
                group by  mi.ITEMCODE, t.facilityid, mi.itemid,b.qastatus,mi.qctest,mi.itemname,mi.strength1,c.categoryname,c.categoryid,itemtypename,mi.ISEDL2021
                order by c.categoryid, mi.itemname ";

            var context = new StockReportFacilityDTO();

            var myList = _context.StockReportFacilityDTOs
            .FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList();

            return myList;
        }



        [HttpGet("getSHCItems")]
        public async Task<ActionResult<IEnumerable<getSHCItemsDTO>>> getSHCItems()
        {

            //string qry = @"   select ITEMID, (ITEMCODE || '-' || ITEMNAME || '-' || STRENGTH1) as ITEMCODE  from masitems where SHC = 'Y'  ";

            string qry = @"  SELECT 
    ITEMID, 
    CONCAT(ITEMCODE, '-', ITEMNAME, '-', STRENGTH1) AS ITEMCODE 
FROM 
    masitems 
WHERE 
    SHC = 'Y'; ";

            var myList = _context.getSHCItemsDbSet
            .FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList();
            return myList;
        }



        [HttpGet("getDistrictAchivementStatus")] //mariadb
        public async Task<ActionResult<IEnumerable<getDistrictAchivementStatusDTO>>> getDistrictAchivementStatus()
        {

            //            string qry = @"  select districtname,count(FACILITYID) as target,sum(target)  as achivement,round(sum(target)/count(FACILITYID)*100,2) as per
            //,districtid
            //from 
            //(
            //select nvl(nosindent,0) as Nosindent,case when lastissueDt is null then 0 else 1 end as Consumptiondoing
            //,case when r.FACRECEIPTID is null then 0 else 1 end as receipt,
            //d.districtname,f.FACILITYID,d.districtid,u.userid
            //,case when (nvl(nosindent,0)+(case when lastissueDt is null then 0 else 1 end)+(case when r.FACRECEIPTID is null then 0 else 1 end))>1 then 1 else 0 end as target
            //from masfacilities f
            //                            inner join usrusers u on u.FACILITYID = f.facilityid
            //                            inner join masfacilitytypes ft on ft.FACILITYTYPEID = f.FACILITYTYPEID
            //                            inner join masdistricts d on d.districtid = f.districtid
            //                            left outer join maslocations l on l.LOCATIONID=f.LOCATIONID
            //                            left outer join masfacilities p on p.facilityid=f.PHC_ID
            //                            left outer join 
            //                            (
            //                            select facilityid,count(nocid) as nosindent from mascgmscnoc faci
            //                            where faci.status='C' group by facilityid
            //                            ) ind on ind.facilityid=f.facilityid


            //                            left outer join 
            //                            (
            //                                 select  max(tb.issueddate) as lastissueDt,tb.facilityid       from tbfacilityissues tb
            //                                where tb.status='C' 
            //                                group by tb.facilityid  
            //                            ) iss on iss.facilityid=f.facilityid



            //                            left outer join 
            //                            (
            //                            select max(FACRECEIPTID) FACRECEIPTID,tb.facilityid from tbFacilityReceipts tb where STATUS ='C'
            //                         group by tb.facilityid
            //                            )r on  r.facilityid=f.facilityid

            //                            where 1=1 and f.is_aam = 'Y'
            //                            ) group by districtname,districtid
            //                            order by districtname ";

            string qry = @" SELECT 
    districtname,
    COUNT(FACILITYID) AS target,
    SUM(target) AS achivement,
    ROUND(SUM(target) / COUNT(FACILITYID) * 100, 2) AS per,
    districtid
FROM 
(
    SELECT 
        IFNULL(nosindent, 0) AS Nosindent,
        CASE WHEN lastissueDt IS NULL THEN 0 ELSE 1 END AS Consumptiondoing,
        CASE WHEN r.FACRECEIPTID IS NULL THEN 0 ELSE 1 END AS receipt,
        d.districtname,
        f.FACILITYID,
        d.districtid,
        u.userid,
        CASE 
            WHEN (IFNULL(nosindent, 0) 
                + CASE WHEN lastissueDt IS NULL THEN 0 ELSE 1 END 
                + CASE WHEN r.FACRECEIPTID IS NULL THEN 0 ELSE 1 END) > 1 
            THEN 1 ELSE 0 
        END AS target
    FROM 
        masfacilities f
    INNER JOIN usrusers u ON u.FACILITYID = f.facilityid
    INNER JOIN masfacilitytypes ft ON ft.FACILITYTYPEID = f.FACILITYTYPEID
    INNER JOIN masdistricts d ON d.districtid = f.districtid
    LEFT OUTER JOIN maslocations l ON l.LOCATIONID = f.LOCATIONID
    LEFT OUTER JOIN masfacilities p ON p.facilityid = f.PHC_ID
    LEFT OUTER JOIN 
    (
        SELECT 
            facilityid,
            COUNT(nocid) AS nosindent
        FROM 
            mascgmscnoc
        WHERE 
            status = 'C' 
        GROUP BY 
            facilityid
    ) ind ON ind.facilityid = f.facilityid
    LEFT OUTER JOIN 
    (
        SELECT  
            MAX(tb.issueddate) AS lastissueDt,
            tb.facilityid       
        FROM 
            tbfacilityissues tb
        WHERE 
            tb.status = 'C' 
        GROUP BY 
            tb.facilityid  
    ) iss ON iss.facilityid = f.facilityid
    LEFT OUTER JOIN 
    (
        SELECT 
            MAX(FACRECEIPTID) AS FACRECEIPTID,
            tb.facilityid 
        FROM 
            tbFacilityReceipts tb 
        WHERE 
            STATUS = 'C'
        GROUP BY 
            tb.facilityid
    ) r ON r.facilityid = f.facilityid
    WHERE 
        f.is_aam = 'Y'
) AS subquery
GROUP BY 
    districtname, districtid
ORDER BY 
    districtname;
 ";


            var myList = _context.getDistrictAchivementStatusDbSet
            .FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList();
            return myList;
        }


        [HttpGet("getItemCategory")] //MariaDB
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> getCategory(string faclityId)
        {
            string qry = @" select  CATEGORYNAME,CATEGORYID from masitemcategories
where categoryid in (52,54,58,59,60,61,62)
order by categoryid ";

            var context = new CategoryDTO();

            var myList = _context.CategoryDTODbSet
            .FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList();

            return myList;
        }


        [HttpGet("stockReport")]
        public async Task<ActionResult<IEnumerable<StockReportFacilityDTO>>> stockReport(string faclityId, string itemid, string catname)
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
                 Where 1=1 " + whclause + @" and  T.Status = 'C'  And (b.Whissueblock = 0 or b.Whissueblock is null) and b.expdate>curdate() 
                and t.facilityid= " + faclityId + @"  " + whcatid + @"
                and (   (case when (b.qastatus ='1' or  mi.qctest='N') then (nvl(b.absrqty,0) - nvl(iq.issueqty,0)) end)) > 0
                group by  mi.ITEMCODE, t.facilityid, mi.itemid,b.qastatus,mi.qctest,mi.itemname,mi.strength1,c.categoryname,c.categoryid,itemtypename,mi.ISEDL2021
                order by c.categoryid, mi.itemname ";

            var context = new StockReportFacilityDTO();

            var myList = _context.StockReportFacilityDTOs
            .FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList();

            return myList;
        }



        //[HttpGet("getTestData")]
        //public ActionResult<object> getTestData()
        //{
        //    var testData = new { City = "Tilda" };
        //    return Ok(testData);
        //}





    }
}
