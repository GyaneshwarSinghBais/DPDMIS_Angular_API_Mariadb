
using DPDMIS_Angular_API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace CgmscHO_API.Utility
{
  
    public class master
    {
        // private readonly OraDbContext _context;
        private readonly MariaDbContext _context;
        public master(MariaDbContext context)
        {
            _context = context;
        }


        public string getOPReceiptItemidID(string facilityId, string itemId)
        {
            string retvalue = "0";
            string strQuery = @"  select facreceiptitemid from tbfacilityreceiptitems  where facreceiptid=" + facilityId + " and itemid=" + itemId;
            //DataTable dt = DBHelper.GetDataTable(strQuery);
            var myList = _context.GetfacreceiptitemidDbSet
         .FromSqlInterpolated(FormattableStringFactory.Create(strQuery)).ToList();

            if (myList.Count > 0)
            {
                retvalue = myList[0].FACRECEIPTID.ToString();
            }
            return retvalue;

        }


        public bool getSameBatchInItems(string batchno, string facreceiptitemid,string itemId)
        {
            bool value = true;
            string strQuery = @" select INWNO from tbFacilityReceiptBatches where itemid=" + itemId + " and batchno='" + batchno + "' and FACreceiptitemid=" + facreceiptitemid;
            var myList = _context.GetInwardNoDbSet
       .FromSqlInterpolated(FormattableStringFactory.Create(strQuery)).ToList();
            if (myList.Count > 0)
            {
                value = false;
            }
            return value;

        }

        public bool isFacilityReceipt(string facReceiptId)
        {
            bool value = false;


            string strQuery = @" select  r.FACRECEIPTID from tbfacilityreceipts r
                    inner join tbfacilityreceiptitems ri on ri.facreceiptid = r.facreceiptid
                    inner join tbfacilityreceiptbatches rb on rb.facreceiptitemid = ri.facreceiptitemid
                    where r.facreceiptid = " + facReceiptId + " ";

            var myList = _context.GetFacilityReceiptIdDbSet
       .FromSqlInterpolated(FormattableStringFactory.Create(strQuery)).ToList();

            if (myList.Count > 0)
            {
                value = true;
            }

            return value;
        }


       


        public string checkstock(string recid)
        {
            string drugcode = "";

            string checkinitems = "select FACRECEIPTITEMID from tbfacilityreceiptitems where FACreceiptid=" + recid;

   
            var myListdtnilrow = _context.GetFacilityReceiptItemIdDbSet
            .FromSqlInterpolated(FormattableStringFactory.Create(checkinitems)).ToList();

            string qry = " select  nvl(i.absrqty,0) opening_qty,sum(nvl(bt.absrqty,0)) as batchqty,i.itemid,mi.itemcode from tbfacilityreceipts r  " +
     " inner join tbfacilityreceiptitems i on r.FACreceiptid=i.FACreceiptid  " +
     " left outer join tbFacilityReceiptBatches bt on bt.FACreceiptitemid=i.FACreceiptitemid  " +
     " inner join masitems mi on mi.itemid=i.itemid  where  FACreceipttype='FC' and r.FACreceiptid=" + recid + " " +
      " group by i.absrqty,i.itemid,mi.itemcode having sum(nvl(bt.absrqty,0)) <>i.absrqty ";

            var myList = _context.GetItemCodeDbSet
            .FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList();

          

            if (myListdtnilrow.Count > 0)
            {
                if (myList.Count > 0)
                {
                    drugcode = myList[0].ITEMCODE.ToString();
                }

                else
                {
                    drugcode = "True";
                }
            }
            else
            {
                drugcode = "True";
            }
            return drugcode;
        }





        public string FacAutoGenerateNumbers(string FacilityID, bool IsReceipt, string mType)
        {
            string mGenNo = "";
            string mFacID = FacilityID;
            string strSQL = "Select FacilityCode from masFacilities Where FacilityID = " + mFacID;



            var myList = _context.GetFacilityCodeDbSet
          .FromSqlInterpolated(FormattableStringFactory.Create(strSQL)).ToList();


            if (myList.Count > 0)
            {
                string mToday = DateTime.Now.ToString();
               // CheckDate(ref mToday, "Today", false);
                string strSQL1 = "Select SHAccYear from masAccYearSettings Where CURDATE() between StartDate and EndDate";


                var myList1 = _context.GetHSccDbSet
          .FromSqlInterpolated(FormattableStringFactory.Create(strSQL1)).ToList();

                if (myList1.Count > 0)
                {
                    string mSHAccYear = myList1[0].SHACCYEAR.ToString();
                    string mWHPrefix = myList[0].FACILITYCODE.ToString();
                    if (IsReceipt)
                    {
                        //strSQL = "Select Lpad(NVL(Max(To_Number(SubStr(FacReceiptNo, -11, 5))), 0) + 1, 5, '0') as WHSLNO from tbFacilityReceipts Where FacReceiptNo Like '" + mWHPrefix + "/" + mType + "/%/" + mSHAccYear + "'";
                         strSQL = @" SELECT LPAD(COALESCE(CAST(SUBSTRING(FacReceiptNo, -11, 5) AS UNSIGNED), 0) + 1, 5, '0') AS WHSLNO 
                    FROM tbFacilityReceipts 
                    WHERE FacReceiptNo LIKE CONCAT('" + mWHPrefix + "', '/', '" + mType + "', '/%', '/') ";
                    }
                    else
                    {
                        //strSQL = "Select Lpad(NVL(Max(To_Number(SubStr(IssueNo, -11, 5))), 0) + 1, 5, '0') as WHSLNO from tbFacilityIssues Where IssueNo Like '" + mWHPrefix + "/" + mType + "/%/" + mSHAccYear + "'";
                        strSQL = "SELECT LPAD(COALESCE(CAST(SUBSTRING(IssueNo, -11, 5) AS UNSIGNED), 0) + 1, 5, '0') AS WHSLNO " +
                "FROM tbFacilityIssues " +
                "WHERE IssueNo LIKE '" + mWHPrefix + "/" + mType + "/%/" + mSHAccYear + "'";
                    }

                    var myList2 = _context.GetWHsSerialNoDbSet
       .FromSqlInterpolated(FormattableStringFactory.Create(strSQL)).ToList();


                    if (myList2.Count > 0)
                        mGenNo = mWHPrefix + "/" + mType + "/" + myList2[0].WHSLNO.ToString() + "/" + mSHAccYear;
                    else
                        mGenNo = mWHPrefix + "/" + mType + "/" + "00001" + "/" + mSHAccYear;
                }
            }
            return mGenNo;
        }




        public bool checkAlreadyFC(Int64 usrFacilityID)
        {
            bool data = false;
            string checkFC = " select FACRECEIPTID from tbfacilityreceipts  s  where  s.facreceipttype='FC' and s.facilityid=" + usrFacilityID + " ";

            var myList = _context.GetFacReceiptIdDbSet
       .FromSqlInterpolated(FormattableStringFactory.Create(checkFC)).ToList();

           
            if (myList.Count == 0)
            {
                data = true;
            }
            else
            {
                data = false;
            }


            return data;
        }


        //Function for MariaDB Date format  //insert example 28-11-2024
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
                    // Define possible date formats
                    string[] formats = { "dd-MM-yyyy", "MM/dd/yyyy h:mm:ss tt" }; // Add more formats as needed

                    // Create the CultureInfo object for "fr-FR"
                    CultureInfo cInfo = new CultureInfo("fr-FR", false);

                    // Try to parse using multiple formats
                    if (DateTime.TryParseExact(Date, formats, cInfo, DateTimeStyles.None, out DateTime tDate))
                    {
                        // Use MariaDB's STR_TO_DATE function for date parsing
                        Date = $"STR_TO_DATE('{CheckDBNullForDate(tDate)}', '%d-%m-%Y')";
                        return "";
                    }
                    else
                    {
                        Date = "null";
                        return FieldDisplayName + " is incorrect. (Expected date format: 'dd-MM-yyyy')<br />";
                    }
                }
                catch (FormatException)
                {
                    Date = "null";
                    return FieldDisplayName + " is incorrect. (Expected date format: 'dd-MM-yyyy')<br />";
                }
            }
        }



        //public static string CheckDate(ref string Date, string FieldDisplayName, bool AllowEmpty)
        //{
        //    if (Date == string.Empty)
        //    {
        //        Date = "null";
        //        if (AllowEmpty)
        //            return "";
        //        else
        //            return FieldDisplayName + " should not be empty<br />";
        //    }
        //    else
        //    {
        //        try
        //        {

        //            // Define possible date formats
        //            string[] formats = { "dd-MM-yyyy", "MM/dd/yyyy h:mm:ss tt" }; // Add more formats as needed

        //            // Create the CultureInfo object for "fr-FR"
        //            CultureInfo cInfo = new CultureInfo("fr-FR", false);

        //            // Try to parse using multiple formats
        //            if (DateTime.TryParseExact(Date, formats, cInfo, DateTimeStyles.None, out DateTime tDate))
        //            {
        //                Date = " To_Date('" + CheckDBNullForDate(tDate) + "', 'dd-MM-yyyy')";
        //                // For MS SQL Server use: " Convert(datetime, '" + Date + "', 103)";
        //                return "";
        //            }
        //            else
        //            {
        //                Date = "null";
        //                return FieldDisplayName + " is incorrect. (Expected date format: 'dd-MM-yyyy')<br />";
        //            }

        //            //CultureInfo cInfo = new CultureInfo("fr-FR", false); //"dd-MM-yyyy"
        //            //DateTime tDate = DateTime.Parse(Date, cInfo.DateTimeFormat);
        //            //Date = " To_Date('" + CheckDBNullForDate(tDate) + "', 'dd-MM-yyyy')";
        //            ////For MS SQL Server use: " Convert(datetime, '" + Date + "', 103)";
        //            //return "";
        //        }
        //        catch (FormatException)
        //        {
        //            Date = "null";
        //            return FieldDisplayName + " is incorrect. (Expected date format: 'dd-MM-yyyy')<br />";
        //        }
        //    }
        //}

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


        //public string getFacCodeForIndent(Int64 facId)
        //{
        //    string yearid = getACCYRSETID();
        //    string qry = " Select a.FACILITYID, Lpad(NVL(Max(To_Number(a.AUTO_NCCODE)), 0) + 1, 5, '0') as FACILITYCODE" +
        //       " From MASCGMSCNOC a" +
        //       " where a.FACILITYID=" + facId + " and a.accyrsetid=" + yearid + " group by a.FACILITYID";

        //    var myList = _context.GenWhIndentNoDbSet
        //    .FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList();

        //    if (myList.Count > 0)
        //    {
        //        string facCode = myList[0].FACILITYCODE; // Assuming IssueItemID is an integer
        //        return facCode.ToString();
        //    }
        //    else
        //    {
        //        return "00001"; // Or any other appropriate indication
        //    }
        //}

    }
}
