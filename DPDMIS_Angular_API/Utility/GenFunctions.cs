#region Using
using System;
using System.Collections;
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

public partial class GenFunctions
{
    #region Constructor
    public GenFunctions()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    #endregion

    #region Auto Generate Numbers
    #region WH-AutogenerateNumbers
    public static string AutoGenerateNumbers(string WarehouseID, bool IsReceipt, string mType)
    { 
        return AutoGenerateNumbers(WarehouseID, true , IsReceipt, mType);
    }

    public static string AutoGenerateNumbers(string ID, bool IsWarehouse, bool IsReceipt, string mType)
    {
        string mGenNo = "";
        string mID = GenFunctions.CheckEmptyString(ID, "0"); ;
        string strSQL = string.Empty;

        if (IsWarehouse)
        {
            strSQL = "Select WarehouseCode as Prefix from masWarehouses Where WarehouseID = " + mID;
        }
        else 
        {
            strSQL = "Select FacilityCode as Prefix from masFacilities Where FacilityID = " + mID;
        }

        DataTable dt = DBHelper.GetDataTable(strSQL);
        if (dt.Rows.Count > 0)
        {
            string mToday = GenFunctions.CheckDBNullForDate(DateTime.Now);
            GenFunctions.CheckDate(ref mToday, "Today", false);
            string strSQL1 = "Select SHAccYear from masAccYearSettings Where " + mToday + " between StartDate and EndDate";
            DataTable dt1 = DBHelper.GetDataTable(strSQL1);
            if (dt1.Rows.Count > 0)
            {
                string mSHAccYear = dt1.Rows[0]["SHAccYear"].ToString();
                string mPrefix = dt.Rows[0]["Prefix"].ToString();

                if (IsWarehouse)
                {
                    if (IsReceipt)
                        strSQL = "Select Lpad(NVL(Max(To_Number(SubStr(ReceiptNo, 10, 5))), 0) + 1, 5, '0') as WHSlNo from tbReceipts Where ReceiptNo Like '" + mPrefix + "/" + mType + "/%/" + mSHAccYear + "'";
                    else
                        strSQL = "Select Lpad(NVL(Max(To_Number(SubStr(IndentNo, 10, 5))), 0) + 1, 5, '0') as WHSlNo from tbIndents Where IndentNo Like '" + mPrefix + "/" + mType + "/%/" + mSHAccYear + "'";
                }
                else 
                {
                    if (IsReceipt)
                        strSQL = "Select Lpad(NVL(Max(To_Number(SubStr(FacReceiptNo, 10, 5))), 0) + 1, 5, '0') as WHSlNo from tbFacilityReceipts Where FacReceiptNo Like '" + mPrefix + "/" + mType + "/%/" + mSHAccYear + "'";
                    else
                        strSQL = "Select Lpad(NVL(Max(To_Number(SubStr(FacIndentNo, 10, 5))), 0) + 1, 5, '0') as WHSlNo from tbFacilityIndents Where FacIndentNo Like '" + mPrefix + "/" + mType + "/%/" + mSHAccYear + "'";
                }
                

                DataTable dt3 = DBHelper.GetDataTable(strSQL);
                if (dt3.Rows.Count > 0)
                    mGenNo = mPrefix + "/" + mType + "/" + dt3.Rows[0]["WHSlNo"].ToString() + "/" + mSHAccYear;
                else
                    mGenNo = mPrefix + "/" + mType + "/" + "00001" + "/" + mSHAccYear;
            }
        }
        return mGenNo;
    }
    #endregion

    #region WH-AutogenerateNumbers - LP Issue Number
    public static string LPAutoGenerateIssueNO(string WarehouseID, bool IsReceipt, string mType)
    {
        string mGenNo = "";
        string mWHID = GenFunctions.CheckEmptyString(WarehouseID, "0");
        string strSQL = "Select WarehouseCode from masWarehouses Where WarehouseID = " + mWHID;
        DataTable dt = DBHelper.GetDataTable(strSQL);
        if (dt.Rows.Count > 0)
        {
            string mToday = GenFunctions.CheckDBNullForDate(DateTime.Now);
            GenFunctions.CheckDate(ref mToday, "Today", false);
            string strSQL1 = "Select SHAccYear from masAccYearSettings Where " + mToday + " between StartDate and EndDate";
            DataTable dt1 = DBHelper.GetDataTable(strSQL1);
            if (dt1.Rows.Count > 0)
            {
                string mSHAccYear = dt1.Rows[0]["SHAccYear"].ToString();
                string mWHPrefix = dt.Rows[0]["WarehouseCode"].ToString();
                if (IsReceipt)
                    strSQL = "Select Lpad(NVL(Max(To_Number(SubStr(ReceiptNo, 10, 5))), 0) + 1, 5, '0') as WHSlNo from LPtbReceipts Where ReceiptNo Like '" + mWHPrefix + "/" + mType + "/%/" + mSHAccYear + "'";
                else
                    strSQL = "Select Lpad(NVL(Max(To_Number(SubStr(IndentNo, 10, 5))), 0) + 1, 5, '0') as WHSlNo from LPtbIndents Where IndentNo Like '" + mWHPrefix + "/" + mType + "/%/" + mSHAccYear + "'";
                DataTable dt3 = DBHelper.GetDataTable(strSQL);
                if (dt3.Rows.Count > 0)
                    mGenNo = mWHPrefix + "/" + mType + "/" + dt3.Rows[0]["WHSlNo"].ToString() + "/" + mSHAccYear;
                else
                    mGenNo = mWHPrefix + "/" + mType + "/" + "00001" + "/" + mSHAccYear;
            }
        }
        return mGenNo;
    }
    #endregion


  public static string AutoGenFunrRequestCode(string mPSAID, string mAccYrSetID)
    {
        string mItemCode = "";
        if (mPSAID.Trim() == "") mPSAID = "0";
        if (mAccYrSetID.Trim() == "") mAccYrSetID = "0";

        string strSQL3 = @"Select Lpad(NVL(Max(To_Number(a.auto_nccode)), 0) + 1, 5, '0') as SOCode
            From MASFACFUNDREQUEST a  ";
        DataTable dt3 = DBHelper.GetDataTable(strSQL3);

        if (dt3.Rows.Count > 0)
            mItemCode = dt3.Rows[0]["SOCode"].ToString();
        else
            mItemCode = "00001";

        return mItemCode;
    }
	
	
	
	   public static string AutoGenFacAiTransferCode(string mPSAID, string mAccYrSetID)
    {
        string mItemCode = "";
        if (mPSAID.Trim() == "") mPSAID = "0";
        if (mAccYrSetID.Trim() == "") mAccYrSetID = "0";

        string strSQL3 = @"Select Lpad(NVL(Max(To_Number(a.auto_code)), 0) + 1, 5, '0') as SOCode
            From MASFACILITYAITRANSFER a  ";
        DataTable dt3 = DBHelper.GetDataTable(strSQL3);

        if (dt3.Rows.Count > 0)
            mItemCode = dt3.Rows[0]["SOCode"].ToString();
        else
            mItemCode = "00001";

        return mItemCode;
    }
	
	
	
	 public static string AutoGenerateDHSAIStoreCode(string mPSAID, string mAccYrSetID)
  {
      string mItemCode = "";
      if (mPSAID.Trim() == "") mPSAID = "0";
      if (mAccYrSetID.Trim() == "") mAccYrSetID = "0";

      string strSQL3 = " Select Lpad(NVL(Max(To_Number(a.AUTO_RAICODE)), 0) + 1, 5, '0') as SOCode" +
          " From masrevisedannualindent  a" +
          " where a.FACILITYID=" + mPSAID + " and a.accyrsetid=" + mAccYrSetID;
      DataTable dt3 = DBHelper.GetDataTable(strSQL3);

      if (dt3.Rows.Count > 0)
          mItemCode = dt3.Rows[0]["SOCode"].ToString();
      else
          mItemCode = "00001";

      return mItemCode;
  }
	
    #region WH-AutogenerateNumbers-Local Purchase
    public static string LPAutoGenerateNumbers(string FacilityID, bool IsReceipt, string mType)
    {
        string mGenNo = "";
        string mFACID = GenFunctions.CheckEmptyString(FacilityID, "0");
        string strSQL = "Select FacilityCode,SUBSTR(FacilityCode, 1, 2) Faccd from masFacilities Where FacilityID = " + mFACID;
        DataTable dt = DBHelper.GetDataTable(strSQL);
        if (dt.Rows.Count > 0)
        {
            string mToday = GenFunctions.CheckDBNullForDate(DateTime.Now);
            GenFunctions.CheckDate(ref mToday, "Today", false);
            string strSQL1 = "Select SHAccYear from masAccYearSettings Where " + mToday + " between StartDate and EndDate";
            DataTable dt1 = DBHelper.GetDataTable(strSQL1);
            if (dt1.Rows.Count > 0)
            {
                string mSHAccYear = dt1.Rows[0]["SHAccYear"].ToString();
                string mWHPrefix = dt.Rows[0]["FacilityCode"].ToString();
                if (IsReceipt)
		{
                    if (dt.Rows[0]["Faccd"].ToString() == "CG")
                    {
                        strSQL = "Select Lpad(NVL(Max(To_Number(SubStr(FacReceiptNo, -11, 5))), 0) + 1, 5, '0') as WHSlNo from tbfacilityReceipts Where facReceiptNo Like '" + mWHPrefix + "/" + mType + "/%/" + mSHAccYear + "'";
                    }
                    else
                    {
                        strSQL = "Select Lpad(NVL(Max(To_Number(SubStr(FacReceiptNo, -11, 5))), 0) + 1, 5, '0') as WHSlNo from tbfacilityReceipts Where facReceiptNo Like '" + mWHPrefix + "/" + mType + "/%/" + mSHAccYear + "'";
                    }
                }
		else
                 {
                     strSQL = "Select Lpad(NVL(Max(To_Number(SubStr(FacReceiptNo, -11, 5))), 0) + 1, 5, '0') as WHSlNo from LPtbOutwards Where OutWNo Like '" + mWHPrefix + "/" + mType + "/%/" + mSHAccYear + "'";
               }
		 DataTable dt3 = DBHelper.GetDataTable(strSQL);
                if (dt3.Rows.Count > 0)
                    mGenNo = mWHPrefix + "/" + mType + "/" + dt3.Rows[0]["WHSlNo"].ToString() + "/" + mSHAccYear;
                else
                    mGenNo = mWHPrefix + "/" + mType + "/" + "00001" + "/" + mSHAccYear;
                 
            }
        }
        return mGenNo;
    }
    #endregion

    #region AutoGenerateItemCode
    public static string AutoGenerateItemCode(string mItemCatID, string mItemTypeID)
    {
        string mItemCode = "";
        string mCatPrefix = "";
        string mITPrefix = "";
        if (mItemCatID.Trim() == "") mItemCatID = "0";
        if (mItemTypeID.Trim() == "") mItemTypeID = "0";
        string strSQL1 = "Select A.CategoryPrefix FROM masitemcategories A Where A.CategoryID = " + mItemCatID;
        DataTable dt1 = DBHelper.GetDataTable(strSQL1);
        if (dt1.Rows.Count > 0)
            mCatPrefix = dt1.Rows[0]["CategoryPrefix"].ToString();
        string strSQL2 = "Select B.TypePrefix from masitemtypes B Where B.ItemTypeID = " + mItemTypeID;
        DataTable dt2 = DBHelper.GetDataTable(strSQL2);
        if (dt2.Rows.Count > 0)
            mITPrefix = dt2.Rows[0]["TypePrefix"].ToString();
        if (mCatPrefix != "" && mITPrefix != "")
        {
            string strSQL3 = "Select Lpad(NVL(Max(To_Number(SubStr(ItemCode, 4, 7))), 0) + 1, 4, '0') as ItemSlNo from masItems Where ItemCode Like '" + mCatPrefix + mITPrefix + "%'";
            DataTable dt3 = DBHelper.GetDataTable(strSQL3);
            if (dt3.Rows.Count > 0)
                mItemCode = mCatPrefix + mITPrefix + dt3.Rows[0]["ItemSlNo"].ToString();
            else
                mItemCode = mCatPrefix + mITPrefix + "0001";
        }
        return mItemCode;
    }
    #endregion

