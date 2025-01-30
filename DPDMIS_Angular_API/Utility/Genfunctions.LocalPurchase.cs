#region Using
using System;
using System.Configuration;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.Collections.Generic;
using DBHelper = Broadline.WMS.Data.DBHelper;
#endregion

/// <summary>
/// Summary description for Genfunctions
/// </summary>
public partial class GenFunctions
{
    #region LocalPurchases
    public class LocalPurchase
    {
    #region Sanction For Finance
        public static ReportDocument Sanction(int? SanctionID)
        {
            #region Prepare Queries

            #region Sanction Query

            #region Where Clause
            string WhereClause = "";
            if (SanctionID != null) { WhereClause += " and San.SanctionID =" + SanctionID.Value.ToString(); }
            #endregion

            #region OLD Query
            //string SanctionQuery = "Select " +
            //                        "Sop.AccYrSetID," +
            //                        "Yr.AccYear," +
            //                        "Sop.SourceID," +
            //                        "So.SourceName," +
            //                        "Sop.SchemeID," +
            //                        "Sc.SchemeName," +
            //                        "Sop.PoNoID," +
            //                        "Sop.PoNo," +
            //                        "Sop.PoDate," +
            //                        "Psa.PsaID," +
            //                        "Psa.PsaName," +
            //                        "Dist.DistrictID," +
            //                        "Dist.DistrictName," +
            //                        "Sop.SupplierID," +
            //                        "Sop.LPSupplierID, " +
            //                        "Sup.SupplierName," +
            //                        "Sup.Address1," +
            //                        "Sup.Address2," +
            //                        "Sup.Address3," +
            //                        "Sup.City," +
            //                        "Sup.Zip," +
            //                        "LPSup.SupplierName," +
            //                        "LPSup.Address1," +
            //                        "LPSup.Address2," +
            //                        "LPSup.Address3," +
            //                        "LPSup.City," +
            //                        "LPSup.Zip," +
            //                        //"Co.CountryID," +
            //                        //"Co.CountryName," +
            //                        //"Acc.BankAccountID," +
            //                        //"Acc.AccountNo," +
            //                        //"Acc.AccountName," +
            //                        //"Acc.BankName," +
            //                        //"Acc.Branch," +
            //                        //"Acc.IFSCCode," +
            //                        //"Acc.MICRCode," +
            //                        "San.SanctionID," +
            //                        "San.SanctionNo," +
            //                        "San.SanctionDate," +
            //                        "San.TotNetAmount," +
            //                        "San.SanctionedAmount," +
            //                        "nvl(San.AdvPaymentDeducted,0) as AdvPaymentDeducted," +
            //                        "Bud.BudgetID," +
            //                        "Bud.AccountCode," +
            //                        "Bud.BudgetDetail," +
            //                        "Bud.AllottedAmount," +
            //                        "Bud.AllottedDate," +
            //                        "Cat.CategoryID," +
            //                        "Cat.CategoryName " +
            //                        "from lpSanctions San " +
            //                        "Inner Join LPsoOrderPlaced Sop on (Sop.PoNoID = San.PoNoID) " +
            //                        "Inner Join masAccYearSettings Yr on (Yr.AccYrSetID = Sop.AccYrSetID) " +
            //                        "Inner Join masSources So on (So.SourceID = Sop.SourceID) " +
            //                        "Inner Join masSchemes Sc on (Sc.SchemeID = Sop.SchemeID) " +
            //                        "Inner Join masPsas Psa on (Psa.PsaID = Sop.PsaID) " +
            //                        "Inner Join masDistricts Dist on (Dist.DistrictID = Psa.DistrictID)" +
            //                        //"Inner Join masCountries Co on (Co.CountryID = Sup.CountryID) " +
            //                        "Left Outer Join masSupplierAccNos Acc on (Acc.BankAccountID = San.BankAccountID) " +
            //                        "Left Outer Join masSuppliers Sup on (Sup.SupplierID = Sop.SupplierID) " +
            //                        "Left Outer Join LPmasSuppliers LPSup on (LPSup.LPSupplierID = Sop.LPSupplierID) " +
            //                        "Left Outer Join FrcstAnnualBudgets Bud on (Bud.BudgetID = San.BudgetID) " +
            //                        "Left Outer Join FrcstAnnualBudgetPsaDet BudDet on (BudDet.BudgetID = Bud.BudgetID and BudDet.BudgetID = Sop.BudgetID and BudDet.PsaID = Sop.PsaID) " +
            //                        //"Inner Join masItemCategories Cat on (Cat.CategoryID = BudDet.CategoryID) " +
            //                        "Where 1=1 " + WhereClause;
            #endregion

            string SanctionQuery = "Select Sop.AccYrSetID,Yr.AccYear," +
                                  " Sop.SourceID,So.SOURCEName,Sop.SchemeID,sc.schemename," +
                                  " Cat.CategoryID,Cat.CategoryName, " +
                                  " Sop.PoNoID,Sop.PoNo,Sop.PoDate," +
                                  " Sop.PsaID,Psa.PsaName,Dist.DistrictID,Dist.DistrictName, " +
                                  " Sop.SupplierID,Sop.LPSupplierID, " +
                                  " Sup.SupplierName,Sup.Address1,Sup.Address2,Sup.Address3,Sup.City,Sup.Zip, " +
                                  " LPSup.SupplierName as LPSupplierName,LPSup.Address1 as LPAddress1,LPSup.Address2 as LPAddress2," +
                                  " LPSup.Address3 as LPAddress3,LPSup.City as LPCity,LPSup.Zip as LPZip, " +
                                  " Acc.BankAccountID,Acc.AccountNo,Acc.AccountName,Acc.BankName,Acc.Branch,Acc.IFSCCode,Acc.MICRCode, " +
                                  " San.SanctionID,San.SanctionNo,San.SanctionDate,San.TotNetAmount, " +
                                  " San.SanctionedAmount,nvl(San.AdvPaymentDeducted,0) as AdvPaymentDeducted, " +
                                  " Bud.BudgetID,Bud.AccountCode,Bud.BudgetDetail,Bud.AllottedAmount,Bud.AllottedDate  " +
                                  " from lpSanctions San " +
                                  " Inner Join LPsoOrderPlaced Sop on (Sop.PoNoID = San.PoNoID) " +
                                  " Inner Join masAccYearSettings Yr on (Yr.AccYrSetID = Sop.AccYrSetID) " +
                                  " Inner Join masSources So on (So.SourceID = Sop.SourceID) " +
                                  " Inner Join masSchemes Sc on (Sc.SchemeID = Sop.SchemeID) " +
                                  " Inner Join masItemCategories Cat on (Cat.CategoryID = sop.CategoryID) " +
                                  " Inner Join masPsas Psa on (Psa.PsaID = Sop.PsaID) " +
                                  " Inner Join masDistricts Dist on (Dist.DistrictID = Psa.DistrictID) " +
                                  " Left Outer Join masSupplierAccNos Acc on (Acc.BankAccountID = San.BankAccountID) " +
                                  " Left Outer Join FrcstAnnualBudgets Bud on (Bud.BudgetID = San.BudgetID) " +
                                  " Left Outer Join FrcstAnnualBudgetPsaDet BudDet on (BudDet.BudgetID = Bud.BudgetID and BudDet.PsaID = Sop.PsaID and BudDet.BudgetID = sop.BudgetID) " +
                                  " Left Outer Join masSuppliers Sup on (Sup.SupplierID = Sop.SupplierID) " +
                                  " Left Outer Join LPmasSuppliers LPSup on (LPSup.LPSupplierID = Sop.LPSupplierID) " +
                                  " Where 1=1 " + WhereClause;

           

            #endregion

            #region Invoice Query

            #region Where Clause
            WhereClause = "";
            if (SanctionID != null) { WhereClause += " and Inv.SanctionID =" + SanctionID.Value.ToString(); }
            #endregion

            #region Query
            string InvoiveQuery = "Select " +
                                    "Inv.SanctionID," +
                                    "Inv.InvoiceID," +
                                    "Inv.InvoiceNo," +
                                    "Inv.InvoiceDate," +
                                    "Inv.GrossAmount as GrossAmount," +
                                    "nvl(Inv.TaxAmount,0) as TaxAmount," +
                                    "nvl(Inv.Deductions,0) as Deductions," +
                                    "Inv.NetAmount," +
                                    "Inv.OnHoldAmount," +
                                    "Inv.OnHoldAmountPaid " +
                                    "from lpInvoices Inv " +
                                    "Where 1=1 " +
                                    "and Inv.SanctionID is not null " + WhereClause +
                                    " Order by Inv.InvoiceDate,Inv.InvoiceNo";
            #endregion

            #endregion

            #region Tax Query

            #region Where Clause
            WhereClause = "";
            if (SanctionID != null) { WhereClause += " and Tax.SanctionID =" + SanctionID.Value.ToString(); }
            #endregion

            #region Query
            string TaxQuery = "Select " +
                                "Tax.TaxID," +
                                "Tax.TaxTypeID," +
                                "TTyp.TaxCategory," +
                                "TTyp.TaxTypeName," +
                                "Tax.TaxValue " +
                                "from lpTaxs Tax " +
                                "inner join blpTaxTypes TTyp on (TTyp.TaxTypeID = Tax.TaxTypeID) " +
                                "Where 1=1 " + WhereClause;
            #endregion

            #endregion

            #endregion

            #region Fetch Data

            DataSet dsSanction = new DataSet();
            DataSet dsInvoice = new DataSet();
            DataSet dsTax = new DataSet();

            #region Sanction Table

            DataTable dtSanction = DBHelper.GetDataTable(SanctionQuery);

            #region Validate Fetched Data For Records
            if (dtSanction.Rows.Count <= 0) { throw new CustomExceptions.NoDataFoundException("Creation of  Sanction Document Failed Due! Reason : Sanction Details Missing"); }
            #endregion

            #region SanctionData Modification

            dtSanction.Columns.Add("SanAmountInwords", typeof(string));

            foreach (DataRow iRow in dtSanction.Rows)
            {
                iRow["SanAmountInwords"] = GenFunctions.ToWord(double.Parse(dtSanction.Rows[0]["SanctionedAmount"].ToString()));
            }

            #endregion

            dtSanction.TableName = "dtLPSanction";
            dsSanction.Tables.Add(dtSanction);

            #endregion

            #region Invoice Table

            DataTable dtInvoice = DBHelper.GetDataTable(InvoiveQuery);

            #region Validate Fetched Data For Records
            if (dtInvoice.Rows.Count <= 0) { throw new CustomExceptions.NoDataFoundException("Creation of  Sanction Document Failed Due! Reason :No Invoices Found"); }
            #endregion

            dtInvoice.TableName = "dtLPInvoice";
            dsInvoice.Tables.Add(dtInvoice);

            #endregion

            #region Tax Table

            DataTable dtTax = DBHelper.GetDataTable(TaxQuery);

            #region Validate Fetched Data For Records
            if (dtTax.Rows.Count <= 0)
            {
                DataRow NoTaxRow = dtTax.NewRow();
                NoTaxRow["TaxID"] = 0;
                NoTaxRow["TaxTypeID"] = 0;
                NoTaxRow["TaxCategory"] = "N/A";
                NoTaxRow["TaxTypeName"] = "N/A";
                NoTaxRow["TaxValue"] = 0;
                dtTax.Rows.Add(NoTaxRow);
            }
            #endregion

            dtTax.TableName = "dtLPTax";
            dsTax.Tables.Add(dtTax);

            #endregion

            #endregion

            #region Crystal Report [Sub Reports Datasets]

            List<SubReportDatasets> Subsets = new List<SubReportDatasets>();

            Subsets.Add(new SubReportDatasets("LPSanction_Invoice.rpt", dsInvoice));
            Subsets.Add(new SubReportDatasets("LPSanction_Tax.rpt", dsTax));

            #endregion

            #region Prepare Crystal Report Parameter
            List<Parameters> Params = new List<Parameters>();
            Params.Add(new Parameters("Sanctioned Amount", (decimal)dtSanction.Rows[0]["SanctionedAmount"], "LPSanction_Invoice.rpt"));
            Params.Add(new Parameters("Gross Amount", (decimal)dtSanction.Rows[0]["TotNetAmount"], "LPSanction_Tax.rpt"));
            Params.Add(new Parameters("Net Amount", (decimal)dtSanction.Rows[0]["SanctionedAmount"], "LPSanction_Tax.rpt"));
            #endregion

            #region Create Crystal Report Document
            ReportDocument Document = GenFunctions.CrystalReports.GenerateReport(@"~/LocalPurchases/CrystalReport/LPSanction.rpt", dsSanction,Params ,Subsets );
            #endregion

            return Document;
        }
        #endregion

