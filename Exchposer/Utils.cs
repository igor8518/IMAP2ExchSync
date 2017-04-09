using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Security.Cryptography;
using IMAP2ExchSync;
using System.Collections;
//using System.Runtime.Serialization;
//using System.Runtime.Serialization.Formatters.Binary;

namespace IMAP2ExchSync
{
    public static class Crypto
    {
        private const int keySize = 256;
        private const string initVector = "C#$7gMNVFH^&%6hZ";

        public static string Encrypt(string plainText, string password)
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            string result;

            using (ICryptoTransform encryptor = (new RijndaelManaged() { Mode = CipherMode.CBC }).
                CreateEncryptor((new PasswordDeriveBytes(password, null)).GetBytes(keySize / 8), Encoding.UTF8.GetBytes(initVector)))
            using (MemoryStream memoryStream = new MemoryStream())
            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
            {
                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                cryptoStream.FlushFinalBlock();
                result = Convert.ToBase64String(memoryStream.ToArray());
            }

            return result;
        }

        public static string Decrypt(string encryptedText, string password)
        {
            if (encryptedText == "")
                return "";

            byte[] encryptedTextBytes = Convert.FromBase64String(encryptedText);
            byte[] plainTextBytes = new byte[encryptedTextBytes.Length];
            string result;

            using (ICryptoTransform decryptor = (new RijndaelManaged() { Mode = CipherMode.CBC }).
                CreateDecryptor((new PasswordDeriveBytes(password, null)).GetBytes(keySize / 8), Encoding.UTF8.GetBytes(initVector)))
            using (MemoryStream memoryStream = new MemoryStream(encryptedTextBytes))
            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
            {
                int plainTextByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                result = Encoding.UTF8.GetString(plainTextBytes, 0, plainTextByteCount);
            }

            return result;
        }
    }

    //[Serializable()]
    public class XmlSettings<T> where T : XmlSettings<T>, new()
    {
        protected string fileName = null;

        public XmlSettings()
        {
        }

        public XmlSettings(XmlSettings<T> xmlSettings)
        {
            fileName = xmlSettings.fileName;
        }

        public void Save()
        {
            lock (this)
            {
                Save(fileName);
            }
        }

        public void Save(string fileName)
        {
            lock (this)
            {
                //Directory.CreateDirectory(Path.GetDirectoryName(fileName));
                using (TextWriter writer = new StreamWriter(fileName))
                {
                    (new XmlSerializer(typeof(T))).Serialize(writer, this);
                }
            }
        }



        public static T Load(string fileName)
        {
            
            T t;


            if ((fileName != null) && File.Exists(fileName))
                using (TextReader reader = new StreamReader(fileName))
                {
                    t = (T)(new XmlSerializer(typeof(T))).Deserialize(reader);
                }
            else
                t = new T();

            t.fileName = fileName;
            return t;
        }

        public static void Load(ref T t, string fileName = null)
        {
            if ((t != null) && (fileName == null))
                fileName = t.fileName;
            t = Load(fileName);
        }

        /*
        public T Clone()
        {
            IFormatter formatter = new BinaryFormatter();
            using (Stream stream = new MemoryStream())
            {
                formatter.Serialize(stream, this);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }
        */
    }

    public class FileString
    {
        private string fileName = null;
        private string value = null;

        public string Value
        {
            get { return Load(); }
            set { Save(value); }
        }

        public FileString(string fileName)
        {
            this.fileName = fileName;
        }

        public void Save(string value)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fileName));
            using (TextWriter writer = new StreamWriter(fileName))
            {
                writer.Write(value);
            }
            this.value = value;
        }

        public string Load()
        {
            if ((fileName != null) && File.Exists(fileName))
                using (TextReader reader = new StreamReader(fileName))
                {
                    value = reader.ReadToEnd();
                }
            else
                value = null;

            return value;
        }
    }

    public class MailData
    {
        public string UID = "";
        public string Size = "0";
        public string messageID = "<>";
        public DateTime dateTime = DateTime.MinValue;
        public DateTime dateTimeDown = DateTime.MinValue;
        public DateTime dateTimeSend = DateTime.MinValue;
        public int status = 0;
        public string subtitle = "";
        public MailData()
        {
            UID = "";
            messageID = "<>";
            dateTime = DateTime.Now;
            subtitle = "";
        }
        public MailData(string UID, string messageID, DateTime dateTime,string subtitle, string Size = "0")
        {
            this.Size = Size;
            this.UID = UID;
            this.messageID = messageID;
            this.dateTime = dateTime;
            this.subtitle = subtitle;        }
    }
    
    public class MailList<MailData>  : XmlSettings<MailList<MailData>>, IEnumerable<MailData>, IList<MailData> where MailData : IMAP2ExchSync.MailData
    {
        public class MailListUIDSearch
        {
            String _s;

            public MailListUIDSearch(String s)
            {
                _s = s;
            }

            public bool EquUID(MailData e)
            {
                return e.UID.Equals(_s, StringComparison.CurrentCulture);
            }
        }
        private List<MailData> mailList = new List<MailData>();
        public string fileName = null;
        public MailList()
        {
          
        }
        public MailList(Mails mailData)
        {
            this.fileName = Path.Combine(AppSettings.AppDefaultFolder, mailData.ExchangeMailBox + ".id");
            //this.mailList = Load(fileName) as List<MailData>;
            //this.mailList.Concat(mailList);

            //this.mailList.enu = mailList as List<MailData>;
            //Load(fileName);

        }

        public  static MailList<MailData> Load(Mails mailData)
        {
            string fileName = Path.Combine(AppSettings.AppDefaultFolder, mailData.ExchangeMailBox + ".id");
            return Load(fileName);
        }
        public static MailList<MailData> Load2(Mails mailData)
        {
            string fileName = Path.Combine(AppSettings.AppDefaultFolder, mailData.ExchangeMailBox + ".md");
            return Load(fileName);
        }
        public new void Save()
        {
            base.Save();
        }

        public MailData this[int index]
        {
            get
            {
                return ((IList<MailData>)mailList)[index];
            }

            set
            {
                ((IList<MailData>)mailList)[index] = value;
            }
        }

        public int Count
        {
            get
            {
                return ((IList<MailData>)mailList).Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return ((IList<MailData>)mailList).IsReadOnly;
            }
        }

        public void Add(MailData item)
        {
            MailListUIDSearch mlUID = new MailListUIDSearch(item.UID);
            if (mailList.FindIndex(mlUID.EquUID)<0)
            {
                ((IList<MailData>)mailList).Add(item);

            }
            
        }

        public int FindIndexByUID(string UID)
        {
            MailListUIDSearch mlUID = new MailListUIDSearch(UID);
            return mailList.FindIndex(mlUID.EquUID);            
        }

        public void Clear()
        {
            ((IList<MailData>)mailList).Clear();
        }

        public bool Contains(MailData item)
        {
            return ((IList<MailData>)mailList).Contains(item);
        }

        public void CopyTo(MailData[] array, int arrayIndex)
        {
            ((IList<MailData>)mailList).CopyTo(array, arrayIndex);
        }

        public IEnumerator<MailData> GetEnumerator()
        {
            return mailList.GetEnumerator();
        }

        public int IndexOf(MailData item)
        {
            return ((IList<MailData>)mailList).IndexOf(item);
        }

        public void Insert(int index, MailData item)
        {
            ((IList<MailData>)mailList).Insert(index, item);
        }

        public bool Remove(MailData item)
        {
            return ((IList<MailData>)mailList).Remove(item);
        }

        public void RemoveAt(int index)
        {
            ((IList<MailData>)mailList).RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return mailList.GetEnumerator();
        }
    }



       
    
}
