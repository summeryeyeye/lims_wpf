using Lims.Common.Dtos;

namespace Lims.WPF.Resources.Converters
{
    [ValueConversion(typeof(IEnumerable<ItemDto>), typeof(string))]
    public class MyItemsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                var items = value as IEnumerable<ItemDto>;
                string userName = UserDto.Inatance!.UserName!; // "杨升"; //ModuleViewModelBase._user.UserName;
                int progress = System.Convert.ToInt32(parameter);
                var arr = items!.Where(i => i.Tester == userName && i.TestProgress == progress).Select(i => i.TestItem);
                return string.Join(',', arr);
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}