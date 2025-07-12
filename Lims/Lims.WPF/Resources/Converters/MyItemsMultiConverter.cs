using Lims.Common.Dtos;

namespace Lims.WPF.Resources.Converters
{
    public class MyItemsMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var sampleCode = (values[0] as SampleDto).SampleCode;

                IEnumerable<string> arr = (values[1] as IEnumerable<ItemDto>).Where(t => t.SampleCode == sampleCode).Select(i => i.TestItem);
                ;
                return string.Join(',', arr);
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