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
using System.Timers;
using System.Threading.Tasks;

namespace IMAP2ExchSync
{
    public class IMAP2ExchSyncApplicationContext : ApplicationContext, IIMAP2ExchSync
    {
        RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
        public BindingSource mailBindingSource { get; set; }
        
        private const int maxLogFileSize = 1024 * 1024 * 10;//Максимальный размер лога
        private const int maxLogFileCount = 15;//Максимальное количество файлов лога       
#if DEBUG
        private const int maxThread = 1;
#endif
        private const int exchangeReconnectTimeout = 10;
        //public Action<int, object, Mails, string> Logger;
        //private void NewLog(int level, bool Error = false, Mails mails = null)
        //public void Log(int level, object message, Mails m = null, string origMessage = "")
        private Action<LogObject> DefaultLogger = null;
        private System.Timers.Timer timer = null;
        public DataTable FullListMail = null;

        public int ExchangeReconnectTimeout
        {
            get
            {
                return exchangeReconnectTimeout;
            }
        }

        static AppSettings appSettings = null; //Класс настроек программы и список ящиков для обработки

        LogWriter logWriter = null; //Ссылка на логгер
        //MailServer mailServer = null;
        //ExchangeServer exchangeServer = null;

        //Icon AppIconNormal = new Icon("AppIconNormal.ico");
        //Icon AppIconBusy = new Icon("AppIconBusy.ico");

        public bool Initialized { get { return notifyIcon != null; } }

        public static NotifyIcon notifyIcon = null;
        public static ContextMenuStrip notifyIconMenu = null;
        public static OptionsForm optionsForm = null;
        public static MailsForm mailForm = null;
        LogViewForm logViewForm = null;
        Queue<Mails> mainQueue = null; //Основная очередь обработки

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



        delegate void SetLogCallback(string message = "", int type = 10, Mails mails = null, MailData md = null, int status = 0);
        private void CreateAppDefaultFolder(string folderName)
        {
            if (System.IO.Path.GetFullPath(folderName).Equals(System.IO.Path.GetFullPath(AppSettings.AppDefaultFolder)))
                Directory.CreateDirectory(folderName);
        }


        public static DataTable LogTableForm = new DataTable(); //Бинд к таблице лога в основной форме
        
        private Queue<LogObject> QueueLogs = null;

        private void NewLog(LogObject logobject)
        {
            //lock (((ICollection)QueueLogs).SyncRoot)
            while (QueueLogs.Count > 100)
            {

            }
            lock (QueueLogs)
            {
                //Thread.Sleep(1000);
                if (logobject.message == null)
                {
                    //MessageBox.Show("Error!!!");
                }
                QueueLogs.Enqueue(logobject);
            }
        }

        private void DequeueLog(Object source, ElapsedEventArgs e)
        {
            //lock (((ICollection)QueueLogs).SyncRoot)
            if (QueueLogs.Count > 0)
            {
                lock (QueueLogs)
                {

                    for (int i = 0; i < QueueLogs.Count; i++)
                    {
                        LogObject logObject = QueueLogs.Dequeue();

                        if (notifyIconMenu.InvokeRequired)
                        {
                            SetLogCallback d = new SetLogCallback(Log);
                            notifyIconMenu.Invoke(d, new object[] { logObject.message, logObject.type, logObject.mails, logObject.mailData, logObject.status });
                            return;
                        }
                        else
                        {
                            Application.DoEvents();
                            Log(logObject.message, logObject.type, logObject.mails, logObject.mailData, logObject.status);
                        }

                    }

                }
            }
            //throw new NotImplementedException();
        }


        private void logger(int level, string message)
        {
            LogObject logObject = new LogObject();
            logObject.message = message;
            logObject.type = level;
            DefaultLogger(logObject);
            logObject = null;
        }

