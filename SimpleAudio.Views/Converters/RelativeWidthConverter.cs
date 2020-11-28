using System;
using System.Windows.Data;

namespace SimpleAudio.Views.Converters
{
    public class RelativeWidthConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(values[0] is double) || !(values[1] is double))
                return null;

            var relativeWidth = (double)values[0];
            var containingWidth = (double)values[1];

            return containingWidth * relativeWidth;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
