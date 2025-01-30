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
using System.Net;
#endregion

/// <summary>
/// Summary description for GenFunctions
/// </summary>
public partial class GenFunctions
{
    public class AuditLogs
    {
        #region Insert Audit Log
        public static void InsertAuditLog(string Operation, string Description)
        {
            InsertAuditLog(Operation, "", Description);
        }

        public static void InsertAuditLog(string Operation, string TableName, string Description)
        {
            #region Collect Data
            string UserID = HttpContext.Current.Session["MemberID"].ToString();
            string LogDate = DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss tt");
            string strIPAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString();
            if (strIPAddress == "::1") { strIPAddress = "Server"; };
            #endregion

            #region Validate Data
            string ErrMsg = "";
            ErrMsg += GenFunctions.CheckNumber(ref UserID, "UserID", false);
            ErrMsg += GenFunctions.CheckDateWithTime(ref LogDate, "Log date", false);
            ErrMsg += GenFunctions.CheckNumber(ref Operation, "Operation", false);
            ErrMsg += GenFunctions.CheckStringForEmpty(ref TableName, "Table Name", true);
            ErrMsg += GenFunctions.CheckStringForEmpty(ref Description, "Description", true);
            ErrMsg += GenFunctions.CheckStringForEmpty(ref strIPAddress, "IPAddress", false);

            if (ErrMsg != "")
            {
                throw new Exception(ErrMsg);
            }
            #endregion

            #region Save Data
            string strSQL = "Insert into genAuditLogs (UserID,LogDate,Operation,TableName,Description,IPADDRESS) values (" +
                UserID + "," + LogDate + "," + Operation + "," + TableName + "," + Description + "," + strIPAddress + ")";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            #endregion
        }
        #endregion
    }
}
