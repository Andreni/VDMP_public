using System;
using Windows.UI;
using Windows.UI.Xaml.Data;

namespace VDMP.App.Helpers
{
    public class BooleanToColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool condition;

            if (!bool.TryParse(value.ToString(), out condition))
                condition = false;

            return condition ? Colors.Green.ToString() : Colors.Red.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}