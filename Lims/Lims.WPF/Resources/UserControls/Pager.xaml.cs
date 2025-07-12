using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Lims.WPF.Resources.UserControls
{
    /// <summary>
    /// Pager.xaml 的交互逻辑
    /// </summary>
    public partial class Pager : UserControl
    {
        public static RoutedEvent FirstPageEvent;
        public static RoutedEvent PreviousPageEvent;
        public static RoutedEvent NextPageEvent;
        public static RoutedEvent LastPageEvent;
        public static RoutedEvent RefreshEvent;

        public static readonly DependencyProperty CurrentPageProperty;
        public static readonly DependencyProperty TotalPageProperty;
        public static readonly DependencyProperty TotalItemProperty;

        public string CurrentPage
        {
            get
            {
                return (string)GetValue(CurrentPageProperty);
            }
            set
            {
                SetValue(CurrentPageProperty, value);
            }
        }

        public string TotalPage
        {
            get
            {
                return (string)GetValue(TotalPageProperty);
            }
            set
            {
                SetValue(TotalPageProperty, value);
            }
        }

        public string TotalItem
        {
            get
            {
                return (string)GetValue(TotalItemProperty);
            }
            set
            {
                SetValue(TotalItemProperty, value);
            }
        }

        public Pager()
        {
            InitializeComponent();
        }

        static Pager()
        {
            FirstPageEvent = EventManager.RegisterRoutedEvent("FirstPage", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(Pager));
            PreviousPageEvent = EventManager.RegisterRoutedEvent("PreviousPage", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(Pager));
            NextPageEvent = EventManager.RegisterRoutedEvent("NextPage", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(Pager));
            LastPageEvent = EventManager.RegisterRoutedEvent("LastPage", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(Pager));
            RefreshEvent = EventManager.RegisterRoutedEvent("Refresh", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(Pager));

            CurrentPageProperty = DependencyProperty.Register("CurrentPage", typeof(string), typeof(Pager), new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnCurrentPageChanged)));
            TotalPageProperty = DependencyProperty.Register("TotalPage", typeof(string), typeof(Pager), new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnTotalPageChanged)));
            TotalItemProperty = DependencyProperty.Register("TotalItem", typeof(string), typeof(Pager), new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnTotalItemChanged)));
        }

        public event RoutedEventHandler FirstPage
        {
            add
            {
                AddHandler(FirstPageEvent, value);
            }
            remove
            {
                RemoveHandler(FirstPageEvent, value);
            }
        }

        public event RoutedEventHandler PreviousPage
        {
            add
            {
                AddHandler(PreviousPageEvent, value);
            }
            remove
            {
                RemoveHandler(PreviousPageEvent, value);
            }
        }

        public event RoutedEventHandler NextPage
        {
            add
            {
                AddHandler(NextPageEvent, value);
            }
            remove
            {
                RemoveHandler(NextPageEvent, value);
            }
        }

        public event RoutedEventHandler LastPage
        {
            add
            {
                AddHandler(LastPageEvent, value);
            }
            remove
            {
                RemoveHandler(LastPageEvent, value);
            }
        }

        public event RoutedEventHandler Refresh
        {
            add
            {
                AddHandler(RefreshEvent, value);
            }
            remove
            {
                RemoveHandler(RefreshEvent, value);
            }
        }

        public static void OnTotalPageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Pager p = d as Pager;
            if (p != null)
            {
                Run rTotalPage = (Run)p.FindName("rTotalPage");

                rTotalPage.Text = (string)e.NewValue;
            }
        }

        public static void OnTotalItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Pager p = d as Pager;
            if (p != null)
            {
                Run rTotalItem = (Run)p.FindName("rTotalItem");

                rTotalItem.Text = (string)e.NewValue;
            }
        }

        private static void OnCurrentPageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Pager p = d as Pager;

            if (p != null)
            {
                Run rCurrrent = (Run)p.FindName("rCurrent");

                rCurrrent.Text = (string)e.NewValue;
            }
        }

        private void FirstPageButton_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(FirstPageEvent, this));
        }

        private void PreviousPageButton_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(PreviousPageEvent, this));
        }

        private void NextPageButton_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(NextPageEvent, this));
        }

        private void LastPageButton_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(LastPageEvent, this));
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(RefreshEvent, this));
        }
    }
}