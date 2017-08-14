using DeadDog.Audio.Playback;
using System;
using System.Globalization;
using System.Windows.Data;

namespace SimpleAudio.Converters
{
    public class StatusIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is PlayerStatus status))
                throw new ArgumentException($"Only values of type {nameof(PlayerStatus)} can be converted.");
            
            switch (status)
            {
                case PlayerStatus.Playing:
                    return new Uri("pack://siteoforigin:,,,/Resources/play.png");

                case PlayerStatus.Paused:
                    return new Uri("pack://siteoforigin:,,,/Resources/pause.png");

                case PlayerStatus.Stopped:
                case PlayerStatus.NoFileOpen:
                    return new Uri("pack://siteoforigin:,,,/Resources/stop.png");

                default: throw new ArgumentOutOfRangeException(nameof(status));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidCastException();
        }
    }
}
