namespace Lims.WPF.Resources.Converters
{
    [ValueConversion(typeof(bool), typeof(string))]
    public class CheckAllConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && (bool)value)
            {
                return "取消";
            }
            return "全选";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}