using DevExpress.Xpf.Core;
using Lims.WPF.ViewModels;

namespace Lims.WPF.Views
{
    public partial class MainView : ThemedWindow
    {

        public MainView()
        {
            InitializeComponent();
            DataContext = new MainViewModel();



            //menuBar.MouseDoubleClick += (s, e) =>
            //{
            //    if (WindowState == System.Windows.WindowState.Maximized)
            //    {
            //        this.WindowState = System.Windows.WindowState.Normal;
            //    }
            //    else
            //    {
            //        this.WindowState = System.Windows.WindowState.Maximized;
            //    }
            //};
            //menuBar.MouseLeftButtonDown += (s, e) =>
            //{
            //    DragMove();
            //};

            //minBtn.ItemClick += (s, e) =>
            //{
            //    this.WindowState = System.Windows.WindowState.Minimized;
            //};
            //maxBtn.ItemClick += (s, e) =>
            //{
            //    //this.btnMaximize.Visibility = Visibility.Collapsed;
            //    //this.btnNormal.Visibility = Visibility.Visible;
            //    //rcnormal = new Rect(this.Left, this.Top, this.Width, this.Height);//保存下当前位置与大小
            //    //this.Left = 0;//设置位置
            //    //this.Top = 0;
            //    //Rect rc = SystemParameters.WorkArea;//获取工作区大小
            //    //this.Width = rc.Width;
            //    //this.Height = rc.Height;
            //    if (WindowState == System.Windows.WindowState.Maximized)
            //    {
            //        this.WindowState = System.Windows.WindowState.Normal;
            //    }
            //    else
            //    {
            //        this.WindowState = System.Windows.WindowState.Maximized;
            //    }
            //};
            //closeBtn.ItemClick += (s, e) =>
            //{
            //    Application.Current.Shutdown();
            //};
        }
    }
}