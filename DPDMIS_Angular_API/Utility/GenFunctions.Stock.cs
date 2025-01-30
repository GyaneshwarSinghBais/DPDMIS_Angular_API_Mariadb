#region Using
using System;
using System.Data;
using System.Configuration;
using System.Globalization;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using DBHelper = Broadline.WMS.Data.DBHelper;
#endregion

/// <summary>
/// Summary description for GenFunctions
/// </summary>
public partial class GenFunctions
{
    #region Stock
    public class Stock
    {
        #region GetCurStock
        public static double GetCurStock(string mWarehouseID, string mSourceID, string mSchemeID, string mItemID)
        {
            string strSQL = "Select nvl(Sum(NVL(b.CurBal, 0) - NVL(b.Reserved, 0)),0) as CurBal" +
               " from V_CURSTOCKAVAILABLE b" +
               " Where b.WarehouseID=" + mWarehouseID +
                //"   and b.SourceID=" + mSourceID +
                //"   and b.SchemeID=" + mSchemeID +
               "   and b.ItemID = " + mItemID;
            DataTable dt = DBHelper.GetDataTable(strSQL);
            if (dt.Rows.Count > 0 && dt.Rows[0]["CurBal"] != DBNull.Value)
                return double.Parse(dt.Rows[0]["CurBal"].ToString());
            else
                return 0;
        }
        #endregion

        #region GetFacilityCurStock - tbReceipts
        public static double GetFacilityCurStock(string mFacilityID, string mSourceID, string mSchemeID, string mItemID)
        {
            string WhereCondition = "";

            WhereCondition += (mSourceID != "0")? " and b.SourceID=" + mSourceID : "";
            WhereCondition += (mSchemeID != "0")? " and b.SchemeID=" + mSchemeID : "";

            string strSQL = "Select nvl(Sum(NVL(b.CurBal, 0) - NVL(b.Reserved, 0)),0) as CurBal" +
                " from V_CURSTOCKFACILITYAVAILABLE b" +
                " Where b.FacilityID=" + mFacilityID +
                " and b.ItemID = " + mItemID + WhereCondition;

            DataTable dt = DBHelper.GetDataTable(strSQL);
            if (dt.Rows.Count > 0 && dt.Rows[0]["CurBal"] != DBNull.Value)
                return double.Parse(dt.Rows[0]["CurBal"].ToString());
            else
                return 0;
        }
        #endregion

        #region GetFacilityCurStock - tbFacilityReceipts
        public static double GetFacilityCurStock(string mFacilityID, string mItemID)
        {
            string strSQL = " select T.FacilityID,T.ItemID,Sum(Nvl(T.WAREHOUSE_RECEIPTS,0) + Nvl(T.WARD_RETURNS,0) - Nvl(T.WARD_ISSUES,0) - Nvl(T.WAREHOUSE_RETURNS,0) - Nvl(T.EXPIRED,0) - Nvl(T.SHORTAGES,0) - Nvl(T.DAMAGES,0)) as CurBal " +
                            " from v_facilitytransummarybyitems T " +
                            " Inner Join masFacilityWH M on (M.FacilityID = T.FacilityID) " +
                            " Where T.FacilityID=" + mFacilityID +
                            " and T.ItemID = " + mItemID +
                            " GROUP BY T.FacilityID, T.ItemID";

            DataTable dt = DBHelper.GetDataTable(strSQL);
            if (dt.Rows.Count > 0 && dt.Rows[0]["CurBal"] != DBNull.Value)
                return double.Parse(dt.Rows[0]["CurBal"].ToString());
            else
                return 0;
        }
        #endregion

        #region UpdateAllotQty
        public static void UpdateAllotQty(string WarehouseID, string SourceID, string SchemeID, string ItemID, double AllotQty, string IndentItemID)
        {
            //Allow to distribute to facility only when QAStatus=pass from batches (tbReceiptBatches)
            string WhereConditions = " and b.QAStatus = 1 and (b.ExpDate >= SysDate or nvl(b.ExpDate,SysDate) >= SysDate)";

            #region Delete existing allotements (tbOutwards)
            string strSQL = "Delete from tbOutWards where IndentItemID = " + IndentItemID;
            DBHelper.ExecuteNonQuery(strSQL);
            #endregion

            #region Insert new allotements (tbOutwards)
            double TotAllotQty = AllotQty;
            strSQL = "Select b.InwNo, b.BatchNo, b.MfgDate, b.ExpDate, NVL(b.AbsRQty, 0) as AbsRQty, NVL(b.AllotQty, 0) as AllotQty" +
                " from tbReceiptBatches b" +
                " Inner Join tbReceiptItems a on (a.ReceiptItemID = b.ReceiptItemID)" +
                " Inner Join tbReceipts a1 on (a1.ReceiptID = a.ReceiptID)" +
                " Where a1.WarehouseID = " + WarehouseID + " and a1.SourceID = " + SourceID + " and a1.SchemeID = " + SchemeID +
                "   and a.ItemID = " + ItemID + WhereConditions + " and NVL(b.AbsRQty, 0) - NVL(b.AllotQty, 0) > 0 and b.BatchStatus='R'" +
                " Order By b.ExpDate, b.AbsRQty";
            DataTable dt = DBHelper.GetDataTable(strSQL);

            foreach (DataRow dr in dt.Rows)
            {
                #region Calculate Allotted Qty
                double dblCurAllot = 0;
                double dblAbsRQty = double.Parse(dr["AbsRQty"].ToString());
                double dblAllotQty = double.Parse(dr["AllotQty"].ToString());
                if (dblAbsRQty - dblAllotQty >= TotAllotQty)
                    dblCurAllot = TotAllotQty;
                else
                    dblCurAllot = dblAbsRQty - dblAllotQty;
                TotAllotQty -= dblCurAllot;
                #endregion

                #region Data to be Saved
                string mIndentItemID = IndentItemID;
                string mItemID = ItemID;
                string mInwNo = dr["InwNo"].ToString();
                string mIssueQty = dblCurAllot.ToString();
                string mIssuePrice = "0";
                string mIssued = "0";
                #endregion

                #region Validate Data
                GenFunctions.CheckNumber(ref mIndentItemID, "", false);
                GenFunctions.CheckNumber(ref mItemID, "", false);
                GenFunctions.CheckNumber(ref mInwNo, "", false);
                //GenFunctions.CheckStringForEmpty(ref mIssueType, "", false);
                //GenFunctions.CheckDate(ref mIssueDate, "", false);
                GenFunctions.CheckNumber(ref mIssueQty, "", false);
                GenFunctions.CheckNumber(ref mIssuePrice, "", false);
                GenFunctions.CheckNumber(ref mIssued, "", false);
                #endregion

                #region Save Data
                strSQL = "Insert into tbOutWards (IndentItemID, ItemID, InwNo, IssueQty, IssuePrice, Issued) values (" + mIndentItemID + ", " + mItemID + ", " + mInwNo + ", " + mIssueQty + ", " + mIssuePrice + ", " + mIssued + ")";
                DBHelper.GetDataTable(strSQL);
                #endregion

                if (TotAllotQty == 0) break;
            }
            #endregion
        }
        #endregion

        #region CheckStockRecon
        public static void CheckStockRecon(string mDate)
        {
            GenFunctions.CheckDate(ref mDate, "Date", false); 
            string strSQL = "INSERT INTO stkRecon (SourceID, SchemeID, WarehouseID, ItemID, MonYr) SELECT a.SourceID, a.SchemeID, b.WarehouseID, a.ItemID, TRUNC(" + mDate + ", 'Mon') FROM masItemsBySourceByScheme a INNER JOIN masWarehouses b ON (1=1) WHERE NOT EXISTS (SELECT * FROM stkRecon WHERE SourceID = a.SourceID AND SchemeID = a.SchemeID AND WarehouseID = b.WarehouseID AND ItemID = a.ItemID AND MonYr = TRUNC(" + mDate + ", 'Mon')) AND WarehouseID IN (SELECT DISTINCT WarehouseID FROM stkRecon)";
            DBHelper.ExecuteNonQuery(strSQL);
        }
        #endregion



        #region UpdateFacilityAllotReturnToWH
        public static void UpdateFacilityAllotReturnToWH(string FacilityID, string SourceID, string SchemeID, string ItemID, double IssueQty, string IssueItemID)
        {
            string WhereConditions = " and (rb.ExpDate >= SysDate or nvl(rb.ExpDate,SysDate) >= SysDate)";

            #region Delete existing allotements
            string strSQL = "Delete from tbFacilityOutwards where inwno=" + ItemID + " and IssueItemID=" + IssueItemID;
            DBHelper.ExecuteNonQuery(strSQL);
            #endregion

            double TotAllotQty = IssueQty;

            strSQL = @"select distinct rb.FACReceiptItemid FacReceiptItemID,rb.BatchNo,rb.MfgDate,rb.ExpDate,nvl(rb.AbsRqty,0) AbsRQty,nvl(x.issueQty,0) AllotQty , nvl(rb.AbsRqty,0)-nvl(x.issueQty,0) avlQty
 , rb.Inwno 
  from tbFacilityReceiptBatches rb  
  inner join tbfacilityreceiptitems i on rb.FACreceiptitemid=i.FACreceiptitemid  
  inner join tbfacilityreceipts r on r.FACreceiptid=i.FACreceiptid 
  left outer join (
    select   '' BatchNo,'' MfgDate,'' ExpDate,sum(nvl(a.IssueQty,0)) issueQty, 0 AbsRQty,  
  a.Inwno   
  from  tbfacilityoutwards a
  inner join tbfacilityissueitems tbi on tbi.issueitemid =a.issueitemid   
  inner join tbfacilityissues tb on tb.issueid=tbi.issueid    
  Where tb.status='C' and tb.FacilityID=" + FacilityID + @"   and a.inwno =" + ItemID + @"  group by a.Inwno    
  ) x on x.inwno=rb.inwno
  where  r.status='C' and r.FacilityID=" + FacilityID + @"   and rb.inwno= " + ItemID + @"    
   and (rb.ExpDate >= SysDate or nvl(rb.ExpDate,SysDate) >= SysDate)  and (nvl(rb.AbsRqty,0)-nvl(x.issueQty,0))>0
   order by expdate";
            //and to_date (rb.ExpDate,'dd-MM-yyyy')-to_date (sysdate,'dd-MM-yyyy') >= 15 
            DataTable dt = DBHelper.GetDataTable(strSQL);
            foreach (DataRow dr in dt.Rows)
            {
                #region Calculate Allotted Qty
                double dblCurAllot = 0;
                double availableQty = double.Parse(dr["avlQty"].ToString());

                double issueQty = 0;
                #region Data to be Saved
                string mIssueItemID = IssueItemID;
                string mItemID = ItemID;
                string mFacReceiptItemID = dr["FacReceiptItemID"].ToString();
                string mInWno = dr["Inwno"].ToString();
                string mIssueQty = dblCurAllot.ToString();
                string mIssued = "0";
                #endregion

                #region Validate Data
                GenFunctions.CheckNumber(ref mIssueItemID, "", false);
                GenFunctions.CheckNumber(ref mItemID, "", false);
                GenFunctions.CheckNumber(ref mFacReceiptItemID, "", false);
                GenFunctions.CheckNumber(ref mIssued, "", false);
                #endregion
                if (TotAllotQty <= availableQty)
                {
                    issueQty = TotAllotQty;
                    TotAllotQty -= issueQty;
                    #region Save Data
                    strSQL = "Insert into tbFacilityOutwards (IssueItemID,ItemID,FacReceiptItemID,IssueQty,Issued,Inwno) values (" +
                        mIssueItemID + "," + mItemID + "," + mFacReceiptItemID + "," + issueQty + "," + mIssued + "," + mInWno + ")";
                    DBHelper.GetDataTable(strSQL);
                    #endregion
                }
                else
                {
                    issueQty = availableQty;
                    TotAllotQty -= issueQty;
                    #region Save Data
                    strSQL = "Insert into tbFacilityOutwards (IssueItemID,ItemID,FacReceiptItemID,IssueQty,Issued,Inwno) values (" +
                        mIssueItemID + "," + mItemID + "," + mFacReceiptItemID + "," + issueQty + "," + mIssued + "," + mInWno + ")";
                    DBHelper.GetDataTable(strSQL);
                    #endregion

                }
                #endregion
                if (TotAllotQty == 0) break;
            }
        }
        #endregion



