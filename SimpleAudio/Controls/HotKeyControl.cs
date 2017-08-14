using SimpleAudio.Hotkeys;
using System.Windows;
using System.Windows.Input;

namespace SimpleAudio.Controls
{
    public class HotKeyControl : FrameworkElement
    {
        private readonly HotKeyManager _hotkeys;

        private static readonly DependencyProperty PlayCommandProperty = DependencyProperty.Register(nameof(PlayCommand), typeof(ICommand), typeof(HotKeyControl));
        private static readonly DependencyProperty PauseCommandProperty = DependencyProperty.Register(nameof(PauseCommand), typeof(ICommand), typeof(HotKeyControl));
        private static readonly DependencyProperty PlayPauseCommandProperty = DependencyProperty.Register(nameof(PlayPauseCommand), typeof(ICommand), typeof(HotKeyControl));
        private static readonly DependencyProperty StopCommandProperty = DependencyProperty.Register(nameof(StopCommand), typeof(ICommand), typeof(HotKeyControl));

        private static readonly DependencyProperty PreviousCommandProperty = DependencyProperty.Register(nameof(PreviousCommand), typeof(ICommand), typeof(HotKeyControl));
        private static readonly DependencyProperty NextCommandProperty = DependencyProperty.Register(nameof(NextCommand), typeof(ICommand), typeof(HotKeyControl));

        private static readonly DependencyProperty SeekBackwardsCommandProperty = DependencyProperty.Register(nameof(SeekBackwardsCommand), typeof(ICommand), typeof(HotKeyControl));
        private static readonly DependencyProperty SeekForwardsCommandProperty = DependencyProperty.Register(nameof(SeekForwardsCommand), typeof(ICommand), typeof(HotKeyControl));

        public HotKeyControl()
        {
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
                return;

            _hotkeys = new HotKeyManager(App.Current.MainWindow);

            var CtrlAlt = ModifierKeys.Control | ModifierKeys.Alt;

            _hotkeys.AddHotKey(Key.Insert, CtrlAlt, () => PlayCommand);
            _hotkeys.AddHotKey(Key.Home, CtrlAlt, () => PauseCommand);
            _hotkeys.AddHotKey(Key.End, CtrlAlt, () => StopCommand);

            _hotkeys.AddHotKey(Key.PageUp, CtrlAlt, () => PreviousCommand);
            _hotkeys.AddHotKey(Key.PageDown, CtrlAlt, () => NextCommand);
            _hotkeys.AddHotKey(Key.Left, CtrlAlt, () => SeekBackwardsCommand);
            _hotkeys.AddHotKey(Key.Right, CtrlAlt, () => SeekForwardsCommand);

            _hotkeys.AddHotKey(Key.MediaPlayPause, ModifierKeys.None, () => PlayPauseCommand);
            _hotkeys.AddHotKey(Key.MediaStop, ModifierKeys.None, () => StopCommand);
            _hotkeys.AddHotKey(Key.MediaPreviousTrack, ModifierKeys.None, () => PreviousCommand);
            _hotkeys.AddHotKey(Key.MediaNextTrack, ModifierKeys.None, () => NextCommand);
        }

        public ICommand PlayCommand
        {
            get => GetValue(PlayCommandProperty) as ICommand;
            set => SetValue(PlayCommandProperty, value);
        }
        public ICommand PauseCommand
        {
            get => GetValue(PauseCommandProperty) as ICommand;
            set => SetValue(PauseCommandProperty, value);
        }
        public ICommand PlayPauseCommand
        {
            get => GetValue(PlayPauseCommandProperty) as ICommand;
            set => SetValue(PlayPauseCommandProperty, value);
        }
        public ICommand StopCommand
        {
            get => GetValue(StopCommandProperty) as ICommand;
            set => SetValue(StopCommandProperty, value);
        }

        public ICommand PreviousCommand
        {
            get => GetValue(PreviousCommandProperty) as ICommand;
            set => SetValue(PreviousCommandProperty, value);
        }
        public ICommand NextCommand
        {
            get => GetValue(NextCommandProperty) as ICommand;
            set => SetValue(NextCommandProperty, value);
        }

        public ICommand SeekBackwardsCommand
        {
            get => GetValue(SeekBackwardsCommandProperty) as ICommand;
            set => SetValue(SeekBackwardsCommandProperty, value);
        }
        public ICommand SeekForwardsCommand
        {
            get => GetValue(SeekForwardsCommandProperty) as ICommand;
            set => SetValue(SeekForwardsCommandProperty, value);
        }
    }
}
