using Lims.WPF.ViewModels;

namespace Lims.WPF.Resources.Converters
{
    [ValueConversion(typeof(ProductLevel), typeof(string))]
    public class LevelToAlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && ((ProductLevel)value).Equals(ProductLevel.ExecuteStandard))
            {
                return "Stretch";
            }
            return "Center";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}