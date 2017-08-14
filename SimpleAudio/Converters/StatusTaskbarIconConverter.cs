using DeadDog.Audio.Playback;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace SimpleAudio.Converters
{
    public class StatusTaskbarIconConverter : IValueConverter
    {
        private readonly DependencyObject _designObject = new DependencyObject();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is PlayerStatus status))
                throw new ArgumentException($"Only values of type {nameof(PlayerStatus)} can be converted.");

            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(_designObject))
                return null;

            switch (status)
            {
                case PlayerStatus.Playing:
                    return new BitmapImage(new Uri("pack://application:,,,/Resources/headset_play.ico"));

                case PlayerStatus.Paused:
                    return new BitmapImage(new Uri("pack://application:,,,/Resources/headset_pause.ico"));

                case PlayerStatus.Stopped:
                    return new BitmapImage(new Uri("pack://application:,,,/Resources/headset_stop.ico"));

                default:
                    return new BitmapImage(new Uri("pack://application:,,,/Resources/headset.ico"));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidCastException();
        }
    }
}
