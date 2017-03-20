using System.ComponentModel;
using System.Windows.Forms;

namespace IMAP2ExchSync
{
    partial class OptionsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OptionsForm));
            this.lblBevel = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.grpExchange = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtMailboxList = new System.Windows.Forms.ComboBox();
            this.chkExchangeAcceptInvalidCertificate = new System.Windows.Forms.CheckBox();
            this.lblExchangeSubscriptionLifetime = new System.Windows.Forms.Label();
            this.updExchangeSubscriptionLifetime = new System.Windows.Forms.NumericUpDown();
            this.txtExchangeDomain = new System.Windows.Forms.TextBox();
            this.lblExchangeDomain = new System.Windows.Forms.Label();
            this.txtExchangePassword = new System.Windows.Forms.TextBox();
            this.lblExchangePassword = new System.Windows.Forms.Label();
            this.txtExchangeUserName = new System.Windows.Forms.TextBox();
            this.lblExchangeUserName = new System.Windows.Forms.Label();
            this.lblExchangeUrl = new System.Windows.Forms.Label();
            this.txtExchangeUrl = new System.Windows.Forms.TextBox();
            this.grpMail = new System.Windows.Forms.GroupBox();
            this.lblMailServerType = new System.Windows.Forms.Label();
            this.cboMailServerType = new System.Windows.Forms.ComboBox();
            this.txtMailServerPort = new System.Windows.Forms.TextBox();
            this.lblMailServerPort = new System.Windows.Forms.Label();
            this.txtMailFolderName = new System.Windows.Forms.TextBox();
            this.lblMailFolderName = new System.Windows.Forms.Label();
            this.txtMailToAddress = new System.Windows.Forms.TextBox();
            this.lblMailToAddress = new System.Windows.Forms.Label();
            this.txtMailPassword = new System.Windows.Forms.TextBox();
            this.lblMailPassword = new System.Windows.Forms.Label();
            this.txtMailUserName = new System.Windows.Forms.TextBox();
            this.lblMailUserName = new System.Windows.Forms.Label();
            this.txtMailServerName = new System.Windows.Forms.TextBox();
            this.lblMailServerName = new System.Windows.Forms.Label();
            this.grpLog = new System.Windows.Forms.GroupBox();
            this.chkLogClearOnStartup = new System.Windows.Forms.CheckBox();
            this.updLogLevel = new System.Windows.Forms.NumericUpDown();
            this.lblLogLevel = new System.Windows.Forms.Label();
            this.btnLogFileName = new System.Windows.Forms.Button();
            this.txtLogFileName = new System.Windows.Forms.TextBox();
            this.lblLogFileName = new System.Windows.Forms.Label();
            this.dlgFileOpen = new System.Windows.Forms.OpenFileDialog();
            this.grpOnlineSync = new System.Windows.Forms.GroupBox();
            this.chkSyncEnabled = new System.Windows.Forms.CheckBox();
            this.lblMaxDaysToSync = new System.Windows.Forms.Label();
            this.updMaxDaysToSync = new System.Windows.Forms.NumericUpDown();
            this.grpOfflineSync = new System.Windows.Forms.GroupBox();
            this.lblOfflineSyncFromTo = new System.Windows.Forms.Label();
            this.btnOfflineSyncNow = new System.Windows.Forms.Button();
            this.dtpOffilneSyncTo = new System.Windows.Forms.DateTimePicker();
            this.dtpOffilneSyncFrom = new System.Windows.Forms.DateTimePicker();
            this.chkAutoRun = new System.Windows.Forms.CheckBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.txtLogView = new System.Windows.Forms.TextBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnChange = new System.Windows.Forms.Button();
            this.chbSync = new System.Windows.Forms.CheckBox();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.cbThreadFilter = new System.Windows.Forms.ComboBox();
            this.cbMailFilter = new System.Windows.Forms.ComboBox();
            this.mailsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.No = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.exchangeMailBoxDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.mailUserNameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.syncEnabledDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.lastStatusDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.grpExchange.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.updExchangeSubscriptionLifetime)).BeginInit();
            this.grpMail.SuspendLayout();
            this.grpLog.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.updLogLevel)).BeginInit();
            this.grpOnlineSync.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.updMaxDaysToSync)).BeginInit();
            this.grpOfflineSync.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mailsBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // lblBevel
            // 
            this.lblBevel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblBevel.Location = new System.Drawing.Point(653, 482);
            this.lblBevel.Margin = new System.Windows.Forms.Padding(0);
            this.lblBevel.Name = "lblBevel";
            this.lblBevel.Size = new System.Drawing.Size(511, 2);
            this.lblBevel.TabIndex = 6;
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Font = new System.Drawing.Font("Tahoma", 7.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnOk.Location = new System.Drawing.Point(1009, 497);
            this.btnOk.Margin = new System.Windows.Forms.Padding(2);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 7;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Font = new System.Drawing.Font("Tahoma", 7.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnCancel.Location = new System.Drawing.Point(1089, 497);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(2);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // grpExchange
            // 
            this.grpExchange.Controls.Add(this.label1);
            this.grpExchange.Controls.Add(this.txtMailboxList);
            this.grpExchange.Controls.Add(this.chkExchangeAcceptInvalidCertificate);
            this.grpExchange.Controls.Add(this.lblExchangeSubscriptionLifetime);
            this.grpExchange.Controls.Add(this.updExchangeSubscriptionLifetime);
            this.grpExchange.Controls.Add(this.txtExchangeDomain);
            this.grpExchange.Controls.Add(this.lblExchangeDomain);
            this.grpExchange.Controls.Add(this.txtExchangePassword);
            this.grpExchange.Controls.Add(this.lblExchangePassword);
            this.grpExchange.Controls.Add(this.txtExchangeUserName);
            this.grpExchange.Controls.Add(this.lblExchangeUserName);
            this.grpExchange.Controls.Add(this.lblExchangeUrl);
            this.grpExchange.Controls.Add(this.txtExchangeUrl);
            this.grpExchange.Location = new System.Drawing.Point(653, 8);
            this.grpExchange.Margin = new System.Windows.Forms.Padding(2);
            this.grpExchange.Name = "grpExchange";
            this.grpExchange.Padding = new System.Windows.Forms.Padding(2);
            this.grpExchange.Size = new System.Drawing.Size(265, 182);
            this.grpExchange.TabIndex = 0;
            this.grpExchange.TabStop = false;
            this.grpExchange.Text = "Настройки сервера Exchange";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 99);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "Почтовый ящик:";
            // 
            // txtMailboxList
            // 
            this.txtMailboxList.FormattingEnabled = true;
            this.txtMailboxList.Location = new System.Drawing.Point(8, 114);
            this.txtMailboxList.Name = "txtMailboxList";
            this.txtMailboxList.Size = new System.Drawing.Size(246, 21);
            this.txtMailboxList.TabIndex = 11;
            this.txtMailboxList.DropDown += new System.EventHandler(this.txtMailboxList_DropDown);
            this.txtMailboxList.SelectedIndexChanged += new System.EventHandler(this.txtMailboxList_SelectedIndexChanged);
            this.txtMailboxList.DropDownClosed += new System.EventHandler(this.txtMailboxList_DropDownClosed);
            this.txtMailboxList.Click += new System.EventHandler(this.comboBox1_Click);
            // 
            // chkExchangeAcceptInvalidCertificate
            // 
            this.chkExchangeAcceptInvalidCertificate.AutoSize = true;
            this.chkExchangeAcceptInvalidCertificate.Location = new System.Drawing.Point(8, 161);
            this.chkExchangeAcceptInvalidCertificate.Margin = new System.Windows.Forms.Padding(2);
            this.chkExchangeAcceptInvalidCertificate.Name = "chkExchangeAcceptInvalidCertificate";
            this.chkExchangeAcceptInvalidCertificate.Size = new System.Drawing.Size(254, 17);
            this.chkExchangeAcceptInvalidCertificate.TabIndex = 10;
            this.chkExchangeAcceptInvalidCertificate.Text = "Принять недействительный SSL сертификат";
            this.chkExchangeAcceptInvalidCertificate.UseVisualStyleBackColor = true;
            this.chkExchangeAcceptInvalidCertificate.CheckedChanged += new System.EventHandler(this.chkExchangeAcceptInvalidCertificate_CheckedChanged);
            // 
            // lblExchangeSubscriptionLifetime
            // 
            this.lblExchangeSubscriptionLifetime.AutoSize = true;
            this.lblExchangeSubscriptionLifetime.Location = new System.Drawing.Point(84, 140);
            this.lblExchangeSubscriptionLifetime.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblExchangeSubscriptionLifetime.Name = "lblExchangeSubscriptionLifetime";
            this.lblExchangeSubscriptionLifetime.Size = new System.Drawing.Size(126, 13);
            this.lblExchangeSubscriptionLifetime.TabIndex = 9;
            this.lblExchangeSubscriptionLifetime.Text = "Время жизни подписки";
            // 
            // updExchangeSubscriptionLifetime
            // 
            this.updExchangeSubscriptionLifetime.Location = new System.Drawing.Point(8, 140);
            this.updExchangeSubscriptionLifetime.Margin = new System.Windows.Forms.Padding(2);
            this.updExchangeSubscriptionLifetime.Name = "updExchangeSubscriptionLifetime";
            this.updExchangeSubscriptionLifetime.Size = new System.Drawing.Size(75, 20);
            this.updExchangeSubscriptionLifetime.TabIndex = 8;
            this.updExchangeSubscriptionLifetime.ValueChanged += new System.EventHandler(this.updExchangeSubscriptionLifetime_ValueChanged);
            // 
            // txtExchangeDomain
            // 
            this.txtExchangeDomain.Location = new System.Drawing.Point(8, 75);
            this.txtExchangeDomain.Margin = new System.Windows.Forms.Padding(2);
            this.txtExchangeDomain.Name = "txtExchangeDomain";
            this.txtExchangeDomain.Size = new System.Drawing.Size(76, 20);
            this.txtExchangeDomain.TabIndex = 3;
            this.txtExchangeDomain.WordWrap = false;
            // 
            // lblExchangeDomain
            // 
            this.lblExchangeDomain.AutoSize = true;
            this.lblExchangeDomain.Location = new System.Drawing.Point(5, 58);
            this.lblExchangeDomain.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblExchangeDomain.Name = "lblExchangeDomain";
            this.lblExchangeDomain.Size = new System.Drawing.Size(42, 13);
            this.lblExchangeDomain.TabIndex = 2;
            this.lblExchangeDomain.Text = "Домен";
            // 
            // txtExchangePassword
            // 
            this.txtExchangePassword.Location = new System.Drawing.Point(166, 75);
            this.txtExchangePassword.Margin = new System.Windows.Forms.Padding(2);
            this.txtExchangePassword.Name = "txtExchangePassword";
            this.txtExchangePassword.PasswordChar = '*';
            this.txtExchangePassword.Size = new System.Drawing.Size(88, 20);
            this.txtExchangePassword.TabIndex = 7;
            this.txtExchangePassword.WordWrap = false;
            // 
            // lblExchangePassword
            // 
            this.lblExchangePassword.AutoSize = true;
            this.lblExchangePassword.Location = new System.Drawing.Point(164, 58);
            this.lblExchangePassword.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblExchangePassword.Name = "lblExchangePassword";
            this.lblExchangePassword.Size = new System.Drawing.Size(45, 13);
            this.lblExchangePassword.TabIndex = 6;
            this.lblExchangePassword.Text = "Пароль";
            // 
            // txtExchangeUserName
            // 
            this.txtExchangeUserName.Location = new System.Drawing.Point(87, 75);
            this.txtExchangeUserName.Margin = new System.Windows.Forms.Padding(2);
            this.txtExchangeUserName.Name = "txtExchangeUserName";
            this.txtExchangeUserName.Size = new System.Drawing.Size(76, 20);
            this.txtExchangeUserName.TabIndex = 5;
            this.txtExchangeUserName.WordWrap = false;
            // 
            // lblExchangeUserName
            // 
            this.lblExchangeUserName.AutoSize = true;
            this.lblExchangeUserName.Location = new System.Drawing.Point(85, 58);
            this.lblExchangeUserName.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblExchangeUserName.Name = "lblExchangeUserName";
            this.lblExchangeUserName.Size = new System.Drawing.Size(80, 13);
            this.lblExchangeUserName.TabIndex = 4;
            this.lblExchangeUserName.Text = "Пользователь";
            // 
            // lblExchangeUrl
            // 
            this.lblExchangeUrl.AutoSize = true;
            this.lblExchangeUrl.Location = new System.Drawing.Point(5, 18);
            this.lblExchangeUrl.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblExchangeUrl.Name = "lblExchangeUrl";
            this.lblExchangeUrl.Size = new System.Drawing.Size(240, 13);
            this.lblExchangeUrl.TabIndex = 0;
            this.lblExchangeUrl.Text = "Адрес подключения Exchange сервера (.asmx)";
            // 
            // txtExchangeUrl
            // 
            this.txtExchangeUrl.Location = new System.Drawing.Point(8, 34);
            this.txtExchangeUrl.Margin = new System.Windows.Forms.Padding(2);
            this.txtExchangeUrl.Name = "txtExchangeUrl";
            this.txtExchangeUrl.Size = new System.Drawing.Size(246, 20);
            this.txtExchangeUrl.TabIndex = 1;
            this.txtExchangeUrl.WordWrap = false;
            // 
            // grpMail
            // 
            this.grpMail.Controls.Add(this.lblMailServerType);
            this.grpMail.Controls.Add(this.cboMailServerType);
            this.grpMail.Controls.Add(this.txtMailServerPort);
            this.grpMail.Controls.Add(this.lblMailServerPort);
            this.grpMail.Controls.Add(this.txtMailFolderName);
            this.grpMail.Controls.Add(this.lblMailFolderName);
            this.grpMail.Controls.Add(this.txtMailToAddress);
            this.grpMail.Controls.Add(this.lblMailToAddress);
            this.grpMail.Controls.Add(this.txtMailPassword);
            this.grpMail.Controls.Add(this.lblMailPassword);
            this.grpMail.Controls.Add(this.txtMailUserName);
            this.grpMail.Controls.Add(this.lblMailUserName);
            this.grpMail.Controls.Add(this.txtMailServerName);
            this.grpMail.Controls.Add(this.lblMailServerName);
            this.grpMail.Location = new System.Drawing.Point(923, 8);
            this.grpMail.Margin = new System.Windows.Forms.Padding(2);
            this.grpMail.Name = "grpMail";
            this.grpMail.Padding = new System.Windows.Forms.Padding(2);
            this.grpMail.Size = new System.Drawing.Size(254, 182);
            this.grpMail.TabIndex = 1;
            this.grpMail.TabStop = false;
            this.grpMail.Text = "Mail server";
            // 
            // lblMailServerType
            // 
            this.lblMailServerType.AutoSize = true;
            this.lblMailServerType.Location = new System.Drawing.Point(122, 58);
            this.lblMailServerType.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblMailServerType.Name = "lblMailServerType";
            this.lblMailServerType.Size = new System.Drawing.Size(61, 13);
            this.lblMailServerType.TabIndex = 4;
            this.lblMailServerType.Text = "Server type";
            // 
            // cboMailServerType
            // 
            this.cboMailServerType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMailServerType.FormattingEnabled = true;
            this.cboMailServerType.ItemHeight = 13;
            this.cboMailServerType.Items.AddRange(new object[] {
            "IMAP",
            "SMTP",
            "SMTPMX"});
            this.cboMailServerType.Location = new System.Drawing.Point(127, 75);
            this.cboMailServerType.Margin = new System.Windows.Forms.Padding(2);
            this.cboMailServerType.Name = "cboMailServerType";
            this.cboMailServerType.Size = new System.Drawing.Size(116, 21);
            this.cboMailServerType.TabIndex = 5;
            this.cboMailServerType.SelectedIndexChanged += new System.EventHandler(this.cboMailServerType_SelectedIndexChanged);
            // 
            // txtMailServerPort
            // 
            this.txtMailServerPort.Location = new System.Drawing.Point(8, 75);
            this.txtMailServerPort.Margin = new System.Windows.Forms.Padding(2);
            this.txtMailServerPort.Name = "txtMailServerPort";
            this.txtMailServerPort.Size = new System.Drawing.Size(116, 20);
            this.txtMailServerPort.TabIndex = 3;
            this.txtMailServerPort.WordWrap = false;
            // 
            // lblMailServerPort
            // 
            this.lblMailServerPort.AutoSize = true;
            this.lblMailServerPort.Location = new System.Drawing.Point(5, 58);
            this.lblMailServerPort.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblMailServerPort.Name = "lblMailServerPort";
            this.lblMailServerPort.Size = new System.Drawing.Size(26, 13);
            this.lblMailServerPort.TabIndex = 2;
            this.lblMailServerPort.Text = "Port";
            // 
            // txtMailFolderName
            // 
            this.txtMailFolderName.Location = new System.Drawing.Point(127, 156);
            this.txtMailFolderName.Margin = new System.Windows.Forms.Padding(2);
            this.txtMailFolderName.Name = "txtMailFolderName";
            this.txtMailFolderName.Size = new System.Drawing.Size(116, 20);
            this.txtMailFolderName.TabIndex = 13;
            this.txtMailFolderName.WordWrap = false;
            // 
            // lblMailFolderName
            // 
            this.lblMailFolderName.AutoSize = true;
            this.lblMailFolderName.Location = new System.Drawing.Point(124, 140);
            this.lblMailFolderName.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblMailFolderName.Name = "lblMailFolderName";
            this.lblMailFolderName.Size = new System.Drawing.Size(84, 13);
            this.lblMailFolderName.TabIndex = 12;
            this.lblMailFolderName.Text = "Mail folder name";
            // 
            // txtMailToAddress
            // 
            this.txtMailToAddress.Location = new System.Drawing.Point(8, 156);
            this.txtMailToAddress.Margin = new System.Windows.Forms.Padding(2);
            this.txtMailToAddress.Name = "txtMailToAddress";
            this.txtMailToAddress.Size = new System.Drawing.Size(116, 20);
            this.txtMailToAddress.TabIndex = 11;
            this.txtMailToAddress.WordWrap = false;
            this.txtMailToAddress.TextChanged += new System.EventHandler(this.txtMailToAddress_TextChanged);
            // 
            // lblMailToAddress
            // 
            this.lblMailToAddress.AutoSize = true;
            this.lblMailToAddress.Location = new System.Drawing.Point(5, 140);
            this.lblMailToAddress.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblMailToAddress.Name = "lblMailToAddress";
            this.lblMailToAddress.Size = new System.Drawing.Size(78, 13);
            this.lblMailToAddress.TabIndex = 10;
            this.lblMailToAddress.Text = "Mail to address";
            // 
            // txtMailPassword
            // 
            this.txtMailPassword.Location = new System.Drawing.Point(127, 115);
            this.txtMailPassword.Margin = new System.Windows.Forms.Padding(2);
            this.txtMailPassword.Name = "txtMailPassword";
            this.txtMailPassword.PasswordChar = '*';
            this.txtMailPassword.Size = new System.Drawing.Size(116, 20);
            this.txtMailPassword.TabIndex = 9;
            this.txtMailPassword.WordWrap = false;
            // 
            // lblMailPassword
            // 
            this.lblMailPassword.AutoSize = true;
            this.lblMailPassword.Location = new System.Drawing.Point(124, 99);
            this.lblMailPassword.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblMailPassword.Name = "lblMailPassword";
            this.lblMailPassword.Size = new System.Drawing.Size(53, 13);
            this.lblMailPassword.TabIndex = 8;
            this.lblMailPassword.Text = "Password";
            // 
            // txtMailUserName
            // 
            this.txtMailUserName.Location = new System.Drawing.Point(8, 115);
            this.txtMailUserName.Margin = new System.Windows.Forms.Padding(2);
            this.txtMailUserName.Name = "txtMailUserName";
            this.txtMailUserName.Size = new System.Drawing.Size(116, 20);
            this.txtMailUserName.TabIndex = 7;
            this.txtMailUserName.WordWrap = false;
            // 
            // lblMailUserName
            // 
            this.lblMailUserName.AutoSize = true;
            this.lblMailUserName.Location = new System.Drawing.Point(5, 99);
            this.lblMailUserName.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblMailUserName.Name = "lblMailUserName";
            this.lblMailUserName.Size = new System.Drawing.Size(58, 13);
            this.lblMailUserName.TabIndex = 6;
            this.lblMailUserName.Text = "User name";
            // 
            // txtMailServerName
            // 
            this.txtMailServerName.Location = new System.Drawing.Point(8, 34);
            this.txtMailServerName.Margin = new System.Windows.Forms.Padding(2);
            this.txtMailServerName.Name = "txtMailServerName";
            this.txtMailServerName.Size = new System.Drawing.Size(235, 20);
            this.txtMailServerName.TabIndex = 1;
            this.txtMailServerName.WordWrap = false;
            // 
            // lblMailServerName
            // 
            this.lblMailServerName.AutoSize = true;
            this.lblMailServerName.Location = new System.Drawing.Point(5, 18);
            this.lblMailServerName.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblMailServerName.Name = "lblMailServerName";
            this.lblMailServerName.Size = new System.Drawing.Size(67, 13);
            this.lblMailServerName.TabIndex = 0;
            this.lblMailServerName.Text = "Server name";
            // 
            // grpLog
            // 
            this.grpLog.Controls.Add(this.chkLogClearOnStartup);
            this.grpLog.Controls.Add(this.updLogLevel);
            this.grpLog.Controls.Add(this.lblLogLevel);
            this.grpLog.Controls.Add(this.btnLogFileName);
            this.grpLog.Controls.Add(this.txtLogFileName);
            this.grpLog.Controls.Add(this.lblLogFileName);
            this.grpLog.Location = new System.Drawing.Point(653, 404);
            this.grpLog.Margin = new System.Windows.Forms.Padding(2);
            this.grpLog.Name = "grpLog";
            this.grpLog.Padding = new System.Windows.Forms.Padding(2);
            this.grpLog.Size = new System.Drawing.Size(511, 68);
            this.grpLog.TabIndex = 5;
            this.grpLog.TabStop = false;
            this.grpLog.Text = "Log";
            // 
            // chkLogClearOnStartup
            // 
            this.chkLogClearOnStartup.AutoSize = true;
            this.chkLogClearOnStartup.Location = new System.Drawing.Point(382, 37);
            this.chkLogClearOnStartup.Margin = new System.Windows.Forms.Padding(2);
            this.chkLogClearOnStartup.Name = "chkLogClearOnStartup";
            this.chkLogClearOnStartup.Size = new System.Drawing.Size(116, 17);
            this.chkLogClearOnStartup.TabIndex = 5;
            this.chkLogClearOnStartup.Text = "log clear on startup";
            this.chkLogClearOnStartup.UseVisualStyleBackColor = true;
            // 
            // updLogLevel
            // 
            this.updLogLevel.Location = new System.Drawing.Point(297, 37);
            this.updLogLevel.Margin = new System.Windows.Forms.Padding(2);
            this.updLogLevel.Name = "updLogLevel";
            this.updLogLevel.Size = new System.Drawing.Size(75, 20);
            this.updLogLevel.TabIndex = 4;
            // 
            // lblLogLevel
            // 
            this.lblLogLevel.AutoSize = true;
            this.lblLogLevel.Location = new System.Drawing.Point(295, 20);
            this.lblLogLevel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblLogLevel.Name = "lblLogLevel";
            this.lblLogLevel.Size = new System.Drawing.Size(50, 13);
            this.lblLogLevel.TabIndex = 3;
            this.lblLogLevel.Text = "Log level";
            // 
            // btnLogFileName
            // 
            this.btnLogFileName.Font = new System.Drawing.Font("Tahoma", 7.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnLogFileName.Location = new System.Drawing.Point(266, 33);
            this.btnLogFileName.Margin = new System.Windows.Forms.Padding(0);
            this.btnLogFileName.Name = "btnLogFileName";
            this.btnLogFileName.Size = new System.Drawing.Size(22, 21);
            this.btnLogFileName.TabIndex = 2;
            this.btnLogFileName.Text = "...";
            this.btnLogFileName.UseVisualStyleBackColor = true;
            this.btnLogFileName.Click += new System.EventHandler(this.btnLogFileName_Click);
            // 
            // txtLogFileName
            // 
            this.txtLogFileName.Location = new System.Drawing.Point(8, 35);
            this.txtLogFileName.Margin = new System.Windows.Forms.Padding(2);
            this.txtLogFileName.Name = "txtLogFileName";
            this.txtLogFileName.Size = new System.Drawing.Size(257, 20);
            this.txtLogFileName.TabIndex = 1;
            // 
            // lblLogFileName
            // 
            this.lblLogFileName.AutoSize = true;
            this.lblLogFileName.Location = new System.Drawing.Point(5, 19);
            this.lblLogFileName.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblLogFileName.Name = "lblLogFileName";
            this.lblLogFileName.Size = new System.Drawing.Size(70, 13);
            this.lblLogFileName.TabIndex = 0;
            this.lblLogFileName.Text = "Log file name";
            // 
            // grpOnlineSync
            // 
            this.grpOnlineSync.Controls.Add(this.chkSyncEnabled);
            this.grpOnlineSync.Controls.Add(this.lblMaxDaysToSync);
            this.grpOnlineSync.Controls.Add(this.updMaxDaysToSync);
            this.grpOnlineSync.Location = new System.Drawing.Point(653, 195);
            this.grpOnlineSync.Margin = new System.Windows.Forms.Padding(2);
            this.grpOnlineSync.Name = "grpOnlineSync";
            this.grpOnlineSync.Padding = new System.Windows.Forms.Padding(2);
            this.grpOnlineSync.Size = new System.Drawing.Size(265, 63);
            this.grpOnlineSync.TabIndex = 2;
            this.grpOnlineSync.TabStop = false;
            this.grpOnlineSync.Text = "Автоматическая синхронизация";
            // 
            // chkSyncEnabled
            // 
            this.chkSyncEnabled.AutoSize = true;
            this.chkSyncEnabled.Location = new System.Drawing.Point(8, 18);
            this.chkSyncEnabled.Margin = new System.Windows.Forms.Padding(2);
            this.chkSyncEnabled.Name = "chkSyncEnabled";
            this.chkSyncEnabled.Size = new System.Drawing.Size(75, 17);
            this.chkSyncEnabled.TabIndex = 0;
            this.chkSyncEnabled.Text = "Включить";
            this.chkSyncEnabled.UseVisualStyleBackColor = true;
            this.chkSyncEnabled.CheckedChanged += new System.EventHandler(this.chkSyncEnabled_CheckedChanged);
            // 
            // lblMaxDaysToSync
            // 
            this.lblMaxDaysToSync.AutoSize = true;
            this.lblMaxDaysToSync.Location = new System.Drawing.Point(55, 41);
            this.lblMaxDaysToSync.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblMaxDaysToSync.Name = "lblMaxDaysToSync";
            this.lblMaxDaysToSync.Size = new System.Drawing.Size(173, 13);
            this.lblMaxDaysToSync.TabIndex = 2;
            this.lblMaxDaysToSync.Text = "Количество дней синхронизации";
            // 
            // updMaxDaysToSync
            // 
            this.updMaxDaysToSync.Location = new System.Drawing.Point(8, 39);
            this.updMaxDaysToSync.Margin = new System.Windows.Forms.Padding(2);
            this.updMaxDaysToSync.Name = "updMaxDaysToSync";
            this.updMaxDaysToSync.Size = new System.Drawing.Size(43, 20);
            this.updMaxDaysToSync.TabIndex = 1;
            // 
            // grpOfflineSync
            // 
            this.grpOfflineSync.Controls.Add(this.lblOfflineSyncFromTo);
            this.grpOfflineSync.Controls.Add(this.btnOfflineSyncNow);
            this.grpOfflineSync.Controls.Add(this.dtpOffilneSyncTo);
            this.grpOfflineSync.Controls.Add(this.dtpOffilneSyncFrom);
            this.grpOfflineSync.Location = new System.Drawing.Point(653, 262);
            this.grpOfflineSync.Margin = new System.Windows.Forms.Padding(2);
            this.grpOfflineSync.Name = "grpOfflineSync";
            this.grpOfflineSync.Padding = new System.Windows.Forms.Padding(2);
            this.grpOfflineSync.Size = new System.Drawing.Size(265, 49);
            this.grpOfflineSync.TabIndex = 3;
            this.grpOfflineSync.TabStop = false;
            this.grpOfflineSync.Text = "Offline synchronization";
            // 
            // lblOfflineSyncFromTo
            // 
            this.lblOfflineSyncFromTo.AutoSize = true;
            this.lblOfflineSyncFromTo.Font = new System.Drawing.Font("Courier New", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblOfflineSyncFromTo.Location = new System.Drawing.Point(86, 20);
            this.lblOfflineSyncFromTo.Margin = new System.Windows.Forms.Padding(0);
            this.lblOfflineSyncFromTo.Name = "lblOfflineSyncFromTo";
            this.lblOfflineSyncFromTo.Size = new System.Drawing.Size(16, 16);
            this.lblOfflineSyncFromTo.TabIndex = 1;
            this.lblOfflineSyncFromTo.Text = "-";
            this.lblOfflineSyncFromTo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnOfflineSyncNow
            // 
            this.btnOfflineSyncNow.Location = new System.Drawing.Point(182, 18);
            this.btnOfflineSyncNow.Margin = new System.Windows.Forms.Padding(2);
            this.btnOfflineSyncNow.Name = "btnOfflineSyncNow";
            this.btnOfflineSyncNow.Size = new System.Drawing.Size(62, 21);
            this.btnOfflineSyncNow.TabIndex = 3;
            this.btnOfflineSyncNow.Text = "Sync now";
            this.btnOfflineSyncNow.UseVisualStyleBackColor = true;
            this.btnOfflineSyncNow.Click += new System.EventHandler(this.btnOfflineSyncNow_Click);
            // 
            // dtpOffilneSyncTo
            // 
            this.dtpOffilneSyncTo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpOffilneSyncTo.Location = new System.Drawing.Point(103, 20);
            this.dtpOffilneSyncTo.Margin = new System.Windows.Forms.Padding(2);
            this.dtpOffilneSyncTo.Name = "dtpOffilneSyncTo";
            this.dtpOffilneSyncTo.Size = new System.Drawing.Size(76, 20);
            this.dtpOffilneSyncTo.TabIndex = 2;
            // 
            // dtpOffilneSyncFrom
            // 
            this.dtpOffilneSyncFrom.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpOffilneSyncFrom.Location = new System.Drawing.Point(10, 20);
            this.dtpOffilneSyncFrom.Margin = new System.Windows.Forms.Padding(2);
            this.dtpOffilneSyncFrom.Name = "dtpOffilneSyncFrom";
            this.dtpOffilneSyncFrom.Size = new System.Drawing.Size(76, 20);
            this.dtpOffilneSyncFrom.TabIndex = 0;
            this.dtpOffilneSyncFrom.Value = new System.DateTime(2017, 2, 16, 10, 59, 30, 105);
            // 
            // chkAutoRun
            // 
            this.chkAutoRun.AutoSize = true;
            this.chkAutoRun.Location = new System.Drawing.Point(653, 383);
            this.chkAutoRun.Margin = new System.Windows.Forms.Padding(2);
            this.chkAutoRun.Name = "chkAutoRun";
            this.chkAutoRun.Size = new System.Drawing.Size(140, 17);
            this.chkAutoRun.TabIndex = 4;
            this.chkAutoRun.Text = "Run at Windows startup";
            this.chkAutoRun.UseVisualStyleBackColor = true;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.AutoGenerateColumns = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.No,
            this.exchangeMailBoxDataGridViewTextBoxColumn,
            this.mailUserNameDataGridViewTextBoxColumn,
            this.syncEnabledDataGridViewCheckBoxColumn,
            this.lastStatusDataGridViewTextBoxColumn});
            this.dataGridView1.DataSource = this.mailsBindingSource;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridView1.ImeMode = System.Windows.Forms.ImeMode.Hiragana;
            this.dataGridView1.Location = new System.Drawing.Point(12, 12);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(627, 246);
            this.dataGridView1.StandardTab = true;
            this.dataGridView1.TabIndex = 9;
            this.dataGridView1.VirtualMode = true;
            this.dataGridView1.Click += new System.EventHandler(this.dataGridView1_Click);
            // 
            // txtLogView
            // 
            this.txtLogView.BackColor = System.Drawing.SystemColors.Window;
            this.txtLogView.Location = new System.Drawing.Point(12, 272);
            this.txtLogView.Margin = new System.Windows.Forms.Padding(2);
            this.txtLogView.Multiline = true;
            this.txtLogView.Name = "txtLogView";
            this.txtLogView.ReadOnly = true;
            this.txtLogView.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtLogView.Size = new System.Drawing.Size(10, 212);
            this.txtLogView.TabIndex = 10;
            this.txtLogView.TabStop = false;
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(923, 288);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(83, 23);
            this.btnAdd.TabIndex = 11;
            this.btnAdd.Text = "Добавить";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnChange
            // 
            this.btnChange.Enabled = false;
            this.btnChange.Location = new System.Drawing.Point(923, 259);
            this.btnChange.Name = "btnChange";
            this.btnChange.Size = new System.Drawing.Size(83, 23);
            this.btnChange.TabIndex = 12;
            this.btnChange.Text = "Изменить";
            this.btnChange.UseVisualStyleBackColor = true;
            this.btnChange.Click += new System.EventHandler(this.btnChange_Click);
            // 
            // chbSync
            // 
            this.chbSync.AutoSize = true;
            this.chbSync.Location = new System.Drawing.Point(653, 361);
            this.chbSync.Name = "chbSync";
            this.chbSync.Size = new System.Drawing.Size(127, 17);
            this.chbSync.TabIndex = 13;
            this.chbSync.Text = "Автосинхронизация";
            this.chbSync.UseVisualStyleBackColor = true;
            this.chbSync.CheckedChanged += new System.EventHandler(this.chbSync_CheckedChanged);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.HorizontalScrollbar = true;
            this.listBox1.Location = new System.Drawing.Point(27, 272);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(612, 212);
            this.listBox1.TabIndex = 14;
            this.listBox1.ValueMemberChanged += new System.EventHandler(this.listBox1_ValueMemberChanged);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(281, 491);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(358, 20);
            this.textBox1.TabIndex = 15;
            // 
            // cbThreadFilter
            // 
            this.cbThreadFilter.FormattingEnabled = true;
            this.cbThreadFilter.Location = new System.Drawing.Point(27, 489);
            this.cbThreadFilter.Name = "cbThreadFilter";
            this.cbThreadFilter.Size = new System.Drawing.Size(121, 21);
            this.cbThreadFilter.TabIndex = 16;
            // 
            // cbMailFilter
            // 
            this.cbMailFilter.FormattingEnabled = true;
            this.cbMailFilter.Location = new System.Drawing.Point(154, 490);
            this.cbMailFilter.Name = "cbMailFilter";
            this.cbMailFilter.Size = new System.Drawing.Size(121, 21);
            this.cbMailFilter.TabIndex = 17;
            // 
            // mailsBindingSource
            // 
            this.mailsBindingSource.DataSource = typeof(IMAP2ExchSync.Mails);
            // 
            // No
            // 
            this.No.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCellsExceptHeader;
            this.No.Frozen = true;
            this.No.HeaderText = "№";
            this.No.Name = "No";
            this.No.ReadOnly = true;
            this.No.Width = 5;
            // 
            // exchangeMailBoxDataGridViewTextBoxColumn
            // 
            this.exchangeMailBoxDataGridViewTextBoxColumn.DataPropertyName = "ExchangeMailBox";
            this.exchangeMailBoxDataGridViewTextBoxColumn.HeaderText = "ExchangeMailBox";
            this.exchangeMailBoxDataGridViewTextBoxColumn.Name = "exchangeMailBoxDataGridViewTextBoxColumn";
            this.exchangeMailBoxDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // mailUserNameDataGridViewTextBoxColumn
            // 
            this.mailUserNameDataGridViewTextBoxColumn.DataPropertyName = "MailUserName";
            this.mailUserNameDataGridViewTextBoxColumn.HeaderText = "MailUserName";
            this.mailUserNameDataGridViewTextBoxColumn.Name = "mailUserNameDataGridViewTextBoxColumn";
            this.mailUserNameDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // syncEnabledDataGridViewCheckBoxColumn
            // 
            this.syncEnabledDataGridViewCheckBoxColumn.DataPropertyName = "SyncEnabled";
            this.syncEnabledDataGridViewCheckBoxColumn.HeaderText = "SyncEnabled";
            this.syncEnabledDataGridViewCheckBoxColumn.Name = "syncEnabledDataGridViewCheckBoxColumn";
            this.syncEnabledDataGridViewCheckBoxColumn.ReadOnly = true;
            // 
            // lastStatusDataGridViewTextBoxColumn
            // 
            this.lastStatusDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.lastStatusDataGridViewTextBoxColumn.DataPropertyName = "LastStatus";
            this.lastStatusDataGridViewTextBoxColumn.HeaderText = "LastStatus";
            this.lastStatusDataGridViewTextBoxColumn.Name = "lastStatusDataGridViewTextBoxColumn";
            this.lastStatusDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // OptionsForm
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(1182, 532);
            this.Controls.Add(this.cbMailFilter);
            this.Controls.Add(this.cbThreadFilter);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.chbSync);
            this.Controls.Add(this.btnChange);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.txtLogView);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.chkAutoRun);
            this.Controls.Add(this.grpOfflineSync);
            this.Controls.Add(this.grpOnlineSync);
            this.Controls.Add(this.grpLog);
            this.Controls.Add(this.grpMail);
            this.Controls.Add(this.grpExchange);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.lblBevel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "OptionsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Options";
            this.grpExchange.ResumeLayout(false);
            this.grpExchange.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.updExchangeSubscriptionLifetime)).EndInit();
            this.grpMail.ResumeLayout(false);
            this.grpMail.PerformLayout();
            this.grpLog.ResumeLayout(false);
            this.grpLog.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.updLogLevel)).EndInit();
            this.grpOnlineSync.ResumeLayout(false);
            this.grpOnlineSync.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.updMaxDaysToSync)).EndInit();
            this.grpOfflineSync.ResumeLayout(false);
            this.grpOfflineSync.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mailsBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblBevel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox grpExchange;
        private System.Windows.Forms.Label lblExchangeSubscriptionLifetime;
        private System.Windows.Forms.NumericUpDown updExchangeSubscriptionLifetime;
        private System.Windows.Forms.TextBox txtExchangeDomain;
        private System.Windows.Forms.Label lblExchangeDomain;
        private System.Windows.Forms.TextBox txtExchangePassword;
        private System.Windows.Forms.Label lblExchangePassword;
        private System.Windows.Forms.TextBox txtExchangeUserName;
        private System.Windows.Forms.Label lblExchangeUserName;
        private System.Windows.Forms.Label lblExchangeUrl;
        private System.Windows.Forms.TextBox txtExchangeUrl;
        private System.Windows.Forms.GroupBox grpMail;
        private System.Windows.Forms.GroupBox grpLog;
        private System.Windows.Forms.NumericUpDown updLogLevel;
        private System.Windows.Forms.Label lblLogLevel;
        private System.Windows.Forms.Button btnLogFileName;
        private System.Windows.Forms.TextBox txtLogFileName;
        private System.Windows.Forms.Label lblLogFileName;
        private System.Windows.Forms.OpenFileDialog dlgFileOpen;
        private System.Windows.Forms.CheckBox chkLogClearOnStartup;
        private System.Windows.Forms.ComboBox cboMailServerType;
        private System.Windows.Forms.TextBox txtMailServerPort;
        private System.Windows.Forms.Label lblMailServerPort;
        private System.Windows.Forms.TextBox txtMailFolderName;
        private System.Windows.Forms.Label lblMailFolderName;
        private System.Windows.Forms.TextBox txtMailToAddress;
        private System.Windows.Forms.Label lblMailToAddress;
        private System.Windows.Forms.TextBox txtMailPassword;
        private System.Windows.Forms.Label lblMailPassword;
        private System.Windows.Forms.TextBox txtMailUserName;
        private System.Windows.Forms.Label lblMailUserName;
        private System.Windows.Forms.TextBox txtMailServerName;
        private System.Windows.Forms.Label lblMailServerName;
        private System.Windows.Forms.Label lblMailServerType;
        private System.Windows.Forms.GroupBox grpOnlineSync;
        private System.Windows.Forms.Label lblMaxDaysToSync;
        private System.Windows.Forms.NumericUpDown updMaxDaysToSync;
        private System.Windows.Forms.CheckBox chkSyncEnabled;
        private System.Windows.Forms.GroupBox grpOfflineSync;
        private System.Windows.Forms.Label lblOfflineSyncFromTo;
        private System.Windows.Forms.Button btnOfflineSyncNow;
        private System.Windows.Forms.DateTimePicker dtpOffilneSyncTo;
        private System.Windows.Forms.DateTimePicker dtpOffilneSyncFrom;
        private System.Windows.Forms.CheckBox chkAutoRun;
        private System.Windows.Forms.CheckBox chkExchangeAcceptInvalidCertificate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox txtMailboxList;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TextBox txtLogView;
        private Button btnAdd;
        private Button btnChange;
        private CheckBox chbSync;
        private ListBox listBox1;
        private TextBox textBox1;
        private ComboBox cbThreadFilter;
        private ComboBox cbMailFilter;
        private BindingSource mailsBindingSource;
        private DataGridViewTextBoxColumn No;
        private DataGridViewTextBoxColumn exchangeMailBoxDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn mailUserNameDataGridViewTextBoxColumn;
        private DataGridViewCheckBoxColumn syncEnabledDataGridViewCheckBoxColumn;
        private DataGridViewTextBoxColumn lastStatusDataGridViewTextBoxColumn;
    }
}