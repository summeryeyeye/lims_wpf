namespace Lims.WPF.Resources.Converters
{
    [ValueConversion(typeof(bool), typeof(string))]
    public class IsSelectedToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && (bool)value)
            {
                return "Visible";
            }
            return "Hidden";

            // throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}