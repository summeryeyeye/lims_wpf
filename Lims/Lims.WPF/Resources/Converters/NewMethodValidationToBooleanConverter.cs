using Lims.Common.Dtos;

namespace Lims.WPF.Resources.Converters
{
    [ValueConversion(typeof(MethodStandardDto), typeof(bool))]
    public class NewMethodValidationToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            var method = value as MethodStandardDto;
            if (string.IsNullOrWhiteSpace(method.TestItem) || string.IsNullOrWhiteSpace(method.TestMethod) || string.IsNullOrWhiteSpace(method.Tester))
                return false;
            return true;


        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
