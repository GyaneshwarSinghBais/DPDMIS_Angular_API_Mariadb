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
    #region Receipts
    public class Receipts
    {
        #region FillSourcesByReceipts
        public static void FillSourcesByReceipts(DropDownList ddl, string WarehouseID, bool AddAllSources, bool SelectFirst)
        {
            ddl.Items.Clear();
            string strSQL = "Select SourceID, SourceName from masSources where SourceID in (Select SourceID from tbReceipts a where a.WarehouseID = " + WarehouseID + " and a.ReceiptType = 'NO') Order By SourceName";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            ddl.DataTextField = "SourceName";
            ddl.DataValueField = "SourceID";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (AddAllSources)
                ddl.Items.Insert(0, new ListItem("All Sources", "0"));

            if (ddl.Page.Session["usrSourceID"] != null)
            {
                ddl.SelectedValue = ddl.Page.Session["usrSourceID"].ToString();
                ddl.Enabled = false;
            }
            else if (SelectFirst && ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }
        #endregion

        #region FillSchemesByReceipts
        public static void FillSchemesByReceipts(DropDownList ddl, string WarehouseID, string SourceID, bool AddAllSchemes, bool SelectFirst)
        {
            ddl.Items.Clear();
            string strSQL = "Select SchemeID, SchemeName from masSchemes Where SchemeID in (Select SchemeID from tbReceipts a where a.WarehouseID = " + WarehouseID + " and a.SourceID = " + SourceID + " and a.ReceiptType = 'NO') Order By SchemeName";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            ddl.DataTextField = "SchemeName";
            ddl.DataValueField = "SchemeID";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (AddAllSchemes)
                ddl.Items.Insert(0, new ListItem("All Schemes", "0"));

            if (ddl.Page.Session["usrSchemeID"] != null)
            {
                ddl.SelectedValue = ddl.Page.Session["usrSchemeID"].ToString();
                ddl.Enabled = false;
            }
            else if (SelectFirst && ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }
        #endregion

        #region Fill Batches BySource, Scheme and Item
        public static void FillBatchesBySourceBySchemeByItem(DropDownList ddl, string SourceID, string SchemeID, string ItemID, 
            bool AddAllBatches, bool SelectFirst)
        {
            ddl.Items.Clear();
            string strSQL = "Select rb.InwNo,rb.BatchNo" +
                " from tbReceiptBatches rb" +
                " Inner Join tbReceiptItems ri on (ri.ReceiptItemID=rb.ReceiptItemID)" +
                " Inner Join tbReceipts rct on (rct.ReceiptID=ri.ReceiptID)" +
                " inner join masItems m1 on (m1.ItemID = rb.ItemID)" +
                " Where rb.Status not in ('IN') and m1.QcTest = 'Y' and rb.BatchStatus = 'R' and rb.QAIssued=0 and rb.QAStatus=0" +
                "   and rct.ReceiptType in ('NO') and rct.SourceID=" + SourceID + " and rct.SchemeID=" + SchemeID + " and rb.ItemID=" + ItemID +
                " Order By BatchNo";
            DataTable dt = DBHelper.GetDataTable(strSQL);

            ddl.DataTextField = "BatchNo";
            ddl.DataValueField = "InwNo";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (AddAllBatches)
                ddl.Items.Insert(0, new ListItem("All Batches", "0"));

            if (SelectFirst && ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }
        #endregion

        #region CheckAndCompleteReceipt
        public static bool CheckAndCompleteReceipt(string mReceiptID, ref string ErrMsg, bool CheckVoucherAvailable)
        {
            GenFunctions.Issues.IssueAllExpired();
            ErrMsg = string.Empty;
            if (mReceiptID.Trim() == string.Empty || mReceiptID.Trim() == "0")
                ErrMsg += "Invalid Receipt No.<br />";
            else
            {
                string strSQL = "Select a.ReceiptItemID, a.ReceiptAbsQty from tbReceiptItems a Where a.ReceiptID = " + mReceiptID;
                DataTable dt = DBHelper.GetDataTable(strSQL);
                if (dt.Rows.Count == 0)
                    ErrMsg += "Add atleast one item under items tab. Status cannot be changed.<br />";

              strSQL = "Select a.ReceiptItemID, a.ReceiptAbsQty from tbReceiptItems a Left Outer Join tbReceiptBatches b on (b.ReceiptItemID = a.ReceiptItemID) Where a.ReceiptID = " + mReceiptID + " Group By a.ReceiptItemID, a.ReceiptAbsQty Having (NVL(a.ReceiptAbsQty, 0) = 0 or NVL(a.ReceiptAbsQty, 0) != NVL(Sum(NVL(b.AbsRQty, 0) + NVL(b.ShortQty, 0)), 0))";
              

  dt = DBHelper.GetDataTable(strSQL);
                if (dt.Rows.Count > 0)
                    ErrMsg += "All items should have atleast one batch with quantity greater than zero. Status cannot be changed.<br />";

                strSQL = "Select a.ReceiptItemID, a.ReceiptAbsQty from tbReceiptItems a Left Outer Join tbReceiptBatches b on (b.ReceiptItemID = a.ReceiptItemID) Where a.ReceiptID = " + mReceiptID + " Group By a.ReceiptItemID, a.ReceiptAbsQty Having NVL(a.ReceiptAbsQty, 0) != NVL(Sum(NVL(b.AbsRQty, 0) + NVL(b.ShortQty, 0)), 0)";
                dt = DBHelper.GetDataTable(strSQL);
                if (dt.Rows.Count > 0)
                    ErrMsg += "Receipt Qty and Total Batch Qty should be same. Status cannot be changed.<br />";

                strSQL = "Select r2.ItemID, m1.ItemName, r1.ReceiptDate, Min(r3.ExpDate) as ExpDate from tbReceiptBatches r3 Inner Join tbReceiptItems r2 on (r2.ReceiptItemID = r3.ReceiptItemID) Inner Join tbReceipts r1 on (r1.ReceiptID = r2.ReceiptID) Inner Join masItems m1 on (m1.ItemID = r2.ItemID) Where 1=1 and r3.ExpDate is not null and r1.ReceiptID = " + mReceiptID + " GROUP BY r2.ItemID, m1.ItemName, r1.ReceiptDate Having r1.ReceiptDate > Min(r3.ExpDate)";
                dt = DBHelper.GetDataTable(strSQL);
                if (dt.Rows.Count > 0)
                    ErrMsg += "Batch expiry dates should be greater than receipt date. Check Item " + dt.Rows[0]["ItemName"].ToString() + ". Status cannot be changed.<br />";

                //Following SQL checks whether received qty is greater than ordered receivable quantity (Only in case on supplier receipts)
                strSQL = "Select a.OrderItemID, a.WarehouseID, b.ItemID, m1.ItemName, NVL(a.AbsQty, 0) as OrderQty, NVL(a.AbsQty, 0) - NVL(a.ReceiptAbsQty, 0) as Receivable, NVL(b.ReceiptAbsQty, 0) as ReceiptQty from soWHDist a Inner Join tbReceiptItems b on (b.OrderItemID = a.OrderItemID) Inner Join tbReceipts c on (c.WarehouseID = a.WarehouseID and c.ReceiptID = b.ReceiptID) Inner Join masItems m1 on (m1.ItemID = b.ItemID) Where c.ReceiptID = " + mReceiptID + " and b.OrderItemID is not null and c.ReceiptType in ('NO') and NVL(a.ReceiptAbsQty, 0) + NVL(b.ReceiptAbsQty, 0) > NVL(a.AbsQty, 0)";
                dt = DBHelper.GetDataTable(strSQL);
                if (dt.Rows.Count > 0)
                    ErrMsg += "Receipt item " + dt.Rows[0]["ItemName"].ToString() + " has receipt quantity greater than receivable quantity. Status cannot be changed.<br />";

                if (CheckVoucherAvailable)
                {
                    strSQL = "Select StkRegDate, StkRegNo from tbReceipts Where (stkRegNo is null or stkRegDate is null) and ReceiptID = " + mReceiptID;
                    dt = DBHelper.GetDataTable(strSQL);
                    if (dt.Rows.Count > 0)
                        ErrMsg += "Voucher No. and Voucher Date. should not be empty and should be saved before completing.<br />";
                }
            }
            if (ErrMsg == string.Empty)
            {
                string mReceiptType = "";
                string strSQL1 = "Select ReceiptType from tbReceipts where ReceiptID = " + mReceiptID;
                DataTable dt1 = DBHelper.GetDataTable(strSQL1);
                if (dt1.Rows.Count > 0)
                {
                    mReceiptType = dt1.Rows[0]["ReceiptType"].ToString();
                }
                if(mReceiptType !="NO")
                {
                    string strQuery = "Update tbReceipts set Status = 'C' Where ReceiptID = " + mReceiptID;
                    DBHelper.ExecuteNonQuery(strQuery);
                    strQuery = "Update tbReceiptItems set Status = 'C' Where ReceiptID = " + mReceiptID;
                    DBHelper.ExecuteNonQuery(strQuery);
                    strQuery = "Update tbReceiptBatches set Status = 'C' Where ReceiptItemID in (Select ReceiptItemID from tbReceiptItems Where ReceiptID = " + mReceiptID + ")";
                    DBHelper.ExecuteNonQuery(strQuery);
                }
                return true;
            }
            else
                return false;
        }
        #endregion

        #region DeleteReceipt
        public static bool DeleteReceipt(string mReceiptID, ref string ErrMsg)
        {
            ErrMsg = string.Empty;
            if (mReceiptID.Trim() == string.Empty || mReceiptID.Trim() == "0")
                ErrMsg += "Invalid Receipt No.<br />";
            else
            {
                try
                {
                    string strSQL = "Delete from tbReceiptBatches Where ReceiptItemID in (Select ReceiptItemID from tbReceiptItems Where ReceiptID = " + mReceiptID + ")";
                    DBHelper.ExecuteNonQuery(strSQL);
                    strSQL = "Delete from tbReceiptItems Where ReceiptID = " + mReceiptID;
                    DBHelper.ExecuteNonQuery(strSQL);
                    strSQL = "Delete from tbReceipts where ReceiptID = " + mReceiptID;
                    DBHelper.ExecuteNonQuery(strSQL);
                }
                catch
                {
                    ErrMsg += "Delete not allowed, references found.<br />";
                }
            }
            return (ErrMsg == string.Empty);
        }
        #endregion

        #region [Sub Class] Facility Receipts
        public class Facility
        {
            #region FillSourcesByFacilityReceipts
            public static void FillSourcesByFacilityReceipts(DropDownList ddl, string FacilityID, bool AddAllSources, bool SelectFirst)
            {
                ddl.Items.Clear();
                string strSQL = "Select SourceID,SourceName" +
                    " from masSources" +
                    " where SourceID in (Select SourceID from tbFacilityReceipts a where a.FacilityID=" + FacilityID +
                    "       and a.ReceiptType='NO')" +
                    " Order By SourceName";
                DataTable dt = DBHelper.GetDataTable(strSQL);
                ddl.DataTextField = "SourceName";
                ddl.DataValueField = "SourceID";
                ddl.DataSource = dt;
                ddl.DataBind();
                if (AddAllSources)
                    ddl.Items.Insert(0, new ListItem("All Sources", "0"));

                if (ddl.Page.Session["usrSourceID"] != null)
                {
                    ddl.SelectedValue = ddl.Page.Session["usrSourceID"].ToString();
                    ddl.Enabled = false;
                }
                else if (SelectFirst && ddl.Items.Count > 0)
                    ddl.SelectedIndex = 0;
            }
            #endregion

            #region FillSchemesByFacilityReceipts
            public static void FillSchemesByFacilityReceipts(DropDownList ddl, string FacilityID, string SourceID, bool AddAllSchemes, bool SelectFirst)
            {
                ddl.Items.Clear();
                string strSQL = "Select SchemeID,SchemeName" +
                    " from masSchemes" +
                    " Where SchemeID in (Select SchemeID from tbFacilityReceipts a where a.FacilityID=" + FacilityID +
                    "       and a.SourceID=" + SourceID + " and a.ReceiptType='NO')" +
                    " Order By SchemeName";
                DataTable dt = DBHelper.GetDataTable(strSQL);
                ddl.DataTextField = "SchemeName";
                ddl.DataValueField = "SchemeID";
                ddl.DataSource = dt;
                ddl.DataBind();
                if (AddAllSchemes)
                    ddl.Items.Insert(0, new ListItem("All Schemes", "0"));

                if (ddl.Page.Session["usrSchemeID"] != null)
                {
                    ddl.SelectedValue = ddl.Page.Session["usrSchemeID"].ToString();
                    ddl.Enabled = false;
                }
                else if (SelectFirst && ddl.Items.Count > 0)
                    ddl.SelectedIndex = 0;
            }
            #endregion
            #region CheckAndCompleteFacilityReceipt
            public static bool CheckAndCompleteFacilityReceipt(string mFacReceiptID, ref string ErrMsg, bool CheckVoucherAvailable)
            {
                //GenFunctions.Issues.Facility.FacilityIssueAllExpired();
                ErrMsg = string.Empty;
                if (mFacReceiptID.Trim() == string.Empty || mFacReceiptID.Trim() == "0")
                    ErrMsg += "Invalid Facility Receipt No.<br />";
                else
                {
                    string strSQL = "Select FacReceiptItemID,AbsRQty from tbFacilityReceiptItems Where FacReceiptID=" + mFacReceiptID;
                    DataTable dt = DBHelper.GetDataTable(strSQL);
                    if (dt.Rows.Count == 0)
                        ErrMsg += "Add atleast one item under items tab. Status cannot be changed.<br />";

                    //strSQL = " Select a.FacReceiptItemID,a.AbsRQty,ti.allotted        from tbFacilityReceiptItems a       inner join tbfacilityreceipts f on f.FacReceiptID=a.FacReceiptID "+
                    //      "  inner join tbindentitems ti on ti.itemid=a.itemid and ti.indentid=f.indentid "+
                    //    " Where a.FacReceiptID=" + mFacReceiptID + " and ti.allotted>0 "+
                    //    " Group By a.FacReceiptItemID, a.AbsRQty,ti.allotted" +
                    //    " Having (NVL(a.AbsRQty,0)=0 or NVL(a.AbsRQty,0)!= NVL(Sum(NVL(a.AbsRQty,0)+NVL(a.ShortQty,0)),0))";

                    strSQL = "Select a.FacReceiptItemID,a.AbsRQty,ti.allotted        from tbFacilityReceiptItems a   "+
                           " inner join tbfacilityreceipts f on f.FacReceiptID=a.FacReceiptID left outer join tbfacilityreceiptbatches b on b.facreceiptitemid=a.facreceiptitemid "+
                           " inner join tbindentitems ti on ti.itemid=a.itemid and ti.indentid=f.indentid  Where a.FacReceiptID=" + mFacReceiptID + " and ti.allotted>0  " +
                           " Group By a.FacReceiptItemID, a.AbsRQty,ti.allotted Having (NVL(a.AbsRQty,0)=0 or NVL(a.AbsRQty,0)!= NVL(Sum(NVL(b.AbsRQty,0)+NVL(b.ShortQty,0)),0)) ";

                    dt = DBHelper.GetDataTable(strSQL);
                    if (dt.Rows.Count > 0)
                        ErrMsg += "All items should have atleast one batch with quantity greater than zero. Status cannot be changed.<br />";

                    strSQL = "Select a.FacReceiptItemID, tout.IssueQty, a.AbsRQty, a.ShortQty" +
                        " from tbFacilityReceiptItems a" +
                        " Inner Join tbOutwards tout on (tout.OutwNo=a.OutwNo)" +
                        " Where a.FacReceiptID=" + mFacReceiptID +
                        "   and NVL(tout.IssueQty,0)!= NVL((NVL(a.AbsRQty,0)+NVL(a.ShortQty,0)),0)";
                    dt = DBHelper.GetDataTable(strSQL);
                    if (dt.Rows.Count > 0)
                        ErrMsg += "Sum of Receipt Qty & Shortage Qty and Warehouse Issue Qty should be same.<br />Status cannot be changed.<br />";

                    //strSQL = "Select r3.ItemID,m1.ItemName,r1.FacReceiptDate,Min(r3.ExpDate) as ExpDate" +
                    //    " from tbFacilityReceiptItems r3" +
                    //    " Inner Join tbFacilityReceipts r1 on (r1.FacReceiptID = r3.FacReceiptID)" +
                    //    " Inner Join masItems m1 on (m1.ItemID = r3.ItemID)" +
                    //    " Where 1=1 and r3.ExpDate is not null and r1.FacReceiptID=" + mFacReceiptID +
                    //    " GROUP BY r3.ItemID,m1.ItemName,r1.FacReceiptDate" +
                    //    " Having r1.FacReceiptDate > Min(r3.ExpDate)";

                    strSQL = @"Select r3.ItemID,m1.ItemName,r1.FacReceiptDate,r4.ExpDate as ExpDate
                         from 
                         tbfacilityreceiptbatches r4 
                         inner join  tbfacilityreceiptitems r3 on r3.facreceiptitemid=r4.facreceiptitemid
                        Inner Join tbFacilityReceipts r1 on (r1.FacReceiptID = r3.FacReceiptID)
                         Inner Join masItems m1 on (m1.ItemID = r3.ItemID)
                         Where 1=1 and r4.ExpDate is not null and r1.FacReceiptID=" + mFacReceiptID + " and r1.FacReceiptDate > r4.ExpDate";

                    dt = DBHelper.GetDataTable(strSQL);
                    if (dt.Rows.Count > 0)
                        ErrMsg += "Batch expiry dates should be greater than receipt date. Check Item " + dt.Rows[0]["ItemName"].ToString() + ". Status cannot be changed.<br />";

                    ////Following SQL checks whether received qty is greater than ordered receivable quantity (Only in case on supplier receipts)
                    //strSQL = "Select a.OrderItemID, a.WarehouseID, b.ItemID, m1.ItemName, NVL(a.AbsQty, 0) as OrderQty, NVL(a.AbsQty, 0) - NVL(a.ReceiptAbsQty, 0) as Receivable, NVL(b.ReceiptAbsQty, 0) as ReceiptQty from soWHDist a Inner Join tbReceiptItems b on (b.OrderItemID = a.OrderItemID) Inner Join tbReceipts c on (c.WarehouseID = a.WarehouseID and c.ReceiptID = b.ReceiptID) Inner Join masItems m1 on (m1.ItemID = b.ItemID) Where c.ReceiptID = " + mReceiptID + " and b.OrderItemID is not null and c.ReceiptType in ('NO') and NVL(a.ReceiptAbsQty, 0) + NVL(b.ReceiptAbsQty, 0) > NVL(a.AbsQty, 0)";
                    //dt = DBHelper.GetDataTable(strSQL);
                    //if (dt.Rows.Count > 0)
                    //    ErrMsg += "Receipt item " + dt.Rows[0]["ItemName"].ToString() + " has receipt quantity greater than receivable quantity. Status cannot be changed.<br />";

                    //if (CheckVoucherAvailable)
                    //{
                    //    strSQL = "Select StkRegDate, StkRegNo from tbReceipts Where (stkRegNo is null or stkRegDate is null) and ReceiptID = " + mReceiptID;
                    //    dt = DBHelper.GetDataTable(strSQL);
                    //    if (dt.Rows.Count > 0)
                    //        ErrMsg += "Voucher No. and Voucher Date. should not be empty and should be saved before completing.<br />";
                    //}
                }
                if (ErrMsg == string.Empty)
                {
                    string strQuery = "Update tbFacilityReceipts set Status = 'C' Where FacReceiptID = " + mFacReceiptID;
                    DBHelper.ExecuteNonQuery(strQuery);
                    strQuery = "Update tbFacilityReceiptItems set Status = 'C' Where FacReceiptID = " + mFacReceiptID;
                    DBHelper.ExecuteNonQuery(strQuery);
                    //strQuery = "Update tbReceiptBatches set Status = 'C' Where ReceiptItemID in (Select ReceiptItemID from tbReceiptItems Where ReceiptID = " + mReceiptID + ")";
                    //DBHelper.ExecuteNonQuery(strQuery);
                    return true;
                }
                else
                    return false;
            }
            #endregion

            #region CheckAndCompleteFacilityReceipt- for WR
            public static bool CheckAndCompleteFacilityReceiptRW(string mFacReceiptID, ref string ErrMsg, bool CheckVoucherAvailable,string DrugType)
            {
                //GenFunctions.Issues.Facility.FacilityIssueAllExpired();
                ErrMsg = string.Empty;
                string strSQL = string.Empty;
                if (mFacReceiptID.Trim() == string.Empty || mFacReceiptID.Trim() == "0")
                    ErrMsg += "Invalid Facility Receipt No.<br />";
                else
                {
                    if (DrugType == "EDL")
                    {
                        strSQL = "Select FacReceiptItemID,AbsRQty from tbFacilityReceiptItems Where FacReceiptID=" + mFacReceiptID;
                        DataTable dt = DBHelper.GetDataTable(strSQL);
                        if (dt.Rows.Count == 0)
                            ErrMsg += "Add atleast one item under items tab. Status cannot be changed.<br />";
                    }
                    else
                    {
                        strSQL = "Select i.ReceiptItemID,(nvl(b.AbsRQty,0)) AbsRQty from lptbReceiptItems i,lptbReceiptBatches b Where i.ReceiptItemID=b.ReceiptItemID and ReceiptID=" + mFacReceiptID;
                        DataTable dt = DBHelper.GetDataTable(strSQL);
                        if (dt.Rows.Count == 0)
                            ErrMsg += "Add atleast one item under items tab. Status cannot be changed.<br />";
                    }
                    
                               
                }
                if (ErrMsg == string.Empty)
                {
                    if (DrugType == "EDL")
                    {
                        string strQuery = "Update tbFacilityReceipts set Status = 'C' Where FacReceiptID = " + mFacReceiptID;
                        DBHelper.ExecuteNonQuery(strQuery);
                        strQuery = "Update tbFacilityReceiptItems set Status = 'C' Where FacReceiptID = " + mFacReceiptID;
                        DBHelper.ExecuteNonQuery(strQuery);
                        return true;
                    }
                    else
                    {
                        string strQuery = "Update lptbReceipts set Status = 'C' Where ReceiptID = " + mFacReceiptID;
                        DBHelper.ExecuteNonQuery(strQuery);
                        strQuery = "Update lptbReceiptItems set Status = 'C' Where ReceiptID = " + mFacReceiptID;
                        DBHelper.ExecuteNonQuery(strQuery);
                        return true;
                    }
                }
                else
                    return false;  
               
            }
            #endregion

            #region DeleteFacilityReceipt
            public static bool DeleteFacilityReceipt(string mFacReceiptID, ref string ErrMsg)
            {
                ErrMsg = string.Empty;
                if (mFacReceiptID.Trim() == string.Empty || mFacReceiptID.Trim() == "0")
                    ErrMsg += "Invalid Receipt No.<br />";
                else
                {
                    try
                    {
                        //string strSQL = "Delete from tbFacilityReceiptBatches Where ReceiptItemID in (Select ReceiptItemID from tbReceiptItems Where ReceiptID = " + mReceiptID + ")";
                        //DBHelper.ExecuteNonQuery(strSQL);
                        string strSQL = "Delete from tbFacilityReceiptItems Where FacReceiptID = " + mFacReceiptID;
                        DBHelper.ExecuteNonQuery(strSQL);
                        strSQL = "Delete from tbFacilityReceipts where FacReceiptID = " + mFacReceiptID;
                        DBHelper.ExecuteNonQuery(strSQL);
                        strSQL = " delete from tbfacilityreceiptbatches where FACReceiptitemid in (select facreceiptitemid from tbfacilityreceiptitems where facreceiptid=" + mFacReceiptID + ")";
                        DBHelper.ExecuteNonQuery(strSQL);
                    }
                    catch
                    {
                        ErrMsg += "Delete not allowed, references found.<br />";
                    }
                }
                return (ErrMsg == string.Empty);
            }
            #endregion

            #region DeleteFacilityReceipt-Ward Return
            public static bool DeleteFacilityReceiptWR(string mFacReceiptID, ref string ErrMsg,string DrugType)
            {
                ErrMsg = string.Empty;
                if (mFacReceiptID.Trim() == string.Empty || mFacReceiptID.Trim() == "0")
                    ErrMsg += "Invalid Receipt No.<br />";
                else
                {
                    try
                    {
                        //string strSQL = "Delete from tbFacilityReceiptBatches Where ReceiptItemID in (Select ReceiptItemID from tbReceiptItems Where ReceiptID = " + mReceiptID + ")";
                        //DBHelper.ExecuteNonQuery(strSQL);
                        if (DrugType == "EDL")
                        {
                            string strSQL = "Delete from tbFacilityReceiptItems Where FacReceiptID = " + mFacReceiptID;
                            DBHelper.ExecuteNonQuery(strSQL);
                            strSQL = "Delete from tbFacilityReceipts where FacReceiptID = " + mFacReceiptID;
                            DBHelper.ExecuteNonQuery(strSQL);
                        }
                        else
                        {
                            string strSQL = "Delete from lptbReceiptBatches Where ReceiptItemID in (select ReceiptItemID from lptbReceiptItems where ReceiptID="+mFacReceiptID+")";
                            DBHelper.ExecuteNonQuery(strSQL);
                            strSQL = "Delete from lptbReceiptItems where ReceiptID = " + mFacReceiptID;
                            DBHelper.ExecuteNonQuery(strSQL);
                            strSQL = "Delete from lptbReceipts where ReceiptID = " + mFacReceiptID;
                            DBHelper.ExecuteNonQuery(strSQL);
                        }
                    }
                    catch
                    {
                        ErrMsg += "Delete not allowed, references found.<br />";
                    }
                }
                return (ErrMsg == string.Empty);
            }
            #endregion
        }
        #endregion

    }
    #endregion

    #region LPReceipts
    public class LPReceipts
    {
        #region CheckAndCompleteReceipt Local Purchase
        public static bool CheckAndCompleteReceipt(string mReceiptID, ref string ErrMsg, bool CheckVoucherAvailable)
        {
            GenFunctions.Issues.IssueAllExpired();
            ErrMsg = string.Empty;
            if (mReceiptID.Trim() == string.Empty || mReceiptID.Trim() == "0")
                ErrMsg += "Invalid Receipt No.<br />";
            else
            {
                string strSQL = "Select a.ReceiptItemID, a.ReceiptAbsQty from LPtbReceiptItems a Where a.ReceiptID = " + mReceiptID;
                DataTable dt = DBHelper.GetDataTable(strSQL);
                if (dt.Rows.Count == 0)
                    ErrMsg += "Add atleast one item under items tab. Status cannot be changed.<br />";

                strSQL = "Select a.ReceiptItemID, a.ReceiptAbsQty from LPtbReceiptItems a Left Outer Join LPtbReceiptBatches b on (b.ReceiptItemID = a.ReceiptItemID) Where a.ReceiptID = " + mReceiptID + " Group By a.ReceiptItemID, a.ReceiptAbsQty Having (NVL(a.ReceiptAbsQty, 0) = 0 or NVL(a.ReceiptAbsQty, 0) != NVL(Sum(NVL(b.AbsRQty, 0) + NVL(b.ShortQty, 0)), 0))";
                dt = DBHelper.GetDataTable(strSQL);
                if (dt.Rows.Count > 0)
                    ErrMsg += "All items should have atleast one batch with quantity greater than zero. Status cannot be changed.<br />";

                strSQL = "Select a.ReceiptItemID, a.ReceiptAbsQty from LPtbReceiptItems a Left Outer Join LPtbReceiptBatches b on (b.ReceiptItemID = a.ReceiptItemID) Where a.ReceiptID = " + mReceiptID + " Group By a.ReceiptItemID, a.ReceiptAbsQty Having NVL(a.ReceiptAbsQty, 0) != NVL(Sum(NVL(b.AbsRQty, 0) + NVL(b.ShortQty, 0)), 0)";
                dt = DBHelper.GetDataTable(strSQL);
                if (dt.Rows.Count > 0)
                    ErrMsg += "Receipt Qty and Total Batch Qty should be same. Status cannot be changed.<br />";

                strSQL = "Select r2.ItemID, Coalesce(m1.ItemName,m11.ItemName) as ItemName, r1.ReceiptDate, Min(r3.ExpDate) as ExpDate from LPtbReceiptBatches r3 Inner Join LPtbReceiptItems r2 on (r2.ReceiptItemID = r3.ReceiptItemID) Inner Join LPtbReceipts r1 on (r1.ReceiptID = r2.ReceiptID) Left Outer Join masItems m1 on (m1.ItemID = r2.ItemID) Left Outer Join LPmasItems m11 on (m11.LPItemID = r2.LPItemID) Where 1=1 and r3.ExpDate is not null and r1.ReceiptID = " + mReceiptID + " GROUP BY r2.ItemID, r2.LPItemID, m1.ItemName, m11.ItemName, r1.ReceiptDate Having r1.ReceiptDate > Min(r3.ExpDate)";
                dt = DBHelper.GetDataTable(strSQL);
                if (dt.Rows.Count > 0)
                    ErrMsg += "Batch expiry dates should be greater than receipt date. Check Item " + dt.Rows[0]["ItemName"].ToString() + ". Status cannot be changed.<br />";

                //Following SQL checks whether received qty is greater than ordered receivable quantity (Only in case on supplier receipts)
                strSQL = "Select a.OrderItemID, a.WarehouseID, b.ItemID, b.LPItemID, Coalesce(m1.ItemName,m11.ItemName) as ItemName, NVL(a.AbsQty, 0) as OrderQty, NVL(a.AbsQty, 0) - NVL(a.ReceiptAbsQty, 0) as Receivable, NVL(b.ReceiptAbsQty, 0) as ReceiptQty from LPSOOrderDistribution a Inner Join LPtbReceiptItems b on (b.OrderItemID = a.OrderItemID) Inner Join LPtbReceipts c on (c.WarehouseID = a.WarehouseID and c.ReceiptID = b.ReceiptID) Left Outer Join masItems m1 on (m1.ItemID = b.ItemID) Left Outer Join LPmasItems m11 on (m11.LPItemID = b.LPItemID) Where c.ReceiptID = " + mReceiptID + " and b.OrderItemID is not null and c.ReceiptType in ('NO') and NVL(a.ReceiptAbsQty, 0) + NVL(b.ReceiptAbsQty, 0) > NVL(a.AbsQty, 0)";

                dt = DBHelper.GetDataTable(strSQL);
                if (dt.Rows.Count > 0)
                    ErrMsg += "Receipt item " + dt.Rows[0]["ItemName"].ToString() + " has receipt quantity greater than receivable quantity. Status cannot be changed.<br />";

                if (CheckVoucherAvailable)
                {
                    strSQL = "Select StkRegDate, StkRegNo from LPtbReceipts Where (stkRegNo is null or stkRegDate is null) and ReceiptID = " + mReceiptID;
                    dt = DBHelper.GetDataTable(strSQL);
                    if (dt.Rows.Count > 0)
                        ErrMsg += "Voucher No. and Voucher Date. should not be empty and should be saved before completing.<br />";
                }
            }
            if (ErrMsg == string.Empty)
            {
                string strQuery = "Update LPtbReceipts set Status = 'C' Where ReceiptID = " + mReceiptID;
                DBHelper.ExecuteNonQuery(strQuery);
                strQuery = "Update LPtbReceiptItems set Status = 'C' Where ReceiptID = " + mReceiptID;
                DBHelper.ExecuteNonQuery(strQuery);
                //strQuery = "Update tbReceiptBatches set Status = 'C' Where ReceiptItemID in (Select ReceiptItemID from tbReceiptItems Where ReceiptID = " + mReceiptID + ")";
                //DBHelper.ExecuteNonQuery(strQuery);
                return true;
            }
            else
                return false;
        }
        #endregion

        #region DeleteReceipt Local Purchase
        public static bool DeleteReceipt(string mReceiptID, ref string ErrMsg)
        {
            ErrMsg = string.Empty;
            if (mReceiptID.Trim() == string.Empty || mReceiptID.Trim() == "0")
                ErrMsg += "Invalid Receipt No.<br />";
            else
            {
                try
                {
                    string strSQL = "Delete from tbfacilityReceiptBatches Where facReceiptItemID in (Select facReceiptItemID from tbfacilityReceiptItems Where facReceiptID = " + mReceiptID + ")";
                    DBHelper.ExecuteNonQuery(strSQL);
                    strSQL = "Delete from tbfacilityReceiptItems Where facReceiptID = " + mReceiptID;
                    DBHelper.ExecuteNonQuery(strSQL);
                    strSQL = "Delete from tbfacilityReceipts where facReceiptID = " + mReceiptID;
                    DBHelper.ExecuteNonQuery(strSQL);
                }
                catch
                {
                    ErrMsg += "Delete not allowed, references found.<br />";
                }
            }
            return (ErrMsg == string.Empty);
        }
        #endregion
    }
    #endregion

    #region Issues
    public class Issues
    {
        #region IssueAllExpired
        public static void IssueAllExpired()
        {
            string strSQL = "Select Distinct SourceID, SchemeID, WarehouseID, IndentDate as MonYr from v_ExpiredItemsInStock";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    #region Data to be stored
                    //string mIndentID = string.Empty;
                    string mSourceID = GenFunctions.CheckEmptyString(dr["SourceID"], "0");
                    string mSchemeID = GenFunctions.CheckEmptyString(dr["SchemeID"], "0");
                    string mWarehouseID = GenFunctions.CheckEmptyString(dr["WarehouseID"], "0");
                    string mIssueType = "EX";
                    string mIndentNo = GenFunctions.AutoGenerateNumbers(mWarehouseID, false, mIssueType);
                    //string mIndentDate = GenFunctions.CheckDBNullForDate(dr["MonYr"]);
                    //string ErrMsg = "";
                    Broadline.WMS.Data.DBHelperExtended.sp_IssueExpiredBatches((decimal)dr["SourceID"], (decimal)dr["SchemeID"], (decimal)dr["WarehouseID"], mIndentNo, (DateTime)dr["MonYr"], mIssueType);
                    #endregion
                }
            }
            //string strSQL = "Select r1.SourceID, r1.SchemeID, r1.WarehouseID, Trunc(r3.ExpDate, 'Mon') as MonYr from tbReceiptBatches r3 Inner Join tbReceiptItems r2 on (r2.ReceiptItemID = r3.ReceiptItemID) Inner Join tbReceipts r1 on (r1.ReceiptID = r2.ReceiptID) Where r1.Status not in ('I') and NVL(r3.AbsRQty, 0) > NVL(r3.IssueQty, 0) and r3.ExpDate is Not Null and Trunc(r3.ExpDate, 'Mon') <= Trunc(SysDate)";
            //DataTable dt = DBHelper.GetDataTable(strSQL);
            //if (dt.Rows.Count > 0)
            //{
            //    foreach (DataRow dr in dt.Rows)
            //    {
            //        #region Create IndentID if not found
            //        #region Data to be stored
            //        string mIndentID = string.Empty;
            //        string mIssueType = "EX";
            //        string mSourceID = GenFunctions.CheckEmptyString(dr["SourceID"], "0");
            //        string mSchemeID = GenFunctions.CheckEmptyString(dr["SchemeID"], "0");
            //        string mWarehouseID = GenFunctions.CheckEmptyString(dr["WarehouseID"], "0");
            //        string mIndentNo = GenFunctions.AutoGenerateNumbers(mWarehouseID, false, mIssueType);
            //        string mIndentDate = GenFunctions.CheckDBNullForDate(dr["MonYr"]);
            //        string ErrMsg = "";
            //        #endregion

            //        #region Validate Data
            //        GenFunctions.CheckDateGreaterThanToday(mIndentDate, "Issue date");
            //        GenFunctions.CheckDuplicate(mIndentNo, "Issue No", "tbIndents", "IndentNo", mIndentID, "IndentID", " and WarehouseID = " + mWarehouseID);
            //        GenFunctions.CheckNumber(ref mWarehouseID, "Warehouse", false);
            //        GenFunctions.CheckNumber(ref mSourceID, "Source", false);
            //        GenFunctions.CheckNumber(ref mSchemeID, "Scheme", false);
            //        GenFunctions.CheckStringForEmpty(ref mIndentNo, "Issue No", false);
            //        GenFunctions.CheckDate(ref mIndentDate, "Issue date", false);
            //        GenFunctions.CheckStringForEmpty(ref mIssueType, "Issue Type", false);
            //        #endregion

            //        #region Save Data
            //        string strSQL_I1 = "Insert into tbIndents (WarehouseID, SourceID, SchemeID, IndentNo, IndentDate, IssueType) values (" + mWarehouseID + ", " + mSourceID + ", " + mSchemeID + ", " + mIndentNo + ", " + mIndentDate + ", " + mIssueType + ")";
            //        DBHelper.ExecuteNonQuery(strSQL_I1);
            //        strSQL_I1 = "Select IndentID from tbIndents Where WarehouseID = " + mWarehouseID + " and SourceID = " + mSourceID + " and SchemeID = " + mSchemeID + " and IndentNo = " + mIndentNo + " and IndentDate = " + mIndentDate + " and IssueType = " + mIssueType;
            //        DataTable dtIndentID = DBHelper.GetDataTable(strSQL_I1);
            //        if (dtIndentID.Rows.Count > 0)
            //            mIndentID = dtIndentID.Rows[0]["IndentID"].ToString();
            //        #endregion
            //        #endregion

            //        string strSQL_I2 = "Select r1.SourceID, r1.SchemeID, r1.WarehouseID, Trunc(r3.ExpDate, 'Mon') as MonYr, r2.ItemID, Sum(NVL(r3.AbsRQty, 0) - NVL(r3.IssueQty, 0)) as TotRQty from tbReceiptBatches r3 Inner Join tbReceiptItems r2 on (r2.ReceiptItemID = r3.ReceiptItemID) Inner Join tbReceipts r1 on (r1.ReceiptID = r2.ReceiptID) Where r1.Status not in ('I') and r1.SourceID = " + mSourceID + " and r1.SchemeID = " + mSchemeID + " and r1.WarehouseID = " + mWarehouseID + " and Trunc(r3.ExpDate, 'Mon') = " + mIndentDate + " and NVL(r3.AbsRQty, 0) > NVL(r3.IssueQty, 0) and r3.ExpDate is Not Null and Trunc(r3.ExpDate, 'Mon') <= Trunc(SysDate) GROUP BY r1.SourceID, r1.SchemeID, r1.WarehouseID, Trunc(r3.ExpDate, 'Mon'), r2.ItemID Order By r2.ItemID";
            //        DataTable dt_Items = DBHelper.GetDataTable(strSQL_I2);
            //        if (dt_Items.Rows.Count > 0)
            //        {
            //            foreach (DataRow drItem in dt_Items.Rows)
            //            {
            //                #region Create IndentItemID if not found
            //                string mIndentItemID = "0";
            //                string mItemID = drItem["ItemID"].ToString();
            //                #region Validate Row Data
            //                if (mIndentID != "")
            //                    GenFunctions.CheckDuplicate(mItemID, "Item", "tbIndentItems", "ItemID", mIndentItemID, "IndentItemID", " and IndentID = " + mIndentID);
            //                GenFunctions.CheckNumber(ref mIndentID, "Indent No", false);
            //                GenFunctions.CheckNumber(ref mItemID, "Item", false);
            //                #endregion

            //                #region Save Row Data
            //                string strSQL_I3 = "Insert into tbIndentItems (IndentID, ItemID) values (" + mIndentID + ", " + mItemID + ")";
            //                DBHelper.ExecuteNonQuery(strSQL_I3);
            //                strSQL_I3 = "Select IndentItemID from tbIndentItems Where IndentID = " + mIndentID + " and ItemID = " + mItemID;
            //                DataTable dtIndentItemID = DBHelper.GetDataTable(strSQL_I3);
            //                if (dtIndentItemID.Rows.Count > 0)
            //                    mIndentItemID = dtIndentItemID.Rows[0]["IndentItemID"].ToString();
            //                #endregion
            //                #endregion

            //                string strSQL_I4 = "Select r1.SourceID, r1.SchemeID, r1.WarehouseID, Trunc(r3.ExpDate, 'Mon') as MonYr, r2.ItemID, r3.InwNo, NVL(r3.AbsRQty, 0) - NVL(r3.IssueQty, 0) as RQty from tbReceiptBatches r3 Inner Join tbReceiptItems r2 on (r2.ReceiptItemID = r3.ReceiptItemID) Inner Join tbReceipts r1 on (r1.ReceiptID = r2.ReceiptID) Where r1.Status not in ('I') and r1.SourceID = " + mSourceID + " and r1.SchemeID = " + mSchemeID + " and r1.WarehouseID = " + mWarehouseID + " and r2.ItemID = " + mItemID + " and Trunc(r3.ExpDate, 'Mon') = " + mIndentDate + " and NVL(r3.AbsRQty, 0) > NVL(r3.IssueQty, 0) and r3.ExpDate is Not Null and Trunc(r3.ExpDate, 'Mon') <= Trunc(SysDate)";
            //                DataTable dt_Batches = DBHelper.GetDataTable(strSQL_I4);
            //                if (dt_Batches.Rows.Count > 0)
            //                {
            //                    foreach (DataRow dt_Batch in dt_Batches.Rows)
            //                    {
            //                        #region Create Issues by batch
            //                        string mInwNo = dt_Batch["InwNo"].ToString();
            //                        string mIssueQty = dt_Batch["RQty"].ToString();
            //                        string strSQL_I5 = "Insert into tbOutWards (IndentItemID, ItemID, InwNo, IssueQty) Values (" + mIndentItemID + ", " + mItemID + ", " + mInwNo + ", " + mIssueQty + ")";
            //                        DBHelper.ExecuteNonQuery(strSQL_I5);
            //                        #endregion
            //                    }
            //                }
            //            }
            //        }
            //        GenFunctions.Issues.CheckAndCompleteIndent(mIndentID, ref ErrMsg, false);
            //    }
            //}
            //strSQL = "Update tbOutWards Set IssueQty = 0 Where OutwNo in (Select c.OutwNo from tbIndents a Inner Join tbIndentItems b on (b.IndentID = a.IndentID) Inner Join tbOutWards c on (c.IndentItemID = b.IndentItemID) Where c.InwNo in (Select InwNo from tbReceiptBatches Where Trunc(ExpDate, 'Mon') <= Trunc(sysdate) and (AbsRQty > AllotQty or AllotQty > IssueQty or AbsRQty > IssueQty)) and a.Status in ('IN'))";
            //DBHelper.ExecuteNonQuery(strSQL);
        }
        #endregion

        #region CheckAndCompleteIndent
        public static bool CheckAndCompleteIndent(string mIndentID, ref string ErrMsg, bool CheckVoucherAvailable)
        {
            ErrMsg = string.Empty;
            if (mIndentID.Trim() == string.Empty || mIndentID.Trim() == "0")
                ErrMsg += "Invalid Indent/Issue No.<br />";
            else
            {
                string strSQL = "Select a.IndentItemID, a.Allotted from tbIndentItems a Where a.IndentID = " + mIndentID;
                DataTable dt = DBHelper.GetDataTable(strSQL);
                if (dt.Rows.Count == 0)
                    ErrMsg += "Add atleast one item under items tab. Status cannot be changed.<br />";

                strSQL = "Select a.IndentItemID, a.Allotted from tbIndentItems a Left Outer Join tbOutwards b on (b.IndentItemID = a.IndentItemID) Where a.IndentID = " + mIndentID + " Group By a.IndentItemID, a.Allotted Having (NVL(a.Allotted, 0) = 0 or NVL(a.Allotted, 0) != NVL(Sum(NVL(b.IssueQty, 0)), 0))";
                dt = DBHelper.GetDataTable(strSQL);
                if (dt.Rows.Count > 0)
                    ErrMsg += "All items should have atleast one batch with quantity greater than zero. Status cannot be changed.<br />";

                strSQL = "Select a.IndentItemID, a.Allotted from tbIndentItems a Left Outer Join tbOutwards b on (b.IndentItemID = a.IndentItemID) Where a.IndentID = " + mIndentID + " Group By a.IndentItemID, a.Allotted Having NVL(a.Allotted, 0) != NVL(Sum(NVL(b.IssueQty, 0)), 0)";
                dt = DBHelper.GetDataTable(strSQL);
                if (dt.Rows.Count > 0)
                    ErrMsg += "Issue Qty and Total Batch Qty should be same. Status cannot be changed.<br />";

                strSQL = "Select i2.ItemID, m1.ItemName, i1.IndentDate, Max(r1.ReceiptDate) as ReceiptDate from tbIndents i1 Inner Join tbIndentItems i2 on (i2.IndentID = i1.IndentID) Inner Join tbOutWards i3 on (i3.IndentItemID = i2.IndentItemID) Inner Join tbReceiptBatches r3 on (r3.InwNo = i3.InwNo) Inner Join tbReceiptItems r2 on (r2.ReceiptItemID = r3.ReceiptItemID) Inner Join tbReceipts r1 on (r1.ReceiptID = r2.ReceiptID) Inner Join masItems m1 on (m1.ItemID = i2.ItemID) Where 1=1 and i1.IndentID = " + mIndentID + " GROUP BY i2.ItemID, m1.ItemName, i1.IndentDate Having i1.IndentDate < Max(r1.ReceiptDate)";
                dt = DBHelper.GetDataTable(strSQL);
                if (dt.Rows.Count > 0)
                    ErrMsg += "Issue date should be greater than the batch receipt date. Check Item " + dt.Rows[0]["ItemName"].ToString() + ". Status cannot be changed.<br />";

                if (CheckVoucherAvailable)
                {
                    strSQL = "Select StkRegDate, StkRegNo from tbIndents Where (stkRegNo is null or stkRegDate is null) and IndentID = " + mIndentID;
                    dt = DBHelper.GetDataTable(strSQL);
                    if (dt.Rows.Count > 0)
                        ErrMsg += "Voucher No. and Voucher Date. should not be empty and should be saved before completing.<br />";
                }
            }
            if (ErrMsg == string.Empty)
            {
                string mIssueDate = GenFunctions.CheckDBNullForDate(DateTime.Now);
                GenFunctions.CheckDate(ref mIssueDate, "Issue Date", false);
                string strQuery = "Update tbIndents set Status = 'C', IssueDate = " + mIssueDate + " Where IndentID = " + mIndentID;
                DBHelper.ExecuteNonQuery(strQuery);
                strQuery = "Update tbOutWards set Status = 'C', Issued = 1 Where IndentItemID in (Select IndentItemID from tbIndentItems Where IndentID = " + mIndentID + ")";
                DBHelper.ExecuteNonQuery(strQuery);
                strQuery = "Update tbIndentItems set Status = 'C' Where IndentID = " + mIndentID;
                DBHelper.ExecuteNonQuery(strQuery);
                //string strQuery = "Update tbOutwards set Issued = 1 Where IndentItemID in (Select IndentItemID from tbIndentItems Where IndentID = " + mIndentID + ")";
                //DBHelper.ExecuteNonQuery(strQuery);
                return true;
            }
            else
                return false;
        }
        #endregion

        #region DeleteIndent
        public static bool DeleteIndent(string mIndentID, ref string ErrMsg)
        {
            ErrMsg = string.Empty;
            if (mIndentID.Trim() == string.Empty || mIndentID.Trim() == "0")
                ErrMsg += "Invalid Indent/Issue No.<br />";
            else
            {
                try
                {
                    string strSQL = "Delete from tbOutwards Where IndentItemID in (Select IndentItemID from tbIndentItems Where IndentID = " + mIndentID + ")";
                    DBHelper.GetDataTable(strSQL);
                    strSQL = "Delete from tbIndentItems where IndentID = " + mIndentID;
                    DBHelper.GetDataTable(strSQL);
                    strSQL = "Delete from tbIndents where IndentID = " + mIndentID;
                    DBHelper.GetDataTable(strSQL);
                }
                catch
                {
                    ErrMsg += "Delete not allowed, references found.<br />";
                }
            }
            return (ErrMsg == string.Empty);
        }
        #endregion

        #region [Sub Class] Facility Issues
        public class Facility
        {
            #region FacilityIssueAllExpired
            public static void FacilityIssueAllExpired()
            {
                //string strSQL = "Select Distinct SourceID,SchemeID, FacilityID, IssueDate as MonYr from v_ExpiredItemsInStock";
                //DataTable dt = DBHelper.GetDataTable(strSQL);
                //if (dt.Rows.Count > 0)
                //{
                //    foreach (DataRow dr in dt.Rows)
                //    {
                //        #region Data to be stored
                //        string mSourceID = GenFunctions.CheckEmptyString(dr["SourceID"], "0");
                //        string mSchemeID = GenFunctions.CheckEmptyString(dr["SchemeID"], "0");
                //        string mFacilityID = GenFunctions.CheckEmptyString(dr["FacilityID"], "0");
                //        string mIssueType = "EX";
                //        string mIndentNo = GenFunctions.AutoGenerateNumbers(mWarehouseID, false, mIssueType);
                //        Broadline.WMS.Data.DBHelperExtended.sp_IssueExpiredBatches((decimal)dr["SourceID"], (decimal)dr["SchemeID"], (decimal)dr["WarehouseID"], mIndentNo, (DateTime)dr["MonYr"], mIssueType);
                //        #endregion
                //    }
                //}
            }
            #endregion

            #region CheckAndCompleteFacilityIssues
            public static bool CheckAndCompleteFacilityIssues(string mIssueID, ref string ErrMsg, bool CheckVoucherAvailable)
            {
                ErrMsg = string.Empty;
                if (mIssueID.Trim() == string.Empty || mIssueID.Trim() == "0")
                    ErrMsg += "Invalid Indent/Issue No.<br />";
                else
                {
                    string strSQL = "Select a.IssueItemID,a.IssueQty from tbFacilityIssueItems a Where a.IssueID=" + mIssueID;
                    DataTable dt = DBHelper.GetDataTable(strSQL);
                    if (dt.Rows.Count == 0)
                        ErrMsg += "Add atleast one item under items tab. Status cannot be changed.<br />";

                    strSQL = "Select a.IssueItemID, a.IssueQty,nvl(sum(tbo.issueqty),0) issqty" +
                        " from tbFacilityIssueItems a" +
                        " left outer join tbfacilityoutwards tbo on tbo.issueitemid=a.issueitemid and tbo.inwno not in (select distinct inwno from tbfacilityreceiptbatches where whissueblock=1 ) "+
                        " Where a.IssueID=" + mIssueID +
                        " Group By a.IssueItemID, a.IssueQty" +
                        " Having (NVL(a.IssueQty,0)=0 or NVL(a.IssueQty,0)!=NVL(Sum(NVL(tbo.issueqty,0)),0))";
                    dt = DBHelper.GetDataTable(strSQL);
                    if (dt.Rows.Count > 0)
                        ErrMsg += "All items should have atleast one batch with quantity greater than zero. Status cannot be changed.<br />";

                    strSQL = "Select a.IssueItemID, a.IssueQty" +
                        " from tbFacilityIssueItems a" +
                        " Where a.IssueID=" + mIssueID +
                        " Group By a.IssueItemID, a.IssueQty" +
                        " Having (NVL(a.IssueQty,0)!=NVL(Sum(NVL(a.IssueQty,0)),0))";
                    dt = DBHelper.GetDataTable(strSQL);
                    if (dt.Rows.Count > 0)
                        ErrMsg += "Issue Qty and Total Batch Qty should be same. Status cannot be changed.<br />";

                    strSQL = "Select i2.ItemID,m1.ItemName,i1.IssueDate,Max(r1.FacReceiptDate) as FacReceiptDate" +
                        " from tbFacilityIssues i1" +
                        " Inner Join tbFacilityIssueItems i2 on (i2.IssueID = i1.IssueID)" +
                        " Inner Join tbFacilityReceiptItems r2 on (r2.FacReceiptItemID = i2.FacReceiptItemID)" +
                        " Inner Join tbFacilityReceipts r1 on (r1.FacReceiptID = r2.FacReceiptID)" +
                        " Inner Join masItems m1 on (m1.ItemID = i2.ItemID)" +
                        " Where 1=1 and i1.IssueID = " + mIssueID +
                        " GROUP BY i2.ItemID, m1.ItemName, i1.IssueDate" +
                        " Having i1.IssueDate < Max(r1.FacReceiptDate)";
                    dt = DBHelper.GetDataTable(strSQL);
                    if (dt.Rows.Count > 0)
                        ErrMsg += "Issue date should be greater than the batch receipt date. Check Item " + dt.Rows[0]["ItemName"].ToString() + ". Status cannot be changed.<br />";



                    strSQL = @"Select a.IssueItemID,b.inwno,rb.absrqty, b.IssueQty,nvl(iss.TotissueQty,0) TotissueQty
, rb.absrqty ,(b.IssueQty+nvl(iss.TotissueQty,0)) tot,m.itemcode
                         from tbFacilityIssueItems a
                         inner join tbfacilityoutwards b on b.issueitemid=a.issueitemid
                         inner join tbfacilityreceiptbatches  rb on rb.inwno=b.inwno
                         inner join vmasitems m on m.itemid=a.itemid
                         left outer join 
                         (
                          select FacilityID,sum(nvl(a.IssueQty,0)) TotissueQty, a.Inwno   
                            from  tbfacilityoutwards a
                            inner join tbfacilityissueitems tbi on tbi.issueitemid =a.issueitemid   
                            inner join tbfacilityissues tb on tb.issueid=tbi.issueid    
                            where tb.status='C' 
                            group by a.Inwno ,FacilityID 
                         )  iss on iss.inwno=  b.inwno               
                         Where a.IssueID=" + mIssueID + @"
                         Group By m.itemcode,a.IssueItemID, b.IssueQty,b.inwno,iss.TotissueQty,rb.absrqty
                         Having (b.IssueQty+nvl(iss.TotissueQty,0))>rb.absrqty  ";
                    dt = DBHelper.GetDataTable(strSQL);
                    if (dt.Rows.Count > 0)
                    {
                        string mDrug = "";
                        string MdrugSw = "";
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            mDrug = dt.Rows[i]["itemcode"].ToString() + ",";
                            MdrugSw += mDrug;
                        }
                        MdrugSw = MdrugSw;
                        ErrMsg += "Total Batch Qty should not be same.Delete This Item " + MdrugSw + " and Re-Issue.<br />";

                    }

                    //if (dt.Rows.Count > 0)
                    //    ErrMsg += "Total Batch Qty should not be same. Status cannot be changed.<br />";




                    //if (CheckVoucherAvailable)
                    //{
                    //    strSQL = "Select StkRegDate, StkRegNo from tbIndents Where (stkRegNo is null or stkRegDate is null) and IndentID = " + mIndentID;
                    //    dt = DBHelper.GetDataTable(strSQL);
                    //    if (dt.Rows.Count > 0)
                    //        ErrMsg += "Voucher No. and Voucher Date. should not be empty and should be saved before completing.<br />";
                    //}
                }
                if (ErrMsg == string.Empty)
                {
                    string mIssueDate = GenFunctions.CheckDBNullForDate(DateTime.Now);
                    GenFunctions.CheckDate(ref mIssueDate, "Issue Date", false);

                    string strQuery = "Update tbFacilityIssues set Status='C',IssuedDate=" + mIssueDate + " Where IssueID=" + mIssueID;
                    DBHelper.ExecuteNonQuery(strQuery);

                    strQuery = "Update tbFacilityOutwards set Status='C', Issued=1" +
                        " Where IssueItemID in (Select IssueItemID from tbFacilityIssueItems Where IssueID=" + mIssueID + ")";
                    DBHelper.ExecuteNonQuery(strQuery);

                    strQuery = "Update tbFacilityIssueItems set Status='C',Issued=1 Where IssueID=" + mIssueID;
                    DBHelper.ExecuteNonQuery(strQuery);

                    return true;
                }
                else
                    return false;
            }
            #endregion

            #region CheckAndCompleteFacilityIssues
            public static bool CheckAndCompFacIssuesRWH(string mIssueID, ref string ErrMsg, bool CheckVoucherAvailable)
            {
                ErrMsg = string.Empty;
                if (mIssueID.Trim() == string.Empty || mIssueID.Trim() == "0")
                    ErrMsg += "Invalid Indent/Issue No.<br />";
                else
                {
                    string strSQL = "Select a.IssueItemID,a.IssueQty from tbFacilityIssueItems a Where a.IssueID=" + mIssueID;
                    DataTable dt = DBHelper.GetDataTable(strSQL);
                    if (dt.Rows.Count == 0)
                        ErrMsg += "Add atleast one item under items tab. Status cannot be changed.<br />";

                    strSQL = "Select a.IssueItemID, a.IssueQty,nvl(sum(tbo.issueqty),0) issqty" +
                        " from tbFacilityIssueItems a" +
                        " left outer join tbfacilityoutwards tbo on tbo.issueitemid=a.issueitemid " +
                        " Where a.IssueID=" + mIssueID +
                        " Group By a.IssueItemID, a.IssueQty" +
                        " Having (NVL(a.IssueQty,0)=0 or NVL(a.IssueQty,0)!=NVL(Sum(NVL(tbo.issueqty,0)),0))";
                    dt = DBHelper.GetDataTable(strSQL);
                    if (dt.Rows.Count > 0)
                        ErrMsg += "All items should have atleast one batch with quantity greater than zero. Status cannot be changed.<br />";

                    strSQL = "Select a.IssueItemID, a.IssueQty" +
                        " from tbFacilityIssueItems a" +
                        " Where a.IssueID=" + mIssueID +
                        " Group By a.IssueItemID, a.IssueQty" +
                        " Having (NVL(a.IssueQty,0)!=NVL(Sum(NVL(a.IssueQty,0)),0))";
                    dt = DBHelper.GetDataTable(strSQL);
                    if (dt.Rows.Count > 0)
                        ErrMsg += "Issue Qty and Total Batch Qty should be same. Status cannot be changed.<br />";

                    strSQL = "Select i2.ItemID,m1.ItemName,i1.IssueDate,Max(r1.FacReceiptDate) as FacReceiptDate" +
                        " from tbFacilityIssues i1" +
                        " Inner Join tbFacilityIssueItems i2 on (i2.IssueID = i1.IssueID)" +
                        " Inner Join tbFacilityReceiptItems r2 on (r2.FacReceiptItemID = i2.FacReceiptItemID)" +
                        " Inner Join tbFacilityReceipts r1 on (r1.FacReceiptID = r2.FacReceiptID)" +
                        " Inner Join masItems m1 on (m1.ItemID = i2.ItemID)" +
                        " Where 1=1 and i1.IssueID = " + mIssueID +
                        " GROUP BY i2.ItemID, m1.ItemName, i1.IssueDate" +
                        " Having i1.IssueDate < Max(r1.FacReceiptDate)";
                    dt = DBHelper.GetDataTable(strSQL);
                    if (dt.Rows.Count > 0)
                        ErrMsg += "Issue date should be greater than the batch receipt date. Check Item " + dt.Rows[0]["ItemName"].ToString() + ". Status cannot be changed.<br />";



                    strSQL = @"Select a.IssueItemID,b.inwno,rb.absrqty, b.IssueQty,nvl(iss.TotissueQty,0) TotissueQty
, rb.absrqty ,(b.IssueQty+nvl(iss.TotissueQty,0)) tot,m.itemcode
                         from tbFacilityIssueItems a
                         inner join tbfacilityoutwards b on b.issueitemid=a.issueitemid
                         inner join tbfacilityreceiptbatches  rb on rb.inwno=b.inwno
                         inner join vmasitems m on m.itemid=a.itemid
                         left outer join 
                         (
                          select FacilityID,sum(nvl(a.IssueQty,0)) TotissueQty, a.Inwno   
                            from  tbfacilityoutwards a
                            inner join tbfacilityissueitems tbi on tbi.issueitemid =a.issueitemid   
                            inner join tbfacilityissues tb on tb.issueid=tbi.issueid    
                            where tb.status='C' 
                            group by a.Inwno ,FacilityID 
                         )  iss on iss.inwno=  b.inwno               
                         Where a.IssueID=" + mIssueID + @"
                         Group By m.itemcode,a.IssueItemID, b.IssueQty,b.inwno,iss.TotissueQty,rb.absrqty
                         Having (b.IssueQty+nvl(iss.TotissueQty,0))>rb.absrqty  ";
                    dt = DBHelper.GetDataTable(strSQL);
                    if (dt.Rows.Count > 0)
                    {
                        string mDrug = "";
                        string MdrugSw = "";
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            mDrug = dt.Rows[i]["itemcode"].ToString() + ",";
                            MdrugSw += mDrug;
                        }
                        MdrugSw = MdrugSw;
                        ErrMsg += "Total Batch Qty should not be same.Delete This Item " + MdrugSw + " and Re-Issue.<br />";

                    }

                    //if (dt.Rows.Count > 0)
                    //    ErrMsg += "Total Batch Qty should not be same. Status cannot be changed.<br />";




                    //if (CheckVoucherAvailable)
                    //{
                    //    strSQL = "Select StkRegDate, StkRegNo from tbIndents Where (stkRegNo is null or stkRegDate is null) and IndentID = " + mIndentID;
                    //    dt = DBHelper.GetDataTable(strSQL);
                    //    if (dt.Rows.Count > 0)
                    //        ErrMsg += "Voucher No. and Voucher Date. should not be empty and should be saved before completing.<br />";
                    //}
                }
                if (ErrMsg == string.Empty)
                {
                    string mIssueDate = GenFunctions.CheckDBNullForDate(DateTime.Now);
                    GenFunctions.CheckDate(ref mIssueDate, "Issue Date", false);

                    string strQuery = "Update tbFacilityIssues set Status='C',IssuedDate=" + mIssueDate + " Where IssueID=" + mIssueID;
                    DBHelper.ExecuteNonQuery(strQuery);

                    strQuery = "Update tbFacilityOutwards set Status='C', Issued=1" +
                        " Where IssueItemID in (Select IssueItemID from tbFacilityIssueItems Where IssueID=" + mIssueID + ")";
                    DBHelper.ExecuteNonQuery(strQuery);

                    strQuery = "Update tbFacilityIssueItems set Status='C',Issued=1 Where IssueID=" + mIssueID;
                    DBHelper.ExecuteNonQuery(strQuery);

                    return true;
                }
                else
                    return false;
            }
            #endregion


            #region DeleteFacilityIssues
            public static bool DeleteFacilityIssues(string mIssueID, ref string ErrMsg)
            {
                ErrMsg = string.Empty;
                if (mIssueID.Trim() == string.Empty || mIssueID.Trim() == "0")
                    ErrMsg += "Invalid Indent/Issue No.<br />";
                else
                {
                    try
                    {
                        string strSQL = "Delete from tbFacilityOutwards Where IssueItemID in" +
                            " (Select IssueItemID from tbFacilityIssueItems Where IssueID=" + mIssueID + ")";
                        DBHelper.GetDataTable(strSQL);

                        strSQL = "Delete from tbFacilityIssueItems where IssueID=" + mIssueID;
                        DBHelper.GetDataTable(strSQL);

                        strSQL = "Delete from tbFacilityIssues where IssueID=" + mIssueID;
                        DBHelper.GetDataTable(strSQL);
                    }
                    catch
                    {
                        ErrMsg += "Delete not allowed, references found.<br />";
                    }
                }
                return (ErrMsg == string.Empty);
            }
            #endregion

            #region DeleteFacilityIssuesItems
            public static bool DeleteFacilityIssuesItems(string mIssueItemid, ref string ErrMsg)
            {
                ErrMsg = string.Empty;
                if (mIssueItemid.Trim() == string.Empty || mIssueItemid.Trim() == "0")
                    ErrMsg += "Invalid Indent/Issue No.<br />";
                else
                {
                    try
                    {
                        string strSQL = "Delete from tbFacilityOutwards Where IssueItemID = " + mIssueItemid;
                          
                        DBHelper.GetDataTable(strSQL);

                       

                       
                    }
                    catch
                    {
                        ErrMsg += "Delete not allowed, references found.<br />";
                    }
                }
                return (ErrMsg == string.Empty);
            }
            #endregion
        }
        #endregion

        #region DeleteIndent
        public static bool DeleteIndent1(string mIndentID, ref string ErrMsg)
        {
            ErrMsg = string.Empty;
            if (mIndentID.Trim() == string.Empty || mIndentID.Trim() == "0")
                ErrMsg += "Invalid Indent/Issue No.<br />";
            else
            {
                try
                {
                    string strSQL = "Delete from tbOutwards Where IndentItemID in (Select IndentItemID from tbIndentItems Where IndentID = " + mIndentID + ")";
                    DBHelper.GetDataTable(strSQL);
                    strSQL = "Delete from tbIndentItems where IndentID = " + mIndentID;
                    DBHelper.GetDataTable(strSQL);
                    strSQL = "Delete from tbIndents where IndentID = " + mIndentID;
                    DBHelper.GetDataTable(strSQL);
                }
                catch
                {
                    ErrMsg += "Delete not allowed, references found.<br />";
                }
            }
            return (ErrMsg == string.Empty);
        }
        #endregion
    }
    #endregion

    #region Facility Indent
    public class FacilityIndent
    {
        #region CheckAndCompleteIndent
        public static bool CheckAndCompleteIndent(string mIndentID, ref string ErrMsg)
        {
            ErrMsg = string.Empty;
            if (mIndentID.Trim() == string.Empty || mIndentID.Trim() == "0")
                ErrMsg += "Invalid Indent/Issue No.<br />";
            else
            {
                string strSQL = "Select a.FacIndentItemID, a.Needed from tbFacilityIndentItems a Where a.FacIndentID = " + mIndentID;
                DataTable dt = DBHelper.GetDataTable(strSQL);
                if (dt.Rows.Count == 0)
                    ErrMsg += "Add atleast one item under items tab. Status cannot be changed.<br />";

                DBHelper.ExecuteNonQuery("Delete tbFacilityIndentItems a Where Nvl(Needed,0) = 0 and a.FacIndentID = " + mIndentID);

            }
            if (ErrMsg == string.Empty)
            {
                string mIssueDate = GenFunctions.CheckDBNullForDate(DateTime.Now);
                GenFunctions.CheckDate(ref mIssueDate, "Issue Date", false);
                string strQuery = "Update tbFacilityIndents set Status = 'C' Where FacIndentID = " + mIndentID;
                DBHelper.ExecuteNonQuery(strQuery);
                return true;
            }
            else
                return false;
        }
        #endregion

        #region DeleteIndent
        public static bool DeleteIndent(string mIndentID, ref string ErrMsg)
        {
            ErrMsg = string.Empty;
            if (mIndentID.Trim() == string.Empty || mIndentID.Trim() == "0")
                ErrMsg += "Invalid Indent/Issue No.<br />";
            else
            {
                try
                {
                    string strSQL = "Delete from tbFacilityIndentItems where FacIndentID = " + mIndentID;
                    DBHelper.GetDataTable(strSQL);
                    strSQL = "Delete from tbFacilityIndents where FacIndentID = " + mIndentID;
                    DBHelper.GetDataTable(strSQL);
                }
                catch
                {
                    ErrMsg += "Delete not allowed, references found.<br />";
                }
            }
            return (ErrMsg == string.Empty);
        }
        #endregion
    }
    #endregion
}
