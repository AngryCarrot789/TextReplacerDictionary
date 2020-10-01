using System;
using System.Globalization;
using System.Windows.Data;

namespace TextDictionaryReplacer.Converters
{
    public class IsSearchingToStartStopConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isSearching)
            {
                return isSearching ? "Stop" : "Start";
            }

            else return "Error";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string searchingText)
            {
                return searchingText == "Stop";
            }

            else return false;
        }
    }
}
