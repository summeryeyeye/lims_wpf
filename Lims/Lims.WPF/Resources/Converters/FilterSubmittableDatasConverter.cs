using DevExpress.Mvvm.Native;
using Lims.Common.Dtos;

namespace Lims.WPF.Resources.Converters
{
    [ValueConversion(typeof(ObservableCollection<ItemDto>), typeof(ObservableCollection<ItemDto>))]
    public class FilterSubmittableDatasConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
                return (value as ObservableCollection<ItemDto>).Where(i => !string.IsNullOrWhiteSpace(i.Temp_TestResult)).ToObservableCollection();
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}