using Lims.Common.Dtos;
using Lims.ToolsForClient;
using System.Windows;

namespace Lims.WPF.Resources.Converters
{
    public class TasksCheckableToIsEnableConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {

            if (values[0] != DependencyProperty.UnsetValue)
            {
                var sample = values[0] as SampleDto;
                if (sample == null)
                    return false;

                if (sample.Items.Min(i => i.TestProgress) < (int)(values[1] as TestProgress?))
                    return false;
            }

            return true;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}