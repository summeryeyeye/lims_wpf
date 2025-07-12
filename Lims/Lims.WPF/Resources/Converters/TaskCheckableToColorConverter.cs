using Lims.Common.Dtos;
using Lims.ToolsForClient;
using System.Windows;
using System.Windows.Media;

namespace Lims.WPF.Resources.Converters
{
    public class TaskCheckableToColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] != DependencyProperty.UnsetValue && values[0] != null)
            {
                var sample = values[0] as SampleDto;
                var items = sample.Items;
                //if (items.Count > 0)
                if (items.Min(i => i.TestProgress) < (int)(values[1] as TestProgress?))
                    return Color.FromRgb(255, 255, 255);
            }

            return Color.FromRgb(127, 255, 212);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
