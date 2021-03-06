﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Microsoft.Exchange.WebServices.Data;
using System.Text.RegularExpressions;

namespace IMAP2ExchSync
{
    public partial class OptionsForm : Form
    {
        private Action<int, string> logger;
        public AppSettings appSetting { get; set; } = new AppSettings();
        private IIMAP2ExchSync exchposer = null;

        public OptionsForm()
        {
            InitializeComponent();
        }

        protected void Log(int level, string message)
        {

            logger(level, message);

        }

        public OptionsForm(AppSettings appSetting, IIMAP2ExchSync exchposer, Action<int, string> logger)
        {
            this.logger = logger;
            InitializeComponent();
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = exchposer.mailBindingSource;
            this.appSetting = appSetting;
            //var bindingList = new BindingList<Mails>(appSetting.ListMails);
            //var source = new BindingSource(bindingList, null);
            //mailsBindingSource.DataSource = appSetting.ListMails;
            //dataGridView1.DataSource = mailsBindingSource;
            this.exchposer = exchposer;
            if (appSetting.ListMails.Count > 0)
            {
                txtExchangeUrl.Text = appSetting.ListMails[0].ExchangeUrl;
                txtExchangeDomain.Text = appSetting.ListMails[0].ExchangeDomain;
                txtExchangeUserName.Text = appSetting.ListMails[0].ExchangeUserName;
                txtExchangePassword.Text = appSetting.ListMails[0].ExchangePassword;
                updExchangeSubscriptionLifetime.Value = appSetting.ListMails[0].ExchangeSubscriptionLifetime;
                chkExchangeAcceptInvalidCertificate.Checked = appSetting.ListMails[0].ExchangeAcceptInvalidCertificate;

                txtMailServerName.Text = appSetting.ListMails[0].MailServerName;
                txtMailServerPort.Text = appSetting.ListMails[0].MailServerPort.ToString();
                cboMailServerType.SelectedIndex = (int)appSetting.ListMails[0].MailServerType - 1;
                txtMailUserName.Text = appSetting.ListMails[0].MailUserName;
                txtMailPassword.Text = appSetting.ListMails[0].MailPassword;
                txtMailToAddress.Text = appSetting.ListMails[0].MailToAddress;
                txtMailFolderName.Text = appSetting.ListMails[0].MailFolderName;
                txtMailboxList.Items.Clear();
                if (appSetting.ListMails[0].ExchangeMailBox != "")
                {
                    txtMailboxList.Items.Add(appSetting.ListMails[0].ExchangeMailBox);
                    txtMailboxList.SelectedIndex = 0;
                }

                chkSyncEnabled.Checked = appSetting.ListMails[0].SyncEnabled;
            }
            cbMailFilter.Items.Add("");
            foreach (Mails m in appSetting.ListMails)
            {
                cbMailFilter.Items.Add(m.ExchangeMailBox);
            }
            cbThreadFilter.Items.Add("");
            cbThreadFilter.Items.Add("Поток 0");
            cbThreadFilter.Items.Add("Поток 1");
            cbThreadFilter.Items.Add("Поток 2");
            cbThreadFilter.Items.Add("Поток 3");
            cbThreadFilter.Items.Add("Поток 4");
            cbThreadFilter.Items.Add("Поток 5");
            chbSync.Checked = appSetting.AutoSynch;
            txtLogFileName.Text = appSetting.LogFileName;
            updLogLevel.Text = appSetting.DefLogLevel.ToString();
            chkLogClearOnStartup.Checked = appSetting.DefLogClearOnStartup;

            chkAutoRun.Checked = exchposer.AutoRun;

            UpdateControlsEnabling();

            this.cbThread = cbThreadFilter;
            this.cbMail = cbMailFilter;
            this.ListBox = listBox1;

            this.Controls.AddRange(new Control[] { this.cbThread,this.cbMail, this.ListBox });

            //string[] names = new string[] { "Миша", "Коля", "Вася", "Алексей", "Женя",
            //"Алена", "Наташа", "Марина", "Глаша", "Люба" };
          
            this.BindingSource = new BindingSource();
            this.BindingSource.DataSource = IMAP2ExchSyncApplicationContext.LogTableForm;
            //Array.ForEach(names, x => LogTableForm.Rows.Add(x));
            this.ListBox.DataSource = this.BindingSource;
            this.ListBox.DisplayMember = "LOG";
            this.cbMail.SelectedIndexChanged += delegate
            {
                try
                {
                    if (this.cbMail.Text == string.Empty)
                        this.BindingSource.Filter = string.Empty;
                    else
                        this.BindingSource.Filter = string.Format("(LOG LIKE '*{0}*') AND (LOG LIKE '*{1}*')",  this.cbThread.Text, this.cbMail.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(String.Format("Ошибка фильтра логов:" + " {0}", ex.Message));
                }
            };
            this.cbThread.SelectedIndexChanged += delegate
            {
                try
                {
                    if (this.cbThread.Text == string.Empty)
                        this.BindingSource.Filter = string.Empty;
                    else
                        this.BindingSource.Filter = string.Format("(LOG LIKE '*{0}*') AND (LOG LIKE '*{1}*')", this.cbThread.Text, this.cbMail.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(String.Format("Ошибка фильтра логов:" + " {0}", ex.Message));
                }
            };
            IMAP2ExchSyncApplicationContext.LogTableForm.RowChanged += delegate
            {

                for (int i = 0; (i < (IMAP2ExchSyncApplicationContext.LogTableForm.Rows.Count - 10000)); i++)
                {
                    IMAP2ExchSyncApplicationContext.LogTableForm.Rows.RemoveAt(i);
                }

                ListBox.SelectedIndex = ListBox.Items.Count - 1;

            };
        }
        public ListBox ListBox
        {
            get;
            set;
        }
        public ComboBox cbThread
        {
            get;
            set;
        }
        public ComboBox cbMail
        {
            get;
            set;
        }
        public BindingSource BindingSource
        {
            get;
            set;
        }

        public AppSettings GetSettings()
        {
            return appSetting;
        }

        delegate void SetTextCallback(string text);

        public void AppendLog(string msg)
        {
            //txtLogView.Text = System.IO.File.ReadAllText(logFileName);
            //txtLogView.Text = logReader.ReadToEnd();
            if (txtLogView.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(AppendLog);
                this.Invoke(d, new object[] { msg });
            }
            else
            {
                //txtLogView.AppendText(msg);
                //IMAP2ExchSyncApplicationContext.LogTableForm.Rows.Add(msg);


            }
        }
        public void UpdateSettings(int index = -1)
        {
            lock (appSetting)
            {
                if (index >= 0)
                {
                    if (index > appSetting.ListMails.Count - 1)
                    {
                        appSetting.ListMails.Add(new Mails());
                    }
                    appSetting.ListMails[index].ExchangeUrl = txtExchangeUrl.Text;
                    appSetting.ListMails[index].ExchangeDomain = txtExchangeDomain.Text;
                    appSetting.ListMails[index].ExchangeUserName = txtExchangeUserName.Text;
                    try
                    {
                        appSetting.ListMails[index].ExchangePassword = txtExchangePassword.Text;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    appSetting.ListMails[index].ExchangeSubscriptionLifetime = (int)updExchangeSubscriptionLifetime.Value;
                    appSetting.ListMails[index].ExchangeAcceptInvalidCertificate = chkExchangeAcceptInvalidCertificate.Checked;

                    appSetting.ListMails[index].MailServerName = txtMailServerName.Text;
                    appSetting.ListMails[index].MailServerPort = Convert.ToInt32(txtMailServerPort.Text);
                    appSetting.ListMails[index].MailServerType = (MailServerTypes)(cboMailServerType.SelectedIndex + 1);
                    appSetting.ListMails[index].MailUserName = txtMailUserName.Text;
                    appSetting.ListMails[index].MailPassword = txtMailPassword.Text;
                    appSetting.ListMails[index].MailToAddress = txtMailToAddress.Text;
                    appSetting.ListMails[index].MailFolderName = txtMailFolderName.Text;
                    //appSetting.ListMails[index].LastStatus = "LAST";
                    if (txtMailboxList.SelectedIndex >= 0)
                    {
                        appSetting.ListMails[index].ExchangeMailBox = txtMailboxList.Items[txtMailboxList.SelectedIndex].ToString();
                    }
                    else
                    {
                        appSetting.ListMails[index].ExchangeMailBox = "";
                    }

                    appSetting.ListMails[index].SyncEnabled = chkSyncEnabled.Checked;
                }
                appSetting.LogFileName = txtLogFileName.Text;
                appSetting.DefLogLevel = Convert.ToInt32(updLogLevel.Text);
                appSetting.DefLogClearOnStartup = chkLogClearOnStartup.Checked;
                appSetting.AutoSynch = chbSync.Checked;
                
                exchposer.AutoRun = chkAutoRun.Checked;
                //dataGridView1.DataSource = null;
                //mailsBindingSource.DataSource = appSetting.ListMails;
            }
        }


        private void UpdateControlsEnabling()
        {
            MailServerTypes mailServerType = (MailServerTypes)(cboMailServerType.SelectedIndex + 1);

            txtMailUserName.Enabled = ((mailServerType == MailServerTypes.IMAP));
            txtMailPassword.Enabled = ((mailServerType == MailServerTypes.IMAP));
            txtMailToAddress.Enabled = false;
            txtMailFolderName.Enabled = (mailServerType == MailServerTypes.IMAP);
            updMaxDaysToSync.Enabled = chkSyncEnabled.Checked;
        }

        private void cboMailServerType_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateControlsEnabling();
        }

        private void chkSyncEnabled_CheckedChanged(object sender, EventArgs e)
        {
            UpdateControlsEnabling();
        }

        private void btnLogFileName_Click(object sender, EventArgs e)
        {
            dlgFileOpen.InitialDirectory = Path.GetDirectoryName(txtLogFileName.Text);
            dlgFileOpen.FileName = Path.GetFileName(txtLogFileName.Text);
            if (dlgFileOpen.ShowDialog() == DialogResult.OK)
                txtLogFileName.Text = dlgFileOpen.FileName;
        }

        private void btnOfflineSyncNow_Click(object sender, EventArgs e)
        {
            DateTime syncFromTime = dtpOffilneSyncFrom.Value.Date;
            DateTime syncToTime = dtpOffilneSyncTo.Value.Date.AddDays(1).AddSeconds(-1);

            if (exchposer == null)
                return;

            //if (MessageBox.Show(String.Format("Do you want to synchronize messages from time {0} to time {1}?",
            //    syncFromTime, syncToTime), "Offline synchronization", MessageBoxButtons.YesNo) == DialogResult.Yes)
            //{
                exchposer.OfflineSync(syncFromTime, syncToTime,dataGridView1.SelectedRows[0].Index);
            //}
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateSettings();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in values "+ex.Message);
                DialogResult = DialogResult.None;
            }
        }

        private void txtMailToAddress_TextChanged(object sender, EventArgs e)
        {

        }

        private void chkExchangeAcceptInvalidCertificate_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void txtMailboxList_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private string GetSMTPAddressFromString(string str)
        {
            string smtp = "";
            string rege = "[\\w\\-\\._]+\\@[\\w\\-\\._]+\\.\\w+";
            MatchCollection matches = System.Text.RegularExpressions.Regex.Matches(str, rege);
            foreach (Match match in matches)
            {
                smtp = match.ToString();
            }
            
            return smtp;
        }

        private void txtMailboxList_DropDown(object sender, EventArgs e)
        {
            ExchangeServer Ex = new ExchangeServer(txtExchangeUserName.Text, txtExchangePassword.Text, 
                txtExchangeDomain.Text, txtExchangeUrl.Text, exchposer.ExchangeReconnectTimeout,exchposer, logger, "" );
            Ex.Open();
            txtMailboxList.Items.Clear();
            SearchableMailbox[] mailboxes = Ex.GetMailboxes();
            List<string> names = new List<string> { };
            foreach (SearchableMailbox mailbox in mailboxes)
            {
                if (mailbox.SmtpAddress != null)
                {
                    //names.Add(mailbox.SmtpAddress + " (" + mailbox.DisplayName + ")");
                    names.Add(mailbox.DisplayName + " (" + mailbox.SmtpAddress + ")");
                }                
            }
            names.Sort();
            txtMailboxList.Items.AddRange(names.ToArray());
            Ex.Close();
            Ex = null;
        }

        private void comboBox1_Click(object sender, EventArgs e)
        {
            
        }

        private void txtMailboxList_DropDownClosed(object sender, EventArgs e)
        {
            if (txtMailboxList.SelectedIndex >= 0)
            {
                string selectedMailbox = GetSMTPAddressFromString(txtMailboxList.Items[txtMailboxList.SelectedIndex].ToString());
                txtMailboxList.Items[txtMailboxList.SelectedIndex] = selectedMailbox;
            }
        }

        private void dataGridView1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                btnChange.Enabled = true;
                txtExchangeUrl.Text = appSetting.ListMails[dataGridView1.SelectedRows[0].Index].ExchangeUrl;
                txtExchangeDomain.Text = appSetting.ListMails[dataGridView1.SelectedRows[0].Index].ExchangeDomain;
                txtExchangeUserName.Text = appSetting.ListMails[dataGridView1.SelectedRows[0].Index].ExchangeUserName;
                txtExchangePassword.Text = appSetting.ListMails[dataGridView1.SelectedRows[0].Index].ExchangePassword;
                updExchangeSubscriptionLifetime.Value = appSetting.ListMails[dataGridView1.SelectedRows[0].Index].ExchangeSubscriptionLifetime;
                chkExchangeAcceptInvalidCertificate.Checked = appSetting.ListMails[dataGridView1.SelectedRows[0].Index].ExchangeAcceptInvalidCertificate;

                txtMailServerName.Text = appSetting.ListMails[dataGridView1.SelectedRows[0].Index].MailServerName;
                txtMailServerPort.Text = appSetting.ListMails[dataGridView1.SelectedRows[0].Index].MailServerPort.ToString();
                cboMailServerType.SelectedIndex = (int)appSetting.ListMails[dataGridView1.SelectedRows[0].Index].MailServerType - 1;
                txtMailUserName.Text = appSetting.ListMails[dataGridView1.SelectedRows[0].Index].MailUserName;
                txtMailPassword.Text = appSetting.ListMails[dataGridView1.SelectedRows[0].Index].MailPassword;
                txtMailToAddress.Text = appSetting.ListMails[dataGridView1.SelectedRows[0].Index].MailToAddress;
                txtMailFolderName.Text = appSetting.ListMails[dataGridView1.SelectedRows[0].Index].MailFolderName;
                txtMailboxList.Items.Clear();
                if (appSetting.ListMails[0].ExchangeMailBox != "")
                {
                    txtMailboxList.Items.Add(appSetting.ListMails[dataGridView1.SelectedRows[0].Index].ExchangeMailBox);
                    txtMailboxList.SelectedIndex = 0;
                }

                chkSyncEnabled.Checked = appSetting.ListMails[dataGridView1.SelectedRows[0].Index].SyncEnabled;
            }
            else
            {
                btnChange.Enabled = false;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            UpdateSettings(dataGridView1.RowCount-1);
        }

        private void btnChange_Click(object sender, EventArgs e)
        {
            UpdateSettings(dataGridView1.SelectedRows[0].Index);
        }

        private void chbSync_CheckedChanged(object sender, EventArgs e)
        {
            if (chbSync.Checked == true)
            {

            }
        }

        private void updExchangeSubscriptionLifetime_ValueChanged(object sender, EventArgs e)
        {

        }

        private void listBox1_ValueMemberChanged(object sender, EventArgs e)
        {

        }
    }
}
