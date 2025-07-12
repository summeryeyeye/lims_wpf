namespace Lims.WPF.Resources.Converters
{
    [ValueConversion(typeof(string), typeof(string))]
    public class ResizeItemIsCheckedToGlyphConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if ((bool)value)
                {
                    return "SvgImages/Outlook Inspired/DefaultPrinter.svg";
                }
                else
                {
                    return "SvgImages/Outlook Inspired/DefaultPrinter.svg";
                }
            }
            else
            {
                return string.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}