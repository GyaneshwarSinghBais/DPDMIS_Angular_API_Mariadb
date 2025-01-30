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
    public class Forecast
    {
        #region FillBudgetsByAccYear
        public static void FillBudgetsByAccYearBySourceByScheme(DropDownList ddl, string AccYrSetID, string SourceID, string SchemeID, bool AddAllBudgets, bool SelectFirst)
        {
            ddl.Items.Clear();
            string strSQL = "Select a.AccountCode, a.BudgetDetail, a.BudgetID" +
                " from frcstAnnualBudgets a" +
                " Where a.AccYrSetID = " + AccYrSetID + " and a.SourceID = " + SourceID + " and a.SchemeID = " + SchemeID +
                " Order By a.AccountCode";
            DataTable dt = DBHelper.GetDataTable(strSQL);

            ddl.DataTextField = "AccountCode";
            ddl.DataValueField = "BudgetID";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (AddAllBudgets)
                ddl.Items.Insert(0, new ListItem("All Budgets", "0"));
            if (SelectFirst && ddl.Items.Count > 0)
            {
                ListItem li = ddl.Items.FindByText(DateTime.Now.Year.ToString());
                if (li != null) li.Selected = true;
                if (ddl.SelectedIndex < 0) ddl.SelectedIndex = 0;
            }
        }
        #endregion

        #region FillAccYearsByAnnualBudget
        public static void FillAccYearsByAnnualBudget(DropDownList ddl, bool AddAllAccYear, bool SelectFirst)
        {
            ddl.Items.Clear();
            string strSQL = "Select Distinct a.AccYrSetID, a.AccYear, a.StartDate, a.EndDate from masAccYearSettings a, frcstAnnualBudgets b Where a.AccYrSetID = b.AccYrSetID Order By a.StartDate Desc";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            ddl.DataTextField = "AccYear";
            ddl.DataValueField = "AccYrSetID";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (AddAllAccYear)
                ddl.Items.Insert(0, new ListItem("All Fin. Years", "0"));
            if (SelectFirst && ddl.Items.Count > 0)
            {
                ListItem li = ddl.Items.FindByText(DateTime.Now.Year.ToString());
                if (li != null) li.Selected = true;
                if (ddl.SelectedIndex < 0) ddl.SelectedIndex = 0;
            }
        }
        #endregion

        #region FillSourcesByAnnualBudget
        public static void FillSourcesByAnnualBudget(DropDownList ddl, string AccYrSetID, bool AddAllSources, bool SelectFirst)
        {
            ddl.Items.Clear();
            if (AccYrSetID == "") AccYrSetID = "0";
            string strSQL = "Select Distinct a.SourceID, a.SourceName from masSources a, frcstAnnualBudgets b Where a.SourceID = b.SourceID and b.AccYrSetID = " + AccYrSetID + " Order By a.SourceName";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            ddl.DataTextField = "SourceName";
            ddl.DataValueField = "SourceID";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (AddAllSources)
                ddl.Items.Insert(0, new ListItem("All Sources", "0"));

            if (ddl.Page.Session["usrSourceID"] != null)
            {
                try
                {
                    ddl.SelectedValue = ddl.Page.Session["usrSourceID"].ToString();
                    ddl.Enabled = false;
                }
                catch
                {
                    ddl.Items.Clear();
                    ddl.Items.Add(new ListItem("Source not found", "0"));
                }
            }
            else if (SelectFirst && ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }
        #endregion

        #region FillSchemesByAnnualBudget
        public static void FillSchemesByAnnualBudget(DropDownList ddl, string AccYrSetID, string SourceID, bool AddAllSchemes, bool SelectFirst)
        {
            ddl.Items.Clear();
            if (AccYrSetID == "") AccYrSetID = "0";
            if (SourceID == "") SourceID = "0";
            string strSQL = "Select Distinct a.SchemeID, a.SchemeName from masSchemes a, frcstAnnualBudgets b Where a.SchemeID = b.SchemeID and b.AccYrSetID = " + AccYrSetID + " and b.SourceID = " + SourceID + " Order By a.SchemeName";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            ddl.DataTextField = "SchemeName";
            ddl.DataValueField = "SchemeID";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (AddAllSchemes)
                ddl.Items.Insert(0, new ListItem("All Schemes", "0"));

            if (ddl.Page.Session["usrSchemeID"] != null)
            {
                try
                {
                    ddl.SelectedValue = ddl.Page.Session["usrSchemeID"].ToString();
                    ddl.Enabled = false;
                }
                catch
                {
                    ddl.Items.Clear();
                    ddl.Items.Add(new ListItem("Scheme not found", "0"));
                }
            }
            else if (SelectFirst && ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }
        #endregion

        #region FillAccYearsByForecast
        public static void FillAccYearsByForecast(DropDownList ddl, bool AddAllAccYear, bool SelectFirst)
        {
            ddl.Items.Clear();
            string strSQL = "Select Distinct a.AccYrSetID, a.AccYear from masAccYearSettings a, frcstForecast b Where a.AccYrSetID = b.AccYrSetID and b.Status = 'FP' Order By a.AccYear Desc";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            ddl.DataTextField = "AccYear";
            ddl.DataValueField = "AccYrSetID";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (AddAllAccYear)
                ddl.Items.Insert(0, new ListItem("All Fin. Years", "0"));
            if (SelectFirst && ddl.Items.Count > 0)
            {
                ListItem li = ddl.Items.FindByText(DateTime.Now.Year.ToString());
                if (li != null) li.Selected = true;
                if (ddl.SelectedIndex < 0) ddl.SelectedIndex = 0;
            }
        }
        #endregion

        #region FillSourcesByForecast
        public static void FillSourcesByForecast(DropDownList ddl, string AccYrSetID, bool AddAllSources, bool SelectFirst)
        {
            ddl.Items.Clear();
            if (AccYrSetID == "") AccYrSetID = "0";
            string strSQL = "Select Distinct a.SourceID, a.SourceName from masSources a, frcstForecast b Where a.SourceID = b.SourceID and b.AccYrSetID = " + AccYrSetID + " and b.Status = 'FP' Order By a.SourceName";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            ddl.DataTextField = "SourceName";
            ddl.DataValueField = "SourceID";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (AddAllSources)
                ddl.Items.Insert(0, new ListItem("All Sources", "0"));

            if (ddl.Page.Session["usrSourceID"] != null)
            {
                try
                {
                    ddl.SelectedValue = ddl.Page.Session["usrSourceID"].ToString();
                    ddl.Enabled = false;
                }
                catch
                {
                    ddl.Items.Clear();
                    ddl.Items.Add(new ListItem("Source not found", "0"));
                }
            }
            else if (SelectFirst && ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }
        #endregion

        #region FillSchemesByForecast
        public static void FillSchemesByForecast(DropDownList ddl, string AccYrSetID, string SourceID, bool AddAllSchemes, bool SelectFirst)
        {
            ddl.Items.Clear();
            if (AccYrSetID == "") AccYrSetID = "0";
            if (SourceID == "") SourceID = "0";
            string strSQL = "Select Distinct a.SchemeID, a.SchemeName from masSchemes a, frcstForecast b Where a.SchemeID = b.SchemeID and b.AccYrSetID = " + AccYrSetID + " and b.SourceID = " + SourceID + " and b.Status = 'FP' Order By a.SchemeName";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            ddl.DataTextField = "SchemeName";
            ddl.DataValueField = "SchemeID";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (AddAllSchemes)
                ddl.Items.Insert(0, new ListItem("All Schemes", "0"));

            if (ddl.Page.Session["usrSchemeID"] != null)
            {
                try
                {
                    ddl.SelectedValue = ddl.Page.Session["usrSchemeID"].ToString();
                    ddl.Enabled = false;
                }
                catch
                {
                    ddl.Items.Clear();
                    ddl.Items.Add(new ListItem("Scheme not found", "0"));
                }
            }
            else if (SelectFirst && ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }
        #endregion

        #region ForecastByWarehouseParameters
        public struct ForecastByWarehouseParameters
        {
            public string BackPage;
            public string ForecastID;
            public string StateID;
        }

        public static ForecastByWarehouseParameters CreateForecastByWarehouseParameters(string BackPage, string ForecastID, string StateID)
        {
            ForecastByWarehouseParameters f = new ForecastByWarehouseParameters();
            f.BackPage = BackPage;
            f.ForecastID = ForecastID;
            f.StateID = StateID;
            return f;
        }
        #endregion

        #region ForecastByFacilityParameters
        public struct ForecastByFacilityParameters
        {
            public string BackPage;
            public string ForecastID;
            public string WarehouseID;
        }

        public static ForecastByFacilityParameters CreateForecastByFacilityParameters(string BackPage, string ForecastID, string WarehouseID)
        {
            ForecastByFacilityParameters f = new ForecastByFacilityParameters();
            f.BackPage = BackPage;
            f.ForecastID = ForecastID;
            f.WarehouseID = WarehouseID;
            return f;
        }
        #endregion

        #region RequirementParameters
        public struct RequirementParameters
        {
            public string BackPage;
            public string ForecastID;
            public string FacilityID;
            public string Mode;
        }

        public static RequirementParameters CreateRequirementParameters(string BackPage, string ForecastID, string FacilityID, string Mode)
        {
            RequirementParameters f = new RequirementParameters();
            f.BackPage = BackPage;
            f.ForecastID = ForecastID;
            f.FacilityID = FacilityID;
            f.Mode = Mode;
            return f;
        }
        #endregion

        #region ConsolidateRequirementsByPSAParameters
        public struct ConsolidateRequirementsByPSAParameters
        {
            public string BackPage;
            public string ForecastID;
            public string PSAID;
            public string Mode;
            public string RequirementCollectionLevel;
        }

        public static ConsolidateRequirementsByPSAParameters CreateConsolidateRequirementsByPSAParameters(string BackPage, string ForecastID, string PSAID, string Mode, string RequirementCollectionLevel)
        {
            ConsolidateRequirementsByPSAParameters f = new ConsolidateRequirementsByPSAParameters();
            f.BackPage = BackPage;
            f.ForecastID = ForecastID;
            f.PSAID = PSAID;
            f.Mode = Mode;
            f.RequirementCollectionLevel = RequirementCollectionLevel;
            return f;
        }
        #endregion

        #region ConsolidateRequirementsByWarehouseParameters
        public struct ConsolidateRequirementsByWarehouseParameters
        {
            public string BackPage;
            public string ForecastID;
            public string WarehouseID;
            public string Mode;
            public string RequirementCollectionLevel;
        }

        public static ConsolidateRequirementsByWarehouseParameters CreateConsolidateRequirementsByWarehouseParameters(string BackPage, string ForecastID, string WarehouseID, string Mode, string RequirementCollectionLevel)
        {
            ConsolidateRequirementsByWarehouseParameters f = new ConsolidateRequirementsByWarehouseParameters();
            f.BackPage = BackPage;
            f.ForecastID = ForecastID;
            f.WarehouseID = WarehouseID;
            f.Mode = Mode;
            f.RequirementCollectionLevel = RequirementCollectionLevel;
            return f;
        }
        #endregion
    }
}

    //#region Forecast
    //#endregion
//#region FillAccYearsByForecastSettings
//public static void FillAccYearsByForecastSettings(DropDownList ddl, bool AddAllAccYear, bool SelectFirst)
//{
//    ddl.Items.Clear();
//    string strSQL = "Select Distinct a.AccYrSetID, a.AccYear, a.StartDate, a.EndDate from masAccYearSettings a, frcstSettings b Where a.AccYrSetID = b.AccYrSetID Order By a.AccYear Desc";
//    DataTable dt = DBHelper.GetDataTable(strSQL);
//    ddl.DataTextField = "AccYear";
//    ddl.DataValueField = "AccYrSetID";
//    ddl.DataSource = dt;
//    ddl.DataBind();
//    if (AddAllAccYear)
//        ddl.Items.Insert(0, new ListItem("All Fin. Years", "0"));
//    if (SelectFirst && ddl.Items.Count > 0)
//    {
//        ListItem li = ddl.Items.FindByText(DateTime.Now.Year.ToString());
//        if (li != null) li.Selected = true;
//        if (ddl.SelectedIndex < 0) ddl.SelectedIndex = 0;
//    }
//}
//#endregion

//#region FillSourcesByForecastSettings
//public static void FillSourcesByForecastSettings(DropDownList ddl, string AccYrSetID, bool AddAllSources, bool SelectFirst)
//{
//    ddl.Items.Clear();
//    if (AccYrSetID == "") AccYrSetID = "0";
//    string strSQL = "Select Distinct a.SourceID, a.SourceName from masSources a, frcstSettings b Where a.SourceID = b.SourceID and b.AccYrSetID = " + AccYrSetID + " Order By a.SourceName";
//    DataTable dt = DBHelper.GetDataTable(strSQL);
//    ddl.DataTextField = "SourceName";
//    ddl.DataValueField = "SourceID";
//    ddl.DataSource = dt;
//    ddl.DataBind();
//    if (AddAllSources)
//        ddl.Items.Insert(0, new ListItem("All Sources", "0"));
//    if (SelectFirst && ddl.Items.Count > 0)
//        ddl.SelectedIndex = 0;
//}
//#endregion

//#region FillSchemesByForecastSettings
//public static void FillSchemesByForecastSettings(DropDownList ddl, string AccYrSetID, string SourceID, bool AddAllSchemes, bool SelectFirst)
//{
//    ddl.Items.Clear();
//    if (AccYrSetID == "") AccYrSetID = "0";
//    if (SourceID == "") SourceID = "0";
//    string strSQL = "Select Distinct a.SchemeID, a.SchemeName from masSchemes a, frcstSettings b Where a.SchemeID = b.SchemeID and b.AccYrSetID = " + AccYrSetID + " and b.SourceID = " + SourceID + " Order By a.SchemeName";
//    DataTable dt = DBHelper.GetDataTable(strSQL);
//    ddl.DataTextField = "SchemeName";
//    ddl.DataValueField = "SchemeID";
//    ddl.DataSource = dt;
//    ddl.DataBind();
//    if (AddAllSchemes)
//        ddl.Items.Insert(0, new ListItem("All Schemes", "0"));
//    if (SelectFirst && ddl.Items.Count > 0)
//        ddl.SelectedIndex = 0;
//}
//#endregion
