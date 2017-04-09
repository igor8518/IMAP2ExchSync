using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Exchange.WebServices.Data;
using System.Text.RegularExpressions;
using System.IO;
using System.Drawing;
using Microsoft.Win32;
using System.Threading;
using System.Collections;
using System.Data;

namespace IMAP2ExchSync
{
    public class IMAP2ExchSyncApplicationContext : ApplicationContext, IIMAP2ExchSync
    {
        RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
        public BindingSource mailBindingSource { get; set; }
        private const int maxLogFileSize = 1024 * 50;
        private const int maxLogFileCount = 3;

        private const int exchangeReconnectTimeout = 10;
        public int ExchangeReconnectTimeout
        {
            get
            {
                return exchangeReconnectTimeout;
            }
        }

        static AppSettings appSettings = null;


        LogWriter logWriter = null;
        //MailServer mailServer = null;
        //ExchangeServer exchangeServer = null;

        //Icon AppIconNormal = new Icon("AppIconNormal.ico");
        //Icon AppIconBusy = new Icon("AppIconBusy.ico");

        public bool Initialized { get { return notifyIcon != null; } }

        public static NotifyIcon notifyIcon = null;
        public static ContextMenuStrip notifyIconMenu = null;
        public static OptionsForm optionsForm = null;
        LogViewForm logViewForm = null;
        Queue<Mails> mainQueue = null;

        public bool AutoRun
        {
            get
            {
                return (rkApp.GetValue(AppSettings.AppName) != null);
            }
            set
            {
                if (AutoRun != value)
                {
                    if (value)
                        rkApp.SetValue(AppSettings.AppName, Application.ExecutablePath.ToString());
                    else
                        rkApp.DeleteValue(AppSettings.AppName, false);
                }
            }
        }



        delegate void SetLogCallback(int level, object message, Mails m, string origMessage);

        private void CreateAppDefaultFolder(string folderName)
        {
            if (System.IO.Path.GetFullPath(folderName).Equals(System.IO.Path.GetFullPath(AppSettings.AppDefaultFolder)))
                Directory.CreateDirectory(folderName);
        }


        public static DataTable table = new DataTable();

        public void Log(int level, object message, Mails m = null, string origMessage = "")
        {
            string managedThreadId = System.Threading.Thread.CurrentThread.Name;
            // int managedThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
            string firstMessage = message.ToString();
            SynchincSearch ss = new SynchincSearch(managedThreadId);
            int findIndex = threads.FindIndex(ss.Equ);
            string mailbox = "";
            Mails mails = null;
            if (findIndex >= 0)
            {
                mails = threads[findIndex].mailObject;
                if (mails != null)
                {
                    mailbox = " (" + mails.ExchangeMailBox + ") ";
                }
                else
                {
                    mailbox = " ";
                }
            }

            if (message.GetType() == typeof(string))
            {


                message = "[" + managedThreadId + "]" + mailbox + message;
                if (notifyIconMenu.InvokeRequired)
                {
                    
                    SetLogCallback d = new SetLogCallback(Log);
                    notifyIconMenu.Invoke(d, new object[] { level, message, mails, firstMessage });
                    return;
                }
                
                if (logWriter != null)
                {
                    string logText = logWriter.Log(level, message.ToString());
                    if (logText != "")
                    {
                        if (level != 55)
                        {
                            table.Rows.Add(logText + Environment.NewLine);
                        }
                        if (m != null)
                            m.LastStatus = origMessage.ToString();
                    }
                    else
                    {
                        if (level != 55)
                        {
                            table.Rows.Add(message);
                        }
                        if (m != null)
                            m.LastStatus = origMessage.ToString();
                    }

                    if ((logViewForm != null) && (logText != ""))
                    {
                        if (level != 55)
                        {
                            logViewForm.AppendLog(logText + Environment.NewLine);
                        }
                    }
                    if ((optionsForm != null) && (logText != ""))
                    {
                        if (level != 55)
                        {
                            optionsForm.AppendLog(logText + Environment.NewLine);
                        }
                        

                    }

                }
            }
            else if (message.GetType() == typeof(Queue<Mails>))
            {
                int index = 0;
                foreach (Mails mail in message as Queue<Mails>)
                {
                    index++;

                    string message1 = "[" + managedThreadId + "] " + mail.ExchangeMailBox + " В очереди: " + index;
                    if (notifyIconMenu.InvokeRequired)
                    {
                        SetLogCallback d = new SetLogCallback(Log);
                        notifyIconMenu.Invoke(d, new object[] { level, message, mail, origMessage });
                        return;
                    }
                    

                    if (logWriter != null)
                    {
                        string logText = logWriter.Log(level, message1.ToString());
                        if (logText != "")
                        {
                            if (level != 55)
                            {
                                table.Rows.Add(logText + Environment.NewLine);
                            }
                            //mail.LastStatus = message.ToString();
                        }
                        else
                        {
                            if (level != 55)
                            {
                                table.Rows.Add(message1);
                            }
                            //mail.LastStatus = message.ToString();
                        }
                        if ((logViewForm != null) && (logText != ""))
                        {
                            if (level != 55)
                            {
                                logViewForm.AppendLog(logText + Environment.NewLine);
                            }
                        }
                        if ((optionsForm != null) && (logText != ""))
                        {
                            if (level != 55)
                            {
                                optionsForm.AppendLog(logText + Environment.NewLine);
                            }
                            

                        }

                    }
                }

            }
        }




