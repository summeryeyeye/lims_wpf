using Lims.Common.Dtos;
using System.Data;

namespace Lims.WPF.Resources.Converters
{
    [ValueConversion(typeof(ObservableCollection<ItemDto>), typeof(string))]
    public class MyItemsInCellConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                ObservableCollection<ItemDto> items = value as ObservableCollection<ItemDto>;
                string paras = parameter.ToString();
                string UserName = UserDto.Inatance.UserName; //"杨升"; //ModuleViewModelBase._user.UserName;
                DataTable dt = new();

                IEnumerable<string> arr = items.Where(i => i.Tester == UserName && (bool)dt.Compute(i.TestProgress + paras, "")).Select(i => i.TestItem);


                return string.Join(",", arr);
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}