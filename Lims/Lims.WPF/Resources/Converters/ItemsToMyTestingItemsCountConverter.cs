using Lims.Common.Dtos;
using Lims.ToolsForClient;

namespace Lims.WPF.Resources.Converters
{
    [ValueConversion(typeof(ObservableCollection<ItemDto>), typeof(string))]
    public class ItemsToMyTestingItemsCountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            int myCount = 0;
            try
            {
                if (value is ObservableCollection<ItemDto> items)
                    myCount = items.Where(i => i.TestProgress == (int)TestProgress.检测中 && i.Tester == UserDto.Inatance.UserName).Count();
            }
            catch (Exception)
            {
            }
            return $"({myCount.ToString()})";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