    #region Sanction Supplier Annexure (Invoice Break Up)
        public static ReportDocument SanctionSupAnnexure(int? SanctionID)
        {
            #region Prepare Queries

            #region Invoices

            #region Where Clause
            string WhereClause = "";
            if (SanctionID != null) { WhereClause += " and Inv.SanctionID =" + SanctionID.Value.ToString(); }
            #endregion

            #region Query
            string InvoicesQuery = "Select " +
                           "San.SanctionID," +
                           "Sop.PonoID,Sop.Pono,Sop.PoDate,Sop.SoValue," +
                           "Inv.InvoiceID,Inv.InvoiceNo,Inv.InvoiceDate,Inv.Grossamount," +
                           "TaxType.TaxTypeID,TaxType.TaxCategory,TaxType.TaxTypeName," +
                           "Tax.TaxID,Case When TaxType.TaxCategory = 'D' then Tax.Taxvalue * -1 else Tax.Taxvalue end as Taxvalue " +
                           "from lpSanctions San " +
                           "Inner Join LPSoOrderPlaced Sop on (Sop.PonoID = San.PonoID) " +
                           "Inner Join lpInvoices Inv on (Inv.SanctionID = San.SanctionID and Inv.PonoID = San.PonoID) " +
                           "Left Outer Join lpTaxs Tax on (Tax.InvoiceID = Inv.InvoiceID) " +
                           "Left Outer Join blpTaxTypes TaxType on (TaxType.TaxTypeID = Tax.TaxTypeID) " +
                           "Where 1=1 " + WhereClause +
                           " Order by Inv.InvoiceDate, Inv.InvoiceNo, Tax.TaxValue desc";
            #endregion

            #endregion

            #region Sanction

            #region Where Clause
            WhereClause = "";
            if (SanctionID != null) { WhereClause += " and San.SanctionID =" + SanctionID.Value.ToString(); }
            #endregion

            #region OLD Query
            //string SanctionQuery = "Select " +
            //                        "Sop.AccYrSetID," +
            //                        "Yr.AccYear," +
            //                        "Sop.SourceID," +
            //                        "So.SourceName," +
            //                        "Sop.SchemeID," +
            //                        "Sc.SchemeName," +
            //                        "Sop.PoNoID," +
            //                        "Sop.PoNo," +
            //                        "Sop.PoDate," +
            //                        "Psa.PsaID," +
            //                        "Psa.PsaName," +
            //                        "Dist.DistrictID," +
            //                        "Dist.DistrictName," +
            //                        "Sop.SupplierID," +
            //                        "Sup.SupplierName," +
            //                        "Sup.Address1," +
            //                        "Sup.Address2," +
            //                        "Sup.Address3," +
            //                        "Sup.City," +
            //                        "Sup.Zip," +
            //                        "Acc.BankAccountID," +
            //                        "Acc.AccountNo," +
            //                        "Acc.AccountName," +
            //                        "Acc.BankName," +
            //                        "Acc.Branch," +
            //                        "Acc.IFSCCode," +
            //                        "Acc.MICRCode," +
            //                        "San.SanctionID," +
            //                        "San.SanctionNo," +
            //                        "San.SanctionDate," +
            //                        "San.TotNetAmount," +
            //                        "San.SanctionedAmount," +
            //                        "nvl(San.AdvPaymentDeducted,0) as AdvPaymentDeducted," +
            //                        "Bud.BudgetID," +
            //                        "Bud.AccountCode," +
            //                        "Bud.BudgetDetail," +
            //                        "Bud.AllottedAmount," +
            //                        "Bud.AllottedDate," +
            //                        "Cat.CategoryID," +
            //                        "Cat.CategoryName " +
            //                        "from lpSanctions San " +
            //                        "Inner Join LPsoOrderPlaced Sop on (Sop.PoNoID = San.PoNoID) " +
            //                        "Inner Join masAccYearSettings Yr on (Yr.AccYrSetID = Sop.AccYrSetID) " +
            //                        "Inner Join masSources So on (So.SourceID = Sop.SourceID) " +
            //                        "Inner Join masSchemes Sc on (Sc.SchemeID = Sop.SchemeID) " +
                                    
            //                        "Inner Join masPsas Psa on (Psa.PsaID = Sop.PsaID) " +
            //                        "Inner Join masDistricts Dist on (Dist.DistrictID = Psa.DistrictID)" +
            //                        "Left Outer Join masSuppliers Sup on (Sup.SupplierID = Sop.SupplierID) " +
            //                        "Left Outer Join LPmasSuppliers LPSup on (LPSup.LPSupplierID = Sop.LPSupplierID) " +
                                   
            //                        "Left Outer Join masSupplierAccNos Acc on (Acc.BankAccountID = San.BankAccountID) " +
            //                        "Inner Join FrcstAnnualBudgets Bud on (Bud.BudgetID = San.BudgetID) " +
            //                        "Inner Join FrcstAnnualBudgetPsaDet BudDet on (BudDet.BudgetID = Bud.BudgetID and BudDet.BudgetID = sop.BudgetID and BudDet.PsaID = Sop.PsaID) " +
            //                        "Inner Join masItemCategories Cat on (Cat.CategoryID = BudDet.CategoryID) " +
            //                        "Where 1=1 " + WhereClause;
            #endregion


            string SanctionQuery = "Select Sop.AccYrSetID,Yr.AccYear," +
                                 " Sop.SourceID,So.SOURCEName,Sop.SchemeID,sc.schemename," +
                                 " Cat.CategoryID,Cat.CategoryName, " +
                                 " Sop.PoNoID,Sop.PoNo,Sop.PoDate," +
                                 " Sop.PsaID,Psa.PsaName,Dist.DistrictID,Dist.DistrictName, " +
                                 " Sop.SupplierID,Sop.LPSupplierID, " +
                                 " Sup.SupplierName,Sup.Address1,Sup.Address2,Sup.Address3,Sup.City,Sup.Zip, " +
                                 " LPSup.SupplierName as SupplierName,LPSup.Address1 as LPAddress1,LPSup.Address2 as LPAddress2," +
                                 " LPSup.Address3 as LPAddress3,LPSup.City as LPCity,LPSup.Zip as LPZip, " +
                                 " Acc.BankAccountID,Acc.AccountNo,Acc.AccountName,Acc.BankName,Acc.Branch,Acc.IFSCCode,Acc.MICRCode, " +
                                 " San.SanctionID,San.SanctionNo,San.SanctionDate,San.TotNetAmount, " +
                                 " San.SanctionedAmount,nvl(San.AdvPaymentDeducted,0) as AdvPaymentDeducted, " +
                                 " Bud.BudgetID,Bud.AccountCode,Bud.BudgetDetail,Bud.AllottedAmount,Bud.AllottedDate  " +
                                 " from lpSanctions San " +
                                 " Inner Join LPsoOrderPlaced Sop on (Sop.PoNoID = San.PoNoID) " +
                                 " Inner Join masAccYearSettings Yr on (Yr.AccYrSetID = Sop.AccYrSetID) " +
                                 " Inner Join masSources So on (So.SourceID = Sop.SourceID) " +
                                 " Inner Join masSchemes Sc on (Sc.SchemeID = Sop.SchemeID) " +
                                 " Inner Join masItemCategories Cat on (Cat.CategoryID = sop.CategoryID) " +
                                 " Inner Join masPsas Psa on (Psa.PsaID = Sop.PsaID) " +
                                 " Inner Join masDistricts Dist on (Dist.DistrictID = Psa.DistrictID) " +
                                 " Left Outer Join masSupplierAccNos Acc on (Acc.BankAccountID = San.BankAccountID) " +
                                 " Left Outer Join FrcstAnnualBudgets Bud on (Bud.BudgetID = San.BudgetID) " +
                                 " Left Outer Join FrcstAnnualBudgetPsaDet BudDet on (BudDet.BudgetID = Bud.BudgetID and BudDet.PsaID = Sop.PsaID and BudDet.BudgetID = sop.BudgetID) " +
                                 " Left Outer Join masSuppliers Sup on (Sup.SupplierID = Sop.SupplierID) " +
                                 " Left Outer Join LPmasSuppliers LPSup on (LPSup.LPSupplierID = Sop.LPSupplierID) " +
                                 " Where 1=1 " + WhereClause;
            #endregion

            #region Invoice(s) Items

            #region Where Clause
            WhereClause = "";
            if (SanctionID != null) { WhereClause += " and Inv.SanctionID =" + SanctionID.Value.ToString(); }
            #endregion

            #region Query
            //string InvoiceItemsQuery = "Select " +
            //                        "Inv.InvoiceID,InvItm.ItemID,InvItm.InvoiceAbsQty,InvItm.SingleUnitPrice,InvItm.ItemValue," +
            //                        "Item.ItemCode, Item.ItemName, Item.Unit as Sku, Item.Strength1 as Strength, Type.ItemTypeCode as ItemType " +
            //                        "from blpSanctions San " +
            //                        "Inner Join SoOrderPlaced Sop on (Sop.PonoID = San.PonoID) " +
            //                        "Inner Join BlpInvoices Inv on (Inv.SanctionID = San.SanctionID and Inv.PonoID = San.PonoID) " +
            //                        "Inner Join BlpInvoiceItems InvItm on (InvItm.InvoiceID = Inv.InvoiceID) " +
            //                        "Inner Join MasItems Item on(Item.ItemID = InvItm.ItemID) " +
            //                        "Inner Join Masitemtypes Type on(Type.ItemTypeID = Item.ItemTypeID) " +
            //                        "Order By San.SanctionID,Sop.PonoID,Inv.InvoiceID,InvItm.ItemID";
            #endregion

            string InvoiceItemsQuery = "Select Inv.InvoiceID,InvItm.InvoiceAbsQty,InvItm.SingleUnitPrice,InvItm.ItemValue, " +
                                          " coalesce(INVITM.ITEMID,invitm.lpitemid) as ItemID, " +
                                          " coalesce(ITEM.ITEMCODE,lp.itemcode) as ItemCode, " +
                                          " coalesce(ITEM.ITEMNAME,lp.itemname) as ItemName, " +
                                          " coalesce(ITEM.STRENGTH1,lp.strength) as Strength, " +
                                          " coalesce(item.unit, lp.unit) as SKU, " +
                                          " coalesce(item.packingqty,lp.packingqty) as PackingQty " +
                                          " from lpSanctions San  " +
                                          " Inner Join LPSoOrderPlaced Sop on (Sop.PonoID = San.PonoID) " +
                                          " Inner Join lpInvoices Inv on (Inv.SanctionID = San.SanctionID and Inv.PonoID = San.PonoID) " +
                                          " Inner Join lpInvoiceItems InvItm on (InvItm.InvoiceID = Inv.InvoiceID) " +
                                          " left Outer  Join MasItems Item on(Item.ItemID = InvItm.ItemID) " +
                                          " left Outer  Join LPMasItems LP on(LP.LPItemID = InvItm.LPItemID) " +
                                          " where 1=1 " + WhereClause + 
                                          " Order By San.SanctionID,Sop.PonoID,Inv.InvoiceID,InvItm.ItemID,LP.LPItemID "; 

            #endregion

            #region General Invoice Summery Query

            #region Where Clause
            WhereClause = "";
            if (SanctionID != null) { WhereClause += " and Inv.SanctionID =" + SanctionID.Value.ToString(); }
            #endregion

            #region Query
            string GeneralInvoiveQuery = "Select " +
                                    "Inv.SanctionID," +
                                    "Inv.InvoiceID," +
                                    "Inv.InvoiceNo," +
                                    "Inv.InvoiceDate," +
                                    "Inv.GrossAmount as GrossAmount," +
                                    "nvl(Inv.TaxAmount,0) as TaxAmount," +
                                    "nvl(Inv.Deductions,0) as Deductions," +
                                    "Inv.NetAmount," +
                                    "Inv.OnHoldAmount," +
                                    "Inv.OnHoldAmountPaid " +
                                    "from lpInvoices Inv " +
                                    "Where 1=1 " +
                                    "and Inv.SanctionID is not null " + WhereClause +
                                    " Order by Inv.InvoiceDate,Inv.InvoiceNo ";
            #endregion

            #endregion

            #region General Tax Summery Query

            #region Where Clause
            WhereClause = "";
            if (SanctionID != null) { WhereClause += " and Tax.SanctionID =" + SanctionID.Value.ToString(); }
            #endregion

            #region Query
            string GeneralTaxQuery = "Select " +
                                "Tax.TaxID," +
                                "Tax.TaxTypeID," +
                                "TTyp.TaxCategory," +
                                "TTyp.TaxTypeName," +
                                "Tax.TaxValue " +
                                "from blpTaxs Tax " +
                                "inner join blpTaxTypes TTyp on (TTyp.TaxTypeID = Tax.TaxTypeID) " +
                                "Where 1=1 " + WhereClause;
            #endregion

            #endregion

            #endregion

            #region Fetch Data

            DataSet dsInvoices = new DataSet();
            DataSet dsSanction = new DataSet();
            DataSet dsInvoiceItems = new DataSet();
            DataSet dsGenInvoices = new DataSet();
            DataSet dsGenTaxes = new DataSet();

            #region Invoices Table

            DataTable dtInvoices = DBHelper.GetDataTable(InvoicesQuery);

            #region Validate Fetched Data For Records
            if (dtInvoices.Rows.Count <= 0) { throw new CustomExceptions.NoDataFoundException("Creation of  Supplier Annexure (Invoice with Tax Info) Document Failed"); }
            #endregion

            dtInvoices.TableName = "dtLPSupAnnexure";
            dsInvoices.Tables.Add(dtInvoices);

            #endregion

            #region Sanction Table

            DataTable dtSanction = DBHelper.GetDataTable(SanctionQuery);

            #region Validate Fetched Data For Records
            if (dtSanction.Rows.Count <= 0) { throw new CustomExceptions.NoDataFoundException("Creation of  Sanction Document Failed Due! Reason : Sanction Details Missing"); }
            #endregion

            #region SanctionData Modification

            dtSanction.Columns.Add("SanAmountInwords", typeof(string));

            foreach (DataRow iRow in dtSanction.Rows)
            {
                iRow["SanAmountInwords"] = GenFunctions.ToWord(double.Parse(dtSanction.Rows[0]["SanctionedAmount"].ToString()));
            }

            #endregion

            dtSanction.TableName = "dtLPSanction";
            dsSanction.Tables.Add(dtSanction);

            #endregion

            #region Invoice Items Table

            DataTable dtInvoiceItems = DBHelper.GetDataTable(InvoiceItemsQuery);

            #region Validate Fetched Data For Records
            if (dtInvoiceItems.Rows.Count <= 0) { throw new CustomExceptions.NoDataFoundException("Creation of  Supplier Annexure (Invoice with Tax Info) Document Failed"); }
            #endregion

            dtInvoiceItems.TableName = "dtLPSupAnnexureInvoiceItems";
            dsInvoiceItems.Tables.Add(dtInvoiceItems);

            #endregion

            #region General Invoice Table

            DataTable dtGenInvoice = DBHelper.GetDataTable(GeneralInvoiveQuery);

            #region Validate Fetched Data For Records
            if (dtGenInvoice.Rows.Count <= 0) { throw new CustomExceptions.NoDataFoundException("Creation of  Sanction Document Failed Due! Reason :No Invoices Found"); }
            #endregion

            dtGenInvoice.TableName = "dtLPInvoice";
            dsGenInvoices.Tables.Add(dtGenInvoice);

            #endregion

            #region General Tax Table

            DataTable dtGenTax = DBHelper.GetDataTable(GeneralTaxQuery);

            #region Validate Fetched Data For Records
            if (dtGenTax.Rows.Count <= 0)
            {
                DataRow NoTaxRow = dtGenTax.NewRow();
                NoTaxRow["TaxID"] = 0;
                NoTaxRow["TaxTypeID"] = 0;
                NoTaxRow["TaxCategory"] = "N/A";
                NoTaxRow["TaxTypeName"] = "N/A";
                NoTaxRow["TaxValue"] = 0;
                dtGenTax.Rows.Add(NoTaxRow);
            }
            #endregion

            dtGenTax.TableName = "dtLPTax";
            dsGenTaxes.Tables.Add(dtGenTax);

            #endregion

            #endregion

            #region Crystal Report [Sub Reports Datasets]
            List<SubReportDatasets> Subsets = new List<SubReportDatasets>();
            Subsets.Add(new SubReportDatasets("LPSanctionSupLetter.rpt", dsSanction));
            Subsets.Add(new SubReportDatasets("LPSanctionSupAnnexureInvoiceItems.rpt", dsInvoiceItems));
            Subsets.Add(new SubReportDatasets("LPSanction_Invoice.rpt", dsGenInvoices));
            Subsets.Add(new SubReportDatasets("LPSanction_Tax.rpt", dsGenTaxes));
            #endregion

            #region Prepare Crystal Report Parameter
            List<Parameters> Params = new List<Parameters>();
            Params.Add(new Parameters("Sanctioned Amount", (decimal)dtSanction.Rows[0]["SanctionedAmount"], "LPSanction_Invoice.rpt"));
            Params.Add(new Parameters("Gross Amount", (decimal)dtSanction.Rows[0]["TotNetAmount"], "LPSanction_Tax.rpt"));
            Params.Add(new Parameters("Net Amount", (decimal)dtSanction.Rows[0]["SanctionedAmount"], "LPSanction_Tax.rpt"));
            #endregion

            #region Create Crystal Report Document
            ReportDocument Document = GenFunctions.CrystalReports.GenerateReport(@"~/LocalPurchases/CrystalReport/LPSanctionSupAnnexure.rpt", dsInvoices,Params,Subsets);
            #endregion
                        
            return Document;
        }
        #endregion

