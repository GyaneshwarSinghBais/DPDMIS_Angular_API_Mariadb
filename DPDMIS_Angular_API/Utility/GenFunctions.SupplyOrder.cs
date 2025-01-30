#region Using
using System;
using System.Data;
using System.Drawing;
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
    #region SupplyOrder
    public class SupplyOrder
    {
        #region FillPoNo
        public static void FillPoNo(DropDownList ddl, bool AddAllPoNo, bool SelectFirst, string Conditions)
        {
            ddl.Items.Clear();
            string strSQL = "Select PoNoID, PoNo from soOrderPlaced where 1=1 " + Conditions + " Order By PoNo";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            ddl.DataTextField = "PoNo";
            ddl.DataValueField = "PoNoID";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (AddAllPoNo)
                ddl.Items.Insert(0, new ListItem("All Supply Orders", "0"));
            if (SelectFirst && ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }
        #endregion

        #region FillSanctionNumbersByBid
        public static void FillBudgetReleaseNumbersByAccYearBySourceByScheme(DropDownList ddl, string AccYrSetID, string SourceID, string SchemeID, bool AddAllBudgetReleaseNumbers, bool SelectFirst)
        {
            ddl.Items.Clear();
            if (AccYrSetID == "") AccYrSetID = "0";
            if (SourceID == "") SourceID = "0";
            if (SchemeID == "") SchemeID = "0";
            string strFilters = "";
            strFilters += " and a.AccYrSetID = " + AccYrSetID;
            strFilters += " and a.SourceID = " + SourceID;
            strFilters += " and a.SchemeID = " + SchemeID;
            string strSQL = "Select b.BudgetReleaseID, b.ReleaseNumber from soBudgetReleases b Inner Join frcstAnnualBudgets a on (a.BudgetID = b.BudgetID) where 1=1 " + strFilters + " Order By b.ReleaseNumber";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            ddl.DataTextField = "ReleaseNumber";
            ddl.DataValueField = "BudgetReleaseID";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (AddAllBudgetReleaseNumbers)
                ddl.Items.Insert(0, new ListItem("All Budget Release Numbers", "0"));
            if (SelectFirst && ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }

        public static void FillBudgetNumbersByAccYearBySourceByScheme(DropDownList ddl, string AccYrSetID, string SourceID, 
            string SchemeID, bool AddAllBudgetReleaseNumbers, bool SelectFirst)
        {
            ddl.Items.Clear();

            if (AccYrSetID == "") AccYrSetID = "0";
            if (SourceID == "") SourceID = "0";
            if (SchemeID == "") SchemeID = "0";
            
            string strFilters = "";
            strFilters += " and a.AccYrSetID = " + AccYrSetID;
            strFilters += " and a.SourceID = " + SourceID;
            strFilters += " and a.SchemeID = " + SchemeID;

            string strSQL = "Select a.BudgetID, a.AccountCode" +
                " from frcstAnnualBudgets a"+
                " Where 1=1 " + strFilters +
                " Order By a.AccountCode";
            DataTable dt = DBHelper.GetDataTable(strSQL);

            ddl.DataTextField = "AccountCode";
            ddl.DataValueField = "BudgetID";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (AddAllBudgetReleaseNumbers)
                ddl.Items.Insert(0, new ListItem("All Budget Numbers", "0"));
            if (SelectFirst && ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }

        public static void FillBudgetsByPSA(DropDownList ddl, string AccYrSetID, string SourceID,
            string SchemeID, string PSAID, bool AddAllBudgetReleaseNumbers, bool SelectFirst)
        {
            ddl.Items.Clear();

            if (AccYrSetID == "") AccYrSetID = "0";
            if (SourceID == "") SourceID = "0";
            if (SchemeID == "") SchemeID = "0";

            string strFilters = "";
            strFilters += " and a.AccYrSetID = " + AccYrSetID;
            strFilters += " and a.SourceID = " + SourceID;
            strFilters += " and a.SchemeID = " + SchemeID;
            strFilters += " and a1.PSAID = " + PSAID;

            string strSQL = "Select a.BudgetID, a.AccountCode" +
                " From frcstAnnualBudgets a" +
                " Inner Join V_frcstAnnualBudgetPsas a1 on (a1.BudgetID=a.BudgetID)" +
                " Where a.IsProvisionalBudget=0" + strFilters +
                " Order By a.AccountCode";
            DataTable dt = DBHelper.GetDataTable(strSQL);

            ddl.DataTextField = "AccountCode";
            ddl.DataValueField = "BudgetID";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (AddAllBudgetReleaseNumbers)
                ddl.Items.Insert(0, new ListItem("All Budget Numbers", "0"));
            if (SelectFirst && ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }
        #endregion

        #region FillTranchesByPoNoID
        public static void FillTranchesByPoNoID(DropDownList ddl, string PoNoID, bool AddAllTranches, bool SelectFirst)
        {
            FillTranchesByPoNoID(ddl, PoNoID, AddAllTranches, SelectFirst, "");
        }
        public static void FillTranchesByPoNoID(DropDownList ddl, string PoNoID, bool AddAllTranches, bool SelectFirst, string WhereConditions)
        {
            ddl.Items.Clear();
            if (PoNoID == "") PoNoID = "0";
            string strFilters = "";
            strFilters += " and PoNoID = " + PoNoID;
            strFilters += WhereConditions;
            string strSQL = "Select DurationID, Duration as TDuration, DType, Tranche from soTranches where 1=1" + strFilters + " Order By DType, Duration, Tranche";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            ddl.DataTextField = "Tranche";
            ddl.DataValueField = "DurationID";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (AddAllTranches)
                ddl.Items.Insert(0, new ListItem("All Tranches", "0"));
            if (SelectFirst && ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }
        #endregion

        #region FillStatesByPoNo
        public static void FillStatesByPoNo(DropDownList ddl, string mPoNoID, bool AddAllStates, bool SelectFirst)
        {
            if (mPoNoID == "") mPoNoID = "0";
            Masters.FillStates(ddl, AddAllStates, SelectFirst, " and StateID in (Select Distinct m1.StateID from soWHDist a Inner Join soOrderedItems a1 on (a1.OrderItemID = a.OrderItemID) Inner Join masWarehouses m1 on (m1.WarehouseID = a.WarehouseID) Where a1.PoNoID = " + mPoNoID + ")");
        }
        #endregion

        #region FillWarehousesByPoNo (By State)
        public static void FillWarehousesByPoNo(DropDownList ddl, string mStateID, string mPoNoID, bool AddAllWarehouses, bool SelectFirst)
        {
            if (mStateID == "") mStateID = "0";
            if (mPoNoID == "") mPoNoID = "0";
            Masters.FillWarehouses(ddl, mStateID, AddAllWarehouses, SelectFirst, " and WarehouseID in (Select Distinct a.WarehouseID from soWHDist a Inner Join soOrderedItems a1 on (a1.OrderItemID = a.OrderItemID) Inner Join masWarehouses m1 on (m1.WarehouseID = a.WarehouseID) Where a1.PoNoID = " + mPoNoID + " and m1.StateID = " + mStateID + ")");
        }
        #endregion

        #region  ReconcileIssueDate
        public static void ReconcileIssueDate(string PonoID)
        {
            string SoIssueDate = GenFunctions.CheckDBNullForDate(DBHelper.ExecuteScalarQuery("Select Nvl((Select To_Char(PoDate,'dd-mm-yyyy') SoIssueDate from soOrderPlaced Where PoNoID = @PoNoID),Null) from dual".Replace("@PoNoID", PonoID)));
            if (SoIssueDate != "") { ReconcileIssueDate(PonoID,SoIssueDate,null); }
        }
        public static void ReconcileIssueDate(string PonoID, string SoIssueDate, Label lblMsg)
        {
            string mPoNoID = PonoID;
            string mSupplyStartDate = SoIssueDate;
            string mPoDate = "";
            string strSQLC = "Select PoDate from soOrderPlaced Where PoNoID = " + mPoNoID;
            DataTable dtC = DBHelper.GetDataTable(strSQLC);
            if (dtC.Rows.Count > 0)
                mPoDate = GenFunctions.CheckDBNullForDate(dtC.Rows[0]["PoDate"]);

            string ErrMsg = "";
            ErrMsg += GenFunctions.CheckDateGreaterThanToday(mSupplyStartDate, "Supply Order Issue Date");
            ErrMsg += GenFunctions.CheckDateByComparison(mSupplyStartDate, "Supply Order Issue Date", mPoDate, "Supply order date", "", "");
            ErrMsg += GenFunctions.CheckDate(ref mSupplyStartDate, "Supply Order Issue Date", false);
            ErrMsg += GenFunctions.CheckNumber(ref mPoNoID, "Supply Order", false);
            if (lblMsg != null && ErrMsg != "")
            {
                return;
            }

            string strSQL = "Update soOrderPlaced set SOIssueDate = " + mSupplyStartDate + " where PoNoID = " + mPoNoID;
            DBHelper.GetDataTable(strSQL);
            if (SoIssueDate.Trim() == "")
            {
                strSQL = "Update soTranches set ExpDate = null Where PoNoID = " + mPoNoID;
                DBHelper.GetDataTable(strSQL);
            }
            else
            {
                strSQL = "Update soTranches set ExpDate = Case DType When 'D' then (" + mSupplyStartDate + " + Duration) When 'M' then Add_Months(" + mSupplyStartDate + ", Duration) else (" + mSupplyStartDate + "+(Duration*7)) End Where PoNoID = " + mPoNoID;
                DBHelper.GetDataTable(strSQL);
            }
    }
        #endregion

        #region CheckSupplyOrder
        public static bool CheckSupplyOrder(string mPoNoID, bool CheckSchedule, Label lblMsg)
        {
            string ErrMsg = "";
            if (mPoNoID == "0" || mPoNoID == "")
            {
                ErrMsg += "Invalid Supply Order. Try again<br />";
            }
            else
            {
                try
                {
                    string strSQL = "Delete from soWHDist Where NVL(AbsQty, 0) = 0 and OrderItemID in (Select OrderItemID from soOrderedItems Where PoNoID = " + mPoNoID + ")";
                    DBHelper.GetDataTable(strSQL);
                    strSQL = "Delete from soOrderDistribution Where NVL(AbsQty, 0) = 0 and OrderItemID in (Select OrderItemID from soOrderedItems Where PoNoID = " + mPoNoID + ")";
                    DBHelper.GetDataTable(strSQL);
                    strSQL = "Delete from soOrderedItems Where NVL(AbsQty, 0) = 0 and PoNoID = " + mPoNoID;
                    DBHelper.GetDataTable(strSQL);

                    strSQL = "Select a.OrderItemID, a.AbsQty from soOrderedItems a Where a.PoNoID = " + mPoNoID;
                    DataTable dt = DBHelper.GetDataTable(strSQL);
                    if (dt.Rows.Count == 0)
                        ErrMsg += "Add atleast one item under items tab. Status cannot be changed.<br />";

                    strSQL = "Select Nvl((Select Count(*) from SoOrderPlaced So Inner Join SoOrderedItems SI on (SI.PoNoID =  So.PoNoID) Where SI.PoNoID = @PoNoID and So.AmendNo = 0 and So.Status = 'IN' and Not Exists (Select * from V_ActiveContractActiveItems CI Where CI.ContractItemID = SI.ContractItemID)),0)Count from dual".Replace("@PoNoID",mPoNoID);
                    decimal Count = (decimal)DBHelper.ExecuteScalarQuery(strSQL);

                    ErrMsg += (Count > 0) ? "Expired contract items are found in this Supply Order.<br />" : "";

                    strSQL = "Select Nvl((Select Sum(Nvl(SI.ItemValue,0)) from SoOrderPlaced So Inner Join SoOrderedItems SI on (SI.PoNoID =  So.PoNoID) Where SI.PoNoID = @PoNoID),0) SoValue from dual".Replace("@PoNoID", mPoNoID);
                    decimal TotItemValue = (decimal)DBHelper.ExecuteScalarQuery(strSQL);

                    ErrMsg += (TotItemValue > 0) ? "" : "Supply Order value should not be zero or negative.<br />";

                    if (CheckSchedule)
                    {
                        strSQL = "Select a.OrderItemID, a.AbsQty from soOrderedItems a Left Outer Join soWHDist b on (b.OrderItemID = a.OrderItemID) Where a.PoNoID = " + mPoNoID + " Group By a.OrderItemID, a.AbsQty Having (NVL(a.AbsQty, 0) = 0 or NVL(a.AbsQty, 0) != NVL(Sum(NVL(b.AbsQty, 0)), 0))";
                        dt = DBHelper.GetDataTable(strSQL);
                        if (dt.Rows.Count > 0)
                            ErrMsg += "All items should be distributed properly. Status cannot be changed.<br />";
                    }
                    
                }
                catch (Exception Exp)
                {
                    ErrMsg += Exp.Message + "<br />";
                }
            }

            if (ErrMsg != "")
            {
                lblMsg.Text = ErrMsg;
                lblMsg.ForeColor = Color.Red;
                return false;
            }
            else
                return true;
        }
        #endregion

        #region FillBudgetsByYearBySourceByScheme
        public static void FillBudgetsBySourceByScheme(DropDownList ddl, string SourceID, string SchemeID, bool AddAll, bool SelectFirst)
        {
            ddl.Items.Clear();
            if (SourceID == "") SourceID = "0";
            if (SchemeID == "") SchemeID = "0";

            string strFilters = "";
            strFilters += " and a.SourceID = " + SourceID;
            strFilters += " and a.SchemeID = " + SchemeID;

            string strSQL = "Select a.BudgetID, a.AccountCode" +
                " From frcstAnnualBudgets a" +
                " Left Outer Join soOrderPlaced b on (b.BudgetID=a.BudgetID and b.Status!='I')" +
                " where a.Status!='IN' " + strFilters +
                " Group by a.BudgetID,a.AccountCode,a.AllottedAmount" +
                " having a.AllottedAmount > Sum(nvl(b.SOValue,0))" +
                " Order By a.AccountCode";
            DataTable dt = DBHelper.GetDataTable(strSQL);

            ddl.DataTextField = "AccountCode";
            ddl.DataValueField = "BudgetID";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (AddAll)
                ddl.Items.Insert(0, new ListItem("All Budgets", "0"));
            if (SelectFirst && ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }
        #endregion

        #region FillPSABudgetsBySourceByScheme
        public static void FillPSABudgetsBySourceByScheme(DropDownList ddl, string PSAID, string AccYrSetID, string SourceID, string SchemeID, string CategoryID,double CurrentSoValue,
            bool AddAll, bool SelectFirst)
        {
            ddl.Items.Clear();
            if (SourceID == "") SourceID = "0";
            if (SchemeID == "") SchemeID = "0";
            if (AccYrSetID == "") AccYrSetID = "0";

            string strFilters = "";
            strFilters += " and a.SourceID = " + SourceID;
            strFilters += " and a.SchemeID = " + SchemeID;
            strFilters += " and a.AccYrSetID = " + AccYrSetID;
            strFilters += " and a1.PSAID = " + PSAID;
            strFilters += " and a1.CategoryID = " + CategoryID;
            strFilters += " and a.IsProvisionalBudget = 0";

            string strSQL = "Select a.BudgetID, a.AccountCode, a.BudgetDetail" +
                " From frcstAnnualBudgets a" +
                " Inner Join V_frcstAnnualBudgetPsas a1 on (a1.BudgetID=a.BudgetID)" +
                " Left Outer Join soOrderPlaced b on (b.BudgetID=a.BudgetID and b.PSAID=a1.PSAID and b.Status!='I' and b.CategoryID = a1.CategoryID)" +
                " where a.Status!='IN' " + strFilters +
                " Group by a.BudgetID,a.AccountCode,a.IsProvisionalBudget,a.BudgetDetail,a1.DHFWBudgetAmount" +
                " having nvl(a1.DHFWBudgetAmount,0) > Sum(nvl(b.SOValue,0)) and nvl(a1.DHFWBudgetAmount,0) - Sum(nvl(b.SOValue,0)) >= " + CurrentSoValue.ToString() +
                " Order By a.AccountCode";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            ListItem Item ;
            foreach( DataRow iRow in dt.Rows)
            {
                Item = new ListItem(iRow["AccountCode"].ToString(),iRow["BudgetID"].ToString());
                Item.Attributes.Add("Title",iRow["BudgetDetail"].ToString());
                ddl.Items.Add(Item);
            }
            if (AddAll)
                ddl.Items.Insert(0, new ListItem("All Budgets", "0"));
            if (SelectFirst && ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }
        #endregion

        #region FillOrderCancelReasons
        public static void FillOrderCancelReasons(DropDownList ddl, bool AddOthers, bool SelectFirst, string Conditions)
        {
            ddl.Items.Clear();
            string strSQL = "Select ReasonID, Description from soOrderCancellationReason where IsActive = 1 " + Conditions + " Order By Description";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            ddl.DataTextField = "Description";
            ddl.DataValueField = "ReasonID";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (AddOthers)
                ddl.Items.Insert(ddl.Items.Count, new ListItem("Other", "0"));
            if (SelectFirst && ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }
        #endregion
    }
    #endregion
}
