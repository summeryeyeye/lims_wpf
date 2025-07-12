namespace Lims.WPF.Resources.Converters
{
    [ValueConversion(typeof(string), typeof(string))]
    public class FilePathToFileNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return value.ToString().Split('\\').Last();
            }
            catch (Exception)
            {
                return string.Empty;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