 #region UpdateFacilityAllotQtyOP
        public static void UpdateFacilityAllotinwnoQty(string FacilityID, string SourceID, string SchemeID, string ItemID, double IssueQty, string IssueItemID)
        {
            string WhereConditions = " and (rb.ExpDate >= SysDate or nvl(rb.ExpDate,SysDate) >= SysDate)";

            #region Delete existing allotements
            string strSQL = "Delete from tbFacilityOutwards where inwno=" + ItemID + " and IssueItemID=" + IssueItemID;
            DBHelper.ExecuteNonQuery(strSQL);
            #endregion

            double TotAllotQty = IssueQty;
       
            strSQL = @"select distinct rb.FACReceiptItemid FacReceiptItemID,rb.BatchNo,rb.MfgDate,rb.ExpDate,nvl(rb.AbsRqty,0) AbsRQty,nvl(x.issueQty,0) AllotQty , nvl(rb.AbsRqty,0)-nvl(x.issueQty,0) avlQty
 , rb.Inwno 
  from tbFacilityReceiptBatches rb  
  inner join tbfacilityreceiptitems i on rb.FACreceiptitemid=i.FACreceiptitemid  
  inner join tbfacilityreceipts r on r.FACreceiptid=i.FACreceiptid 
  left outer join (
    select   '' BatchNo,'' MfgDate,'' ExpDate,sum(nvl(a.IssueQty,0)) issueQty, 0 AbsRQty,  
  a.Inwno   
  from  tbfacilityoutwards a
  inner join tbfacilityissueitems tbi on tbi.issueitemid =a.issueitemid   
  inner join tbfacilityissues tb on tb.issueid=tbi.issueid    
  Where tb.status='C' and tb.FacilityID=" + FacilityID + @"   and a.inwno =" + ItemID + @"  group by a.Inwno    
  ) x on x.inwno=rb.inwno
  where  r.status='C' and r.FacilityID=" + FacilityID + @"   and rb.inwno= " + ItemID + @"    
   and (rb.ExpDate >= SysDate or nvl(rb.ExpDate,SysDate) >= SysDate)  and (nvl(rb.AbsRqty,0)-nvl(x.issueQty,0))>0
   and to_date (rb.ExpDate,'dd-MM-yyyy')-to_date (sysdate,'dd-MM-yyyy') >= 15 order by expdate";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            foreach (DataRow dr in dt.Rows)
            {
                #region Calculate Allotted Qty
                double dblCurAllot = 0;
                double availableQty = double.Parse(dr["avlQty"].ToString());

                double issueQty = 0;
                #region Data to be Saved
                string mIssueItemID = IssueItemID;
                string mItemID = ItemID;
                string mFacReceiptItemID = dr["FacReceiptItemID"].ToString();
                string mInWno = dr["Inwno"].ToString();
                string mIssueQty = dblCurAllot.ToString();
                string mIssued = "0";
                #endregion

                #region Validate Data
                GenFunctions.CheckNumber(ref mIssueItemID, "", false);
                GenFunctions.CheckNumber(ref mItemID, "", false);
                GenFunctions.CheckNumber(ref mFacReceiptItemID, "", false);               
                GenFunctions.CheckNumber(ref mIssued, "", false);
                #endregion
                if (TotAllotQty <= availableQty)
                {
                    issueQty = TotAllotQty;
                    TotAllotQty -= issueQty;
                    #region Save Data
					  string strquerCheck1 = @" select FacilityID,rb.absrqty,sum(nvl(a.IssueQty,0)) TotissueQty, a.Inwno   
                            from  tbfacilityoutwards a
                            inner join tbfacilityissueitems tbi on tbi.issueitemid =a.issueitemid   
                            inner join tbfacilityissues tb on tb.issueid=tbi.issueid   
                            inner join tbfacilityreceiptbatches  rb on rb.inwno=a.inwno
                            where 1=1 and  a.inwno=" + mInWno + @"
                            group by a.Inwno ,FacilityID, rb.absrqty
                            having sum(nvl(a.IssueQty,0))>=rb.absrqty ";
                   DataTable td1= DBHelper.GetDataTable(strquerCheck1);
                   if (td1.Rows.Count > 0)
                   {
                   }
                   else
                   {
                    strSQL = "Insert into tbFacilityOutwards (IssueItemID,ItemID,FacReceiptItemID,IssueQty,Issued,Inwno) values (" +
                        mIssueItemID + "," + mItemID + "," + mFacReceiptItemID + "," + issueQty + "," + mIssued + "," + mInWno + ")";
                    DBHelper.GetDataTable(strSQL);
				   }
                    #endregion
                }
                else
                {
                    issueQty = availableQty;
                    TotAllotQty -= issueQty;
                    #region Save Data
					string strquerCheck = @" select FacilityID,rb.absrqty,sum(nvl(a.IssueQty,0)) TotissueQty, a.Inwno   
                            from  tbfacilityoutwards a
                            inner join tbfacilityissueitems tbi on tbi.issueitemid =a.issueitemid   
                            inner join tbfacilityissues tb on tb.issueid=tbi.issueid   
                            inner join tbfacilityreceiptbatches  rb on rb.inwno=a.inwno
                            where 1=1 and  a.inwno=" + mInWno + @"
                            group by a.Inwno ,FacilityID, rb.absrqty
                            having sum(nvl(a.IssueQty,0))>=rb.absrqty ";
                   DataTable td= DBHelper.GetDataTable(strquerCheck);
                   if (td.Rows.Count > 0)
                   {
                   }
                   else
                   {
                    strSQL = "Insert into tbFacilityOutwards (IssueItemID,ItemID,FacReceiptItemID,IssueQty,Issued,Inwno) values (" +
                        mIssueItemID + "," + mItemID + "," + mFacReceiptItemID + "," + issueQty + "," + mIssued + "," + mInWno + ")";
                    DBHelper.GetDataTable(strSQL);
				   }
                    #endregion

                }
                #endregion
                if (TotAllotQty == 0) break;
            }
        }
        #endregion

