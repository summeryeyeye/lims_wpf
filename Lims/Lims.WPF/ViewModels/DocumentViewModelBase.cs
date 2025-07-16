using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
using Lims.Common.Dtos;
using Lims.Common.Parameters;
using Lims.ToolsForClient;
using Lims.WPF.Resources;
using Lims.WPF.Services;
using Lims.WPF.Services.Interface;
using Lims.WPF.Tools;
using NPOI.Util;
using RestSharp;
using Spire.Doc;
using Spire.Doc.Documents;
using System.Configuration;
using System.Drawing.Printing;
using System.Windows.Input;

namespace Lims.WPF.ViewModels
{
    public abstract class DocumentViewModelBase : ViewModelBase, IDocumentModule, IDisposable
    {
        protected HttpRestClient client => (HttpRestClient)ServiceContainer.GetService<IRestClient>();
        protected ISampleService _sampleService => ServiceContainer.GetService<ISampleService>();
        protected IItemService _itemService => ServiceContainer.GetService<IItemService>();      
        protected ISubItemService _subItemService => ServiceContainer.GetService<ISubItemService>();
        protected IUserService _userService => ServiceContainer.GetService<IUserService>();
        protected ILoggerService _loggerService => ServiceContainer.GetService<ILoggerService>();
        protected ISubItemStandardService _subItemStandardService => ServiceContainer.GetService<ISubItemStandardService>();
        protected IMethodStandardService _methodStandardService => ServiceContainer.GetService<IMethodStandardService>();
        protected IProductStandardService _productStandardService => ServiceContainer.GetService<IProductStandardService>();
        protected IMessageBoxService _messageBoxService => ServiceContainer.GetService<IMessageBoxService>();
        protected IOpenFileDialogService _iOpenFileDialogService => ServiceContainer.GetService<IOpenFileDialogService>();
        protected IReagentService _iReagentService => ServiceContainer.GetService<IReagentService>();
        protected Configuration? cfa;
        protected override async void OnInitializeInRuntime()
        {
            base.OnInitializeInRuntime();

            cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            await GetAllUsers();
        }
        public DocumentViewModelBase()
        {
            GetFormattingRules();
        }
        protected abstract Task LoadMainDatas(UserDto? user);
        public ObservableCollection<UserDto?>? Users { get; set; }
        private ObservableCollection<UserDto?>? testers;

        public ObservableCollection<UserDto?>? Testers
        {
            get { return testers; }
            set { testers = value; RaisePropertyChanged(nameof(Testers)); }
        }

        private async Task GetAllUsers()
        {
            var response = await _userService.GetAllAsync();
            if (response.Status)
            {
                Users = response.Result.ToObservableCollection();
                Testers = Users.Where(u => u.CanTest).ToObservableCollection();
                MethodTesters = Testers.DistinctBy(t => t.SuperiorName).Select(t => t.SuperiorName).ToObservableCollection();
            }

        }
        /// <summary>
        /// 用于添加新方法的检测人名称集合
        /// </summary>
        private ObservableCollection<string?>? methodTesters;

        public ObservableCollection<string?>? MethodTesters
        {
            get { return methodTesters; }
            set { methodTesters = value; RaisePropertyChanged(nameof(MethodTesters)); }
        }

        private string? selectedMethodTester;

        public string? SelectedMethodTester
        {
            get { return selectedMethodTester; }
            set { selectedMethodTester = value; RaisePropertyChanged(nameof(SelectedMethodTester)); }
        }




        public static UserDto? CurrentUser => UserDto.Inatance;

        protected async Task ShowNotifaction(string? str1, string str2, string str3)
        {
            var notificationService = GetService<INotificationService>("NotificationService");

            var notify = notificationService.CreatePredefinedNotification(str1, str2, str3);

            _ = await notify.ShowAsync();
            //Thread.Sleep(100);
        }
        public static int CurrentPrinterIndex
        {
            get; set;
        }
        public MethodStandardDto? CreattingMethod
        {
            get; set;
        }


        [Command]
        public virtual async void RefreshSampleDatas()
        {
            OnlyViewUrgent = false;
            await LoadMainDatas(CurrentUser);
            //刷新数据,同时刷新菜单栏项目数
        }