    #region AutoGenerateWHCode
    public static string AutoGenerateWHCode(string mStateID)
    {
        string strWHCode = "";
        if (mStateID.Trim() == "") mStateID = "0";
        string strSQL = "Select StateCode from masStates Where StateID = " + mStateID;
        DataTable dt = DBHelper.GetDataTable(strSQL);
        if (dt.Rows.Count > 0)
        {
            string mStatePrefix = dt.Rows[0]["StateCode"].ToString();
            strSQL = "Select Lpad(NVL(Max(To_Number(SubStr(WarehouseCode, 3))), 0) + 1, 3, '0') as WHSlNo from masWarehouses Where WarehouseCode Like '" + mStatePrefix + "%'";
            DataTable dt3 = DBHelper.GetDataTable(strSQL);
            if (dt3.Rows.Count > 0)
                strWHCode = mStatePrefix + dt3.Rows[0]["WHSlNo"].ToString();
            else
                strWHCode = mStatePrefix + "001";
        }
        return strWHCode;
    }
    #endregion


 public static string AutoGenerateLPconsumptionCode(string mPSAID, string mAccYrSetID)
    {
        string mItemCode = "";
        if (mPSAID.Trim() == "") mPSAID = "0";
        if (mAccYrSetID.Trim() == "") mAccYrSetID = "0";

        string strSQL3 = " Select Lpad(NVL(Max(To_Number(a.Auto_LPCNCODE)), 0) + 1, 5, '0') as SOCode" +
            " From maslpconsumption a" +
            " where a.FACILITYID=" + mPSAID + " and a.accyrsetid=" + mAccYrSetID;
        DataTable dt3 = DBHelper.GetDataTable(strSQL3);

        if (dt3.Rows.Count > 0)
            mItemCode = dt3.Rows[0]["SOCode"].ToString();
        else
            mItemCode = "00001";

        return mItemCode;
    }



    public static string AutoGenerateconsumptionCode(string mPSAID, string mAccYrSetID)
    {
        string mItemCode = "";
        if (mPSAID.Trim() == "") mPSAID = "0";
        if (mAccYrSetID.Trim() == "") mAccYrSetID = "0";

        string strSQL3 = " Select Lpad(NVL(Max(To_Number(a.Auto_CNCODE)), 0) + 1, 5, '0') as SOCode" +
            " From MasFacilityConsumption  a" +
            " where a.FACILITYID=" + mPSAID + " and a.accyrsetid=" + mAccYrSetID;
        DataTable dt3 = DBHelper.GetDataTable(strSQL3);

        if (dt3.Rows.Count > 0)
            mItemCode = dt3.Rows[0]["SOCode"].ToString();
        else
            mItemCode = "00001";

        return mItemCode;
    }

 public static string AutoGenerateAnualIndentCode(string mPSAID, string mAccYrSetID)
    {
        string mItemCode = "";
        if (mPSAID.Trim() == "") mPSAID = "0";
        if (mAccYrSetID.Trim() == "") mAccYrSetID = "0";

        string strSQL3 = " Select Lpad(NVL(Max(To_Number(a.Auto_AICODE)), 0) + 1, 5, '0') as SOCode" +
            " From masAnualIndent  a" +
            " where a.FACILITYID=" + mPSAID + " and a.accyrsetid=" + mAccYrSetID;
        DataTable dt3 = DBHelper.GetDataTable(strSQL3);

        if (dt3.Rows.Count > 0)
            mItemCode = dt3.Rows[0]["SOCode"].ToString();
        else
            mItemCode = "00001";

        return mItemCode;
    }

 public static string AutoGenerateRevisedAnnualindentCode(string mPSAID, string mAccYrSetID)
 {
     string mItemCode = "";
     if (mPSAID.Trim() == "") mPSAID = "0";
     if (mAccYrSetID.Trim() == "") mAccYrSetID = "0";

     string strSQL3 = " Select Lpad(NVL(Max(To_Number(a.AUTO_RAICODE)), 0) + 1, 5, '0') as SOCode" +
         " From masrevisedannualindent  a" +
         " where a.DISTRICTID=" + mPSAID + " and a.accyrsetid=" + mAccYrSetID;
     DataTable dt3 = DBHelper.GetDataTable(strSQL3);

     if (dt3.Rows.Count > 0)
         mItemCode = dt3.Rows[0]["SOCode"].ToString();
     else
         mItemCode = "00001";

     return mItemCode;
 }

 public static string AutoGenerateEQAnualIndentCode(string mPSAID, string mAccYrSetID)
 {
     string mItemCode = "";
     if (mPSAID.Trim() == "") mPSAID = "0";
     if (mAccYrSetID.Trim() == "") mAccYrSetID = "0";

     string strSQL3 = " Select Lpad(NVL(Max(To_Number(a.Auto_AICODE)), 0) + 1, 5, '0') as SOCode" +
         " From masAnualIndentEQ  a" +
         " where a.FACILITYID=" + mPSAID + " and a.accyrsetid=" + mAccYrSetID;
     DataTable dt3 = DBHelper.GetDataTable(strSQL3);

     if (dt3.Rows.Count > 0)
         mItemCode = dt3.Rows[0]["SOCode"].ToString();
     else
         mItemCode = "00001";

     return mItemCode;
 }

    #region AutoGenerateFACCode
    public static string AutoGenerateFACCode(string mStateID)
    {
        string strFacilityCode = "";
        if (mStateID.Trim() == "") mStateID = "0";
        string strSQL = "Select StateCode from masStates Where StateID = " + mStateID;
        DataTable dt = DBHelper.GetDataTable(strSQL);
        if (dt.Rows.Count > 0)
        {
            string mStatePrefix = dt.Rows[0]["StateCode"].ToString();
            strSQL = "Select Lpad(NVL(Max(To_Number(SubStr(FacilityCode, 3))), 0) + 1, 5, '0') as WHSlNo from masFacilities Where FacilityCode Like '" + mStatePrefix + "%'";
            DataTable dt3 = DBHelper.GetDataTable(strSQL);
            if (dt3.Rows.Count > 0)
                strFacilityCode = mStatePrefix + dt3.Rows[0]["WHSlNo"].ToString();
            else
                strFacilityCode = mStatePrefix + "00001";
        }
        return strFacilityCode;
    }
    #endregion

    #region RO-AutogenerateNumbers
    public static string AutoGenerateNumbersRO(string ReleaseOrderDate)
    {
        string mGenNo = "";
        string mReleaseOrderDate = ReleaseOrderDate;
        GenFunctions.CheckDate(ref mReleaseOrderDate, "Release Order date", false);
        string strSQL = "Select SHAccYear from masAccYearSettings Where " + mReleaseOrderDate + " between StartDate and EndDate";
        DataTable dt = DBHelper.GetDataTable(strSQL);
        if (dt.Rows.Count > 0)
        {
            string mSHAccYear = dt.Rows[0]["SHAccYear"].ToString();
            string mWHPrefix = "RO";
            strSQL = "Select Lpad(NVL(Max(To_Number(SubStr(ReleaseOrderNo, 4, 5))), 0) + 1, 5, '0') as ReleaseOrderNo from roReleaseOrders Where ReleaseOrderNo Like '" + mWHPrefix + "-%/" + mSHAccYear + "'";
            dt = DBHelper.GetDataTable(strSQL);
            if (dt.Rows.Count > 0)
                mGenNo = mWHPrefix + "-" + dt.Rows[0]["ReleaseOrderNo"].ToString() + "/" + mSHAccYear;
            else
                mGenNo = mWHPrefix + "-" + "00001" + "/" + mSHAccYear;
        }
        return mGenNo;
    }
    #endregion

    #region Fac-AutogenerateNumbers
    public static string FacAutoGenerateNumbers(string FacilityID, bool IsReceipt, string mType)
    {
        string mGenNo = "";
        string mFacID = GenFunctions.CheckEmptyString(FacilityID, "0");
        string strSQL = "Select FacilityCode from masFacilities Where FacilityID = " + mFacID;
        DataTable dt = DBHelper.GetDataTable(strSQL);
        if (dt.Rows.Count > 0)
        {
            string mToday = GenFunctions.CheckDBNullForDate(DateTime.Now);
            GenFunctions.CheckDate(ref mToday, "Today", false);
            string strSQL1 = "Select SHAccYear from masAccYearSettings Where " + mToday + " between StartDate and EndDate";
            DataTable dt1 = DBHelper.GetDataTable(strSQL1);
            if (dt1.Rows.Count > 0)
            {
                string mSHAccYear = dt1.Rows[0]["SHAccYear"].ToString();
                string mWHPrefix = dt.Rows[0]["FacilityCode"].ToString();
                if (IsReceipt)
                    strSQL = "Select Lpad(NVL(Max(To_Number(SubStr(FacReceiptNo, -11, 5))), 0) + 1, 5, '0') as WHSlNo from tbFacilityReceipts Where FacReceiptNo Like '" + mWHPrefix + "/" + mType + "/%/" + mSHAccYear + "'";
                else
                    strSQL = "Select Lpad(NVL(Max(To_Number(SubStr(IssueNo, -11, 5))), 0) + 1, 5, '0') as WHSlNo from tbFacilityIssues Where IssueNo Like '" + mWHPrefix + "/" + mType + "/%/" + mSHAccYear + "'";
                DataTable dt3 = DBHelper.GetDataTable(strSQL);
                if (dt3.Rows.Count > 0)
                    mGenNo = mWHPrefix + "/" + mType + "/" + dt3.Rows[0]["WHSlNo"].ToString() + "/" + mSHAccYear;
                else
                    mGenNo = mWHPrefix + "/" + mType + "/" + "00001" + "/" + mSHAccYear;
            }
        }
        return mGenNo;
    }
    #endregion

    #region Auto Generate - Supply Order Code
    public static string AutoGenerateSOCode(string mPSAID, string mAccYrSetID)
    {
        string mItemCode = "";
        if (mPSAID.Trim() == "") mPSAID = "0";
        if (mAccYrSetID.Trim() == "") mAccYrSetID = "0";

        string strSQL3 = "Select Lpad(NVL(Max(To_Number(a.auto_socode)), 0) + 1, 5, '0') as SOCode" +
            " From soOrderPlaced a" +
            " where a.psaid=" + mPSAID + " and a.accyrsetid=" + mAccYrSetID;
        DataTable dt3 = DBHelper.GetDataTable(strSQL3);

        if (dt3.Rows.Count > 0)
            mItemCode = dt3.Rows[0]["SOCode"].ToString();
        else
            mItemCode = "00001";

        return mItemCode;
    }
    #endregion
    
    #region Auto Generate - Supply Order Code for Local Purchase
    public static string AutoGenerateLPSOCode(string mPSAID, string mAccYrSetID)
    {
        string mItemCode = "";
        if (mPSAID.Trim() == "") mPSAID = "0";
        if (mAccYrSetID.Trim() == "") mAccYrSetID = "0";

        string strSQL3 = "Select Lpad(NVL(Max(To_Number(a.auto_socode)), 0) + 1, 5, '0') as SOCode" +
            " From LPsoOrderPlaced a" +
            " where a.psaid=" + mPSAID + " and a.accyrsetid=" + mAccYrSetID;
        DataTable dt3 = DBHelper.GetDataTable(strSQL3);

        if (dt3.Rows.Count > 0)
            mItemCode = dt3.Rows[0]["SOCode"].ToString();
        else
            mItemCode = "00001";

        return mItemCode;
    }
    #endregion