        #region UpdateFacilityAllotQtyOP
        public static void UpdateFacilityAllotQtyOP(string FacilityID, string SourceID, string SchemeID, string ItemID, double IssueQty, string IssueItemID)
        {
            string WhereConditions = " and (rb.ExpDate >= SysDate or nvl(rb.ExpDate,SysDate) >= SysDate)";

            #region Delete existing allotements
            string strSQL = "Delete from tbFacilityOutwards where IssueItemID=" + IssueItemID;
            DBHelper.ExecuteNonQuery(strSQL);
            #endregion

            double TotAllotQty = IssueQty;
            //strSQL = "Select b.FacReceiptItemID,b.BatchNo,b.MdgDate,b.ExpDate,NVL(b.AbsRQty,0) as AbsRQty,NVL(b.AllotQty,0) as AllotQty" +
            //    " from tbFacilityReceiptItems b" +
            //    " Inner Join tbFacilityReceipts a1 on (a1.FacReceiptID=b.FacReceiptID)" +
            //    " Where a1.FacilityID=" + FacilityID +
            //    "   and b.ItemID=" + ItemID + WhereConditions + " and NVL(b.AbsRQty,0)-NVL(b.AllotQty,0)>0 and b.BatchStatus='R' to_date (b.ExpDate,'dd-MM-yyyy')-to_date (sysdate,'dd-MM-yyyy')>=10 " +
            ////and (to_Date(to_char(b.ExpDate,'dd-mm-yyyy'),'dd-mm-yyyy') - to_Date(to_char(sysdate,'dd-mm-yyyy'),'dd-mm-yyyy')) >= 15 " +
            //    " Order By b.ExpDate,b.AbsRQty";
 //           strSQL = " select FacReceiptItemID,BatchNo,MfgDate,ExpDate,sum(nvl(AbsRQty,0)) AbsRQty,sum(nvl(AllotQty,0)) AllotQty,Inwno "+
 //           " from ( Select b.FacReceiptItemID,rb.BatchNo,rb.MfgDate,rb.ExpDate,NVL(o.IssueQty,0)* mi.unitcount as AbsRQty,NVL(o.IssueQty,0) as AllotQty,rb.Inwno " +
 //                    " from tbFacilityReceiptItems b " +
 //                    " inner  join masitems mi on  mi.itemid=b.itemid " +
 //                    " Inner Join tbFacilityReceipts a1 on (a1.FacReceiptID=b.FacReceiptID) " +
 //                    " inner join tbindents i on i.indentid=a1.indentid" +
 //                    " inner join tbindentitems ii on ii.indentid=i.indentid and ii.itemid=b.itemid " +
 //                    " inner join tboutwards o on o.indentitemid=ii.indentitemid" +
 //                    " inner join tbreceiptbatches rb on rb.inwno=o.inwno" +
 //                    " Where a1.FacilityID=" + FacilityID + " and b.ItemID=" + ItemID + WhereConditions +
 //                    " and to_date (rb.ExpDate,'dd-MM-yyyy')-to_date (sysdate,'dd-MM-yyyy') >= 15" +
 //                   "  union "+    
                  
 
 // " select distinct a.facreceiptitemid FacReceiptItemID,rb.BatchNo,rb.MfgDate,rb.ExpDate,nvl(a.IssueQty,0) AbsRQty, nvl(a.IssueQty,0) AllotQty,rb.Inwno  "  +       
 //" from "+
 //" tbFacilityReceipts r "+
 //" inner join tbFacilityReceiptItems ri on ri.facreceiptid=r.facreceiptid "+
 //" inner join tbfacilityissues tb on tb.issueid=r.issueid "+
 //" inner join tbfacilityissueitems tbi on tbi.issueid =tb.issueid "+
 //" inner join tbfacilityoutwards a on a.issueitemid=tbi.issueitemid "+
 //" inner join tbreceiptbatches rb on rb.inwno=a.inwno "+
 // " Where r.FacilityID=" + FacilityID + " and tbi.itemid=" + ItemID+ " and (rb.ExpDate >= SysDate or nvl(rb.ExpDate,SysDate) >= SysDate) " +
 // " and to_date (rb.ExpDate,'dd-MM-yyyy')-to_date (sysdate,'dd-MM-yyyy') >= 15 "+
 //        " union "+
 //       "  select b.FACReceiptItemid ReceiptItemid,b.BatchNo,b.MfgDate,b.ExpDate,nvl(b.AbsRqty,0)AbsRQty, nvl(b.IssueQty,0) AllotQty,b.Inwno "+
 //" from tbFacilityReceiptBatches b  inner join tbfacilityreceiptitems i on b.FACreceiptitemid=i.FACreceiptitemid "+
 //" inner join tbfacilityreceipts r on r.FACreceiptid=i.FACreceiptid "+
 //" where r.FACreceipttype='FC' and r.status='C' and r.FacilityID= " + FacilityID + "  and i.ItemID= " + ItemID +
 //            //" select b.ReceiptItemid,BatchNo,MfgDate,ExpDate,nvl(AbsRqty,0)op_qty,0,Inwno  from tbreceiptbatches b" +
 //            //       "  inner join tbreceiptitems i on b.receiptitemid=i.receiptitemid" +
 //            //       "  inner join tbreceipts r on r.receiptid=i.receiptid where r.receipttype='FC' and r.status='C' and FacilityID= " + FacilityID + " and i.ItemID= " + ItemID +
 //                   "  ) group by FacReceiptItemID,BatchNo,MfgDate,ExpDate,Inwno order by expdate ";
                     //"  union " +
                     //"  select i.ReceiptItemID,rb.BatchNo,rb.MfgDate,rb.ExpDate,nvl(rb.absrqty,0) LPReceipt, nvl(AllotQty,0) AllotQty ,rb.Inwno " +
                     //"  from lptbreceiptbatches rb " +
                     //"  inner join lptbreceiptitems i on rb.receiptitemid=i.receiptitemid " +
                     //"  inner join lptbreceipts a1 on a1.receiptid=i.receiptid Where a1.status='C' and to_date (rb.ExpDate,'dd-MM-yyyy')-to_date (sysdate,'dd-MM-yyyy') >= 15 and a1.FacilityID=" + FacilityID + " and i.LPItemID=" + ItemID + WhereConditions + " Order by ExpDate";
                   
  
//            strSQL= "select FacReceiptItemID,BatchNo,MfgDate,ExpDate,sum(nvl(AbsRQty,0)) AbsRQty,sum(nvl(AllotQty,0)) AllotQty,Inwno  "+
//             " from( "+
//   " select distinct a.facreceiptitemid FacReceiptItemID,rb.BatchNo,rb.MfgDate,rb.ExpDate,nvl(a.IssueQty,0) AbsRQty, nvl(a.IssueQty,0) AllotQty,rb.Inwno     "+     
// "  from "+
//  " tbFacilityReceipts r "+
//  " inner join tbFacilityReceiptItems ri on ri.facreceiptid=r.facreceiptid "+
//  " inner join tbfacilityissues tb on tb.issueid=r.issueid  "+
//  " inner join tbfacilityissueitems tbi on tbi.issueid =tb.issueid  "+
// " inner join tbfacilityoutwards a on a.issueitemid=tbi.issueitemid  "+
//  " inner join tbFacilityReceiptBatches rb on rb.inwno=a.inwno "+
//  " Where r.FacilityID= " + FacilityID + "  and tbi.itemid =" + ItemID + "  and (rb.ExpDate >= SysDate or nvl(rb.ExpDate,SysDate) >= SysDate)  " +
//  " and to_date (rb.ExpDate,'dd-MM-yyyy')-to_date (sysdate,'dd-MM-yyyy') >= 15 "+
//       "   union "+
//        "  select rb.FACReceiptItemid ReceiptItemid,rb.BatchNo,rb.MfgDate,rb.ExpDate,nvl(rb.AbsRqty,0)AbsRQty, nvl(rb.IssueQty,0) AllotQty,rb.Inwno "+
// " from tbFacilityReceiptBatches rb  inner join tbfacilityreceiptitems i on rb.FACreceiptitemid=i.FACreceiptitemid "+
// " inner join tbfacilityreceipts r on r.FACreceiptid=i.FACreceiptid "+
//"  where  r.status='C' and r.FacilityID=" + FacilityID + " and i.ItemID=" + ItemID + "  " +
// " and (rb.ExpDate >= SysDate or nvl(rb.ExpDate,SysDate) >= SysDate)  "+
//  " and to_date (rb.ExpDate,'dd-MM-yyyy')-to_date (sysdate,'dd-MM-yyyy') >= 15 " +
//                  "    ) group by FacReceiptItemID,BatchNo,MfgDate,ExpDate,Inwno order by expdate ";

  //          strSQL = "select FacReceiptItemID,BatchNo,MfgDate,ExpDate,sum(AbsRQty) AbsRQty,sum(nvl(issueQty,0)) AllotQty,sum(AbsRQty)-sum(nvl(issueQty,0)) avlQty,Inwno  " +
  //"from(   " +
  //"select distinct a.facreceiptitemid FacReceiptItemID,rb.BatchNo,rb.MfgDate,rb.ExpDate,nvl(a.IssueQty,0) issueQty, 0 AbsRQty,  " +
  //"rb.Inwno ,a.facoutwardid   " +
  //"from  tbFacilityReceipts r   " +
  //"inner join tbFacilityReceiptItems ri on ri.facreceiptid=r.facreceiptid   " +
  //"inner join tbFacilityReceiptBatches rb on rb.facreceiptitemid=ri.facreceiptitemid " +
  //"inner join tbfacilityoutwards a on a.inwno=rb.inwno  " +
  //"inner join tbfacilityissueitems tbi on tbi.issueitemid =a.issueitemid   " +
  //"inner join tbfacilityissues tb on tb.issueid=tbi.issueid    " +
  //"Where r.FacilityID= " + FacilityID + "  and tbi.itemid =" + ItemID + "   and (rb.ExpDate >= SysDate or nvl(rb.ExpDate,SysDate) >= SysDate)  " +
  //"and to_date (rb.ExpDate,'dd-MM-yyyy')-to_date (sysdate,'dd-MM-yyyy') >= 15  " +
  //"union   " +
  //"select distinct rb.FACReceiptItemid FacReceiptItemID,rb.BatchNo,rb.MfgDate,rb.ExpDate,0 issueQty,nvl(rb.AbsRqty,0) AbsRQty, " +
  //"rb.Inwno ,0 facoutwardid from tbFacilityReceiptBatches rb  " +
  //"inner join tbfacilityreceiptitems i on rb.FACreceiptitemid=i.FACreceiptitemid  " +
  //"inner join tbfacilityreceipts r on r.FACreceiptid=i.FACreceiptid   " +
  //"where  r.status='C' and r.FacilityID=" + FacilityID + " and i.ItemID=" + ItemID + "    " +
  //"and (rb.ExpDate >= SysDate or nvl(rb.ExpDate,SysDate) >= SysDate)  " +
  //"and to_date (rb.ExpDate,'dd-MM-yyyy')-to_date (sysdate,'dd-MM-yyyy') >= 15  " +
  //") group by FacReceiptItemID,BatchNo,MfgDate,ExpDate,Inwno having  nvl((sum(AbsRQty)-sum(nvl(issueQty,0))),0)>0 order by expdate  ";
  
  int days=15;
  
  

            strSQL = @"select distinct rb.FACReceiptItemid FacReceiptItemID,rb.BatchNo,rb.MfgDate,rb.ExpDate,nvl(rb.AbsRqty,0) AbsRQty,nvl(x.issueQty,0) AllotQty , nvl(rb.AbsRqty,0)-nvl(x.issueQty,0) avlQty
 , rb.Inwno ,rb.whinwno
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
   and (rb.ExpDate >= SysDate or nvl(rb.ExpDate,SysDate) >= SysDate)  and (nvl(rb.AbsRqty,0)-nvl(x.issueQty,0))>0
   and Round(rb.ExpDate-sysdate,0) >= "+days+" order by expdate";

            //and to_date (rb.ExpDate,'dd-MM-yyyy')-to_date (sysdate,'dd-MM-yyyy') 

            DataTable dt = DBHelper.GetDataTable(strSQL);
            foreach (DataRow dr in dt.Rows)
            {
                #region Calculate Allotted Qty
                double dblCurAllot = 0;
                double availableQty = double.Parse(dr["avlQty"].ToString());

                double issueQty = 0;
                #region Data to be Saved
                string mIssueItemID = IssueItemID;
                string mItemID = ItemID;
                string mFacReceiptItemID = dr["FacReceiptItemID"].ToString();
                string mInWno = dr["Inwno"].ToString();
                string mWhInWno = dr["whinwno"].ToString();
                string mIssueQty = dblCurAllot.ToString();
                string mIssued = "0";
                #endregion

                #region Validate Data
                GenFunctions.CheckNumber(ref mIssueItemID, "", false);
                GenFunctions.CheckNumber(ref mItemID, "", false);
                GenFunctions.CheckNumber(ref mFacReceiptItemID, "", false);               
                GenFunctions.CheckNumber(ref mIssued, "", false);
                #endregion
                if (TotAllotQty <= availableQty)
                {
                    issueQty = TotAllotQty;
                    TotAllotQty -= issueQty;
                    #region Save Data
                    strSQL = "Insert into tbFacilityOutwards (IssueItemID,ItemID,FacReceiptItemID,IssueQty,Issued,Inwno,whinwno) values (" +
                        mIssueItemID + "," + mItemID + "," + mFacReceiptItemID + "," + issueQty + "," + mIssued + "," + mInWno + ",'" + mWhInWno + "' )";
                    DBHelper.GetDataTable(strSQL);
                    #endregion
                }
                else
                {
                    issueQty = availableQty;
                    TotAllotQty -= issueQty;
                    #region Save Data
                    strSQL = "Insert into tbFacilityOutwards (IssueItemID,ItemID,FacReceiptItemID,IssueQty,Issued,Inwno,whinwno) values (" +
                        mIssueItemID + "," + mItemID + "," + mFacReceiptItemID + "," + issueQty + "," + mIssued + "," + mInWno + ",'" + mWhInWno + "')";
                    DBHelper.GetDataTable(strSQL);
                    #endregion

                }
                #endregion
                if (TotAllotQty == 0) break;
            }
        }
        #endregion