        public void Log(string message = "", int type = 10, Mails m = null, MailData md = null, int status = 0)
        {
            if (message == null)
            {
                message = "";
            }
            string origMessage = message;
            string managedThreadId;
            if (m != null)
            {
                managedThreadId = m.Thread;
            }
            else
            {
                managedThreadId = "Неизв. поток";
            }
            // int managedThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
            string firstMessage = message;
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




                message = "[" + managedThreadId + "]" + mailbox + message;
                if (notifyIconMenu.InvokeRequired)
                {
                    
                    /*SetLogCallback d = new SetLogCallback(Log);
                    notifyIconMenu.Invoke(d, new object[] { type, message, mails, firstMessage });
                    return;*/
                }
                if (type == 66)
                {
                    FullListMail.Rows.Add(new object[] { md.dateTimeSend, md.ToName, md.ToAddress, md.FromName, md.FromAddress, md.subtitle, md.status, md.progress,md.GUID });                    
                    if (mailForm != null)
                    {
                        //mailForm.Refresh();
                    }
                }
                if (type == 55)
                {
                    /* object key = ((FullListMail.DefaultView)[4]);
                     int FindIndex = FullListMail.DefaultView.Find(key);
                     if (FindIndex >= 0)
                     {
                         FullListMail.Rows[FindIndex][4] = origMessage;
                     }*/
                    var FindRow = FullListMail.Select("GUID='" + md.GUID+"'");
                    if (FindRow.Length>0)
                    {
                        FindRow[0]["Progress"] = origMessage;
                    }
                }
                if (type == 44)
                {
                    /*int FindIndex = FullListMail.DefaultView.Find(m.currentMailWork.GUID);
                    if (FindIndex >= 0)
                    {
                        FullListMail.Rows[FindIndex][3] = m.currentMailWork.status;
                    }*/
                    var FindRow = FullListMail.Select("GUID='" + md.GUID + "'");
                    if (FindRow.Length > 0)
                    {
                        FindRow[0]["Status"] = status;
                        FindRow[0]["Time Sended"] = md.dateTimeSend;
                    }
                }
                if (logWriter != null)
                {
                    string logText = logWriter.Log(type, message);
                    if (logText != "")
                    {
                        if (type != 55)
                        {
                            LogTableForm.Rows.Add(logText + Environment.NewLine);
                        }
                        if (m != null)
                            m.LastStatus = origMessage.ToString();
                    }
                    else
                    {
                        if (type != 55)
                        {
                            LogTableForm.Rows.Add(message);
                        }
                        if (m != null)
                            m.LastStatus = origMessage.ToString();
                    }

                    if ((logViewForm != null) && (logText != ""))
                    {
                        if (type != 55)
                        {
                            logViewForm.AppendLog(logText + Environment.NewLine);
                        }
                    }
                    if ((optionsForm != null) && (logText != ""))
                    {
                        if (type != 55)
                        {
                            optionsForm.AppendLog(logText + Environment.NewLine);
                        }
                        

                    }

                }
            
        }

