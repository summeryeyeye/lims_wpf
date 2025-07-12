using DevExpress.Xpf.Editors.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lims.WPF.Resources.Converters
{
    [ValueConversion(typeof(DateTime), typeof(DateTimeOffset))]
    internal class DateTimeToOffsetConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dateTime = (DateTimeOffset)value;

            if (dateTime.Offset.Equals(TimeSpan.Zero))
                return dateTime.UtcDateTime;
            else if (dateTime.Offset.Equals(TimeZoneInfo.Local.GetUtcOffset(dateTime.DateTime)))
                return DateTime.SpecifyKind(dateTime.DateTime, DateTimeKind.Local);
            else
                return dateTime.DateTime;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var date = (DateTime)value;
            DateTimeOffset d = DateTime.SpecifyKind(date, DateTimeKind.Local);
            return d;
        }
    }
}
