using DeadDog.Audio.Scan;
using System;
using System.ComponentModel;

namespace SimpleAudio
{
    public class ScannerBackgroundWorker
    {
        private BackgroundWorker worker;
        private AudioScanner scanner;

        public ScannerBackgroundWorker(AudioScanner scanner)
        {
            if (scanner == null)
                throw new ArgumentNullException("scanner");

            this.scanner = scanner;
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
