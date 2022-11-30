using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;


namespace IMAP2ExchSync
{
    class Program
    {
        string ProductVersion = "0.2.1808.10";
        private static Mutex mutex;

        [STAThread]
        static void Main(string[] args)
        {
            string appSettingsFileName = (args.Length > 1 ? args[0] : null);          

            bool running;
            mutex = new Mutex(false, "Local\\IMAP2ExchSync{216EE2D8-ADC7-4B5B-818A-5AB2B4E68EF1}", out running);
            if (!running)
            {
                MessageBox.Show("Приложение уже запущено", "Ошибка");
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                IMAP2ExchSyncApplicationContext exchposerApplicationContext = new IMAP2ExchSyncApplicationContext(appSettingsFileName);
                if (exchposerApplicationContext.Initialized)
                    Application.Run(exchposerApplicationContext);
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Критическая ошибка приложения" + " {0}", ex.Message));
                return;
            }
        }
    }

}
