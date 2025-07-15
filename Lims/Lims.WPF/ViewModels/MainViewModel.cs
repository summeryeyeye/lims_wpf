using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.ModuleInjection;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
using Lims.Common.Dtos;
using Lims.ToolsForClient;
using Lims.ToolsForClient.Extensions;
using Lims.WPF.Navigations;
using Lims.WPF.Services.Interface;
using Lims.WPF.Tools;
using Lims.WPF.Views;
using Microsoft.AspNetCore.SignalR.Client;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.Common;
//using System.Data.SqlClient;
using System.Diagnostics;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Module = DevExpress.Mvvm.ModuleInjection.Module;
namespace Lims.WPF.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private IMessageBoxService MessageBoxService => ServiceContainer.GetService<IMessageBoxService>();
        private IUserService UserService => ServiceContainer.GetService<IUserService>();
        private ILoggerService LoggerService => ServiceContainer.GetService<ILoggerService>();
        private IReagentService ReagentService => ServiceContainer.GetService<IReagentService>();
        public virtual int Delay
        {
            get; set;
        }
        protected ISplashScreenManagerService SplashScreenManagerService
        {
            get
            {
                return ServiceContainer.GetService<ISplashScreenManagerService>();
            }
        }
        private void Display()
        {

            SplashScreenManagerService.ViewModel = new DXSplashScreenViewModel();
            SplashScreenManagerService.ViewModel.Copyright = "农业农村部微生物产品质量检验测试中心（武汉）";
            SplashScreenManagerService.ViewModel.Status = "数据加载中，请稍等...";
            SplashScreenManagerService.ViewModel.Title = "数据管理系统（LIMS）";
            SplashScreenManagerService.ViewModel.Subtitle = CurrentVersion;
            SplashScreenManagerService.ViewModel.Logo = new Uri("../../Images/Logo.png", UriKind.Relative);
            SplashScreenManagerService.Show();
            Thread.Sleep(TimeSpan.FromSeconds(Delay));
            SplashScreenManagerService.Close();

        }


        [Command]
        public void Test()
        {



        }


        protected override async void OnInitializeInRuntime()
        {
            base.OnInitializeInRuntime();
            Delay = 1;
            string versionString = Environment.GetEnvironmentVariable("ClickOnce_CurrentVersion") ?? "0.0.0.0";
            Version version = Version.Parse(versionString);
            CurrentVersion = version.ToString();
            Display();
            RegisterModules();


            if (await Login())
            {

              
                //1.初始化
                InitInfo();
                //2.监听
                Listen();
                //3.连接
                Link();
              

            }
        }

        public void Dispose()
        {
            // SignalR 断开连接并解绑事件
            if (hubConnection != null)
            {
                hubConnection.Remove("TaskCount");
                hubConnection.StopAsync().Wait();
                hubConnection.DisposeAsync().AsTask().Wait();
            }
            // Messenger 解绑
            Messenger.Default.Unregister(this);

            // 其他资源释放
        }

        private readonly string serviceRoutePath = ConfigurationManager.AppSettings["ServiceRoutePath"];
        private HubConnection hubConnection;
        /// <summary>
        /// 初始化Connection对象
        /// </summary>
        private void InitInfo()
        {
            var url= $"{serviceRoutePath}TaskCount";
            HubConnectionBuilder hubConnectionBuilder = new HubConnectionBuilder();
            hubConnectionBuilder.WithUrl(url, options => { });
            hubConnection = new HubConnectionBuilder().WithUrl(serviceRoutePath + "TaskCount").WithAutomaticReconnect().Build();
            //自定义重连规则实现
            hubConnectionBuilder = (HubConnectionBuilder)hubConnectionBuilder
                .WithAutomaticReconnect(new RetryPolicy());
            hubConnection.KeepAliveInterval = TimeSpan.FromSeconds(60);
        }
        //实现IRetryPolicy接口
        class RetryPolicy : IRetryPolicy
        {
            /// <summary>
            /// 重连规则：重连次数<50：间隔1s;重试次数<250:间隔30s;重试次数>250:间隔1m
            /// </summary>
            /// <param name="retryContext"></param>
            /// <returns></returns>
            public TimeSpan? NextRetryDelay(RetryContext retryContext)
            {
                var count = retryContext.PreviousRetryCount / 50;
                if (count < 1)//重试次数<50,间隔1s
                {
                    return new TimeSpan(0, 0, 1);
                }
                else if (count < 5)//重试次数<250:间隔30s
                {
                    return new TimeSpan(0, 0, 30);
                }
                else //重试次数>250:间隔1m
                {
                    return new TimeSpan(0, 1, 0);
                }
            }
        }
        /// <summary>
        /// 监听
        /// </summary>
        private void Listen()
        {
            hubConnection.On<string>("TaskCount", ReceiveInfos);
        }

        /// <summary>
        /// 连接
        /// </summary>
        private async void Link()
        {
            try
            {
                await hubConnection.StartAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ReceiveInfos(string data)
        {
            if (string.IsNullOrWhiteSpace(data))            
                return;            
            var taskCount = Newtonsoft.Json.JsonConvert.DeserializeObject<TaskCountDto>(data);
            if (taskCount == null)
                return;

            int.TryParse(taskCount.unFinishedTasks.ToString(), out int unFinishedTasks);
            int.TryParse(taskCount.firstCheckTasks.ToString(), out int firstCheckTasks);
            int.TryParse(taskCount.sencondCheckTasks.ToString(), out int sencondCheckTasks);
            int.TryParse(taskCount.thirdCheckTasks.ToString(), out int thirdCheckTasks);

            if (unFinishedTasksNavigationItem != null)
                unFinishedTasksNavigationItem.Count = unFinishedTasks;
            if (firstCheckTasksNavigationItem != null)
                firstCheckTasksNavigationItem.Count = firstCheckTasks;
            if (sencondCheckTasksNavigationItem != null)
                sencondCheckTasksNavigationItem.Count = sencondCheckTasks;
            if (thirdCheckTasksNavigationItem != null)
                thirdCheckTasksNavigationItem.Count = thirdCheckTasks;

            if (myReceivableTasksNavigationItem != null)
            {
                var dr = taskCount.MyReceivableTasks.Select($"Tester='{currentUser?.UserName}'").FirstOrDefault();
                myReceivableTasksNavigationItem.Count = dr != null ? System.Convert.ToInt32(dr["count"]) : 0;
            }

            if (myTestingTasksNavigationItem != null)
            {
                var dr = taskCount.MyTestingTasks.Select($"Tester='{currentUser?.UserName}'").FirstOrDefault();
                myTestingTasksNavigationItem.Count = dr != null ? System.Convert.ToInt32(dr["count"]) : 0;

            }
            if (myReturnedTasksNavigationItem != null)
            {
                var dr = taskCount.MyReturnedTasks.Select($"Tester='{currentUser?.UserName}'").FirstOrDefault();
                myReturnedTasksNavigationItem.Count = dr != null ? System.Convert.ToInt32(dr["count"]) : 0;

                //myReturnedTasksNavigationItem.Count = (int)taskCount.MyReturnedTasks.Select(CurrentUser.UserName).FirstOrDefault()["count"];
            }


            var drLog = taskCount.MyUnreadLogs.Select($"Tester='{currentUser?.UserName}'").FirstOrDefault();
            UnReadLoggersCount = drLog != null ? System.Convert.ToInt32(drLog["count"]) : 0;

            //UnReadLoggersCount = (int)taskCount.MyUnreadLogs.Select(CurrentUser.UserName).FirstOrDefault()["count"];
        }


        private string? currentVersion;

        public string? CurrentVersion
        {
            get
            {
                return currentVersion;
            }
            set
            {
                currentVersion = value;
                RaisePropertyChanged(nameof(CurrentVersion));
            }
        }

        public MainViewModel()
        {

        }
        public ObservableCollection<LoggerDto>? MyLogsSource
        {
            get; set;
        }
        public List<string>? LoginedUserIds { get; set; } = new();

        /// <summary>
        /// 打开消息窗口
        /// </summary>
        /// <returns></returns>
        [Command]
        public async Task ViewLogger()
        {
            if (UserDto.Inatance != null)
            {
                var response = await LoggerService.GetLoggersByFilterAsync(new Common.Parameters.LoggerFilterParam() { ReceiverName = UserDto.Inatance.UserName, LogLevel = (int)LogLevel.WARN });
                if (response.Status)
                    if (response.Result != null)
                        MyLogsSource = response.Result.OrderByDescending(l => l.CreateTime).ToObservableCollection();
            }

            var dialogService = GetService<IDialogService>("MyLogsViewDialogService");
            dialogService.ShowDialog(
                new List<UICommand> {
                        new UICommand{Caption = "关闭",IsDefault = false,IsCancel = true,}
                }
                , "", this);
        }
        /// <summary>
        /// 将选定消息标记为已读
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        [Command]
        public async Task ReadLog(LoggerDto log)
        {
            try
            {
                await LoggerService.UpdateAsync(log);
            }
            catch (Exception)
            {
                // ignored
            }
        }
        /// <summary>
        /// 一键已读所有我的消息
        /// </summary>
        /// <returns></returns>
        [Command]
        public async Task ReadAllLogs()
        {
            try
            {
                if (MyLogsSource != null)
                {
                    foreach (var l in MyLogsSource)
                        l.IsReaded = true;

                    await LoggerService.UpdateRangeAsync(MyLogsSource.ToList());
                }
            }
            catch (Exception)
            {

                //throw;
            }
        }
        /// <summary>
        /// 打开公共盘
        /// </summary>
        /// <returns></returns>
        [Command]
        public async static Task OpenCommonDisk()
        {
            await Task.Run(() =>
            {
                string commonDiskRootPath = @"\\" + ConfigurationManager.AppSettings["CommonDiskRootPath"];
                _ = Process.Start(new ProcessStartInfo($@"{commonDiskRootPath}\微生物测试中心公共盘") { UseShellExecute = true });

            });
        }

        public ObservableCollection<StandardFileItem>? StandardFileItems
        {
            get => standardFileItems;
            set => SetProperty(ref standardFileItems, value, "StandardFileItems");
        }

        /// <summary>
        /// 查看标准
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        [Command]
        public Task SearchStandardKeyUp(KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var methodName = ((e.OriginalSource as TextBox)!).Text;
                if (!string.IsNullOrWhiteSpace(methodName))
                {

                    string standardBasePath = @"\\" + ConfigurationManager.AppSettings["CommonDiskRootPath"] + @"\微生物测试中心公共盘\标准库";
                    try
                    {
                        var paths = FilesManager.GetFileSystemEntries(standardBasePath, methodName, recurse: true);
                        StandardFileItems = new();
                        foreach (var path in paths)
                            StandardFileItems.Add(new StandardFileItem { FilePath = path });
                        var dialog = GetService<IDialogService>("SearchStandardFileViewService");
                        dialog.ShowDialog(
                    new List<UICommand> {
                            new UICommand{Caption="关闭",IsDefault=false,IsCancel=true,},
                    }, "", this);
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// 弹出试剂管理窗口
        /// </summary>
        /// <returns></returns>
        [Command]
        public async void PopupReagentManagementView()
        {
            (await ReagentService.GetAllAsync()).Result.ToObservableCollection();
            var dialog = GetService<IDialogService>("ReagentIOManagementViewService");
            dialog.ShowDialog(
        new List<UICommand> {
                            new UICommand{Caption="关闭",IsDefault=false,IsCancel=true,},
                            }, "", this);
        }
        private int spinValue;

        public int SpinValue
        {
            get
            {
                return spinValue;
            }
            set
            {
                spinValue = value;
                RaisePropertyChanged(nameof(SpinValue));
            }
        }

        /// <summary>
        /// 试剂入库
        /// </summary>
        /// <param name="reagentDto"></param>
        [Command]
        public void ReagentInput(ReagentDto reagentDto)
        {
            SpinValue = 0;
            var dialogService = GetService<IDialogService>("SpinViewDialogService");

            async void ExecuteMethod()
            {
                reagentDto.Count += SpinValue;
                await ReagentService.UpdateAsync(reagentDto);
            }

            dialogService.ShowDialog(
                new List<UICommand> {
                        new UICommand{Caption = "确定",IsDefault = true,IsCancel = false,Command=new DelegateCommand(ExecuteMethod),},
                        new UICommand{Caption = "关闭窗口",IsDefault = false,IsCancel = true,},
                }
                , "", this);
        }
        /// <summary>
        /// 试剂出库
        /// </summary>
        /// <param name="reagentDto"></param>
        [Command]
        public void ReagentOutput(ReagentDto reagentDto)
        {
            SpinValue = 0;
            var dialogService = GetService<IDialogService>("SpinViewDialogService");

            async void ExecuteMethod()
            {
                if (SpinValue > reagentDto.Count)
                {
                    DXMessageBox.Show("出库数量不得大于库存余量！");
                    return;
                }

                reagentDto.Count -= SpinValue;
                await ReagentService.UpdateAsync(reagentDto);
            }

            dialogService.ShowDialog(
                new List<UICommand> {
                        new UICommand{Caption = "确定",IsDefault = true,IsCancel = false,Command=new DelegateCommand(ExecuteMethod),},
                        new UICommand{Caption = "关闭窗口",IsDefault = false,IsCancel = true,},
                }
                , "", this);
        }


        #region login

        public virtual string? UserId
        {
            get; set;
        }

        public virtual string? Password
        {
            get; set;
        }

        //private bool rememberMe;

        //public bool RememberMe
        //{
        //    get { return rememberMe; }
        //    set { rememberMe = value; RaisePropertyChanged(nameof(RememberMe)); }
        //}

        private bool autoLogin;

        public bool AutoLogin
        {
            get => autoLogin;
            set
            {
                autoLogin = value;
                RaisePropertyChanged(nameof(AutoLogin));
            }
        }

        private string title = string.Empty;

        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                title = value;
                RaisePropertyChanged(nameof(Title));
            }
        }

        [Command(false)]
        public List<UICommand> Buttons()
        {
            return new List<UICommand>
            {
                new UICommand
                {
                    Caption = "登录",
                    IsDefault = true,
                    IsCancel = false,
                    Command = new DelegateCommand<CancelEventArgs>(_ => { },
                        _ => !string.IsNullOrWhiteSpace(UserId) && !string.IsNullOrWhiteSpace(Password))
                },
                new UICommand
                {
                    Caption = "免密登录",
                    IsDefault = false,
                    IsCancel = false,
                    Command = new DelegateCommand<CancelEventArgs>(_ => { })
                },
            };
        }

        /// <summary>
        /// 免密登录
        /// </summary>
        private async Task<bool> Login()
        {
            UserId = ConfigurationManager.AppSettings["userID"];
            AutoLogin = Convert.ToBoolean(ConfigurationManager.AppSettings["autoLogin"]);
            LoginedUserIds = ConfigurationManager.AppSettings["loginedUserIds"]?.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
            //RememberMe = Convert.ToBoolean(ConfigurationManager.AppSettings["rememberMe"].ToString());

            if (AutoLogin)
            {
                var user = (await UserService.GetSingleAsync(UserId)).Result;
                if (user == null)
                {
                    MessageBoxService.ShowMessage("账号不存在,请重试!");
                    AutoLogin = false;
                    return false;
                }
                UserDto.Inatance = CurrentUser = user;



                Password = ConfigurationManager.AppSettings["passWord"];
                await CreateNavigationItems();
                //await UpdateTasksCount();
                return true;
            }
            else
            {
                var dialogService = GetService<IDialogService>("LoginDialogService");
                var notificationService = GetService<INotificationService>("NotificationService");

                while (CurrentUser == null)
                {
                    var result = dialogService.ShowDialog(Buttons(), "", this);

                    if (result.Caption.ToString() == "免密登录")
                    {
                        IsGuest = true;
                        await CreateNavigationItems();
                        UserDto.Inatance = CurrentUser = new UserDto() { UserName = "guest" };
                        return true;
                    }
                    else if (result.Caption.ToString() == "登录")
                    {

                        UserDto.Inatance = CurrentUser = (await UserService.GetSingleAsync(UserId)).Result;
                        //CurrentUser = UserDto.Inatance;
                        if (CurrentUser == null)
                        {
                            await notificationService.CreatePredefinedNotification("登陆失败！", "请检查账号或密码是否正确！", "").ShowAsync();
                        }
                        else if (CurrentUser.Passwd == Password)
                        {

                            await CreateNavigationItems();
                            Cfa.AppSettings.Settings["userID"].Value = UserId;
                            Cfa.AppSettings.Settings["passWord"].Value = Password;
                            Cfa.AppSettings.Settings["autoLogin"].Value = AutoLogin.ToString();
                            if (LoginedUserIds != null && UserId != null && !LoginedUserIds.Contains(UserId))
                            {
                                LoginedUserIds.Add(UserId);
                            }

                            if (LoginedUserIds != null)
                                Cfa.AppSettings.Settings["loginedUserIds"].Value = string.Join(',', LoginedUserIds);
                            Cfa.Save();
                            return true;
                        }

                    }
                }
                return false;
            }
        }


        [Command]
        public void ExistLogin()
        {
            try
            {
                AutoLogin = false;
                Cfa.AppSettings.Settings["autoLogin"].Value = AutoLogin.ToString();
                Cfa.Save();
            }
            catch (Exception)
            {
                // ignored
            }
            finally
            {
                Application.Current.Shutdown();
            }

        }
        #endregion login
        protected static IModuleManager NavigationManager => ModuleManager.DefaultManager;
        /// <summary>
        /// 注册模块
        /// </summary>
        private void RegisterModules()
        {
            NavigationManager.GetRegion(Regions.Documents).VisualSerializationMode = VisualSerializationMode.PerKey;
            NavigationManager.Register(Regions.Documents, new Module(Indexes.Index, () => IndexViewModel.Create("首页"), typeof(IndexView)));
            NavigationManager.Register(Regions.Documents, new Module(Indexes.TestingTasks, () => TestingTasksViewModel.Create("未完成任务"), typeof(TestingTasksView)));
            NavigationManager.Register(Regions.Documents, new Module(Indexes.ViewAllTasks, () => ViewAllTasksViewModel.Create("所有任务"), typeof(ViewAllTasksView)));
            NavigationManager.Register(Regions.Documents, new Module(MyTasks.MyReceivableTasks, () => MyReceivableTasksViewModel.Create("待领取任务"), typeof(MyReceivableTasksView)));
            NavigationManager.Register(Regions.Documents, new Module(MyTasks.MyTestingTasks, () => MyTestingTasksViewModel.Create("检测中任务"), typeof(MyTestingTasksView)));
            NavigationManager.Register(Regions.Documents, new Module(MyTasks.MyReturnedTasks, () => MyReturnedTasksViewModel.Create("已退回任务"), typeof(MyReturnedTasksView)));
            NavigationManager.Register(Regions.Documents, new Module(MyTasks.MySubmittedTasks, () => MySubmittedTasksViewModel.Create("已提交任务"), typeof(MySubmittedTasksView)));
            NavigationManager.Register(Regions.Documents, new Module(TasksManagement.TasksCreate, () => TasksCreateViewModel.Create("任务指派"), typeof(TasksCreateView)));
            NavigationManager.Register(Regions.Documents, new Module(TasksManagement.TasksEdit, () => CurrentTasksManageViewModel.Create("任务管理"), typeof(CurrentTasksManageView)));


            //_iNavigationManager.Register(Regions.Documents, new Module(TasksManagement.DataRecovery, () => DataRecoveryViewModel.Create("数据恢复"), typeof(DataRecoveryView)));


            NavigationManager.Register(Regions.Documents, new Module(TasksChecking.TasksFirtCheck, () => TasksFirstCheckViewModel.Create("数据一审"), typeof(TasksFirstCheckView)));
            NavigationManager.Register(Regions.Documents, new Module(TasksChecking.TasksSecondCheck, () => TasksSecondCheckViewModel.Create("数据二审"), typeof(TasksSecondCheckView)));
            NavigationManager.Register(Regions.Documents, new Module(TasksChecking.TasksThirdCheck, () => TasksThirdCheckViewModel.Create("数据三审"), typeof(TasksThirdCheckView)));
            NavigationManager.Register(Regions.Documents, new Module(BackstageManagement.UserManagement, () => UserManagementViewModel.Create("用户管理"), typeof(UserManagementView)));
            NavigationManager.Register(Regions.Documents, new Module(BackstageManagement.StandardManagement, () => StandardManagementViewModel.Create("标准库管理"), typeof(StandardManagementView)));
            NavigationManager.Register(Regions.Documents, new Module(MaterialManagement.ReagentManegement, () => ReagentManagementViewModel.Create("试剂管理"), typeof(ReagentManagementView)));
        }
        public ObservableCollection<NavigationItem> NavigationItems { get; set; } = new();
        public bool IsGuest
        {
            get; set;
        }

        private NavigationItem? unFinishedTasksNavigationItem;
        private NavigationItem? myReceivableTasksNavigationItem;
        private NavigationItem? myTestingTasksNavigationItem;
        private NavigationItem? myReturnedTasksNavigationItem;
        private NavigationItem? firstCheckTasksNavigationItem;
        private NavigationItem? sencondCheckTasksNavigationItem;
        private NavigationItem? thirdCheckTasksNavigationItem;
        private Task CreateNavigationItems()
        {
            UserDto.Inatance = new UserDto() { UserName = "guest" };

            unFinishedTasksNavigationItem = new NavigationItem("未完成任务") { ShowCount = true, Glyph = new BitmapImage(new Uri("pack://application:,,,/DevExpress.Images.v24.2;component/Images/Toolbox Items/Subreport_16x16.png")) };
            var indexs = new List<NavigationItem>() { new NavigationItem("首页",false) {
                Childrens =
                [
                    new NavigationItem("首页")
                    {
                        Glyph = new BitmapImage(new Uri(
                            "pack://application:,,,/DevExpress.Images.v24.2;component/Images/Navigation/Home_16x16.png"))
                    },
                    new NavigationItem("所有任务")
                    {
                        Glyph = new BitmapImage(new Uri(
                            "pack://application:,,,/DevExpress.Images.v24.2;component/Images/RichEdit/ViewMergedData_16x16.png"))
                    },
                    unFinishedTasksNavigationItem
                ]
            } };
            NavigationItems.AddRange(indexs);

            if (!IsGuest)
            {
                UserDto.Inatance = CurrentUser;
                if (CurrentUser != null && CurrentUser.CanTest)
                {
                    myReceivableTasksNavigationItem = new NavigationItem("待领取任务") { ShowCount = true, Glyph = new BitmapImage(new Uri("pack://application:,,,/DevExpress.Images.v24.2;component/Images/Tasks/NewTask_16x16.png")) };
                    myTestingTasksNavigationItem = new NavigationItem("检测中任务") { ShowCount = true, Glyph = new BitmapImage(new Uri("pack://application:,,,/DevExpress.Images.v24.2;component/Images/Tasks/EditTask_16x16.png")) };
                    myReturnedTasksNavigationItem = new NavigationItem("已退回任务") { ShowCount = true, Glyph = new BitmapImage(new Uri("pack://application:,,,/DevExpress.Images.v24.2;component/Images/Spreadsheet/TableConvertToRange_16x16.png")) };
                    var myTasks = new List<NavigationItem>() { new NavigationItem("我的任务",false) {
                        Childrens = new List<NavigationItem?>() {
                    myReceivableTasksNavigationItem,
                    myTestingTasksNavigationItem,
                    myReturnedTasksNavigationItem,
                    new NavigationItem("已提交任务") { Glyph = new BitmapImage(new Uri("pack://application:,,,/DevExpress.Images.v24.2;component/Images/Tasks/Task_16x16.png")) } ,
                 } } };
                    NavigationItems.AddRange(myTasks);
                }
                if (CurrentUser != null && CurrentUser.CanCheck)
                {
                    firstCheckTasksNavigationItem = new NavigationItem("数据一审") { ShowCount = true, Glyph = new BitmapImage(new Uri("pack://application:,,,/DevExpress.Images.v24.2;component/Images/People/User_16x16.png")) };
                    sencondCheckTasksNavigationItem = new NavigationItem("数据二审") { ShowCount = true, Glyph = new BitmapImage(new Uri("pack://application:,,,/DevExpress.Images.v24.2;component/Images/Scheduling/GroupByResource_16x16.png")) };
                    thirdCheckTasksNavigationItem = new NavigationItem("数据三审") { ShowCount = true, Glyph = new BitmapImage(new Uri("pack://application:,,,/DevExpress.Images.v24.2;component/Images/Business Objects/BOPosition2_16x16.png")) };
                    var checkTasks = new List<NavigationItem>() { new NavigationItem("任务审核",false) {
                        Childrens = new List<NavigationItem?>() {
                    firstCheckTasksNavigationItem,
                    sencondCheckTasksNavigationItem,
                    thirdCheckTasksNavigationItem,
                } } };
                    NavigationItems.AddRange(checkTasks);
                }
                if (CurrentUser != null && CurrentUser.CanManage)
                {
                    var manageTasks = new List<NavigationItem>() { new NavigationItem("任务管理", false) {
                        Childrens =
                        [
                            new NavigationItem("任务指派")
                            {
                                Glyph = new BitmapImage(new Uri(
                                    "pack://application:,,,/DevExpress.Images.v24.2;component/Images/Snap/Datasource_16x16.png"))
                            },
                            new NavigationItem("任务管理")
                            {
                                Glyph = new BitmapImage(new Uri(
                                    "pack://application:,,,/DevExpress.Images.v24.2;component/Images/Conditional Formatting/ManageRules_16x16.png"))
                            }
                            //new NavigationItem("数据恢复") {Glyph=new BitmapImage(new Uri("pack://application:,,,/DevExpress.Images.v24.2;component/Images/Data/ManageQueries_16x16.png"))},

                        ]
                    } };
                    NavigationItems.AddRange(manageTasks);
                }
                if (CurrentUser != null && CurrentUser.IsAdmin)
                {
                    var backStagementManage = new List<NavigationItem>() { new NavigationItem("后台管理", false) {
                        Childrens =
                        [
                            new NavigationItem("用户管理")
                            {
                                Glyph = new BitmapImage(new Uri(
                                    "pack://application:,,,/DevExpress.Images.v24.2;component/Images/Business Objects/BOUser_16x16.png"))
                            },
                            new NavigationItem("标准库管理")
                            {
                                Glyph = new BitmapImage(new Uri(
                                    "pack://application:,,,/DevExpress.Images.v24.2;component/Images/Maps/WeightedPies_16x16.png"))
                            }
                        ]
                    } };
                    NavigationItems.AddRange(backStagementManage);
                }

                if (CurrentUser is not { IsAdmin: true })
                    return Task.CompletedTask;
                var materialManagement = new List<NavigationItem>() { new NavigationItem("物料管理", false) {
                    Childrens =
                    [
                        new NavigationItem("试剂管理")
                        {
                            Glyph = new BitmapImage(new Uri(
                                "pack://application:,,,/DevExpress.Images.v24.2;component/Images/Business Objects/BOUser_16x16.png"))
                        }
                    ]
                } };
                NavigationItems.AddRange(materialManagement);
            }

            return Task.CompletedTask;
        }

        private UserDto? currentUser;

        public UserDto? CurrentUser
        {
            get
            {
                return currentUser;
            }
            set
            {
                currentUser = value;
                RaisePropertyChanged(nameof(CurrentUser));
                Title = $"{currentUser?.UserName}_农业农村部微生物产品质量检验测试中心（武汉）——数据管理系统";
            }
        }

        // public static UserDto CurrentUser => UserDto.Inatance;

        private static bool _isLogined = true;
        public bool IsLogined
        {
            get
            => _isLogined;
            set
            {
                _isLogined = value;
                RaisePropertyChanged(nameof(IsLogined));
            }
        }

        protected Configuration Cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        private int unReadloggersCount;
        private ObservableCollection<StandardFileItem>? standardFileItems;

        public int UnReadLoggersCount
        {
            get => unReadloggersCount;
            set
            {
                unReadloggersCount = value;
                RaisePropertyChanged(nameof(UnReadLoggersCount));
            }
        }

    }
    public class NavigationItem : INavigationItem
    {
        public NavigationItem(string header, bool canExcute = true)
        {
            Header = header;
            if (canExcute)
                Command = new DelegateCommand(() =>
                {
                    ModuleManager.DefaultManager.InjectOrNavigate(Regions.Documents, header);
                });

        }

        public string? FontWeight
        {
            get; set;
        }
        public string Header
        {
            get; set;
        }

        public bool ShowCount
        {
            get;
            set;
        }



        private int count;
        public int Count
        {
            get => count;
            set
            {
                count = value;
                RaisePropertyChanged(nameof(Count));
            }
        }

        public BitmapImage? Glyph
        {
            get; set;
        }


        //public bool IsEnabled
        //{
        //    get; set;
        //}
        //public string Region { get; set; }
        public object? DataContext
        {
            get; set;
        }

        public DataTemplate? PeekFormTemplate
        {
            get; set;
        }

        public DataTemplateSelector? PeekFormTemplateSelector
        {
            get; set;
        }

        public ICommand? Command
        {
            get; set;
        }

        public object? CommandParameter
        {
            get; set;
        }

        public IInputElement? CommandTarget
        {
            get; set;
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public List<NavigationItem?>? Childrens
        {
            get; set;
        }

    }


}