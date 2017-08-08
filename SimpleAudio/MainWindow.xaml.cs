using System.Windows;
using System.Windows.Input;

namespace SimpleAudio
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool exiting = false;
        private PopupWindow popup;

        public MainWindow()
        {
            InitializeComponent();

            var hotkeys = new Hotkeys.HotKeyManager(this);
            hotkeys.AddHotKey(Key.J, ModifierKeys.Control | ModifierKeys.Alt, () => { this.Show(); searchControl.ResetSearch(); });
            hotkeys.AddHotKey(Key.Q, ModifierKeys.Control | ModifierKeys.Alt, () => { exiting = true; this.Close(); });

            popup = new PopupWindow();
            hotkeys.AddHotKey(Key.Space, ModifierKeys.Control | ModifierKeys.Alt, () => popup.ShowPopup());

            searchControl.ResetSearch();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (exiting)
            {
                popup.Close();
                base.OnClosing(e);
            }
            else
            {
                e.Cancel = true;
                base.OnClosing(e);
                this.Hide();
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            this.DragMove();
            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Hide();
                e.Handled = true;
            }
            else
                base.OnPreviewKeyDown(e);
        }
    }
}