        #region UpdateFacilityAllotQty
        public static void UpdateFacilityAllotQty(string FacilityID, string SourceID, string SchemeID, string ItemID, double IssueQty, string IssueItemID)
        {
            string WhereConditions = " and (rb.ExpDate >= SysDate or nvl(rb.ExpDate,SysDate) >= SysDate)";

            #region Delete existing allotements
            string strSQL = "Delete from tbFacilityOutwards where IssueItemID=" + IssueItemID;
            DBHelper.ExecuteNonQuery(strSQL);
            #endregion

            double TotAllotQty = IssueQty;
            //strSQL = "Select b.FacReceiptItemID,b.BatchNo,b.MdgDate,b.ExpDate,NVL(b.AbsRQty,0) as AbsRQty,NVL(b.AllotQty,0) as AllotQty" +
            //    " from tbFacilityReceiptItems b" +
            //    " Inner Join tbFacilityReceipts a1 on (a1.FacReceiptID=b.FacReceiptID)" +
            //    " Where a1.FacilityID=" + FacilityID +
            //    "   and b.ItemID=" + ItemID + WhereConditions + " and NVL(b.AbsRQty,0)-NVL(b.AllotQty,0)>0 and b.BatchStatus='R' to_date (b.ExpDate,'dd-MM-yyyy')-to_date (sysdate,'dd-MM-yyyy')>=10 " +
            ////and (to_Date(to_char(b.ExpDate,'dd-mm-yyyy'),'dd-mm-yyyy') - to_Date(to_char(sysdate,'dd-mm-yyyy'),'dd-mm-yyyy')) >= 15 " +
            //    " Order By b.ExpDate,b.AbsRQty";
            strSQL = " select FacReceiptItemID,BatchNo,MfgDate,ExpDate,sum(nvl(AbsRQty,0)) AbsRQty,sum(nvl(AllotQty,0)) AllotQty,Inwno " +
            " from ( Select b.FacReceiptItemID,rb.BatchNo,rb.MfgDate,rb.ExpDate,NVL(o.IssueQty,0)* mi.unitcount as AbsRQty,NVL(o.IssueQty,0) as AllotQty,rb.Inwno " +
                     " from tbFacilityReceiptItems b " +
                     " inner  join masitems mi on  mi.itemid=b.itemid " +
                     " Inner Join tbFacilityReceipts a1 on (a1.FacReceiptID=b.FacReceiptID) " +
                     " inner join tbindents i on i.indentid=a1.indentid" +
                     " inner join tbindentitems ii on ii.indentid=i.indentid and ii.itemid=b.itemid " +
                     " inner join tboutwards o on o.indentitemid=ii.indentitemid" +
                     " inner join tbreceiptbatches rb on rb.inwno=o.inwno" +
                     " Where a1.FacilityID=" + FacilityID + " and b.ItemID=" + ItemID + WhereConditions +
                     " and to_date (rb.ExpDate,'dd-MM-yyyy')-to_date (sysdate,'dd-MM-yyyy') >= 15" +
                    "  union " +


  " select distinct a.facreceiptitemid FacReceiptItemID,rb.BatchNo,rb.MfgDate,rb.ExpDate,nvl(a.IssueQty,0) AbsRQty, nvl(a.IssueQty,0) AllotQty,rb.Inwno  " +
 " from " +
 " tbFacilityReceipts r " +
 " inner join tbFacilityReceiptItems ri on ri.facreceiptid=r.facreceiptid " +
 " inner join tbfacilityissues tb on tb.issueid=r.issueid " +
 " inner join tbfacilityissueitems tbi on tbi.issueid =tb.issueid " +
 " inner join tbfacilityoutwards a on a.issueitemid=tbi.issueitemid " +
 " inner join tbreceiptbatches rb on rb.inwno=a.inwno " +
  " Where r.FacilityID=" + FacilityID + " and tbi.itemid=" + ItemID + " and (rb.ExpDate >= SysDate or nvl(rb.ExpDate,SysDate) >= SysDate) " +
  " and to_date (rb.ExpDate,'dd-MM-yyyy')-to_date (sysdate,'dd-MM-yyyy') >= 15 " +
         " union " +
             " select b.ReceiptItemid,BatchNo,MfgDate,ExpDate,nvl(AbsRqty,0)op_qty,0,Inwno  from tbreceiptbatches b" +
                    "  inner join tbreceiptitems i on b.receiptitemid=i.receiptitemid" +
                    "  inner join tbreceipts r on r.receiptid=i.receiptid where r.receipttype='FC' and r.status='C' and FacilityID= " + FacilityID + " and i.ItemID= " + ItemID +
                    "  ) group by FacReceiptItemID,BatchNo,MfgDate,ExpDate,Inwno order by expdate ";
            //"  union " +
            //"  select i.ReceiptItemID,rb.BatchNo,rb.MfgDate,rb.ExpDate,nvl(rb.absrqty,0) LPReceipt, nvl(AllotQty,0) AllotQty ,rb.Inwno " +
            //"  from lptbreceiptbatches rb " +
            //"  inner join lptbreceiptitems i on rb.receiptitemid=i.receiptitemid " +
            //"  inner join lptbreceipts a1 on a1.receiptid=i.receiptid Where a1.status='C' and to_date (rb.ExpDate,'dd-MM-yyyy')-to_date (sysdate,'dd-MM-yyyy') >= 15 and a1.FacilityID=" + FacilityID + " and i.LPItemID=" + ItemID + WhereConditions + " Order by ExpDate";

            DataTable dt = DBHelper.GetDataTable(strSQL);
            foreach (DataRow dr in dt.Rows)
            {
                #region Calculate Allotted Qty
                double dblCurAllot = 0;
                double availableQty = double.Parse(dr["AbsRQty"].ToString());

                double issueQty = 0;
                #region Data to be Saved
                string mIssueItemID = IssueItemID;
                string mItemID = ItemID;
                string mFacReceiptItemID = dr["FacReceiptItemID"].ToString();
                string mInWno = dr["Inwno"].ToString();
                string mIssueQty = dblCurAllot.ToString();
                string mIssued = "0";
                #endregion

                #region Validate Data
                GenFunctions.CheckNumber(ref mIssueItemID, "", false);
                GenFunctions.CheckNumber(ref mItemID, "", false);
                GenFunctions.CheckNumber(ref mFacReceiptItemID, "", false);
                GenFunctions.CheckNumber(ref mIssued, "", false);
                #endregion
                if (TotAllotQty <= availableQty)
                {
                    issueQty = TotAllotQty;
                    TotAllotQty -= issueQty;
                    #region Save Data
                    strSQL = "Insert into tbFacilityOutwards (IssueItemID,ItemID,FacReceiptItemID,IssueQty,Issued,Inwno) values (" +
                        mIssueItemID + "," + mItemID + "," + mFacReceiptItemID + "," + issueQty + "," + mIssued + "," + mInWno + ")";
                    DBHelper.GetDataTable(strSQL);
                    #endregion
                }
                else
                {
                    issueQty = availableQty;
                    TotAllotQty -= issueQty;
                    #region Save Data
                    strSQL = "Insert into tbFacilityOutwards (IssueItemID,ItemID,FacReceiptItemID,IssueQty,Issued,Inwno) values (" +
                        mIssueItemID + "," + mItemID + "," + mFacReceiptItemID + "," + issueQty + "," + mIssued + "," + mInWno + ")";
                    DBHelper.GetDataTable(strSQL);
                    #endregion

                }
                #endregion
                if (TotAllotQty == 0) break;
            }
        }
        #endregion
        #region GetFacilityStock
//        public static double GetFacilityStock(string mFacilityID, string mSourceID, string mSchemeID, string mItemID)
//        {
//            string facilityid = mFacilityID;
////      string       strSQL = " Select  0 as SlNo,nvl(m1.multiple,1) multiple,m1.ItemID,m1.itemname, " +
////" ((NVL(stk.CurStock,0) +nvl(FTF_Receipt,0) + nvl(GET_FOPstock(" + mFacilityID + ",m1.itemid),0))-((case when nvl(cum6.IssueQty,0)>0 then nvl(cum6.IssueQty,0) else 0 end)+ (case when (nvl(stk.Reserved,0))>0 then  (nvl(stk.Reserved,0)) else 0 end))+(nvl(WR.WardReturnQty,0)))  CurStock, " +

////" ((case when nvl(m1.unitcount,0)>0 and (nvl(stk.Reserved,0))>0 " +
////" then  (nvl(stk.Reserved,0)) else 0 end))  ReservedQty, " +
////" ((case when nvl(m1.unitcount,0)>0 and nvl(cum6.IssueQty,0)>0 then nvl(cum6.IssueQty,0) else 0 end))  as CumlMon6," +
////" (nvl(WR.WardReturnQty,0))  WardReturnQty,nvl(FTF_Receipt,0) " +

////           " From masItems m1" +
////           " Left Outer Join v_FacilityStock stk on (stk.ItemID=m1.ItemID and stk.FacilityID=" + mFacilityID + ")" +
////           " Left Outer Join (Select rc.FacilityID,rcit.ItemID, sum(case when rc.Status='C' then (ow.IssueQty) else 0 end) IssueQty ,sum(case when rc.Status='IN' then (ow.IssueQty) else 0 end) ReservedQty_OP " +
////           " From tbFacilityIssues rc" +
////           " Inner Join tbFacilityIssueItems rcit on (rc.IssueID=rcit.IssueID)" +
////           " Left Outer Join tbFacilityOutwards ow on (ow.IssueItemID=rcit.IssueItemID)" +
////           " Where rc.IssueDate > Add_Months(sysdate,-6 ) " +
////           " Group by rc.FacilityID,rcit.ItemID) cum6 on (cum6.FacilityID=" + mFacilityID + " and cum6.ItemID=m1.ItemID)" +
////           " Left Outer Join " +
////           " ( " +
////           "    Select r.FacilityID,i.ItemID,sum(nvl(i.AbsRQty,0)) WardReturnQty  from " +
////           "    tbFacilityReceipts r " +
////           "    inner join tbFacilityReceiptItems i on r.FacReceiptID=i.FacReceiptID " +
////           "    where r.FacReceiptType='RW' and r.status='C' and ReasonFlag='Y' group by r.FacilityID,i.ItemID " +
////           " ) WR on WR.ItemID=stk.ItemID and WR.facilityID=stk.facilityID " +

////           "  Left Outer Join  "+
//// " ( Select r.FacilityID,i.ItemID,sum(nvl(i.AbsRQty,0)) FTF_Receipt  "+
//// " from     tbFacilityReceipts r "+ 
//// " inner join tbFacilityReceiptItems i on r.FacReceiptID=i.FacReceiptID    "+
//// " where r.FacReceiptType='SP' and r.status='C' "+
//// " group by r.FacilityID,i.ItemID  )   FF  on FF.ItemID=m1.ItemID and FF.facilityID=" + mFacilityID + " "+

////           " Where 1=1 and m1.ItemID = " + mItemID +


////           " union " +
////           " Select 0 as SlNo,nvl(m1.multiple,1) multiple,m1.LPItemID,ItemName,(sum(nvl(b.AbsRQty,0))-(nvl(iss.issueqty,0)+ nvl(iss.ReservedQty,0)) + nvl(WardReturnQty,0))  CurBal, " +
////           " ((case when nvl(iss.ReservedQty ,0)>0 then nvl(iss.ReservedQty ,0) else 0 end))  ReservedQty,((case when nvl(iss.IssueQty ,0)>0 then nvl(iss.IssueQty,0) else 0 end)) IssueQty,((case when nvl(WardReturnQty ,0)>0 then nvl(WardReturnQty,0) else 0 end))   WardReturnQty,0 FTF_Receipt   " +
////           " from lptbreceipts r inner join lptbreceiptitems i on r.receiptid=i.receiptid inner join lptbreceiptbatches b on b.receiptitemid=i.receiptitemid inner join lpmasItems m1 on m1.lpitemid=i.lpitemid" +
////           " left outer join     " +
////           "(   " +
////                " select b.itemid, facilityid,sum(case when a.status='C' then nvl(ow.issueqty,0) end) issueqty, sum(case when a.status='IN' then nvl(ow.issueqty,0) end) ReservedQty " +
////                " from tbfacilityissues a inner join tbfacilityissueitems b on a.issueid=b.issueid " +
////                " Left Outer Join tbFacilityOutwards ow on (ow.IssueItemID=b.IssueItemID)  Left Outer Join tbReceiptBatches rb on ow.inwno=rb.inwno and rb.Inwno=ow.Inwno " +
////                " group by b.itemid, facilityid    " +
////           " ) iss on (iss.itemid=i.LPItemID and iss.facilityid=r.facilityid )" +
////           " Left Outer Join " +
////           " ( " +
////           "    select i.LPItemID,r.FacilityID, SUm(nvl(b.AbsRQty,0)) WardReturnQty " +
////           "   from lptbReceipts r" +
////           "    inner join lptbReceiptItems i on r.ReceiptID=i.ReceiptID " +
////           "    inner join lptbReceiptBatches b on b.ReceiptItemId=i.ReceiptItemId" +
////           "    where r.status='C' and ReasonFlag='Y' and ReceiptType='RW' group by i.LPItemID,r.FacilityID" +
////           "  )WR on WR.LPItemID=m1.LPItemID and WR.FacilityID=r.FacilityID" +
////           " Where  r.ReceiptType='NO' and r.status='C' and expdate>sysdate+29 and r.FacilityID= " + mFacilityID +" and  m1.LPItemID ="+mItemID + " group by m1.LPItemID,ItemName,iss.issueqty,iss.ReservedQty,WR.WardReturnQty,m1.unitcount ,m1.multiple Order by ItemName";

//            //old stock code

//            //           string strSQL = " Select ((Sum(NVL(b.CurStock,0))+ nvl(GET_FOPstock(" + mFacilityID + "," + mItemID + "),0)) -(((nvl(iss.issueqty,0))/m1.unitcount)+(nvl(iss.ReservedQty,0))/m1.unitcount) + (nvl(WardReturnQty,0))/m1.unitcount)* m1.unitcount CurStock from masitems m1 left outer join v_FacilityStock b on (b.ItemID=m1.ItemID and b.facilityid=" + mFacilityID + " and b.itemid=" + mItemID + ") left outer join " +
//            //" (  select itemid, facilityid,sum(case when a.status='C' then nvl(issueqty,0) end) issueqty,sum(case when a.status='IN' then nvl(issueqty,0) end) ReservedQty  " +
//            //"  from tbfacilityissues a  inner join tbfacilityissueitems b on a.issueid=b.issueid group by itemid, facilityid ) iss on iss.itemid=b.itemid and iss.facilityid=b.facilityid " +
//            //" Left Outer Join " +
//            //" ( " +
//            //"    Select r.FacilityID,i.ItemID,sum(nvl(i.AbsRQty,0)) WardReturnQty  from " +
//            //"    tbFacilityReceipts r " +
//            //"    inner join tbFacilityReceiptItems i on r.FacReceiptID=i.FacReceiptID " +
//            //"    where r.FacReceiptType='RW' and r.status='C' and ReasonFlag='Y' group by r.FacilityID,i.ItemID " +
//            //" ) WR on WR.ItemID=b.ItemID and WR.facilityID=b.facilityID " +

//            //" group by iss.issueqty,iss.ReservedQty,WardReturnQty,m1.unitcount " +

//            //" Union " +
//            //" Select  (sum(nvl(b.absrqty,0))-(nvl(iss.issueqty,0)/nvl(m1.unitcount,0)+ nvl(iss.ReservedQty,0)/nvl(m1.unitcount,0)) + nvl(WardReturnQty,0)/nvl(m1.unitcount,0)) * m1.unitcount CurStock from lptbreceipts r inner join lptbreceiptitems i on r.receiptid=i.receiptid    inner join lpmasItems m1 on m1.lpitemid=i.lpitemid " +
//            //" inner join lptbreceiptbatches b on b.receiptItemID=i.receiptItemID " +
//            //" left outer join   " +
//            //" (   " +
//            //" select itemid, facilityid,sum(case when a.status='C' then nvl(issueqty,0) end) issueqty, " +
//            //" sum(case when a.status='IN' then nvl(issueqty,0) end) ReservedQty    " +
//            //" from tbfacilityissues a inner join tbfacilityissueitems b on a.issueid=b.issueid group by itemid, facilityid " +
//            //" ) iss on (iss.itemid=i.LPItemID and iss.facilityid=r.facilityid ) " +
//            //" Left Outer Join " +
//            //" ( " +
//            //"   select i.LPItemID,r.FacilityID, SUm(nvl(b.AbsRQty,0)) WardReturnQty " +
//            //"   from lptbReceipts r" +
//            //"   inner join lptbReceiptItems i on r.ReceiptID=i.ReceiptID " +
//            //"   inner join lptbReceiptBatches b on b.ReceiptItemId=i.ReceiptItemId" +
//            //"   where r.status='C' and ReasonFlag='Y' and ReceiptType='RW' group by i.LPItemID,r.FacilityID" +
//            //"  )WR on WR.LPItemID=i.LPItemID and WR.FacilityID=r.FacilityID" +
//            //" Where r.ReceiptType='NO' and r.facilityid=" + mFacilityID + " and i.LPItemID=" + mItemID + " group by iss.issueqty,iss.ReservedQty,WardReturnQty,m1.unitcount";

////            string strSQL = " select 0 as SlNo,multiple,ItemID,itemcode,unitcount,itemname," +
////" ReservedQtyM AS ReservedQty,issueqtyM AS CumlMon6,WardReturnQtyM AS WardReturnQty,nvl(CurBalN+opM+WardReturnQtyM+FTF_Receipt-(ReservedQtyM+issueqtyM),0) as CurStock" +
////" ,nvl((CurBalN+opM+WardReturnQtyM+FTF_Receipt-(ReservedQtyM+issueqtyM)),0) AS CurBal" +
////" from (" +
////"  select 0 as SlNo,nvl(multiple,1) multiple,ItemID,itemcode,unitcount,itemname," +
////"  sum(CurStock) as CurStock, sum(nvl(GET_FOPstock(" + facilityid + ",ItemID),0)) as opM,sum(ReservedQty) as ReservedQtyM " +
////"  ,sum(CumlMon6) as issueqtyM ,sum(WardReturnQty) as  WardReturnQtyM ,sum(CurStock) as CurBalN,sum(FTF_Receipt) FTF_Receipt from (" +
////"  Select  0 as SlNo,nvl(m1.multiple,1) multiple,m1.ItemID,m1.itemcode, m1.unitcount," +
////"  m1.itemname,((NVL(stk.CurStock,0))-(nvl(stk.Reserved,0))) as CurStock," +
////"  ((case when nvl(m1.unitcount,0)>0 and (nvl(stk.Reserved,0))>0  then  (nvl(stk.Reserved,0)) else 0 end))  ReservedQty, " +
////"  ((case when nvl(m1.unitcount,0)>0 and nvl(cum6.IssueQty,0)>0 then nvl(cum6.IssueQty,0) else 0 end))  as CumlMon6," +
////"  (nvl(WR.WardReturnQty,0))  WardReturnQty, nvl(FTF_Receipt,0) FTF_Receipt " +
////"  From masItems m1 " +
////"  Left Outer Join v_FacilityStock stk on (stk.ItemID=m1.ItemID and stk.FacilityID=" + facilityid + ") " +
////"  Left Outer Join (Select rc.FacilityID,rcit.ItemID, sum(case when rc.Status='C' then (ow.IssueQty) else 0 end) IssueQty ," +
////"  sum(case when rc.Status='IN' then (ow.IssueQty) else 0 end) ReservedQty_OP  From tbFacilityIssues rc " +
////"  Inner Join tbFacilityIssueItems rcit on (rc.IssueID=rcit.IssueID)" +
////"  Left Outer Join tbFacilityOutwards ow on (ow.IssueItemID=rcit.IssueItemID) Where rc.IssueDate > Add_Months(sysdate,-6 ) " +
////"  Group by rc.FacilityID,rcit.ItemID) cum6 on (cum6.FacilityID=" + facilityid + " and cum6.ItemID=m1.ItemID)" +
////"  Left Outer Join  (     Select r.FacilityID,i.ItemID,sum(nvl(i.AbsRQty,0)) WardReturnQty  from     tbFacilityReceipts r   " +
////"  inner join tbFacilityReceiptItems i on r.FacReceiptID=i.FacReceiptID    " +
////"  where r.FacReceiptType='RW' and r.status='C' and ReasonFlag='Y' group by r.FacilityID,i.ItemID  ) " +
////"  WR on WR.ItemID=stk.ItemID and WR.facilityID=stk.facilityID " +

////"  Left Outer Join " +
////" ( Select r.FacilityID,i.ItemID,sum(nvl(i.AbsRQty,0)) FTF_Receipt  " +
////" from     tbFacilityReceipts r  " +
////" inner join tbFacilityReceiptItems i on r.FacReceiptID=i.FacReceiptID " +
////" where r.FacReceiptType='SP' and r.status='C' " +
////" group by r.FacilityID,i.ItemID  )     FF  on FF.ItemID=m1.ItemID and FF.facilityID=" + facilityid + "" +
//// " Where 1=1" +
////"  union all" +
////"  Select 0 as SlNo,nvl(m1.multiple,1) multiple,m1.LPItemID as ItemID,m1.itemcode,m1.unitcount,ItemName, " +
////"  sum(nvl(b.AbsRQty,0)) as CurStock," +
////"  ((case when nvl(iss.ReservedQty ,0)>0 then nvl(iss.ReservedQty ,0)/nvl(m1.unitcount,0) else 0 end) ) * nvl(m1.unitcount,1) ReservedQty, " +
////"  ((case when nvl(iss.IssueQty ,0)>0 then nvl(iss.IssueQty,0)/nvl(m1.unitcount,0) else 0 end) ) * nvl(m1.unitcount,1) CumlMon6," +
////"  ((case when nvl(WardReturnQty ,0)>0 then nvl(WardReturnQty,0)/nvl(m1.unitcount,0) else 0 end))* nvl(m1.unitcount,1)   WardReturnQty,0  FTF_Receipt    from lptbreceipts r" +
////"  inner join lptbreceiptitems i on r.receiptid=i.receiptid inner join lptbreceiptbatches b on b.receiptitemid=i.receiptitemid" +
////"  inner join lpmasItems m1 on m1.lpitemid=i.lpitemid " +
////"  left outer join  " +
////"  (    select b.itemid, facilityid,sum(case when a.status='C' then nvl(ow.issueqty,0) end) issueqty," +
////"  sum(case when a.status='IN' then nvl(ow.issueqty,0) end) ReservedQty  from tbfacilityissues a " +
////"  inner join tbfacilityissueitems b on a.issueid=b.issueid " +
////"  Left Outer Join tbFacilityOutwards ow on (ow.IssueItemID=b.IssueItemID) " +
////"  Left Outer Join tbReceiptBatches rb on ow.inwno=rb.inwno and rb.Inwno=ow.Inwno  group by b.itemid, facilityid     ) iss" +
////"  on (iss.itemid=i.LPItemID and iss.facilityid=r.facilityid )" +
////"  Left Outer Join " +
////"  (     select i.LPItemID,r.FacilityID, SUm(nvl(b.AbsRQty,0)) WardReturnQty    from lptbReceipts r   " +
////"  inner join lptbReceiptItems i on r.ReceiptID=i.ReceiptID " +
////"  inner join lptbReceiptBatches b on b.ReceiptItemId=i.ReceiptItemId  " +
////"  where r.status='C' and ReasonFlag='Y' and ReceiptType='RW'" +
////"  group by i.LPItemID,r.FacilityID  )WR on WR.LPItemID=m1.LPItemID and WR.FacilityID=r.FacilityID " +



////"  Where  r.ReceiptType='NO' and r.status='C' and expdate>sysdate+29 and r.FacilityID= " + facilityid + "   and i.LPItemID=" + mItemID + " " +
////"  group by m1.LPItemID,ItemName,iss.issueqty,iss.ReservedQty,WR.WardReturnQty,m1.unitcount ,m1.multiple,m1.itemcode" +
////"  ) alldata     " +
////"  group by multiple,ItemID,itemname,unitcount,itemcode" +
////"  ) alldata1 " +
////"   where alldata1.ItemID=" + mItemID + " and (CurBalN+opM+WardReturnQtyM+FTF_Receipt-(ReservedQtyM+issueqtyM))>0  " +
////"  order by itemname ";

//            string strSQL = " select itemid,facilityid,sum(absqty) as ReceiptQTY,sum(issueqty) issueqty,sum(ReservedQty) as ReservedQty,0 as WardReturnQty ,(sum(absqty)-(sum(issueqty)+sum(ReservedQty))) as CurStock " +
//" from ( " +
//" select  nvl(tbrr.absrqty,0) absqty,0 issueqty,0 ReservedQty, tfri.itemid,tfr.facilityid   " +
//" from tbfacilityreceiptbatches tbrr " +
//" inner join tbFacilityReceiptItems tfri on tfri.facreceiptitemid=tbrr.facreceiptitemid " +
//" inner join tbfacilityreceipts tfr on tfr.facreceiptid=tfri.facreceiptid " +
//" where tfr.status='C' and tbrr.ExpDate >sysdate and tfr.facilityid=" + facilityid + " and tfri.itemid=" + mItemID + " " +
//" union " +
//" select 0 as absqty, case when tbo.status='C' then nvl(sum(tbr.issueqty),0) end issueqty, " +
//" case when tbo.status='IN' then nvl(nvl(sum(tbr.allotqty),0)-nvl(sum(tbr.issueqty),0),0) end ReservedQty ,tbi.itemid,tb.facilityid " +
//" from tbfacilityoutwards tbo  " +
//" inner join tbfacilityissueitems tbi on tbi.issueitemid=tbo.issueitemid " +
//" inner join tbfacilityissues tb on tb.issueid=tbi.issueid " +
//" inner join tbfacilityreceiptbatches tbr on tbr.inwno=tbo.inwno " +
//" where tb.facilityid=" + facilityid + " and tbi.itemid=" + mItemID + " " +
//" group by tbi.itemid,tb.facilityid,tbo.status,tbr.issueqty,tbr.allotqty " +
//" ) alldata where alldata.facilityid=" + facilityid + " group by itemid,facilityid ";
//            DataTable dt = DBHelper.GetDataTable(strSQL);
//            double stock = 0;
//            if (dt.Rows.Count > 0 && dt.Rows[0]["CurStock"] != DBNull.Value)
//            {
//                double r=double.Parse(dt.Rows[0]["ReservedQty"].ToString());
//                double ret = double.Parse(dt.Rows[0]["WardReturnQty"].ToString());
//              double s=  double.Parse(dt.Rows[0]["CurStock"].ToString());
//             // stock = (s+ret) - r;
//              return s;
//            }

//            else
//                return 0;
//        }   

