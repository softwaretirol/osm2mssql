using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace osm2mssql.Importer.Classes
{
    public class OsmTextWriterTraceListener : TraceListener
    {
        private string _logFileLocation;
        private DateTime _CurrentDate;
        StreamWriter _TraceWriter;
        private const string LogDirectory = "Logfiles";
        public OsmTextWriterTraceListener(string fileName)
        {
            _logFileLocation = LogDirectory + "\\" + fileName;
            
            OpenWriter();

            WriteLine("-----------------------------------------");
            WriteLine(string.Empty);
            WriteLine(string.Empty);
            WriteLine("- Logging started . . .");
            WriteLine(string.Empty);
            WriteLine(string.Empty);
            WriteLine("-----------------------------------------");
        }

        private void OpenWriter()
        {
            if (_TraceWriter != null)
            {
                _TraceWriter.Close();
                _TraceWriter.Dispose();
            }

            var file = new FileStream(GenerateFileName(), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            file.Position = file.Length;
            _TraceWriter = new StreamWriter(file);
            _TraceWriter.AutoFlush = true;
        }

        public override void Write(string message)
        {
            CheckRollover();
            WriteLine(message);
        }

        public override void Write(string message, string category)
        {
            CheckRollover();
            WriteLine(category + "\t" + message);
        }


        public override void WriteLine(string message)
        {
            CheckRollover();
            var sb = new StringBuilder();
            sb.Append(DateTime.Now);
            sb.Append(" - ");
            sb.Append(message);
            _TraceWriter.WriteLine(sb.ToString());
        }


        private string GenerateFileName()
        {
            if (!Directory.Exists(LogDirectory))
                Directory.CreateDirectory(LogDirectory);
            _CurrentDate = DateTime.Today;
            return Path.Combine(Path.GetDirectoryName(_logFileLocation), Path.GetFileNameWithoutExtension(_logFileLocation) + "_" + _CurrentDate.ToString("yyyyMMdd") + Path.GetExtension(_logFileLocation));
        }

        private void CheckRollover()
        {
            if (_CurrentDate != DateTime.Today)
            {
                OpenWriter();
            }
        }

        public override void Flush()
        {
            lock (this)
            {
                if (_TraceWriter != null)
                {
                    _TraceWriter.Flush();
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _TraceWriter.Close();
            }
        }
    }
}
