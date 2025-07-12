namespace Lims.WPF.Resources.Converters
{
    [ValueConversion(typeof(int), typeof(string))]
    public class CountToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //int c = System.Convert.ToInt32(parameter);

            if (value != null && System.Convert.ToInt32(value) > 0)
            {
                return "Visible";
            }
            return "Hidden";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}