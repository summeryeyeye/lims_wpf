using Lims.Common.Dtos;

namespace Lims.WPF.Resources.Converters
{
    [ValueConversion(typeof(string), typeof(int))]
    public class TaskTypeToSelectedIndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                TaskType type = (TaskType)Enum.Parse(typeof(TaskType), value.ToString());
                return (int)type;
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((TaskType)(int)value).ToString();
        }
    }
}