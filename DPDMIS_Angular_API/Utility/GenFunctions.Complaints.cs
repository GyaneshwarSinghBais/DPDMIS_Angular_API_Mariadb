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
public partial  class GenFunctions
{
    public class Complaints
     {
    # region Archive
   public static void Archive ()
    {

        string strSQL = "Select Value From Massettings Where Keys='ComplaintArchiveMonth'";
        DataTable dt = DBHelper.GetDataTable(strSQL);
        if (dt.Rows.Count > 0)
        {
            DataRow dr = dt.Rows[0]; 
            string strUpdateQuery = "Update  Cmpcomplaints  Set Archive=null";
            DBHelper.GetDataTable(strUpdateQuery);
            string strQuery = "Update  Cmpcomplaints  Set Archive='A' Where Complaintdate < '" + DateTime.Now.AddMonths(-(int.Parse(GenFunctions.CheckDBNull(dr["Value"])))).ToString("dd-MMM-yyyy") + "'  and status='C'";
            DBHelper.GetDataTable(strQuery);
        }
    }
    # endregion
     }
}
