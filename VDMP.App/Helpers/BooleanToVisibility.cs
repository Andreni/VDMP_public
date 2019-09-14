using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace VDMP.App.Helpers
{
    public class BooleanToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!bool.TryParse(value.ToString(), out var condition))
                condition = false;

            return condition ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}