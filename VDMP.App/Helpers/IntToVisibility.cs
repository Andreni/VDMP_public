using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace VDMP.App.Helpers
{
    internal class IntToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!int.TryParse(value.ToString(), out var result))
                result = 0;

            return result > 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}