 public static double GetFacilityStock(string mFacilityID, string mSourceID, string mSchemeID, string mItemID)
        {
            string facilityid = mFacilityID;

//            string strSQL = " select itemid,facilityid,sum(absqty) as ReceiptQTY,sum(issueqty) issueqty,sum(ReservedQty) as ReservedQty,0 as WardReturnQty ,(sum(absqty)-(sum(issueqty)+sum(ReservedQty))) as CurStock " +
//" from ( " +
//" select  nvl(sum(tbrr.absrqty),0) absqty,0 issueqty,0 ReservedQty, tfri.itemid,tfr.facilityid   " +
//" from tbfacilityreceiptbatches tbrr " +
//" inner join tbFacilityReceiptItems tfri on tfri.facreceiptitemid=tbrr.facreceiptitemid " +
//" inner join tbfacilityreceipts tfr on tfr.facreceiptid=tfri.facreceiptid " +
//" where tfr.status='C' and tbrr.ExpDate >sysdate and tfr.facilityid=" + facilityid +" and tfri.itemid=" + mItemID + " "+
//" group  by tfri.itemid,tfr.facilityid  " +
//" union " +
//" select 0 as absqty, case when tbo.status='C' then nvl(sum(tbo.issueqty),0) end issueqty, " +
//" case when tbo.status='IN' then nvl(sum(tbo.issueqty),0) end ReservedQty ,tbi.itemid,tb.facilityid " +
//" from tbfacilityoutwards tbo  " +
//" inner join tbfacilityissueitems tbi on tbi.issueitemid=tbo.issueitemid " +
//" inner join tbfacilityissues tb on tb.issueid=tbi.issueid " +
//"  inner join tbfacilityreceiptbatches tbr on tbr.inwno=tbo.inwno  " +
//" where  tbr.ExpDate >sysdate and tb.facilityid=" + facilityid + " and tbi.itemid=" + mItemID + " " +
//" group by tbi.itemid,tb.facilityid,tbo.status " +
//" ) alldata where alldata.facilityid=" + facilityid + " group by itemid,facilityid ";

  int days=15;
  
  

            string strSQL = @"select  r.FacilityID,i.itemid,0 ReservedQty,0 WardReturnQty,sum(nvl(rb.AbsRqty,0)) AbsRQty, sum(nvl(x.issueQty,0)) issueQty  , sum(nvl(rb.AbsRqty,0))- sum(nvl(x.issueQty,0)) CurStock
from tbFacilityReceiptBatches rb  
inner join tbfacilityreceiptitems i on rb.FACreceiptitemid=i.FACreceiptitemid  
inner join tbfacilityreceipts r on r.FACreceiptid=i.FACreceiptid 
left outer join (
  select  FacilityID,sum(nvl(a.IssueQty,0)) issueQty, 0 AbsRQty,  
a.Inwno   
from  tbfacilityoutwards a
inner join tbfacilityissueitems tbi on tbi.issueitemid =a.issueitemid   
inner join tbfacilityissues tb on tb.issueid=tbi.issueid    
where tb.status in ('C','IN')  and tb.FacilityID=" + facilityid + @" and tbi.itemid=" + mItemID + @"group by a.Inwno ,FacilityID   
) x on x.inwno=rb.inwno and x.FacilityID=r.FacilityID
where rb.qastatus=1 and   (rb.ExpDate >= SysDate or nvl(rb.ExpDate,SysDate) >= SysDate) and r.status='C' and round(rb.ExpDate-SysDate,0) >= "+days+" and r.FacilityID=" + facilityid + @" and i.itemid=" + mItemID + "  and (rb.Whissueblock = 0 or rb.Whissueblock is null) group by r.FacilityID,i.itemid";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            double stock = 0;
            if (dt.Rows.Count > 0 && dt.Rows[0]["CurStock"] != DBNull.Value)
            {
                double r = double.Parse(dt.Rows[0]["ReservedQty"].ToString());
                double ret = double.Parse(dt.Rows[0]["WardReturnQty"].ToString());
              double s=  double.Parse(dt.Rows[0]["CurStock"].ToString());
             // stock = (s+ret) - r;
              return s;
            }

            else
                return 0;
        }




