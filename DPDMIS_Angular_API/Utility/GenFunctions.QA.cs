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
using System.Text;
#endregion

/// <summary>
/// Summary description for GenFunctions
/// </summary>
public partial class GenFunctions
{
    #region QA
    public class QA
    {
        #region FillFinancialYearBySamples
        public static void FillFinancialYearBySamples(DropDownList ddl, bool AddAllSources, bool SelectFirst)
        {
            ddl.Items.Clear();
            string strSQL = "SELECT DISTINCT A.accyrsetid,B.accyear FROM soorderplaced A INNER JOIN masaccyearsettings B ON (A.accyrsetid = B.accyrsetid) WHERE a.status NOT IN 'I'";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            ddl.DataTextField = "accyear";
            ddl.DataValueField = "accyrsetid";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (AddAllSources)
                ddl.Items.Insert(0, new ListItem("All Year", "0"));
            if (SelectFirst && ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }
        #endregion

        #region FillFinancialYearBySamples
        public static void FillSupplierByYear(DropDownList ddl, string accyrsetid, bool AddAllSchemes, bool SelectFirst)
        {
            ddl.Items.Clear();
            string strSQL = "SELECT DISTINCT A.supplierid,B.suppliername FROM soorderplaced A INNER JOIN MASSUPPLIERS B ON (A.supplierid = B.supplierid) WHERE A.accyrsetid= " + accyrsetid + "  AND a.status NOT IN 'I' Order By B.suppliername";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            ddl.DataTextField = "suppliername";
            ddl.DataValueField = "supplierid";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (AddAllSchemes)
                ddl.Items.Insert(0, new ListItem("All Suppliers", "0"));
            if (SelectFirst && ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }
        #endregion

        #region FillPoBySamples_Source_Scheme
        public static void FillPoBySamples_Source_Scheme(DropDownList ddl, string accyrsetid, string SipplierId, bool AddAllPoNos, bool SelectFirst)
        {
            string strCondition = string.Empty;
            ddl.Items.Clear();
            string strSQL = "SELECT distinct A.PoNoID, A.PoNo FROM soOrderPlaced A WHERE  A.accyrsetid = " + accyrsetid + " and A.SupplierId = " + SipplierId + "  AND a.status NOT IN 'I'  Order by a.PoNo";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            ddl.DataTextField = "PoNo";
            ddl.DataValueField = "PoNoID";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (AddAllPoNos)
                ddl.Items.Insert(0, new ListItem("All supply orders", "0"));
            if (SelectFirst && ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }
        #endregion

        #region FillSourcesBySamples
        public static void FillSourcesBySamples(DropDownList ddl, bool AddAllSources, bool SelectFirst)
        {
            ddl.Items.Clear();
            string strSQL = "Select Distinct a.SourceID, a.SourceName from masSources a inner join qcSamples b on (b.SourceID = a.SourceID) Order By a.SourceName";
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

        #region FillNSQDrug
        public static void FillNSQDrug(DropDownList ddl, bool AddAllNSQDrug, bool SelectFirst)
        {
            ddl.Items.Clear();
            string strSQL = " select distinct i.itemname || ' - ' || itemcode as itemname, i.itemid from qcsamples s, masitems i, qctests t, qcnsqbatches b "+
                            " where s.itemid=i.itemid and t.testresult='NSQ' and t.sampleid=s.sampleid and t.testno=1 "+
                            " and b.sampleid=t.sampleid and isconfirmed=1";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            ddl.DataTextField = "itemname";
            ddl.DataValueField = "itemid";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (AddAllNSQDrug)
                ddl.Items.Insert(0, new ListItem("All NSQ Drug", "0"));
            if (SelectFirst && ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }
        #endregion
        #region FillSchemesBySamples
        public static void FillSchemesBySamples(DropDownList ddl, string SourceID, bool AddAllSchemes, bool SelectFirst)
        {
            if (SourceID == "") SourceID = "0";
            ddl.Items.Clear();
            string strSQL = "Select Distinct a.SchemeID, a.SchemeName from masSchemes a inner join qcSamples b on (b.SchemeID = a.SchemeID) Where b.SourceID = " + SourceID + " Order By a.SchemeName";
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

        #region FillLabByState
        public static void FillLabsByState(DropDownList ddl, string StateID, bool AddAllLabs, bool SelectFirst)
        {
            ddl.Items.Clear();
            if (StateID != "") StateID = " Where b.StateID = " + StateID;
            string strSQL = "Select distinct b.LabID, b.LabCode, b.LabName from qcLabs b " + StateID + " Order by b.LabName";
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
        #endregion

        #region FillLabs
        public static void FillLabs(DropDownList ddl, bool AddAllLabs, bool SelectFirst)
        {
            ddl.Items.Clear();
            string strSQL = "Select distinct b.LabID, b.LabCode, b.LabName from qcLabs b Order by b.LabName";
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
        #endregion

        #region FillSuppliersBySamples_Source_Scheme
        public static void FillSuppliersBySamples_Source_Scheme(DropDownList ddl, string SourceID, string SchemeID, bool AddAllSuppliers, bool SelectFirst)
        {
            ddl.Items.Clear();
            if (SourceID == "") SourceID = "0";
            if (SchemeID == "") SchemeID = "0";
            string strSQL = "Select distinct b.SupplierID, b.SupplierCode, b.SupplierName from masSuppliers b inner join qcSamples a on (a.SupplierID = b.SupplierID) where a.SourceID = " + SourceID + " and a.SchemeID = " + SchemeID + " Order by b.SupplierName";
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

        #region FillPoNosBySamples_Source_Scheme_Supplier
        public static void FillPoNosBySamples_Source_Scheme_Supplier(DropDownList ddl, string SourceID, string SchemeID, string SupplierID, bool AddAllPoNos, bool SelectFirst)
        {
            string strCondition = string.Empty;
            ddl.Items.Clear();
            if (SourceID == "") SourceID = "0";
            if (SchemeID == "") SchemeID = "0";
            //if (SupplierID == "") SupplierID = "0";
            if (!string.IsNullOrEmpty(SupplierID) && SupplierID != "0" )
            {
                strCondition += " and b.SupplierID = " + SupplierID;
            }
            string strSQL = "Select distinct a.PoNoID, a.PoNo from soOrderPlaced a inner join qcSamples b on (b.PoNoID = a.PoNoID) where b.SourceID = " + SourceID + " and b.SchemeID = " + SchemeID + strCondition + " Order by a.PoNo";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            ddl.DataTextField = "PoNo";
            ddl.DataValueField = "PoNoID";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (AddAllPoNos)
                ddl.Items.Insert(0, new ListItem("All supply orders", "0"));
            if (SelectFirst && ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }
        #endregion

        #region FillLabsByTests_Source_Scheme
        public static void FillLabsByTests_Source_Scheme(DropDownList ddl, string SourceID, string SchemeID, bool AddAllLabs, bool SelectFirst)
        {
            ddl.Items.Clear();
            if (SourceID == "") SourceID = "0";
            if (SchemeID == "") SchemeID = "0";

            //string strSQL = "Select distinct b.LabID, b.LabCode, b.LabName" +
            //    " from qcLabs b" +
            //    " inner join tbOutwards a on (a.LabID = b.LabID)" +
            //    " inner join qcSamples c on (c.OutwNo = a.OutwNo)" +
            //    " where c.SourceID = " + SourceID + " and c.SchemeID = " + SchemeID +
            //    " Order by b.LabName";
            string strSQL = "Select distinct b.LabID, b.LabCode, b.LabName from qcLabs b inner join qcTests a on (a.LabID = b.LabID) inner join qcSamples c on (c.SampleID = a.SampleID) where c.SourceID = " + SourceID + " and c.SchemeID = " + SchemeID + " Order by b.LabName";
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
        #endregion

        #region GetAllLabCodes
        public static string[] GetAllLabCodes(string prefixText, int count, string contextKey)
        {
            if (contextKey == null) contextKey = "";
            return GetDBValues(prefixText, count, contextKey, "qcLabs", "LabCode");
        }
        #endregion

        #region GetAllLabNames
        public static string[] GetAllLabNames(string prefixText, int count, string contextKey)
        {
            if (contextKey == null) contextKey = "";
            return GetDBValues(prefixText, count, contextKey, "qcLabs", "LabName");
        }
        #endregion


        #region FillQADates        
        public static void FillQADates(DropDownList ddl, bool AddAllItems, bool SelectFirst, string Conditions)
        {
            ddl.Items.Clear();
            DataTable dt = DBHelper.GetDataTable(Conditions);
            ddl.DataTextField = "QADate";
            ddl.DataValueField = "QADate";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (AddAllItems)
                ddl.Items.Insert(0, new ListItem("All Dates", "0"));
            if (SelectFirst && ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }
        #endregion


        #region AutoGenerateSampleNumber
        public static string AutoGenerateSampleNumber(bool IsNext)
        {
            string mToday = GenFunctions.CheckDBNullForDate(DateTime.Now);
            string mSampleNumber = string.Empty;
            GenFunctions.CheckDate(ref mToday, "Today", false);
            string strSQL1 = "Select SHAccYear from masAccYearSettings Where " + mToday + " between StartDate and EndDate";
            DataTable dt1 = DBHelper.GetDataTable(strSQL1);
            if (dt1.Rows.Count > 0)
            {
                string mSHAccYear = dt1.Rows[0]["SHAccYear"].ToString();
                string strSql = string.Empty;
                if (!IsNext)
                {
                    strSql = "Select Lpad(NVL(Max(To_Number(SubStr(SampleNo, 11, 6))), 0) + 1, 6, '0') SampleNo from qcTests";
                }
                else
                {
                    strSql = "Select Lpad(NVL(Max(To_Number(SubStr(SampleNo, 11, 6))), 0) + 2, 6, '0') SampleNo from qcTests";
                }
                DataTable dt = DBHelper.GetDataTable(strSql);
                if (dt.Rows.Count > 0)
                {
                    mSampleNumber = RandomString(3) + "-" + mSHAccYear + "-" + dt.Rows[0]["SampleNo"].ToString();
                }
                else
                {
                    mSampleNumber = RandomString(3) + "-" + mSHAccYear + "-" + "000001";
                }
            }
            return mSampleNumber;
        }
        #endregion



        private static Random random = new Random((int)DateTime.Now.Ticks);
        public static string RandomString(int size)
        {
            StringBuilder builder = new StringBuilder();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }

      
    }
    #endregion
}
