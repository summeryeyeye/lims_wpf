namespace Lims.WPF.Resources.Converters
{
    [ValueConversion(typeof(string), typeof(string))]
    public class TextLengthToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrWhiteSpace(value.ToString()))
                return "Hidden";
            return "Visible";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}