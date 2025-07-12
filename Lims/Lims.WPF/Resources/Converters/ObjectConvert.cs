namespace Lims.WPF.Resources.Converters
{
    /// <summary>
    /// CommandParameter 多参数传递
    /// </summary>
    public class ObjectConvert : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return values.Clone();
        }

        public object[] ConvertBack(object value, Type[] targetTypes,
          object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion IMultiValueConverter Members
    }
}