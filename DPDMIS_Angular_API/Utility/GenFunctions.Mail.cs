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
using System.Net.Mail;
using System.Net;
using System.IO;
using System.ComponentModel;
using System.Threading;
using System.Collections.Generic;
#endregion

/// <summary>
/// Mail sending functions
/// </summary>
public partial class GenFunctions
{
    #region Mail Sender
    public class Mail
    {
        public delegate void UserSendCompleted(object sender,AsyncCompletedEventArgs e);
        
        #region Send Mail - Without Attachments
        public static bool SendMail(MailAddress[] To, MailAddress[] CC, MailAddress[] Bcc, string Subject, string BodyHTML, MailLog CurrentLog)
        {
            return SendMail(To, CC, Bcc, Subject, BodyHTML,CurrentLog,null,null,false);
        }
        #endregion

        #region Send Mail - With Attachments
        public static bool SendMail(MailAddress[] To, MailAddress[] CC, MailAddress[] Bcc, string Subject, string BodyHTML, FileAttachment[] Files, MailLog CurrentLog)
        {
            return SendMail(To, CC, Bcc, Subject, BodyHTML, Files, CurrentLog, null,null,false);
        }
        #endregion

        #region Send Mail Asynchronously - Without Attachments
        public static bool SendMail(MailAddress[] To, MailAddress[] CC, MailAddress[] Bcc, string Subject, string BodyHTML, MailLog CurrentLog, SendCompletedEventHandler OnSendCompleted, object UserState, bool UseAsyncSender)
        {
            byte[] data = new byte[0];
            FileAttachment[] Files = new FileAttachment[1];
            Files[0] = new FileAttachment("", data, "");

            return SendMail(To, CC, Bcc, Subject, BodyHTML, Files, CurrentLog, OnSendCompleted, UserState, UseAsyncSender);
        }
        #endregion

        #region Send Mail Asynchronously - With PDF Attachments
        public static bool SendMail(MailAddress[] To, MailAddress[] CC, MailAddress[] Bcc, string Subject, string BodyHTML, FileAttachment[] Files, MailLog CurrentLog, SendCompletedEventHandler OnSendCompleted, object UserState,bool UseAsyncSender)
        {
            #region Variables
            string FromName = (ConfigurationManager.AppSettings["FromName"] != null &&
                ConfigurationManager.AppSettings["FromName"] != "" ? ConfigurationManager.AppSettings["FromName"] : "Web Admin");

            string FromEmail = (ConfigurationManager.AppSettings["FromEmail"] != null &&
                ConfigurationManager.AppSettings["FromEmail"] != "" ? ConfigurationManager.AppSettings["FromEmail"] : "cgmsc.medicines@gmail.com");

            string Password = (ConfigurationManager.AppSettings["Password"] != null &&
                ConfigurationManager.AppSettings["Password"] != "" ? ConfigurationManager.AppSettings["Password"] : "cgmsc2013");

            string HostName = (ConfigurationManager.AppSettings["HostName"] != null &&
                ConfigurationManager.AppSettings["HostName"] != "" ? ConfigurationManager.AppSettings["HostName"] : "smtp.gmail.com");

            string PortNo = (ConfigurationManager.AppSettings["PortNo"] != null &&
                ConfigurationManager.AppSettings["PortNo"] != "" ? ConfigurationManager.AppSettings["PortNo"] : "25");
            
            SmtpClient client = new SmtpClient();
            
            MailMessage mMsg = new MailMessage();
            #endregion

            MailAddress fromAddr = new MailAddress(FromEmail, FromName);
            
            client.Host = HostName;
            client.Port = int.Parse(PortNo);
            // Added For Gmail
            client.EnableSsl = true; 

            mMsg.From = fromAddr;

            #region Add ToAddresses
            foreach (MailAddress ToAddr in To)
            {
                if (ToAddr != null)
                {
                    mMsg.To.Add(ToAddr); 
                }
            }
            #endregion

            #region Add CcAddresses
            foreach (MailAddress CCAddr in CC)
            {
                if (CCAddr != null)
                {
                    mMsg.CC.Add(CCAddr);
                }
            }
            #endregion

            #region Add BccAddresses
            foreach (MailAddress BccAddr in Bcc)
            {
                if (BccAddr != null)
                {
                    mMsg.Bcc.Add(BccAddr);
                }
            }
            #endregion

            #region Attach Files
            foreach (FileAttachment File in Files)
            {
                if (File.AttachedFile.Length > 0)
                {
                    Attachment AttachThisFile = new Attachment(new MemoryStream(File.AttachedFile), File.AttachedFileName, File.AttachedFileType);
                    mMsg.Attachments.Add(AttachThisFile);
                }
            }
            #endregion

            mMsg.Subject = Subject;
            mMsg.IsBodyHtml = true;
            mMsg.Body = BodyHTML;

            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(FromEmail, Password);

            CurrentLog.To = mMsg.To.ToString();
            CurrentLog.Bcc = mMsg.Bcc.ToString();
            CurrentLog.Cc = mMsg.CC.ToString();

            #region Anonymous OnSendComplete
            UserSendCompleted AnonymousOnSendCompleted = delegate(object VirtualSender, AsyncCompletedEventArgs AsyncArg)
            {
                if (AsyncArg.Cancelled || AsyncArg.Error != null)
                {
                    #region On Sending Failed
                    CurrentLog.Status = "F";
                    #endregion
                }
                else
                {
                    #region On Sent Successfully
                    if(OnSendCompleted != null)
                    OnSendCompleted(null, new AsyncCompletedEventArgs(null, false, AsyncArg.UserState));
                    CurrentLog.Status = "S";
                    #endregion
                }

                #region Logging Of Sent Mail
                try
                {
                    GenFunctions.Mail.LogMail(CurrentLog);
                }
                catch (System.Data.OracleClient.OracleException LogError)
                {
                    //GenFunctions.AlertOnLoad(this, "Mail was send but not logged");
                }
                #endregion
            };
            #endregion

            if (UseAsyncSender)
            {
                #region Async eMail Sending
                #region Logging Mail on SendCompleted
                client.SendCompleted += new SendCompletedEventHandler(AnonymousOnSendCompleted);
                #endregion

                Thread SendAsync = new Thread(delegate() { client.SendAsync(mMsg, UserState); });
                SendAsync.Start();
                #endregion
            }
            else
            {
                #region Sync eMail Sending

                client.Send(mMsg);

                #region Logging Mail on SendCompleted
                AnonymousOnSendCompleted(null, new AsyncCompletedEventArgs(null, false, UserState));
                #endregion

                #endregion
            }

            return true;
        }
        #endregion