        //protected async Task UpdateSampleTestProgress(SampleDto sample)
        //{
        //    var response = await _itemService.GetAllItemsBySampleCodeAsync(new ItemFilterParam() { SampleCode = sample.SampleCode });
        //    if (response.Status)
        //    {
        //        var items = response.Result;
        //        sample.MinTestProgress = items.Min(i => i.TestProgress);
        //        sample.MaxTestProgress = items.Max(i => i.TestProgress);
        //        _ = await _sampleService.UpdateAsync(sample);
        //    }
        //}

        public List<FormattingRule> SampleFormatConditionRules
        {
            get; protected set;
        }
        public List<FormattingRule> TaskListFormatConditionRules
        {
            get; protected set;
        }
        protected virtual void GetFormattingRules()
        {
            var sampleRules = new List<FormattingRule>();
            sampleRules.Add(new FormattingRule()
            {
                Expression = "[IsUrgent]='True'",
                ApplyToRow = true,
                Type = FormattingType.Urgent
            });
            sampleRules.Add(new FormattingRule()
            {
                Expression = "[CurrentUrgent]='True'",
                ApplyToRow = true,
                Type = FormattingType.CurrentUrgent
            });
            SampleFormatConditionRules = sampleRules;

            var taskListRules = new List<FormattingRule>();
            taskListRules.Add(new FormattingRule()
            {
                Expression = "[Sample.IsUrgent]='True'",
                ApplyToRow = true,
                Type = FormattingType.Urgent
            });
            taskListRules.Add(new FormattingRule()
            {
                Expression = "[Sample.CurrentUrgent]='True'",
                ApplyToRow = true,
                Type = FormattingType.CurrentUrgent
            });
            taskListRules.Add(new FormattingRule()
            {
                Expression = "[SingleConclusion]='不符合'",
                ApplyToRow = true,
                Type = FormattingType.SingleConclusion
            });

            TaskListFormatConditionRules = taskListRules;
        }

        /// <summary>
        /// 样品数据源
        /// </summary>

        private ObservableCollection<SampleDto?> samplesSource = new ObservableCollection<SampleDto?>();

        public ObservableCollection<SampleDto?> SamplesSource
        {
            get => samplesSource;
            set
            {
                samplesSource = value;
                RaisePropertyChanged(nameof(SamplesSource));
            }
        }
        private ObservableCollection<ItemDto?> itemsSource = [];

        public ObservableCollection<ItemDto?> ItemsSource
        {
            get => itemsSource;
            set
            {
                itemsSource = value;
                RaisePropertyChanged(nameof(ItemsSource));
            }
        }
        private ObservableCollection<ItemDto?>? taskDatasSource = new ObservableCollection<ItemDto?>();

        public ObservableCollection<ItemDto?>? TaskDatasSource
        {
            get => taskDatasSource;
            set
            {
                taskDatasSource = value;
                RaisePropertyChanged(nameof(TaskDatasSource));
            }
        }

        private SampleDto currentSample;

        public SampleDto CurrentSample
        {
            get { return currentSample; }
            set { currentSample = value; RaisePropertyChanged(nameof(CurrentSample)); }
        }


        #region 只看加急
        protected Collection<ItemDto?> itemDtos;
        protected Collection<SampleDto?> sampleDtos;
        private bool onlyViewUrgent;
        public bool OnlyViewUrgent
        {
            get
            {
                return onlyViewUrgent;
            }
            set
            {
                onlyViewUrgent = value;
                urgentFilter(value);
                RaisePropertyChanged(nameof(OnlyViewUrgent));
            }
        }

        protected virtual void urgentFilter(bool value)
        {
            if (value)
            {
                try
                {
                    itemDtos = TaskDatasSource?.Copy();
                    TaskDatasSource = TaskDatasSource?.Where(i => i.Sample.IsUrgent || i.Sample.CurrentUrgent).ToObservableCollection();

                    sampleDtos = SamplesSource?.Copy();
                    SamplesSource = SamplesSource?.Where(s => s.IsUrgent || s.CurrentUrgent).ToObservableCollection();
                }
                catch (Exception)
                {

                }
            }
            else
            {
                TaskDatasSource = itemDtos?.ToObservableCollection();
                SamplesSource = sampleDtos?.ToObservableCollection();
            }
        }