    #region Generate SuggestionList (PDF)
        public static ReportDocument GenerateSuggestionList(string IndentID, bool isIssued)
        {
            #region Parameter Validation
            IndentID = GenFunctions.CheckEmptyString(IndentID, "0");
            #endregion

            #region Prepare Query(SuggestionList)
            string Query = "Select a.ItemID,a.LPItemID,a.IndentItemID, a.Present, a.Needed, a.Allotted, " +
                " cast(a.ItemValue as numeric(10,2)) as ItemValue, " +
                " cast(a.SingleUnitPrice as numeric(10,2)) as SingleUnitPrice, NVL(cs.CurBal, 0) - NVL(cs.Reserved, 0) + NVL(a.Allotted, 0) as CurBal " +
                " from  LPtbIndentItems a " +
                " inner join LPtbIndents a1 on (a1.IndentID = a.IndentID) " +
                " Left outer Join V_LPCURSTOCKAVAILABLE cs on (cs.SourceID = a1.SourceID and cs.SchemeID = a1.SchemeID  " +
                " and cs.WarehouseID = a1.WarehouseID  and cs.ItemID = a.ItemID or cs.LPItemID=a.LPItemID) " +
                " where a.IndentID = " + IndentID;

            #endregion

            #region Fatch Data (SuggestionList)
            DataTable SuggestionList = DBHelper.GetDataTable(Query);

            string mItemID = SuggestionList.Rows[0]["ItemID"].ToString();
            string mLPItemID = SuggestionList.Rows[0]["LPItemID"].ToString();

            SuggestionList.TableName = "LPSuggestList";
            DataSet dsSuggestionList = new DataSet();
            dsSuggestionList.Tables.Add(SuggestionList);
            #endregion

            #region Prepare Query(dsBatchList)
            Query = "Select a.IndentItemID,a.InwNo,b.BatchNo,b.StockLocation,b.MfgDate,b.ExpDate,a.IssueQty" +
                    " from LPtbOutwards a" +
                    " inner join LPtbReceiptBatches b on (b.InwNo = a.InwNo)" +
                    " inner join LPtbReceiptItems b1 on (b1.ReceiptItemID = b.ReceiptItemID)" +
                    " inner join LPtbReceipts b2 on (b2.ReceiptID = b1.ReceiptID)" +
                    " Order By b.ExpDate,b.IssueQty";

            #endregion

            #region Fetch Data (dsBatchList)
            DataTable BatchList = DBHelper.GetDataTable(Query);
            BatchList.TableName = "LPSuggBatchList";
            DataSet dsBatchList = new DataSet();
            dsBatchList.Tables.Add(BatchList);
            #endregion

            #region Prepare Crystal Report Parameters
            Query = "Select So.SourceName,Sc.SchemeName,a.IndentNo,to_char(a.IndentDate,'dd/MM/yyyy')as IndentDate,a.StkRegNo as VoucherNo," +
                           " to_char(a.StkRegDate,'dd/MM/yyyy') as VoucherDate , m1.FacilityCode, m1.FacilityName," +
                           " Ft.FacilityTypeCode,m1.Address1,m1.Address2,m1.Address3,m1.City,m1.Zip,m1.Phone1,m1.Phone2,m1.Fax,m1.Email,Dist.DistrictName  " +
                           " from LPtbIndents a" +
                           " Inner Join masFacilities m1 on (m1.FacilityID = a.FacilityID)" +
                           " Inner Join MasSources So on (a.SourceID = So.SourceID)" +
                           " Inner Join MasSchemes Sc on (a.SchemeID = Sc.SchemeID)" +
                           " Inner Join MASFACILITYTYPES Ft on (m1.FacilityTypeID = Ft.FacilityTypeID)" +
                           " Inner Join MasDistricts Dist on (m1.DistrictID = Dist.DistrictID)" +
                           " Left Outer Join masAccYearSettings yr on (a.IndentDate between yr.StartDate and yr.EndDate)" +
                           " Where a.IndentID = " + IndentID;

            DataTable ParamTable = DBHelper.GetDataTable(Query);
            DataRow Row = ParamTable.Rows[0];

            List<Parameters> Params = new List<Parameters>();
            //Source and Scheme
            Params.Add(new Parameters("Param_Source", GenFunctions.CheckEmptyString(Row["SourceName"])));
            Params.Add(new Parameters("Param_Scheme", GenFunctions.CheckEmptyString(Row["SchemeName"])));

            //Facility
            Params.Add(new Parameters("Param_Code", GenFunctions.CheckEmptyString(Row["FacilityCode"])));
            Params.Add(new Parameters("Param_Facility", GenFunctions.CheckEmptyString(Row["FacilityName"])));
            Params.Add(new Parameters("Param_Address1", GenFunctions.CheckEmptyString(Row["Address1"])));
            Params.Add(new Parameters("Param_Address2", GenFunctions.CheckEmptyString(Row["Address2"])));
            Params.Add(new Parameters("Param_Address3", GenFunctions.CheckEmptyString(Row["Address3"])));
            Params.Add(new Parameters("Param_City", GenFunctions.CheckEmptyString(Row["City"])));
            Params.Add(new Parameters("Param_Zip", GenFunctions.CheckEmptyString(Row["Zip"])));
            Params.Add(new Parameters("Param_Phone1", GenFunctions.CheckEmptyString(Row["Phone1"])));
            Params.Add(new Parameters("Param_Phone2", GenFunctions.CheckEmptyString(Row["Phone2"])));
            Params.Add(new Parameters("Param_Email", GenFunctions.CheckEmptyString(Row["Email"])));
            Params.Add(new Parameters("Param_DistrictName", GenFunctions.CheckEmptyString(Row["DistrictName"])));

            //Indent
            Params.Add(new Parameters("Param_IndentNo", GenFunctions.CheckEmptyString(Row["IndentNo"])));
            Params.Add(new Parameters("Param_IndentDate", GenFunctions.CheckEmptyString(Row["IndentDate"])));
            Params.Add(new Parameters("Param_VoucherNo", GenFunctions.CheckEmptyString(Row["Voucherno"])));
            Params.Add(new Parameters("Param_VoucherDate", GenFunctions.CheckEmptyString(Row["Voucherdate"])));
            #endregion

            #region Crystal Report Addons
            List<SubReportDatasets> Subsets = GenFunctions.CrystalReports.ConfigureItemInfo(false);
            Subsets = GenFunctions.CrystalReports.ConfigureLPItemInfo(Subsets, true, "");
            Subsets.Add(new SubReportDatasets((isIssued) ? "ILBatchList.rpt" : "SuggBatchList.rpt", dsBatchList));
            #endregion

            #region Create Crystal Report Document
            ReportDocument Document = new ReportDocument();
            Document = GenFunctions.CrystalReports.GenerateReport(@"~/LocalPurchases/CrystalReport/SuggestionList.rpt", dsSuggestionList, Params, Subsets);
            #endregion

            return Document;
           

        }
        #endregion

