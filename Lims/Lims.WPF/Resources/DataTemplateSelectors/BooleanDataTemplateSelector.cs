using System.Windows;
using System.Windows.Controls;

namespace Lims.WPF.Resources.DataTemplateSelectors
{
    public class BooleanDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate TrueTemplate
        {
            get; set;
        }
        public DataTemplate FalseTemplate
        {
            get; set;
        }
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var res = (bool)item;
            return res ? TrueTemplate : FalseTemplate;
        }
    }
}
