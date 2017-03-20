using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace IMAP2ExchSync
{
    public partial class LogViewForm : Form
    {
        delegate void SetTextCallback(string text);

        public LogViewForm()
        {
            InitializeComponent();
        }

        private void LogViewForm_Shown(object sender, EventArgs e)
        {
            txtLogView.SelectionStart = txtLogView.Text.Length;
            txtLogView.ScrollToCaret();
            //txtLogView.Refresh();
        }

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
                txtLogView.AppendText(msg);
            }
        }
    }
}
