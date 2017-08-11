using System.Windows;
using System.Windows.Input;

namespace SimpleAudio
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var hotkeys = new Hotkeys.HotKeyManager(this);
            hotkeys.AddHotKey(Key.J, ModifierKeys.Control | ModifierKeys.Alt, () => 
            {
                Show(); searchControl.ResetSearch();
            });
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            base.OnClosing(e);
            Hide();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            DragMove();
            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Hide();
                e.Handled = true;
            }
            else
                base.OnPreviewKeyDown(e);
        }
    }
}
