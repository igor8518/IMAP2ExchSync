using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections;
using System.ComponentModel;
using System.Net.Sockets;
using System.IO;
using System.Net.Security;
using System.Net.Mime;
using System.Text.RegularExpressions;

namespace IMAP2ExchSync
{
    public class Mx
    {
        public Mx()
        {
        }

        [DllImport("dnsapi", EntryPoint = "DnsQuery_W", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
        private static extern int DnsQuery([MarshalAs(UnmanagedType.VBByRefStr)]ref string pszName, QueryTypes wType, QueryOptions options, int aipServers, ref IntPtr ppQueryResults, int pReserved);

        [DllImport("dnsapi", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern void DnsRecordListFree(IntPtr pRecordList, int FreeType);

        public static string[] GetMXRecords(string domain)
        {

            IntPtr ptr1 = IntPtr.Zero;
            IntPtr ptr2 = IntPtr.Zero;
            MXRecord recMx;
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
            {
                throw new NotSupportedException();
            }
            ArrayList list1 = new ArrayList();
            int num1 = Mx.DnsQuery(ref domain, QueryTypes.DNS_TYPE_MX, QueryOptions.DNS_QUERY_BYPASS_CACHE, 0, ref ptr1, 0);
            if (num1 != 0)
            {
                throw new Win32Exception(num1);
            }
            for (ptr2 = ptr1; !ptr2.Equals(IntPtr.Zero); ptr2 = recMx.pNext)
            {
                recMx = (MXRecord)Marshal.PtrToStructure(ptr2, typeof(MXRecord));
                if (recMx.wType == 15)
                {
                    string text1 = Marshal.PtrToStringAuto(recMx.pNameExchange);
                    list1.Add(text1);
                }
            }
            Mx.DnsRecordListFree(ptr2, 0);
            return (string[])list1.ToArray(typeof(string));
        }

        private enum QueryOptions
        {
            DNS_QUERY_ACCEPT_TRUNCATED_RESPONSE = 1,
            DNS_QUERY_BYPASS_CACHE = 8,
            DNS_QUERY_DONT_RESET_TTL_VALUES = 0x100000,
            DNS_QUERY_NO_HOSTS_FILE = 0x40,
            DNS_QUERY_NO_LOCAL_NAME = 0x20,
            DNS_QUERY_NO_NETBT = 0x80,
            DNS_QUERY_NO_RECURSION = 4,
            DNS_QUERY_NO_WIRE_QUERY = 0x10,
            DNS_QUERY_RESERVED = -16777216,
            DNS_QUERY_RETURN_MESSAGE = 0x200,
            DNS_QUERY_STANDARD = 0,
            DNS_QUERY_TREAT_AS_FQDN = 0x1000,
            DNS_QUERY_USE_TCP_ONLY = 2,
            DNS_QUERY_WIRE_ONLY = 0x100
        }

        private enum QueryTypes
        {
            DNS_TYPE_MX = 15
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MXRecord
        {
            public IntPtr pNext;
            public string pName;
            public short wType;
            public short wDataLength;
            public int flags;
            public int dwTtl;
            public int dwReserved;
            public IntPtr pNameExchange;
            public short wPreference;
            public short Pad;
        }
    }


    public abstract class MailServer
    {

        static string[] mthString = { "", "jan", "feb", "mar", "apr", "may", "jun", "jul", "aug", "sep", "oct", "nov", "dec" };
        public static string getDateString(DateTime dateTime)
        {
            int year = dateTime.Year;
            if (dateTime.Year > DateTime.Now.Year)
            {
                // year = DateTime.Now.Year;
            }
            return dateTime.Day + "-" + mthString[dateTime.Month] + "-" + year;
        }
        protected void TcpClientTimeoutConnect(TcpClient tc, string server, int port, int timemout)
        {
            IAsyncResult ar = tc.BeginConnect(server, port, null, null);
            System.Threading.WaitHandle wh = ar.AsyncWaitHandle;
            try
            {
                if (!ar.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(timemout * 0.001), false))
                {
                    tc.Close();
                    throw new TimeoutException();
                }
                tc.EndConnect(ar);
            }
            finally
            {
                wh.Close();
            }
        }

        protected const int connectTimeout = 5000;
        protected const int readTimeout = 100000;
        protected const int writeTimeout = 100000;

        protected string server = null;
        protected int port = 0;
        protected TcpClient client = null;
        protected NetworkStream stream = null;
        protected StreamReaderWithLog reader = null;
        protected StreamWriterWithLog writer = null;
        protected BinaryReaderS readerB = null;
        protected BinaryWriter writerB = null;

        private readonly Action<int, string> logger;

        protected void Log(int level, string message)
        {
            if (logger != null)
                logger(level, message);
        }

        public MailServer(Action<int, string> logger = null)
        {
            this.logger = logger;
        }

        virtual public void Open()
        {
            try
            {
                Close();

                client = new TcpClient();
                TcpClientTimeoutConnect(client, server, port, connectTimeout);
                stream = client.GetStream();

                Log(22, String.Format("Mail server connection opened"));
            }
            catch
            {
                if (stream != null)
                    stream.Dispose();
                stream = null;

                if (client != null)
                    client.Close();
                client = null;

                throw;
            }
            //return "";
        }

        virtual public bool GetMessageIDs(DateTime fromTime, DateTime toTime, string mailBox, Action<MailMessageIDs> messageAction)
        {
            return false;
        }
        virtual public bool GetMessageIDs(string lastUID, string mailBox, Action<MailMessageIDs> messageAction)
        {
            return false;
        }
        virtual public string DownloadMessage(string UID, string mailBox, long StartOctet, Func<byte[], string> messageAction)
        {
            return "";
        }



        virtual public void Close()
        {
            if (stream != null)
                stream.Dispose();
            stream = null;

            if (client != null)
            {
                client.Close();
                Log(22, String.Format("Mail server connection closed"));
            }
            client = null;
            //return "";
        }

        abstract public void Send(string fromAddress, string toAddress, string folderName, string fileToSend, string addHeader,string UID,string Size);
        
    }


    public class ImapServer : MailServer
    {
        private readonly string userName;
        private readonly string password;

        private SslStream sslStream = null;
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public static string DecodeText(string str)
        {
            string str1 = str.Trim(new char[] { '\r', '\t', ' ', '\n' });
            return str = Regex.Replace(str1, @"([\x22]{0,1})\=\?(?<cp>[\w\d\-]+)\?(?<ct>[\w]{1})\?(?<value>[^\x3f]+)\?\=([\x22]{0,1})", HeadersEncode, RegexOptions.Multiline | RegexOptions.IgnoreCase);

        }
        private static string ParseQuotedPrintable(string source, string cp)
        {
            source = source.Replace("_", " ");
            var i = 0;
            var output = new List<byte>();
            while (i < source.Length)
            {
                if (source[i] == '=' && source[i + 1] == '\r' && source[i + 2] == '\n')
                {
                    //Skip
                    i += 3;
                }
                else if (source[i] == '=')
                {
                    string sHex = source;
                    sHex = sHex.Substring(i + 1, 2);
                    int hex = Convert.ToInt32(sHex, 16);
                    byte b = Convert.ToByte(hex);
                    output.Add(b);
                    i += 3;
                }
                else
                {
                    output.Add((byte)source[i]);
                    i++;
                }
            }


            if (String.IsNullOrEmpty(cp))
                return Encoding.UTF8.GetString(output.ToArray());
            else
            {
                if (String.Compare(cp, "ISO-2022-JP", true) == 0)
                {
                    return Encoding.GetEncoding("Shift_JIS").GetString(output.ToArray());
                }
                else
                {
                    if (cp != "")
                    {
                        return Encoding.GetEncoding(cp).GetString(output.ToArray());
                    }
                    else
                    {
                        return Encoding.Default.GetString(output.ToArray());
                    }
                }
            }
        }
        private static string HeadersEncode(Match m)
        {
            string result = String.Empty;
            Encoding cp = null;
            try
            {
                cp = Encoding.GetEncoding(m.Groups["cp"].Value);
            }
            catch
            {
                cp = Encoding.Default;
            }
            Encoding dcp = Encoding.Default;
            if (m.Groups["ct"].Value.ToUpper() == "Q")
            { // кодируем из Quoted-Printable
                result = ParseQuotedPrintable(m.Groups["value"].Value, m.Groups["cp"].Value);
            }
            else if (m.Groups["ct"].Value.ToUpper() == "B")
            { // кодируем из Base64
                result = cp.GetString(Convert.FromBase64String(m.Groups["value"].Value));
            }
            else
            { // такого быть не должно, оставляем текст как есть
                result = m.Groups["value"].Value;
            }
            return result;
        }

        public static string Base64Decode(string str)
        {
            string coding = "";
            string codePage = "";
            string method = "";
            str.Trim(new char[] { ' ', '\t', '\n' });
            //TypeCode tc = Convert.GetTypeCode(str);
            string Rex = ("(^=?(?<encod>[a-zA-Z0-9-]*)?(?<type>[a-zA-Z]{1})?(?<text>.*)?=$)|(^=?(?<encod>[a-zA-Z0-9-]*)?(?<type>[a-zA-Z]{1})?(?<text>.*)?=(?<othertext>s.*)$)");
            //string sttr = Regex.Replace(str, Rex, "");
            string[] sttr = Regex.Split(str, Rex);
            str = Regex.Replace(str, @"([\x22]{0,1})\=\?(?<cp>[\w\d\-]+)\?(?<ct>[\w]{1})\?(?<value>[^\x3f]+)\?\=([\x22]{0,1})", HeadersEncode, RegexOptions.Multiline | RegexOptions.IgnoreCase);
            //string[] splitString= { "","","","",""};
            if (str.StartsWith("=?") && str.EndsWith("?="))
            {
                string[] splitString = str.Split('?');
                coding = splitString[3];
                codePage = splitString[1];
                method = splitString[2];
            }
            //if ((splitString[1] == "="))
            //{
            if ((method.ToUpper() == "B") || (method == ""))
            {
                byte[] buffer = Convert.FromBase64String(coding);
                if (codePage != "")
                {
                    return Encoding.GetEncoding(codePage).GetString(buffer);
                }
                else
                {
                    try
                    {
                        return Encoding.Default.GetString(buffer);
                    }
                    catch
                    {
                        return str;
                    }
                }
            }
            if (method.ToUpper() == "Q")
            {
                var i = 0;
                var output = new List<byte>();
                while (i < coding.Length)
                {
                    if (coding[i] == '=' && coding[i + 1] == '\r' && coding[i + 2] == '\n')
                    {
                        //Skip
                        i += 3;
                    }
                    else if (coding[i] == '=')
                    {
                        string sHex = coding;
                        sHex = sHex.Substring(i + 1, 2);
                        int hex = Convert.ToInt32(sHex, 16);
                        byte b = Convert.ToByte(hex);
                        output.Add(b);
                        i += 3;
                    }
                    else
                    {
                        output.Add((byte)coding[i]);
                        i++;
                    }
                }


                if (String.IsNullOrEmpty(codePage))
                    return Encoding.UTF8.GetString(output.ToArray());
                else
                {
                    if (String.Compare(codePage, "ISO-2022-JP", true) == 0)
                    {
                        return Encoding.GetEncoding("Shift_JIS").GetString(output.ToArray());
                    }
                    else
                    {
                        if (codePage != "")
                        {
                            return Encoding.GetEncoding(codePage).GetString(output.ToArray());
                        }
                        else
                        {
                            return Encoding.Default.GetString(output.ToArray());
                        }
                    }
                }

            }
            return coding;
            // }
            //return str;
        }
        public static string checkBase64coding(string str)
        {
            if (str.StartsWith("=?") && str.EndsWith("?="))
            {
                string[] splitString = str.Split('?');
                return splitString[3];
            }
            return str;
        }



        public ImapServer(string server, int port, string userName, string password, Action<int, string> logger = null)
            : base(logger)
        {
            this.server = server;
            this.port = port;
            this.userName = userName;
            this.password = password;
        }

        override public void Open()
        {
            try
            {
                Close();

                base.Open();

                sslStream = new SslStream(stream);
                stream.ReadTimeout = 10000;
                stream.WriteTimeout = 10000;
                Log(1, "Start SSL Auth");
                sslStream.AuthenticateAsClient(server);
                Log(1, "End SSL Auth");

                reader = new StreamReaderWithLog(sslStream, Log);
                //reader.
                writer = new StreamWriterWithLog(sslStream, Log) { AutoFlush = true };

                readerB = new BinaryReaderS(sslStream);
                writerB = new BinaryWriter(sslStream);

                reader.BaseStream.ReadTimeout = readTimeout;
                writer.BaseStream.ReadTimeout = writeTimeout;

                readerB.BaseStream.ReadTimeout = readTimeout;
                writerB.BaseStream.ReadTimeout = writeTimeout;

                string serverResponse;


                serverResponse = reader.ReadLine();
                Log(23, serverResponse);

                if (!serverResponse.StartsWith("* OK"))
                    throw new InvalidOperationException("IMAP server respond to connection request: " + serverResponse);

                writer.WriteLine(". LOGIN " + userName + " " + password);
                Log(23, ". LOGIN " + userName + " " + "*****");
                do
                    serverResponse = reader.ReadLine();
                while (serverResponse.StartsWith("*"));
                if (!serverResponse.StartsWith(". OK"))
                {
                    string AuthB64 = Base64Encode("\0" + userName + "\0" + password);
                    writer.WriteLine(". AUTHENTICATE PLAIN");
                    Log(23, ". AUTHENTICATE PLAIN");

                    serverResponse = reader.ReadLine();
                    Log(23, serverResponse);
                    if (!serverResponse.StartsWith("+"))
                    {
                        throw new InvalidOperationException("IMAP server respond to LOGIN request: " + serverResponse);
                    }
                    writer.WriteLine(AuthB64);
                    Log(23, "***************");
                    serverResponse = reader.ReadLine();
                    Log(23, serverResponse);
                    if (!serverResponse.StartsWith(". OK"))
                    {
                        throw new InvalidOperationException("IMAP server respond to LOGIN request: " + serverResponse);
                    }
                }

                Log(22, String.Format("IMAP server opened"));
            }
            catch (Exception ex)
            {
                Log(1, String.Format("IMAP server open error: {0}", ex.Message));

                if (writer != null)
                    writer.Dispose();
                writer = null;

                if (reader != null)
                    reader.Dispose();
                reader = null;

                if (sslStream != null)
                    sslStream.Dispose();
                sslStream = null;

                base.Close();

                throw;
            }
        }

        override public bool GetMessageIDs(string lastUID, string mailBox, Action<MailMessageIDs> messageAction)
        {
            UInt64 startUID;
            UInt64 endUID;
            try
            {
                string sendMess = "MBX SELECT " + mailBox;
                writer.WriteLine(sendMess);
                Log(21, sendMess);
                string searchResult = reader.ReadLine();
                Log(21, searchResult);
                do
                    searchResult = reader.ReadLine();
                while (searchResult.StartsWith("*"));
                if (!searchResult.StartsWith("MBX OK"))
                {
                    throw new InvalidOperationException("IMAP server respond to SELECT request: " + searchResult);
                }

                sendMess = "SEA UID SEARCH UID " + (int.Parse(lastUID)) + ":" + (int.MaxValue - 1);
                writer.WriteLine(sendMess);
                Log(21, sendMess);
                searchResult = reader.ReadLine();
                Log(21, searchResult);
                string[] searchArray = searchResult.Split(' ');
                if (searchArray[0] == "*")
                {
                    if (searchArray.Length > 2)
                    {
                        startUID = UInt64.Parse(searchArray[2]);
                        endUID = UInt64.Parse(searchArray.Last());
                    }
                    else
                    {
                        searchResult = reader.ReadLine();
                        return false;
                    }
                    searchResult = reader.ReadLine();
                    if (!searchResult.StartsWith("SEA OK"))
                    {
                        throw new InvalidOperationException("IMAP server respond to SEARCH request: " + searchResult);
                    }
                    //uid fetch x:y body[header.fields (message-id)]
                    sendMess = "FCH UID FETCH " + startUID + ":" + endUID + " (RFC822.SIZE BODY[HEADER.FIELDS (message-id subject date from)])";
                    writer.WriteLine(sendMess);
                    Log(21, sendMess);
                    string fetchResult = reader.ReadLine();
                    Log(21, fetchResult);
                    string[] fetchhArray = fetchResult.Split(' ');
                    string from = "";
                    if (fetchhArray[0] == "*")
                    {

                        do
                        {
                            MailMessageIDs mailID = new MailMessageIDs();
                            mailID.UIDMessage = fetchhArray[4];
                            mailID.Size = fetchhArray[6];
                            mailID.Subject = "";
                            string lastCase = "";
                            while (fetchResult != ")")
                            {
                                fetchResult = reader.ReadLine();
                                fetchhArray = fetchResult.Split(' ');
                                switch (fetchhArray[0].ToUpper())
                                {
                                    case "MESSAGE-ID:":
                                        {
                                            mailID.MessageID = fetchhArray[1];
                                            lastCase = "MESSAGE-ID:";
                                            break;
                                        }
                                    case "SUBJECT:":
                                        {
                                            mailID.Subject += DecodeText(fetchResult.Substring(9));
                                            lastCase = "SUBJECT:";
                                            break;
                                        }
                                    case "FROM:":
                                        {
                                            from = DecodeText(fetchResult.Substring(6));
                                            
                                            //mailID.From += DecodeText(fetchResult.Substring(6));
                                            lastCase = "FROM:";
                                            break;
                                        }
                                    case "DATE:":
                                        {
                                            try
                                            {
                                                mailID.dateTime = Convert.ToDateTime(fetchhArray[2] + " " + fetchhArray[3] + " " + fetchhArray[4] + " " + fetchhArray[5] + " " + fetchhArray[6]);
                                            }
                                            catch
                                            {
                                                try
                                                {
                                                    mailID.dateTime = Convert.ToDateTime(fetchhArray[1] + " " + fetchhArray[2] + " " + fetchhArray[3] + " " + fetchhArray[4] + " " + fetchhArray[5]);
                                                }
                                                catch
                                                {
                                                    Log(1, "Failed convert Date: " + fetchResult);
                                                    mailID.dateTime = DateTime.MinValue;
                                                }
                                            }
                                            lastCase = "DATE:";
                                            break;
                                            
                                        }
                                    case ")":
                                        {
                                            break;
                                        }
                                    default:
                                        {
                                            switch(lastCase)
                                                   {
                                                case "SUBJECT:":
                                                    {
                                                        if (fetchResult.Length > 0)
                                                        {
                                                            mailID.Subject += DecodeText(fetchResult.Substring(1));
                                                        }
                                                        break;
                                                    }
                                                case "FROM:":
                                                    {
                                                        if (fetchResult.Length > 0)
                                                        {
                                                            from = DecodeText(fetchResult.Substring(6));
                                                        }
                                                        break;

                                                    }
                                                default:
                                                    {
                                                        break;
                                                    }
                                        }
                                            break;
                                        
                                        }
                                }


                            }
                            if (from != "")
                            {
                                int Ls = from.IndexOf('<');
                                int Rs = from.IndexOf('>');
                                if ((Ls < 0) || (Rs < 0))
                                {
                                    mailID.FromAddress = from;
                                }
                                else
                                {
                                    mailID.FromName += from.Substring(0, Ls);
                                    mailID.FromAddress += from.Substring(Ls + 1, Rs - Ls - 1);
                                }
                            }
                            //mailID.Subject = Base64Decode(mailID.Subject);
                            messageAction(mailID);
                            fetchResult = reader.ReadLine();
                            fetchhArray = fetchResult.Split(' ');
                            from = "";
                        }
                        while (fetchResult.StartsWith("*"));

                    }
                    if (!fetchResult.StartsWith("FCH OK"))
                    {
                        throw new InvalidOperationException("IMAP server respond to SEARCH request: " + searchResult);
                    }
                }
                else
                {
                    if ((searchArray[0] == "SEA") && (searchArray[1] == "BAD"))
                    {
                        throw new InvalidOperationException("IMAP server respond to SEARCH request: " + searchResult);
                    }
                }
            }
            catch (Exception ex)
            {
                Log(1, String.Format("IMAP server search error: {0}", ex.Message));

                if (writer != null)
                    writer.Dispose();
                writer = null;

                if (reader != null)
                    reader.Dispose();
                reader = null;

                if (sslStream != null)
                    sslStream.Dispose();
                sslStream = null;

                base.Close();

                throw;
            }
            return true;
        }

        override public bool GetMessageIDs(DateTime fromTime, DateTime toTime, string mailBox, Action<MailMessageIDs> messageAction)
        {
            //uid search not or not(since dd - mmm - yyy) not(before dd - mmm - yyy)
            UInt64 startUID;
            UInt64 endUID;
            try
            {
                string sendMess = "MBX SELECT " + mailBox;
                writer.WriteLine(sendMess);
                //Log(21, sendMess);
                string searchResult = reader.ReadLine();
                //Log(21, searchResult);
                do
                    searchResult = reader.ReadLine();
                while (searchResult.StartsWith("*"));
                if (!searchResult.StartsWith("MBX OK"))
                {
                    throw new InvalidOperationException("IMAP server respond to SELECT request: " + searchResult);
                }
                DateTime maxDate = toTime;
                if (maxDate.Year != 9999)
                {
                    maxDate.AddDays(1);
                }
                sendMess = "SEA UID SEARCH NOT OR NOT (BEFORE " + getDateString(maxDate) + ") NOT (SINCE " + getDateString(fromTime) + ")";
                writer.WriteLine(sendMess);
                //Log(21, sendMess);
                searchResult = reader.ReadLine();
                //Log(21, searchResult);
                string[] searchArray = searchResult.Split(' ');
                if (searchArray[0] == "*")
                {
                    if (searchArray.Length > 2)
                    {
                        startUID = UInt64.Parse(searchArray[2]);
                        endUID = UInt64.Parse(searchArray.Last());
                    }
                    else
                    {
                        searchResult = reader.ReadLine();
                        return false;
                    }
                    searchResult = reader.ReadLine();
                    if (!searchResult.StartsWith("SEA OK"))
                    {
                        throw new InvalidOperationException("IMAP server respond to SEARCH request: " + searchResult);
                    }
                    //uid fetch x:y body[header.fields (message-id)]

                    sendMess = "FCH UID FETCH " + startUID + ":" + endUID + " (RFC822.SIZE BODY[HEADER.FIELDS (message-id subject date)])";
                    writer.WriteLine(sendMess);
                    //Log(21, sendMess);
                    string fetchResult = reader.ReadLine();
                    //Log(21, fetchResult);
                    string[] fetchhArray = fetchResult.Split(' ');

                    if (fetchhArray[0] == "*")
                    {

                        do
                        {
                            MailMessageIDs mailID = new MailMessageIDs();
                            mailID.UIDMessage = fetchhArray[4];
                            mailID.Size = fetchhArray[6];
                            mailID.Subject = "";
                            while (fetchResult != ")")
                            {
                                fetchResult = reader.ReadLine();
                                //Log(1, String.Format("Trace message: {0}", fetchResult));                                                                
                                fetchhArray = fetchResult.Split(' ');
                                //Log(1, String.Format("Trace message: {0}", fetchhArray[0]));
                                switch (fetchhArray[0].ToUpper())
                                {
                                    case "MESSAGE-ID:":
                                        {
                                            mailID.MessageID = fetchhArray[1];
                                            break;
                                        }
                                    case "SUBJECT:":
                                        {
                                            mailID.Subject += DecodeText(fetchResult.Substring(9));
                                            break;
                                        }
                                    case "DATE:":
                                        {
                                            try
                                            {
                                                mailID.dateTime = Convert.ToDateTime(fetchhArray[2] + " " + fetchhArray[3] + " " + fetchhArray[4] + " " + fetchhArray[5] + " " + fetchhArray[6]);
                                            }
                                            catch
                                            {
                                                try
                                                {
                                                    mailID.dateTime = Convert.ToDateTime(fetchhArray[1] + " " + fetchhArray[2] + " " + fetchhArray[3] + " " + fetchhArray[4] + " " + fetchhArray[5]);
                                                }
                                                catch
                                                {
                                                    Log(1, "Failed convert Date: " + fetchResult);
                                                    mailID.dateTime = DateTime.MinValue;
                                                }
                                            }
                                            break;
                                        }
                                    case ")":
                                        {
                                            break;
                                        }
                                    default:
                                        {
                                            if (fetchResult.Length > 0)
                                            {
                                                mailID.Subject += DecodeText(fetchResult.Substring(1));
                                            }
                                            break;
                                        }
                                }

                            }
                            //mailID.Subject = Base64Decode(mailID.Subject);
                            messageAction(mailID);
                            fetchResult = reader.ReadLine();
                            fetchhArray = fetchResult.Split(' ');

                        }
                        while (fetchResult.StartsWith("*"));

                    }
                    if (!fetchResult.StartsWith("FCH OK"))
                    {
                        throw new InvalidOperationException("IMAP server respond to SEARCH request: " + searchResult);
                    }
                }
                else
                {
                    if ((searchArray[0] == "SEA") && (searchArray[1] == "BAD"))
                    {
                        throw new InvalidOperationException("IMAP server respond to SEARCH request: " + searchResult);
                    }
                }
            }
            catch (Exception ex)
            {
                Log(1, String.Format("IMAP server search error: {0}", ex.Message));

                if (writer != null)
                    writer.Dispose();
                writer = null;

                if (reader != null)
                    reader.Dispose();
                reader = null;

                if (sslStream != null)
                    sslStream.Dispose();
                sslStream = null;

                base.Close();

                throw;
            }
            return true;


        }

        override public string DownloadMessage(string UID, string mailBox, long StartOctet, Func<byte[], string> messageAction)
        {
            try
            {
                string sendMess = "MBX SELECT " + mailBox;
                writer.WriteLine(sendMess);
                //Log(21, sendMess);
                string searchResult = reader.ReadLine();
                //Log(21, searchResult);
                do
                    searchResult = reader.ReadLine();
                while (searchResult.StartsWith("*"));
                if (!searchResult.StartsWith("MBX OK"))
                {
                    throw new InvalidOperationException("IMAP server respond to SELECT request: " + searchResult);
                }
                //sendMess = "FCH UID FETCH " + UID + " RFC822 <5";
                sendMess = "FCH UID FETCH " + UID + " BODY[]<"+StartOctet.ToString()+ ".2097152>"; //Лимит 2 МБ
                writer.WriteLine(sendMess);
                string fetchResult;
                //fetchResult = reader.ReadLine();
                //fetchResult = readerB.ReadString();

                fetchResult = readerB.ReadLn();
                byte byteAdd;
                bool CRLF = true;
                if (fetchResult[0] == '0')
                {
                    byteAdd = (byte)fetchResult[fetchResult.Length];
                    CRLF = false;
                }
                else
                {
                    byteAdd = 0;
                    CRLF = true;
                }
                fetchResult = fetchResult.Substring(1, fetchResult.Length-1);
                string Rex = (@"\{[0-9]+\}");
                MatchCollection match = Regex.Matches(fetchResult, Rex);
                if (match.Count > 0)
                {
                    long Size = long.Parse(match[0].Value.Substring(1, match[0].Value.Length-2));
                    //Log(21, fetchResult);
                    string[] fetchhArray = fetchResult.Split(' ');
                    //byte[] fetchByte;
                    
                        if (fetchhArray[0] == "*")
                    {


                        
                        long downloaded = 0;
                        do
                        {

                            byte[] fetchBytes;
                           /* if (!CRLF)
                            {
                                fetchBytes = fetchBytes +  byteAdd;
                            }*/
                            if (Size > (downloaded + 1024))
                            {
                                
                                fetchBytes = readerB.ReadBytes(1024);
                                //fetchResult = fetchBytes.ToString();
                                downloaded += fetchBytes.Length;
                                string ReturnMessageAction = messageAction(fetchBytes);
                                if (ReturnMessageAction != "")
                                {
                                    return ReturnMessageAction;
                                }
                            }
                            else
                            {
                                fetchBytes = readerB.ReadBytes((int)(Size - downloaded));
                                downloaded += fetchBytes.Length;

                                messageAction(fetchBytes);
                                break;
                            }
                            
                                
                        }
                        while (true);// (!fetchResult.StartsWith(")"));
                        do
                        {
                            fetchResult = reader.ReadLine();
                        }
                        while (!fetchResult.StartsWith("FCH"));
                    }
                }
                else
                {
                    /*  //Log(21, fetchResult);
                      string[] fetchhArray = fetchResult.Split(' ');
                      //byte[] fetchByte;
                      if (fetchhArray[0] == "*")
                      {
                          do
                          {
                              fetchResult = reader.ReadLine();
                              if (fetchResult == ")")
                              {
                                  string fetchResultP = fetchResult;
                                  fetchResult = reader.ReadLine();
                                  if ((!fetchResult.StartsWith("FCH OK")) && (!fetchResult.StartsWith("FCH BAD") && (!fetchResult.StartsWith("FCH NO"))))
                                  {
                                      messageAction(fetchResultP);
                                  }
                                  else
                                  {
                                      break;
                                  }
                              }
                              messageAction(fetchResult);
                          }
                          while (true);// (!fetchResult.StartsWith(")"));

                      }*/
                    
                    return "IMAP Сервер не вернул сообщения для скачки: "+ fetchResult;

                }
                //fetchResult = reader.ReadLine();
                if (!fetchResult.StartsWith("FCH OK"))
                {
                    throw new InvalidOperationException("IMAP server download error: " + fetchResult);
                }
            }
            catch (Exception ex)
            {
                Log(1, String.Format("IMAP server download error: {0}", ex.Message));
                return String.Format("IMAP server download error: {0}", ex.Message);
            }

            return "";
        }

        override public void Close()
        {
            try
            {
                if (writer != null)
                {
                    string serverResponse;

                    writer.WriteLine(". LOGOUT");
                    do
                        serverResponse = reader.ReadLine();
                    while (serverResponse.StartsWith("*"));
                    if (!serverResponse.StartsWith(". OK"))
                        throw new InvalidOperationException("IMAP server respond to LOGOUT request: " + serverResponse);

                    Log(22, String.Format("IMAP server closed"));
                }
            }
            catch (Exception ex)
            {
                Log(1, String.Format("IMAP server close error: {0}", ex.Message));
            }
            finally
            {
                if (writer != null)
                    writer.Dispose();
                writer = null;

                if (reader != null)
                    reader.Dispose();
                reader = null;

                if (sslStream != null)
                    sslStream.Dispose();
                sslStream = null;

                base.Close();
            }
        }

        override public void Send(string fromAddress, string toAddress, string folderName, string messageData, string addHeader, string UID, string Size)
        {
            Send(folderName, messageData);
        }

        public void Send(string folderName, string messageData)
        {
            string serverResponse;

            try
            {
                writer.WriteLine(". APPEND \"" + folderName + "\" () {" + writer.Encoding.GetByteCount(messageData) + "}");
                serverResponse = reader.ReadLine();
                if (!serverResponse.StartsWith("+"))
                    throw new InvalidOperationException("IMAP server respond to APPEND request: " + serverResponse);

                writer.WriteLine(messageData);
                do
                    serverResponse = reader.ReadLine();
                while (serverResponse.StartsWith("*"));
                if (!serverResponse.StartsWith(". OK"))
                    throw new InvalidOperationException("IMAP server respond to message data: " + serverResponse);

                Log(12, String.Format("IMAP message appended (to folded: {0})", folderName));
            }
            catch (Exception ex)
            {
                Log(1, String.Format("IMAP append error: {0}", ex.Message));
            }
        }


    }
    public class MailMessageIDs
    {
        public string MessageID;
        public string UIDMessage;
        public string Subject;
        public string Size;
        public string FromName;
        public string FromAddress;
        public DateTime dateTime;
    }


    public class SmtpServer : MailServer
    {
        private readonly string userName;
        private readonly string password;

        private StreamReader clearTextReader = null;
        private StreamWriter clearTextWriter = null;
        private SslStream sslStream = null;

        public SmtpServer(string server, int port, string userName, string password, Action<int, string> logger = null)
            : base(logger)
        {
            this.server = server;
            this.port = port;
            this.userName = userName;
            this.password = password;
        }

        override public void Open()
        {
            try
            {
                Close();

                base.Open();

                clearTextReader = new StreamReader(stream);
                clearTextWriter = new StreamWriter(stream) { AutoFlush = true };

                clearTextReader.BaseStream.ReadTimeout = readTimeout;
                clearTextWriter.BaseStream.WriteTimeout = writeTimeout;

                
                string serverResponse;

                serverResponse = clearTextReader.ReadLine();
                if (!serverResponse.StartsWith("220"))
                    throw new InvalidOperationException("SMTP server respond to connection request: " + serverResponse);

                clearTextWriter.WriteLine("HELO");
                serverResponse = clearTextReader.ReadLine();
                if (!serverResponse.StartsWith("250"))
                    throw new InvalidOperationException("SMTP server respond to HELO request: " + serverResponse);
                /////////////////////////////
                //clearTextWriter.WriteLine("STARTTLS");
                //serverResponse = clearTextReader.ReadLine();
                //if (!serverResponse.StartsWith("220"))
                //    throw new InvalidOperationException("SMTP server respond to STARTTLS request: " + serverResponse);

                //sslStream = new SslStream(stream);
                //sslStream.AuthenticateAsClient(server);


                //reader = new StreamReaderWithLog(sslStream);
                //writer = new StreamWriterWithLog(sslStream) { AutoFlush = true };



                //readerB = new BinaryReaderS(sslStream);
                //writerB = new BinaryWriter(sslStream);
                ////////////////////////////////
                reader = new StreamReaderWithLog(stream);
                writer = new StreamWriterWithLog(stream) { AutoFlush = true };

                readerB = new BinaryReaderS(stream);
                writerB = new BinaryWriter(stream);
                ////////////////////////////////

                reader.BaseStream.ReadTimeout = readTimeout;
                writer.BaseStream.ReadTimeout = writeTimeout;

                readerB.BaseStream.ReadTimeout = readTimeout;
                writerB.BaseStream.ReadTimeout = writeTimeout;


                writer.WriteLine("EHLO " + server);
                serverResponse = reader.ReadLine();
                if (!serverResponse.StartsWith("250"))
                    throw new InvalidOperationException("SMTP server respond to EHLO request: " + serverResponse);
                while (reader.Peek() > -1)
                {
                    string reaa = reader.ReadLine();
                }

                var authString = Convert.ToBase64String(Encoding.ASCII.GetBytes("\0" + userName + "\0" + password));
                authString = userName + " " + password;
                //writer.WriteLine("AUTH LOGIN " + authString);
                //serverResponse = reader.ReadLine();
                //if (!serverResponse.StartsWith("235"))
                //    throw new InvalidOperationException("SMTP server respond to AUTH PLAIN request: " + serverResponse);

                Log(22, String.Format("SMTP Mx server opened"));
            }
            catch (Exception ex)
            {
                Log(1, String.Format("SMTP Mx server open error: {0}", ex.Message));

                if (writer != null)
                    writer.Dispose();
                writer = null;

                if (reader != null)
                    reader.Dispose();
                reader = null;

                if (sslStream != null)
                    sslStream.Dispose();
                sslStream = null;

                if (clearTextWriter != null)
                    clearTextWriter.Dispose();
                clearTextWriter = null;

                if (clearTextReader != null)
                    clearTextReader.Dispose();
                clearTextReader = null;

                base.Close();
                
                throw;
            }
        }

        override public void Close()
        {
            try
            {
                if (writer != null)
                {
                    string serverResponse;

                    writer.WriteLine("QUIT");
                    serverResponse = reader.ReadLine();
                    if (!serverResponse.StartsWith("221"))
                        throw new InvalidOperationException("SMTP server respond to QUIT request: " + serverResponse);

                    Log(22, String.Format("SMTP Mx server closed"));
                }
            }
            catch (Exception ex)
            {
                Log(1, String.Format("SMTP Mx server close error: {0}", ex.Message));
                throw;
            }
            finally
            {
                if (writer != null)
                    writer.Dispose();
                writer = null;

                if (reader != null)
                    reader.Dispose();
                reader = null;

                if (writerB != null)
                    writerB.Dispose();
                writerB = null;

                if (readerB != null)
                    readerB.Dispose();
                readerB = null;

                if (sslStream != null)
                    sslStream.Dispose();
                sslStream = null;

                if (clearTextWriter != null)
                    clearTextWriter.Dispose();
                clearTextWriter = null;

                if (clearTextReader != null)
                    clearTextReader.Dispose();
                clearTextReader = null;

                base.Close();
            }
        }

        override public void Send(string fromAddress, string toAddress, string folderName, string fileMessageName, string addHeader, string UID, string Size)
        {
            Send(fromAddress, toAddress, fileMessageName, addHeader, UID, Size);
        }

        public void Send(string fromAddress, string toAddress, string fileMessageName, string addHeader, string UID, string Size)
        {
            string serverResponse;
            FileStream fileMessageRead = null;
            try
            {
                fileMessageRead = new FileStream(fileMessageName, FileMode.Open, FileAccess.Read, FileShare.Read);
                writer.WriteLine("MAIL FROM: <" + fromAddress + ">");
                serverResponse = reader.ReadLine();
                if (!serverResponse.StartsWith("250"))
                    throw new InvalidOperationException("SMTP server respond to MAIL FROM request: " + serverResponse);

                writer.WriteLine("RCPT TO: <" + toAddress + ">");
                serverResponse = reader.ReadLine();
                if (!serverResponse.StartsWith("250"))
                    throw new InvalidOperationException("SMTP server respond to RCPT TO request: " + serverResponse);

                writer.WriteLine("BDAT "+ Size.ToString()+" LAST");
                /*serverResponse = reader.ReadLine();
                if (!serverResponse.StartsWith("354"))
                    throw new InvalidOperationException("SMTP server respond to DATA request: " + serverResponse);*/
                string line;
                writer.WriteLine(addHeader);
                writer.Flush();
                int sended = 0;
                int readed = 0;
                DateTime TimerS = DateTime.Now;
                byte[] buff = new byte[1024];
                int readFact = 0;
                do
                {
                    readFact = fileMessageRead.Read(buff, readed, 1024);
                    //line = fileMessageRead.ReadLine();
                    /*  if (line == ".")
                      {
                          line = line + ".";
                      }*/
                    if (readFact > 0)
                    {
                        writerB.Write(buff, 0, readFact);
                        writerB.Flush();
                        sended += readFact;

                        if (TimerS.AddMilliseconds(200) < DateTime.Now)
                        {
                            Log(55, "Отправка: " + UID + " " + Synchinc.GetKBMBGB(sended.ToString()) + "/" + Synchinc.GetKBMBGB(Size));
                            TimerS = DateTime.Now;
                        }
                    }
                }
                while (readFact != 0);
                //writer.WriteLine();
                //writer.WriteLine(".");
                //writer.WriteLine();
                serverResponse = reader.ReadLine();
                if (!serverResponse.StartsWith("250"))
                    throw new InvalidOperationException("SMTP server respond to end data request: " + serverResponse);

                Log(12, String.Format("SMTP Mx message sent (from address: {0} to address: {1})", fromAddress, toAddress));
                //fileMessageRead = null;
                sended = 0;


               }
               catch (Exception ex)
               {
                   Log(1, String.Format("SMTP Mx send error: {0}", ex.Message));
                throw;
               }
            finally
            {
                if (fileMessageRead != null)
                {
                    fileMessageRead.Close();
                    fileMessageRead.Dispose();
                }
                

            }
        }
       }


      /* public class SmtpMxServer : MailServer
       {
           private readonly string toAddress;

           public SmtpMxServer(string toAddress, Action<int, string> logger = null)
               : base(logger)
           {
               this.toAddress = toAddress;

               int i = toAddress.IndexOf('@');
               if ((i <= 0) || (i >= toAddress.Length - 1))
                   throw new InvalidOperationException("Bad recipient address");
               var mxs = Mx.GetMXRecords(toAddress.Substring(i + 1));
               if (mxs.Length < 1)
                   throw new InvalidOperationException("MX record for recipient address is not found");

               server = mxs[0];
               port = 25;
           }

           override public void Open()
           {
               try
               {
                   Close();

                   base.Open();

                   reader = new StreamReaderWithLog(stream);
                   writer = new StreamWriterWithLog(stream) { AutoFlush = true };

                   reader.BaseStream.ReadTimeout = readTimeout;
                   writer.BaseStream.ReadTimeout = writeTimeout;

                   string serverResponse;

                   serverResponse = reader.ReadLine();
                   if (!serverResponse.StartsWith("220"))
                       throw new InvalidOperationException("SMTP server respond to connection request: " + serverResponse);

                   writer.WriteLine("HELO");
                   serverResponse = reader.ReadLine();
                   if (!serverResponse.StartsWith("250"))
                       throw new InvalidOperationException("SMTP server respond to HELO request: " + serverResponse);

                   Log(22, String.Format("SMTP server opened"));
               }
               catch (Exception ex)
               {
                   Log(1, String.Format("SMTP server open error: {0}", ex.Message));

                   if (writer != null)
                       writer.Dispose();
                   writer = null;

                   if (reader != null)
                       reader.Dispose();
                   reader = null;

                   base.Close();

                   throw;
               }
           }

           override public void Close()
           {
               try
               {
                   if (writer != null)
                   {

                       string serverResponse;

                       writer.WriteLine("QUIT");
                       serverResponse = reader.ReadLine();
                       if (!serverResponse.StartsWith("221"))
                           throw new InvalidOperationException("SMTP server respond to QUIT request: " + serverResponse);

                       Log(22, String.Format("SMTP server closed"));
                   }
               }
               catch (Exception ex)
               {
                   Log(1, String.Format("SMTP server close error: {0}", ex.Message));
               }
               finally
               {
                   if (writer != null)
                       writer.Dispose();
                   writer = null;

                   if (reader != null)
                       reader.Dispose();
                   reader = null;

                   base.Close();
               }
           }

           override public void Send(string fromAddress, string toAddress, string folderName, string messageData)
           {
               Send(fromAddress, messageData);
           }

           public void Send(string fromAddress, string messageData)
           {
               string serverResponse;

               try
               {
                   writer.WriteLine("MAIL FROM: <" + fromAddress + ">");
                   serverResponse = reader.ReadLine();
                   if (!serverResponse.StartsWith("250"))
                       throw new InvalidOperationException("SMTP server respond to MAIL FROM request: " + serverResponse);

                   writer.WriteLine("RCPT TO: <" + toAddress + ">");
                   serverResponse = reader.ReadLine();
                   if (!serverResponse.StartsWith("250"))
                       throw new InvalidOperationException("SMTP server respond to RCPT TO request: " + serverResponse);

                   writer.WriteLine("DATA");
                   serverResponse = reader.ReadLine();
                   if (!serverResponse.StartsWith("354"))
                       throw new InvalidOperationException("SMTP server respond to DATA request: " + serverResponse);

                   writer.WriteLine(messageData);
                   writer.WriteLine(".");
                   serverResponse = reader.ReadLine();
                   if (!serverResponse.StartsWith("250"))
                       throw new InvalidOperationException("SMTP server respond to end data request: " + serverResponse);

                   Log(12, String.Format("SMTP message sent (from address: {0})", fromAddress));
               }
               catch (Exception ex)
               {
                   Log(1, String.Format("SMTP send error: {0}", ex.Message));
               }
           }
       }*/
}
