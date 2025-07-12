namespace Lims.WPF.Resources.Converters
{
    [ValueConversion(typeof(string), typeof(bool))]
    public class ValueableResultToIsEnableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if (string.IsNullOrWhiteSpace(value as string))
                    return false;
                return true;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
            //throw new NotImplementedException();
        }
    }
}