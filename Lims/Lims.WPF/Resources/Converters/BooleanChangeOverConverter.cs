namespace Lims.WPF.Resources.Converters
{
    public class BooleanChangeOverConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null ? !(System.Convert.ToInt32(value) > 0) : false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}