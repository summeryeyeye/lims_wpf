using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;

namespace Lims.WPF.Resources.Converters
{
    [ValueConversion(typeof(bool), typeof(BitmapImage))]
    public class BooleanToGlyphConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string param = parameter as string;
            if (Regex.IsMatch(param, @"\|{1}"))
            {
                return (bool)value
                    ? new BitmapImage(new Uri(param.Split('|')[0]))
                    : new BitmapImage(new Uri(param.Split('|')[1]));
            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
