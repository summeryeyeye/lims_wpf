using System.Text.RegularExpressions;

namespace Lims.WPF.Resources.Converters
{
    [ValueConversion(typeof(bool), typeof(string))]
    public class BooleanToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string param = parameter as string;
            if (Regex.IsMatch(param, @"\|{1}"))
                return value != null && (bool)value ? param.Split('|')[0] : param.Split('|')[1];
            return Binding.DoNothing;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}