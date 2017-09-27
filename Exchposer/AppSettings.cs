using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.ComponentModel;

namespace IMAP2ExchSync
{
    public enum MailServerTypes { IMAP = 1};

    public class AppSettings : XmlSettings<AppSettings>
    {
        public static string AppName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
        public static string AppDefaultFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppName);
        

        public List<Mails> ListMails = new List<Mails>();
        public int DefMaxDaysFullSync = 7;

        public string LogFileName = Path.Combine(AppDefaultFolder, "Log.txt");
        public int DefLogLevel = 11;
        public bool DefLogClearOnStartup = true;
        public bool AutoSynch = false;
        public bool DefSyncEnabled = false;
        public int maxThreads = 3;

        public void Copy(AppSettings appSettings)
        {
            lock (this)
            {
                /* ExchangeUrl = appSettings.ExchangeUrl;
                 ExchangeDomain = appSettings.ExchangeDomain;
                 ExchangeUserName = appSettings.ExchangeUserName;
                 ExchangeMailBox = appSettings.ExchangeMailBox;
                 ExchangePasswordCrypted = appSettings.ExchangePasswordCrypted;
                 ExchangeSubscriptionLifetime = appSettings.ExchangeSubscriptionLifetime;
                 ExchangeAcceptInvalidCertificate = appSettings.ExchangeAcceptInvalidCertificate;
                 MailServerName = appSettings.MailServerName;
                 MailServerPort = appSettings.MailServerPort;
                 MailServerType = appSettings.MailServerType;
                 MailUserName = appSettings.MailUserName;
                 MailPasswordCrypted = appSettings.MailPasswordCrypted;
                 MailToAddress = appSettings.MailToAddress;
                 MailFolderName = appSettings.MailFolderName;
                 MailPassword = appSettings.MailPassword;
                 SyncEnabled = appSettings.SyncEnabled;*/
                ListMails = appSettings.ListMails;
                DefMaxDaysFullSync = appSettings.DefMaxDaysFullSync;
                LogFileName = appSettings.LogFileName;
                DefLogLevel = appSettings.DefLogLevel;
                DefLogClearOnStartup = appSettings.DefLogClearOnStartup;
                AutoSynch = appSettings.AutoSynch;
            }
        }

        /*
        public AppSettings(AppSettings appSettings) : base(appSettings)
        {
            Copy(appSettings);
        }
        */
        
    }
    public class Mails : INotifyPropertyChanged, ICloneable
    {
        public object Clone()
        {
            Mails newMailsObject = new Mails();
          /*  newMailsObject. = (Mails)this.mails.Clone();
            newMailsObject.message = message;
            newMailsObject.type = type;*/
            return newMailsObject;
        }
        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении
        private void NotifyPropertyChanged(String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public int sort = 0;
        public const string SettingsPassword = "ExCh2811";
        public string ExchangeUrl = "";
        public string ExchangeDomain = "";
        public string ExchangeUserName = "";
        public string ExchangePasswordCrypted = "";
        [XmlIgnore]
        private string exchangeMailBox;
        [XmlIgnore]
        public MailData currentMailWork = null;
        /*[XmlIgnore]
        public string Progress = "";*/

        public string ExchangeMailBox
        {
            get { return exchangeMailBox; }
            set
            {
                if (value != exchangeMailBox)
                {
                    exchangeMailBox = value;
                    NotifyPropertyChanged();
                }
            }
        }

        [XmlIgnore]
        private string lastStatus;
        [XmlIgnore]
        public string Thread;
        public string LastStatus
        {
            get { return lastStatus; }
            set
            {
                
                    lastStatus = value;
                
                    NotifyPropertyChanged();
                
                
            }
        }


        public int ExchangeSubscriptionLifetime = 30;
        public bool ExchangeAcceptInvalidCertificate = false;
        [XmlIgnore]
        public string ExchangePassword
        {
            get { return Crypto.Decrypt(ExchangePasswordCrypted, SettingsPassword); }
            set { ExchangePasswordCrypted = Crypto.Encrypt(value, SettingsPassword); }
        }

        public string MailServerName = "";
        public int MailServerPort = 0;
        public MailServerTypes MailServerType = MailServerTypes.IMAP;
        [XmlIgnore]
        private string mailUserName;
        
        public string MailUserName
        {
            get { return mailUserName; }
            set
            {
                if (value != mailUserName)
                {
                    mailUserName = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string MailPasswordCrypted = "";
        public string MailToAddress = "";
        public string MailFolderName = "";
        [XmlIgnore]
        public string MailPassword
        {
            get { return Crypto.Decrypt(MailPasswordCrypted, SettingsPassword); }
            set { MailPasswordCrypted = Crypto.Encrypt(value, SettingsPassword); }
        }
        [XmlIgnore]
        private bool syncEnabled;

        public bool SyncEnabled
        {
            get { return syncEnabled; }
            set
            {
                if (value != syncEnabled)
                {
                    syncEnabled = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public Mails()
        {
           
            exchangeMailBox = "";
            mailUserName = "";
            syncEnabled = false;
            lastStatus = "";
        }

    }

}
