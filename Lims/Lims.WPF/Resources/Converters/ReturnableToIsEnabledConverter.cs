using Lims.ToolsForClient;

namespace Lims.WPF.Resources.Converters
{
    public class ReturnableToIsEnabledConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                int relativeProgress = System.Convert.ToInt32((TestProgress)values[0]);
                int progress = System.Convert.ToInt32(values[1]);
                if (relativeProgress.Equals(progress))
                {
                    return true;
                }
            }
            catch (Exception)
            {
            }
            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}