    #region Auto Generate - Supply Order Code for Local Purchase
    public static string AutoGenerateNOCCode(string mPSAID, string mAccYrSetID)
    {
        string mItemCode = "";
        if (mPSAID.Trim() == "") mPSAID = "0";
        if (mAccYrSetID.Trim() == "") mAccYrSetID = "0";

        string strSQL3 = " Select Lpad(NVL(Max(To_Number(a.AUTO_NCCODE)), 0) + 1, 5, '0') as SOCode" +
            " From MASCGMSCNOC a" +
            " where a.FACILITYID=" + mPSAID + " and a.accyrsetid=" + mAccYrSetID;
        DataTable dt3 = DBHelper.GetDataTable(strSQL3);

        if (dt3.Rows.Count > 0)
            mItemCode = dt3.Rows[0]["SOCode"].ToString();
        else
            mItemCode = "00001";

        return mItemCode;
    }
    #endregion

    #region Auto Generate - Centralised Supply Order Code
    public static string MOHAutoGenerateSOCode(string mAccYrSetID)
    {
        string mItemCode = "";
        if (mAccYrSetID.Trim() == "") mAccYrSetID = "0";

        string strSQL3 = "Select Lpad(NVL(Max(To_Number(a.auto_socode)), 0) + 1, 5, '0') as SOCode" +
            " From soOrderPlaced a" +
            " where a.accyrsetid=" + mAccYrSetID;
        DataTable dt3 = DBHelper.GetDataTable(strSQL3);

        if (dt3.Rows.Count > 0)
            mItemCode = dt3.Rows[0]["SOCode"].ToString();
        else
            mItemCode = "00001";

        return mItemCode;
    }
    #endregion

    #region AutoGenerateSampleNumber
    public static string AutoGenerateSampleNumber()
    {
        //string mToday = GenFunctions.CheckDBNullForDate(DateTime.Now);
        //string mQAPrefix = "QA";
        //GenFunctions.CheckDate(ref mToday, "Today", false);
        //string strSQL1 = "Select SHAccYear from masAccYearSettings Where " + mToday + " between StartDate and EndDate";
        //DataTable dt1 = DBHelper.GetDataTable(strSQL1);
        //if (dt1.Rows.Count > 0)
        //{
        //    string mSHAccYear = dt1.Rows[0]["SHAccYear"].ToString();

        //}
        //string strWHCode = "";
        //if (mStateID.Trim() == "") mStateID = "0";
        //string strSQL = "Select StateCode from masStates Where StateID = " + mStateID;
        //DataTable dt = DBHelper.GetDataTable(strSQL);
        //if (dt.Rows.Count > 0)
        //{
        //    string mStatePrefix = dt.Rows[0]["StateCode"].ToString();
        //    strSQL = "Select Lpad(NVL(Max(To_Number(SubStr(WarehouseCode, 3))), 0) + 1, 3, '0') as WHSlNo from masWarehouses Where WarehouseCode Like '" + mStatePrefix + "%'";
        //    DataTable dt3 = DBHelper.GetDataTable(strSQL);
        //    if (dt3.Rows.Count > 0)
        //        strWHCode = mStatePrefix + dt3.Rows[0]["WHSlNo"].ToString();
        //    else
        //        strWHCode = mStatePrefix + "001";
        //}
        //return strWHCode;
        return null;
    }
    #endregion

    #endregion

    #region SerialNumber
    public static DataTable GenerateSerialNumber(DataTable dtbl, DataColumn Column)
    {
        if (dtbl != null && dtbl.Rows.Count != 0)
        {
            for (int i = 0; i < dtbl.Rows.Count; i++)
            {
                if (dtbl.Rows[i].RowState != DataRowState.Deleted)
                    dtbl.Rows[i][Column] = i + 1;
            }
        }
        return dtbl;
    }
    #endregion

    #region CreateItems
    private static string[] CreateItems(DataTable dt, DataColumn dc)
    {
        if (dt != null && dt.Rows.Count > 0)
        {
            string[] items = new string[dt.Rows.Count];
            for(int i = 0; i < dt.Rows.Count; i++)
            {
                items.SetValue(dt.Rows[i][dc].ToString(), i);
            }
            return items;
        }
        else
            return new string[] { "No Data Found" };
    }
    #endregion

    #region GetDBValues
    private static string[] GetDBValues(string prefixText, int count, string contextKey, string TableName, string ColumnName)
    {
        string strSQL = "Select * from (SELECT " + ColumnName + ", ROW_NUMBER() OVER (ORDER BY " + ColumnName + ") RNum FROM " + TableName + " WHERE Upper(" + ColumnName + ") like Upper('" + CheckSingleQuote(prefixText) + "%') " + contextKey + " Order By " + ColumnName + ") Where RNum <= " + count.ToString() +
            " union Select * from (SELECT " + ColumnName + ", ROW_NUMBER() OVER (ORDER BY " + ColumnName + ") RNum FROM LPMasItems  WHERE Upper(" + ColumnName + ") like Upper('" + CheckSingleQuote(prefixText) + "%') " + " Order By " + ColumnName + ") Where RNum <= " + count.ToString();
        DataTable dtblLst = DBHelper.GetDataTable(strSQL);
        return CreateItems(dtblLst, dtblLst.Columns[0]);
    }
    #endregion
    #region GetDBValuesLP
    private static string[] GetDBValuesLP(string prefixText, int count, string contextKey, string TableName, string ColumnName)
    {
        string strSQL = "Select * from (SELECT " + ColumnName + ", ROW_NUMBER() OVER (ORDER BY " + ColumnName + ") RNum FROM " + TableName + " WHERE Upper(" + ColumnName + ") like Upper('" + CheckSingleQuote(prefixText) + "%') " + contextKey + " Order By " + ColumnName + ") Where RNum <= " + count.ToString();
         //   " union Select * from (SELECT " + ColumnName + ", ROW_NUMBER() OVER (ORDER BY " + ColumnName + ") RNum FROM LPMasItems  WHERE Upper(" + ColumnName + ") like Upper('" + CheckSingleQuote(prefixText) + "%') " + " Order By " + ColumnName + ") Where RNum <= " + count.ToString();
        DataTable dtblLst = DBHelper.GetDataTable(strSQL);
        return CreateItems(dtblLst, dtblLst.Columns[0]);
    }
    #endregion

    #region GetDBValues
    private static string[] GetDBValues(string Query)
    {
        string strSQL = Query;
        DataTable dtblLst = DBHelper.GetDataTable(strSQL);
        return  CreateItems(dtblLst, dtblLst.Columns[0]);
    }
    #endregion

    #region AlertOnLoad
    public static void AlertOnLoad(Page page, string Message)
    {
        string strScript = "alert('";
        strScript += Message.Replace("<br />", "\\n").Replace("\"", "\\\"").Replace("'", "\\'").Replace("<br/>", "\\n");
        strScript += "');";
        ScriptManager.RegisterStartupScript(page, page.GetType(), "onload", strScript, true);
    }
    #endregion

    #region ValidateDateRange
    public static string ValidateDateRange(string CheckDate)
    {
        string ErrMsg = "";
        string strQur = "select to_char(startdate,'dd-mon-yyyy')startdate, to_char(enddate,'dd-mon-yyyy')enddate from masaccyearsettings where accyrsetID in(select AccyrsetID from masaccyearsettings where to_date('" + DateTime.Now.ToString("dd-MMM-yyyy") + "','dd-mon-yyyy') between startdate and enddate)";
        //string strQur = "select startdate, enddate from masaccyearsettings where accyrsetID in(select AccyrsetID from masaccyearsettings where to_date('" + DateTime.Now.ToString("dd/MM/yyyy") + "','dd-mm-yyyy') between startdate and enddate)";
        DataTable dtblFromDate = DBHelper.GetDataTable(strQur);
        if (dtblFromDate != null && dtblFromDate.Rows.Count > 0)
        {
            DateTime dtF = (DateTime.Parse(dtblFromDate.Rows[0]["startdate"].ToString()));
            DateTime dtT = (DateTime.Parse(dtblFromDate.Rows[0]["endDate"].ToString()));
            DateTime dtS = DateTime.Parse(CheckDate, System.Globalization.CultureInfo.CreateSpecificCulture("en-CA"));
            int stS = DateTime.Compare(dtF, dtS);
            int stT = DateTime.Compare(dtT, dtS);
            if ((stS == -1 || stS == 0) && (stT == 1 || stT == 0))
               ErrMsg = "";          
            else 
            {
                string strFrom = dtF.ToShortDateString();
                string strTo = dtT.ToShortDateString();
                ErrMsg = "Select From" + " " + strFrom + " " + "To " + strTo + "'";
            }
        }
        else
            ErrMsg = "Check Account Year";           
        return ErrMsg;
    }
    #endregion

    #region CheckEmptyString
    public static string CheckEmptyString(object value)
    {
        return CheckEmptyString(value, "");
    }
    public static string CheckEmptyString(object value, string EmptyValue)
    {
        if (value != DBNull.Value && value != null && value.ToString() != "")
            return value.ToString();
        else
            return EmptyValue;
    }
    #endregion

    #region CheckDBNull (with returnObject)
    public static object CheckDBNull(object value, object ReturnIfFoundNull)
    {
        if (value == DBNull.Value || value == null)
            return ReturnIfFoundNull;
        else
            return value;
    }
    #endregion

    #region CheckDBNullForDate
    public static object CheckDBNullDate(object value, object ReturnIfFoundNull)
    {
        
            if (value == DBNull.Value || value == null)
                return ReturnIfFoundNull;
            else
                return ((DateTime)value).ToString("dd-MM-yyyy");
       
    }
    #endregion


    #region IsDBNull
    public static bool IsDBNull(object value)
    {
        if (value == DBNull.Value || value == null || value.ToString() != "")
            return true;
        else
            return false;
    }
    #endregion

    #region CheckDBNull
    public static string CheckDBNull(object value)
    {
        if (value == DBNull.Value || value == null)
            return "";
        else
            return value.ToString();
    }
    #endregion

    #region CheckDBNullForDate
    public static string CheckDBNullForDate(object value)
    {
        try
        {
            if (value == DBNull.Value || value == null)
                return "";
            else
                return ((DateTime)value).ToString("dd-MM-yyyy");
        }
        catch
        {
            return "";
        }
    }
    #endregion

    #region CheckDBNullForDate
    public static string CheckDBNullForDate(object value, string DateFormat)
    {
        try
        {
            if (value == DBNull.Value || value == null)
                return "";
            else
                return ((DateTime)value).ToString(DateFormat);
        }
        catch
        {
            return "";
        }
    }
    #endregion

    #region CheckDBNullForDateTime
    public static string CheckDBNullForDateTime(object value)
    {
        try
        {
            if (value == DBNull.Value || value == null)
                return "";
            else
                return ((DateTime)value).ToString("dd-MM-yyyy hh:mm:ss tt");
        }
        catch
        {
            return "";
        }
    }
    #endregion

    #region CheckDBNullForSingleUnitQty
    public static string CheckDBNullForSingleUnitQty(object value)
    {
        try
        {
            if (value == DBNull.Value || value == null)
                return "";
            else
                return IndianSystemNumber(decimal.Parse(value.ToString()));
        }
        catch
        {
            return "";
        }
    }