    #region FillWarehousesByPoNo (By State)
        public static void FillWarehousesByPoNo(DropDownList ddl, string mStateID, string mPoNoID, bool AddAllWarehouses, bool SelectFirst)
        {
            if (mStateID == "") mStateID = "0";
            if (mPoNoID == "") mPoNoID = "0";
           // FillWarehouses(ddl, mStateID, AddAllWarehouses, SelectFirst, " and WarehouseID in (Select Distinct a.WarehouseID from soWHDist a Inner Join LPsoOrderedItems a1 on (a1.OrderItemID = a.OrderItemID) Inner Join masWarehouses m1 on (m1.WarehouseID = a.WarehouseID) Where a1.PoNoID = " + mPoNoID + " and m1.StateID = " + mStateID + ")");

            FillWarehouses(ddl, mStateID, AddAllWarehouses, SelectFirst, " and WarehouseID in (Select Distinct m2.WarehouseID from lpsoorderplaced a Inner Join LPsoOrderedItems a1 on (a1.ponoid = a.ponoid) inner join maspsas m2 on (a.psaid=m2.psaid) Inner Join masWarehouses m1 on (m1.WarehouseID = m2.WarehouseID) Where a1.PoNoID = " + mPoNoID + " and m1.StateID = " + mStateID + ")");


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
        #endregion

    #region FillAllSuppliers
        public static void FillALLSuppliers(DropDownList ddl, bool AddAllSuppliers, bool SelectFirst, string Conditions)
        {
            ddl.Items.Clear();
            //string strSQL = "Select LPSupplierID as SupplierID, SupplierName from LPmasSuppliers Where 1=1" + Conditions + " Order By SupplierName";

            string strSQL = "select case when SupplierID is null then 'Null' else To_Char(SupplierID) end || '/' || 'null' as SupplierID," +
                " SupplierNAME|| '-MOH' as SupplierNAME " +
                " from masSuppliers   " +
                " union" +
                " select 'null' || '/ ' || case when LPSupplierID is null then 'Null'  else To_Char(LPSupplierID) end as SupplierID, " +
                " SupplierName || '-LP'as SupplierNAME from lpmasSuppliers " +
                "  order by SupplierName ";

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

    #region FillLPItems
        public static void FillLPItems(DropDownList ddl, bool AddAllItems, bool SelectFirst)
        {
            FillLPItems(ddl, AddAllItems, SelectFirst, "");
        }

        public static void FillLPItems(DropDownList ddl, bool AddAllItems, bool SelectFirst, string CategoryID)
        {
            ddl.Items.Clear();
            string Conditions = "";
            if (CategoryID != "" && CategoryID != "0")
                Conditions += " and CategoryID = " + CategoryID;

            string strSQL = "Select LPItemID, ItemName || ' - (' || ItemCode || ')' as ItemName from LPmasItems Where 1=1 " + Conditions + " Order By ItemName";
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
        #endregion

    #region FillAllItems
        public static void FillAllItems(DropDownList ddl, bool AddAllItems, bool SelectFirst, string CategoryID)
        {
            ddl.Items.Clear();
            string Conditions = "";
            if (CategoryID != "" && CategoryID != "0")
                Conditions += " and CategoryID = " + CategoryID;

            string strSQL = "Select LPItemID as ItemID, ItemName || ' - (' || ItemCode || ')' as ItemName from LPmasItems Where 1=1 "
                + Conditions + " Order By ItemName";
            //string strSQL = "select case when ItemID is null then 'Null' else To_Char(ItemID) end || '/' || 'null' as ItemID,ITEMNAME|| '-MOH' as ItemName " +
            //             " from masitems "; // where 1=1 " + Conditions;
            //"union  " +
            //"select 'null' || '/ ' || case when LPItemID is null then 'Null'  else To_Char(LPItemID) end as ItemID,ItemName || '-LP' as ItemName " +
            //" from lpmasitems  order by ITEMNAME "; //where 1=1 " + Conditions + "

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

    #region Auto Generate Numbers
    #region WH-AutogenerateNumbers
        public static string AutoGenerateNumbers(string WarehouseID, bool IsReceipt, string mType)
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
    #endregion

    #region FillTranchesByPoNoID
    public static void FillTranchesByPoNoID(DropDownList ddl, string PoNoID, bool AddAllTranches, bool SelectFirst)
    {
        FillTranchesByPoNoID(ddl, PoNoID, AddAllTranches, SelectFirst, "");
    }
    public static void FillTranchesByPoNoID(DropDownList ddl, string PoNoID, bool AddAllTranches, bool SelectFirst, string WhereConditions)
    {
        ddl.Items.Clear();
        if (PoNoID == "") PoNoID = "0";
        string strFilters = "";
        strFilters += " and PoNoID = " + PoNoID;
        strFilters += WhereConditions;
        string strSQL = "Select DurationID, Duration as TDuration, DType, Tranche from LPsoTranches where 1=1" + strFilters + " Order By DType, Duration, Tranche";
        DataTable dt = DBHelper.GetDataTable(strSQL);
        ddl.DataTextField = "Tranche";
        ddl.DataValueField = "DurationID";
        ddl.DataSource = dt;
        ddl.DataBind();
        if (AddAllTranches)
            ddl.Items.Insert(0, new ListItem("All Tranches", "0"));
        if (SelectFirst && ddl.Items.Count > 0)
            ddl.SelectedIndex = 0;
    }
    #endregion

    #region  ReconcileIssueDate
    public static void ReconcileIssueDate(string PonoID)
    {
        string SoIssueDate = GenFunctions.CheckDBNullForDate(DBHelper.ExecuteScalarQuery("Select Nvl((Select To_Char(PoDate,'dd-mm-yyyy') SoIssueDate from LPsoOrderPlaced Where PoNoID = @PoNoID),Null) from dual".Replace("@PoNoID", PonoID)));
        if (SoIssueDate != "") { ReconcileIssueDate(PonoID, SoIssueDate,null); }
    }
    public static void ReconcileIssueDate(string PonoID, string SoIssueDate, Label lblMsg)
    {
        string mPoNoID = PonoID;
        string mSupplyStartDate = SoIssueDate;
        string mPoDate = "";
        string strSQLC = "Select PoDate from LPsoOrderPlaced Where PoNoID = " + mPoNoID;
        DataTable dtC = DBHelper.GetDataTable(strSQLC);
        if (dtC.Rows.Count > 0)
            mPoDate = GenFunctions.CheckDBNullForDate(dtC.Rows[0]["PoDate"]);

        string ErrMsg = "";
        ErrMsg += GenFunctions.CheckDateGreaterThanToday(mSupplyStartDate, "Supply Order Issue Date");
        ErrMsg += GenFunctions.CheckDateByComparison(mSupplyStartDate, "Supply Order Issue Date", mPoDate, "Supply order date", "", "");
        ErrMsg += GenFunctions.CheckDate(ref mSupplyStartDate, "Supply Order Issue Date", false);
        ErrMsg += GenFunctions.CheckNumber(ref mPoNoID, "Supply Order", false);
        if (lblMsg != null && ErrMsg != "")
        {
            lblMsg.Text = ErrMsg;
            lblMsg.ForeColor = Color.Red;
            return;
        }
        else
            lblMsg.Text = "";

        string strSQL = "Update LPsoOrderPlaced set SOIssueDate = " + mSupplyStartDate + " where PoNoID = " + mPoNoID;
        DBHelper.GetDataTable(strSQL);
        lblMsg.Text = "Saved successfully";
        lblMsg.ForeColor = Color.Green;
        if (SoIssueDate.Trim() == "")
        {
            strSQL = "Update LPsoTranches set ExpDate = null Where PoNoID = " + mPoNoID;
            DBHelper.GetDataTable(strSQL);
        }
        else
        {
            strSQL = "Update LPsoTranches set ExpDate = Case DType When 'D' then (" + mSupplyStartDate + " + Duration) When 'M' then Add_Months(" + mSupplyStartDate + ", Duration) else (" + mSupplyStartDate + "+(Duration*7)) End Where PoNoID = " + mPoNoID;
            DBHelper.GetDataTable(strSQL);
        }
    }
    #endregion

    #region CheckLPSupplyOrder
    public static bool CheckLPSupplyOrder(string mPoNoID, bool CheckSchedule, Label lblMsg)
    {
        string ErrMsg = "";
        if (mPoNoID == "0" || mPoNoID == "")
        {
            ErrMsg += "Invalid Local Supply Order. Try again<br />";
        }
        else
        {
            try
            {
                string strSQL = "Delete from LPsoWHDist Where NVL(AbsQty, 0) = 0 and OrderItemID in (Select OrderItemID from LPsoOrderedItems Where PoNoID = " + mPoNoID + ")";
                DBHelper.GetDataTable(strSQL);
                strSQL = "Delete from LPsoOrderDistribution Where NVL(AbsQty, 0) = 0 and OrderItemID in (Select OrderItemID from LPsoOrderedItems Where PoNoID = " + mPoNoID + ")";
                DBHelper.GetDataTable(strSQL);
                strSQL = "Delete from LPsoOrderedItems Where NVL(AbsQty, 0) = 0 and PoNoID = " + mPoNoID;
                DBHelper.GetDataTable(strSQL);

                strSQL = "Select a.OrderItemID, a.AbsQty from LPsoOrderedItems a Where a.PoNoID = " + mPoNoID;
                DataTable dt = DBHelper.GetDataTable(strSQL);
                if (dt.Rows.Count == 0)
                    ErrMsg += "Add atleast one item under items tab. Status cannot be changed.<br />";

                if (CheckSchedule)
                {
                    strSQL = "Select a.OrderItemID, a.AbsQty from LPsoOrderedItems a Left Outer Join LPSOORDERDISTRIBUTION b on (b.OrderItemID = a.OrderItemID) Where a.PoNoID = " + mPoNoID + " Group By a.OrderItemID, a.AbsQty Having (NVL(a.AbsQty, 0) = 0 or NVL(a.AbsQty, 0) != NVL(Sum(NVL(b.AbsQty, 0)), 0))";
                    dt = DBHelper.GetDataTable(strSQL);
                    if (dt.Rows.Count > 0)
                        ErrMsg += "All items should be distributed properly. Status cannot be changed.<br />";
                }

                strSQL = "Select Nvl((Select Count(*) from LPSoOrderPlaced So Inner Join LPSoOrderedItems SI on (SI.PoNoID =  So.PoNoID) Where SI.PoNoID = @PoNoID and So.AmendNo = 0 and So.Status = 'IN' and Not Exists (Select * from V_LPActiveContractActiveItems CI Where CI.ContractItemID = SI.ContractItemID)),0)Count from dual".Replace("@PoNoID", mPoNoID);
                decimal Count = (decimal)DBHelper.ExecuteScalarQuery(strSQL);

                ErrMsg += (Count > 0) ? "Expired contract items are found in this Supply Order.<br />" : "";

                strSQL = "Select Nvl((Select Sum(Nvl(SI.ItemValue,0)) from LPSoOrderPlaced So Inner Join LPSoOrderedItems SI on (SI.PoNoID =  So.PoNoID) Where SI.PoNoID = @PoNoID),0) SoValue from dual".Replace("@PoNoID", mPoNoID);
                decimal TotItemValue = (decimal)DBHelper.ExecuteScalarQuery(strSQL);

                ErrMsg += (TotItemValue > 0) ? "" : "Supply Order value should not be zero or negative.<br />";
            }
            catch (Exception Exp)
            {
                ErrMsg += Exp.Message + "<br />";
            }
        }

        if (ErrMsg != "")
        {
            lblMsg.Text = ErrMsg;
            lblMsg.ForeColor = Color.Red;
            return false;
        }
        else
            return true;
    }
    #endregion

    }
    #endregion

    #region LPIssues
    public class LPIssues
    {
        #region CheckAndCompleteIndent
        public static bool CheckAndCompleteIndent(string mIndentID, ref string ErrMsg, bool CheckVoucherAvailable)
        {
            ErrMsg = string.Empty;
            if (mIndentID.Trim() == string.Empty || mIndentID.Trim() == "0")
                ErrMsg += "Invalid Indent/Issue No.<br />";
            else
            {
                string strSQL = "Select a.IndentItemID, a.Allotted from LPtbIndentItems a Where a.IndentID = " + mIndentID;
                DataTable dt = DBHelper.GetDataTable(strSQL);
                if (dt.Rows.Count == 0)
                    ErrMsg += "Add atleast one item under items tab. Status cannot be changed.<br />";

                strSQL = "Select a.IndentItemID, a.Allotted from LPtbIndentItems a Left Outer Join LPtbOutwards b on (b.IndentItemID = a.IndentItemID) Where a.IndentID = " + mIndentID + " Group By a.IndentItemID, a.Allotted Having (NVL(a.Allotted, 0) = 0 or NVL(a.Allotted, 0) != NVL(Sum(NVL(b.IssueQty, 0)), 0))";
                dt = DBHelper.GetDataTable(strSQL);
                if (dt.Rows.Count > 0)
                    ErrMsg += "All items should have atleast one batch with quantity greater than zero. Status cannot be changed.<br />";

                strSQL = "Select a.IndentItemID, a.Allotted from LPtbIndentItems a Left Outer Join LPtbOutwards b on (b.IndentItemID = a.IndentItemID) Where a.IndentID = " + mIndentID + " Group By a.IndentItemID, a.Allotted Having NVL(a.Allotted, 0) != NVL(Sum(NVL(b.IssueQty, 0)), 0)";
                dt = DBHelper.GetDataTable(strSQL);
                if (dt.Rows.Count > 0)
                    ErrMsg += "Issue Qty and Total Batch Qty should be same. Status cannot be changed.<br />";

                strSQL = "Select i2.ItemID, m1.ItemName, i1.IndentDate, Max(r1.ReceiptDate) as ReceiptDate from LPtbIndents i1 Inner Join LPtbIndentItems i2 on (i2.IndentID = i1.IndentID) Inner Join LPtbOutWards i3 on (i3.IndentItemID = i2.IndentItemID) Inner Join LPtbReceiptBatches r3 on (r3.InwNo = i3.InwNo) Inner Join LPtbReceiptItems r2 on (r2.ReceiptItemID = r3.ReceiptItemID) Inner Join LPtbReceipts r1 on (r1.ReceiptID = r2.ReceiptID) Inner Join masItems m1 on (m1.ItemID = i2.ItemID) Where 1=1 and i1.IndentID = " + mIndentID + " GROUP BY i2.ItemID, m1.ItemName, i1.IndentDate Having i1.IndentDate < Max(r1.ReceiptDate)";
                dt = DBHelper.GetDataTable(strSQL);
                if (dt.Rows.Count > 0)
                    ErrMsg += "Issue date should be greater than the batch receipt date. Check Item " + dt.Rows[0]["ItemName"].ToString() + ". Status cannot be changed.<br />";

                if (CheckVoucherAvailable)
                {
                    strSQL = "Select StkRegDate, StkRegNo from LPtbIndents Where (stkRegNo is null or stkRegDate is null) and IndentID = " + mIndentID;
                    dt = DBHelper.GetDataTable(strSQL);
                    if (dt.Rows.Count > 0)
                        ErrMsg += "Voucher No. and Voucher Date. should not be empty and should be saved before completing.<br />";
                }
            }
            if (ErrMsg == string.Empty)
            {
                string mIssueDate = GenFunctions.CheckDBNullForDate(DateTime.Now);
                GenFunctions.CheckDate(ref mIssueDate, "Issue Date", false);
                string strQuery = "Update LPtbIndents set Status = 'C', IssueDate = " + mIssueDate + " Where IndentID = " + mIndentID;
                DBHelper.ExecuteNonQuery(strQuery);
                strQuery = "Update LPtbOutWards set Status = 'C', Issued = 1 Where IndentItemID in (Select IndentItemID from LPtbIndentItems Where IndentID = " + mIndentID + ")";
                DBHelper.ExecuteNonQuery(strQuery);
                strQuery = "Update LPtbIndentItems set Status = 'C' Where IndentID = " + mIndentID;
                DBHelper.ExecuteNonQuery(strQuery);
                strQuery = "Update LPtbOutwards set Issued = 1 Where IndentItemID in (Select IndentItemID from tbIndentItems Where IndentID = " + mIndentID + ")";
                DBHelper.ExecuteNonQuery(strQuery);
                return true;
            }
            else
                return false;
        }
        #endregion

        #region DeleteIndent
        public static bool DeleteIndent(string mIndentID, ref string ErrMsg)
        {
            ErrMsg = string.Empty;
            if (mIndentID.Trim() == string.Empty || mIndentID.Trim() == "0")
                ErrMsg += "Invalid Indent/Issue No.<br />";
            else
            {
                try
                {
                    string strSQL = "Delete from LPtbOutwards Where IndentItemID in (Select IndentItemID from LPtbIndentItems Where IndentID = " + mIndentID + ")";
                    DBHelper.GetDataTable(strSQL);
                    strSQL = "Delete from LPtbIndentItems where IndentID = " + mIndentID;
                    DBHelper.GetDataTable(strSQL);
                    strSQL = "Delete from LPtbIndents where IndentID = " + mIndentID;
                    DBHelper.GetDataTable(strSQL);
                }
                catch
                {
                    ErrMsg += "Delete not allowed, references found.<br />";
                }
            }
            return (ErrMsg == string.Empty);
        }
        #endregion

        #region DeleteOutward Local Purchase
        //public static bool LPDeleteOutward(string mOutwardID, ref string ErrMsg)
        //{
        //    ErrMsg = string.Empty;
        //    if (mOutwardID.Trim() == string.Empty || mOutwardID.Trim() == "0")
        //        ErrMsg += "Invalid Outward/Issue No.<br />";
        //    else
        //    {
        //        try
        //        {
        //            string strSQL = "Delete from LPtbOutwards Where OutwardItemID in (Select OutwardItemID from LPtbOutwardItems Where OutwardID = " + mOutwardID + ")";
        //            DBHelper.GetDataTable(strSQL);
        //            strSQL = "Delete from LPtbOutwardItems where OutwardID = " + mOutwardID;
        //            DBHelper.GetDataTable(strSQL);
        //            strSQL = "Delete from LPtbOutwards where OutwardID = " + mOutwardID;
        //            DBHelper.GetDataTable(strSQL);
        //        }
        //        catch
        //        {
        //            ErrMsg += "Delete not allowed, references found.<br />";
        //        }
        //    }
        //    return (ErrMsg == string.Empty);
        //}
        #endregion

        #region LPIssueAllExpired
        public static void LPIssueAllExpired()
        {
            string strSQL = "Select Distinct SourceID, SchemeID, WarehouseID, IndentDate as MonYr from v_LPExpiredItemsInStock";
            DataTable dt = DBHelper.GetDataTable(strSQL);
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    #region Data to be stored
                    //string mIndentID = string.Empty;
                    string mSourceID = GenFunctions.CheckEmptyString(dr["SourceID"], "0");
                    string mSchemeID = GenFunctions.CheckEmptyString(dr["SchemeID"], "0");
                    string mWarehouseID = GenFunctions.CheckEmptyString(dr["WarehouseID"], "0");
                    string mIssueType = "EX";
                    string mIndentNo = GenFunctions.LocalPurchase.AutoGenerateNumbers(mWarehouseID, false, mIssueType);
                    //string mIndentDate = GenFunctions.CheckDBNullForDate(dr["MonYr"]);
                    //string ErrMsg = "";
                    Broadline.WMS.Data.DBHelperExtended.sp_LPIssueExpiredBatches((decimal)dr["SourceID"], (decimal)dr["SchemeID"], (decimal)dr["WarehouseID"], mIndentNo, (DateTime)dr["MonYr"], mIssueType);
                    #endregion
                }
            }
        }
        #endregion

        #region CheckAndComplete LP Indent
        public static bool CheckAndCompleteLPIndent(string mIndentID, ref string ErrMsg, bool CheckVoucherAvailable)
        {
            ErrMsg = string.Empty;
            if (mIndentID.Trim() == string.Empty || mIndentID.Trim() == "0")
                ErrMsg += "Invalid Indent/Issue No.<br />";
            else
            {
                string strSQL = "Select a.IndentItemID, a.Allotted from LPtbIndentItems a Where a.IndentID = " + mIndentID;
                DataTable dt = DBHelper.GetDataTable(strSQL);
                if (dt.Rows.Count == 0)
                    ErrMsg += "Add atleast one item under items tab. Status cannot be changed.<br />";

                strSQL = "Select a.IndentItemID, a.Allotted from LPtbIndentItems a Left Outer Join LPtbOutwards b on (b.IndentItemID = a.IndentItemID) Where a.IndentID = " + mIndentID + " Group By a.IndentItemID, a.Allotted Having (NVL(a.Allotted, 0) = 0 or NVL(a.Allotted, 0) != NVL(Sum(NVL(b.IssueQty, 0)), 0))";
                dt = DBHelper.GetDataTable(strSQL);
                if (dt.Rows.Count > 0)
                    ErrMsg += "All items should have atleast one batch with quantity greater than zero. Status cannot be changed.<br />";

                strSQL = "Select a.IndentItemID, a.Allotted from LPtbIndentItems a Left Outer Join LPtbOutwards b on (b.IndentItemID = a.IndentItemID) Where a.IndentID = " + mIndentID + " Group By a.IndentItemID, a.Allotted Having NVL(a.Allotted, 0) != NVL(Sum(NVL(b.IssueQty, 0)), 0)";
                dt = DBHelper.GetDataTable(strSQL);
                if (dt.Rows.Count > 0)
                    ErrMsg += "Issue Qty and Total Batch Qty should be same. Status cannot be changed.<br />";
                //
                strSQL = "Select i2.ItemID, m1.ItemName, i1.IndentDate, Max(r1.ReceiptDate) as ReceiptDate from LPtbIndents i1 Inner Join LPtbIndentItems i2 on (i2.IndentID = i1.IndentID) Inner Join LPtbOutWards i3 on (i3.IndentItemID = i2.IndentItemID) Inner Join LPtbReceiptBatches r3 on (r3.InwNo = i3.InwNo) Inner Join LPtbReceiptItems r2 on (r2.ReceiptItemID = r3.ReceiptItemID) Inner Join LPtbReceipts r1 on (r1.ReceiptID = r2.ReceiptID) Inner Join masItems m1 on (m1.ItemID = i2.ItemID) Where 1=1 and i1.IndentID = " + mIndentID + " GROUP BY i2.ItemID, m1.ItemName, i1.IndentDate Having i1.IndentDate < Max(r1.ReceiptDate)";
                dt = DBHelper.GetDataTable(strSQL);
                if (dt.Rows.Count > 0)
                    ErrMsg += "Issue date should be greater than the batch receipt date. Check Item " + dt.Rows[0]["ItemName"].ToString() + ". Status cannot be changed.<br />";

                if (CheckVoucherAvailable)
                {
                    strSQL = "Select StkRegDate, StkRegNo from LPtbIndents Where (stkRegNo is null or stkRegDate is null) and IndentID = " + mIndentID;
                    dt = DBHelper.GetDataTable(strSQL);
                    if (dt.Rows.Count > 0)
                        ErrMsg += "Voucher No. and Voucher Date. should not be empty and should be saved before completing.<br />";
                }
            }
            if (ErrMsg == string.Empty)
            {
                string mIssueDate = GenFunctions.CheckDBNullForDate(DateTime.Now);
                GenFunctions.CheckDate(ref mIssueDate, "Issue Date", false);
                string strQuery = "Update LPtbIndents set Status = 'C', IssueDate = " + mIssueDate + " Where IndentID = " + mIndentID;
                DBHelper.ExecuteNonQuery(strQuery);
                strQuery = "Update LPtbOutWards set Status = 'C', Issued = 1 Where IndentItemID in (Select IndentItemID from tbIndentItems Where IndentID = " + mIndentID + ")";
                DBHelper.ExecuteNonQuery(strQuery);
                strQuery = "Update LPtbIndentItems set Status = 'C' Where IndentID = " + mIndentID;
                DBHelper.ExecuteNonQuery(strQuery);
                //string strQuery = "Update tbOutwards set Issued = 1 Where IndentItemID in (Select IndentItemID from tbIndentItems Where IndentID = " + mIndentID + ")";
                //DBHelper.ExecuteNonQuery(strQuery);
                return true;
            }
            else
                return false;
        }
        #endregion
                
    }
    #endregion

    #region LPStock
    public class LPStock
    {

        #region GetCurStock
        public static double GetCurStock(string mWarehouseID, string mSourceID, string mSchemeID, int? mItemID,bool IsLocalItem)
        {
            string wherecondition = "";
            if (IsLocalItem)
            {
                wherecondition = "  b.LPItemID = " + mItemID;
            }
            else
            {
                wherecondition = "   b.ItemID = " + mItemID;
            }

            string strSQL = "Select nvl(Sum(NVL(b.CurBal, 0) - NVL(b.Reserved, 0)),0) as CurBal" +
                " from V_LPCURSTOCKAVAILABLE b" +
                " Where b.WarehouseID=" + mWarehouseID +
                "   and b.SourceID=" + mSourceID +
                "   and b.SchemeID=" + mSchemeID +
                "   and " + wherecondition;
            
            DataTable dt = DBHelper.GetDataTable(strSQL);
            if (dt.Rows.Count > 0 && dt.Rows[0]["CurBal"] != DBNull.Value)
                return double.Parse(dt.Rows[0]["CurBal"].ToString());
            else
                return 0;
        }
        #endregion

        #region UpdateAllotQty
        public static void UpdateAllotQty(string WarehouseID, string SourceID, string SchemeID, string ItemID,string LPItemID, double AllotQty, string IndentItemID,bool IsLocalItem)
        {
            //Allow to distribute to facility only when QAStatus=pass from batches (LPtbReceiptBatches)
            string Item = string.Empty;
            if (IsLocalItem)
            {
                Item = " a.LPItemID = " + LPItemID;
            }
            else
            {
                Item = " a.ItemID = " + ItemID;
            }


            string WhereConditions = "  and (b.ExpDate >= SysDate or nvl(b.ExpDate,SysDate) >= SysDate)";  //b.QAStatus = 1 and 

            #region Delete existing allotements (tbOutwards)
            string strSQL = "Delete from LPtbOutWards where IndentItemID = " + IndentItemID;
            DBHelper.ExecuteNonQuery(strSQL);
            #endregion

            #region Insert new allotements (tbOutwards)
            double TotAllotQty = AllotQty;
            strSQL = "Select b.InwNo, b.BatchNo, b.MfgDate, b.ExpDate, NVL(b.AbsRQty, 0) as AbsRQty, NVL(b.AllotQty, 0) as AllotQty" +
                " from LPtbReceiptBatches b" +
                " Inner Join LPtbReceiptItems a on (a.ReceiptItemID = b.ReceiptItemID)" +
                " Inner Join LPtbReceipts a1 on (a1.ReceiptID = a.ReceiptID)" +
                " Where a1.WarehouseID = " + WarehouseID + " and a1.SourceID = " + SourceID + " and a1.SchemeID = " + SchemeID +
                "   and " + Item + WhereConditions + " and NVL(b.AbsRQty, 0) - NVL(b.AllotQty, 0) >= 0 and b.BatchStatus='R'" +
                " Order By b.ExpDate, b.AbsRQty";
            DataTable dt = DBHelper.GetDataTable(strSQL);

            foreach (DataRow dr in dt.Rows)
            {
                #region Calculate Allotted Qty
                double dblCurAllot = 0;
                double dblAbsRQty = double.Parse(dr["AbsRQty"].ToString());
                double dblAllotQty = double.Parse(dr["AllotQty"].ToString());
                if (dblAbsRQty - dblAllotQty >= TotAllotQty)
                    dblCurAllot = TotAllotQty;
                else
                    dblCurAllot = dblAbsRQty - dblAllotQty;
                TotAllotQty -= dblCurAllot;
                #endregion

                #region Data to be Saved
                string mIndentItemID = IndentItemID;
                string mItemID = ItemID;
                string mLPItemID = LPItemID;
                string mInwNo = dr["InwNo"].ToString();
                string mIssueQty = dblCurAllot.ToString();
                string mIssuePrice = "0";
                string mIssued = "0";
                #endregion

                #region Validate Data
                GenFunctions.CheckNumber(ref mIndentItemID, "", false);
                GenFunctions.CheckNumber(ref mItemID, "", false);
                GenFunctions.CheckNumber(ref mLPItemID, "", false);
                GenFunctions.CheckNumber(ref mInwNo, "", false);
                //GenFunctions.CheckStringForEmpty(ref mIssueType, "", false);
                //GenFunctions.CheckDate(ref mIssueDate, "", false);
                GenFunctions.CheckNumber(ref mIssueQty, "", false);
                GenFunctions.CheckNumber(ref mIssuePrice, "", false);
                GenFunctions.CheckNumber(ref mIssued, "", false);
                #endregion  

                #region Save Data
                strSQL = "Insert into LPtbOutWards (IndentItemID, ItemID,LPItemID, InwNo, IssueQty, IssuePrice, Issued) values (" +
                    mIndentItemID + ", " + mItemID + "," + mLPItemID + ", " + mInwNo + ", " + mIssueQty + ", " + mIssuePrice + ", " + mIssued + ")";
                DBHelper.GetDataTable(strSQL);
                #endregion

                if (TotAllotQty == 0) break;
            }
            #endregion
        }
        #endregion
    }
    #endregion

}
