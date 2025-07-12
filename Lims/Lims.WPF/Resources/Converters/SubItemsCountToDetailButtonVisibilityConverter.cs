using Lims.Common.Dtos;

namespace Lims.WPF.Resources.Converters
{
    [ValueConversion(typeof(ItemDto), typeof(string))]
    public class SubItemsCountToDetailButtonVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                var row = value as ItemDto;

                if (row.SubItems != null && row.SubItems.Count > 0)
                {
                    return "Visible";
                }
            }
            return "Hidden";
            //throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}