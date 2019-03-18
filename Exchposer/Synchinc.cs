using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Net;

namespace IMAP2ExchSync
{
    public class SynchincSearch
    {
        String _s;

        public SynchincSearch(String s)
        {
            _s = s;
        }

        public bool Equ(Synchinc e)
        {
            return e.threadName.Equals(_s, StringComparison.CurrentCulture);
        }
    }

    public class MessageIDSearch
    {
        String _s;

        public MessageIDSearch(String s)
        {
            _s = s;
        }

        public bool Equ(string e)
        {
            return e.Equals(_s, StringComparison.CurrentCulture);
        }
    }
    public class Synchinc
    {
        private AppSettings appSettings = null;
        MailServer mailServer = null;
        ExchangeServer exchangeServer = null;
        private const int exchangeReconnectTimeout = 10;
        private const string MessageEndThread = "Поток завершен";
        private IIMAP2ExchSync exchposer = null;
        //private FileString syncId = null;
        public string threadName = "";
        public Mails mailObject = null;
        private SyncEvents syncEvents;
        public Thread SyncThread;
        public List<string> exchMessageIDs = new List<string>();
        public List<MailMessageIDs> mailMessageIDs = new List<MailMessageIDs>();
        public List<String> mailMessage = new List<string>();
        private BinaryWriter fileMessage = null;
        private FileStream fileMessageB = null;
        private StreamReader fileMessageRead = null;
        private Action<LogObject> logger = null;
        

        public MailList<MailData> mailList = null;
        public MailList<MailData> DownLoadedmailList = null;
        public List<MailData> MailsToRemove = null;
        public SmtpServer smtp = null;
        private string fileMessageName = "";

        public Synchinc(Queue<Mails> queue, IIMAP2ExchSync exchposer, AppSettings appSettings, string name, SyncEvents syncEvents, Action<LogObject> Logger)
        {
            logger = Logger;
            this.syncEvents = syncEvents;
            this.appSettings = appSettings;
            this.exchposer = exchposer;
            SyncThread = new Thread(delegate () { SyncStart(queue); });
            SyncThread.Name = name;
            SyncThread.Priority = ThreadPriority.Lowest;
            threadName = name;
            SyncThread.Start();
        }

        protected void Log(int level, string message)
        {
            LogObject logObject = new LogObject();
            logObject.mails = mailObject;
            if (mailObject != null)
            {
                logObject.mailData = mailObject.currentMailWork;
                if (mailObject.currentMailWork != null)
                {
                    logObject.status = mailObject.currentMailWork.status;
                }
            }
            logObject.message = message;
            logObject.type = level;
            logger(logObject);
            logObject = null;
        }

        public bool SyncInit(DateTime syncFromTime, DateTime syncToTime, int indexMails, bool offlineSync)
        {
            switch (mailObject.MailServerType)
            {
                case MailServerTypes.IMAP:
                    mailServer = new ImapServer(mailObject.MailServerName, mailObject.MailServerPort,
                        mailObject.MailUserName, mailObject.MailPassword,
                        (l, m) => { Log(l, m); });
                    break;

                default:
                    throw new InvalidOperationException("Не поддерживаемый тип внешнего сервера");
            }

            //Если включена синхронизация на ящике и даты синхронизации соответствубт хронологии,
            //выполняем полную синхронизацию в пределах указанных дат
            if ((syncToTime > syncFromTime) && (mailObject.SyncEnabled))
            {
                Log(1, mailObject.ExchangeMailBox + " Начало синхронизации");
                //Подключение к серверу Exchange
                CertificateCallback.AcceptInvalidCertificate = mailObject.ExchangeAcceptInvalidCertificate;
                exchangeServer = new ExchangeServer(mailObject.ExchangeUserName, mailObject.ExchangePassword,
                mailObject.ExchangeDomain, mailObject.ExchangeUrl, exchangeReconnectTimeout, exchposer, Log, mailObject.ExchangeMailBox);
                try
                {
                    exchangeServer.Open();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(String.Format("Ошибка подключения к серверу Exchange:" + " {0}", ex.Message));
                    return false;
                }

                //Подключение к внешнему серверу
                
                //Проводим полную синхронизацию             
                FullSyncExchangeMessages(syncFromTime, syncToTime, offlineSync);

                //После выключаем признак полной синхронизации на данный ящик
                if (IMAP2ExchSyncApplicationContext.notifyIconMenu.InvokeRequired)
                {

                    SetSyncCallback d = new SetSyncCallback(SetSync);
                    IMAP2ExchSyncApplicationContext.notifyIconMenu.Invoke(d, new object[] { mailObject, false });
                }
                else
                {
                    SetSync(mailObject, false);
                }

            }

            return true;
        }

