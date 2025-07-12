namespace Lims.WPF.Resources.Converters
{
    public class HasStringToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? "Hidden" : string.IsNullOrEmpty(value.ToString()) ? "Hidden" : "Visible";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
