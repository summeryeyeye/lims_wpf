using Lims.Common.Dtos;

namespace Lims.WPF.Resources.Converters
{
    [ValueConversion(typeof(List<UserDto>), typeof(List<UserDto>))]
    public class UsersCanTestConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                var users = value as List<UserDto>;
                return users.Where(u => u.CanTest);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}