        public string MailSend(string Server,int port, string user, string pass, string sender, string recipient, string fileToSend, string addHeader,string UID, string Size)
        {
            try 
            {
                smtp = new SmtpServer(Server, port, user, pass, Log);
                smtp.Open();
                smtp.Send(sender, recipient, fileToSend, addHeader, UID, Size);
                smtp.Close();
            }
            catch (Exception ex)
            {
                Log(1, ex.Message);
                return ex.Message ;
            }
            return "";
        }

        public static string GetKBMBGB(string Size)
        {
            string returnStr = "";
            double size = UInt64.Parse(Size);
            if (size>1000000000)
            {
                size = size / 1024/1024/1024;
                returnStr = size.ToString("###0.00GB");
            }
            else if (size > 1000000)
            {
                size = size / 1024/1024;
                returnStr = size.ToString("###0.00MB");
            }
            else if (size > 1000)
            {
                size = size / 1024;
                returnStr = size.ToString("###0.00KB");
            }
            else if (size >= 0)
            {                
                returnStr = size.ToString("###0B");
            }
            return returnStr;
        }

        public void SyncStart(Queue<Mails> queue)
        {
            Log(1, "Старт потока");
            while (!syncEvents.ExitThreadEvent.WaitOne(0, false))
            {
                Thread.Sleep(50);
                try
                {
                    //Достаем объект ящика из очереди
                    lock (((ICollection)queue).SyncRoot)
                    {
                        mailObject = queue.Dequeue();
                        mailObject.Thread = threadName;
                    }

                    //Загружаем список писем для скачивания из файла
                    mailList = MailList<MailData>.Load(mailObject);
                    DownLoadedmailList = MailList<MailData>.Load2(mailObject);

                    //Инициализация указателей почтовых серверов
                    SyncStop();

                    //Определение пути файла последнего UID
                    Log(1, "Обработка начата");

                    //Инициализация констант даты и времени
                    DateTime currentTime = DateTime.Now;
                    DateTime syncFromTime = DateTime.MinValue;
                    DateTime syncToTime = DateTime.MaxValue;

                    //Определение начальной даты синхронизации исходя из максимального количества дней синхронизации
                    if ((currentTime - syncFromTime).TotalDays > appSettings.DefMaxDaysFullSync)
                    {
                        syncFromTime = currentTime.AddDays(-appSettings.DefMaxDaysFullSync + 1).Date;
                    }

                    //Запуск полной синхронизации ящиков если необходимо
                    if (!SyncInit(syncFromTime, syncToTime, 0, false))
                    {
                        Log(1, "Обработка завершена syncFromTime не доступен: " + mailObject.ExchangeMailBox);
                        mailObject = null;
                        return;
                    }
                    
                    //Подключение к серверу
                    mailServer.Open();

                    //Запрос новых писем
                    if (!SearchNew())
                    {

                    }
                    //MailsToRemove = new List<MailData>();


                    for (int i = 0; i < mailList.Count; i++)
                    {
                        mailObject.currentMailWork = mailList[i];
                        if (syncEvents.ExitThreadEvent.WaitOne(0, false))
                        {
                            mailList.Save();
                            break;
                        }
                        fileMessageName = AppSettings.AppDefaultFolder + "\\DownLoadedMessages\\" + mailObject.ExchangeMailBox + "[" + mailList[i].UID + "]_" + mailList[i].dateTime.ToString("yyyy_MM_dd.HH.mm.ss") + ".eml";

                        //fileMessage.
                        mailMessage.Clear();
                        //List<Byte> mailMessageBinary = new List<byte>();
                        if (mailList[i].status == 0)
                        {
                            mailServer.Open();
                            long startOctet = 0;
                            //fileMessage = new BinaryWriter(new FileStream(fileMessageName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite));
                            fileMessageB = new FileStream(fileMessageName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                            //fileMessage.AutoFlush = true;
                            long downloaded = 0;
                            if (File.Exists(fileMessageName))
                            {
                                FileInfo file = new FileInfo(fileMessageName);
                                startOctet = file.Length;
                                downloaded = startOctet;
                            }
                            Log(55, "Скачивание: " + mailList[i].UID + " " + GetKBMBGB(mailList[i].Size));
                            DateTime TimerD = DateTime.Now;
                            SetNotifyIcon(Properties.Resources.AppIconBusy, 10, "Начало скачивания: " + mailList[i].subtitle + " " + GetKBMBGB(mailList[i].Size),true);
                            string DownloadReturnMessage = mailServer.DownloadMessage(mailList[i].UID, mailObject.MailFolderName, startOctet, (line) =>
                            {
                                if (syncEvents.ExitThreadEvent.WaitOne(0, false))
                                {
                                    return "Завершение работы программы";
                                }
                                //mailMessage.Add(line);
                                fileMessageB.Write(line, 0, line.Length);
                                fileMessageB.Flush();
                                downloaded += line.Length;
                                if (TimerD.AddMilliseconds(200) < DateTime.Now)
                                {
                                    SetNotifyIcon(Properties.Resources.AppIconBusy);
                                    if (startOctet == 0)
                                    {
                                        Log(55, "Скачивание: " + mailList[i].UID + " " + GetKBMBGB(downloaded.ToString()) + "/" + GetKBMBGB(mailList[i].Size));
                                    }
                                    else
                                    {
                                        Log(55, "Докачка: " + mailList[i].UID + " " + GetKBMBGB(downloaded.ToString()) + "/" + GetKBMBGB(mailList[i].Size));
                                    }
                                    TimerD = DateTime.Now;
                                }
                                return "";


                            });
                            if (DownloadReturnMessage == "")
                            {
                                if (downloaded >= long.Parse(mailList[i].Size))
                                {
                                    mailList[i].dateTimeDown = DateTime.Now;
                                    mailList[i].status = 1;
                                    
                                    Log(44, "");
                                    mailList.Save();
                                    downloaded = 0;
                                    SetNotifyIcon(Properties.Resources.AppIconNormal, 10, "Конец скачивания: " + mailList[i].subtitle + " " + GetKBMBGB(mailList[i].Size), true);
                                    Log(55, "Скачано");
                                }
                                else
                                {
                                    Log(55, "Ожидание докачки");
                                }
                            }
                            else
                            {
                                if (fileMessageB != null)
                                {
                                    fileMessageB.Close();
                                    fileMessageB.Dispose();
                                    fileMessageB = null;
                                }
                                mailList.Save();
                                downloaded = 0;
                                startOctet = 0;
                                SetNotifyIcon(Properties.Resources.AppIconNormal, 10, "Ошибка скачивания: " + mailList[i].subtitle + " " + GetKBMBGB(mailList[i].Size), true);
                                Log(55, "Не удалось скачать");
                                /*try
                                {

                                    File.Delete(fileMessageName);
                                }
                                catch (Exception ex)
                                {
                                    Log(1, ex.MessageEndThread);
                                }*/
                            }
                            if (fileMessageB != null)
                            {
                                fileMessageB.Close();
                                fileMessageB.Dispose();
                                fileMessageB = null;
                            }
                            mailServer.Close();
                        }
                        //Received: from imap.yandex.ru ([127.0.0.1])
                        //  by mailserv.maxim-td.ru with IMAP id l1x0Pn2c;
                        //  Sun, 2 Apr 2017 14:06:05 +0300
                        if (mailList[i].status == 1)
                        {

                            if (File.Exists(fileMessageName))
                            {
                                FileInfo file = new FileInfo(fileMessageName);
                                if (file.Length >= long.Parse(mailList[i].Size))
                                {
                                    //mailServer.Open();
                                    string AddHeader = "Received: from " + mailObject.MailServerName + " (" + mailObject.MailServerName + " [" + Dns.GetHostEntry(mailObject.MailServerName).AddressList[0].ToString() + "])\n" +
                                    "\tby mailserv.maxim-td.ru with " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + " id " + System.Reflection.Assembly.GetExecutingAssembly().ImageRuntimeVersion + ";\n" +
                                    "\t" + mailList[i].dateTimeDown.ToUniversalTime().ToString("R");
                                    Log(55, "Отправка: " + mailList[i].UID + " " + GetKBMBGB((int.Parse(mailList[i].Size) + AddHeader.Length).ToString()));
                                    SetNotifyIcon(Properties.Resources.AppIconBusy, 10, "Начало отправки: " + mailList[i].subtitle + " " + GetKBMBGB((int.Parse(mailList[i].Size) + AddHeader.Length).ToString()), true);
                                    string hrs = MailSend("192.168.51.51", 25, mailObject.ExchangeDomain + "\\" + mailObject.ExchangeUserName, mailObject.ExchangePassword, "sysadmin3@uk-kvazar.ru", mailObject.ExchangeMailBox, fileMessageName, AddHeader, mailList[i].UID, (int.Parse(mailList[i].Size) + AddHeader.Length + 2).ToString());
                                    if (hrs == "")
                                    {
                                        SetNotifyIcon(Properties.Resources.AppIconNormal, 10, "Конец отправки: " + mailList[i].subtitle + " " + GetKBMBGB((int.Parse(mailList[i].Size) + AddHeader.Length).ToString()), true);
                                        mailList[i].dateTimeSend = DateTime.Now;
                                        mailList[i].status = 2;
                                        Log(44, "");
                                        mailList.Save();
                                        Log(55, "Отправлено");
                                        //File.Delete(fileMessageName);
                                    }
                                    else
                                    {
                                        SetNotifyIcon(Properties.Resources.AppIconNormal, 10, "Не удалось отправить: " + mailList[i].subtitle + " " + GetKBMBGB((int.Parse(mailList[i].Size) + AddHeader.Length).ToString()), true);
                                        Log(55, "Не удалось отправить " + hrs);
                                    }
                                    mailServer.Close();
                                }
                                else
                                {
                                    
                                    mailList[i].status = 0;
                                    mailList.Save();
                                }
                            }
                            else
                            {
                                mailList[i].status = 0;
                                mailList.Save();
                            }
                        }

                        if ((mailList[i].status == 2) && (mailList[i].dateTimeSend < (DateTime.Now.AddMinutes(-15.0))))
                        {
                            if (File.Exists(fileMessageName))
                            {
                                try
                                {


                                    File.Delete(fileMessageName);
                                }
                                catch (Exception ex)
                                {
                                    Log(1, ex.Message);
                                }
                            }
                        }
                    }
                   /* foreach (MailData MailD in MailsToRemove)
                    {
                        mailList.Remove(MailD);
                        DownLoadedmailList.Remove(MailD);
                        MailD.dateTimeDown = DateTime.Now;
                        MailD.status = 1;
                        DownLoadedmailList.Add(MailD);

                    }
                    DownLoadedmailList.Save();*/
                    mailList.Save();
                    mailServer.Close();



                    /*  exchangeServer.StartStreamingNotifications(msg =>
                      {
                          try
                          {
                              mailServer.Open();
                              ProcessExchangeMessage(msg, false);
                              mailServer.Close();
                          }
                          catch (Exception ex)
                          {
                              Log(1, String.Format("Ошибка работы с сообщениями на Exchange сервере:" + " {0}", ex.MessageEndThread));
                          }
                      }, mailObject.ExchangeSubscriptionLifetime);*/

                    lock (((ICollection)queue).SyncRoot)
                    {

                        queue.Enqueue(mailObject);
                    }
                    if (IMAP2ExchSyncApplicationContext.optionsForm != null)
                    {
                        if (IMAP2ExchSyncApplicationContext.optionsForm.InvokeRequired)
                        {

                            UpdateSettingCallback d = new UpdateSettingCallback(UpdateSetting);
                            IMAP2ExchSyncApplicationContext.optionsForm.Invoke(d, new object[] { mailObject });
                        }
                        else
                        {
                            UpdateSetting(mailObject);
                        }
                    }
                    else
                    {
                        UpdateSetting(mailObject);
                    }
                    
                    Log(1, "Обработка завершена");
                }
                catch (Exception ex)
                {
                    Log(1, ex.Message);
                    queue.Enqueue(mailObject);
                }
                finally
                {
                    mailObject = null;
                }
            }

            Log(1, MessageEndThread);
        }

        delegate void UpdateSettingCallback(Mails mailObject);
        void UpdateSetting(Mails mailObject)
        {
            appSettings.ListMails[mailObject.sort] = mailObject;
            appSettings.Save();

        }

        private bool SearchNew()
        {
            string LastUID;
            //LastUID = syncId.Load();
            LastUID = (mailList.GetMaxUID()+1).ToString();
            if ((LastUID != null)&&(LastUID != "1"))
            {
                mailMessageIDs.Clear();
                
                if (!mailServer.GetMessageIDs(LastUID, mailObject.MailFolderName, (MessageID) =>
                {
                    mailMessageIDs.Add(MessageID);

                }))
                {
                    mailMessageIDs.Clear();

                }               
                

                int count = 0;
                foreach (MailMessageIDs mailID in mailMessageIDs)
                {
                    //MessageIDSearch ss = new MessageIDSearch(mailID.MessageID);
                    //int findIndex = exchMessageIDs.FindIndex(ss.Equ);

                   // if (findIndex < 0)
                   // {
                    mailList.Add(new MailData(mailID.UIDMessage, mailID.MessageID, mailID.dateTime, mailID.Subject, mailID.Size, mailObject.MailUserName, mailObject.ExchangeMailBox, mailID.FromName, mailID.FromAddress, mailID.MessageID+mailObject.MailUserName));
                    mailObject.currentMailWork = mailList[mailList.Count - 1];
                    Log(66, count.ToString());
                    count++;                        


                }
                mailList.Save();
                if (mailMessageIDs.Count > 0)
                {
                    int maxUID = 0;
                    for (int i=0; i<mailMessageIDs.Count;i++)
                    {
                        if (int.Parse(mailMessageIDs[i].UIDMessage) > maxUID)
                        {
                            maxUID = int.Parse(mailMessageIDs[i].UIDMessage);
                        }
                    }
                    //syncId.Save((maxUID+1).ToString());
                }
                if (count != 0)
                {
                    Log(1, "Новых писем: " + count);
                    return true;
                }
                else
                {
                    Log(1, "Нет новых писем");
                    return true;
                }
            }
            else
            {
                if (IMAP2ExchSyncApplicationContext.notifyIconMenu.InvokeRequired)
                {

                    SetSyncCallback d = new SetSyncCallback(SetSync);
                    IMAP2ExchSyncApplicationContext.notifyIconMenu.Invoke(d, new object[] { mailObject, true });
                }
                else
                {
                    SetSync(mailObject,true);
                }
                return false;
            }
            
        }
        delegate void SetSyncCallback(Mails mailObject, bool Sync);
        void SetSync(Mails mailObject, bool Sync)
        {
            mailObject.SyncEnabled = Sync;

        }


        void SetNotifyIcon(System.Drawing.Icon state, int showTime = 0, string Text= "", bool showTip = false)
        {
            
            if ((IMAP2ExchSyncApplicationContext.notifyIcon == null) || (exchangeServer == null) || (mailServer == null))
                return;

            IMAP2ExchSyncApplicationContext.notifyIcon.Icon = state;
            if (showTip)
            {
                IMAP2ExchSyncApplicationContext.notifyIcon.BalloonTipText = Text;
                // устанавливаем зголовк
                IMAP2ExchSyncApplicationContext.notifyIcon.BalloonTipTitle = "Информация";
                // отображаем подсказку 5 секунд
                IMAP2ExchSyncApplicationContext.notifyIcon.ShowBalloonTip(showTime);
            }
        }

       /* void ProcessExchangeMessage(EmailMessage msg, bool offlineSync)
        {

            SetNotifyIcon(Properties.Resources.AppIconBusy,true);

             exchangeServer.LoadMessage(msg);

            Log(11, String.Format("Exchange message processing. Time: {0}, Subject: {1}", msg.DateTimeReceived.ToString(), msg.Subject));

             string fromAddress = Regex.Match(msg.Sender.ToString(), "[a-zA-Z0-9_.+-]+@33[a-zA-Z0-9-]+\\.[a-zA-Z0-9-.]+", RegexOptions.None).ToString();
             //mailServer.Send(fromAddress, appSettings.MailToAddress, appSettings.MailFolderName, msg.MimeContent.ToString());

             if (!offlineSync)
             {
                 syncId.Value = msg.DateTimeReceived.ToString();
                 appSettings.Save();
             }
            Thread.Sleep(5000);
            SetNotifyIcon(Properties.Resources.AppIconNormal);

        }*/
        
        
        void FullSyncExchangeMessages(DateTime fromTime, DateTime toTime, bool offlineSync)
        {
            try
            {

                //Получаем список MessageEndThread-ID из Exchange за указанный период для сравнения с внешним сервером
                exchMessageIDs.Clear();
               /* exchangeServer.ProcessMessages(fromTime, toTime, (msg) =>
                {
                    if (msg.InternetMessageId != "")
                    {
                        exchMessageIDs.Add(msg.InternetMessageId);
                        //ProcessExchangeMessage(msg, offlineSync);
                        //Application.DoEvents();
                    }
                });
                exchangeServer.Close();*/
                //////////////////////////////////////
                //Получаем список MessageEndThread-ID,Subject,Date и пр. с внешнего сервера за указанный период для сравнения с Exchange
                mailMessageIDs.Clear();
                mailServer.Open();
                if (!mailServer.GetMessageIDs(fromTime, toTime, mailObject.MailFolderName, (MessageID) =>
                {
                    mailMessageIDs.Add(MessageID);
                    
                }))
                {
                    mailMessageIDs.Clear();

                }
                mailServer.Close();
                //////////////////////////////////////////////
                
                //Очистка списка сообщений на загрузку
                mailList.Clear();
                //Количество добавляемых элементов
                int count = 0;
                //Сверка 
                foreach (MailMessageIDs mailID in mailMessageIDs)
                {
                    MessageIDSearch ss = new MessageIDSearch(mailID.MessageID);
                    int findIndex = exchMessageIDs.FindIndex(ss.Equ);
                    //mailList.Add(new MailData(mailID.UIDMessage, mailID.MessageID, mailID.dateTime, mailID.Subject, mailID.Size));
                    mailList.Add(new MailData(mailID.UIDMessage, mailID.MessageID, mailID.dateTime, mailID.Subject, mailID.Size, mailObject.MailUserName, mailObject.ExchangeMailBox, mailID.FromName, mailID.FromAddress, mailID.MessageID + mailObject.MailUserName));
                    mailList[mailList.Count - 1].status = 2;
                    mailObject.currentMailWork = mailList[mailList.Count - 1];
                    Log(66, count.ToString());
                    if (findIndex<0)
                    {

                        mailList[mailList.Count - 1].status = 0;
                        count++;
                    }
                    
                }
                Log(1, "Писем для скачивания: " + mailList.Count);
                //Thread.Sleep(5000);
                mailList.Save();
                //syncId.Save(mailMessageIDs[mailMessageIDs.Count - 1].UIDMessage);
                if (IMAP2ExchSyncApplicationContext.notifyIconMenu.InvokeRequired)
                {

                    SetSyncCallback d = new SetSyncCallback(SetSync);
                    IMAP2ExchSyncApplicationContext.notifyIconMenu.Invoke(d, new object[] { mailObject, false });
                }
                else
                {
                    SetSync(mailObject, false);
                }

            }
            catch (Exception ex)
            {
                Log(1, String.Format("Ошибка получения идентификаторов сообщений в Exchange server: {0}", ex.Message));
            }
        }

        public void SyncStop()
        {
            if (exchangeServer != null)
                exchangeServer.Close();
            exchangeServer = null;

            if (mailServer != null)
                mailServer.Close();
            mailServer = null;
        }

        public void OfflineSync(DateTime syncFromTime, DateTime syncToTime, int indexMails, int thread = -1)
        {
            if (thread >= 0)
            {
                thread = System.Threading.Thread.CurrentThread.ManagedThreadId;
                Log(1, "Создан новый поток: " + thread);
                if (exchangeServer == null)
                {
                    SyncInit(syncFromTime, syncToTime, indexMails, true);
                    SyncStop();
                }
                else
                    if (syncToTime > syncFromTime)
                    FullSyncExchangeMessages(syncFromTime, syncToTime, true);
            }
            else
            {
                Queue<int> queue = new Queue<int>();
                SyncEvents syncEvents = new SyncEvents();
                Log(1, "Создаем поток");
                //Thread manualSyncThread = new Thread(delegate () { OfflineSync(syncFromTime, syncToTime, indexMails,0); });
                //manualSyncThread.Name = "Manual thread";
                //manualSyncThread.Start();
                Log(1, "Configuring worker threads...");
                ProducerTest producer = new ProducerTest(queue, syncEvents, exchposer);
                ConsumerTest consumer = new ConsumerTest(queue, syncEvents, exchposer);
                Thread producerThread = new Thread(producer.ThreadRun);
                Thread consumerThread = new Thread(consumer.ThreadRun);
                Log(1, "Launching producer and consumer threads...");
                producerThread.Start();
                consumerThread.Start();
                for (int i = 0; i < 4; i++)
                {
                    Thread.Sleep(2500);
                    ShowQueueContents(queue);
                }
                Log(1, "Signaling threads to terminate...");
                syncEvents.ExitThreadEvent.Set();
                //producerThread.Join();
                //consumerThread.Join();
            }
            Log(1, MessageEndThread);
        }

        private void ShowQueueContents(Queue<int> q)
        {
            lock (((ICollection)q).SyncRoot)
            {
                foreach (int item in q)
                {
                    Log(1, string.Format("{0} ", item));
                }
            }
            Log(1, "End Show");
        }
    }
}
