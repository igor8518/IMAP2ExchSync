using System;
using System.Windows.Forms;

namespace IMAP2ExchSync
{
    public interface IIMAP2ExchSync
    {
        void OfflineSync(DateTime syncFromTime, DateTime syncToTime, int indexMails, int thread=-1);
        void Log(int level, object message, Mails m = null, string origMessage = "");
        bool AutoRun { get; set; }
        BindingSource mailBindingSource { get; set; }
        int ExchangeReconnectTimeout { get; }
    }
}
