using Lims.Common.Dtos;
using Lims.ToolsForClient;

namespace Lims.WPF.Resources.Converters
{
    [ValueConversion(typeof(ObservableCollection<ItemDto>), typeof(bool))]
    public class SubmitableItemsToIsEnableConverter : IValueConverter
    {
        private string UserName = UserDto.Inatance.UserName; // "杨升";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                var items = value as ObservableCollection<ItemDto>;
                if (items.Any(i => i.Tester == UserName && i.TestProgress == ((int)TestProgress.检测中) && !string.IsNullOrWhiteSpace(i.Temp_TestResult)))
                {
                    return true;
                }
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}