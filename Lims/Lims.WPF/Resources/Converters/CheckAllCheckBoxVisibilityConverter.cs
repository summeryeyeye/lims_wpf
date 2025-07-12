using Lims.WPF.ViewModels;

namespace Lims.WPF.Resources.Converters
{
    [ValueConversion(typeof(ObservableCollection<ProductNode>), typeof(string))]
    public class CheckAllCheckBoxVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                ObservableCollection<ProductNode> nodes = (ObservableCollection<ProductNode>)value;
                if (nodes.Count > 0 && (nodes.First().ProductLevel == ProductLevel.TestItem))
                {
                    return "Visible";
                }
            }
            return "Hidden";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}