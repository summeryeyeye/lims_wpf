using Lims.Common.Dtos;

namespace Lims.WPF.Resources.Converters
{
    [ValueConversion(typeof(MethodStandardDto), typeof(string))]
    public class StandardValidToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var method = value as MethodStandardDto;
            var state = method?.StandardState;
            switch (state)
            {
                case StandardState.Expire:
                    return "Red";
                case StandardState.PartialValidity:
                    return "Yellow";
                case StandardState.Validity:
                    return "Green";
                default:
                    return "Green";
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
