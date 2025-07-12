using System.Windows;
using System.Windows.Controls;

namespace Lims.WPF.Resources
{
    public class FormattingRule
    {
        public virtual bool ApplyToRow
        {
            get; set;
        }
        public virtual string Expression
        {
            get; set;
        }
        public virtual string FieldName
        {
            get; set;
        }
        public virtual FormattingType Type
        {
            get; set;
        }
    }
    public enum FormattingType
    {
        Urgent, CurrentUrgent, SingleConclusion
    }

    public class FormatConditionSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (!(item is FormattingRule))
                return null;
            var vm = item as FormattingRule;
            switch (vm.Type)
            {
                case FormattingType.Urgent:
                    return UrgentTemplate;
                case FormattingType.CurrentUrgent:
                    return CurrentUrgentTemplate;
                case FormattingType.SingleConclusion:
                    return SingleConclusionTemplate;
                default:
                    return null;
            }
        }

        public DataTemplate CurrentUrgentTemplate
        {
            get; set;
        }
        public DataTemplate UrgentTemplate
        {
            get; set;
        }
        public DataTemplate SingleConclusionTemplate
        {
            get; set;
        }
    }
}
