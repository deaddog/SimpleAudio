using DeadDog.Audio.Scan;
using System;
using System.ComponentModel;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SimpleAudio
{
    public class ScannerBackgroundWorker
    {
        private static readonly DeadDog.Audio.Parsing.IDataParser parser = new DeadDog.Audio.MediaParser();
        private static readonly MD5 md5 = MD5.Create();

        private static string getHash(string text)
        {
            byte[] buffer = md5.ComputeHash(Encoding.UTF8.GetBytes(text));
            StringBuilder sb = new StringBuilder(buffer.Length * 2);

            foreach (byte b in buffer)
                sb.AppendFormat("{0:x2}", b);

            return sb.ToString();
        }

        private BackgroundWorker worker;
        private AudioScanner scanner;
        private readonly string cachepath;

        public ScannerBackgroundWorker(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            string hash = getHash(path);
            this.cachepath = Path.ChangeExtension(Path.Combine(App.ApplicationDataPath, hash), "cache");

            if (File.Exists(cachepath))
            {
                throw new NotImplementedException();
            }
            else
                this.scanner = new AudioScanner(parser, path);

            scanner.FileParsed += scanner_FileParsed;

            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += worker_DoWork;
            worker.ProgressChanged += worker_ProgressChanged;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
        }

        public void RunAync()
        {
            worker.RunWorkerAsync();
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            scanner.RunScanner(null);
        }
        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (FileParsed != null)
                FileParsed(this, e.UserState as ScanFileEventArgs);
        }
        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (ScanDone != null)
                ScanDone(this, EventArgs.Empty);
        }

        public event EventHandler ScanDone;
        public event ScanFileEventHandler FileParsed;

        void scanner_FileParsed(object sender, ScanFileEventArgs e)
        {
            worker.ReportProgress(0, e);
        }
    }
}