    private static string IndianSystemNumber(decimal Value)
    {
        bool IsNegative = false;
        if (Value < 0)
        {
            IsNegative = true;
            Value = Math.Abs(Value);
        }
        string val = Value.ToString();
        string[] num = val.Split('.');
        char[] ch = num[0].ToCharArray();
        string nVal = "";
        for (int i = ch.Length; i > 0; i--)
        {
            int numpos = ch.Length - i;
            if ((numpos == 2 || numpos == 4 || numpos == 6 || numpos == 8 || numpos == 10 || numpos == 12 || numpos == 14) && (numpos != (ch.Length - 1)))
                nVal = "," + ch[i - 1] + nVal;
            else
                nVal = ch[i - 1] + nVal;
        }
        return ((IsNegative) ? "-" : "") + nVal;
    }
        // +"." + num[1];
        //if (val.IndexOf('.') < 0)
        //    val += ".00";
    #endregion

    #region CheckDBNullForINR
    public static string CheckDBNullForINR(object value)
    {
        try
        {
            if (value == DBNull.Value || value == null)
                return "";
            else

                return INR(decimal.Parse(value.ToString()));
        }
        catch
        {
            return "";
        }
    }

    private static string INR(decimal Value)
    {
        bool IsNegative = false;
        if (Value < 0)
        {
            IsNegative = true;
            Value = Math.Abs(Value);
        }
        string val = Value.ToString("0.00");

        //if (val.IndexOf('.') < 0)
        //    val += ".00";

        //if (val.IndexOf('.') > 0)
        //{
        //    if ((val.IndexOf('.') + 2) > val.Length-1)
        //        val += "0";
        //}
        string[] num = val.Split('.');
        char[] ch = num[0].ToCharArray();
        string nVal = "";
        for (int i = ch.Length; i > 0; i--)
        {
            int numpos = ch.Length - i;
            if ((numpos == 2 || numpos == 4 || numpos == 6 || numpos == 8 || numpos == 10 || numpos == 12 || numpos == 14) && (numpos != (ch.Length - 1)))
                nVal = "," + ch[i - 1] + nVal;
            else
                nVal = ch[i - 1] + nVal;
        }
        return ((IsNegative) ? "-" : "") + nVal + "." + num[1];
    }
    //Indian Numbering system
    //1(Ek)
    //10(Das)
    //100(Sau)
    //1000(Hazaar)
    //1,00,000(Lakh)
    //1,00,00,000(Crore)
    //1,00,00,00,000(Arawb)
    //1,00,00,00,00,000(Kharawb)
    //1,00,00,00,00,00,000(Neel)
    //1,00,00,00,00,00,00,000(Padma)
    //1,00,00,00,00,00,00,00,000(Shankh)
    //1,00,00,00,00,00,00,00,00,000(Mahashankh)
    #endregion

    #region CheckSingleQuote
    public static string CheckSingleQuote(string value)
    {
        return value.Replace("'", "''");
    }
    #endregion