 public static double GetFacilityStockReturnToWH(string mFacilityID, string mSourceID, string mSchemeID, string mItemID)
 {
     string facilityid = mFacilityID;

     //            string strSQL = " select itemid,facilityid,sum(absqty) as ReceiptQTY,sum(issueqty) issueqty,sum(ReservedQty) as ReservedQty,0 as WardReturnQty ,(sum(absqty)-(sum(issueqty)+sum(ReservedQty))) as CurStock " +
     //" from ( " +
     //" select  nvl(sum(tbrr.absrqty),0) absqty,0 issueqty,0 ReservedQty, tfri.itemid,tfr.facilityid   " +
     //" from tbfacilityreceiptbatches tbrr " +
     //" inner join tbFacilityReceiptItems tfri on tfri.facreceiptitemid=tbrr.facreceiptitemid " +
     //" inner join tbfacilityreceipts tfr on tfr.facreceiptid=tfri.facreceiptid " +
     //" where tfr.status='C' and tbrr.ExpDate >sysdate and tfr.facilityid=" + facilityid +" and tfri.itemid=" + mItemID + " "+
     //" group  by tfri.itemid,tfr.facilityid  " +
     //" union " +
     //" select 0 as absqty, case when tbo.status='C' then nvl(sum(tbo.issueqty),0) end issueqty, " +
     //" case when tbo.status='IN' then nvl(sum(tbo.issueqty),0) end ReservedQty ,tbi.itemid,tb.facilityid " +
     //" from tbfacilityoutwards tbo  " +
     //" inner join tbfacilityissueitems tbi on tbi.issueitemid=tbo.issueitemid " +
     //" inner join tbfacilityissues tb on tb.issueid=tbi.issueid " +
     //"  inner join tbfacilityreceiptbatches tbr on tbr.inwno=tbo.inwno  " +
     //" where  tbr.ExpDate >sysdate and tb.facilityid=" + facilityid + " and tbi.itemid=" + mItemID + " " +
     //" group by tbi.itemid,tb.facilityid,tbo.status " +
     //" ) alldata where alldata.facilityid=" + facilityid + " group by itemid,facilityid ";

     string strSQL = @"select  r.FacilityID,i.itemid,0 ReservedQty,0 WardReturnQty,sum(nvl(rb.AbsRqty,0)) AbsRQty, sum(nvl(x.issueQty,0)) issueQty  , sum(nvl(rb.AbsRqty,0))- sum(nvl(x.issueQty,0)) CurStock
from tbFacilityReceiptBatches rb  
inner join tbfacilityreceiptitems i on rb.FACreceiptitemid=i.FACreceiptitemid  
inner join tbfacilityreceipts r on r.FACreceiptid=i.FACreceiptid 
left outer join (
  select  FacilityID,sum(nvl(a.IssueQty,0)) issueQty, 0 AbsRQty,  
a.Inwno   
from  tbfacilityoutwards a
inner join tbfacilityissueitems tbi on tbi.issueitemid =a.issueitemid   
inner join tbfacilityissues tb on tb.issueid=tbi.issueid    
where tb.status='C'  and tb.FacilityID=" + facilityid + @" and tbi.itemid=" + mItemID + @"group by a.Inwno ,FacilityID   
) x on x.inwno=rb.inwno and x.FacilityID=r.FacilityID
where    (rb.ExpDate >= SysDate or nvl(rb.ExpDate,SysDate) >= SysDate) and r.status='C' and r.FacilityID=" + facilityid + @" and i.itemid=" + mItemID + "   group by r.FacilityID,i.itemid";
     DataTable dt = DBHelper.GetDataTable(strSQL);
     double stock = 0;
     if (dt.Rows.Count > 0 && dt.Rows[0]["CurStock"] != DBNull.Value)
     {
         double r = double.Parse(dt.Rows[0]["ReservedQty"].ToString());
         double ret = double.Parse(dt.Rows[0]["WardReturnQty"].ToString());
         double s = double.Parse(dt.Rows[0]["CurStock"].ToString());
         // stock = (s+ret) - r;
         return s;
     }

     else
         return 0;
 }



