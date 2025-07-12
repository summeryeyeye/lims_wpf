using Lims.Common.Dtos;
using Lims.ToolsForClient;

namespace Lims.WPF.Resources.Converters
{
    [ValueConversion(typeof(ObservableCollection<ItemDto>), typeof(int))]
    public class SubmittedItemsCountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                var source = value as ObservableCollection<ItemDto>;

                return source.Where(s => s.TestProgress > (int)TestProgress.检测中).Count();
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}