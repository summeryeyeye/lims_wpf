using Lims.Common.Dtos;

namespace Lims.WPF.Resources.Converters
{
    [ValueConversion(typeof(StandardBaseDto), typeof(string))]
    public class StandardIsInvalidConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var method = value as StandardBaseDto;
            var state = method?.StandardState;
            switch (state)
            {
                case StandardState.Expire:
                    return "Strikethrough";
                case StandardState.PartialValidity:
                    return "None";
                case StandardState.Validity:
                    return "None";
                default:
                    return "None";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
