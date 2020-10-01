using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace TextDictionaryReplacer.Converters
{
    public class IsSearchingToColourConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isSearching)
            {
                return isSearching ? new SolidColorBrush(Color.FromRgb(200, 50, 50)) : new SolidColorBrush(Color.FromRgb(50, 200, 50));
            }

            else return new SolidColorBrush(Color.FromRgb(50, 50, 200));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SolidColorBrush background)
            {
                return background.Color == Color.FromRgb(200, 50, 50);
            }

            else return false;
        }
    }
}