        #region PDF Sent Log - After Send Mail
        public static void SaveLogAfterPDFSent(string SupplierID, string Subject, string Remarks)
        {
        //    #region Data to be stored
        //    string UserID = "";
        //    if (HttpContext.Current.Session["MemberID"] != null)
        //    {
        //        UserID = HttpContext.Current.Session["MemberID"].ToString();
        //    }
        //    string IPAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
        //    string SentDate = DateTime.Now.ToString("dd-MM-yyyy");
        //    #endregion

        //    #region Validate Data
        //    string ErrMsg = "";
        //    ErrMsg += GenFunctions.CheckNumber(ref UserID, "From User", false);
        //    ErrMsg += GenFunctions.CheckNumber(ref SupplierID, "Target User", true);
        //    ErrMsg += GenFunctions.CheckStringForEmpty(ref IPAddress, "User IP Address", false);
        //    ErrMsg += GenFunctions.CheckStringForEmpty(ref Subject, "Subject", false);
        //    ErrMsg += GenFunctions.CheckStringForEmpty(ref Remarks, "Remarks", true);
        //    ErrMsg += GenFunctions.CheckDate(ref SentDate, "Sent date", false);

        //    if (ErrMsg != "")
        //    {
        //        throw new Exception(ErrMsg);
        //    }
        //    #endregion

        //    #region Save Data
        //    string strSQL = "Insert into soPDFSentLog (SentDate,IPAddress,UserID,SupplierID,Subject,Remarks)" +
        //        " values (" + SentDate + "," + IPAddress + "," + UserID + "," + SupplierID + "," + Subject + "," + Remarks + ")";
        //    DBHelper.GetDataTable(strSQL);
        //    #endregion
        }
        #endregion

        #region LogMail
        public static void LogMail(MailLog CurMailLog)
        {
            string strSQL = "Insert into SOMailLogs (PONOID,PSAID,SUPPLIERID,SOFILEID,SENTDATE,FROMEMAILID,TOEMAILID,BCC,CC,SENTBY,IPADDRESS,STATUS)" +
                                             " Values (" +
                                                         GenFunctions.CheckDBNull(CurMailLog.PonoID,"null") + "," +
                                                         GenFunctions.CheckDBNull(CurMailLog.PsaID, "null") + "," +
                                                         GenFunctions.CheckDBNull(CurMailLog.SupplierID, "null") + "," +
                                                         GenFunctions.CheckDBNull(CurMailLog.SoFileID, "null") + "," +
                                                         "to_date('" + CurMailLog.SentDate.ToString("dd/MM/yyyy") + "','dd/MM/yyyy')" + "," +
                                                         "'" + CurMailLog.FromEmail + "'," +
                                                         "'" + CurMailLog.To + "'," +
                                                         "'" + CurMailLog.Bcc + "'," +
                                                         "'" + CurMailLog.Cc + "'," +
                                                         "" + CurMailLog.SentBy.ToString() + "," +
                                                         "'" + CurMailLog.IPAddress + "'," +
                                                         "'" + CurMailLog.Status + "')";

                
            DBHelper.GetDataTable(strSQL);
        }
        #endregion