    #region IsDateGreaterThanToday
    public static bool IsDateGreaterThanToday(string Date)
    {
        return IsDateGreaterThanToday(Date, false);
    }
    public static bool IsDateGreaterThanToday(string Date, bool IncludeEqualto)
    {
        if (Date != string.Empty)
        {
            try
            {
                CultureInfo cInfo = new CultureInfo("fr-FR", false);
                DateTime dateval = DateTime.Parse(Date, cInfo.DateTimeFormat);
                DateTime datecur = DateTime.Parse(CheckDBNullForDate(DateTime.Now), cInfo.DateTimeFormat);
                if (IncludeEqualto)
                {
                    if (dateval >= datecur)
                        return true;
                }
                else
                {
                    if (dateval > datecur)
                        return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
        return false;
    }
    #endregion

    #region IsDateLessThanToday
    public static bool IsDateLessThanToday(string Date)
    {
        return IsDateLessThanToday(Date, false);
    }
    public static bool IsDateLessThanToday(string Date, bool IncludeEqualto)
    {
        if (Date != string.Empty)
        {
            try
            {
                CultureInfo cInfo = new CultureInfo("fr-FR", false);
                DateTime dateval = DateTime.Parse(Date, cInfo.DateTimeFormat);
                DateTime datecur = DateTime.Parse(CheckDBNullForDate(DateTime.Now), cInfo.DateTimeFormat);
                if (IncludeEqualto)
                {
                    if (dateval <= datecur)
                        return true;
                }
                else
                {
                    if (dateval < datecur)
                        return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
        return false;
    }
    #endregion

    #region CheckDateGreaterThanToday
    public static string CheckDateGreaterThanToday(string Date, string FieldName)
    {
        if (Date != string.Empty)
        {
            try
            {
                CultureInfo cInfo = new CultureInfo("fr-FR", false);
                DateTime dateval = DateTime.Parse(Date, cInfo.DateTimeFormat);
                DateTime datecur = DateTime.Parse(CheckDBNullForDate(DateTime.Now), cInfo.DateTimeFormat);
                if (dateval > datecur)
                    return FieldName + " should not be greater than today<br />";
                else
                    return "";
            }
            catch
            {
                return "";
            }
        }
        return "";
    }
    #endregion

    #region ValidateDateByFinancialYear
    public static string ValidateDateByFinancialYear(string Date, string FieldName, string AccYrSetID)
    {
        if (Date != string.Empty)
        {
            try
            {
                AccYearInfo accYrInfo = new AccYearInfo(AccYrSetID);
                CultureInfo cInfo = new CultureInfo("fr-FR", false);
                DateTime dateval = DateTime.Parse(Date, cInfo.DateTimeFormat);
                if (dateval < accYrInfo.StartDate || dateval > accYrInfo.EndDate)
                    return FieldName + " should be a valid date within the financial year " + accYrInfo.AccYear + "<br />";
                //else if ()
                //    return FieldName + " should not be greater than " + accYrInfo.EndDate.ToString("dd-MM-yyyy") + "<br />";
                else
                    return "";
            }
            catch
            {
                return "";
            }
        }
        return "";
    }
    #endregion

    #region ValidateDateLessThanGiven
    public static string ValidateDateLessThanGiven(string DateToValidate, string FieldNameToValidate, string DateToCompare, string FieldNameToCompare)
    {
        if (DateToValidate != string.Empty && DateToCompare != string.Empty)
        {
            try
            {
                CultureInfo cInfo = new CultureInfo("fr-FR", false);
                DateTime dateToval = DateTime.Parse(DateToValidate, cInfo.DateTimeFormat);
                DateTime dateToCom = DateTime.Parse(DateToCompare, cInfo.DateTimeFormat);
                if (dateToval < dateToCom)
                    return FieldNameToValidate + " should not be less than " + FieldNameToCompare + "<br />";
                else
                    return "";
            }
            catch
            {
                return "";
            }
        }
        return "";
    }
    #endregion

    #region CheckDate
    public static string CheckDate(ref string Date, string FieldDisplayName, bool AllowEmpty)
    {
        if (Date == string.Empty)
        {
            Date = "null";
            if (AllowEmpty)
                return "";
            else
                return FieldDisplayName + " should not be empty<br />";
        }
        else
        {
            try
            {
                CultureInfo cInfo = new CultureInfo("fr-FR", false); //"dd-MM-yyyy"
                DateTime tDate = DateTime.Parse(Date, cInfo.DateTimeFormat);
                Date = " To_Date('" + CheckDBNullForDate(tDate) + "', 'dd-MM-yyyy')";
                //For MS SQL Server use: " Convert(datetime, '" + Date + "', 103)";
                return "";
            }
            catch (FormatException)
            {
                Date = "null";
                return FieldDisplayName + " is incorrect. (Expected date format: 'dd-MM-yyyy')<br />";
            }
        }
    }
    #endregion

    #region CheckDateWithTime
    public static string CheckDateWithTime(ref string Date, string FieldDisplayName, bool AllowEmpty)
    {
        if (Date == string.Empty)
        {
            Date = "null";
            if (AllowEmpty)
                return "";
            else
                return FieldDisplayName + " should not be empty<br />";
        }
        else
        {
            CultureInfo cInfo = new CultureInfo("fr-FR", false); //"dd-MM-yyyy"
            try
            {
                DateTime.Parse(Date, cInfo.DateTimeFormat);
                Date = " To_Date('" + Date + "', 'dd-MM-yyyy HH:MI:SS AM')";
                //For MS SQL Server use: " Convert(datetime, '" + Date + "', 103)";
                return "";
            }
            catch (FormatException)
            {
                Date = "null";
                return FieldDisplayName + " should be in 'dd-MM-yyyy HH24:MI:SS' format<br />";
            }
        }
    }
    #endregion

    #region CheckDateByComparison
    public static string CheckDateByComparison(string date, string FieldDisplayName, string MinDate, string MinDateFieldName, string MaxDate, string MaxDateFieldName)
    {
        return CheckDateByComparison(date, FieldDisplayName, MinDate, MinDateFieldName, MaxDate, MaxDateFieldName, false);
    }
    public static string CheckDateByComparison(string date, string FieldDisplayName, string MinDate, string MinDateFieldName, string MaxDate, string MaxDateFieldName, bool IncludeEqualTo)
    {
        string errMsg = "";
        if (date != string.Empty)
        {
            try
            {
                CultureInfo cInfo = new CultureInfo("fr-FR", false); //"dd-MM-yyyy"
                DateTime _date = DateTime.Parse(date, cInfo.DateTimeFormat);
                DateTime? min_date = null;
                if (MinDate != string.Empty)
                    min_date = DateTime.Parse(MinDate, cInfo.DateTimeFormat);
                DateTime? max_date = null;
                if (MaxDate != string.Empty)
                    max_date = DateTime.Parse(MaxDate, cInfo.DateTimeFormat);
                if (min_date != null && max_date != null)
                {
                    if (IncludeEqualTo)
                    {
                        if (_date <= min_date || _date >= max_date)
                            errMsg += FieldDisplayName + " should be within " + MinDateFieldName + " and " + MaxDateFieldName + "<br />";
                    }
                    else
                    {
                        if (_date < min_date || _date > max_date)
                            errMsg += FieldDisplayName + " should be between " + MinDateFieldName + " and " + MaxDateFieldName + "<br />";
                    }
                }
                else
                {
                    if (IncludeEqualTo)
                    {
                        if (min_date != null && _date <= min_date)
                            errMsg += FieldDisplayName + " should not be less than or equal to " + MinDateFieldName + "<br />";
                        if (max_date != null && _date >= max_date)
                            errMsg += FieldDisplayName + " should not be greater than or equal to " + MaxDateFieldName + "<br />";
                    }
                    else
                    {
                        if (min_date != null && _date < min_date)
                            errMsg += FieldDisplayName + " should not be less than " + MinDateFieldName + "<br />";
                        if (max_date != null && _date > max_date)
                            errMsg += FieldDisplayName + " should not be greater than " + MaxDateFieldName + "<br />";
                    }
                }
            }
            catch (FormatException)
            {
                return errMsg;
            }
        }
        return errMsg;
    }
    #endregion

    #region CheckNumberForZero
    public static string CheckNumberForZero(string Number, string FieldDisplayName, int Roundoff)
    {
        if (Number != string.Empty)
        {
            try
            {
                decimal tc = decimal.Parse(Number);
                if (Math.Round(tc, 2) == 0)
                    return FieldDisplayName + " should not be zero<br />";
                else
                    return "";
            }
            catch (FormatException)
            {
                return "";
            }
        }
        return "";
    }
    #endregion

    #region CheckNumber
    public static string CheckNumber(ref string Number, string FieldDisplayName, bool AllowEmpty)
    {
        if (Number == string.Empty)
        {
            Number = "null";
            if (AllowEmpty)
                return "";
            else
                return FieldDisplayName + " should not be empty<br />";
        }
        else
        {
            try
            {
                decimal.Parse(Number);
                return "";
            }
            catch (FormatException)
            {
                Number = "null";
                return FieldDisplayName + " is not a valid number<br />";
            }
        }
    }
    #endregion

    #region CheckNumberForPercent
    public static string CheckNumberForPercent(string Number, string FieldDisplayName)
    {
        if (Number != string.Empty)
        {
            try
            {
                decimal num = decimal.Parse(Number);
                if (num < 0 || num > 100)
                    return FieldDisplayName + " should be within 0 and 100<br />";
                else
                    return "";
            }
            catch (FormatException)
            {
                return "";
            }
        }
        return "";
    }
    #endregion

    #region CheckNumberByComparison
    public static string CheckNumberByComparison(string Number, string FieldDisplayName, string MinNumber, string MinNumberFieldName, string MaxNumber, string MaxNumberFieldName)
    {
        return CheckNumberByComparison(Number, FieldDisplayName, MinNumber, MinNumberFieldName, MaxNumber, MaxNumberFieldName, false);
    }
    public static string CheckNumberByComparison(string Number, string FieldDisplayName, string MinNumber, string MinNumberFieldName, string MaxNumber, string MaxNumberFieldName, bool IncludeEqualTo)
    {
        string errMsg = "";
        if (Number != string.Empty)
        {
            try
            {
                decimal num = decimal.Parse(Number);
                decimal? minNum = null;
                if (MinNumber != string.Empty)
                    minNum = decimal.Parse(MinNumber);
                decimal? maxNum = null;
                if (MaxNumber != string.Empty)
                    maxNum = decimal.Parse(MaxNumber);
                if (minNum != null && maxNum != null)
                {
                    if (IncludeEqualTo)
                    {
                        if (num <= minNum || num >= maxNum)
                            errMsg += FieldDisplayName + " should be within " + MinNumberFieldName + " and " + MaxNumberFieldName + "<br />";
                    }
                    else
                    {
                        if (num < minNum || num > maxNum)
                            errMsg += FieldDisplayName + " should be within " + MinNumberFieldName + " and " + MaxNumberFieldName + "<br />";
                    }
                }
                else
                {
                    if (IncludeEqualTo)
                    {
                        if (minNum != null && num <= minNum)
                            errMsg += FieldDisplayName + " should not be less than or equal to " + MinNumberFieldName + "<br />";
                        if (maxNum != null && num >= maxNum)
                            errMsg += FieldDisplayName + " should not be greater than or equal to " + MaxNumberFieldName + "<br />";
                    }
                    else
                    {
                        if (minNum != null && num < minNum)
                            errMsg += FieldDisplayName + " should not be less than " + MinNumberFieldName + "<br />";
                        if (maxNum != null && num > maxNum)
                            errMsg += FieldDisplayName + " should not be greater than " + MaxNumberFieldName + "<br />";
                    }
                }
            }
            catch (FormatException)
            {
                return errMsg;
            }
        }
        return errMsg;
    }
    #endregion

    #region CheckFirstCharacterForSepecial
    public static string CheckFirstCharacterForSepecial(string Value, string FieldDisplayName)
    {
        if (Value != string.Empty)
        {
            if (!char.IsLetterOrDigit(Value, 0))
            {
                return "First character should not be a special character<br />";
            }
        }
        return "";
    }
    #endregion

    #region CheckEmail
    public static string CheckEmail(ref string Value, string FieldDisplayName, bool AllowEmpty)
    {
        if (Value != "")
        {
            if (Value.IndexOf("@") > 0)
            {
                string[] domainName = Value.Substring(Value.IndexOf("@")).Split((new char[] { '.' }));
                if (domainName.Length > 3)
                    return "Invalid " + FieldDisplayName + ". Enter a valid domain name.<br />";
                if (domainName[domainName.Length-1].Length <= 1)
                    return "Invalid " + FieldDisplayName + ". Enter a valid top-level domain name.<br />";
            }
        }
        return CheckStringForEmpty(ref Value, FieldDisplayName, AllowEmpty);
    }
    #endregion

    #region CheckStringForEmpty
    public static string CheckStringForEmpty(ref string Value, string FieldDisplayName, bool AllowEmpty)
    {
        if (Value == string.Empty)
        {
            Value = "null";
            if (AllowEmpty)
                return "";
            else
                return FieldDisplayName + " should not be empty<br />";
        }
        else
        {
            Value = "'" + CheckSingleQuote(Value) + "'";
            return "";
        }
    }
    #endregion

    #region CheckDuplicate
    public static string CheckDuplicate(string Value, string FieldDisplayName, string TableName, string ColumnName, string NotForID, string NotForIDColumnName)
    {
        return CheckDuplicate(Value, FieldDisplayName, TableName, ColumnName, NotForID, NotForIDColumnName, "");
    }

    public static string CheckDuplicate(string Value, string FieldDisplayName, string TableName, string ColumnName, string NotForID, string NotForIDColumnName, string OtherConditions)
    {
        string strSQL = "";
        if (Value != "")
            strSQL = "Select * from " + TableName + " where Upper(" + ColumnName + ") = Upper('" + CheckSingleQuote(Value) + "') " + ((NotForID == "") ? "" : " and " + NotForIDColumnName + " <> " + NotForID) + OtherConditions;
        else
            strSQL = "Select * from " + TableName + " where Upper(" + ColumnName + ") is null " + ((NotForID == "") ? "" : " and " + NotForIDColumnName + " <> " + NotForID) + OtherConditions;

        DataTable dt = DBHelper.GetDataTable(strSQL);
        if (dt.Rows.Count > 0)
            return FieldDisplayName + " already exists and cannot be saved<br />";
        else
            return "";
    }
    #endregion

   

    #region ApplyFilteredTextBoxExtender
    public static void ApplyFilteredTextBoxExtender(TextBox txt, AjaxControlToolkit.FilterModes FilterMode, AjaxControlToolkit.FilterTypes FilterType, string Chars)
    {
        AjaxControlToolkit.FilteredTextBoxExtender filterCrtl = new AjaxControlToolkit.FilteredTextBoxExtender();
        filterCrtl.TargetControlID = txt.ID;
        filterCrtl.FilterMode = FilterMode;
        filterCrtl.FilterType = FilterType;
        if (FilterMode == AjaxControlToolkit.FilterModes.InvalidChars)
            filterCrtl.InvalidChars = Chars;
        if (FilterMode == AjaxControlToolkit.FilterModes.ValidChars)
            filterCrtl.ValidChars = Chars;
        Control pCtrl = txt.Parent;
        pCtrl.Controls.Add(filterCrtl);
    }
    #endregion

    #region ApplyFilterByInvalidChars
    public static void ApplyFilterByInvalidChars(GridViewRow gvRow, string txtBoxID, string Chars, int MaxLength)
    {
        TextBox txt = (TextBox)gvRow.FindControl(txtBoxID);
        ApplyFilterByInvalidChars(txt, Chars, MaxLength);
    }
    public static void ApplyFilterByInvalidChars(GridViewRow gvRow, string txtBoxID, string Chars)
    {
        TextBox txt = (TextBox)gvRow.FindControl(txtBoxID);
        ApplyFilterByInvalidChars(txt, Chars);
    }
    public static void ApplyFilterByInvalidChars(TextBox txt, string Chars)
    {
        ApplyFilteredTextBoxExtender(txt, AjaxControlToolkit.FilterModes.InvalidChars, AjaxControlToolkit.FilterTypes.Custom, Chars);
    }
    public static void ApplyFilterByInvalidChars(TextBox txt, string Chars, int MaxLength)
    {
        txt.MaxLength = MaxLength;
        ApplyFilterByInvalidChars(txt, Chars);
    }
    #endregion

    #region ApplyFilterByValidChars
    public static void ApplyFilterByValidChars(GridViewRow gvRow, string txtBoxID, string Chars, int MaxLength)
    {
        TextBox txt = (TextBox)gvRow.FindControl(txtBoxID);
        ApplyFilterByValidChars(txt, Chars, MaxLength);
    }
    public static void ApplyFilterByValidChars(GridViewRow gvRow, string txtBoxID, string Chars)
    {
        TextBox txt = (TextBox)gvRow.FindControl(txtBoxID);
        ApplyFilterByValidChars(txt, Chars);
    }
    public static void ApplyFilterByValidChars(TextBox txt, string Chars)
    {
        ApplyFilteredTextBoxExtender(txt, AjaxControlToolkit.FilterModes.ValidChars, AjaxControlToolkit.FilterTypes.Custom, Chars);
    }
    public static void ApplyFilterByValidChars(TextBox txt, string Chars, int Maxlength)
    {
        txt.MaxLength = Maxlength;
        ApplyFilterByValidChars(txt, Chars);
    }
    #endregion

    #region ApplyFilterForItemCode
    public static void ApplyFilterForItemCode(TextBox txt)
    {
        txt.MaxLength = 15;
        string Chars = @"|?~`!@#$%^&*()_-+={}[]:;,.'<>\""";
        ApplyFilteredTextBoxExtender(txt, AjaxControlToolkit.FilterModes.InvalidChars, AjaxControlToolkit.FilterTypes.Custom, Chars);
    }
    #endregion

    #region ApplyFilterForItemName
    public static void ApplyFilterForItemName(TextBox txt)
    {
        txt.MaxLength = 150;
        string Chars = @"|?~`!@#$^*_={}[];'<>\""";
        ApplyFilteredTextBoxExtender(txt, AjaxControlToolkit.FilterModes.InvalidChars, AjaxControlToolkit.FilterTypes.Custom, Chars);
    }
    #endregion

    #region ApplyFilterForFacilityCode
    public static void ApplyFilterForFacilityCode(TextBox txt)
    {
        txt.MaxLength = 10;
        string Chars = @"|?~`!@#$%^&*()_-+={}[]:;,.<>/'\""";
        ApplyFilteredTextBoxExtender(txt, AjaxControlToolkit.FilterModes.InvalidChars, AjaxControlToolkit.FilterTypes.Custom, Chars);
    }
    #endregion

    #region ApplyFilterForFacilityName
    public static void ApplyFilterForFacilityName(TextBox txt)
    {
        txt.MaxLength = 100;
        string Chars = @"|?~`!@#$%^&*()_-+={}[]:;,.<>/'\""";
        ApplyFilteredTextBoxExtender(txt, AjaxControlToolkit.FilterModes.InvalidChars, AjaxControlToolkit.FilterTypes.Custom, Chars);
    }
    #endregion

    #region ApplyFilterForSupplierCode
    public static void ApplyFilterForSupplierCode(TextBox txt)
    {
        txt.MaxLength = 10;
        string Chars = @"|?~`!@#$%^&*()_-+={}[]:;,.<>/'\""";
        ApplyFilteredTextBoxExtender(txt, AjaxControlToolkit.FilterModes.InvalidChars, AjaxControlToolkit.FilterTypes.Custom, Chars);
    }
    #endregion

    #region ApplyFilterForSupplierName
    public static void ApplyFilterForSupplierName(TextBox txt)
    {
        txt.MaxLength = 100;
        string Chars = @"|?~`!@#$%^&*_-+={}[]:;,<>""";
        ApplyFilteredTextBoxExtender(txt, AjaxControlToolkit.FilterModes.InvalidChars, AjaxControlToolkit.FilterTypes.Custom, Chars);
    }
    #endregion

    #region ApplyFilterForEmail
    public static void ApplyFilterForEmail(TextBox txt)
    {
        txt.MaxLength = 75;
        RegularExpressionValidator filterCrtl = new RegularExpressionValidator();
        filterCrtl.ControlToValidate = txt.ID;
        filterCrtl.ErrorMessage = "Invaild Email.";
        filterCrtl.Text = "Invaild Email.";
        filterCrtl.SetFocusOnError = true;
        filterCrtl.ValidationExpression = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";
        Control pCtrl = txt.Parent;
        pCtrl.Controls.Add(filterCrtl);
        string Chars = @"|?~`!#$%^&*()+={}[]:;,'<>/\""";
        ApplyFilteredTextBoxExtender(txt, AjaxControlToolkit.FilterModes.InvalidChars, AjaxControlToolkit.FilterTypes.Custom, Chars);
    }
    #endregion

    #region ApplyFilterForAddress
    public static void ApplyFilterForAddress(TextBox txt)
    {
        txt.MaxLength = 100;
        string Chars = @"?~`!@$%^*_+={}[]:;.<>""";
        //@"|?~`!@#$%^&*()_-+={}[]:;,.<>/'\""";
        ApplyFilteredTextBoxExtender(txt, AjaxControlToolkit.FilterModes.InvalidChars, AjaxControlToolkit.FilterTypes.Custom, Chars);
    }
    #endregion

    #region ApplyFilterForBatchNo
    public static void ApplyFilterForBatchNo(GridViewRow gvRow, string txtBoxID)
    {
        TextBox txt = (TextBox)gvRow.FindControl(txtBoxID);
        ApplyFilterForBatchNo(txt);
    }

    public static void ApplyFilterForBatchNo(TextBox txt)
    {
        txt.MaxLength = 40;
        string Chars = @" |?~`!@#$%^&*()_+={}[]:;,.'<>\""";
        ApplyFilteredTextBoxExtender(txt, AjaxControlToolkit.FilterModes.InvalidChars, AjaxControlToolkit.FilterTypes.Custom, Chars);
    }
    #endregion

    #region Helper GroupHeader
    public static void helper_GroupHeader(string groupName, object[] values, GridViewRow row)
    {
        if (groupName == "SourceName")
            row.Cells[0].Text = "Source: " + values[0].ToString();
        if (groupName == "SchemeName")
            row.Cells[0].Text = "Scheme: " + values[0].ToString();
        if (groupName == "SourceName+SchemeName")
            row.Cells[0].Text = "<table width='100%'><tr><td align='Left' colspan='" + Math.Ceiling(row.Cells.Count / 2.0) + "'>" + "Source: " + values[0].ToString() + "</td><td align='Right' colspan='" + Math.Floor(row.Cells.Count / 2.0) + "'>" + "Scheme: " + values[1].ToString() + "</td></tr></table>";
    }
    #endregion

    #region Items Related Methods
    #region GetItemCodes
    public static string[] GetItemCodes(string prefixText, int count, string contextKey)
    {
        if (contextKey == null) contextKey = "";
        return GetDBValues(prefixText, count, contextKey, "masItems", "ItemCode");
    }
    #endregion

    #region GetItemNames
    public static string[] GetItemNames(string prefixText, int count, string contextKey)
    {
        if (contextKey == null) contextKey = "";
        return GetDBValues(prefixText, count, contextKey, "masItems", "ItemName");
    }
    #endregion
    #endregion

    #region Combined Item Related Methods
    #region GetCombinedItemCodes
    public static string[] GetCombinedItemCodes(string PrefixText, int Count, string ContextKey)
    {
        string Query =  " " +
                        " WITH " +
                        " CombinedItems  AS " +
                        " ( " +
                        "     SELECT CombinedItemID,ItemID, LPItemID,ItemCode,ItemName,ActiveFlag FROM V_CombinedItems " +
                        " ), " +
                        " CombinedItemList " +
                        " AS " +
                        " ( " +
                        " SELECT " +
                        " CombinedItemID, ItemCode, ROW_NUMBER() OVER (ORDER BY ItemCode) ListSize " +
                        " FROM CombinedItems " +
                        " WHERE Upper(ItemCode) LIKE Upper('@PrefixText%') " +
                        " AND ActiveFlag='Y' " +
                        " @ContextKey " +
                        " ) " +
                        " Select ItemCode from CombinedItemList Where ListSize <= @ListSize ";

        Query = Query.Replace("@ContextKey", ContextKey).Replace("@PrefixText", PrefixText).Replace("@ListSize", Count.ToString());

        return GetDBValues(Query);
    }
    #endregion

    #region GetCombinedItemNames
    public static string[] GetCombinedItemNames(string PrefixText, int Count, string ContextKey)
    {
        string Query =  " " +
                        " WITH " +
                        " CombinedItems  AS " +
                        " ( " +
                        "     SELECT CombinedItemID,ItemID, LPItemID,ItemCode,ItemName,ActiveFlag FROM V_CombinedItems " +
                        " ), " +
                        " CombinedItemList " +
                        " AS " +
                        " ( " +
                        " SELECT " +
                        " CombinedItemID, ItemName, ROW_NUMBER() OVER (ORDER BY ItemName) ListSize " +
                        " FROM CombinedItems " +
                        " WHERE Upper(ItemName) LIKE Upper('@PrefixText%') " +
                        " AND ActiveFlag='Y' " +
                        " @ContextKey " +
                        " ) " +
                        " Select ItemName from CombinedItemList Where ListSize <= @ListSize ";

        Query = Query.Replace("@ContextKey", ContextKey).Replace("@PrefixText", PrefixText).Replace("@ListSize", Count.ToString());

        return GetDBValues(Query);
    }
    #endregion
    #endregion

    #region Local Items Related Methods
    #region GetAllLocalItemCodes
    public static string[] GetAllLocalItemCodes(string prefixText, int count, string contextKey)
    {
        if (contextKey == null) contextKey = "";
        return GetDBValuesLP(prefixText, count, contextKey, "LPMasItems", "ItemCode");
    }
    #endregion

    #region GetAllLocalItemNames
    public static string[] GetAllLocalItemNames(string prefixText, int count, string contextKey)
    {
        if (contextKey == null) contextKey = "";
        return GetDBValues(prefixText, count, contextKey, "LPMasItems", "ItemName || ' ' || Strength");
    }
    #endregion
    #endregion

    #region Fill Country
    public static void FillCountry(DropDownList ddl, bool AddAllCountries, bool SelectFirst)
    {
        ddl.Items.Clear();
        string strSQL = "Select CountryID, CountryName from masCountries Order By CountryName";
        DataTable dt = DBHelper.GetDataTable(strSQL);
        ddl.DataTextField = "CountryName";
        ddl.DataValueField = "CountryID";
        ddl.DataSource = dt;
        ddl.DataBind();
        if (AddAllCountries)
            ddl.Items.Insert(0, new ListItem("All Countries", "0"));
        if (SelectFirst && ddl.Items.Count > 0)
            ddl.SelectedIndex = 0;
    }
    #endregion

    #region GetAllSupplierCodes
    public static string[] GetAllSupplierCodes(string prefixText, int count, string contextKey)
    {
        if (contextKey == null) contextKey = "";
        return GetDBValues(prefixText, count, contextKey, "masSuppliers", "SupplierCode");
    }
    #endregion

    #region GetAllLPSupplierCodes
    public static string[] GetAllLPSupplierCodes(string prefixText, int count, string contextKey)
    {
        if (contextKey == null) contextKey = "";
        return GetDBValues(prefixText, count, contextKey, "LPmasSuppliers", "SupplierCode");
    }
    #endregion

    #region GetAllSupplierNames
    public static string[] GetAllSupplierNames(string prefixText, int count, string contextKey)
    {
        if (contextKey == null) contextKey = "";
        return GetDBValues(prefixText, count, contextKey, "masSuppliers", "SupplierName");
    }
    #endregion

    #region GetAllLPSupplierNames
    public static string[] GetAllLPSupplierNames(string prefixText, int count, string contextKey)
    {
        if (contextKey == null) contextKey = "";
        return GetDBValues(prefixText, count, contextKey, "LPmasSuppliers", "SupplierName");
    }
    #endregion

    #region GetAllWarehouseCodes
    public static string[] GetAllWarehouseCodes(string prefixText, int count, string contextKey)
    {
        if (contextKey == null) contextKey = "";
        return GetDBValues(prefixText, count, contextKey, "masWarehouses", "WarehouseCode");
    }
    #endregion

    #region GetAllWarehouseCodes
    public static string[] GetAllWarehouseNames(string prefixText, int count, string contextKey)
    {
        if (contextKey == null) contextKey = "";
        return GetDBValues(prefixText, count, contextKey, "masWarehouses", "WarehouseName");
    }
    #endregion

    #region Facility Related Methods
    #region FillDistricts
    public static void FillDistricts(DropDownList ddl, string StateID, bool AddAllDistricts, bool SelectFirst)
    {
        FillDistricts(ddl, StateID, AddAllDistricts, SelectFirst, "");
    }
    #endregion

    #region FillDistricts
    public static void FillDistricts(DropDownList ddl, string StateID, bool AddAllDistricts, bool SelectFirst, string WhereCondition)
    {
        ddl.Items.Clear();
        if (StateID.Trim() != "")
        {
            string strSQL = "Select b.DistrictID, b.DistrictName from masDistricts b Where StateID = " + StateID.Trim() + " " + WhereCondition + " Order by b.DistrictName";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            ddl.DataTextField = "DistrictName";
            ddl.DataValueField = "DistrictID";
            ddl.DataSource = dt;
            ddl.DataBind();
        }
        if (AddAllDistricts)
            ddl.Items.Insert(0, new ListItem("All Districts", "0"));
        if (SelectFirst && ddl.Items.Count > 0)
            ddl.SelectedIndex = 0;
    }
    #endregion
    

    #region FillAllDistricts
    public static void FillAllDistricts(DropDownList ddl, string StateID, bool AddAllDistricts, bool SelectFirst)
    {
        FillAllDistricts(ddl, StateID, AddAllDistricts, SelectFirst, "");
    }
    #endregion

    #region FillAllDistricts
    public static void FillAllDistricts(DropDownList ddl, string StateID, bool AddAllDistricts, bool SelectFirst, string WhereCondition)
    {
        ddl.Items.Clear();
        if (StateID.Trim() != "")
        {
            string strSQL = "Select b.DistrictID, b.DistrictName from masDistricts b Where StateID = " + StateID.Trim() + " " + WhereCondition + " Order by b.DistrictName";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            ddl.DataTextField = "DistrictName";
            ddl.DataValueField = "DistrictID";
            ddl.DataSource = dt;
            ddl.DataBind();
        }
        if (AddAllDistricts)
            ddl.Items.Insert(0, new ListItem("All Districts", "0"));
        if (SelectFirst && ddl.Items.Count > 0)
            ddl.SelectedIndex = 1;
    }
    #endregion

    #region FillLocation
    public static void FillLocation(DropDownList ddl, string StateID, string DistrictID, bool AddAllLocations, bool SelectFirst)
    {
        ddl.Items.Clear();
        if (StateID != "" && DistrictID != "")
        {
            string strSQL = "Select LocationID, LocationName from masLocations where StateID = " + StateID.ToString() + " and DistrictID = " + DistrictID.ToString() + " Order By LocationName";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            ddl.DataTextField = "LocationName";
            ddl.DataValueField = "LocationID";
            ddl.DataSource = dt;
            ddl.DataBind();
        }
        if (AddAllLocations)
            ddl.Items.Insert(0, new ListItem("All Locations", "0"));
        if (SelectFirst && ddl.Items.Count > 0)
            ddl.SelectedIndex = 0;
    }
    #endregion

    #region FillFacilityType
    public static void FillFacilityType(DropDownList ddl, bool AddAllFacilityTypes, bool SelectFirst)
    {
        ddl.Items.Clear();
        string strSQL = "Select FacilityTypeID, FacilityTypeDesc from masFacilityTypes Order By FacilityTypeDesc";
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

    #region FillBudgetType
    public static void FillBudgetType(DropDownList ddl, bool AddAllBudgetTypes, bool SelectFirst)
    {
        ddl.Items.Clear();
        string strSQL = "Select BudgetTypeID, BudgetTypeName from masFacilityBudgetTypes Order By BudgetTypeName";
        DataTable dt = DBHelper.GetDataTable(strSQL);
        ddl.DataTextField = "BudgetTypeName";
        ddl.DataValueField = "BudgetTypeID";
        ddl.DataSource = dt;
        ddl.DataBind();
        if (AddAllBudgetTypes)
            ddl.Items.Insert(0, new ListItem("All Budget Types", "0"));
        if (SelectFirst && ddl.Items.Count > 0)
            ddl.SelectedIndex = 0;
    }
    #endregion

    #region FillFacilitiesByState_District
    public static void FillFacilitiesByState_District(DropDownList ddl, string StateID, string DistrictID, bool AddAllFacilities, bool SelectFirst)
    {
        ddl.Items.Clear();
        if (StateID != "" && DistrictID != "")
        {
            string strSQL = "Select FacilityID, FacilityName from masFacilities where StateID = " + StateID + " and DistrictID = " + DistrictID + " Order By FacilityName";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            ddl.DataTextField = "FacilityName";
            ddl.DataValueField = "FacilityID";
            ddl.DataSource = dt;
            ddl.DataBind();
        }
        if (AddAllFacilities)
            ddl.Items.Insert(0, new ListItem("All Facilities", "0"));
        if (SelectFirst && ddl.Items.Count > 0)
            ddl.SelectedIndex = 0;
    }
    #endregion

    #region GetAllActiveFacilityCodes
    public static string[] GetAllActiveFacilityCodes(string prefixText, int count, string contextKey)
    {
        if (contextKey == null) contextKey = "";
        contextKey += " and IsActive = 1";
        return GetDBValues(prefixText, count, contextKey, "masFacilities", "FacilityCode");
    }
    #endregion

    #region GetAllActiveFacilityNames
    public static string[] GetAllActiveFacilityNames(string prefixText, int count, string contextKey)
    {
        if (contextKey == null) contextKey = "";
        contextKey += " and IsActive = 1";
        return GetDBValues(prefixText, count, contextKey, "masFacilities", "FacilityName");
    }
    #endregion
    #endregion

    #region GetSettings
    public static string GetSettings(string Key)
    {
        string strSQL = "Select * from masSettings where lower(Keys) = '" + Key.ToLower() + "'";
        DataTable dt = DBHelper.GetDataTable(strSQL);
        if (dt.Rows.Count > 0)
        {
            if (dt.Rows[0]["Value"] != DBNull.Value)
                return dt.Rows[0]["Value"].ToString();
            else
                return "";
        }
        else
            return "";
    }
    #endregion

    #region SaveSettings
    public static void SaveSettings(string keys, string value)
    {
        string strSQL = "Select * from masSettings where Keys = '" + keys + "'";
        DataTable dt = DBHelper.GetDataTable(strSQL);
        if (dt.Rows.Count > 0)
        {
            strSQL = "Update masSettings set Value = '" + value + "' where Keys = '" + keys + "'";
            DBHelper.GetDataTable(strSQL);
        }
        else
        {
            strSQL = "Insert into masSettings (Keys, Value) Values ('" + keys + "', '" + value + "')";
            DBHelper.GetDataTable(strSQL);
        }
    }
    #endregion

    #region FillPayModes
    public static void FillPayModes(DropDownList ddl, bool AddAllPayModes, bool SelectFirst)
    {
        ddl.Items.Clear();
        string strSQL = "Select PayModeID, PayModeDesc from masPayModes Order By PayModeDesc";
        DataTable dt = DBHelper.GetDataTable(strSQL);
        ddl.DataTextField = "PayModeDesc";
        ddl.DataValueField = "PayModeID";
        ddl.DataSource = dt;
        ddl.DataBind();
        if (AddAllPayModes)
            ddl.Items.Insert(0, new ListItem("All Pay Modes", "0"));
        if (SelectFirst && ddl.Items.Count > 0)
            ddl.SelectedIndex = 0;
    }
    #endregion

    #region Number to Word
    public static string ToWord(double cost)
    {
        string[] unit = new string[19];
        string[] tens = new string[9];
        string outstring = "";
        string crore, lacs, thous, hunds, ten_s, paise, amt;
        unit[0] = "One "; unit[9] = "Ten ";
        unit[1] = "Two "; unit[10] = "Eleven ";
        unit[2] = "Three "; unit[11] = "Twelve ";
        unit[3] = "Four "; unit[12] = "Thirteen ";
        unit[4] = "Five "; unit[13] = "Fourteen ";
        unit[5] = "Six "; unit[14] = "Fifteen ";
        unit[6] = "Seven "; unit[15] = "Sixteen ";
        unit[7] = "Eight "; unit[16] = "Seventeen ";
        unit[8] = "Nine "; unit[17] = "Eighteen ";
        unit[18] = "Nineteen ";

        tens[0] = "    "; tens[5] = "Sixty ";
        tens[1] = "Twenty "; tens[6] = "Seventy ";
        tens[2] = "Thirty "; tens[7] = "Eighty ";
        tens[3] = "Forty "; tens[8] = "Ninety ";
        tens[4] = "Fifty ";

        crore = ""; lacs = ""; thous = ""; hunds = ""; ten_s = ""; paise = "";
        amt = cost.ToString("000000000.00");

        crore = amt.Substring(0, 2);
        lacs = amt.Substring(2, 2);
        thous = amt.Substring(4, 2);
        hunds = amt.Substring(6, 1);
        ten_s = amt.Substring(7, 2);
        paise = amt.Substring(10, 2);

        if (crore != "")
        {
            if (Convert.ToInt32(crore) != 0)
            {
                if (Convert.ToInt32(crore.Trim()) < 2)
                {
                    outstring += unit[Convert.ToInt16(crore.Substring(0, 1))] + "Crore ";
                }
                else
                {
                    if (Convert.ToInt32(crore.Substring(0, 1)) == 0)
                    {
                        outstring += unit[Convert.ToInt32(crore.Substring(1, 1)) - 1].ToString();
                    }
                    else if (Convert.ToInt32(crore.Substring(0, 1)) > 0 && Convert.ToInt32(crore.Substring(0, 2)) < 20)
                    {
                        outstring += unit[Convert.ToInt32(crore.Substring(0, 2)) - 1].ToString();
                    }
                    else
                    {
                        outstring += tens[Convert.ToInt32(crore.Substring(0, 1)) - 1].ToString();
                        if (Convert.ToInt32(crore.Substring(1, 1)) > 0)
                            outstring += unit[Convert.ToInt32(crore.Substring(1, 1)) - 1].ToString();
                    }

                    outstring = outstring.Trim() + " Crores ";
                }
            }
        }

        if (lacs != "")
        {
            if (Convert.ToInt32(lacs) != 0)
            {
                if (Convert.ToInt32(lacs.Trim()) < 2)
                {
                    //outstring +=  unit[Convert.ToInt16(lacs.Substring(2,1))] + "lakh ";
                    outstring += unit[Convert.ToInt16(lacs.Substring(0, 1))] + "lakh ";
                }
                else
                {
                    if (Convert.ToInt32(lacs.Substring(0, 1)) == 0)
                    {
                        outstring += unit[Convert.ToInt32(lacs.Substring(1, 1)) - 1].ToString();
                    }
                    else if (Convert.ToInt32(lacs.Substring(0, 1)) > 0 && Convert.ToInt32(lacs.Substring(0, 2)) < 20)
                    {
                        outstring += unit[Convert.ToInt32(lacs.Substring(0, 2)) - 1].ToString();
                    }
                    else
                    {
                        outstring += tens[Convert.ToInt32(lacs.Substring(0, 1)) - 1].ToString();
                        if (Convert.ToInt32(lacs.Substring(1, 1)) > 0)
                            outstring += unit[Convert.ToInt32(lacs.Substring(1, 1)) - 1].ToString();
                    }

                    outstring = outstring.Trim() + " lakhs ";
                }
            }
        }

        if (thous != "")
        {
            if (Convert.ToInt32(thous) != 0)
            {
                if (Convert.ToInt32(thous.Trim()) < 2)
                {
                    //outstring +=  unit[Convert.ToInt16(thous.Substring(2,1))] + " Thousand ";
                    outstring += unit[Convert.ToInt16(thous.Substring(0, 1))] + " Thousand ";
                }
                else
                {
                    if (Convert.ToInt32(thous.Substring(0, 1)) == 0)
                    {
                        outstring += unit[Convert.ToInt32(thous.Substring(1, 1)) - 1].ToString();
                    }
                    else if (Convert.ToInt32(thous.Substring(0, 1)) > 0 && Convert.ToInt32(thous.Substring(0, 2)) < 20)
                    {
                        outstring += unit[Convert.ToInt32(thous.Substring(0, 2)) - 1].ToString();
                    }
                    else
                    {
                        outstring += tens[Convert.ToInt32(thous.Substring(0, 1)) - 1].ToString();
                        if (Convert.ToInt32(thous.Substring(1, 1)) > 0)
                            outstring += unit[Convert.ToInt32(thous.Substring(1, 1)) - 1].ToString();
                    }

                    outstring = outstring.Trim() + " Thousand ";
                }
            }
        }

        if (hunds != "")
        {
            if (Convert.ToInt32(hunds) != 0)
            {
                outstring += unit[Convert.ToInt32(hunds.Substring(0, 1)) - 1].ToString();
                outstring = outstring.Trim() + " Hundred and ";
            }
        }

        if (ten_s != "")
        {
            if (Convert.ToInt32(ten_s) != 0)
            {
                if (Convert.ToInt32(ten_s.Trim()) < 1)
                {
                    //outstring +=  unit[Convert.ToInt16(ten_s.Substring(2,1))] + "Rupee ";
                    outstring += unit[Convert.ToInt16(ten_s.Substring(0, 1))] + "Rupee ";
                }
                else
                {
                    if (Convert.ToInt32(ten_s.Substring(0, 1)) == 0)
                    {
                        outstring += unit[Convert.ToInt32(ten_s.Substring(1, 1)) - 1].ToString();
                    }
                    else if (Convert.ToInt32(ten_s.Substring(0, 2)) > 0 && Convert.ToInt32(ten_s.Substring(0, 2)) < 20)
                    {
                        outstring += unit[Convert.ToInt32(ten_s.Substring(0, 2)) - 1].ToString();
                    }
                    else
                    {
                        outstring += tens[Convert.ToInt32(ten_s.Substring(0, 1)) - 1].ToString();
                        if (Convert.ToInt32(ten_s.Substring(1, 1)) > 0)
                            outstring += unit[Convert.ToInt32(ten_s.Substring(1, 1)) - 1].ToString();
                    }

                    outstring = outstring.Trim() + " Rupees ";
                }
            }
        }

        if (paise != "")
        {
            if (Convert.ToInt32(paise) != 0)
            {
                if (Convert.ToInt32(paise.Trim()) < 2)
                {
                    outstring += unit[Convert.ToInt16(paise.Substring(0, 1))] + "Paise ";
                }
                else
                {
                    if (Convert.ToInt32(paise.Substring(0, 1)) == 0)
                    {
                        outstring += unit[Convert.ToInt32(paise.Substring(1, 1)) - 1].ToString();
                    }
                    else
                    {
                        if (Convert.ToInt32(paise.Substring(0, 2)) > 0 && Convert.ToInt32(paise.Substring(0, 2)) < 20)
                        {
                            outstring += unit[Convert.ToInt32(paise.Substring(0, 2)) - 1].ToString();
                        }
                        else
                        {
                            outstring += tens[Convert.ToInt32(paise.Substring(0, 1)) - 1].ToString();
                            if (Convert.ToInt32(paise.Substring(1, 1)) > 0)
                                outstring += unit[Convert.ToInt32(paise.Substring(1, 1)) - 1].ToString();
                        }
                    }

                    outstring = outstring.Trim() + " Paise ";
                }
            }
        }
        outstring += " only";
        return outstring;
    }
    # endregion 

    #region CheckUserPageOperations (ByControl)
    public static void CheckUserPageOperations(string MemberID, string PageURL, Control Ctrl)
    {
        if (MemberID != "" && PageURL != "" && Ctrl != null)
        {
            string strSQL = "Select b.Operations from usrUsers a Inner join usrScreenOpsByRole b on(b.RoleID = a.RoleID) Inner join usrScreens c on (c.ScreenID = b.ScreenID) Where a.UserID = " + MemberID + " and upper(b.Operations) = 'V' and '~/' || c.ScreenURL = '" + PageURL + "'";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            if (dt.Rows.Count > 0)
                Ctrl.Visible = false;
        }
    }
    #endregion

    #region CheckUserPageOperations (ByArrayList)
    public static void CheckUserPageOperations(string MemberID, string PageURL, ArrayList Ctrls)
    {
        if (MemberID != "" && PageURL != "" && Ctrls.Count > 0)
        {
            string strSQL = "Select b.Operations from usrUsers a Inner join usrScreenOpsByRole b on(b.RoleID = a.RoleID) Inner join usrScreens c on (c.ScreenID = b.ScreenID) Where a.UserID = " + MemberID + " and upper(b.Operations) = 'V' and '~/' || c.ScreenURL = '" + PageURL + "'";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            if (dt.Rows.Count > 0)
            {
                foreach (object ctl in Ctrls)
                {
                    ((Control)ctl).Visible = false;
                }
            }
        }
    }
    #endregion

    #region CheckUserApprovalRights (ByArrayList)
    public static void CheckUserApprovalRights(string MemberID, string PageURL, ArrayList Ctrls)
    {
        if (MemberID != "" && PageURL != "" && Ctrls.Count > 0)
        {
            string strSQL = "Select b.Operations" +
                " from usrUsers a" +
                " Inner join usrScreenOpsByRole b on(b.RoleID = a.RoleID)" +
                " Inner join usrScreens c on (c.ScreenID = b.ScreenID)" +
                " Where a.UserID=" + MemberID + " and instr(upper(b.Operations),'A') > 0 and '~/' || c.ScreenURL='" + PageURL + "'";
            DataTable dt = DBHelper.GetDataTable(strSQL);

            if (dt.Rows.Count > 0)
            {
                foreach (object ctl in Ctrls)
                    ((Control)ctl).Visible = true;
            }
            else
            {
                foreach (object ctl in Ctrls)
                    ((Control)ctl).Visible = false;
            }
        }
    }
    #endregion

    #region CheckUserPageOperations (ByControl)
    public static void CheckUserApprovalRights(string MemberID, string PageURL, Control Ctrl)
    {
        if (MemberID != "" && PageURL != "" && Ctrl != null)
        {
            string strSQL = "Select b.Operations" +
                " from usrUsers a" +
                " Inner join usrScreenOpsByRole b on(b.RoleID = a.RoleID)" +
                " Inner join usrScreens c on (c.ScreenID = b.ScreenID)" +
                " Where a.UserID=" + MemberID + " and instr(upper(b.Operations),'A') > 0 and '~/' || c.ScreenURL='" + PageURL + "'";
            DataTable dt = DBHelper.GetDataTable(strSQL);

            if (dt.Rows.Count > 0)
                Ctrl.Visible = true;
            else
                Ctrl.Visible = false;
        }
    }
    #endregion

    #region CheckUserPageOperations and return to callaing function
    public static bool CheckUserPageOperations(string MemberID, string PageURL)
    {
        if (MemberID != "" && PageURL != "")
        {
            string strSQL = "Select b.Operations from usrUsers a Inner join usrScreenOpsByRole b on(b.RoleID = a.RoleID) Inner join usrScreens c on (c.ScreenID = b.ScreenID) Where a.UserID = " + MemberID + " and upper(b.Operations) = 'V' and '~/' || c.ScreenURL = '" + PageURL + "'";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            if (dt.Rows.Count > 0)
                return false;
            else
                return true;
        }
        else
        {
            return true;
        }
    }
    #endregion

    #region CheckDate(Data from Database)
    public static string CheckDatabaseDate(ref string Date, string FieldDisplayName, bool AllowEmpty)
    {
        if (Date == string.Empty)
        {
            Date = "null";
            if (AllowEmpty)
                return "";
            else
                return FieldDisplayName + " should not be empty<br />";
        }
        else
        {
            try
            {
                CultureInfo cInfo = new CultureInfo("en-US", false); //"MM-dd-yyyy"
                DateTime tDate = DateTime.Parse(Date, cInfo.DateTimeFormat);
                Date = " To_Date('" + CheckDBNullForDate(tDate) + "', 'dd-MM-yyyy')";
                //For MS SQL Server use: " Convert(datetime, '" + Date + "', 103)";
                return "";
            }
            catch (FormatException)
            {
                Date = "null";
                return FieldDisplayName + " is incorrect. (Expected date format: 'dd-MM-yyyy')<br />";
            }
        }
    }
    #endregion

    #region SetDefaultItem
    public static void SetDefaultItem(DropDownList ddl, int Index)
    {
        if (ddl.Items.Count > 0) { ddl.SelectedIndex = 1; }
    }
    #endregion

    #region GetAllItemNames for Local Purchases

    #region GetAllItemNames
    public static string[] GetAllItemNames1(string prefixText, int count, string contextKey)
    {
        if (contextKey == null) contextKey = "";
        return GetDBValues1(prefixText, count, contextKey,"t2.ItemName");
    }
    #endregion

    #region GetDBValues
    private static string[] GetDBValues1(string prefixText, int count, string contextKey, string ColumnName)
    {
         string strSQL1 = "Select * from (SELECT " + ColumnName + ", ROW_NUMBER() OVER (ORDER BY " + ColumnName + ") RNum " +
                 "FROM lptbreceipts t1  " +
                 "inner join lptbreceiptitems t2 on (t1.receiptid =t2.receiptid) " +
                 " WHERE Upper(" + ColumnName + ") like Upper('" + CheckSingleQuote(prefixText) + "%') " + contextKey +
                 " Order By " + ColumnName + ") Where RNum <= " + count.ToString();
        
        DataTable dtblLst = DBHelper.GetDataTable(strSQL1);
        return CreateItems(dtblLst, dtblLst.Columns[0]);
    }
    #endregion

    #endregion

    #region ToNullableInt
    public static int? ToNullableInt(string Value, bool AllowZero)
    {
        return ToNullableInt((Value != null && Value != "" && Value != "null") ? int.Parse(Value) : 0, AllowZero);
    }
    public static int? ToNullableInt(int Value,bool AllowZero)
    {
        if (AllowZero)
        {
            return (int?)Value;
        }
        else
        { 
            return (Value != 0)? (int?)Value : null;
        }
    }
    #endregion

    public static string AutoGenerateNumbersFAC(string ID, bool IsReceipt, string mType)
    {
        string mGenNo = "";
        string mID = GenFunctions.CheckEmptyString(ID, "0"); ;
        string strSQL = string.Empty;
        strSQL = "Select FacilityCode as Prefix from masFacilities Where FacilityID = " + mID;
        
        DataTable dt = DBHelper.GetDataTable(strSQL);
        if (dt.Rows.Count > 0)
        {
            string mToday = GenFunctions.CheckDBNullForDate(DateTime.Now);
            GenFunctions.CheckDate(ref mToday, "Today", false);
            string strSQL1 = "Select SHAccYear from masAccYearSettings Where " + mToday + " between StartDate and EndDate";
            DataTable dt1 = DBHelper.GetDataTable(strSQL1);
            if (dt1.Rows.Count > 0)
            {
                string mSHAccYear = dt1.Rows[0]["SHAccYear"].ToString();
                string mPrefix = dt.Rows[0]["Prefix"].ToString();

                    if (IsReceipt)
                        strSQL = "Select Lpad(NVL(Max(To_Number(SubStr(ReceiptNo, 10, 5))), 0) + 1, 5, '0') as WHSlNo from tbReceipts Where ReceiptNo Like '" + mPrefix + "/" + mType + "/%/" + mSHAccYear + "' and FacilityID=" + mID;
                
                DataTable dt3 = DBHelper.GetDataTable(strSQL);
                if (dt3.Rows.Count > 0)
                    mGenNo = mPrefix + "/" + mType + "/" + dt3.Rows[0]["WHSlNo"].ToString() + "/" + mSHAccYear;
                else
                    mGenNo = mPrefix + "/" + mType + "/" + "00001" + "/" + mSHAccYear;
            }
        }
        return mGenNo;
    }
    #region GetMultiple
    public static int getMultiple(string ItemId)
    {
        int MulValue = 0;
        string str = "select Multiple from masitems where itemid=" + ItemId + " union select Multiple from lpmasitems where lpitemid=" + ItemId + " ";
        DataTable dtbl = DBHelper.GetDataTable(str);
        if (dtbl.Rows.Count > 0)
        {
            MulValue = Convert.ToInt16(dtbl.Rows[0]["Multiple"].ToString());

        }
        return MulValue;


    }
    #endregion

    #region GetUnitcount
    public static int getunitcount(string ItemId)
    {
        int MulValue = 0;
        string str = "select Unitcount from masitems where itemid=" + ItemId + " union select Unitcount from lpmasitems where lpitemid=" + ItemId + " ";
        DataTable dtbl = DBHelper.GetDataTable(str);
        if (dtbl.Rows.Count > 0)
        {
            MulValue = Convert.ToInt16(dtbl.Rows[0]["Unitcount"].ToString());

        }
        return MulValue;


    }
    #endregion
	
	  #region Auto Generate - InterFac Transfer Code for Local Purchase
    public static string AutoGenerateITRFacCode(string mPSAID, string mAccYrSetID)
    {
        string mItemCode = "";
        if (mPSAID.Trim() == "") mPSAID = "0";
        if (mAccYrSetID.Trim() == "") mAccYrSetID = "0";

        string strSQL3 = " Select Lpad(NVL(Max(To_Number(a.AUTO_CODE)), 0) + 1, 5, '0') as SOCode" +
            " From MASFACTRANSFERS a" +
            " where a.FACILITYID=" + mPSAID + " and a.accyrsetid=" + mAccYrSetID;
        DataTable dt3 = DBHelper.GetDataTable(strSQL3);

        if (dt3.Rows.Count > 0)
            mItemCode = dt3.Rows[0]["SOCode"].ToString();
        else
            mItemCode = "00001";

        return mItemCode;
    }
    #endregion
	
	
	
 public static string AutoGenerateREGAICode(string mPSAID, string mAccYrSetID)
 {
     string mItemCode = "";
     if (mPSAID.Trim() == "") mPSAID = "0";
     if (mAccYrSetID.Trim() == "") mAccYrSetID = "0";

     string strSQL3 = " Select Lpad(NVL(Max(To_Number(a.Auto_AICODE)), 0) + 1, 5, '0') as SOCode" +
         " From MASREGANUALINDENT  a" +
         " where a.FACILITYID=" + mPSAID + " and a.accyrsetid=" + mAccYrSetID;
     DataTable dt3 = DBHelper.GetDataTable(strSQL3);

     if (dt3.Rows.Count > 0)
         mItemCode = dt3.Rows[0]["SOCode"].ToString();
     else
         mItemCode = "00001";

     return mItemCode;
 }

	
	
}
