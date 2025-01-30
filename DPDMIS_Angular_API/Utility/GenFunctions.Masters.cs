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

#region Enumerations
public enum SupplierType { MOHSuppliersOnly = 0, LocalSuppliersOnly = 1 };
#endregion

public partial class GenFunctions
{
    #region Masters
    public class Masters
    {
        #region FillSources
        public static void FillSources(DropDownList ddl, bool AddAllSources, bool SelectFirst)
        {
            FillSources(ddl, AddAllSources, SelectFirst, "");
        }
        #endregion

        #region FillSources With Conditions
        public static void FillSources(DropDownList ddl, bool AddAllSources, bool SelectFirst, string Conditions)
        {
            ddl.Items.Clear();
            string strSQL = "Select SourceID, SourceName from masSources Where 1=1" + Conditions + " Order By Upper(SourceName)";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            ddl.DataTextField = "SourceName";
            ddl.DataValueField = "SourceID";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (AddAllSources)
            {
                ddl.Items.Insert(0, new ListItem("All Sources", "0"));
                
            }
            if (ddl.Page.Session["usrSourceID"] != null)
            {
                try
                {
                    ddl.Enabled = false;
                    ddl.SelectedValue = ddl.Page.Session["usrSourceID"].ToString();
                }
                catch
                {
                    ddl.Items.Insert(0, new ListItem("Source not available", "0"));
                    ddl.SelectedIndex = 0;
                }
            }
            else if (SelectFirst && ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }
        #endregion

        #region FillSchemes
        public static void FillSchemes(DropDownList ddl, bool AddAllSchemes, bool SelectFirst)
        {
            FillSchemes(ddl, AddAllSchemes, SelectFirst, "");
        }
        #endregion

        #region FillSchemes With Conditions
        public static void FillSchemes(DropDownList ddl, bool AddAllSchemes, bool SelectFirst, string Conditions)
        {
            ddl.Items.Clear();
            string strSQL = "Select SchemeID, SchemeName from masSchemes Where 1=1" + Conditions + " Order By Upper(SchemeName)";
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
                    ddl.Enabled = false;
                    ddl.SelectedValue = ddl.Page.Session["usrSchemeID"].ToString();
                }
                catch
                {
                    ddl.Items.Insert(0, new ListItem("Scheme not available", "0"));
                    ddl.SelectedIndex = 0;
                }
            }
            else if (SelectFirst && ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }
        #endregion

        #region FillPSAs
        public static void FillPSAs(DropDownList ddl, bool AddAllPSAs, bool SelectFirst)
        {
            ddl.Items.Clear();
            string strSQL = "Select PSAID, PSAName from masPSAs Order By PSAName";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            ddl.DataTextField = "PSAName";
            ddl.DataValueField = "PSAID";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (AddAllPSAs)
                ddl.Items.Insert(0, new ListItem("All Authority", "0"));

            if (ddl.Page.Session["usrPSAID"] != null)
            {
                ddl.SelectedValue = ddl.Page.Session["usrPSAID"].ToString();
                ddl.Enabled = false;
            }
            else if (SelectFirst && ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }
        #endregion

        #region FillPSAsByDivisionByPsaType
        public static void FillPSAsByDivisionByPsaType(DropDownList ddl, string DivisionID, string PsaTypeID, bool AddAllPSAs, bool SelectFirst)
        {
            ddl.Items.Clear();
            string WhereCondition = "";
            DivisionID = GenFunctions.CheckEmptyString(DivisionID,"0");
            PsaTypeID = GenFunctions.CheckEmptyString(PsaTypeID, "0");
            if (DivisionID != "0") { WhereCondition += " and Dist.DivisionID=" + DivisionID; }
            if (PsaTypeID != "0") { WhereCondition += " and Ware.WarehouseTypeID=" + PsaTypeID; }
            
            string strSQL = "Select Psa.Psaname,Psa.Psaid from MasPSAs Psa "
                          + "Inner Join MasDistricts Dist on (Dist.DistrictID = Psa.DistrictID) "
                          + "Inner Join MasWarehouses Ware on (Ware.WarehouseID = Psa.WarehouseID) "
                          + "where 1 =1" + WhereCondition;

            DataTable dt = DBHelper.GetDataTable(strSQL);
            ddl.DataTextField = "PSAName";
            ddl.DataValueField = "PSAID";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (AddAllPSAs)
                ddl.Items.Insert(0, new ListItem("All Warehouses", "0"));
            
            if (SelectFirst && ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }
        #endregion

        #region FillPSAsByDistrict
        public static void FillPSAsByDistrict(DropDownList ddl, string DistrictID, bool AddAllPSAs, bool SelectFirst)
        {
            ddl.Items.Clear();
            string WhereCondition = "";
            DistrictID = GenFunctions.CheckEmptyString(DistrictID, "0");
            if (DistrictID != "0") { WhereCondition += " and Dist.DistrictID = " + DistrictID; }
            string strSQL = "Select Psa.Psaname,Psa.Psaid from MasPSAs Psa " +
                           " Inner Join MasDistricts Dist on (Dist.DistrictID = Psa.DistrictID)" +
                           " where 1 =1" + WhereCondition;

            DataTable dt = DBHelper.GetDataTable(strSQL);
            ddl.DataTextField = "PSAName";
            ddl.DataValueField = "PSAID";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (AddAllPSAs)
                ddl.Items.Insert(0, new ListItem("All PSAs", "0"));

            if (SelectFirst && ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }
        #endregion

        #region FillAccYears
        public static void FillAccYears(DropDownList ddl, bool AddAllAccYear, bool SelectFirst)
        {
            FillAccYears(ddl, AddAllAccYear, SelectFirst, "");
        }

        public static void FillAccYears(DropDownList ddl, bool AddAllAccYear, bool SelectFirst, string mWhereClause)
        {
            ddl.Items.Clear();
            string strSQL = "Select AccYrSetID, AccYear, StartDate, EndDate from masAccYearSettings Where 1=1 and accyrsetid>=533 " + mWhereClause + " Order By AccYear Desc";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            ddl.DataTextField = "AccYear";
            ddl.DataValueField = "AccYrSetID";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (AddAllAccYear)
                ddl.Items.Insert(0, new ListItem("All Fin. Years", "0"));
            if (SelectFirst && ddl.Items.Count > 0)
            {
                AccYearInfo AccountYear = new AccYearInfo(DateTime.Now);
                ListItem li = ddl.Items.FindByText(AccountYear.AccYear);
                if (li != null) li.Selected = true;
                if (ddl.SelectedIndex < 0) ddl.SelectedIndex = 0;
            }
        }
        #endregion

        #region FillWareHouse
        public static void FillWareHouse(DropDownList ddl, bool AddAllWarehouse, bool SelectFirst)
        {
            ddl.Items.Clear();
            string strSQL = "select warehouseid,warehousename from maswarehouses order by warehousename";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            ddl.DataTextField = "warehousename";
            ddl.DataValueField = "warehouseid";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (AddAllWarehouse)
                ddl.Items.Insert(0, new ListItem("All WareHouse", "0"));
            if (SelectFirst && ddl.Items.Count > 0)
            {
                if (ddl.SelectedIndex < 0) ddl.SelectedIndex = 0;
            }
        }
        #endregion
        #endregion

        #region GetAccYear
        public static string GetAccYear(string AccYrSetID)
        {
            if (AccYrSetID == "") AccYrSetID = "0";
            string strSQL = "select AccYear from MasAccYearSettings where AccYrSetID = " + AccYrSetID;
            DataTable dt = DBHelper.GetDataTable(strSQL);
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0][0].ToString();
            }
            else
                return "";
        }
        #endregion

        #region GetSouceName
        public static string GetSouceName(string SourceID)
        {
            if (SourceID == "") SourceID = "0";
            string strSQL = "Select SourceName from masSources where SourceID = " + SourceID;
            DataTable dt = DBHelper.GetDataTable(strSQL);
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0][0].ToString();
            }
            else
                return "";
        }
        #endregion

