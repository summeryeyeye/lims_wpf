namespace Lims.WPF.Resources.Converters
{
    [ValueConversion(typeof(object), typeof(string))]
    public class IsNullToVisibilityConverterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {           

            return value == null ? "Hidden" : "Visible";           

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}