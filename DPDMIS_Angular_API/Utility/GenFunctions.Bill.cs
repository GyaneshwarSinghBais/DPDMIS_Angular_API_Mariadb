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


    #region Bill
    public class Bill
    {
        #region FillTaxTypesByCategory
        public static void FillTaxTypesByCategory(DropDownList ddl, string Category, string WhereClause, bool AddAllTaxTypes, bool SelectFirst)
        {
            ddl.Items.Clear();
            if (Category == "") Category = "0";
            string strSQL = "Select TaxTypeID, TaxTypeName from blpTaxTypes Where TaxTypeName not in ('Liquidated Damges','Logo Charges','Un-Executed Penalties','Liquidated Damges (System Generated)','VAT Charges')AND TaxCategory = '" + Category + "' " + WhereClause + "  Order By TaxTypeName";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            ddl.DataTextField = "TaxTypeName";
            ddl.DataValueField = "TaxTypeID";
            ddl.DataSource = dt;
            ddl.DataBind();
            if (AddAllTaxTypes)
                ddl.Items.Insert(0, new ListItem("All Sources", "0"));
            if (SelectFirst && ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }
        #endregion

        #region Commented
        //public static bool CheckAndCompleteInvoice(string mPoNoID, ref string ErrMsg)
        //{
        //    ErrMsg = string.Empty;
        //    if (mPoNoID.Trim() == string.Empty || mPoNoID.Trim() == "0")
        //        ErrMsg += "Invalid PoNo.<br />";
        //    else
        //    {
        //        string stQuery = " Select so.PoNoID, so.SOValue, (Nvl(Sum(rb.AbsRQty),0) * soi.SingleUnitPrice)TotReceipt, soi.SingleUnitPrice, soi.ItemValue " +
        //                        " From SoOrderPlaced so " +
        //                        " Inner Join SoOrderedItems soi on (soi.PoNoID = so.PoNoID)  " +
        //                        " Inner Join tbReceipts r on (r.PoNoID=so.PoNoID)" +
        //                        " Inner Join tbReceiptItems ri on (ri.ReceiptID=r.ReceiptID and ri.OrderItemID=soi.OrderItemID and ri.ItemID=soi.ItemID)" +
        //                        " Inner Join tbReceiptBatches rb on (rb.ReceiptItemID=ri.ReceiptItemID) " +
        //                        " Where so.PoNoID=" + mPoNoID +
        //                        " Group by so.PoNoID, so.SOValue, soi.SingleUnitPrice, soi.ItemValue ";
        //        DataTable dt = DBHelper.GetDataTable(stQuery);
        //        if (dt.Rows.Count > 0)
        //        {
        //            decimal SOValue = decimal.Parse(dt.Rows[0]["SOValue"].ToString());
        //            decimal TotReceipt = decimal.Parse(dt.Rows[0]["TotReceipt"].ToString());
        //            decimal Receipt = (TotReceipt / SOValue) * 100;
        //            if (Receipt < 70)

        //                ErrMsg += "Receipt Value should not be less than 70% of  Supply Order Value.";

        //        }

        //    }
        //    if (ErrMsg == string.Empty)
        //    {

        //        return true;
        //    }
        //    else
        //        return false;
        //}


        //#region CheckAndCompleteInvoice
        //public static bool CheckAndCompleteInvoice(string mPoNoID, Label lblMsg)
        //{
        //    string ErrMsg = "";
        //    if (mPoNoID == "0" || mPoNoID == "")
        //    {
        //        ErrMsg += "Invalid Supply Order. Try again<br />";
        //    }
        //    else
        //    {
        //        try
        //        {
        //            string stQuery = " Select so.SOValue, (Nvl(Sum(rb.AbsRQty),0) * soi.SingleUnitPrice)TotReceipt " +
        //                      " From SoOrderPlaced so " +
        //                      " Inner Join SoOrderedItems soi on (soi.PoNoID = so.PoNoID)  " +
        //                      " Inner Join tbReceipts r on (r.PoNoID=so.PoNoID)" +
        //                      " Inner Join tbReceiptItems ri on (ri.ReceiptID=r.ReceiptID and ri.OrderItemID=soi.OrderItemID and ri.ItemID=soi.ItemID)" +
        //                      " Inner Join tbReceiptBatches rb on (rb.ReceiptItemID=ri.ReceiptItemID) " +
        //                      " Where so.PoNoID=" + mPoNoID +
        //                      " Group by so.SOValue ";
        //            DataTable dt = DBHelper.GetDataTable(stQuery);
        //            if (dt.Rows.Count > 0)
        //            {
        //                decimal SOValue = decimal.Parse(dt.Rows[0]["SOValue"].ToString());
        //                decimal TotReceipt = decimal.Parse(dt.Rows[0]["TotReceipt"].ToString());
        //                decimal Receipt = (TotReceipt / SOValue) * 100;
        //                if (Receipt < 70)
        //                {
        //                    ErrMsg += "Receipt Value should not be less than 70% of  Supply Order Value.";
        //                }
        //            }
        //        }
        //        catch (Exception Exp)
        //        {
        //            ErrMsg += Exp.Message + "<br />";
        //        }
        //    }

        //    if (ErrMsg != "")
        //    {
        //        lblMsg.Text = "";
        //        lblMsg.ForeColor = Color.Red;
        //        return false;
        //    }
        //    else
        //        return true;
        //}
        //#endregion
        #endregion

        #region CheckAndCompleteInvoice
        public static bool CheckAndCompleteInvoice(string mPoNoID, ref string ErrMsg)
        {
            CheckOrderCompletionPercent(mPoNoID, ref ErrMsg);

            return (ErrMsg != string.Empty) ? true : false;
        }
        #endregion

        #region CheckOrderCompletionPercent
        public static bool CheckOrderCompletionPercent(string mPoNoID)
        {
            string ErrMsg = string.Empty;
            return CheckOrderCompletionPercent(mPoNoID, ref ErrMsg);
        }
        public static bool CheckOrderCompletionPercent(string mPoNoID, ref string ErrMsg)
        {
            decimal OrderCompletionPercent = 0;

            string sttQuery = "Select Value from massettings where keys = 'CreateInvoiceOn_OrderCompletionPercent' ";
            DataTable dtt1 = DBHelper.GetDataTable(sttQuery);
            if (dtt1.Rows.Count > 0)
            {
                OrderCompletionPercent = decimal.Parse(dtt1.Rows[0]["Value"].ToString());
            }

            string stQuery = " Select  so.PoNoID, so.SOValue, Sum(Nvl(rb.AbsRQty,0) * (soi.SingleUnitPrice)) TotReceipt " +
                      " From SoOrderPlaced so " +
                      " Inner Join SoOrderedItems soi on (soi.PoNoID = so.PoNoID)  " +
                      " Inner Join tbReceipts r on (r.PoNoID=so.PoNoID)" +
                      " Inner Join tbReceiptItems ri on (ri.ReceiptID=r.ReceiptID and ri.OrderItemID=soi.OrderItemID and ri.ItemID=soi.ItemID)" +
                      " Inner Join tbReceiptBatches rb on (rb.ReceiptItemID=ri.ReceiptItemID) " +
                      " Where so.PoNoID=" + mPoNoID +
                      " Group by so.PoNoID, so.SOValue ";

            DataTable dt = DBHelper.GetDataTable(stQuery);
            if (dt.Rows.Count > 0)
            {
                decimal SOValue = decimal.Parse(dt.Rows[0]["SOValue"].ToString());
                decimal TotReceipt = decimal.Parse(dt.Rows[0]["TotReceipt"].ToString());
                decimal Receipt = (TotReceipt / SOValue) * 100;
                if (Receipt < OrderCompletionPercent)
                {
                    ErrMsg += "Receipt Value should not be less than " + OrderCompletionPercent.ToString() + "% of  Supply Order Value.";
                    return false;
                }
            }

            return true;

        }
        #endregion
        

    }
    #endregion
}
