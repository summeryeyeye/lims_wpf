using Lims.Common.Dtos;
using Lims.ToolsForClient;

namespace Lims.WPF.Resources.Converters
{
    [ValueConversion(typeof(ObservableCollection<ItemDto>), typeof(int))]
    public class OriginalRecordCompeleteItemsCountConverter : IValueConverter
    {
        private string UserName = UserDto.Inatance.UserName; //"杨升";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                try
                {
                    //string[] progresses = new string[] { "数据一审", "数据二审", "数据三审", "已完成" };
                    var items = value as ObservableCollection<ItemDto>;
                    int count = items.Where(i => i.Tester == UserName && i.TestProgress > (int)TestProgress.检测中).Count();
                    int filledCount = items.Where(i => i.Tester == UserName && i.TestProgress > (int)TestProgress.检测中 && i.IsOriginalRecordComplete).Count();

                    return System.Convert.ToInt32(((double)filledCount / count * 100));
                }
                catch (Exception)
                {
                    return 0;
                }

                //var items = (value as List<ItemModel>).Where(i => ValidationHelper.ItemIsSubmittable(i)).ToList();
                //return $"{items.Count}";
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}