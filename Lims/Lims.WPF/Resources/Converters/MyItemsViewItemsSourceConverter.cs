using Lims.Common.Dtos;

namespace Lims.WPF.Resources.Converters
{
    public class MyItemsViewItemsSourceConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var sample = values[0] as SampleDto;
                if (sample == null)
                    return null;
                ObservableCollection<ItemDto> allItems = values[1] as ObservableCollection<ItemDto>;

                return allItems.Where(i => i.SampleCode == sample.SampleCode);
            }
            catch (Exception)
            {
                // throw;
            }
            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}