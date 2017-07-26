using System;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace SimpleAudio
{
    /// <summary>
    /// Interaction logic for PopupWindow.xaml
    /// </summary>
    public partial class PopupWindow : Window
    {
        private Timer alphaTimer = null;
        private DateTime countdownStart;
        private const double WAIT_SEC = 3;
        private const double FADE_SEC = 3;

        public PopupWindow()
        {
            InitializeComponent();

            TaskbarHider.HideMe(this);
            this.Loaded += PopupWindow_Loaded;

            this.alphaTimer = new Timer(20);
            this.alphaTimer.AutoReset = true;
            this.alphaTimer.Elapsed += alphaTimer_Elapsed;

            this.MouseMove += PopupWindow_MouseMove;
            this.MouseLeave += PopupWindow_MouseLeave;
        }

        void PopupWindow_MouseMove(object sender, MouseEventArgs e)
        {
            alphaTimer.Stop();
            this.Opacity = 1;
        }
        void PopupWindow_MouseLeave(object sender, MouseEventArgs e)
        {
            countdownStart = DateTime.Now.AddSeconds(-WAIT_SEC);
            alphaTimer.Start();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            alphaTimer.Stop();

            base.OnClosing(e);
        }

        private void alphaTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var diff = (e.SignalTime - countdownStart).TotalSeconds;
            diff -= WAIT_SEC;
            if (diff > 0)
            {
                if (diff >= FADE_SEC)
                {
                    alphaTimer.Stop();
                    this.Dispatcher.Invoke(() => this.Hide());
                }
                else
                    this.Dispatcher.Invoke(() => this.Opacity = 1 - (diff / FADE_SEC));
            }
        }

        private void PopupWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var s = (System.Windows.Interop.HwndSource)System.Windows.Interop.HwndSource.FromVisual(this);
            s.AddHook(new HwndSourceHook(WndProc));
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_MOUSEACTIVATE = 0x0021;
            const int MA_NOACTIVATE = 3;

            if (msg == WM_MOUSEACTIVATE)
            {
                handled = true;
                return new IntPtr(MA_NOACTIVATE);
            }

            return IntPtr.Zero;
        }

        public void ShowPopup()
        {
            this.Show();

            this.Left = SystemParameters.PrimaryScreenWidth - this.Width;
            this.Top = SystemParameters.WorkArea.Height - this.Height;

            alphaTimer.Stop();
            countdownStart = DateTime.Now;
            alphaTimer.Start();
            this.Opacity = 1;
        }

        private void ImagePanel_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;
            alphaTimer.Stop();
            this.Opacity = 1;
        }
        private void ImagePanel_DragLeave(object sender, DragEventArgs e)
        {
            countdownStart = DateTime.Now.AddSeconds(-WAIT_SEC);
            alphaTimer.Start();
            this.Opacity = 1;
        }
    }
}
