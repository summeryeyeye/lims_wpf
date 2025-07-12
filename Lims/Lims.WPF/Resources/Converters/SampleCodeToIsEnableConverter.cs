using System.Text.RegularExpressions;

namespace Lims.WPF.Resources.Converters
{
    [ValueConversion(typeof(object), typeof(bool))]
    public class SampleCodeToIsEnableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value.ToString().Trim().Length == 10)
            {
                Regex sampleCodeCorrectReg = new Regex(@"^[0-9]{4}-[0-9]{5}$|^[0-9]{5}-[0-9]{4}$");

                string sampleCode = value.ToString().Trim();
                return sampleCodeCorrectReg.IsMatch(sampleCode);
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}