        #endregion
        /// <summary>
        /// 显示加载动画
        /// </summary>
        private bool showMainDatasLoadingPanel;

        public bool ShowMainDatasLoadingPanel
        {
            get => showMainDatasLoadingPanel;
            set
            {
                showMainDatasLoadingPanel = value;
                RaisePropertyChanged(nameof(ShowMainDatasLoadingPanel));
            }
        }

        private string? inputBoxText;

        public string? InputBoxText
        {
            get => inputBoxText;
            set
            {
                inputBoxText = value;
                RaisePropertyChanged(nameof(InputBoxText));
            }
        }

        public string SearchStandardKeyword { get; set; }



        private ObservableCollection<StandardFileItem> standardFileItems;
        public ObservableCollection<StandardFileItem> StandardFileItems
        {
            get { return standardFileItems; }
            set { standardFileItems = value; RaisePropertyChanged(nameof(StandardFileItems)); }
        }


        //public ObservableCollection<StandardFileItem> StandardFileItems { get; set; }

        /// 查看标准
        /// </summary>
        /// <param name="item"></param>
        [Command]
        public async void ViewStandard(DevExpress.Xpf.Grid.EditGridCellData item)
        {
            if (item.Value == null || ((Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.Control))
                return;
            string? methodName = item.Value.ToString(); //(item.Row as ItemDto).ExecuteStandard;
            string? method;
            if (!string.IsNullOrEmpty(methodName))
            {
                StandardFileItems = new();
                SearchStandardKeyword = methodName;
                var standards = new ObservableCollection<StandardFileItem>();
                await Task.Run(() =>
                {
                    try
                    {
                        string standardBasePath = @"\\" + ConfigurationManager.AppSettings["CommonDiskRootPath"]?.ToString() + @"\微生物测试中心公共盘\标准库";
                        method = methodName.Trim().Replace("/", "").Split(' ')[0] + " " + methodName.Trim().Replace("/", "").Split(' ')[1];


                        var paths = FilesManager.GetFileSystemEntries(standardBasePath, method, recurse: true);

                        foreach (var p in paths)
                            standards.Add(new StandardFileItem { FilePath = p });

                        StandardFileItems = standards;

                        SearchStandardKeyword = methodName.Trim().Split(' ')[0] + " " + methodName.Trim().Replace("/", "").Split(' ')[1]; ;
                    }
                    catch (Exception)
                    {
                    }
                });


                var dialog = GetService<IDialogService>("SearchStandardFileViewService");
                dialog.ShowDialog(
                new List<UICommand> {
                        new UICommand{Caption="关闭",IsDefault=false,IsCancel=true,},

                }, "", this);





            }
        }

        /// <summary>
        /// 查看项目备注
        /// </summary>
        /// <returns></returns>
        [Command]
        public void ViewItemRemark(ItemDto item)
        {
            //await ShowNotifaction("检测备注", $"{item.ItemRemark}", "");
            _messageBoxService.ShowMessage(item.ItemRemark, "项目备注");
        }

        /// <summary>
        /// 查看检测备注
        /// </summary>
        /// <returns></returns>
        [Command]
        public void ViewTestRemark(ItemDto item)
        {
            _messageBoxService.ShowMessage(item.TestRemark, "检测备注");
        }


        private List<Printer>? printers;

        public List<Printer>? Printers
        {
            get
            {
                printers = new();
                foreach (string item in PrinterSettings.InstalledPrinters)
                {
                    printers.Add(new Printer { PrinterName = item });
                }

                return printers;
            }
            set => printers = value;
        }
        public string limsPath = ConfigurationManager.AppSettings["LimsPath"].ToString();
        protected async Task 打印任务随行单(ObservableCollection<ItemDto> previewSources, Printer selectedPrinter)
        {
            await ShowNotifaction("", "打印程序已启动，请稍等！！", "");
            ItemDto[] printSources = new ItemDto[previewSources.Count];
            previewSources.CopyTo(printSources, 0);
            //await Task.Run(async () =>
            //{
            if (printSources != null)
            {

                string tasksTempDocPath = limsPath + @"\工具库\其他模板\任务随行单.docx";

                try
                {
                    Document document = new Document(tasksTempDocPath);
                    if (document == null)
                    {
                        DXMessageBox.Show("模板不存在,请检查后重试!");
                        return;
                    }
                    var section = document.Sections[0];
                    HeaderFooter footer = section.HeadersFooters.Footer;
                    Paragraph footerPara = footer.AddParagraph();
                    //添加字段类型为页码，添加当前页、分隔线以及总页数
                    footerPara.AppendField("页码", FieldType.FieldPage);
                    footerPara.AppendText(" / ");
                    footerPara.AppendField("总页数", FieldType.FieldNumPages);
                    footerPara.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Right;

                    var table = section.Tables[0];

                    string itemName = "";
                    int tRow = 2;
                    ItemDto preRow = printSources.First();
                    TableRow row = table.Rows[10].Clone();
                    foreach (ItemDto item in printSources)
                    {
                        SampleDto? sample = (await _sampleService.GetSingleAsync(item.SampleCode)).Result;
                        if (item.SampleCode != preRow.SampleCode)//号不同换行
                        {
                            tRow++;
                            if (tRow > 10)
                            {
                                var newRow = row.Clone();
                                table.Rows.Insert(tRow, newRow);
                            }
                            itemName = "";
                        }
                        else if (item.ProductStandardId == 0)//无执行标准换行
                        {
                            if (item.MethodStandardId != preRow.MethodStandardId)
                            {
                                tRow += 1;
                                if (tRow > 10)
                                {
                                    var newRow = row.Clone();
                                    table.Rows.Insert(tRow, newRow);
                                }
                                itemName = "";
                            }
                        }
                        else if (item.ProductStandard?.ExecuteStandard != preRow.ProductStandard?.ExecuteStandard)//号同执行标准不同换号
                        {
                            tRow += 1;
                            if (tRow > 10)
                            {
                                var newRow = row.Clone();
                                table.Rows.Insert(tRow, newRow);
                            }
                            itemName = "";
                        }



                        table[tRow, 0].FirstParagraph.Text = item.SampleCode;
                        table[tRow, 1].FirstParagraph.Text = sample.SampleName;
                        itemName += itemName == "" ? item.TestItem : ',' + item.TestItem;
                        if (item.SubItems != null && item.SubItems.Count > 0)
                        {
                            foreach (SubItemDto subItem in item.SubItems)
                            {
                                itemName += itemName == "" ? subItem.TestItem : ',' + subItem.TestItem;
                            }
                        }
                        table[tRow, 2].FirstParagraph.Text = itemName;
                        table[tRow, 3].FirstParagraph.Text = item.ProductStandardId != 0 ? item.ProductStandard?.ExecuteStandard : item.MethodStandard.TestMethod;
                        preRow = item;
                    }

                    PrintDocument printDoc = document.PrintDocument;
                    printDoc.PrinterSettings.PrinterName = selectedPrinter.PrinterName;
                    printDoc.Print();
                }
                catch (Exception e)
                {
                    DXMessageBox.Show(e.Message);
                }

            }
            //});
        }
        public List<BindingColumn>? SubItemColumnsSource
        {
            get; set;
        }
        protected static void GetSubItemColumns(out List<BindingColumn> list)
        {
            list = new List<BindingColumn>() {
            new BindingColumn(SettingsType.Binding, "TestItem","检测项目"),
            new BindingColumn(SettingsType.Binding, "ReportUnit","报告单位"),
            new BindingColumn(SettingsType.Binding, "Temp_TestResult","检测结果"){ReadOnly=false},
            new BindingColumn(SettingsType.Binding, "IndexRequest","指标要求"),
            new BindingColumn(SettingsType.Binding, "ItemRemark","项目备注"),
            };
        }
        protected static void GetEditableSubItemColumns(out List<BindingColumn> list)
        {
            list = new List<BindingColumn>() {
            new BindingColumn(SettingsType.Binding, "TestItem","检测项目"),
            new BindingColumn(SettingsType.Binding, "ReportUnit","报告单位"),
            new BindingColumn(SettingsType.Binding, "TestResult","检测结果"),
            new BindingColumn(SettingsType.Binding, "IndexRequest","指标要求"),
            new BindingColumn(SettingsType.Binding, "ItemRemark","项目备注"),
            };
        }

        private int selectedCreatingSampleStateIndex;

        public int SelectedCreatingSampleStateIndex
        {
            get
            {
                return selectedCreatingSampleStateIndex;
            }
            set
            {
                selectedCreatingSampleStateIndex = value;
            }
        }

        [Command]
        public void ShowCreateMethodView()
        {
            SelectedMethodTester = string.Empty;
            CreattingMethod = new()
            {
                SampleState = "固体",
                //TestItem = method.Text,
                LastUpdater = CurrentUser.UserName
            };

            var dialogService = GetService<IDialogService>("CreateMethodViewDialogService");
            dialogService.ShowDialog(new List<UICommand> {
                        new UICommand{Caption = "确定添加",IsDefault = true,IsCancel = false,AllowCloseWindow=false,Command=new DelegateCommand( async() =>
                        {
                           if (string.IsNullOrWhiteSpace(CreattingMethod.TestItem) || string.IsNullOrWhiteSpace(CreattingMethod.TestMethod) || string.IsNullOrWhiteSpace(SelectedMethodTester)){
                                _messageBoxService.ShowMessage("方法格式不正确,添加失败!");
                                return;
                            }else
                            {
                                CreattingMethod.KeyItem=string.IsNullOrEmpty(CreattingMethod.KeyItem)?CreattingMethod.TestItem:CreattingMethod.KeyItem;
                                CreattingMethod.StandardState=StandardState.Validity;
                                CreattingMethod.SampleState=SampleStates[SelectedCreatingSampleStateIndex].SampleState;
                                CreattingMethod.Tester=SelectedMethodTester;
                                await _methodStandardService.CreateAsync(CreattingMethod);
                                await ShowNotifaction("", "方法添加成功!", "");
                            }

                        })},
                        new UICommand{Caption = "关闭窗口",IsDefault = false,IsCancel = true,}
                }, "", this);
        }
        //是否回收完毕
        bool _disposed;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
            //throw new NotImplementedException();
        }
        ~DocumentViewModelBase()
        {
            Dispose(false);
        }
        //这里的参数表示示是否需要释放那些实现IDisposable接口的托管对象
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return; //如果已经被回收，就中断执行
            if (disposing)
            {
                //TODO:释放那些实现IDisposable接口的托管对象
            }
            //TODO:释放非托管资源，设置对象为null
            _disposed = true;
        }

