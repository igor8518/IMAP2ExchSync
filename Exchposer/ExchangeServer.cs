using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Microsoft.Exchange.WebServices.Data;

namespace IMAP2ExchSync
{
    public class ExchangeServer
    {
        const int findPageSize = 100;

        private IIMAP2ExchSync exchposer = null;
        private string exchangeUserName = null;
        private string exchangePassword = null;
        private string exchangeDomain = null;
        private string exchangeUrl = null;
        private string exchangeUserMailbox = null;
        
        private ExchangeService service = null;

       // private StreamingSubscriptionConnection subscriptionConnection = null;
       // private StreamingSubscription streamingSubscription = null;
      //  private Action<EmailMessage> onReceive = null;
      //  private int subscriptionConnectionLifetime = 0;

       // private readonly Action<int, string> logger = null;

       // private Timer restartTimer = new Timer();

        protected void Log(int level, string message)
        {

           exchposer.Log(level, message);
           // logger?.Invoke(level, message);
        }

        public ExchangeServer(string exchangeUserName, string exchangePassword, string exchangeDomain, string exchangeUrl,
            int restartTimeout, IIMAP2ExchSync exchposer, string UserMailbox = "")
        {
            this.exchposer = exchposer;
            //this.logger = logger;
            this.exchangeUserName = exchangeUserName;
            this.exchangePassword = exchangePassword;
            this.exchangeDomain = exchangeDomain;
            this.exchangeUrl = exchangeUrl;
            exchangeUserMailbox = UserMailbox;

           // restartTimer.Elapsed += new ElapsedEventHandler(restartTimer_Elapsed);
           // restartTimer.Interval = restartTimeout * 1000;
           // restartTimer.AutoReset = false;
        }

        public void Open()
        {
            try
            {
                Close();

                service = new ExchangeService(ExchangeVersion.Exchange2013);
                service.Credentials = new WebCredentials(exchangeUserName, exchangePassword, exchangeDomain);
                service.Url = new Uri(exchangeUrl);
                if (exchangeUserMailbox != "")
                {
                    service.ImpersonatedUserId = new ImpersonatedUserId(ConnectingIdType.SmtpAddress, exchangeUserMailbox);
                }
                Log(2, "Exchange server opened");
            }
            catch (Exception ex)
            {
                Log(1, String.Format("Exchange server open error: {0}", ex.Message));
                service = null;

                throw;
            }
        }

        public void Close()
        {
           // Log(2, "Exchange server closing...");

            //StopStreamingNotifications();

            //this.onReceive = null;
           // this.subscriptionConnectionLifetime = 0;

            if (service != null)
                Log(2, "Exchange server closed");
            service = null;
        }