        #endregion

        #region GetFacilityStockSKU
        public static double GetFacilityStockSKU(string mFacilityID, string mSourceID, string mSchemeID, string mItemID)
        {

            string strSQL = " Select  0 as SlNo,nvl(m1.multiple,1) multiple,m1.ItemID,m1.itemname, " +
      " ((NVL(stk.CurStock,0)+ nvl(GET_FOPstock(" + mFacilityID + ",m1.itemid),0))-((case when nvl(cum6.IssueQty,0)>0 then nvl(cum6.IssueQty,0)/nvl(m1.unitcount,0) else 0 end)+ (case when (nvl(stk.Reserved,0))>0 then  (nvl(stk.Reserved,0))/nvl(m1.unitcount,0) else 0 end))+(nvl(WR.WardReturnQty,0)/nvl(m1.unitcount,0)))  CurStock, " +

      " ((case when nvl(m1.unitcount,0)>0 and (nvl(stk.Reserved,0))>0 " +
      " then  (nvl(stk.Reserved,0))/nvl(m1.unitcount,0) else 0 end))  ReservedQty, " +
      " ((case when nvl(m1.unitcount,0)>0 and nvl(cum6.IssueQty,0)>0 then nvl(cum6.IssueQty,0)/nvl(m1.unitcount,0) else 0 end))  as CumlMon6," +
      " (nvl(WR.WardReturnQty,0)) WardReturnQty " +

                 " From masItems m1" +
                 " Left Outer Join v_FacilityStock stk on (stk.ItemID=m1.ItemID and stk.FacilityID=" + mFacilityID + ")" +
                 " Left Outer Join (Select rc.FacilityID,rcit.ItemID, sum(case when rc.Status='C' then (ow.IssueQty) else 0 end) IssueQty ,sum(case when rc.Status='IN' then (ow.IssueQty) else 0 end) ReservedQty_OP " +
                 " From tbFacilityIssues rc" +
                 " Inner Join tbFacilityIssueItems rcit on (rc.IssueID=rcit.IssueID)" +
                 " Left Outer Join tbFacilityOutwards ow on (ow.IssueItemID=rcit.IssueItemID)" +
                 " Where rc.IssueDate > Add_Months(sysdate,-6 ) " +
                 " Group by rc.FacilityID,rcit.ItemID) cum6 on (cum6.FacilityID=" + mFacilityID + " and cum6.ItemID=m1.ItemID)" +
                 " Left Outer Join " +
                 " ( " +
                 "    Select r.FacilityID,i.ItemID,sum(nvl(i.AbsRQty,0)) WardReturnQty  from " +
                 "    tbFacilityReceipts r " +
                 "    inner join tbFacilityReceiptItems i on r.FacReceiptID=i.FacReceiptID " +
                 "    where r.FacReceiptType='RW' and r.status='C' and ReasonFlag='Y' group by r.FacilityID,i.ItemID " +
                 " ) WR on WR.ItemID=stk.ItemID and WR.facilityID=stk.facilityID " +
                 " Where 1=1 and m1.ItemID = " + mItemID +
                 " union " +
                 " Select 0 as SlNo,nvl(m1.multiple,1) multiple,m1.LPItemID,ItemName,(sum(nvl(b.AbsRQty,0))-(nvl(iss.issueqty,0)/nvl(m1.unitcount,0)+ nvl(iss.ReservedQty,0)/nvl(m1.unitcount,0)) + nvl(WardReturnQty,0)/nvl(m1.unitcount,0)) * nvl(m1.unitcount,1) CurBal, " +
                 " ((case when nvl(iss.ReservedQty ,0)>0 then nvl(iss.ReservedQty ,0)/nvl(m1.unitcount,0) else 0 end)) * nvl(m1.unitcount,1) ReservedQty,((case when nvl(iss.IssueQty ,0)>0 then nvl(iss.IssueQty,0)/nvl(m1.unitcount,0) else 0 end)) * nvl(m1.unitcount,1) IssueQty,((case when nvl(WardReturnQty ,0)>0 then nvl(WardReturnQty,0)/nvl(m1.unitcount,0) else 0 end))* nvl(m1.unitcount,1)   WardReturnQty   " +
                 " from lptbreceipts r inner join lptbreceiptitems i on r.receiptid=i.receiptid inner join lptbreceiptbatches b on b.receiptitemid=i.receiptitemid inner join lpmasItems m1 on m1.lpitemid=i.lpitemid" +
                 " left outer join     " +
                 "(   " +
                      " select b.itemid, facilityid,sum(case when a.status='C' then nvl(ow.issueqty,0) end) issueqty, sum(case when a.status='IN' then nvl(ow.issueqty,0) end) ReservedQty " +
                      " from tbfacilityissues a inner join tbfacilityissueitems b on a.issueid=b.issueid " +
                      " Left Outer Join tbFacilityOutwards ow on (ow.IssueItemID=b.IssueItemID)  Left Outer Join tbReceiptBatches rb on ow.inwno=rb.inwno and rb.Inwno=ow.Inwno " +
                      " group by b.itemid, facilityid    " +
                 " ) iss on (iss.itemid=i.LPItemID and iss.facilityid=r.facilityid )" +
                 " Left Outer Join " +
                 " ( " +
                 "    select i.LPItemID,r.FacilityID, SUm(nvl(b.AbsRQty,0)) WardReturnQty " +
                 "   from lptbReceipts r" +
                 "    inner join lptbReceiptItems i on r.ReceiptID=i.ReceiptID " +
                 "    inner join lptbReceiptBatches b on b.ReceiptItemId=i.ReceiptItemId" +
                 "    where r.status='C' and ReasonFlag='Y' and ReceiptType='RW' group by i.LPItemID,r.FacilityID" +
                 "  )WR on WR.LPItemID=m1.LPItemID and WR.FacilityID=r.FacilityID" +
                 " Where  r.ReceiptType='NO' and r.status='C' and expdate>sysdate+29 and r.FacilityID= " + mFacilityID + " and  m1.LPItemID =" + mItemID + " group by m1.LPItemID,ItemName,iss.issueqty,iss.ReservedQty,WR.WardReturnQty,m1.unitcount ,m1.multiple Order by ItemName";

            //old stock code

            //           string strSQL = " Select ((Sum(NVL(b.CurStock,0))+ nvl(GET_FOPstock(" + mFacilityID + "," + mItemID + "),0)) -(((nvl(iss.issueqty,0))/m1.unitcount)+(nvl(iss.ReservedQty,0))/m1.unitcount) + (nvl(WardReturnQty,0))/m1.unitcount)* m1.unitcount CurStock from masitems m1 left outer join v_FacilityStock b on (b.ItemID=m1.ItemID and b.facilityid=" + mFacilityID + " and b.itemid=" + mItemID + ") left outer join " +
            //" (  select itemid, facilityid,sum(case when a.status='C' then nvl(issueqty,0) end) issueqty,sum(case when a.status='IN' then nvl(issueqty,0) end) ReservedQty  " +
            //"  from tbfacilityissues a  inner join tbfacilityissueitems b on a.issueid=b.issueid group by itemid, facilityid ) iss on iss.itemid=b.itemid and iss.facilityid=b.facilityid " +
            //" Left Outer Join " +
            //" ( " +
            //"    Select r.FacilityID,i.ItemID,sum(nvl(i.AbsRQty,0)) WardReturnQty  from " +
            //"    tbFacilityReceipts r " +
            //"    inner join tbFacilityReceiptItems i on r.FacReceiptID=i.FacReceiptID " +
            //"    where r.FacReceiptType='RW' and r.status='C' and ReasonFlag='Y' group by r.FacilityID,i.ItemID " +
            //" ) WR on WR.ItemID=b.ItemID and WR.facilityID=b.facilityID " +

            //" group by iss.issueqty,iss.ReservedQty,WardReturnQty,m1.unitcount " +

            //" Union " +
            //" Select  (sum(nvl(b.absrqty,0))-(nvl(iss.issueqty,0)/nvl(m1.unitcount,0)+ nvl(iss.ReservedQty,0)/nvl(m1.unitcount,0)) + nvl(WardReturnQty,0)/nvl(m1.unitcount,0)) * m1.unitcount CurStock from lptbreceipts r inner join lptbreceiptitems i on r.receiptid=i.receiptid    inner join lpmasItems m1 on m1.lpitemid=i.lpitemid " +
            //" inner join lptbreceiptbatches b on b.receiptItemID=i.receiptItemID " +
            //" left outer join   " +
            //" (   " +
            //" select itemid, facilityid,sum(case when a.status='C' then nvl(issueqty,0) end) issueqty, " +
            //" sum(case when a.status='IN' then nvl(issueqty,0) end) ReservedQty    " +
            //" from tbfacilityissues a inner join tbfacilityissueitems b on a.issueid=b.issueid group by itemid, facilityid " +
            //" ) iss on (iss.itemid=i.LPItemID and iss.facilityid=r.facilityid ) " +
            //" Left Outer Join " +
            //" ( " +
            //"   select i.LPItemID,r.FacilityID, SUm(nvl(b.AbsRQty,0)) WardReturnQty " +
            //"   from lptbReceipts r" +
            //"   inner join lptbReceiptItems i on r.ReceiptID=i.ReceiptID " +
            //"   inner join lptbReceiptBatches b on b.ReceiptItemId=i.ReceiptItemId" +
            //"   where r.status='C' and ReasonFlag='Y' and ReceiptType='RW' group by i.LPItemID,r.FacilityID" +
            //"  )WR on WR.LPItemID=i.LPItemID and WR.FacilityID=r.FacilityID" +
            //" Where r.ReceiptType='NO' and r.facilityid=" + mFacilityID + " and i.LPItemID=" + mItemID + " group by iss.issueqty,iss.ReservedQty,WardReturnQty,m1.unitcount";



            DataTable dt = DBHelper.GetDataTable(strSQL);
            double stock = 0;
            if (dt.Rows.Count > 0 && dt.Rows[0]["CurStock"] != DBNull.Value)
            {
                double r = double.Parse(dt.Rows[0]["ReservedQty"].ToString());
                double ret = double.Parse(dt.Rows[0]["WardReturnQty"].ToString());
                double s = double.Parse(dt.Rows[0]["CurStock"].ToString());
                // stock = (s+ret) - r;
                return s;
            }

            else
                return 0;
        }
        #endregion

