using DevExpress.Mvvm;
using DevExpress.Mvvm.UI;
using DevExpress.Mvvm.UI.ModuleInjection;
using DevExpress.Xpf.Core;
using Lims.WPF.Services.Services;
using System.Windows;

namespace Lims.WPF
{
    public partial class App
    {

        public App()
        {

            CompatibilitySettings.UseLightweightThemes = true;
            ApplicationThemeHelper.UpdateApplicationThemeName();
            //SplashScreenManager.CreateThemed().ShowOnStartup();
            ApplicationThemeHelper.Preload(PreloadCategories.Core);
            //SplashScreenManager.CreateThemed().ShowOnStartup();
            //SplashScreenManager.CreateThemed(new DXSplashScreenViewModel
            //{
            //    Status = "Starting...",
            //    Title = "The Best or Nothing!"
            //}).ShowOnStartup();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            RegistServices();
        }

        /// <summary>
        /// 注册服务
        /// </summary>
        private static void RegistServices()
        {
            //模块实例
            ServiceContainer.Default.RegisterService(new SampleService());
            ServiceContainer.Default.RegisterService(new ItemService());
            ServiceContainer.Default.RegisterService(new SubItemService());
            ServiceContainer.Default.RegisterService(new SubItemStandardService());
            ServiceContainer.Default.RegisterService(new UserService());
            ServiceContainer.Default.RegisterService(new LoggerService());
            ServiceContainer.Default.RegisterService(new ProductStandardService());
            ServiceContainer.Default.RegisterService(new MethodStandardService());
            ServiceContainer.Default.RegisterService(new ReagentService());

            //内置服务实例
            ServiceContainer.Default.RegisterService(new DXMessageBoxService());
            ServiceContainer.Default.RegisterService(new VisualStateService());
            ServiceContainer.Default.RegisterService(new SplashScreenManagerService());
            ServiceContainer.Default.RegisterService(new OpenFileDialogService());



            //ServiceContainer.Default.RegisterService(new HttpRestClient(@"http://localhost:5000/"));
        }

        protected override void OnExit(ExitEventArgs e)
        {
            ApplicationThemeHelper.SaveApplicationThemeName();
            base.OnExit(e);
        }
    }
}