        public bool AppInit()
        {
            AppStop();
            FullListMail = new DataTable();
            //DataTable dt = new DataTable("Mails");
            //FullListMail.Table = dt;
            FullListMail.Columns.Add("Time Sended", typeof(DateTime));
            FullListMail.Columns.Add("ToName", typeof(string));
            FullListMail.Columns.Add("ToAddress", typeof(string));
            FullListMail.Columns.Add("FromName", typeof(string));
            FullListMail.Columns.Add("FromAddress", typeof(string));
            FullListMail.Columns.Add("Subtitle", typeof(string));
            FullListMail.Columns.Add("Status", typeof(int));
            FullListMail.Columns.Add("Progress", typeof(string));
            FullListMail.Columns.Add("GUID", typeof(string));
            mainQueue = new Queue<Mails>(appSettings.ListMails);
            for (int i=0;i<mainQueue.Count;i++)
            {
                MailList<MailData> currentMails = MailList<MailData>.Load(appSettings.ListMails[i]);
                if (currentMails != null)
                {
                    for (int k = 0; k < currentMails.Count; k++)
                    {
                        if ((currentMails[k].dateTimeSend >= DateTime.Now.AddDays(-7)) || (currentMails[k].status != 2))
                        {
                            if (currentMails[k].status == 1)
                            {
                                currentMails[k].progress = "Скачано";

                            }
                            else if (currentMails[k].status == 2)
                            {
                                currentMails[k].progress = "Отправлено";

                            }
                            if (currentMails[k].GUID == "")
                            {
                                currentMails[k].GUID = currentMails[k].messageID + appSettings.ListMails[i].MailUserName;
                            }

                            FullListMail.Rows.Add(new object[] { currentMails[k].dateTimeSend, currentMails[k].ToName, currentMails[k].ToAddress, currentMails[k].FromName, currentMails[k].FromAddress, currentMails[k].subtitle, currentMails[k].status, currentMails[k].progress, currentMails[k].GUID });
                        }
                    }
                }
                currentMails = null;
            }
            FullListMail.DefaultView.Sort = "Status, Time Sended DESC, Progress DESC";
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
                logWriter = new LogWriter(true, appSettings.DefLogLevel, appSettings.LogFileName, maxLogFileSize, maxLogFileCount);
                if (appSettings.DefLogClearOnStartup)
                    logWriter.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Ошибка открытия/создания файла лога:" + " {0}", ex.Message));
                return false;
            }
            if (appSettings.AutoSynch)
            {

                threads.Clear();
#if DEBUG
                    threads.Add(new Synchinc(mainQueue, this, appSettings, "Поток 1", syncEvents, DefaultLogger));

#else
                    for (int i = 0; i < appSettings.maxThreads; i++)
                    {
                        threads.Add(new Synchinc(mainQueue, this, appSettings, "Поток "+i.ToString(), syncEvents, DefaultLogger));
                    }
#endif


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
                }
            }
            while (QueueLogs.Count!=0)
            {
                Application.DoEvents();
            }
            if (FullListMail != null)
            {
                FullListMail.Clear();
                FullListMail.Dispose();
                FullListMail = null;
            }
            if (mainQueue != null)
            {
                mainQueue.Clear();
            }
            mainQueue = null;
            if (logWriter != null)
            {
                logWriter.Close();
            }
            logWriter = null;
        }

        public void ClearLog()
        {
            if (logWriter != null)
                logWriter.Clear();
        }
        //
        // Сводка:
        //     Конструктор главного класса в главном потоке
        //
        // Параметры:
        //   appSettingsFileName:
        //     Имя файла конфигурации
        public IMAP2ExchSyncApplicationContext(string appSettingsFileName)
        {
            LogTableForm.Columns.Add("LOG", typeof(string));
            
            DefaultLogger = NewLog;
            QueueLogs = new Queue<LogObject>();
            timer = new System.Timers.Timer(20);
            timer.Elapsed += DequeueLog;
            //timer.AutoReset = true;
            timer.Enabled = true;
            string configFileName = "";
            try
            {
                configFileName = (appSettingsFileName != null ? appSettingsFileName : Path.Combine(AppSettings.AppDefaultFolder, "config.xml"));
                CreateAppDefaultFolder(Path.GetDirectoryName(configFileName));
                appSettings = AppSettings.Load(configFileName);

                if (appSettings.ListMails.Count == 0)
                {
                    MessageBox.Show("Вам необходимо добавить почтовые ящики для синхронизации");
                }
                if (appSettings.ListMails.Count < appSettings.maxThreads)
                {
                    appSettings.maxThreads = appSettings.ListMails.Count;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Ошибка записи/чтения настроек из файла" + " {0}: {1}", configFileName, ex.Message));
                return;
            }
            //Присвоение порядка сортировки. Пока не знаю зачем, но пригодится.
            int MailsIndex = 0;
            foreach (Mails mails in appSettings.ListMails)
            {
                mails.sort = MailsIndex;
                MailsIndex++;
            }
            ///////////////////////////////////
            


            notifyIconMenu = new ContextMenuStrip();

            notifyIconMenu.Items.Add("&Мониторинг").Click += new EventHandler(MonitoringMenuItem_Click);
            notifyIconMenu.Items.Add("&Настройки...").Click += new EventHandler(OptionsMenuItem_Click);
            notifyIconMenu.Items.Add("&Лог...").Click += new EventHandler(LogMenuItem_Click);
            notifyIconMenu.Items.Add("В&ыход").Click += new EventHandler(ExitMenuItem_Click);
            notifyIconMenu.Items[notifyIconMenu.Items.Count - 1].Name = "Exit";

            notifyIcon = new NotifyIcon();
            notifyIcon.Text = AppSettings.AppName;
            notifyIcon.Icon = Properties.Resources.AppIconNormal;
            notifyIcon.ContextMenuStrip = notifyIconMenu;
            notifyIcon.DoubleClick += new EventHandler(NotifyIcon_DoubleClick);
            notifyIcon.Visible = true;

            CertificateCallback.Initialize();

            AppInit();
        }

        List<Synchinc> threads = new List<Synchinc>(); //Список потоков
        SyncEvents syncEvents = new SyncEvents();

        public void OfflineSync(DateTime syncFromTime, DateTime syncToTime, int indexMails, int thread = -1)
        {
            Synchinc Thread1 = new Synchinc(mainQueue, this, appSettings, "Manual", syncEvents, DefaultLogger);
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

            using (optionsForm = new OptionsForm(appSettings, this, logger))
            {
                if (optionsForm.ShowDialog() == DialogResult.OK)
                {
                    appSettings.Copy(optionsForm.GetSettings());
                    appSettings.Save();

                    AppInit();
                }
            }
            optionsForm = null;
        }

        private void MonitoringMenuItem_Click(object sender, EventArgs e)
        {
            if (mailForm != null)
            {
                mailForm.Activate();
                return;
            }
            if (optionsForm != null)
            {
                optionsForm.Dispose();
                optionsForm = null;
            }

            using (mailForm = new MailsForm(FullListMail.DefaultView))
            {
                mailForm.ShowDialog();
            }
            mailForm = null;
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
            notifyIconMenu.Items["Exit"].Enabled = false;
            ExitThreadCore();
        }

        protected override void ExitThreadCore()
        {

            AppStop();
            //Thread.Sleep(10000);
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