        public SearchableMailbox[] GetMailboxes()
        {
            GetSearchableMailboxesResponse resposneSearchableMailbox = service.GetSearchableMailboxes("", false);
            return resposneSearchableMailbox.SearchableMailboxes;


        }         

        
        public void ProcessMessages(DateTime fromTime, DateTime toTime, Action<EmailMessage> messageAction)
        {
            if (messageAction == null)
                return;

            if (service == null)
            {
                Log(2, "Exchange server is null");
                return;
            }
            DateTime fromTimeRound = fromTime.Date;

            //////////////////////
            ExtendedPropertyDefinition allFoldersType = new ExtendedPropertyDefinition(13825, MapiPropertyType.Integer);

            FolderId rootFolderId = new FolderId(WellKnownFolderName.Root);
            FolderView folderView = new FolderView(1000);
            folderView.Traversal = FolderTraversal.Shallow;

            //SearchFilter searchFilter1 = new SearchFilter.IsEqualTo(allFoldersType, "2");
            //SearchFilter searchFilter1 = new SearchFilter.IsEqualTo(FolderSchema.DisplayName, "Deletions");
            SearchFilter searchFilter2 = new SearchFilter.IsEqualTo(FolderSchema.DisplayName, "Allitems");
            SearchFilter searchFilter1 = new SearchFilter.IsEqualTo(allFoldersType, "2");
            SearchFilter.SearchFilterCollection searchFilterCollection = new SearchFilter.SearchFilterCollection(LogicalOperator.And);
            searchFilterCollection.Add(searchFilter1);
            searchFilterCollection.Add(searchFilter2);

            FindFoldersResults findFoldersResults = service.FindFolders(rootFolderId, searchFilterCollection, folderView);

            if (findFoldersResults.Folders.Count > 0)
            {
                Folder allItemsFolder = findFoldersResults.Folders[0];
                Console.WriteLine("Folder:\t" + allItemsFolder.DisplayName);

                ItemView iv = new ItemView(1000);
                //////////////////////////
                //Folder Root = Folder.Bind(service, WellKnownFolderName.Root);

                SearchFilter sf = new SearchFilter.SearchFilterCollection(LogicalOperator.And,
                    new SearchFilter.IsGreaterThanOrEqualTo(EmailMessageSchema.DateTimeReceived, fromTimeRound),
                    new SearchFilter.IsLessThanOrEqualTo(EmailMessageSchema.DateTimeReceived, toTime));

                for (int findOffset = 0; ; findOffset += findPageSize)
                {
                    ItemView view = new ItemView(findPageSize, findOffset, OffsetBasePoint.Beginning); ;
                    view.PropertySet = new PropertySet(BasePropertySet.IdOnly, ItemSchema.Subject, ItemSchema.DateTimeReceived, EmailMessageSchema.InternetMessageId);
                    view.OrderBy.Add(EmailMessageSchema.DateTimeReceived, SortDirection.Descending);

                    //FindItemsResults<Item> findResults = service.FindItems(WellKnownFolderName.Root, sf, view);
                    FindItemsResults<Item> findResults = allItemsFolder.FindItems(sf, view);

                    foreach (Item msg in findResults)
                    {
                        if ((msg.GetType() == typeof(EmailMessage)))
                        {
                            messageAction(msg as EmailMessage);
                        }
                    }

                    if (!findResults.MoreAvailable)
                        break;
                }
                /////////////
            }
            //////////////////
        }


        public void LoadMessage(EmailMessage msg)
        {
            msg.Load(new PropertySet(new PropertyDefinitionBase[] {
                    EmailMessageSchema.DateTimeReceived,
                    EmailMessageSchema.Id,
                    EmailMessageSchema.InternetMessageId,
                    EmailMessageSchema.MimeContent,
                    EmailMessageSchema.From,
                    EmailMessageSchema.Sender,
                    EmailMessageSchema.ToRecipients,
                    EmailMessageSchema.Subject
                }));
        }

      /*  public void StartStreamingNotifications(Action<EmailMessage> onReceive, int lifetime)
        {
            this.onReceive = onReceive;
            this.subscriptionConnectionLifetime = lifetime;

            StartStreamingNotifications();
        }*/

     /*   public void StartStreamingNotifications()
        {
            if (service == null)
            {
                Log(2, "Exchange server is null");
                return;
            }

            StopStreamingNotifications();

            try
            {
                subscriptionConnection = new StreamingSubscriptionConnection(service, subscriptionConnectionLifetime);
                subscriptionConnection.OnNotificationEvent +=
                    new StreamingSubscriptionConnection.NotificationEventDelegate(OnEvent);
                subscriptionConnection.OnSubscriptionError +=
                    new StreamingSubscriptionConnection.SubscriptionErrorDelegate(OnError);
                subscriptionConnection.OnDisconnect +=
                    new StreamingSubscriptionConnection.SubscriptionErrorDelegate(OnDisconnect);
            }
            catch (Exception ex)
            {
                Log(1, String.Format("Exchange subscription connection creating error: {0}", ex.Message));
                if (subscriptionConnection != null)
                    subscriptionConnection.Dispose();
                subscriptionConnection = null;
                return;
            }

            try
            {
                subscriptionConnection.AddSubscription(streamingSubscription = service.SubscribeToStreamingNotifications(
                    new FolderId[] { WellKnownFolderName.Inbox }, EventType.NewMail, EventType.Created, EventType.Deleted));
                subscriptionConnection.Open();
            }
            catch (Exception ex)
            {
                Log(1, String.Format("Exchange subscription creating error: {0}", ex.Message));
                RestartStreamingNotifications();
                return;
            }

            Log(2, "Exchange subscription started");
        }*/

