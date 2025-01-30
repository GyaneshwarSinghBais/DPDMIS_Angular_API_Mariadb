using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using DBHelper = Broadline.WMS.Data.DBHelper;

/// <summary>
/// Summary description for GenFunctions
/// </summary>

public partial  class GenFunctions
{
    public class Public
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
            else if (SelectFirst && ddl.Items.Count > 0)
            {
                ddl.SelectedIndex = 0;
            }
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
            {
                ddl.Items.Insert(0, new ListItem("All Schemes", "0"));
            }

            else if (SelectFirst && ddl.Items.Count > 0)
            {
                ddl.SelectedIndex = 0;
            }
        }
        #endregion

        #region FillItemCategories
        public static void FillItemCategories(DropDownList ddl, bool AddAllCategories, bool SelectMED)
        {
            ddl.Items.Clear();
            string strSQL = "Select CategoryID, CategoryName from masItemCategories Order By CategoryName";
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
            //if (AddAllPSAs)
            //    ddl.Items.Insert(0, new ListItem("All PSAs", "0"));

            //if (ddl.Page.Session["usrPSAID"] != null)
            //{
            //    ddl.SelectedValue = ddl.Page.Session["usrPSAID"].ToString();
            //    ddl.Enabled = false;
            //}
            //else if (SelectFirst && ddl.Items.Count > 0)
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

    //public GenFunctions()
    //{
        //
        // TODO: Add constructor logic here
        //
    //}
    }

}