        public bool AppInit()
        {
            AppStop();
            syncEvents.ExitThreadEvent.Reset();
            try
            {
                AppSettings.Load(ref appSettings);
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Ошибка загрузки настроек:" + " {0}", ex.Message));
                return false;
            }
            mailBindingSource = new BindingSource();
            mailBindingSource.DataSource = mainQueue;
            try
            {
                CreateAppDefaultFolder(Path.GetDirectoryName(appSettings.LogFileName));
                logWriter = new LogWriter(true, appSettings.LogLevel, appSettings.LogFileName, maxLogFileSize, maxLogFileCount);
                if (appSettings.LogClearOnStartup)
                    logWriter.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Ошибка открытия/создания файла лога:" + " {0}", ex.Message));
                return false;
            }
            if (appSettings.Sync)
            {

                threads.Clear();
                threads.Add(new Synchinc(mainQueue, this, appSettings, "Поток 1", syncEvents));
                threads.Add(new Synchinc(mainQueue, this, appSettings, "Поток 2", syncEvents));
                threads.Add(new Synchinc(mainQueue, this, appSettings, "Поток 3", syncEvents));

            }
            return true;
        }

        private Thread StopThread = null;

        public void AppStop()
        {
            syncEvents.ExitThreadEvent.Set();
            foreach (Synchinc syncThread in threads)
            {
                while (!syncThread.SyncThread.Join(10))
                {
                    Application.DoEvents();
                    //Thread.Sleep(10);
                }

            }
            if (logWriter != null)
                logWriter.Close();
            logWriter = null;
            

        }









        public void ClearLog()
        {
            if (logWriter != null)
                logWriter.Clear();
        }

        public IMAP2ExchSyncApplicationContext(string appSettingsFileName)
        {
            IMAP2ExchSyncApplicationContext.table.Columns.Add("LOG", typeof(string));
            string fileName = "";
            try
            {
                fileName = (appSettingsFileName != null ? appSettingsFileName : Path.Combine(AppSettings.AppDefaultFolder, "config.xml"));
                CreateAppDefaultFolder(Path.GetDirectoryName(fileName));
                appSettings = AppSettings.Load(fileName);

                if (appSettings.listMails.Count == 0)
                {

                    appSettings.listMails.Add(new Mails());
                }
                else
                {
                    if (appSettings.listMails[0] == null)
                    {
                        appSettings.listMails[0] = new Mails();
                    }
                }
                appSettings.Save();
               
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Ошибка записи/чтения настроек из файла" + " {0}: {1}", fileName, ex.Message));
                return;
            }
            int MailsIndex = 0;
            foreach (Mails mails in appSettings.listMails)
            {
                mails.sort = MailsIndex;
                MailsIndex++;
            }
            mainQueue = new Queue<Mails>(appSettings.listMails);


            notifyIconMenu = new ContextMenuStrip();

            notifyIconMenu.Items.Add("&Настройки...").Click += new EventHandler(OptionsMenuItem_Click);
            notifyIconMenu.Items.Add("&Лог...").Click += new EventHandler(LogMenuItem_Click);
            notifyIconMenu.Items.Add("В&ыход").Click += new EventHandler(ExitMenuItem_Click);

            notifyIcon = new NotifyIcon();
            notifyIcon.Text = AppSettings.AppName;
            notifyIcon.Icon = Properties.Resources.AppIconNormal;
            notifyIcon.ContextMenuStrip = notifyIconMenu;
            notifyIcon.DoubleClick += new EventHandler(NotifyIcon_DoubleClick);
            notifyIcon.Visible = true;

            CertificateCallback.Initialize();

            AppInit();

        }

        List<Synchinc> threads = new List<Synchinc>();
        SyncEvents syncEvents = new SyncEvents();

        public void OfflineSync(DateTime syncFromTime, DateTime syncToTime, int indexMails, int thread = -1)
        {
            Synchinc Thread1 = new Synchinc(mainQueue, this, appSettings, "Manual", syncEvents);
        }

        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            LogMenuItem_Click(sender, e);
        }

        private void OptionsMenuItem_Click(object sender, EventArgs e)
        {
            if (optionsForm != null)
            {
                optionsForm.Activate();
                return;
            }
            if (logViewForm != null)
            {
                logViewForm.Dispose();
                logViewForm = null;
            }

            using (optionsForm = new OptionsForm(appSettings, this))
            {
                if (optionsForm.ShowDialog() == DialogResult.OK)
                {
                    appSettings.Copy(optionsForm.GetSettings());
                    appSettings.Save();

                    AppInit();
                    //if (appSettings.Sync)
                    // SyncStart();
                    //else
                    //  SyncStop();
                }
            }
            optionsForm = null;
        }

        private void LogMenuItem_Click(object sender, EventArgs e)
        {
            if (logViewForm != null)
            {
                logViewForm.Activate();
                return;
            }
            if (optionsForm != null)
            {
                optionsForm.Dispose();
                optionsForm = null;
            }

            using (logViewForm = new LogViewForm())
            {
                if (logWriter != null)
                    logWriter.Load((msg) => logViewForm.AppendLog(msg));
                logViewForm.ShowDialog();
            }
            logViewForm = null;
        }

        void ExitMenuItem_Click(object sender, EventArgs e)
        {
            
            ExitThreadCore();
        }

        protected override void ExitThreadCore()
        {

            AppStop();
            Thread.Sleep(10000);
            if (logViewForm != null)
            {
                logViewForm.Dispose();
                logViewForm = null;
            }

            if (optionsForm != null)
            {
                optionsForm.Dispose();
                optionsForm = null;
            }

            if (notifyIcon != null)
            {
                notifyIcon.Dispose();
                notifyIcon = null;
            }

            //SyncStop();
            

            base.ExitThreadCore();
        
    }

        /*
        private static void ExitMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        public void OnApplicationExit(object sender, EventArgs e)
        {
            if (notifyIcon != null)
            {
                notifyIcon.Dispose();
                notifyIcon = null;
            }
        }
        */
    }
}