        #region GetDirectorateName
        public static string GetDirectorateName(string SourceID)
        {
            if (SourceID == "") SourceID = "0";
            string strSQL = "Select DirectorateName from masSources where SourceID = " + SourceID;
            DataTable dt = DBHelper.GetDataTable(strSQL);
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0][0].ToString();
            }
            else
                return "";
        }
        #endregion

        #region GetSchemeName
        public static string GetSchemeName(string SchemeID)
        {
            if (SchemeID == "") SchemeID = "0";
            string strSQL = "Select SchemeName from masSchemes where SchemeID = " + SchemeID;
            DataTable dt = DBHelper.GetDataTable(strSQL);
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0][0].ToString();
            }
            else
                return "";
        }
        #endregion

        #region GetStateName
        public static string GetStateName(string StateID)
        {
            if (StateID == "") StateID = "0";
            string strSQL = "Select StateName from masStates where StateID = " + StateID;
            DataTable dt = DBHelper.GetDataTable(strSQL);
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0][0].ToString();
            }
            else
                return "";
        }
        #endregion

        #region GetDivisionName
        public static string GetDivisionName(string DivisionID)
        {
            if (DivisionID == "") DivisionID = "0";
            string strSQL = "Select DivisionName from MasDivisions where DivisionID = " + DivisionID;
            DataTable dt = DBHelper.GetDataTable(strSQL);
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0][0].ToString();
            }
            else
                return "";
        }
        #endregion

        #region GetWarehouseName
        public static string GetWarehouseName(string WarehouseID)
        {
            if (WarehouseID == "") WarehouseID = "0";
            string strSQL = "Select WarehouseName from masWarehouses where WarehouseID = " + WarehouseID;
            DataTable dt = DBHelper.GetDataTable(strSQL);
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0][0].ToString();
            }
            else
                return "";
        }
        #endregion

        #region GetEmailByPsa
        public static string GetEmailByPsa(string PsaID)
        {
           return GetEmailByWarehouse(GetWarehouseIDByPsa(PsaID));
        }
        #endregion

        #region GetWarehouseIDByPsa
        public static string GetWarehouseIDByPsa(string PsaID)
        {
            if (PsaID == "") PsaID = "0";
            string strSQL = "Select WarehouseID from masPsas where PsaID = " + PsaID;
            DataTable dt = DBHelper.GetDataTable(strSQL);
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0][0].ToString();
            }
            else
                return "";
        }
        #endregion

        #region GetEmailByWarehouseID
        public static string GetEmailByWarehouse(string WarehouseID)
        {
            if (WarehouseID == "") WarehouseID = "0";
            string strSQL = "Select Email from masWarehouses where WarehouseID = " + WarehouseID;
            DataTable dt = DBHelper.GetDataTable(strSQL);
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0][0].ToString();
            }
            else
                return "";
        }
        #endregion

        #region GetEmailBySupplierID
        public static string GetEmailBySupplierID(string SupplierID)
        {
            if (SupplierID == "") SupplierID = "0";
            string strSQL = "Select Email from masSuppliers where SupplierID = " + SupplierID;
            DataTable dt = DBHelper.GetDataTable(strSQL);
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0][0].ToString();
            }
            else
                return "";
        }
        #endregion

        #region GetStateNameByWarehouseID
        public static string GetStateNameByWarehouseID(string WarehouseID)
        {
            if (WarehouseID == "") WarehouseID = "0";
            string strSQL = "Select b.StateName from masWarehouses a inner join masStates b on (b.StateID = a.StateID) where a.WarehouseID = " + WarehouseID;
            DataTable dt = DBHelper.GetDataTable(strSQL);
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0][0].ToString();
            }
            else
                return "";
        }
        #endregion

        #region GetFacilityName
        public static string GetFacilityName(string FacilityID)
        {
            if (FacilityID == "") FacilityID = "0";
            string strSQL = "Select FacilityName from masFacilities where FacilityID = " + FacilityID;
            DataTable dt = DBHelper.GetDataTable(strSQL);
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0][0].ToString();
            }
            else
                return "";
        }
        #endregion

        #region GetStateNameByFacilityID
        public static string GetStateNameByFacilityID(string FacilityID)
        {
            if (FacilityID == "") FacilityID = "0";
            string strSQL = "Select b.StateName from masFacilities a inner join masStates b on (b.StateID = a.StateID) where a.FacilityID = " + FacilityID;
            DataTable dt = DBHelper.GetDataTable(strSQL);
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0][0].ToString();
            }
            else
                return "";
        }
        #endregion

        #region GetCategoryName
        public static string GetCategoryName(string CategoryID)
        {
            if (CategoryID == "") CategoryID = "0";
            string strSQL = "Select CategoryName from masItemCategories where CategoryID = " + CategoryID;
            DataTable dt = DBHelper.GetDataTable(strSQL);
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0][0].ToString();
            }
            else
                return "";
        }
        #endregion

        #region FillStates
        public static void FillStates(DropDownList ddl, bool AddAllStates, bool SelectFirst)
        {
            Masters.FillStates(ddl, AddAllStates, SelectFirst, "", false);
        }
        #endregion

        #region FillStates (By Condition)
        /// <summary>
        /// To fill states
        /// </summary>
        /// <param name="ddl">DropDownList to be filled</param>
        /// <param name="AddAllStates">Set true to add 'All States'</param>
        /// <param name="SelectFirst">Set true to select first item</param>
        /// <param name="WhereConditions">Eg: ' and StateID in (1, 5, 32)'</param>
        public static void FillStates(DropDownList ddl, bool AddAllStates, bool SelectFirst, string WhereConditions)
        {
            FillStates(ddl, AddAllStates, SelectFirst, WhereConditions, false);
        }
        #endregion

        #region FillStates (By Condition By AllowEditing)
        /// <summary>
        /// To fill states
        /// </summary>
        /// <param name="ddl">DropDownList to be filled</param>
        /// <param name="AddAllStates">Set true to add 'All States'</param>
        /// <param name="SelectFirst">Set true to select first item</param>
        /// <param name="WhereConditions">Eg: ' and StateID in (1, 5, 32)'</param>
        public static void FillStates(DropDownList ddl, bool AddAllStates, bool SelectFirst, string WhereConditions, bool AllowEditing)
        {
            ddl.Items.Clear();
            string strSQL = "Select StateID, StateName from masStates Where 1=1 " + WhereConditions + " Order by StateName";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            ddl.DataTextField = "StateName";
            ddl.DataValueField = "StateID";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (AddAllStates)
                ddl.Items.Insert(0, new ListItem("All States", "0"));

            if (ddl.Page.Session["usrStateID"] != null && !AllowEditing)
            {
                ddl.SelectedValue = ddl.Page.Session["usrStateID"].ToString();
                ddl.Enabled = false;
            }
            else if (SelectFirst && ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }
        #endregion

        #region GetSupplierName
        public static string GetSupplierName(string SupplierID)
        {
            if (SupplierID == "") SupplierID = "0";
            string strSQL = "Select SupplierName from masSuppliers where SupplierID = " + SupplierID;
            DataTable dt = DBHelper.GetDataTable(strSQL);
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0][0].ToString();
            }
            else
                return "";
        }
        #endregion

        #region GetLPSupplierName
        public static string GetLPSupplierName(string LPSupplierID)
        {
            if (LPSupplierID == "") LPSupplierID = "0";
            string strSQL = "Select SupplierName from LPmasSuppliers where LPSupplierID = " + LPSupplierID;
            DataTable dt = DBHelper.GetDataTable(strSQL);
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0][0].ToString();
            }
            else
                return "";
        }
        #endregion

        #region GetPSAName
        public static string GetPSAName(string PSAID)
        {
            if (PSAID == "") PSAID = "0";
            string strSQL = "Select PSAName from masPSAs where PSAID = " + PSAID;
            DataTable dt = DBHelper.GetDataTable(strSQL);
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0][0].ToString();
            }
            else
                return "";
        }
        #endregion
        
        #region GetPSANameByWarehouseID
        public static string GetPSANameByWarehouseID(string WarehouseID)
        {
            if (WarehouseID == "") WarehouseID = "0";
            string strSQL = "Select PsaName From MasPSAs Where WarehouseID = " + WarehouseID;
            DataTable dt = DBHelper.GetDataTable(strSQL);
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0][0].ToString();
            }
            else
                return "";
        }
        #endregion

        #region FillSMOs
        public static void FillSMOs(DropDownList ddl, bool AddAllFacilities, bool SelectFirst)
        {
            ddl.Items.Clear();
            string strSQL = "Select a.FacilityID, a.FacilityName || ', ' || b.StateName as FacilityName from masFacilities a Inner Join masStates b on (b.StateID = a.StateID) where NVL(a.IsSMO, 0) = 1 Order By a.FacilityName";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            ddl.DataTextField = "FacilityName";
            ddl.DataValueField = "FacilityID";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (AddAllFacilities)
                ddl.Items.Insert(0, new ListItem("All SMOs", "0"));
            if (SelectFirst && ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }
        #endregion

        #region FillSMODeliveryLocations
        public static void FillSMODeliveryLocations(DropDownList ddl, string mFacilityID, bool AddAllLocations, bool SelectFirst)
        {
            if (mFacilityID.Trim() == "") mFacilityID = "0";
            ddl.Items.Clear();
            string strSQL = "Select a.SMODeliveryLocID, a.LocationName || ', ' || b.StateName as LocationName from masSMODeliveryLocations a Inner Join masStates b on (b.StateID = a.StateID) Where a.FacilityID = " + mFacilityID + " Order By b.StateName, a.LocationName";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            ddl.DataTextField = "LocationName";
            ddl.DataValueField = "SMODeliveryLocID";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (AddAllLocations)
                ddl.Items.Insert(0, new ListItem("All Locations", "0"));
            if (SelectFirst && ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }
        #endregion

        #region FillWarehouseTypes
        public static void FillWarehouseTypes(DropDownList ddl, bool AddAllWarehouseTypes, bool SelectFirst)
        {
            ddl.Items.Clear();
            string strSQL = "Select WarehouseTypeID, WarehouseTypeName from masWarehouseTypes Order By WarehouseTypeName";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            ddl.DataTextField = "WarehouseTypeName";
            ddl.DataValueField = "WarehouseTypeID";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (AddAllWarehouseTypes)
                ddl.Items.Insert(0, new ListItem("All Warehouse Types", "0"));
            if (SelectFirst && ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }
        #endregion

        #region FillWarehouses (By State)
        public static void FillWarehouses(DropDownList ddl, string StateID, bool AddAllWarehouses, bool SelectFirst)
        {
            FillWarehouses(ddl, StateID, AddAllWarehouses, SelectFirst, "", false);
        }
        #endregion

        #region FillWarehouses (By State By Condition)
        public static void FillWarehouses(DropDownList ddl, string StateID, bool AddAllWarehouses, bool SelectFirst, string WhereConditions)
        {
            FillWarehouses(ddl, StateID, AddAllWarehouses, SelectFirst, WhereConditions, false);
        }
        #endregion

        #region FillWarehouses (By State By Condition By AllowEditing)
        /// <summary>
        /// Fill warehouses based on the given stateID
        /// </summary>
        /// <param name="ddl">DropDownList to be filled</param>
        /// <param name="StateID">StateID to filter</param>
        /// <param name="AddAllWarehouses">Set true to add 'All Warehouses' as first string</param>
        /// <param name="SelectFirst">Set true to mark the first item as selected</param>
        /// <param name="WhereConditions">Eg: ' and WarehouseID in (1, 3, 56, 345)'</param>
        public static void FillWarehouses(DropDownList ddl, string StateID, bool AddAllWarehouses, bool SelectFirst, string WhereConditions, bool AllowEditing)
        {
            ddl.Items.Clear();
            if (StateID == "") StateID = "0";
            string strSQL = "Select WarehouseID, WarehouseName from masWarehouses where StateID = " + StateID + WhereConditions + " Order By WarehouseName";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            ddl.DataTextField = "WarehouseName";
            ddl.DataValueField = "WarehouseID";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (AddAllWarehouses)
                ddl.Items.Insert(0, new ListItem("All Warehouses", "0"));
            if (ddl.Page.Session["usrWarehouseID"] != null && !AllowEditing)
            {
                ddl.SelectedValue = ddl.Page.Session["usrWarehouseID"].ToString();
                ddl.Enabled = false;
            }
            else if (SelectFirst && ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }
        #endregion

        #region FillWarehouses (By District)
        public static void FillWarehousesByDistrict(DropDownList ddl, string DistrictID, bool AddAllWarehouses, bool SelectFirst, string WhereConditions)
        {
            ddl.Items.Clear();
            if (DistrictID == "") DistrictID = "0";
            string strSQL = "Select WarehouseID, WarehouseName from masWarehouses" + (DistrictID != "0" ? " where DistrictID = " + DistrictID + WhereConditions : "") + " Order By WarehouseName";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            ddl.DataTextField = "WarehouseName";
            ddl.DataValueField = "WarehouseID";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (AddAllWarehouses)
                ddl.Items.Insert(0, new ListItem("All Warehouses", "0"));
            if (ddl.Page.Session["usrWarehouseID"] != null)
            {
                ddl.SelectedValue = ddl.Page.Session["usrWarehouseID"].ToString();
                ddl.Enabled = false;
            }
            else if (SelectFirst && ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }
        #endregion

        #region FillWarehouses (By Division)
        public static void FillWarehousesByDivision(DropDownList ddl, string DivisionID, bool AddAllWarehouses, bool SelectFirst, string WhereConditions)
        {
            ddl.Items.Clear();
            if (DivisionID == "") DivisionID = "0";
            string strSQL = "Select Ware.WarehouseID, WarehouseName from masWarehouses Ware " +
            "Inner Join MasDistricts Dist on (Dist.DistrictID = Ware.DistrictID) " +
            "Inner Join Masdivisions Div on (Div.Divisionid = Dist.DivisionID) " +
            (DivisionID != "0" ? " where div.DivisionID = " + DivisionID + WhereConditions : "") + " " +
            "Order By WarehouseName";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            ddl.DataTextField = "WarehouseName";
            ddl.DataValueField = "WarehouseID";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (AddAllWarehouses)
                ddl.Items.Insert(0, new ListItem("All Warehouses", "0"));
            if (SelectFirst && ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }
        #endregion

        #region FillFacilityTypes (By Warehouse By Condition)
        public static void FillFacilityTypes(DropDownList ddl, string WarehouseID, bool AddAllFacilityTypes, bool SelectFirst, string WhereConditions)
        {
            ddl.Items.Clear();
            if (WarehouseID == "") WarehouseID = "0";
            string strSQL = "Select distinct c.FacilityTypeID, c.FacilityTypeDesc from masFacilities a Inner Join masFacilityWH b on (b.FacilityID = a.FacilityID) Inner Join masFacilityTypes c on (c.FacilityTypeID = a.FacilityTypeID) Where b.WarehouseID = " + WarehouseID + WhereConditions + " Order By Upper(c.FacilityTypeDesc)";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            ddl.DataTextField = "FacilityTypeDesc";
            ddl.DataValueField = "FacilityTypeID";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (AddAllFacilityTypes)
                ddl.Items.Insert(0, new ListItem("All Facility Types", "0"));
            if (SelectFirst && ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }
        #endregion

        #region FillFacilityTypes
        public static void FillFacilityTypes(DropDownList ddl, bool AddAllFacilityTypes, bool SelectFirst, string WhereConditions)
        {
            ddl.Items.Clear();
            string strSQL = "Select c.FacilityTypeID, c.FacilityTypeDesc" +
                " from masFacilityTypes c" +
                " Where 1=1 " + WhereConditions +
                " Order By Upper(c.FacilityTypeDesc)";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            ddl.DataTextField = "FacilityTypeDesc";
            ddl.DataValueField = "FacilityTypeID";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (AddAllFacilityTypes)
                ddl.Items.Insert(0, new ListItem("All Facility Types", "0"));
            if (SelectFirst && ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }
        #endregion

        #region FillFacilities (By Warehouse By Condition)
        public static void FillFacilities(DropDownList ddl, string WarehouseID, bool AddAllFacilities, bool SelectFirst, string WhereConditions)
        {
            ddl.Items.Clear();
            if (WarehouseID == "") WarehouseID = "0";
            string strSQL = "Select a.FacilityID, a.FacilityName from masFacilities a Inner Join masFacilityWH b on (b.FacilityID = a.FacilityID) Where b.WarehouseID = " + WarehouseID + WhereConditions + " Order By a.FacilityName";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            ddl.DataTextField = "Facilityname";
            ddl.DataValueField = "FacilityID";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (AddAllFacilities)
                ddl.Items.Insert(0, new ListItem("All Facilities", "0"));
            if (SelectFirst && ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }
        #endregion

        #region FillFacilities (By Condition)
        public static void FillFacilities(DropDownList ddl, bool AddAllFacilities, bool SelectFirst, string WhereConditions)
        {
            ddl.Items.Clear();
            string strSQL = "Select FacilityID, FacilityName from masFacilities Where 1=1 " + WhereConditions + " Order By FacilityName";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            ddl.DataTextField = "Facilityname";
            ddl.DataValueField = "FacilityID";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (AddAllFacilities)
                ddl.Items.Insert(0, new ListItem("All Facilities", "0"));
            if (SelectFirst && ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }
        #endregion

        #region FillSuppliers
        public static void FillSuppliers(DropDownList ddl, bool AddAllSuppliers, bool SelectFirst, string Conditions)
        {
            ddl.Items.Clear();
            string strSQL = "Select SupplierID, SupplierName from masSuppliers Where isactive=1 " + Conditions + " Order By SupplierName";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            ddl.DataTextField = "SupplierName";
            ddl.DataValueField = "SupplierID";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (AddAllSuppliers)
                ddl.Items.Insert(0, new ListItem("All Suppliers", "0"));
            if (SelectFirst && ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }
        #endregion

        #region FillContractedSuppliers
        public static void FillContractedSuppliersByPsa(DropDownList ddl, bool AddAllSuppliers, string PsaID, string SourceID, string SchemeID, string CategoryID, string PoDate, bool SelectFirst)
        {

            ddl.Items.Clear();

            string Error = GenFunctions.CheckDate(ref PoDate, "PoDate", true);
            if (PsaID == "" || PsaID == null) PsaID = "0";
            string Query = " With ContractedSuppliers " +
                            " As " +
                            " ( " +
                            " Select Distinct ContractID,SupplierID from v_ActiveContractActiveItems Where SourceID = @SourceID and SchemeID = @SchemeID and CategoryID = @CategoryID and Nvl(@PODate,sysdate) Between ContractStartDate and ContractEndDate " + 
                            " ) " +
                            " Select distinct " +
                            " S.SupplierID, " +
                            " S.SupplierName " +
                            " from MasSuppliers S " +
                            " Inner Join ContractedSuppliers CS on (CS.SupplierID = S.SupplierID) " +
                            " Inner Join AocContracts C on (C.ContractID = CS.ContractID and (C.PsaID is Null or C.PsaID = @PsaID)) Order By S.SupplierName";

            Query = Query.Replace("@PsaID", PsaID);
            Query = Query.Replace("@SourceID", SourceID);
            Query = Query.Replace("@SchemeID", SchemeID);
            Query = Query.Replace("@PODate", PoDate);
            Query = Query.Replace("@CategoryID", CategoryID);

            DataTable dt = DBHelper.GetDataTable(Query);
            ddl.DataTextField = "SupplierName";
            ddl.DataValueField = "SupplierID";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (AddAllSuppliers)
                ddl.Items.Insert(0, new ListItem("All Suppliers", "0"));
            if (SelectFirst && ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }
        #endregion

        #region FillContractedLPSuppliers
        public static void FillLPContractedSuppliersByPsa(DropDownList ddl, bool AddAllSuppliers, string PsaID, string CategoryID, string PoDate, bool SelectFirst, SupplierType Suppliertype)
        {
            FillLPContractedSuppliersByPsa(ddl, AddAllSuppliers, PsaID, CategoryID, PoDate, SelectFirst, Suppliertype, false);
        }
        
        public static void FillLPContractedSuppliersByPsa(DropDownList ddl, bool AddAllSuppliers, string PsaID, string CategoryID, string PoDate, bool SelectFirst, SupplierType Suppliertype,bool ActiveSupplierOnly)
        {

            ddl.Items.Clear(); 

            string Error = GenFunctions.CheckDate(ref PoDate, "PoDate", true);
            string Query = string.Empty;
            if (ActiveSupplierOnly)
            {
                    Query = " With LPContractedSuppliers " +
                            " As " +
                            " ( " +
                            " Select Distinct ContractID,SupplierID,LPSupplierID from v_LPActiveContractActiveItems Where CategoryID = Nvl(@CategoryID,CategoryID) and Nvl(@PODate,sysdate) Between ContractStartDate and ContractEndDate " +
                            " ) " +
                            " Select distinct " +
                            " Coalesce (S.SupplierID,LS.LPSupplierID) as SupplierID, " +
                            " Coalesce (S.SupplierName,LS.SupplierName) as SupplierName " +
                            " from LPContractedSuppliers CS " +
                            " Left Outer Join MasSuppliers S On (S.SupplierID = CS.SupplierID)" + 
                            " Left Outer Join LPMasSuppliers LS On (LS.LPSupplierID = CS.LPSupplierID)" +
                            " Inner Join LPContracts C on (C.ContractID = CS.ContractID and (C.PsaID is Null or C.PsaID = Nvl(@PsaID,C.PsaID))) " +
                            " Where " + ((Suppliertype != SupplierType.LocalSuppliersOnly) ? " S.SupplierID Is Not Null " : " LS.LPSupplierID Is Not Null ") + " Order By SupplierName ";
            }
            else
            {
                Query = " Select distinct " +
                    " Coalesce (S.SupplierID,LS.LPSupplierID) as SupplierID, " +
                    " Coalesce (S.SupplierName,LS.SupplierName) as SupplierName " +
                    " from V_CombinedSuppliers CS " +
                    " Left Outer Join MasSuppliers S On (S.SupplierID = CS.SupplierID)" +
                    " Left Outer Join LPMasSuppliers LS On (LS.LPSupplierID = CS.LPSupplierID)" +
                    " Where " + ((Suppliertype != SupplierType.LocalSuppliersOnly) ? " S.SupplierID Is Not Null " : " LS.LPSupplierID Is Not Null ") + " Order By SupplierName ";
            }

            Query = Query.Replace("@PsaID", ((string.IsNullOrEmpty(CategoryID)) ? "Null" : PsaID));
            Query = Query.Replace("@PODate", ((string.IsNullOrEmpty(CategoryID)) ? "Null" : PoDate));
            Query = Query.Replace("@CategoryID", ((string.IsNullOrEmpty(CategoryID))? "Null" : CategoryID));

            DataTable dt = DBHelper.GetDataTable(Query);
            ddl.DataTextField = "SupplierName";
            ddl.DataValueField = "SupplierID";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (AddAllSuppliers)
                ddl.Items.Insert(0, new ListItem("All Suppliers", "0"));
            if (SelectFirst && ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }
        #endregion

        #region GetDistrictName
        public static string GetDistrictName(string districtID)
        {
            if (districtID == "") districtID = "0";
            string strSQL = "select districtname from masdistricts where districtid = " + districtID;
            DataTable dt = DBHelper.GetDataTable(strSQL);
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0][0].ToString();
            }
            else
                return "";
        }
        #endregion
        #region FillLPSuppliers
        public static void FillLPSuppliers(DropDownList ddl, bool AddAllSuppliers, bool SelectFirst, string Conditions)
        {
            ddl.Items.Clear();

            string strSQL = "Select LPSupplierID, SupplierName from LPmasSuppliers Where 1=1 " + Conditions + " Order By SupplierName";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            ddl.DataTextField = "SupplierName";
            ddl.DataValueField = "LPSupplierID";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (AddAllSuppliers)
                ddl.Items.Insert(0, new ListItem("All Suppliers", "0"));
            if (SelectFirst && ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }

        public static void FillLPSuppliers2(DropDownList ddl, bool AddAllSuppliers, bool SelectFirst, string Conditions)
        {
            ddl.Items.Clear();
               string strSQL = @"Select distinct lp.LPSupplierID, SupplierName from LPmasSuppliers  lp 
                inner join lpcontracts c on c.lpsupplierid=lp.lpsupplierid  Where 1=1 " + Conditions + " Order By SupplierName ";

           // string strSQL = @"Select ls.LPSupplierID,'Con No-'|| lc.contractno||' '|| ls.SupplierName||'-'||to_char(lc.contractdate,'dd-MM-yyyy') as //SupplierName,accyrsetid from LPmasSuppliers ls inner join lpcontracts lc  on
         //                   ls.lpsupplierid  =lc.lpsupplierid  and ls.psaid=lc.psaid
                   //         Where 1=1 " + Conditions + " Order By lc.contractdate desc";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            ddl.DataTextField = "SupplierName";
            ddl.DataValueField = "LPSupplierID";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (AddAllSuppliers)
                ddl.Items.Insert(0, new ListItem("All Suppliers", "0"));
            if (SelectFirst && ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }

        #endregion

        //#region FillLPSuppliers
        //public static void FillLPSuppliers(DropDownList ddl, bool AddAllSuppliers, bool SelectFirst, string Conditions)
        //{
        //    ddl.Items.Clear();
        //    string strSQL = "Select LPSupplierID, SupplierName from LPmasSuppliers Where 1=1 " + Conditions + " Order By SupplierName";
        //    DataTable dt = DBHelper.GetDataTable(strSQL);
        //    ddl.DataTextField = "SupplierName";
        //    ddl.DataValueField = "LPSupplierID";
        //    ddl.DataSource = dt;
        //    ddl.DataBind();
        //    if (AddAllSuppliers)
        //        ddl.Items.Insert(0, new ListItem("All Suppliers", "0"));
        //    if (SelectFirst && ddl.Items.Count > 0)
        //        ddl.SelectedIndex = 0;
        //}
        //#endregion

        #region FillSuppliersByCountry
        public static void FillSuppliersByCountry(DropDownList ddl, string CountryID, bool AddAllSuppliers, bool SelectFirst)
        {
            string Conditions = "and CountryID = " + GenFunctions.CheckEmptyString(CountryID, "0") + " and (blackliststatus is null or (case when blacklisttdate is not null then blacklisttdate end) < sysdate) ";
            FillSuppliers(ddl, AddAllSuppliers, SelectFirst, Conditions);
        }
        #endregion

        #region FillItemCategories
        public static void FillItemCategories(DropDownList ddl, bool AddAllCategories, bool SelectMED)
        {
            ddl.Items.Clear();
            string strSQL = "Select CategoryID, CategoryName from masItemCategories Order By CategoryCode";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            ddl.DataTextField = "CategoryName";
            ddl.DataValueField = "CategoryID";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (AddAllCategories)
                ddl.Items.Insert(0, new ListItem("All Categories", "0"));
            if (ddl.Items.Count > 0 && SelectMED)
            {
                ListItem li = ddl.Items.FindByText("MEDICINE");
                if (li != null)
                    li.Selected = true;
                else
                    ddl.SelectedIndex = 0;
            }
        }
        #endregion

        #region FillItemTypes
        public static void FillItemTypes(DropDownList ddl, bool AddAllItemTypes, bool SelectFirst)
        {
            ddl.Items.Clear();
            string strSQL = "Select ItemTypeID, ItemTypeCode || ' - ' || ItemTypeName as ItemTypeName from masItemTypes Order By ItemTypeName";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            ddl.DataTextField = "ItemTypeName";
            ddl.DataValueField = "ItemTypeID";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (AddAllItemTypes)
                ddl.Items.Insert(0, new ListItem("All Item Types", "0"));
            if (SelectFirst && ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }
        #endregion

        #region FillItemTypesByItemCategory
        public static void FillItemTypesByItemCategory(DropDownList ddl, string ItemCategoryID, bool IncludeInActiveItems, bool AddAllItemTypes, bool SelectFirst)
        {
            if (ItemCategoryID == "0") ItemCategoryID = "";
            ddl.Items.Clear();
            string strSQL = "Select Distinct a.ItemTypeID, a.ItemTypeCode, a.ItemTypeName from masItemTypes a, masItems b where a.ItemTypeID = b.ItemTypeID " + ((ItemCategoryID == "") ? "" : " and b.CategoryID = " + ItemCategoryID) + ((IncludeInActiveItems) ? "" : " and NVL(b.ActiveFlag, 'N') = 'Y'") + " Order by a.ItemTypeName";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            ddl.DataTextField = "ItemTypeName";
            ddl.DataValueField = "ItemTypeID";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (AddAllItemTypes)
                ddl.Items.Insert(0, new ListItem("All Item Types", "0"));
            if (SelectFirst && ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }
        #endregion

        #region FillItemVEN
        public static void FillItemVEN(DropDownList ddl, bool AddAllItemVENs, bool SelectFirst)
        {
            ddl.Items.Clear();
            string strSQL = "Select VENID, VENCode || ' - ' || VENName as VENName from masItemVENs Order By VENName";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            ddl.DataTextField = "VENName";
            ddl.DataValueField = "VENID";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (AddAllItemVENs)
                ddl.Items.Insert(0, new ListItem("All VENs", "0"));
            if (SelectFirst && ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }
        #endregion

        #region FillItemLevels
        public static void FillItemLevels(DropDownList ddl, bool AddAllItemLevels, bool SelectFirst)
        {
            ddl.Items.Clear();
            string strSQL = "Select StockStatusID, StkStCode || ' - ' || StockStatusName as StockStatusName from masItemLevels Order By StockStatusName";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            ddl.DataTextField = "StockStatusName";
            ddl.DataValueField = "StockStatusID";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (AddAllItemLevels)
                ddl.Items.Insert(0, new ListItem("All Levels", "0"));
            if (SelectFirst && ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }
        #endregion

        #region FillItemExpInds
        public static void FillItemExpInds(DropDownList ddl, bool AddAllItemExpInds, bool SelectFirst)
        {
            ddl.Items.Clear();
            string strSQL = "Select ExpIndicatID, ExpIndicatCode || ' - ' || ExpIndicatName as ExpIndicatName from masItemExpIndicators Order By ExpIndicatName";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            ddl.DataTextField = "ExpIndicatName";
            ddl.DataValueField = "ExpIndicatID";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (AddAllItemExpInds)
                ddl.Items.Insert(0, new ListItem("All Expiry Indicators", "0"));
            if (SelectFirst && ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }
        #endregion

        #region FillItemGroups
        public static void FillItemGroups(DropDownList ddl, bool AddAllGroups, bool SelectFirst)
        {
            FillItemGroups(ddl, AddAllGroups, SelectFirst, "");
        }
        public static void FillItemGroups(DropDownList ddl, bool AddAllGroups, bool SelectFirst, string WhereConditions)
        {
            ddl.Items.Clear();
            string strSQL = "Select GroupID, GroupCode || ' ' || Replace(lpad('_', 3*(level-1), '_'), '_', '.') || SubStr(GroupName, 0, 20-3*(level-1)) as GName from masItemGroups Connect By PRIOR GroupID = RefItemGroupID Start with RefItemGroupID IS NULL";
            //"Select StateID, StateName from masStates Where 1=1 " + WhereConditions + " Order by StateName";
            //&nbsp;
            DataTable dt = DBHelper.GetDataTable(strSQL);
            ddl.DataTextField = "GName";
            ddl.DataValueField = "GroupID";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (AddAllGroups)
                ddl.Items.Insert(0, new ListItem("All Item Groups", "0"));
            if (SelectFirst && ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }
        #endregion

        #region FillCurrencies
        public static void FillCurrencies(DropDownList ddl, bool AddAllCurrencies, bool SelectINR)
        {
            ddl.Items.Clear();
            string strSQL = "Select CurrencyID, CurrencyName || ' (' || CurrencyCode || ')' as CurrencyName from masCurrencies Order By CurrencyName";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            ddl.DataTextField = "CurrencyName";
            ddl.DataValueField = "CurrencyID";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (AddAllCurrencies)
                ddl.Items.Insert(0, new ListItem("All Currencies", "0"));
            if (ddl.Items.Count > 0 && SelectINR)
            {
                ListItem li = ddl.Items.FindByText("Indian Rupee (INR)");
                if (li != null)
                    li.Selected = true;
                else
                    ddl.SelectedIndex = 0;
            }
        }
        #endregion

        #region FillItems
        public static void FillItems(DropDownList ddl, bool AddAllItems, bool SelectFirst)
        {
            FillItems(ddl, AddAllItems, SelectFirst, "");
        }

        public static void FillItems(DropDownList ddl, bool AddAllItems, bool SelectFirst, string Conditions)
        {
            ddl.Items.Clear();
            string strSQL = "Select ItemID, ItemCode || ' - (' || ItemName || ')' as ItemName from masItems Where 1=1 " + Conditions + " Order By ItemName";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            ddl.DataTextField = "ItemName";
            ddl.DataValueField = "ItemID";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (AddAllItems)
                ddl.Items.Insert(0, new ListItem("All Items", "0"));
            if (SelectFirst && ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }
        #endregion

        #region FillLPItemsByPsa
        public static void FillLPItemsByPsa(DropDownList ddl, string PsaID, bool AddAllItems, bool SelectFirst)
        {
            FillItems(ddl, AddAllItems, SelectFirst, "");
        }

        public static void FillLPItemsByPsa(DropDownList ddl, string PsaID, bool AddAllItems, bool SelectFirst, string Conditions)
        {
            ddl.Items.Clear();
            string WhereClause = string.Empty;
            if (PsaID != "" && PsaID != "0")
                WhereClause += " and PsaID = " + PsaID + " ";

            string strSQL = "Select LPItemID, ItemName || ' - (' || ItemCode || ')' as ItemName from LPmasItems Where 1=1 " + WhereClause + Conditions + " Order By ItemName";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            ddl.DataTextField = "ItemName";
            ddl.DataValueField = "LPItemID";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (AddAllItems)
                ddl.Items.Insert(0, new ListItem("All Items", "0"));
            if (SelectFirst && ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }

        public static void FillLPItemsByPsa(DropDownList ddl, bool AddAllItems, bool SelectFirst, string strSQL)
        {
            DataTable dt = DBHelper.GetDataTable(strSQL);
            ddl.DataTextField = "ItemName";
            ddl.DataValueField = "LPItemID";
            //ddl.DataSource = dt;
            //ddl.DataBind();
            if (AddAllItems)
                if (ddl.Items.Count == 0) ddl.Items.Add (new ListItem("All Items", "0"));
            foreach (DataRow dr in dt.Rows)
            {
                ddl.Items.Add(new ListItem(dr["ItemName"].ToString(), dr["LPItemID"].ToString()));
            }
            if (SelectFirst && ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }
        #endregion

        #region FillItemsBySourceByScheme

        public static void FillItemsBySourceByScheme(DropDownList ddl, string SourceID, string SchemeID, bool AddAllItems, bool AnyItems, bool SelectFirst)
        {
            FillItemsBySourceByScheme(ddl, SourceID, SchemeID, "0", AddAllItems, AnyItems, SelectFirst);
        }
        public static void FillItemsBySourceBySchemeByCategory(DropDownList ddl, string SourceID, string SchemeID, string CategoryID, bool AddAllItems, bool AnyItems, bool SelectFirst)
        {
            FillItemsBySourceByScheme(ddl, SourceID, SchemeID, CategoryID, AddAllItems, AnyItems, SelectFirst);
        }

        public static void FillItemsBySourceByScheme(DropDownList ddl, string SourceID, string SchemeID,string CategoryID, bool AddAllItems, bool AnyItems, bool SelectFirst)
        {
            string WhereClause = "";
            ddl.Items.Clear();
            if (SourceID != "" && SourceID != "0")
                WhereClause += " and b.SourceID = " + SourceID;
            if (SchemeID != "" && SchemeID != "0")
                WhereClause += " and b.SchemeID = " + SchemeID;
            if (CategoryID != "" && CategoryID != "0")
                WhereClause += " and a.CategoryID = " + CategoryID;
            string strSQL = "Select a.ItemID, (a.ItemName || '(' || a.ItemCode || ')' ||'') as ItemName from masItems a Inner Join masItemsBySourceByScheme b on (a.ItemID = b.ItemID) Where 1=1 " + WhereClause + " Order By a.ItemName";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            ddl.DataTextField = "ItemName";
            ddl.DataValueField = "ItemID";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (AddAllItems == true)
                ddl.Items.Insert(0, new ListItem("All Items", "0"));
            else if (AnyItems == true)
                ddl.Items.Insert(0, new ListItem("Any Item", "0"));
            if (SelectFirst && ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }
        #endregion

        #region FillDistricts
        public static void FillDistricts(DropDownList ddl, bool AddAllDistricts, bool SelectFirst)
        {
            ddl.Items.Clear();
            string strSQL = "Select DistrictID, DistrictName from masDistricts Order By DistrictName";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            ddl.DataTextField = "DistrictName";
            ddl.DataValueField = "DistrictID";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (AddAllDistricts)
                ddl.Items.Insert(0, new ListItem("All Districts", "0"));

            if (ddl.Page.Session["usrDistrictID"] != null)
            {
                ddl.SelectedValue = ddl.Page.Session["usrDistrictID"].ToString();
                ddl.Enabled = false;
            }
            else if (SelectFirst && ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }
        #endregion

        #region Fill Districts By Division
        public static void FillDistrictsByDivision(DropDownList ddl, string DivisionID, bool AddAllDistricts, bool SelectFirst)
        {
            ddl.Items.Clear();
            string WhereCondition = "";
            if (DivisionID != "" && DivisionID != "0")
                WhereCondition += " Where DivisionID= " + DivisionID;
            string strSQL = "Select DistrictID, DistrictName from masDistricts" + WhereCondition + " Order By DistrictName";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            ddl.DataTextField = "DistrictName";
            ddl.DataValueField = "DistrictID";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (AddAllDistricts)
                ddl.Items.Insert(0, new ListItem("All Districts", "0"));

            if (ddl.Page.Session["usrDistrictID"] != null)
            {
                ddl.SelectedValue = ddl.Page.Session["usrDistrictID"].ToString();
                ddl.Enabled = false;
            }
            else if (SelectFirst && ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }
        #endregion

        #region Fill Districts By State
        public static void FillDistrictsByState(DropDownList ddl, string StateID, bool AddAllDistricts, bool SelectFirst)
        {
            ddl.Items.Clear();
            string strSQL = "Select DistrictID, DistrictName from masDistricts Where StateID = " + StateID + " Order By DistrictName";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            ddl.DataTextField = "DistrictName";
            ddl.DataValueField = "DistrictID";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (AddAllDistricts)
                ddl.Items.Insert(0, new ListItem("All Districts", "0"));

            if (ddl.Page.Session["usrDistrictID"] != null)
            {
                ddl.SelectedValue = ddl.Page.Session["usrDistrictID"].ToString();
                ddl.Enabled = false;
            }
            else if (SelectFirst && ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }
        #endregion

        #region FillWards (By Facility By Condition)
        public static void FillWards(DropDownList ddl, string FacilityID, bool AddAllWards, bool SelectFirst, string WhereConditions)
        {
            ddl.Items.Clear();
            if (FacilityID == "") FacilityID = "0";

            string strSQL = "Select a.WardID, a.WardName" +
                " from masFacilityWards a" +
                " Where a.IsActive=1 and a.FacilityID=" + FacilityID + " " + WhereConditions +
                " Order By a.WardName";
            DataTable dt = DBHelper.GetDataTable(strSQL);

            ddl.DataTextField = "WardName";
            ddl.DataValueField = "WardID";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (AddAllWards)
                ddl.Items.Insert(0, new ListItem("All Wards", "0"));
            if (SelectFirst && ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }
        #endregion

        #region FillDivisions
        public static void FillDivisions(DropDownList ddl, bool AddAllDivisions, bool SelectFirst)
        {
            ddl.Items.Clear();
            string strSQL = "Select DivisionID, DivisionName from masDivisions Order By DivisionName";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            ddl.DataTextField = "DivisionName";
            ddl.DataValueField = "DivisionID";
            ddl.DataSource = dt;
            ddl.DataBind();

            if (AddAllDivisions)
                ddl.Items.Insert(0, new ListItem("All Divisions", "0"));

            if (ddl.Page.Session["usrDivisionID"] != null)
            {
                ddl.SelectedValue = ddl.Page.Session["usrDivisionID"].ToString();
                ddl.Enabled = false;
            }
            else if (SelectFirst && ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }
        #endregion

        #region FillItemGroup
        public static void FillItemGroup(DropDownList ddl, bool MainGroup)
        {
            ddl.Items.Clear();
            string strSQL = "Select GroupID, GroupName from masItemGroups Order By GroupName";//where RefItemGroupId is Null
            DataTable dt = DBHelper.GetDataTable(strSQL);
            ddl.DataTextField = "GroupName";
            ddl.DataValueField = "GroupID";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (MainGroup)
                ddl.Items.Insert(0, new ListItem("MAINGROUP", "0"));

        }
        #endregion

        #region FillGroups
        public static void FillGroups(DropDownList ddl, bool AddAllGroups, bool SelectFirst)
        {
            ddl.Items.Clear();
            string strSQL = "Select GroupID, GroupName from masItemGroups Order By GroupName";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            ddl.DataTextField = "GroupName";
            ddl.DataValueField = "GroupID";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (AddAllGroups)
                ddl.Items.Insert(0, new ListItem("All Groups", "0"));

            if (SelectFirst && ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }
        #endregion

        #region GetItemGroupName
        public static string GetItemGroupName(string GroupID)
        {
            if (GroupID == "") GroupID = "0";
            string strSQL = "Select GroupName from masItemGroups where GroupID = " + GroupID;
            DataTable dt = DBHelper.GetDataTable(strSQL);
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0][0].ToString();
            }
            else
                return "";
        }
        #endregion

        #region FillItemsByAllSourceAndScheme

        public static void FillItemsByAllSourceAndScheme(DropDownList ddl, bool AddAllItems, bool AnyItems, bool SelectFirst)
        {
            FillItemsByAllSourceAndScheme(ddl, "0", AddAllItems, AnyItems, SelectFirst);
        }
        public static void FillItemsByAllSourceAndScheme(DropDownList ddl, string CategoryID, bool AddAllItems, bool AnyItems, bool SelectFirst)
        {
            string WhereClause = "";
            ddl.Items.Clear();
            if (CategoryID != "" && CategoryID != "0")
                WhereClause += " and a.CategoryID = " + CategoryID;
            string strSQL = "Select a.ItemID, (a.ItemName || '(' || a.ItemCode || ')' ||'') as ItemName from masItems a Where 1=1 " + WhereClause + " Order By a.ItemName";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            ddl.DataTextField = "ItemName";
            ddl.DataValueField = "ItemID";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (AddAllItems == true)
                ddl.Items.Insert(0, new ListItem("All Items", "0"));
            else if (AnyItems == true)
                ddl.Items.Insert(0, new ListItem("Any Item", "0"));
            if (SelectFirst && ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }
        #endregion

        # region FillItemsSupplier
        public static void FillItemsSupplier(string SupplierId)
        {
            string strSQL = "insert into masItemsBySupplier (SupplierId, ItemId)" +
                " select distinct OP.SupplierId, OI.ItemId from soOrderedItems OI" +
                " inner join soOrderPlaced OP on (OP.PoNoId = OI.PoNoId)" +
                " where OP.SupplierId = " + SupplierId +
                " and OI.ItemId not in (select ItemId from masItemsBySupplier where SupplierId = OP.SupplierId)";
            DBHelper.GetDataTable(strSQL);
        }
        # endregion

        #region FillLabs
        public static void FillLabs(DropDownList ddl, bool AddAllItems, bool SelectFirst)
        {
            FillLabs(ddl, AddAllItems, SelectFirst, "");
        }

        public static void FillLabs(DropDownList ddl, bool AddAllLabs, bool SelectFirst, string Conditions)
        {
            ddl.Items.Clear();
            string strSQL = "Select LabID, LabName || ' - (' || LabCode || ')' as LabName from QCLabs Where 1=1 " + Conditions + " Order By LabName";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            ddl.DataTextField = "LabName";
            ddl.DataValueField = "LabID";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (AddAllLabs)
                ddl.Items.Insert(0, new ListItem("All Labs", "0"));
            if (SelectFirst && ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }
        #endregion\

        #region FillQAResult
        public static void FillQAResult(DropDownList ddl, bool AddAllItems, bool SelectFirst)
        {
            FillQAResult(ddl, AddAllItems, SelectFirst, "");
        }

        public static void FillQAResult(DropDownList ddl, bool AddAllQAResult, bool SelectFirst, string Conditions)
        {
            ddl.Items.Clear();
            string strSQL = "select distinct  NVL(testresult,'Pending') testresultvalue, (CASE testresult WHEN 'SQ' then 'Pass' WHEN  'NSQ' THEN 'Fail'else 'Pending' end) testresult from QCTests Where 1=1" + Conditions;
            DataTable dt = DBHelper.GetDataTable(strSQL);
            ddl.DataTextField = "testresult";
            ddl.DataValueField = "testresultvalue";
            ddl.DataSource = dt;
            ddl.DataBind();

            //if (AddAllQAResult)
            //    ddl.Items.Insert(0, new ListItem("All Results", "0"));
            //if (SelectFirst && ddl.Items.Count > 0)
            //    ddl.SelectedIndex = 0;

        }
        #endregion

        #region FillResultStatus
        public static void FillResultStatus(DropDownList ddl, bool AddAllItems, bool SelectFirst)
        {
            FillResultStatus(ddl, AddAllItems, SelectFirst, "");
        }

        public static void FillResultStatus(DropDownList ddl, bool AddAllQAResult, bool SelectFirst, string Conditions)
        {
            ddl.Items.Clear();
            string strSQL = "select distinct  testresult  from QCTests where testresult is not null and 1=1" + Conditions;
            DataTable dt = DBHelper.GetDataTable(strSQL);
            ddl.DataTextField = "testresult";
            ddl.DataValueField = "testresult";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (AddAllQAResult)
                ddl.Items.Insert(0, new ListItem("All Results", "0"));
            if (SelectFirst && ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;

        }
        #endregion
       
        #region FillResult
        public static void FillResult(DropDownList ddl, bool AddAllItems, bool SelectFirst)
        {
            FillResult(ddl, AddAllItems, SelectFirst, "");
        }

        public static void FillResult(DropDownList ddl, bool AddAllQAResult, bool SelectFirst, string Conditions)
        {
            ddl.Items.Clear();
            string strSQL = "select distinct  NVL(testresult,'Pending') testresultvalue from QCTests Where 1=1" + Conditions;
            DataTable dt = DBHelper.GetDataTable(strSQL);
            ddl.DataTextField = "testresultvalue";
            ddl.DataValueField = "testresultvalue";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (AddAllQAResult)
                ddl.Items.Insert(0, new ListItem("All Results", "0"));
            if (SelectFirst && ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;

        }
        #endregion

        #region FillProgram
        public static void FillProgram(DropDownList ddl, bool AddAllProgram, bool SelectFirst)
        {
            FillProgram(ddl, AddAllProgram, SelectFirst, "");
        }
        #endregion

        #region FillProgram With Conditions
        public static void FillProgram(DropDownList ddl, bool AddAllProgram, bool SelectFirst, string Conditions)
        {
            ddl.Items.Clear();
            string strSQL = "Select ProgramId,Program from masProgram Where 1=1" + Conditions + " Order By ProgramId";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            ddl.DataTextField = "Program";
            ddl.DataValueField = "ProgramId";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (AddAllProgram)
            {
                    ddl.Items.Insert(0, new ListItem("All Program", "0"));                    
                    //ddl.SelectedIndex = 0;
               
            }
            if (SelectFirst && ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }
        #endregion

        #region FillTender
        public static void FillTender(DropDownList ddl, bool AddAllTender, bool SelectFirst)
        {
            FillTender(ddl, AddAllTender, SelectFirst, "");
        }
        #endregion

        #region FillTender With Conditions
        public static void FillTender(DropDownList ddl, bool AddAllTender, bool SelectFirst, string Conditions)
        {
            ddl.Items.Clear();
            string strSQL = "Select LabtenderId,LabTender from maslabtender Where 1=1" + Conditions + " Order By Upper(LabTender)";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            ddl.DataTextField = "LabTender";
            ddl.DataValueField = "LabtenderId";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (AddAllTender)
                ddl.Items.Insert(0, new ListItem("All Lab Tender", "0"));
           
            else if (SelectFirst && ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }
        #endregion
        #region FillItems_LP
        public static void FillItems_LP(DropDownList ddl, bool AddAllItems, bool SelectFirst)
        {
            FillItems_LP(ddl, AddAllItems, SelectFirst, "");
        }

        public static void FillItems_LP(DropDownList ddl, bool AddAllItems, bool SelectFirst, string Conditions)
        {
            ddl.Items.Clear();
            string strSQL = "Select ItemID, ItemName || ' - (' || ItemCode || ')' as ItemName from masItems Where 1=1 " + Conditions + " " +
                " union Select LPItemID ItemID, ItemName || ' - (' || ItemCode || ')' as ItemName from LPmasItems Where 1=1 " + Conditions + " order by Itemname";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            ddl.DataTextField = "ItemName";
            ddl.DataValueField = "ItemID";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (AddAllItems)
                ddl.Items.Insert(0, new ListItem("All Items", "0"));
            if (SelectFirst && ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }
        #endregion


         #region getCurrentFinYear
        public static string getCFinyear()
        {
            string reFinYear = "";
            string strSQL = "select ACCYRSETID from masaccyearsettings where  sysdate between STARTDATE and ENDDATE";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            reFinYear = dt.Rows[0][0].ToString();
            return reFinYear;
        }
         #endregion

        #region fillCategory
        public static DataTable FillCategory()
        {

            string squery = "select categoryid,categoryName from masitemcategories order by categoryid";
            DataTable dt = DBHelper.GetDataTable(squery);
            return dt;
          
        }
        #endregion
         #region AIItemsFacility
        public static DataTable GetAIItemsFacility(string yearid,string catid,string facid)
        {
            string wh = "";
            if (catid == "" || catid == "0")
            {

            }
            else
            {
                wh = " and m.categoryid=" + catid;
            }

            if (facid == "" || facid == "0")
            {

            }
            else
            {
                wh += " and a.facilityid=" + facid;
            }

            string squery = @" select distinct m.itemid,m.itemname||'-'||m.itemcode as itemname,m.itemname from masanualindent a
inner join anualindent ai on ai.indentid=a.indentid
inner join masitems m on m.itemid=ai.itemid
where m.isfreez_itpr is null  " +wh+@"
and a.accyrsetid=" + yearid + @" order by m.itemname";
            DataTable dt = DBHelper.GetDataTable(squery);
            return dt;
     }
        #endregion




        #region fillCategory
        public static DataTable FillCategory(string factype)
        {
            string squery = "";
            if (factype == "371")
            {
                squery = "select categoryid,categoryName from masitemcategories where categoryid in (53,54,55,58,59,60,61) order by categoryid";
            }
            else if (factype == "364" || factype == "378" || factype == "367")
            {
                squery = "select categoryid,categoryName from masitemcategories where categoryid not in (58,59,60,61) order by categoryid";
            }
            else
            {
                squery = "select categoryid,categoryName from masitemcategories order by categoryid";
            }
            DataTable dt = DBHelper.GetDataTable(squery);
            return dt;

        }
        #endregion

        #region fillCategory3
        public static DataTable FillCategory3(string factype)
        {
            string squery = "";
            if (factype == "371")
            {
                squery = "select categoryid,categoryName from masitemcategories where categoryid in (58,59,60) order by categoryid";
            }
            else if (factype == "364" || factype == "378" || factype == "367")
            {
                squery = "select categoryid,categoryName from masitemcategories where categoryid not in (58,59,60,61) order by categoryid";
            }
            else
            {
                squery = "select categoryid,categoryName from masitemcategories order by categoryid";
            }
            DataTable dt = DBHelper.GetDataTable(squery);
            return dt;

        }
        #endregion

        #region fillSubCategoryAYUSH
        public static DataTable FillSubCategoryAYUSH(string cattype)
        {


            bool Data = false;

            string whereclause = "";
            if (cattype == "All")
            {
                whereclause = "";
            }
            else
            {
                whereclause = " and  c.categoryid =" + cattype + "";
            }


            string strquery = @"select sc.subcatid ,sc.catname  subcategoryname
                            from masitemcategories c
                            inner join MasSubItemCategory sc on sc.categoryid=c.categoryid
                            where 1=1 " + whereclause + @"  order  by sc.subcatid ";
            DataTable dt = DBHelper.GetDataTable(strquery);
            return dt;


        }
        #endregion


        #region AYUSHItems
        public static DataTable GetAYUSHItems(string catid, string subcatid)
        {
            string wh = "";
            if (catid == "" || catid == "0")
            {

            }
            else
            {
                wh = " and c.categoryid=" + catid;
            }

            if (subcatid == "" || subcatid == "0")
            {

            }
            else
            {
                wh += " and sc.SUBCATID=" + subcatid;
            }

            string squery = @" select distinct m.itemid,m.itemname||'-'||m.itemcode as itemnamePar,m.itemname from  masitems m 
left outer join  masitemcategories c on c.categoryid=m.categoryid
left outer join  MasSubItemCategory sc on sc.categoryid=c.categoryid
where m.isfreez_itpr is null  " + wh + @"
 order by m.itemname";
            DataTable dt = DBHelper.GetDataTable(squery);
            return dt;
        }
        #endregion


        #region fillfacTypeAYUSH
        public static DataTable fillfacTypeAYUSH(string disid)
        {


            bool Data = false;

            string whereclause = "";
            if (disid == "All")
            {
                whereclause = "";
            }
            else
            {
                whereclause = "  and f.districtid =" + disid + "";
            }


            string Strquery = @" select   FACTYPEID as facilitytypeid ,FACTYPENAME||'('||to_char(count(f.facilityid))||')' as facilitytypecode,ORDERDT from MASFACILITYTYPEAYUSH ft
left outer join  masfacilities f  on f.AYFACTYPEID=ft.FACTYPEID
where 1=1 " + whereclause + @"
group by FACTYPEID,FACTYPENAME,ORDERDT
order by ORDERDT ";
            DataTable dt = DBHelper.GetDataTable(Strquery);
            return dt;


        }
        #endregion


        #region fillfacAYUSH
        public static DataTable fillfacilityAYUSH(string disid, string factype)
        {




            string whereclauseDist = "";
            if (disid == "0")
            {
                whereclauseDist = "";
            }
            else
            {
                whereclauseDist += "  and f.districtid =" + disid + "";
            }
            if (factype == "0")
            {
            }
            else
            {
                whereclauseDist += "  and f.AYFACTYPEID =" + factype + "";
            }


            string Strquery = @" select f.facilityid,f.facilityname||To_char((case when mf.userid is not null then To_char('-'||DRNAME||'('||DRMOBILE||')' ) else '' end )) as FACName,f.facilityname from masfacilities f
inner join usrusers u on u.facilityid=f.facilityid
left outer join masfacheaderfooter mf on mf.USERID=u.userid
where 1=1 and  f.facilitytypeid=371  and f.isactive=1 " + whereclauseDist + " order by facilityname ";
            DataTable dt = DBHelper.GetDataTable(Strquery);
            return dt;


        }
        #endregion



        #region fillfacStorageLocatin
        public static DataTable fillStoreateLocation(string facid)
        {

            string Strquery =   @" Select nvl(RackID,0) RackID,nvl(locationno,'Null') locationno from masracks where warehouseid="+facid+" order by locationno";
            DataTable dt = DBHelper.GetDataTable(Strquery);
            return dt;
        }
        #endregion


        #region AYUSHAllItems
        public static DataTable GetAYUSHAllItems(string catid)
        {
            string wh = "";
            if (catid == "" || catid == "0")
            {
                wh = " and c.categoryid in (58,59,60)";
            }
            else
            {
                wh = " and c.categoryid=" + catid;
            }



            string squery = @" select distinct m.itemid,m.itemname||'-'||m.itemcode as itemnamePar,m.itemname from  vmasitems m 
left outer join  masitemcategories c on c.categoryid=m.categoryid
left outer join  masitems mm on mm.itemid=m.itemid
where  m.itemcode not in ('T1') and mm.isfreez_itpr is null  " + wh + @" and ( case when  SUBSTR(m.itemcode,1,2) ='LP' and m.edlitemcode is not null then 'N' else 'Y' end)='Y'
 order by m.itemname";
            DataTable dt = DBHelper.GetDataTable(squery);
            return dt;
        }
        #endregion
    }

}