        #region UpdateAllotQty Local Purchase
        public static void LPUpdateAllotQty(string WarehouseID, string SourceID, string SchemeID, string ItemID, double AllotQty, string IndentItemID)
        {
            //Allow to distribute to facility only when QAStatus=pass from batches (tbReceiptBatches)
            string WhereConditions = " and b.QAStatus = 1 and (b.ExpDate >= SysDate or nvl(b.ExpDate,SysDate) >= SysDate)";

            #region Delete existing allotements (tbOutwards)
            string strSQL = "Delete from LPtbOutWards where IndentItemID = " + IndentItemID;
            DBHelper.ExecuteNonQuery(strSQL);
            #endregion

            #region Insert new allotements (tbOutwards)
            double TotAllotQty = AllotQty;
            //strSQL = "Select b.InwNo, b.BatchNo, b.MfgDate, b.ExpDate, NVL(b.AbsRQty, 0) as AbsRQty, NVL(b.AllotQty, 0) as AllotQty" +
            //    " from tbReceiptBatches b" +
            //    " Inner Join tbReceiptItems a on (a.ReceiptItemID = b.ReceiptItemID)" +
            //    " Inner Join tbReceipts a1 on (a1.ReceiptID = a.ReceiptID)" +
            //    " Where a1.WarehouseID = " + WarehouseID + " and a1.SourceID = " + SourceID + " and a1.SchemeID = " + SchemeID +
            //    "   and a.ItemID = " + ItemID + WhereConditions + " and NVL(b.AbsRQty, 0) - NVL(b.AllotQty, 0) > 0 and b.BatchStatus='R'" +
            //    " Order By b.ExpDate, b.AbsRQty";

            strSQL = "Select a.ReceiptItemID, NVL(a.AbsRQty, 0) as AbsRQty, NVL(a.AllotQty, 0) as AllotQty" +
               " from  tbReceiptItems a " +
               " Inner Join tbReceipts a1 on (a1.ReceiptID = a.ReceiptID)" +
               " Where a1.WarehouseID = " + WarehouseID + " and a1.SourceID = " + SourceID + " and a1.SchemeID = " + SchemeID +
               "   and a.ItemID = " + ItemID + WhereConditions + " and NVL(a.AbsRQty, 0) - NVL(a.AllotQty, 0) > 0 and b.BatchStatus='R'" +
               " Order By b.ExpDate, b.AbsRQty";


            DataTable dt = DBHelper.GetDataTable(strSQL);

            foreach (DataRow dr in dt.Rows)
            {
                #region Calculate Allotted Qty
                double dblCurAllot = 0;
                double dblAbsRQty = double.Parse(dr["AbsRQty"].ToString());
                double dblAllotQty = double.Parse(dr["AllotQty"].ToString());
                if (dblAbsRQty - dblAllotQty >= TotAllotQty)
                    dblCurAllot = TotAllotQty;
                else
                    dblCurAllot = dblAbsRQty - dblAllotQty;
                TotAllotQty -= dblCurAllot;
                #endregion

                #region Data to be Saved
                string mIndentItemID = IndentItemID;
                string mItemID = ItemID;
                string mInwNo = dr["InwNo"].ToString();
                string mIssueQty = dblCurAllot.ToString();
                string mIssuePrice = "0";
                string mIssued = "0";
                #endregion

                #region Validate Data
                GenFunctions.CheckNumber(ref mIndentItemID, "", false);
                GenFunctions.CheckNumber(ref mItemID, "", false);
                GenFunctions.CheckNumber(ref mInwNo, "", false);
                //GenFunctions.CheckStringForEmpty(ref mIssueType, "", false);
                //GenFunctions.CheckDate(ref mIssueDate, "", false);
                GenFunctions.CheckNumber(ref mIssueQty, "", false);
                GenFunctions.CheckNumber(ref mIssuePrice, "", false);
                GenFunctions.CheckNumber(ref mIssued, "", false);
                #endregion

                #region Save Data
                strSQL = "Insert into LPtbOutWards (IndentItemID, ItemID, InwNo, IssueQty, IssuePrice, Issued) values (" + mIndentItemID + ", " + mItemID + ", " + mInwNo + ", " + mIssueQty + ", " + mIssuePrice + ", " + mIssued + ")";
                DBHelper.GetDataTable(strSQL);
                #endregion

                if (TotAllotQty == 0) break;
            }
            #endregion
        }
        #endregion
    }
    #endregion
}
