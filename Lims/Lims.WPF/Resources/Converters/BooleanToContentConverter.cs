using System.Text.RegularExpressions;

namespace Lims.WPF.Resources.Converters
{
    [ValueConversion(typeof(bool), typeof(string))]
    public class BooleanToContentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string param = parameter as string;
            var result = Regex.IsMatch(param, @"\|{1}") ? (bool)value ? param.Split('|')[1] : param.Split('|')[0] : Binding.DoNothing;
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
            //throw new NotImplementedException();
        }
    }
}
