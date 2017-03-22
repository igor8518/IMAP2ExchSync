using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;

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
        private IIMAP2ExchSync exchposer = null;
        private FileString syncId = null;
        public string threadName = "";
        public Mails mailObject = null;
        private SyncEvents syncEvents;
        public Thread SyncThread;
        public List<string> exchMessageIDs = new List<string>();
        public List<MailMessageIDs> mailMessageIDs = new List<MailMessageIDs>();
        public MailList<MailData> mailList = null;

        public Synchinc(Queue<Mails> queue, IIMAP2ExchSync exchposer, AppSettings appSettings, string name, SyncEvents syncEvents)
        {
            this.syncEvents = syncEvents;
            this.appSettings = appSettings;
            this.exchposer = exchposer;
            SyncThread = new Thread(delegate () { SyncStart(queue); });
            SyncThread.Name = name;
            threadName = name;
            SyncThread.Start();
        }

        protected void Log(int level, object message)
        {

            exchposer.Log(level, message);

        }

        public bool SyncInit(DateTime syncFromTime, DateTime syncToTime, int indexMails, bool offlineSync)
        {
            Log(1, mailObject.ExchangeMailBox + " Начало синхронизации");
            //Подключение к серверу Exchange
            CertificateCallback.AcceptInvalidCertificate = mailObject.ExchangeAcceptInvalidCertificate;
            exchangeServer = new ExchangeServer(mailObject.ExchangeUserName, mailObject.ExchangePassword,
            mailObject.ExchangeDomain, mailObject.ExchangeUrl, exchangeReconnectTimeout, exchposer, mailObject.ExchangeMailBox);
            try
            {
                exchangeServer.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Ошибка подключения к серверу Exchange:" + " {0}", ex.Message));
                return false;
            }
            /////////////////////
            //Подключение к внешнему серверу
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
            ///////////////////////////////
            //Если включена синхронизация на ящике и даты синхронизации соответствубт хронологии,
            //выполняем полную синхронизацию в пределах указанных дат
            if ((syncToTime > syncFromTime) && (mailObject.SyncEnabled))
            {
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

        public void SyncStart(Queue<Mails> queue)
        {
            Log(1, "Старт потока");
            while (!syncEvents.ExitThreadEvent.WaitOne(0, false))
            {
                try
                {
                    //Достаем объект ящика из очереди
                    lock (((ICollection)queue).SyncRoot)
                    {
                        mailObject = queue.Dequeue();
                        Log(11, queue);
                    }

                    //Загружаем список писем для скачивания из файла
                    mailList = MailList<MailData>.Load(mailObject);

                    //Инициализация указателей почтовых серверов
                    SyncStop();
                    //Определение пути файла последнего UID
                    syncId = new FileString(Path.Combine(AppSettings.AppDefaultFolder, mailObject.ExchangeMailBox + "_syncid"));
                    Log(1, "Обработка начата");
                    //Инициализация констант даты и времени
                    DateTime currentTime = DateTime.Now;
                    DateTime syncFromTime = DateTime.MinValue;
                    DateTime syncToTime = DateTime.MaxValue;
                    /* try
                     {
                         // syncFromTime = (syncId.Value != "" ? Convert.ToDateTime(syncId.Value).AddSeconds(1) : DateTime.MinValue);
                        // syncFromTime = DateTime.MinValue;
                     }
                     catch
                     {
                         MessageBox.Show(String.Format("Error converting sincId \"{0}\" to time. Will using current time", syncId));
                         syncFromTime = currentTime;
                     }*/

                    //if ((currentTime.Date - syncFromTime.Date).TotalDays > appSettings.MaxDaysToSync)

                    //Определение начальной даты синхронизации исходя из максимального количества дней синхронизации
                    if ((currentTime - syncFromTime).TotalDays > appSettings.MaxDaysToSync)
                        syncFromTime = currentTime.AddDays(-appSettings.MaxDaysToSync + 1).Date;

                    //Запуск полной синхронизации ящиков если необходимо
                    if (!SyncInit(syncFromTime, syncToTime, 0, false))
                    {
                        Log(1, "Обработка завершена syncFromTime не доступен: " + mailObject.ExchangeMailBox);
                        mailObject = null;
                        return;
                    }
                    //Запрос новых писем
                    if (!SearchNew())
                    {

                    }
                    mailList.Save();

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
                              Log(1, String.Format("Ошибка работы с сообщениями на Exchange сервере:" + " {0}", ex.Message));
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

            Log(1, "Поток завершен");
        }

        delegate void UpdateSettingCallback(Mails mailObject);
        void UpdateSetting(Mails mailObject)
        {
            appSettings.listMails[mailObject.sort] = mailObject;
            appSettings.Save();

        }

        private bool SearchNew()
        {
            string LastUID;
            LastUID = syncId.Load();
            
            if (LastUID != null)
            {
                mailMessageIDs.Clear();
                mailServer.Open();
                if (!mailServer.GetMessageIDs(LastUID, mailObject.MailFolderName, (MessageID) =>
                {
                    mailMessageIDs.Add(MessageID);

                }))
                {
                    mailMessageIDs.Clear();

                }               
                mailServer.Close();

                int count = 0;
                foreach (MailMessageIDs mailID in mailMessageIDs)
                {
                    MessageIDSearch ss = new MessageIDSearch(mailID.MessageID);
                    int findIndex = exchMessageIDs.FindIndex(ss.Equ);

                    if (findIndex < 0)
                    {
                        mailList.Add(new MailData(mailID.UIDMessage, mailID.MessageID, mailID.dateTime, mailID.Subject));
                        count++;                        
                    }

                }
                if (mailMessageIDs.Count > 0)
                {
                    syncId.Save(mailMessageIDs[mailMessageIDs.Count - 1].UIDMessage);
                }
                if (count == 0)
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

        
        void SetNotifyIcon(System.Drawing.Icon state, bool showTip = false)
        {
            
            if ((IMAP2ExchSyncApplicationContext.notifyIcon == null) || (exchangeServer == null) || (mailServer == null))
                return;

            IMAP2ExchSyncApplicationContext.notifyIcon.Icon = state;
            if (showTip)
            {
                IMAP2ExchSyncApplicationContext.notifyIcon.BalloonTipText = "Обработка: " + mailObject.ExchangeMailBox;
                // устанавливаем зголовк
                IMAP2ExchSyncApplicationContext.notifyIcon.BalloonTipTitle = "Информация";
                // отображаем подсказку 5 секунд
                IMAP2ExchSyncApplicationContext.notifyIcon.ShowBalloonTip(5);
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

                //Получаем список Message-ID из Exchange за указанный период для сравнения с внешним сервером
                exchMessageIDs.Clear();
                exchangeServer.ProcessMessages(fromTime, toTime, (msg) =>
                {
                    if (msg.InternetMessageId != "")
                    {
                        exchMessageIDs.Add(msg.InternetMessageId);
                        //ProcessExchangeMessage(msg, offlineSync);
                        //Application.DoEvents();
                    }
                });
                exchangeServer.Close();
                //////////////////////////////////////
                //Получаем список Message-ID,Subject,Date и пр. с внешнего сервера за указанный период для сравнения с Exchange
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
                    
                    if (findIndex<0)
                    {

                        mailList.Add(new MailData(mailID.UIDMessage, mailID.MessageID, DateTime.Now, "TEST"));
                        count++;
                    }
                    
                }
                Log(1, "Писем для скачивания: " + mailList.Count);
                //Thread.Sleep(5000);
                mailList.Save();
                syncId.Save(mailMessageIDs[mailMessageIDs.Count - 1].UIDMessage);
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
            Log(1, "Поток завершен");
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