        #region MailLog
        public class MailLog
        {
            #region Variables
            private int? iPonoID = null;
            private int? iPsaID = null;
            private int? iSupplierID = null;
            private int? iSoFileID = null;
            private string iFromEmail;
            private string iTo;
            private string iBcc;
            private string iCc;
            private DateTime iSentDate;
            private string iIPAddress;
            private string iStatus;
            private int iSentBy;
            #endregion

            #region Properties

            #region PonoID
            public int? PonoID
            {
                get
                {
                    return iPonoID;
                }
                set
                {
                    iPonoID = value;
                }
            }
            #endregion

            #region PsaID
            public int? PsaID
            {
                get
                {
                    return iPsaID;
                }
                set
                {
                    iPsaID = value;
                }
            }
            #endregion

            #region SupplierID
            public int? SupplierID
            {
                get
                {
                    return iSupplierID;
                }
                set
                {
                    iSupplierID = value;
                }
            }
            #endregion

            #region SoFileID
            public int? SoFileID
            {
                get
                {
                    return iSoFileID;
                }
                set
                {
                    iSoFileID = value;
                }
            }
            #endregion

            #region FromEmail
            public string FromEmail
            {
                get
                {
                    return iFromEmail;
                }
                set
                {
                    iFromEmail = value;
                }
            }
            #endregion

            #region To
            public string To
            {
                get
                {
                    return iTo;
                }
                set
                {
                    iTo = value;
                }
            }
            #endregion

            #region Bcc
            public string Bcc
            {
                get
                {
                    return iBcc;
                }
                set
                {
                    iBcc = value;
                }
            }
            #endregion

            #region Cc
            public string Cc
            {
                get
                {
                    return iCc;
                }
                set
                {
                    iCc = value;
                }
            }
            #endregion

            #region SentDate
            public DateTime SentDate
            {
                get
                {
                    return iSentDate;
                }
                set
                {
                    iSentDate = value;
                }
            }
            #endregion

            #region SentBy
            public int SentBy
            {
                get
                {
                    return iSentBy;
                }
                set
                {
                    iSentBy = value;
                }
            }
            #endregion

            #region IPAddress
            public string IPAddress
            {
                get
                {
                    return iIPAddress;
                }
                set
                {
                    iIPAddress = value;
                }
            }
            #endregion

            #region Status
            public string Status
            {
                get
                {
                    return iStatus;
                }
                set
                {
                    iStatus = value;
                }
            }
            #endregion

            #endregion

            #region Constructer
            public MailLog()
            {
                SentDate = DateTime.Now;
                iIPAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString();
                iStatus = "F";
            }
            public MailLog(string IPAddress)
            {
                SentDate = DateTime.Now;
                iIPAddress = IPAddress;
                iStatus = "F";
            }

            public MailLog(int? PonoID, int? PsaID, int? SupplierID, int? SoFileID, string FromEmail, string To, string Bcc, string Cc,string Status,int SentBy)
            {
                iPonoID = PonoID;
                iPsaID = PsaID;
                iSupplierID = SupplierID;
                iSoFileID = SoFileID;
                iFromEmail = FromEmail;
                iTo = To;
                iBcc = Bcc;
                iCc = Cc;
                SentDate = DateTime.Now;
                iIPAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString();
                iStatus = Status;
                iSentBy = SentBy;
            }
            #endregion
        }
        #endregion

        #region FileAttachment
        public class FileAttachment
        {
            #region Variables
            private string iAttachedFileName;
            private byte[] iAttachedFile;
            private string iAttachedFileType;
            #endregion

            #region Properties

            #region AttachedFileName
            public string AttachedFileName
            {
                get
                {
                    return iAttachedFileName;
                }
                set
                {
                    iAttachedFileName = value;
                }
            }
            #endregion

            #region AttachedFile
            public byte[] AttachedFile
            {
                get
                {
                    return iAttachedFile;
                }
                set
                {
                    iAttachedFile = value;
                }
            }
            #endregion

            #region AttachedFileType
            public string AttachedFileType
            {
                get
                {
                    return iAttachedFileType;
                }
                set
                {
                    iAttachedFileType = value;
                }
            }
            #endregion

            #endregion

            #region Constructer
            public FileAttachment(string AttachedFileName, byte[] BinaryContent, string AttachedFileType)
            {
                iAttachedFileName = AttachedFileName;
                iAttachedFile = BinaryContent;
                iAttachedFileType = AttachedFileType;
            }
            #endregion
        }
        #endregion
    }
    #endregion
}
