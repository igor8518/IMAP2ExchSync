using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace IMAP2ExchSync
{
    
    public class StreamReaderWithLog : StreamReader
    {
        private readonly Action<int, string> Log = null;
        public StreamReaderWithLog(Stream stream, Action<int, string> logger = null) :base(stream)           
        {
            this.Log = logger; 
        }
        public string ReadLine(bool log = false)
        {
            try
            {
                string line;
                line = base.ReadLine();
                if (log)
                {
                    Log?.Invoke(1, "<= " + line);
                }
                return line;
            }
            catch
            {
                throw;
            }
        }

    }
    public class BinaryReaderS : BinaryReader
    {
        private readonly Action<int, string> Log = null;
        public BinaryReaderS(Stream stream) : base(stream)
        {
            
        }
        public string ReadLn()
        {
            char ch;
            try
            {
                string line = "";
                do
                {
                    ch = base.ReadChar();
                    line += ch;
                }
                while (ch != '\r');
                ch = ReadChar();
                line += ch;
                if (ch == '\n')
                {
                    line = "1" + line; ;
                }
                else
                {
                    line = "0" + line;                  
                }
                return line;
            }
            catch
            {
                throw;
            }
        }

    }
    public sealed class StreamWriterWithLog : StreamWriter
    {
        private readonly Action<int, string> Log;
        public StreamWriterWithLog(Stream stream, Action<int, string> logger = null) :base(stream)           
        {
            this.Log = logger;
        }
        public void WriteLine(string line,bool log = false)
        {
            try
            {
                base.WriteLine(line);
                if (log)
                {
                    Log?.Invoke(1, "=> " + line);
                }
            }
            catch
            {
                throw;
            }
        }

    }
}
