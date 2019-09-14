using System;
using Windows.UI.Xaml.Data;

namespace VDMP.App.Helpers
{
    public class BooleanConditionToIcon : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool condition;

            if (!bool.TryParse(value.ToString(), out condition))
                condition = false;

            if (condition)
                return 57643;
            return 57722;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}