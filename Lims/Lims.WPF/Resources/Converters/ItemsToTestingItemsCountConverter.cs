using Lims.Common.Dtos;
using Lims.ToolsForClient;

namespace Lims.WPF.Resources.Converters
{
    [ValueConversion(typeof(ObservableCollection<ItemDto>), typeof(int))]
    public class ItemsToTestingItemsCountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int count = 0;
            try
            {
                var items = value as ObservableCollection<ItemDto>;
                count = items.Where(i => i.TestProgress <= (int)TestProgress.检测中).Count();
            }
            catch (Exception)
            {
            }
            return count;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}