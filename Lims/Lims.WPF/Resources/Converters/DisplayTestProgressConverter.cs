using Lims.ToolsForClient;

namespace Lims.WPF.Resources.Converters
{
    [ValueConversion(typeof(int), typeof(string))]
    public class DisplayTestProgressConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                TestProgress progress = (TestProgress)((int)value);

                return progress.ToString();
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}