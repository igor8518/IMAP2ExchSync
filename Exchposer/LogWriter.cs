using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace IMAP2ExchSync
{
    public class LogWriter
    {
        private int maxLogFileSize;
        private int maxLogFileCount;

        private int level = 0;
        private string logFileName = "";

        private StreamWriter fileWriter = null;

        public string LogFileName { get { return logFileName; } set { logFileName = value; Close(); } }
        public int Level { get { return level; } set { level = value; } }

        
        //Конструктор
        public LogWriter(bool autoOpen, int level, string logFileName, int maxLogFileSize = 0, int maxLogFileCount = 0)
        {
            this.level = level;            
            this.logFileName = logFileName;
            this.maxLogFileSize = maxLogFileSize;
            this.maxLogFileCount = maxLogFileCount;

            if (autoOpen)
                Open();
        }
        //Открытие файла лога
        private void Open()
        {
            Close();

            //Directory.CreateDirectory(Path.GetDirectoryName(logFileName));
            //fileWriter = File.AppendText(logFileName);
            fileWriter = new StreamWriter(new FileStream(logFileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite));
            fileWriter.AutoFlush = true;
        }
        //Закрытие файла лога
        public void Close()
        {
            if (fileWriter != null)
            {
                fileWriter.Close();
                fileWriter.Dispose();
                fileWriter = null;
            }
        }
        //Оборот файла лога
        private void Rotate()
        {
            if (!File.Exists(logFileName))
                return;

            Close();

            string directoryName = Path.GetDirectoryName(logFileName);
            string baseFileName = Path.GetFileName(logFileName);
            int fileNameLength = logFileName.Length;

            foreach (var f in Directory.EnumerateFiles(directoryName, baseFileName + ".*").
                Where(f => (f.Length > fileNameLength + 1) && (Regex.IsMatch(f.Substring(fileNameLength + 1), "^[1-9][0-9]*$"))).
                Select(f => new { ext = f.Substring(fileNameLength + 1), idx = Convert.ToInt32(f.Substring(fileNameLength + 1)) }).
                OrderByDescending(f => f.idx))
            {
                if (f.idx + 1 >= maxLogFileCount)
                    File.Delete(logFileName + "." + f.ext);
                else
                    File.Move(logFileName + "." + f.ext, logFileName + "." + (f.idx + 1).ToString());
            }

            if (maxLogFileCount > 1)
                File.Move(logFileName, logFileName + ".1");
            else
                File.Delete(logFileName);
        }

        
        //Запись строки лога
        public string Log(int level, string message)
        {
            lock (this)
            {
                if (fileWriter == null)
                    Open();

                string logText = "";
                logText = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff").Substring(0, 23) + " " + message;
                if (level <= this.level)
                {
                    
                    fileWriter.WriteLine(logText);
                }

                if ((maxLogFileSize > 0) && (maxLogFileCount > 1) && (fileWriter.BaseStream.Length > maxLogFileSize))
                    Rotate();

                return logText;
            }
        }

        //Отчистка файла лога
        public void Clear()
        {
            if (fileWriter != null)
                Close();
            for (int i = maxLogFileCount - 1; i >= 0; --i)
            {
                string nextFileName = logFileName + (i > 0 ? "." + i.ToString() : "");
                if (File.Exists(nextFileName))
                    File.Delete(nextFileName);
            }
        }
        
        //Загрузка файла лога
        public void Load(Action<string> loadAction)
        {
            if (loadAction == null)
                return;
            for (int i = maxLogFileCount - 1; i >= 0; --i)
            {
                string nextFileName = logFileName + (i > 0 ? "." + i.ToString() : "");
                if (File.Exists(nextFileName))
                    using (FileStream logFile = new FileStream(nextFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (StreamReader logReader = new StreamReader(logFile))
                        loadAction(logReader.ReadToEnd());
            }
        }
    }
}