        public string? Caption
        {
            get; set;
        }

        public virtual bool IsActive
        {
            get; set;
        }
        public static void Print(string tempFileName)
        {

            try
            {
                //初始化Document实例
                Document doc = new Document();

                //加载一个Word文档
                doc.LoadFromFile(tempFileName);

                //获取PrintDocument对象
                PrintDocument printDoc = doc.PrintDocument;

                //设置PrintController属性为StandardPrintController，用于隐藏打印进程
                printDoc.PrintController = new StandardPrintController();

                //打印文档
                printDoc.Print();
            }
            catch (Exception ex)
            {
                DXMessageBox.Show(ex.Message);
            }
        }
        public ObservableCollection<SampleStateModule> SampleStates
        {
            get; set;
        } = [
            new SampleStateModule("固体"),
            new SampleStateModule("液体")
        ];


    }
    public class SampleStateModule : BindableBase
    {
        public SampleStateModule(string sampleState)
        {
            this.SampleState = sampleState;
        }
        public string SampleState
        {
            get;
        }

        private int methodsCount;
        public int MethodsCount
        {
            get
            {
                return methodsCount;
            }
            set
            {
                methodsCount = value;
                RaisePropertyChanged(nameof(MethodsCount));
            }
        }

        private int productsCount;
        public int ProductsCount
        {
            get
            {
                return productsCount;
            }
            set
            {
                productsCount = value;
                RaisePropertyChanged(nameof(ProductsCount));
            }
        }


    }

    public class Printer
    {
        public string? PrinterName
        {
            get; set;
        }
    }
    public class StandardFileItem
    {
        public string? FilePath { get; set; }
        public string? FileName { get { return FilePath?.Split('\\').Last(); } }
        public bool IsSelected { get; set; }
    }


}
