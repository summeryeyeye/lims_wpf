using DevExpress.Mvvm;
using System.Collections;
using System.Windows;

namespace Lims.WPF.Resources
{
    public class ItemColumnModel
    {
        public string Header
        {
            get; set;
        }
        public string FieldName
        {
            get; set;
        }

        public bool ReadOnly
        {
            get; set;
        }

        public System.Windows.Style Style
        {
            get; set;
        }

        public DataTemplate DataTemplate
        {
            get; set;
        }
    }

    public enum SettingsType
    {
        Default, Lookup, Binding
    }

    public class Column : BindableBase
    {
        public Column(SettingsType settings, string fieldName)
        {
            Settings = settings;

            FieldName = fieldName;
        }

        public SettingsType Settings
        {
            get;
        }
        public string FieldName
        {
            get; set;
        }

    }

    public class LookupColumn : Column
    {
        public LookupColumn(SettingsType settings, string fieldName, IList source) : base(settings, fieldName)
        {
            Source = source;
        }

        public IList Source
        {
            get;
        }
    }

    public class BindingColumn : Column
    {
        public BindingColumn(SettingsType settings, string fieldName, string header) : base(settings, fieldName)
        {
            Header = header;
        }
        public int Width { get; set; } = 120;
        public string Header
        {
            get;
        }

        private bool readOnly = true;

        public bool ReadOnly
        {
            get
            {
                return readOnly;
            }
            set
            {
                readOnly = value;
            }
        }

        private bool visible = true;

        public bool Visible
        {
            get
            {
                return visible;
            }
            set
            {
                visible = value;
                RaisePropertyChanged(nameof(Visible));
            }
        }

        public object CellTemplate
        {
            get; set;
        }

        //public object Style { get { return Application.Current.Resources["GridControlBaseColumnStyle"]} set { } }

        private object style = Application.Current.Resources["GridControlBaseColumnStyle"];

        public object Style
        {
            get
            {
                return style;
            }
            set
            {
                style = value;
            }
        }

        private bool resizable = false;

        public bool Resizable
        {
            get
            {
                return resizable;
            }
            set
            {
                resizable = value;
            }
        }

        private object converter;

        public object Converter
        {
            get
            {
                return converter;
            }
            set
            {
                converter = value;
            }
        }

        //private string width = string.Empty;
        //public string Width
        //{
        //    get { return width; }
        //    set { width = value; }
        //}

        private string columnFixed = "None";
        public string ColumnFixed
        {
            get
            {
                return columnFixed;
            }
            set
            {
                columnFixed = value;
            }
        }
        private bool allowResizing = true;

        public bool AllowResizing
        {
            get
            {
                return allowResizing;
            }
            set
            {
                allowResizing = value;
            }
        }

        private bool allowMoving = true;

        public bool AllowMoving
        {
            get
            {
                return allowMoving;
            }
            set
            {
                allowMoving = value;
            }
        }
    }
}