    /*    public void StopStreamingNotifications()
        {
            restartTimer.Stop();

            if (streamingSubscription != null)
                try
                {
                    var tmpStreamingSubscription = streamingSubscription;
                    streamingSubscription = null;
                    tmpStreamingSubscription.Unsubscribe();
                }
                catch { }

            if (subscriptionConnection != null)
                try
                {
                    if (subscriptionConnection.IsOpen)
                        subscriptionConnection.Close();
                    subscriptionConnection.Dispose();
                    subscriptionConnection = null;
                    Log(2, "Exchange subscription stopped");
                }
                catch { }
        }*/

     /*   public void GetSMTPs()
        {
            
        }*/

      /*  public void RestartStreamingNotifications()
        {
            StopStreamingNotifications();
            if (restartTimer.Interval != 0)
                restartTimer.Start();
        }*/

       /* private void restartTimer_Elapsed(object source, ElapsedEventArgs e)
        {
            Log(2, "Restarting exchange streaming notification...");
            StartStreamingNotifications();
        }*/

       /* private void OnEvent(object sender, NotificationEventArgs args)
        {
            StreamingSubscription subscription = args.Subscription;

            foreach (NotificationEvent notification in args.Events)
            {
                string logInfo = (notification is ItemEvent ?
                    "ItemId: " + ((ItemEvent)notification).ItemId.UniqueId :
                    "FolderId: " + ((FolderEvent)notification).FolderId.UniqueId);

                switch (notification.EventType)
                {
                    case EventType.NewMail:
                        Log(20, String.Format("Exchange subscription event: new mail ({0})", logInfo));

                        ItemEvent itemEvent = (ItemEvent)notification;
                        EmailMessage msg = EmailMessage.Bind(service, itemEvent.ItemId, new PropertySet(BasePropertySet.IdOnly, ItemSchema.Subject, ItemSchema.DateTimeReceived));

                        if (onReceive != null)
                            onReceive(msg);

                        break;

                    case EventType.Created:
                        Log(20, String.Format("Exchange subscription event: item or folder created ({0})", logInfo));
                        break;

                    case EventType.Deleted:
                        Log(20, String.Format("Exchange subscription event: item or folder deleted ({0})", logInfo));
                        break;
                }
            }
        }*/

       /* private void OnError(object sender, SubscriptionErrorEventArgs args)
        {
            if (streamingSubscription == null)
                return;

            Log(20, String.Format("Exchange subscription error: {0} Restartting subscribtion...", args.Exception.Message));
            RestartStreamingNotifications();
/*
            try
            {
                if (service != null)
                    subscriptionConnection.AddSubscription(streamingSubscription = service.SubscribeToStreamingNotifications(
                        new FolderId[] { WellKnownFolderName.Inbox }, EventType.NewMail, EventType.Created, EventType.Deleted));
                //subscriptionConnection.Open();
            }
            catch (Exception ex)
            {
                Log(1, String.Format("Exchange subscription creating error: {0}", ex.Message));
                return;
            }

            Log(2, "Exchange subscription started");
 */
       /* }*/

      /*  private void OnDisconnect(object sender, SubscriptionErrorEventArgs args)
        {
            if (streamingSubscription == null)
                return;

            Log(3, String.Format("Exchange subscription disconnected. Reconnecting..."));

            try
            {
                StreamingSubscriptionConnection connection = (StreamingSubscriptionConnection)sender;
                connection.Open();
            }
            catch (Exception ex)
            {
                Log(1, String.Format("Exchange subscription connection open error: {0}", ex.Message));                
                RestartStreamingNotifications();
                return;
            }
        }*/
    }
}
