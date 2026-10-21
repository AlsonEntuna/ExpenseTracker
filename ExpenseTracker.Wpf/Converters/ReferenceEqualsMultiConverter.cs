using System;
using System.Globalization;
using System.Windows.Data;

namespace ExpenseTracker.Wpf.Converters
{
    /// <summary>
    /// Returns true when the two bound values are the same object instance.
    /// </summary>
    public class ReferenceEqualsMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length != 2)
            {
                return false;
            }

            return ReferenceEquals(values[0], values[1]);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
