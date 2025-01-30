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
    #region Bid
    public class Bid
    {
        #region FillBidTypes
        public static void FillBidTypes(DropDownList ddl, bool AddAllBidTypes, bool SelectFirst)
        {
            ddl.Items.Clear();
            string strSQL = "Select BidTypeID, BidTypeName from bpBidTypes Order By BidTypeName";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            ddl.DataTextField = "BidTypeName";
            ddl.DataValueField = "BidTypeID";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (AddAllBidTypes)
                ddl.Items.Insert(0, new ListItem("All Bid Types", "0"));
            if (SelectFirst && ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }
        #endregion

        #region FillContractTypes
        public static void FillContractTypes(DropDownList ddl, bool AddAllContractTypes, bool SelectFirst)
        {
            ddl.Items.Clear();
            string strSQL = "Select ContractTypeID, ContractTypeName from bpContractTypes Order By ContractTypeName";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            ddl.DataTextField = "ContractTypeName";
            ddl.DataValueField = "ContractTypeID";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (AddAllContractTypes)
                ddl.Items.Insert(0, new ListItem("All Contract Types", "0"));
            if (SelectFirst)
                ddl.SelectedIndex = 0;
        }
        #endregion

        #region FillBidAdvertisementModes
        public static void FillBidAdvertisementModes(DropDownList ddl, bool AddAllBidAdvertisementModes, bool SelectFirst)
        {
            ddl.Items.Clear();
            string strSQL = "Select AdModeID, AdModeDescription from bpBidAdvertisementModes Order By AdModeDescription";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            ddl.DataTextField = "AdModeDescription";
            ddl.DataValueField = "AdModeID";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (AddAllBidAdvertisementModes)
                ddl.Items.Insert(0, new ListItem("All Advertisement Modes", "0"));
            if (SelectFirst && ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }
        #endregion

        #region GetAllBidDocCodes
        public static string[] GetAllBidDocCodes(string prefixText, int count, string contextKey)
        {
            if (contextKey == null) contextKey = "";
            return GetDBValues(prefixText, count, contextKey, "bpBidDocs", "BidDocCode");
        }
        #endregion

        #region GetAllBidDocNames
        public static string[] GetAllBidDocNames(string prefixText, int count, string contextKey)
        {
            if (contextKey == null) contextKey = "";
            return GetDBValues(prefixText, count, contextKey, "bpBidDocs", "BidDocName");
        }
        #endregion

        #region FillAccYearsByProcessedBids
        public static void FillAccYearsByProcessedBids(DropDownList ddl, bool AddAllAccYear, bool SelectFirst)
        {
            ddl.Items.Clear();
            string strSQL = "Select distinct a.AccYrSetID, a.AccYear, a.StartDate, a.EndDate from masAccYearSettings a inner join bpBidLeastPrice b on (b.AccYrSetID = a.AccYrSetID) Order By a.AccYear Desc";
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

        #region FillLotNumbersByProcurementPlanItemID
        public static void FillLotNumbersByProcurementPlanItemID(DropDownList ddl, string ProcurementPlanItemID, bool AddAllLotNumbers, bool SelectFirst)
        {
            ddl.Items.Clear();
            if (ProcurementPlanItemID == "") ProcurementPlanItemID = "0";
            string strFilters = "";
            strFilters += " and a.ProcurementPlanItemID = " + ProcurementPlanItemID;
            string strSQL = "Select a.LotID, a.LotNumber from bpLots a where 1=1 " + strFilters + " Order by a.LotNumber";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            ddl.DataTextField = "LotNumber";
            ddl.DataValueField = "LotID";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (AddAllLotNumbers)
                ddl.Items.Insert(0, new ListItem("All Lots", "0"));
            if (SelectFirst && ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }
        #endregion

        #region FillSourcesByProcessedBids
        public static void FillSourcesByProcessedBids(DropDownList ddl, string AccYrSetID, bool AddAllSources, bool SelectFirst)
        {
            ddl.Items.Clear();
            if (AccYrSetID == "") AccYrSetID = "0";
            string strFilters = "";
            strFilters += " and b.AccYrSetID = " + AccYrSetID;
            string strSQL = "Select distinct a.SourceID, a.SourceCode, a.SourceName from masSources a inner join bpBidLeastPrice b on (b.SourceID = a.SourceID) where 1=1 " + strFilters + " Order by a.SourceName";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            ddl.DataTextField = "SourceName";
            ddl.DataValueField = "SourceID";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (AddAllSources)
                ddl.Items.Insert(0, new ListItem("All Sources", "0"));
            if (SelectFirst && ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }
        #endregion

        #region FillSchemesByProcessedBids
        public static void FillSchemesByProcessedBids(DropDownList ddl, string AccYrSetID, string SourceID, bool AddAllSchemes, bool SelectFirst)
        {
            ddl.Items.Clear();
            if (AccYrSetID == "") AccYrSetID = "0";
            if (SourceID == "") SourceID = "0";
            string strFilters = "";
            strFilters += " and b.AccYrSetID = " + AccYrSetID;
            strFilters += " and b.SourceID = " + SourceID;
            string strSQL = "Select distinct a.SchemeID, a.SchemeName from masSchemes a inner join bpBidLeastPrice b on (b.SchemeID = a.SchemeID) where 1=1 " + strFilters + " Order By a.SchemeName";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            ddl.DataTextField = "SchemeName";
            ddl.DataValueField = "SchemeID";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (AddAllSchemes)
                ddl.Items.Insert(0, new ListItem("All Schemes", "0"));
            if (SelectFirst && ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }
        #endregion

    }
    #endregion

   

}
        //#region GetAllBidTypeCodes
        //public static string[] GetAllBidTypeCodes(string prefixText, int count, string contextKey)
        //{
        //    if (contextKey == null) contextKey = "";
        //    return GetDBValues(prefixText, count, contextKey, "bpBidTypes", "BidTypeCode");
        //}
        //#endregion

        //#region GetAllBidTypeNames
        //public static string[] GetAllBidTypeNames(string prefixText, int count, string contextKey)
        //{
        //    if (contextKey == null) contextKey = "";
        //    return GetDBValues(prefixText, count, contextKey, "bpBidTypes", "BidTypeName");
        //}
        //#endregion


        //#region FillSourcesByProcessedBids
        //public static void FillSourcesByProcessedBids(DropDownList ddl, string AccYrSetID, bool AddAllSources, bool SelectFirst)
        //{
        //    ddl.Items.Clear();
        //    if (AccYrSetID == "") AccYrSetID = "0";
        //    string strFilters = "";
        //    strFilters += " and b.AccYrSetID = " + AccYrSetID;
        //    string strSQL = "Select distinct a.SourceID, a.SourceCode, a.SourceName from masSources a inner join bpBidLeastPrice b on (b.SourceID = a.SourceID) where 1=1 " + strFilters + " Order by a.SourceName";
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

        //#region FillSchemesByProcessedBids
        //public static void FillSchemesByProcessedBids(DropDownList ddl, string AccYrSetID, string SourceID, bool AddAllSchemes, bool SelectFirst)
        //{
        //    ddl.Items.Clear();
        //    if (AccYrSetID == "") AccYrSetID = "0";
        //    if (SourceID == "") SourceID = "0";
        //    string strFilters = "";
        //    strFilters += " and b.AccYrSetID = " + AccYrSetID;
        //    strFilters += " and b.SourceID = " + SourceID;
        //    string strSQL = "Select distinct a.SchemeID, a.SchemeName from masSchemes a inner join bpBidLeastPrice b on (b.SchemeID = a.SchemeID) where 1=1 " + strFilters + " Order By a.SchemeName";
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

        //#region FillBidNumbersByProcessedBids
        //public static void FillBidNumbersByProcessedBids(DropDownList ddl, string AccYrSetID, string SourceID, string SchemeID, bool AddAllBidNumbers, bool SelectFirst)
        //{
        //    ddl.Items.Clear();
        //    if (AccYrSetID == "") AccYrSetID = "0";
        //    if (SourceID == "") SourceID = "0";
        //    if (SchemeID == "") SchemeID = "0";
        //    string strFilters = "";
        //    strFilters += " and b.AccYrSetID = " + AccYrSetID;
        //    strFilters += " and b.SourceID = " + SourceID;
        //    strFilters += " and b.SchemeID = " + SchemeID;
        //    string strSQL = "Select distinct a.BidID, a.BidCode as BidNumber from bpBids a inner join bpBidLeastPrice b on (b.BidID = a.BidID) where 1=1 " + strFilters + " Order By a.BidCode";
        //    DataTable dt = DBHelper.GetDataTable(strSQL);
        //    ddl.DataTextField = "BidNumber";
        //    ddl.DataValueField = "BidID";
        //    ddl.DataSource = dt;
        //    ddl.DataBind();
        //    if (AddAllBidNumbers)
        //        ddl.Items.Insert(0, new ListItem("All Bid Numbers", "0"));
        //    if (SelectFirst && ddl.Items.Count > 0)
        //        ddl.SelectedIndex = 0;
        //}
        //#endregion

