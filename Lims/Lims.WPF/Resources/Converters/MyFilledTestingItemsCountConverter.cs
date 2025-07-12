using Lims.Common.Dtos;
using Lims.ToolsForClient;

namespace Lims.WPF.Resources.Converters
{
    [ValueConversion(typeof(ObservableCollection<ItemDto>), typeof(double))]
    public class MyFilledTestingItemsCountConverter : IValueConverter
    {
        private string UserName = UserDto.Inatance.UserName; // "杨升";//ModuleViewModelBase._user.UserName;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                try
                {
                    var items = value as ObservableCollection<ItemDto>;

                    int count = items.Where(i => i.Tester == UserName & i.TestProgress == (int)TestProgress.检测中).Count();
                    int filledCount = items.Where(i => i.Tester == UserName & i.TestProgress == (int)TestProgress.检测中 && !string.IsNullOrWhiteSpace(i.Temp_TestResult)).Count();

                    return System.Convert.ToDouble(((double)filledCount / count * 100));
                }
                catch (Exception)
                {
                    //return 0;
                }
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}