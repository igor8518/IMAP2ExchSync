using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IMAP2ExchSync
{
    public partial class MailsForm : Form
    {
        public MailsForm(DataView dt)
        {
            InitializeComponent();
            MonitoringGrid.DataSource = dt;
            //MonitoringGrid.DataSource = MonitoringBindingSource;
            //MonitoringBindingSource.DataSource = dt;
            
        }
        public void Refresh()
        {
            MonitoringGrid.Refresh();
        }
    }
}
