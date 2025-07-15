using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.Xpf;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Printing;
using DevExpress.XtraPrinting;
using Lims.Common.Dtos;
using Lims.Common.Parameters;
using Lims.ToolsForClient;
using Lims.WPF.Resources;
using Lims.WPF.Views.Dialogs;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace Lims.WPF.ViewModels
{
    public abstract class SampleAndItemDataViewModelBase : DocumentViewModelBase
    {
        /// <summary>
        /// 记录界面切换中项目高度
        /// </summary>
        private string itemViewHeight = "1.618*";
        public string ItemViewHeight
        {
            get
            {
                return itemViewHeight;
            }
            set
            {
                itemViewHeight = value;
                RaisePropertyChanged(nameof(ItemViewHeight));
            }
        }

        /// <summary>
        /// 记录界面切换中项目宽度
        /// </summary>
        private string sampleViewWidth = "2.8*";
        public string SampleViewWidth
        {
            get
            {
                return sampleViewWidth;
            }
            set
            {
                sampleViewWidth = value;
                RaisePropertyChanged(nameof(SampleViewWidth));
            }
        }

        private string CurrentHeight;
        private string CurrentWidth;

        private bool isFullTaskListView;

        public bool IsFullTaskListView
        {
            get
            {
                return isFullTaskListView;
            }
            set
            {
                isFullTaskListView = value;
                RaisePropertyChanged(nameof(IsFullTaskListView));
                if (value)
                {
                    CurrentHeight = ItemViewHeight;
                    ItemViewHeight = "0";
                    CurrentWidth = SampleViewWidth;
                    SampleViewWidth = "0";
                    cfa.Save();
                }
                else
                {
                    SampleViewWidth = CurrentWidth;
                    ItemViewHeight = CurrentHeight;
                }
            }
        }
        public SampleAndItemDataViewModelBase()
        {
        }

        protected override async void OnInitializeInRuntime()
        {
            base.OnInitializeInRuntime();
            RelativeProgress = GetRelativeProgress();

            await LoadMainDatas(CurrentUser);
        }




       
        protected virtual async Task<ObservableCollection<ItemDto>?> GetAllItemsOfSample(SampleDto sample)
        {
            return sample == null ? new ObservableCollection<ItemDto>() : (await _itemService.GetAllItemsBySampleCodeAsync(new ItemFilterParam(sample.SampleCode))).Result?.OrderBy(i => i.ItemId).ToObservableCollection();
        }

        protected async Task ExcuteIfNullSample(SampleDto sample, IEnumerable<ItemDto> currentItems)
        {
            if (sample != null)
            {
                var items = await GetAllItemsOfSample(sample);
                if (currentItems.Count() > 0)
                {
                    sample.Items = items;
                    FocusedSample = sample;
                }
                else
                {
                    pre_MyFocusedSampelRowIndex = FocusedSampleRowHandle;
                    SamplesSource?.Remove(sample);
                    FocusedSampleRowHandle = pre_MyFocusedSampelRowIndex;
                }
                await _sampleService.UpdateAsync(sample);
            }
        }
        public virtual Task RefreshItemDatas(SampleDto sample)
        {
            return Task.CompletedTask;
        }

        protected virtual async Task<ObservableCollection<ItemDto>> GetItemsSource(SampleDto sample)
        {
            return (await _itemService.GetAllItemsBySampleCodeAsync(new Common.Parameters.ItemFilterParam(sample.SampleCode))).Result.OrderBy(i => i.ItemId).ToObservableCollection();
        }
        protected abstract override Task LoadMainDatas(UserDto? user);


        public List<ItemDto> ReportDataSource
        {
            get; set;
        }
        [Command]
        public void DocumentKeyUp(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F12:
                    IsFullTaskListView = !IsFullTaskListView;
                    break;
            }
        }
        [Command]
        public virtual void ItemRowDoubleClick(RowClickArgs args)
        {

            if (args.Item is ItemDto item && item.SubItems != null && item.SubItems.Count > 0)
                ShowSubItemsView(item);
        }

        [Command]
        public virtual async void ShowAllItemsOfCurrentSample(SampleDto sample)
        {
            AllItemsOfFocusedSample = (await _itemService.GetAllItemsBySampleCodeAsync(new ItemFilterParam(sample.SampleCode))).Result.ToObservableCollection();
            DevExpress.Mvvm.IDialogService dialogService = GetService<DevExpress.Mvvm.IDialogService>("AllItemsOfSamplePreviewDialogService");
            dialogService.ShowDialog(
                new List<UICommand> {
                        new UICommand{Caption = "关闭",IsDefault = false,IsCancel = true,}
                }
                , "", this);
        }

        [Command]
        public async void CopySampleInfoToClipBorad(ObservableCollection<SampleDto> samples)
        {
            var table = new DataTable();
            table.Columns.Add(new DataColumn("样品编号"));
            table.Columns.Add(new DataColumn("样品名称"));
            foreach (var sample in samples)
            {
                var row = table.NewRow();
                row.SetField("样品编号", sample.SampleCode);
                row.SetField("样品名称", sample.SampleName);
                table.Rows.Add(row);
            }
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                stringBuilder.Append(table.Rows[i]["样品编号"].ToString());
                stringBuilder.Append("\t");
                stringBuilder.Append(table.Rows[i]["样品名称"].ToString());
                stringBuilder.Append("\r\n");
            }
            Clipboard.SetDataObject(stringBuilder.ToString());
            await ShowNotifaction("", "已将样品编号及样品名称拷贝到剪切板!", "");
        }




        public async void ReportDataPreview(SampleDto sample)
        {
            AllItemsOfFocusedSample = await GetAllItemsOfSample(sample); //TaskDatasSource.Where(i => i.SampleCode == sample.SampleCode).ToObservableCollection();
            if (AllItemsOfFocusedSample != null && AllItemsOfFocusedSample.Count > 0)
            {
                var items = new ItemDto[AllItemsOfFocusedSample.Count];
                AllItemsOfFocusedSample.CopyTo(items, 0);

                foreach (var item in items)
                {
                    try
                    {
                        item.TestMethod = item.TestMethod.Split(' ')[0] + " " + item.TestMethod.Split(' ')[1];
                    }
                    catch (Exception)
                    {

                    }
                    try
                    {
                        item.ExecuteStandard = item.ExecuteStandard?.Split(' ')[0] + " " + item.ExecuteStandard?.Split(' ')[1];
                    }
                    catch (Exception)
                    {

                    }
                }
                ReportDataSource = items.ToList();


                var itemsWithSubItems = AllItemsOfFocusedSample.Where(i => i.SubItems != null && i.SubItems.Count > 0);

                foreach (var item in itemsWithSubItems)
                {
                    foreach (var subitem in item.SubItems)
                    {

                        ReportDataSource.Add(new ItemDto(0, item.ReportUnit, subitem.TestItem, 0, subitem.IndexRequest)
                        {
                            SampleCode = item.SampleCode,

                            TestMethod = item.TestMethod,

                            ExecuteStandard = item.ExecuteStandard,

                            ItemId = subitem.SubItemId,
                            TestResult = subitem.TestResult,
                        });
                    }
                }

                ReportDataSource = ReportDataSource.OrderBy(i => i.ItemId).ToList();
                ReportDataPreview reportDataPreview = new ReportDataPreview(ReportDataSource);
                reportDataPreview.Show();
            }
        }

        [Command]
        public virtual void ShowSubItemsView(ItemDto item)
        {
            SubItemViewColumns = GetSubItemViewColumns(item);
            EdittingItem = item;
            DevExpress.Mvvm.IDialogService dialogService = GetService<DevExpress.Mvvm.IDialogService>("SubItemsViewDialogService");
            dialogService.ShowDialog(
                new List<UICommand> {
                        new UICommand{Caption = "关闭",IsDefault = false,IsCancel = true,}
                }
                , "", this);
        }

        public List<BindingColumn> SubItemViewColumns
        {
            get; set;
        }


        public SubItemDto FocusedSubItem
        {
            get; set;
        }
        [Command]
        public async void TestSubItemResultChanged(CellValueChangedArgs e)
        {
            await _subItemService.UpdateAsync(FocusedSubItem);
        }


        protected static List<BindingColumn> GetSubItemViewColumns(ItemDto item)
        {
            return new List<BindingColumn>
            {
                new BindingColumn(SettingsType.Binding,"TestItem", "检测项目"),
                //new BindingColumn(SettingsType.Binding,"ReportUnit","报告单位"),

                 new BindingColumn(SettingsType.Binding, "FirstTestResult", "平行一结果")
                    { ReadOnly = false, Visible = item.TestItem.Contains("氨基酸") },
                new BindingColumn(SettingsType.Binding, "SecondTestResult", "平行二结果")
                    { ReadOnly = false, Visible = item.TestItem.Contains("氨基酸") },
                new BindingColumn(SettingsType.Binding, "AverageTestResult", "平均值")
                    { ReadOnly = false, Visible = item.TestItem.Contains("氨基酸") },


                new BindingColumn(SettingsType.Binding,"TestResult", "检测结果"),
                new BindingColumn(SettingsType.Binding,"IndexRequest", "指标要求"),
                new BindingColumn(SettingsType.Binding,"ItemRemark", "项目备注") ,
            };
        }

        private ItemDto edittingItem;

        public ItemDto EdittingItem
        {
            get => edittingItem;
            set
            {
                edittingItem = value;
                RaisePropertyChanged(nameof(EdittingItem));
            }
        }

        [Command]
        public void SearchStringToFilterCriteria(SearchStringToFilterCriteriaEventArgs e)
        {
            e.Filter = DevExpress.Data.Filtering.CriteriaOperator.Parse(string.Format("Contains([Name], '{0}')", e.SearchString));
        }

        [Command]
        public void SearchByCurrentCellValue(TableView tv)
        {
            if (tv != null && tv.Grid.CurrentCellValue != null)
                tv.SearchString = tv.Grid.CurrentCellValue.ToString();
        }

        protected ObservableCollection<ItemDto> selectedTaskDatas = new();
        public ObservableCollection<ItemDto> SelectedTaskDatas
        {
            get => selectedTaskDatas;
            set
            {
                selectedTaskDatas = value;
                RaisePropertyChanged(nameof(SelectedTaskDatas));
            }
        }
        protected ObservableCollection<SampleDto> selectedSamples = new();
        public ObservableCollection<SampleDto> SelectedSamples
        {
            get => selectedSamples;
            set
            {
                selectedSamples = value;
                RaisePropertyChanged(nameof(SelectedSamples));
            }
        }

        private bool showItemGridLoadingPanel;

        public bool ShowItemGridLoadingPanel
        {
            get => showItemGridLoadingPanel;
            set
            {
                showItemGridLoadingPanel = value;
                RaisePropertyChanged(nameof(ShowItemGridLoadingPanel));
            }
        }

        //public ObservableCollection<SampleDto> SelectedSamples { get; set; } = new();
        /// <summary>
        /// 选中样品下所有项目
        /// </summary>
        private ObservableCollection<ItemDto> itemsOfFocusedSample;

        public ObservableCollection<ItemDto> AllItemsOfFocusedSample
        {
            get => itemsOfFocusedSample;
            set
            {
                itemsOfFocusedSample = value;
                RaisePropertyChanged();
            }
        }

        private SampleDto focusedSample;

        /// <summary>
        /// 选中的样品项
        /// </summary>
        public SampleDto FocusedSample
        {
            get => focusedSample;
            set
            {
                if (value != null && FocusedSample != value)
                    ShowItemsOfFocusedSample(value);
                focusedSample = value;
                CurrentSample = value;
                RaisePropertyChanged(nameof(FocusedSample));
            }
        }
        protected virtual async void ShowItemsOfFocusedSample(SampleDto sample)
        {
            try
            {
                ShowItemGridLoadingPanel = true;
                ItemsSource = await GetItemsSource(sample);
                ShowItemGridLoadingPanel = false;
            }
            catch (Exception)
            {

                //throw;
            }
        }


        protected ItemDto focusedItem;

        /// <summary>
        /// 选中的项目
        /// </summary>
        public ItemDto FocusedItem
        {
            get => focusedItem;
            set
            {
                focusedItem = value;

                RaisePropertyChanged(nameof(FocusedItem));
            }
        }

        private ItemDto focusedTaskData;

        public ItemDto FocusedTaskData
        {
            get => focusedTaskData;
            set
            {
                if (value != null && focusedTaskData != value)
                {
                    ShowSampleOfFocusedTaskListItem(value);
                    CurrentSample = value.Sample;
                }
                ;
                focusedTaskData = value;
            }
        }

        protected virtual void ShowSampleOfFocusedTaskListItem(ItemDto item)
        {
            try
            {
                FocusedSample = SamplesSource.FirstOrDefault(s => s.SampleCode == item.SampleCode);
            }
            catch (Exception)
            {

                //throw;
            }
        }


        protected ObservableCollection<ItemDto> taskListPreviewSources = new();

        public ObservableCollection<ItemDto> TaskListPreviewSources
        {
            get => taskListPreviewSources;
            set => taskListPreviewSources = value;
        }

        private int myFocusedItemRowHandle;

        public int MyFocusedItemRowHandle
        {
            get => myFocusedItemRowHandle;
            set
            {
                myFocusedItemRowHandle = value;
                RaisePropertyChanged(nameof(MyFocusedItemRowHandle));
            }
        }

        private int allFocusedItemRowHandle;

        public int AllFocusedItemRowHandle
        {
            get => allFocusedItemRowHandle;
            set
            {
                allFocusedItemRowHandle = value;
                RaisePropertyChanged(nameof(AllFocusedItemRowHandle));
            }
        }

        private string moistureContent;

        public string MoistureContent
        {
            get => moistureContent;
            set
            {
                moistureContent = value;
                RaisePropertyChanged(nameof(MoistureContent));
            }
        }

        private string density;

        public string Density
        {
            get => density;
            set
            {
                density = value;
                RaisePropertyChanged(nameof(Density));
            }
        }

        private DateTime sample_BeginDate = DateTime.Today.AddMonths(-2);

        public virtual DateTime Sample_BeginDate
        {
            get => sample_BeginDate;
            set
            {
                sample_BeginDate = value;
                LoadMainDatas(CurrentUser);
            }
        }

        private DateTime sample_EndDate = DateTime.Today.AddDays(7);

        public DateTime Sample_EndDate
        {
            get => sample_EndDate;
            set
            {
                sample_EndDate = value;
                LoadMainDatas(CurrentUser);
            }
        }

        public abstract TestProgress GetRelativeProgress();

        public string PrintHeader
        {
            get; set;
        }

        /// <summary>
        /// 打印表格
        /// </summary>
        [Command]
        public void PrintTableView(DataViewBase tv)
        {
            if ((tv.Parent as GridDataControlBase)?.ItemsSource != null)
            {
                PrintHeader = $"{DateTimeOffset.Now:yyyy/MM/dd HH:mm:ss} ( {CurrentUser.UserName} )";
                PrintableControlLink link = new(tv)
                {
                    PageHeaderData = this,
                    PageHeaderTemplate = (DataTemplate)Application.Current.Resources["DetailPrintHeaderTemplate"],
                    PageFooterTemplate = (DataTemplate)Application.Current.Resources["DetailPrintFooterTemplate"],
                    PaperKind = (DevExpress.Drawing.Printing.DXPaperKind)System.Drawing.Printing.PaperKind.A4,
                    VerticalContentSplitting = VerticalContentSplitting.Smart,
                    Landscape = true,
                    Margins = new System.Drawing.Printing.Margins(0, 0, 0, 0),
                };
                link.ShowPrintPreview(tv);
            }
        }

        public TestProgress RelativeProgress
        {
            get;
            set;
        }

        protected int pre_MyFocusedSampelRowIndex;

        /// <summary>
        /// 选中样品rowhandle
        /// </summary>
        private int focusedSampleRowHandle;

        public int FocusedSampleRowHandle
        {
            get => focusedSampleRowHandle;
            set
            {
                focusedSampleRowHandle = value;
                RaisePropertyChanged(nameof(FocusedSampleRowHandle));
            }
        }

        protected async void ReturnTask(ItemDto edittingItem, SampleDto editingSample, string message, int preTestProgress = (int)TestProgress.无)
        {
            //int preTestProgress = edittingItem.TestProgress;

            var sample = edittingItem.Sample;
            sample.CompleteTime = null;
            await _sampleService.UpdateAsync(sample);

            edittingItem.PreTestProgress = preTestProgress;
            edittingItem.TestProgress = (int)TestProgress.任务已退回;
            edittingItem.TestResult = null;
            edittingItem.Temp_TestResult = $"(任务已退回){edittingItem.Temp_TestResult}";
            edittingItem.SingleConclusion = "/";
            edittingItem.Temp_SingleConclusion = "/";
            await _itemService.UpdateAsync(edittingItem);
            ItemsSource = await GetItemsSource(editingSample);

            if (edittingItem.SubItems != null && edittingItem.SubItems.Count > 0)
            {
                foreach (var subItem in edittingItem.SubItems)
                    subItem.TestResult = string.Empty;
                await _subItemService.UpdateRangeAsync(edittingItem.SubItems.ToList());
            }

            //await UpdateSampleTestProgress(editingSample);
            var log = new LoggerDto(DateTimeOffset.Now)
            {
                LogLevel = LogLevel.WARN,
                ActionType = ActionType.退回任务,
                PublisherIP = LoggerDto.GetLocalIP(),
                PublisherName = CurrentUser.UserName,
                ReceiverName = edittingItem.Tester,
                SampleCode = editingSample.SampleCode,
                TestItem = edittingItem.TestItem,
                Message = message
            };
            await _loggerService.CreateAsync(log);
        }

        #region 定制工具    
        [Command]
#pragma warning disable CA1041 // 提供 ObsoleteAttribute 消息
        [Obsolete]
#pragma warning restore CA1041 // 提供 ObsoleteAttribute 消息
        public async Task 生成微生物下单表()
        {
            System.Windows.Forms.SaveFileDialog saveFileDialog = new()
            {
                Filter = " Excel files(*.xlsx)|*.xlsx|All files(*.*)|*.*",
                RestoreDirectory = true,
                FileName = $"{CurrentUser.UserName}_下单表_{DateTimeOffset.Now:yyyyMMdd_HHmmss}",
            };

            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                List<ItemDto> allItems = new();
                foreach (var item in selectedTaskDatas)
                {
                    allItems.Add(item);
                    if (item.SubItems != null)
                    {
                        foreach (var subItem in item.SubItems)
                        {
                            allItems.Add(new ItemDto(0, item.ReportUnit, subItem.TestItem, 0, subItem.IndexRequest)
                            {
                                SampleCode = item.SampleCode,
                                Sample = item.Sample,
                                ItemRemark = subItem.ItemRemark,
                            });
                        }
                    }
                }
                await ExportTo微生物下单表(allItems, saveFileDialog.FileName);
            }
        }

        [Command]
        public async void 生成元素下单表()
        {
            System.Windows.Forms.SaveFileDialog saveFileDialog = new()
            {
                Filter = " Excel files(*.xlsx)|*.xlsx|All files(*.*)|*.*",
                RestoreDirectory = true,
                FileName = $"{CurrentUser.UserName}_下单表_{DateTimeOffset.Now:yyyyMMdd_HHmmss}",
            };

            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                List<ItemDto> allItems = new();
                foreach (var item in selectedTaskDatas)
                {
                    allItems.Add(item);
                    if (item.SubItems != null)
                    {
                        foreach (var subItem in item.SubItems)
                        {
                            allItems.Add(new ItemDto(0, item.ReportUnit, subItem.TestItem, 0, subItem.IndexRequest)
                            {
                                SampleCode = item.SampleCode,
                                Sample = item.Sample,
                                ItemRemark = subItem.ItemRemark,
                            });
                        }
                    }
                }
                await ExportTo元素下单表(allItems, saveFileDialog.FileName);
            }
        }

        [Command]
#pragma warning disable CA1041 // 提供 ObsoleteAttribute 消息
        [Obsolete]
#pragma warning restore CA1041 // 提供 ObsoleteAttribute 消息
        public async Task 生成微生物结果统计表()
        {
            System.Windows.Forms.SaveFileDialog saveFileDialog = new()
            {
                Filter = " Excel files(*.xlsx)|*.xlsx|All files(*.*)|*.*",
                RestoreDirectory = true,
                FileName = $"{CurrentUser.UserName}_结果统计表_{DateTimeOffset.Now:yyyyMMdd_HHmmss}",
            };

            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                List<ItemDto> allItems = new();
                foreach (var item in selectedTaskDatas)
                {
                    allItems.Add(item);
                    if (item.SubItems != null)
                    {
                        foreach (var subItem in item.SubItems)
                        {
                            allItems.Add(new ItemDto(0, subItem.IndexRequest, subItem.TestItem, 0, item.ReportUnit)
                            {
                                SampleCode = item.SampleCode,
                                Sample = item.Sample,
                                ItemRemark = subItem.ItemRemark,
                            });
                        }
                    }
                }
                await System.Threading.Tasks.Task.Run(async () => { await Export微生物结果统计表(allItems, saveFileDialog.FileName); });
            }
        }


        /// <summary>
        ///
        /// </summary>
        /// <param name="sources">数据源</param>
        /// <param name="filepath">保存文件路径</param>
#pragma warning disable CA1041 // 提供 ObsoleteAttribute 消息
        [Obsolete]
#pragma warning restore CA1041 // 提供 ObsoleteAttribute 消息
        protected async Task ExportTo微生物下单表(List<ItemDto> sources, string filepath)
        {
            List<微生物下单表Class> list = new();
            Type t = typeof(微生物下单表Class);
            string?[] sampleCodes = selectedTaskDatas.Select(s => s.SampleCode).Distinct().OrderByDescending(s => s).ToArray();

            foreach (string? sampleCode in sampleCodes)
            {
                StringBuilder otherItemsStr = new();
                //string sampleName;
                IEnumerable<ItemDto> items = sources.Where(s => s.SampleCode == sampleCode);

                foreach (ItemDto item in items)
                {
                    if (t.GetProperties().All(p => p.Name != item.TestItem))
                    {
                        otherItemsStr.Append(String.IsNullOrEmpty(item.IndexRequest) ? item.TestItem + ',' : item.TestItem + "(" + item.IndexRequest + ")" + ',');
                    }
                }
                微生物下单表Class helper = new()
                {
                    样品编号 = sampleCode,
                    样品名称 = items.FirstOrDefault(i => i.SampleCode.Equals(sampleCode)).Sample.SampleName,
                    枯草芽孢杆菌数 = items.Any(i => i.TestItem == "枯草芽孢杆菌数") ? String.IsNullOrEmpty(items.FirstOrDefault(i => i.TestItem == "枯草芽孢杆菌数").ItemRemark) ? "√" : "√" + "(" + items.FirstOrDefault(i => i.TestItem == "枯草芽孢杆菌数").ItemRemark + ")" : "",

                    地衣芽孢杆菌数 = items.Any(i => i.TestItem == "地衣芽孢杆菌数") ? String.IsNullOrEmpty(items.FirstOrDefault(i => i.TestItem == "地衣芽孢杆菌数").ItemRemark) ? "√" : "√" + "(" + items.FirstOrDefault(i => i.TestItem == "地衣芽孢杆菌数").ItemRemark + ")" : "",

                    巨大芽孢杆菌数 = items.Any(i => i.TestItem == "巨大芽孢杆菌数") ? String.IsNullOrEmpty(items.FirstOrDefault(i => i.TestItem == "巨大芽孢杆菌数").ItemRemark) ? "√" : "√" + "(" + items.FirstOrDefault(i => i.TestItem == "巨大芽孢杆菌数").ItemRemark + ")" : "",

                    杂菌率 = items.Any(i => i.TestItem == "杂菌率") ? String.IsNullOrEmpty(items.FirstOrDefault(i => i.TestItem == "杂菌率").ItemRemark) ? "√" : "√" + "(" + items.FirstOrDefault(i => i.TestItem == "杂菌率").ItemRemark + ")" : "",

                    粪大肠菌群数 = items.Any(i => i.TestItem == "粪大肠菌群数") ? String.IsNullOrEmpty(items.FirstOrDefault(i => i.TestItem == "粪大肠菌群数").ItemRemark) ? "√" : "√" + "(" + items.FirstOrDefault(i => i.TestItem == "粪大肠菌群数").ItemRemark + ")" : "",

                    胶冻样类芽孢杆菌数 = items.Any(i => i.TestItem == "胶冻样类芽孢杆菌数") ? String.IsNullOrEmpty(items.FirstOrDefault(i => i.TestItem == "胶冻样类芽孢杆菌数").ItemRemark) ? "√" : "√" + "(" + items.FirstOrDefault(i => i.TestItem == "胶冻样类芽孢杆菌数").ItemRemark + ")" : "",

                    蛔虫卵死亡率 = items.Any(i => i.TestItem == "蛔虫卵死亡率") ? String.IsNullOrEmpty(items.FirstOrDefault(i => i.TestItem == "蛔虫卵死亡率").ItemRemark) ? "√" : "√" + "(" + items.FirstOrDefault(i => i.TestItem == "蛔虫卵死亡率").ItemRemark + ")" : "",

                    解淀粉芽孢杆菌数 = items.Any(i => i.TestItem == "解淀粉芽孢杆菌数") ? String.IsNullOrEmpty(items.FirstOrDefault(i => i.TestItem == "解淀粉芽孢杆菌数").ItemRemark) ? "√" : "√" + "(" + items.FirstOrDefault(i => i.TestItem == "解淀粉芽孢杆菌数").ItemRemark + ")" : "",

                    酿酒酵母菌数 = items.Any(i => i.TestItem == "酿酒酵母菌数") ? String.IsNullOrEmpty(items.FirstOrDefault(i => i.TestItem == "酿酒酵母菌数").ItemRemark) ? "√" : "√" + "(" + items.FirstOrDefault(i => i.TestItem == "酿酒酵母菌数").ItemRemark + ")" : "",

                    霉菌杂菌数 = items.Any(i => i.TestItem == "霉菌杂菌数") ? String.IsNullOrEmpty(items.FirstOrDefault(i => i.TestItem == "霉菌杂菌数").ItemRemark) ? "√" : "√" + "(" + items.FirstOrDefault(i => i.TestItem == "霉菌杂菌数").ItemRemark + ")" : "",

                    //黑曲霉 = items.Any(i => i.TestItem == "黑曲霉") ? items.FirstOrDefault(i => i.TestItem == "黑曲霉").ItemRemark == null ? "√" : "√" + "(" + items.FirstOrDefault(i => i.TestItem == "黑曲霉").ItemRemark + ")" : "",

                    执行标准 = string.IsNullOrEmpty(items.FirstOrDefault().ExecuteStandard) ? "" : items.FirstOrDefault().ExecuteStandard.Split(' ')[1].Split("-")[0],

                    其他 = otherItemsStr.ToString(),
                };

                SampleDto sample = (await _sampleService.GetSingleAsync(sampleCode)).Result;
                helper.样品状态 = sample.SampleState;
                helper.业务类型 = sample.TaskType;
                helper.是否加急 = sample.IsUrgent ? "√" : "";
                helper.样品备注 = sample.SampleRemark;
                list.Add(helper);
            }
            //ExcelExportHelper excelExportHelper = new ExcelExportHelper();
            ExcelExportHelper.ExportListToExcelXlsx(list, $"{DateTimeOffset.Now} {CurrentUser.UserName} 下单表", filepath);

            ProcessStartInfo processStartInfo = new(filepath);
            Process process = new()
            {
                StartInfo = processStartInfo
            };
            process.StartInfo.UseShellExecute = true;
            process.Start();
        }
        public class 微生物下单表Class
        {
            public string? 样品编号
            {
                get; set;
            }
            public string 样品名称
            {
                get; set;
            }
            //public string 项目备注
            //{
            //    get; set;
            //}
            public string 枯草芽孢杆菌数
            {
                get; set;
            }

            public string 地衣芽孢杆菌数
            {
                get; set;
            }

            public string 解淀粉芽孢杆菌数
            {
                get; set;
            }

            public string 巨大芽孢杆菌数
            {
                get; set;
            }

            public string 胶冻样类芽孢杆菌数
            {
                get; set;
            }

            public string 酿酒酵母菌数
            {
                get; set;
            }

            //public string 黑曲霉
            //{
            //    get; set;
            //}

            public string 霉菌杂菌数
            {
                get; set;
            }

            public string 杂菌率
            {
                get; set;
            }

            public string 粪大肠菌群数
            {
                get; set;
            }

            public string 蛔虫卵死亡率
            {
                get; set;
            }

            public string 其他
            {
                get; set;
            }

            public string 样品状态
            {
                get; set;
            }

            public string 是否加急
            {
                get; set;
            }

            public string 业务类型
            {
                get; set;
            }

            public string 样品备注
            {
                get; set;
            }

            public string 执行标准
            {
                get; set;
            }
        }

        public class 元素下单表Class
        {
            public string? 样品编号
            {
                get; set;
            }
            public string 样品名称
            {
                get; set;
            }
            public string 取样量
            {
                get; set;
            }
            //public string 项目备注
            //{
            //    get; set;
            //}
            public string K
            {
                get; set;
            }

            public string Ca
            {
                get; set;
            }

            public string Mg
            {
                get; set;
            }

            public string Si
            {
                get; set;
            }

            public string B
            {
                get; set;
            }

            public string Mo
            {
                get; set;
            }

            public string S
            {
                get; set;
            }

            public string Cu
            {
                get; set;
            }

            public string Fe
            {
                get; set;
            }

            public string Mn
            {
                get; set;
            }

            public string Zn
            {
                get; set;
            }

            public string Na
            {
                get; set;
            }
            public string Al
            {
                get; set;
            }

            public string Pb
            {
                get; set;
            }

            public string Cd
            {
                get; set;
            }
            public string Cr
            {
                get; set;
            }



            public string 其他
            {
                get; set;
            }

            public string 是否加急
            {
                get; set;
            }

            public string 业务类型
            {
                get; set;
            }



            public string 执行标准
            {
                get; set;
            }
        }

#pragma warning disable CA1041 // 提供 ObsoleteAttribute 消息
        [Obsolete]
#pragma warning restore CA1041 // 提供 ObsoleteAttribute 消息
        protected async Task ExportTo元素下单表(List<ItemDto> sources, string filepath)
        {
            List<元素下单表Class> list = new();
            Type t = typeof(元素下单表Class);
            string?[] sampleCodes = selectedTaskDatas.Select(s => s.SampleCode).Distinct().OrderBy(s => s).ToArray();

            foreach (string? sampleCode in sampleCodes)
            {
                StringBuilder otherItemsStr = new();
                //string sampleName;
                IEnumerable<ItemDto> items = sources.Where(s => s.SampleCode == sampleCode);

                foreach (ItemDto item in items)
                {
                    if (t.GetProperties().All(p => p.Name != item.MethodStandard.KeyItem))
                    {
                        otherItemsStr.Append(item.IndexRequest == null ? item.TestItem + ',' : item.TestItem + "(" + item.IndexRequest + ")" + ',');
                    }
                }
                元素下单表Class helper = new 元素下单表Class();
                try
                {
                    helper = new()
                    {
                        样品编号 = sampleCode,
                        样品名称 = items.FirstOrDefault(i => i.SampleCode.Equals(sampleCode)).Sample.SampleName,

                        K = items.Any(i => i.MethodStandard.KeyItem == "K") ? items.FirstOrDefault(i => i.MethodStandard.KeyItem == "K").TestItem + "-" + items.FirstOrDefault(i => i.MethodStandard.KeyItem == "K").TestMethod.Split(' ')[1].Split("-")[0] : "",

                        Ca = items.Any(i => i.MethodStandard.KeyItem == "Ca") ? items.FirstOrDefault(i => i.MethodStandard.KeyItem == "Ca").TestItem + "-" + items.FirstOrDefault(i => i.MethodStandard.KeyItem == "Ca").TestMethod.Split(' ')[1].Split("-")[0] : "",

                        Mg = items.Any(i => i.MethodStandard.KeyItem == "Mg") ? items.FirstOrDefault(i => i.MethodStandard.KeyItem == "Mg").TestItem + "-" + items.FirstOrDefault(i => i.MethodStandard.KeyItem == "Mg").TestMethod.Split(' ')[1].Split("-")[0] : "",

                        B = items.Any(i => i.MethodStandard.KeyItem == "B") ? items.FirstOrDefault(i => i.MethodStandard.KeyItem == "B").TestItem + "-" + items.FirstOrDefault(i => i.MethodStandard.KeyItem == "B").TestMethod.Split(' ')[1].Split("-")[0] : "",

                        Mo = items.Any(i => i.MethodStandard.KeyItem == "Mo") ? items.FirstOrDefault(i => i.MethodStandard.KeyItem == "Mo").TestItem + "-" + items.FirstOrDefault(i => i.MethodStandard.KeyItem == "Mo").TestMethod.Split(' ')[1].Split("-")[0] : "",

                        S = items.Any(i => i.MethodStandard.KeyItem == "S") ? items.FirstOrDefault(i => i.MethodStandard.KeyItem == "S").TestItem + "-" + items.FirstOrDefault(i => i.MethodStandard.KeyItem == "S").TestMethod.Split(' ')[1].Split("-")[0] : "",

                        Cu = items.Any(i => i.MethodStandard.KeyItem == "Cu") ? items.FirstOrDefault(i => i.MethodStandard.KeyItem == "Cu").TestItem + "-" + items.FirstOrDefault(i => i.MethodStandard.KeyItem == "Cu").TestMethod.Split(' ')[1].Split("-")[0] : "",

                        Fe = items.Any(i => i.MethodStandard.KeyItem == "Fe") ? items.FirstOrDefault(i => i.MethodStandard.KeyItem == "Fe").TestItem + "-" + items.FirstOrDefault(i => i.MethodStandard.KeyItem == "Fe").TestMethod.Split(' ')[1].Split("-")[0] : "",

                        Mn = items.Any(i => i.MethodStandard.KeyItem == "Mn") ? items.FirstOrDefault(i => i.MethodStandard.KeyItem == "Mn").TestItem + "-" + items.FirstOrDefault(i => i.MethodStandard.KeyItem == "Mn").TestMethod.Split(' ')[1].Split("-")[0] : "",

                        Zn = items.Any(i => i.MethodStandard.KeyItem == "Zn") ? items.FirstOrDefault(i => i.MethodStandard.KeyItem == "Zn").TestItem + "-" + items.FirstOrDefault(i => i.MethodStandard.KeyItem == "Zn").TestMethod.Split(' ')[1].Split("-")[0] : "",

                        Na = items.Any(i => i.MethodStandard.KeyItem == "Na") ? items.FirstOrDefault(i => i.MethodStandard.KeyItem == "Na").TestItem + "-" + items.FirstOrDefault(i => i.MethodStandard.KeyItem == "Na").TestMethod.Split(' ')[1].Split("-")[0] : "",

                        Pb = items.Any(i => i.MethodStandard.KeyItem == "Pb") ? items.FirstOrDefault(i => i.MethodStandard.KeyItem == "Pb").TestItem + "-" + items.FirstOrDefault(i => i.MethodStandard.KeyItem == "Pb").TestMethod.Split(' ')[1].Split("-")[0] : "",

                        Cd = items.Any(i => i.MethodStandard.KeyItem == "Cd") ? items.FirstOrDefault(i => i.MethodStandard.KeyItem == "Cd").TestItem + "-" + items.FirstOrDefault(i => i.MethodStandard.KeyItem == "Cd").TestMethod.Split(' ')[1].Split("-")[0] : "",

                        Cr = items.Any(i => i.MethodStandard.KeyItem == "Cr") ? items.FirstOrDefault(i => i.MethodStandard.KeyItem == "Cr").TestItem + "-" + items.FirstOrDefault(i => i.MethodStandard.KeyItem == "Cr").TestMethod.Split(' ')[1].Split("-")[0] : "",

                        Si = items.Any(i => i.MethodStandard.KeyItem == "Si") ? items.FirstOrDefault(i => i.MethodStandard.KeyItem == "Si").TestItem + "-" + items.FirstOrDefault(i => i.MethodStandard.KeyItem == "Si").TestMethod.Split(' ')[1].Split("-")[0] : "",

                        Al = items.Any(i => i.MethodStandard.KeyItem == "Al") ? items.FirstOrDefault(i => i.MethodStandard.KeyItem == "Al").TestItem + "-" + items.FirstOrDefault(i => i.MethodStandard.KeyItem == "Al").TestMethod.Split(' ')[1].Split("-")[0] : "",

                        执行标准 = string.IsNullOrEmpty(items.FirstOrDefault().ExecuteStandard) ? "" : items.FirstOrDefault().ExecuteStandard.Split(' ')[1].Split("-")[0],

                        其他 = otherItemsStr.ToString(),
                    };
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }


                SampleDto sample = (await _sampleService.GetSingleAsync(sampleCode)).Result;
                helper.业务类型 = sample.TaskType;
                helper.是否加急 = sample.IsUrgent || sample.CurrentUrgent ? "√" : "";
                //helper.样品备注 = sample.SampleRemark;
                list.Add(helper);
            }
            //ExcelExportHelper excelExportHelper = new ExcelExportHelper();
            ExcelExportHelper.ExportListToExcelXlsx(list, $"{DateTimeOffset.Now} {CurrentUser.UserName} 下单表", filepath);

            ProcessStartInfo processStartInfo = new(filepath);
            Process process = new()
            {
                StartInfo = processStartInfo
            };
            process.StartInfo.UseShellExecute = true;
            process.Start();
        }

        /// <summary>
        /// 微生物菌数统计
        /// </summary>
#pragma warning disable CA1041 // 提供 ObsoleteAttribute 消息
        [Obsolete]
#pragma warning restore CA1041 // 提供 ObsoleteAttribute 消息
        protected Task Export微生物结果统计表(List<ItemDto> sources, string filepath)
        {
            List<微生物结果统计Class> list = new();
            Type t = typeof(微生物结果统计Class);
            //string[] sampleCodes = selectedTaskDatas.Select(s => s.SampleCode).Distinct().OrderByDescending(s => s).ToArray();
            IOrderedEnumerable<SampleDto> samples = selectedTaskDatas.Select(i => i.Sample).DistinctBy(i => i.SampleCode).OrderByDescending(s => s.SampleCode);

            foreach (SampleDto sample in samples)
            {
                List<ItemDto> items = sources.Where(s => s.SampleCode == sample.SampleCode).ToList();
                foreach (ItemDto item in items)
                {
                    微生物结果统计Class helper = new()
                    {
                        样品编号 = sample.SampleCode,
                        样品名称 = sample.SampleName,
                        业务类型 = sample.TaskType,
                        执行标准 = item.ExecuteStandard,
                        指标要求 = item.IndexRequest,
                        是否加急 = sample.IsUrgent ? "加急" : "",
                        检测项目 = item.TestItem,
                    };
                    list.Add(helper);
                }
            }

            //ExcelExportHelper excelExportHelper = new ExcelExportHelper();

            ExcelExportHelper.ExportListToExcelXlsx(list, $"{DateTimeOffset.Now} {CurrentUser.UserName} 统计表", filepath);

            ProcessStartInfo processStartInfo = new(filepath);
            Process process = new()
            {
                StartInfo = processStartInfo
            };
            process.StartInfo.UseShellExecute = true;
            process.Start();
            return Task.CompletedTask;
        }
        public class 微生物结果统计Class
        {
            public string? 样品编号
            {
                get; set;
            }

            public string 样品名称
            {
                get; set;
            }

            public string 是否加急
            {
                get; set;
            }

            public string 业务类型
            {
                get; set;
            }

            public string 检测项目
            {
                get; set;
            }

            public string 指标要求
            {
                get; set;
            }

            public string 执行标准
            {
                get; set;
            }

            public string 数据结果
            {
                get; set;
            }

            public string 核对
            {
                get; set;
            }
        }
        #endregion


        #region 氨基酸临时工具
        public ObservableCollection<SubItemDto> SelectedEditableSubItems { get; set; } = new();
        [Command]
        public async void EditableSubItemsViwKeyUp(KeyEventArgs e)
        {

            if (e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Control))
            {
                if (SelectedEditableSubItems.Count > 0)
                {
                    var report = GetDataFrom_HITACHIAA_Report();
                    if (report == null)
                        return;
                    var strList = report.aaList;
                    switch (e.Key)
                    {
                        //粘贴至平行一
                        case Key.Q:
                            for (int i = 0; i < SelectedEditableSubItems.Count; i++)
                            {
                                if (i == strList.Count)
                                    return;
                                var subItem = SelectedEditableSubItems[i];
                                if (isJudgeLimitValue)
                                {
                                    if (!string.IsNullOrEmpty(strList[i]))
                                    {
                                        if (IsNumeric(strList[i].Trim()))
                                        {
                                            subItem.FirstTestResult =
                                                Convert.ToDecimal(strList[i].Trim()) <= Convert.ToDecimal(limitValue)
                                                    ? overrunExpress
                                                    : strList[i].Trim();
                                            await _subItemService.UpdateAsync(subItem);
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        subItem.FirstTestResult = overrunExpress;
                                        await _subItemService.UpdateAsync(subItem);
                                        continue;
                                    }
                                }

                                subItem.FirstTestResult = strList[i].Trim();
                                await _subItemService.UpdateAsync(subItem);
                            }

                            EdittingItem.FirstSampleWeight = report.SampleWeight;
                            await _itemService.UpdateAsync(EdittingItem);

                            break;
                        //粘贴至平行二
                        case Key.W:
                            for (int i = 0; i < SelectedEditableSubItems.Count; i++)
                            {
                                if (i == strList.Count)
                                    return;
                                var subItem = SelectedEditableSubItems[i];
                                if (isJudgeLimitValue)
                                {
                                    if (!string.IsNullOrEmpty(strList[i]))
                                    {
                                        if (IsNumeric(strList[i].Trim()))
                                        {
                                            subItem.SecondTestResult =
                                                Convert.ToDecimal(strList[i].Trim()) <= Convert.ToDecimal(limitValue)
                                                    ? overrunExpress
                                                    : strList[i].Trim();
                                            await _subItemService.UpdateAsync(subItem);
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        subItem.SecondTestResult = overrunExpress;
                                        await _subItemService.UpdateAsync(subItem);
                                        continue;
                                    }
                                }

                                subItem.SecondTestResult = strList[i].Trim();
                                await _subItemService.UpdateAsync(subItem);
                            }
                            EdittingItem.SecondSampleWeight = report.SampleWeight;
                            await _itemService.UpdateAsync(EdittingItem);
                            break;
                    }
                }
                else
                {
                    _messageBoxService.ShowMessage("请勾选需要操作的数据后操作！");
                }

            }

        }


        [Command]
        protected virtual async Task CaculateSubTestResultAverage(ItemDto item)
        {
            if (SelectedEditableSubItems.Count > 0)
            {
                foreach (var subItem in SelectedEditableSubItems)
                {
                    if (IsNumeric(subItem.FirstTestResult) && IsNumeric(subItem.SecondTestResult))
                    {
                        var res = new decimal[] { subItem.FirstTestResult.TryConvertToDecimal(), subItem.SecondTestResult.TryConvertToDecimal() };

                        var ave = Math.Round(res.Average(), 2, MidpointRounding.ToEven);

                        subItem.AverageTestResult = ave.ToString();

                        ItemDto DensityItem = (await _itemService.GetFirstItemBySampleCodeAndKeyItemAsync(new ItemFilterParam(item.SampleCode) { KeyItem = "密度" })).Result;


                        //已提交页面不修改Temp_TestResult
                        if (DensityItem != null)
                        {
                            var densityContent = DensityItem.TestResult;
                            var moi = densityContent.TryConvertToDecimal();
                            //subItem.Temp_TestResult = Math.Round(ave * 10 * moi, 1, MidpointRounding.ToEven).ToString();
                        }


                        await _subItemService.UpdateAsync(subItem);
                    }
                    else if (subItem.FirstTestResult == "未检出" && subItem.SecondTestResult == "未检出")
                    {

                        subItem.AverageTestResult = "未检出";
                        //subItem.Temp_TestResult = "未检出";

                        await _subItemService.UpdateAsync(subItem);
                    }
                    else
                    {
                        subItem.AverageTestResult = "";
                        //subItem.Temp_TestResult = "";

                        await _subItemService.UpdateAsync(subItem);

                    }
                }

            }
            else
            {
                _messageBoxService.ShowMessage("请勾选需要操作的数据后操作！");
            }
        }


        #region 单项判定

        /// <summary>
        /// 判定字符串是否为数字
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected static bool IsNumeric(string value)
        {
            return value != null ? double.TryParse(value.Trim(), out double result) : false;
        }

        private string Judgement(string? judge, string? value)
        {
            if (string.IsNullOrEmpty(judge) || string.IsNullOrEmpty(value))
                return "/";

            if (!string.IsNullOrEmpty(value) && IsNumeric(value))
            {
                if (Regex.IsMatch(judge, @"^[≤≥＜＞][+]?(0|([1-9]\d*))(\.\d+)?$"))
                {
                    judge = Regex.Replace(judge, @"^≤", "<=");
                    judge = Regex.Replace(judge, @"^≥", ">=");
                    judge = Regex.Replace(judge, @"^＜", "<");
                    judge = Regex.Replace(judge, @"^＞", ">");
                    DataTable dataTable = new();

                    return (bool)dataTable.Compute(value + judge, "") ? "符合" : "不符合";
                }
                else if (Regex.IsMatch(judge, @"^[+]?(0|([1-9]\d*))(\.\d+)?[~～-][+]?(0|([1-9]\d*))(\.\d+)?$"))
                {
                    decimal min = Convert.ToDecimal(Regex.Match(judge, @"^[+]?(0|([1-9]\d*))(\.\d+)?").Value);
                    decimal max = Convert.ToDecimal(Regex.Match(judge, @"[+]?(0|([1-9]\d*))(\.\d+)?$").Value);

                    return Convert.ToDecimal(value) >= min && Convert.ToDecimal(value) <= max ? "符合" : "不符合";
                }
            }

            return "/";
        }

        #endregion

        #region 数值修约

        /// <summary>
        /// 根据国标8170修约
        /// </summary>
        /// <param name="data_val"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        private static decimal round_gbt8170(decimal data_val, int len)
        {
            return Math.Round(data_val, len, MidpointRounding.ToEven);
        }


        /// <summary>
        /// 数值修约
        /// </summary>
        /// <param name="result"></param>
        /// <param name="rule"></param>
        /// <returns></returns>
        private static bool NumericalRevision(ref string result, string rule)
        {
            //NumberFormatInfo provider = new NumberFormatInfo();
            //provider.NumberDecimalSeparator = ",";
            //provider.NumberGroupSeparator = ".";
            //provider.NumberGroupSizes = [3];
            decimal inputResult = Convert.ToDecimal(result);
            bool IsNeed;
            string legalResult = string.Empty;
            if (Regex.IsMatch(rule, @"[Dd][Nn][0-9]"))
            {
                int num = Convert.ToInt32(Regex.Match(rule, @"[0-9]").Value);
                legalResult = Math.Round(inputResult, num, MidpointRounding.ToEven).ToString("F" + num);
            }
            else if (Regex.IsMatch(rule, "[Vv][Nn][0-9]"))
            {
                int num = Convert.ToInt32(Regex.Match(rule, @"[0-9]").Value);
                legalResult = ToScientific(inputResult, num).ToString();
            }

            IsNeed = result != legalResult;
            result = legalResult;
            return IsNeed;
        }

        /// <summary>
        /// 判断是否需要修约
        /// </summary>
        /// <param name="result"></param>
        /// <param name="rule"></param>
        /// <returns></returns>
        private static bool IsNeedNumericalRevision(ref string result, string? rule)
        {
            if (IsNumeric(result) && !string.IsNullOrEmpty(result) && !string.IsNullOrEmpty(rule))
            {
                double inputResult = Convert.ToDouble(result);
                if (Regex.IsMatch(rule, @"^[DdvV][Nn][0-9]$"))
                {
                    return NumericalRevision(ref result, rule);
                }
                else if (Regex.IsMatch(rule,
                             @"^[iI][fF]\([a-zA-Z][>,<,=][+]?(0|([1-9]\d*))(\.\d+)?,[dDvV][nN][0-9],[dDvV][nN][0-9]\)$"))
                {
                    string condition = Regex.Match(rule, @"[a-zA-Z][>,<,=][+]?(0|([1-9]\d*))(\.\d+)?").Value;
                    condition = Regex.Replace(condition, @"[a-zA-Z]", inputResult.ToString());
                    DataTable dt = new();

                    if ((bool)dt.Compute(condition, ""))
                    {
                        var rule1 = Regex.Match(rule, @",[dDvV][nN][0-9],").Value;

                        return NumericalRevision(ref result, rule1);
                    }
                    else
                    {
                        var rule2 = Regex.Match(rule, @",[dDvV][nN][0-9]\)$").Value;

                        return NumericalRevision(ref result, rule2);
                    }
                }
            }

            return false;
        }

        ///// <summary>
        ///// 科学计数法修约
        ///// </summary>
        ///// <param name="d"></param>
        ///// <param name="n"></param>
        ///// <returns></returns>
        private static string ToScientific(decimal value, int decimalDigits)
        {
            //string format = "E" + (decimalDigits-1);
            //return value.ToString(format, CultureInfo.InvariantCulture);

            int a;
            if (value == 0.0m)
                return "0";
            a = value > 1 || value < -1
                ? decimalDigits - (int)Math.Log10((double)Math.Abs(value)) - 1
                : decimalDigits + (int)Math.Log10((double)(1.0m / Math.Abs(value)));
            return a < 0
                ? string.Format("{0:E" + (decimalDigits - 1) + "}", value)
                : Math.Round(value, a, MidpointRounding.ToEven).ToString("F" + a);
        }

        #endregion

        #region 从剪切板

        private static bool GetDataFromClipboard(ref string[] strArr)
        {
            string str = Clipboard.GetText();
            if (!string.IsNullOrEmpty(str))
            {
                try
                {
                    var ary = Regex.Split(str.Trim(), @"\s{1,}");
                    ary.ForEach(x => x = x.Trim());
                    strArr = ary;
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// 从日立氨基酸分析仪报告获取数据
        /// </summary>
        /// <returns></returns>
        private HITACHIAA_Report GetDataFrom_HITACHIAA_Report()
        {
            List<string> strList;
            string sampleWeight = string.Empty;
            try
            {
                string str = Clipboard.GetText();

                strList = str.Split("\t\t\t\t\r\n\t\t")
                    .ToList()
                    .GetRange(4, 17)
                    .Select(s => s.Split("\t").Last())
                    .ToList();

                sampleWeight = str.Split("称样量（g）:\t")[1].Split("\t\t\r\n\t")[0];

            }
            catch (Exception)
            {
                return null;
                // throw new Exception("剪切板为空/剪切板数据不匹配！");
            }
            return new HITACHIAA_Report(strList, sampleWeight);
        }

        public class HITACHIAA_Report
        {
            public List<string> aaList;
            public string SampleWeight = string.Empty;


            public HITACHIAA_Report(List<string> aaList, string sampleWeight)
            {
                this.aaList = aaList;
                SampleWeight = sampleWeight;
            }

        }


        /// <summary>
        /// 是否判断检出限
        /// </summary>
        private bool isJudgeLimitValue = true;

        public bool IsJudgeLimitValue
        {
            get
            {
                return isJudgeLimitValue;
            }
            set
            {
                isJudgeLimitValue = value;
                RaisePropertyChanged(nameof(IsJudgeLimitValue));
            }
        }

        private string limitValue = "0";

        public string LimitValue
        {
            get
            {
                return limitValue;
            }
            set
            {
                limitValue = value;
                RaisePropertyChanged(nameof(LimitValue));
            }
        }

        private string overrunExpress = "未检出";

        public string OverrunExpress
        {
            get
            {
                return overrunExpress;
            }
            set
            {
                overrunExpress = value;
                RaisePropertyChanged(nameof(OverrunExpress));
            }
        }


        /// <summary>
        /// 粘贴数据到子项目结果栏
        /// </summary>
        /// <returns></returns>
        [Command]
        public async Task PasteSubItemTempResultFromClipboard()
        {
            if (_messageBoxService.ShowMessage("确认将剪切板上的内容粘贴至检测结果栏?", "确认粘贴?", MessageButton.OKCancel,
                    MessageIcon.Question,
                    MessageResult.Cancel) != MessageResult.OK)
                return;
            string[] strAry = Array.Empty<string>();
            var result = GetDataFromClipboard(ref strAry);
            if (!result)
            {
                _messageBoxService.ShowMessage("剪切板为空！");
                return;
            }

            if (strAry.Length != SelectedEditableSubItems.Count)
            {
                if (_messageBoxService.ShowMessage("剪切板上数据数量与勾选行数不一致，是否继续？", "警告", MessageButton.OKCancel,
                        MessageIcon.Warning, MessageResult.Cancel) != MessageResult.OK)
                    return;
            }

            for (int i = 0; i < SelectedEditableSubItems.Count; i++)
            {
                if (i == strAry.Length)
                    return;
                var subItem = SelectedEditableSubItems[i];
                if (isJudgeLimitValue)
                    if (IsNumeric(strAry[i].Trim()))
                    {
                        subItem.Temp_TestResult = Convert.ToDecimal(strAry[i].Trim()) <= Convert.ToDecimal(limitValue)
                            ? overrunExpress
                            : strAry[i].Trim();
                        await _subItemService.UpdateAsync(subItem);
                        continue;
                    }

                subItem.Temp_TestResult = strAry[i].Trim();
                await _subItemService.UpdateAsync(subItem);
            }
        }

        [Command]
        public void PasteItemTempResultFromClipboard()
        {
            if (_messageBoxService.ShowMessage("确认将剪切板上的内容粘贴至检测结果栏?", "确认粘贴?", MessageButton.OKCancel,
                    MessageIcon.Question,
                    MessageResult.Cancel) != MessageResult.OK)
                return;


            string[] strAry = Array.Empty<string>();
            var result = GetDataFromClipboard(ref strAry);
            if (!result)
            {
                _messageBoxService.ShowMessage("剪切板为空！");
                return;
            }

            if (strAry.Length != selectedTaskDatas.Count)
            {
                if (_messageBoxService.ShowMessage("剪切板上数据数量与勾选行数不一致，是否继续？", "警告", MessageButton.OKCancel,
                        MessageIcon.Warning, MessageResult.Cancel) != MessageResult.OK)
                    return;
            }

            for (int i = 0; i < selectedTaskDatas.Count; i++)
            {
                if (i == strAry.Length)
                    return;
                var item = selectedTaskDatas[i];
                item.Temp_TestResult = strAry[i].Trim();
                ItemTempResultChanged(item, item.Temp_TestResult);
                //await _itemService.UpdateAsync(item);
            }

            _messageBoxService.ShowMessage("操作完成！");
        }
        /// <summary>
        /// 清空子项目结果编辑页面结果
        /// </summary>
        [Command]
        public async void ClearTempSubItemTestResultColumn()
        {
            foreach (var s in SelectedEditableSubItems)
            {
                s.Temp_TestResult = string.Empty;
                s.FirstTestResult = string.Empty;
                s.SecondTestResult = string.Empty;
                s.AverageTestResult = string.Empty;
                await _subItemService.UpdateAsync(s);
            }
        }

        [Command]
        public async void ClearItemTestResultColumn()
        {
            foreach (var s in selectedTaskDatas)
            {
                s.Temp_TestResult = string.Empty;
                await _itemService.UpdateAsync(s);
            }
        }

        #endregion

        private bool allowAutoRounding;

        public bool AllowAutoRounding
        {
            get
            {
                return allowAutoRounding;
            }
            set
            {
                allowAutoRounding = value;
                RaisePropertyChanged(nameof(AllowAutoRounding));
            }
        }

        /// <summary>
        /// 同步子项目检测结果
        /// </summary>
        /// <param name="e"></param>
        [Command]
        public async Task Temp_TestSubItemResultChanged(CellValueChangedArgs e)
        {
            await _subItemService.UpdateAsync(FocusedSubItem);
        }
        /// <summary>
        /// 双击检测项目事件
        /// </summary>
        /// <param name="args"></param>
        [Command]
        public void EditedItemRowDoubleClick(RowClickArgs args)
        {
            if (args.Item is ItemDto item && item != null && item.SubItems.Count > 0)
            {
                PopupMyEditedSubItemsView(item);
            }
        }
        /// <summary>
        /// 弹出编辑子项目窗口
        /// </summary>
        /// <param name="item"></param>
        [Command]
        public void PopupMyEditedSubItemsView(ItemDto item)
        {
            SubItemViewColumns = GetEditedSubItemViewColumns(item);
            EdittingItem = item;
            DevExpress.Mvvm.IDialogService dialogService = GetService<DevExpress.Mvvm.IDialogService>("MyEdiatbleSubItemsViewDialogService");
            dialogService.ShowDialog(
                new List<UICommand>
                {
                    AddTotalResultToItem(item),
                    new UICommand { Caption = "关闭", IsDefault = false, IsCancel = true, }
                }
                , "", this);
        }



        protected async void showNotifaction(string text)
        {
            await ShowNotifaction("", text, "");
        }


        #region 同步项目检测结果



        protected async void ItemTempResultChanged(ItemDto editingItem, string oldValue)
        {
            if (AllowAutoRounding && !string.IsNullOrEmpty(oldValue))
            {
                IsNeedNumericalRevision(ref oldValue, editingItem.RoundRule);
                editingItem.Temp_TestResult = oldValue;
            }

            var conclusion = Judgement(editingItem.IndexRequest, editingItem.Temp_TestResult);
            editingItem.Temp_SingleConclusion = conclusion;

            await _itemService.UpdateAsync(editingItem);



            if (SamplesSource != null)
            {
                var edittingSample = SamplesSource.FirstOrDefault(s => s?.SampleCode == editingItem.SampleCode);
                if (edittingSample != null)
                    edittingSample.Items = await GetAllItemsOfSample(edittingSample);
            }
        }




        //public SubItemDto FocusedSubItem
        //{
        //    get; set;
        //}



        protected List<BindingColumn> GetEditedSubItemViewColumns(ItemDto item)
        {
            return new List<BindingColumn>
            {
                new BindingColumn(SettingsType.Binding, "TestItem", "检测项目"),
                //new BindingColumn(SettingsType.Binding,"ReportUnit","报告单位"),


                new BindingColumn(SettingsType.Binding, "FirstTestResult", "平行一结果")
                    { ReadOnly = false, Visible = item.TestItem.Contains("氨基酸") },
                new BindingColumn(SettingsType.Binding, "SecondTestResult", "平行二结果")
                    { ReadOnly = false, Visible = item.TestItem.Contains("氨基酸") },
                new BindingColumn(SettingsType.Binding, "AverageTestResult", "平均值")
                    { ReadOnly = false, Visible = item.TestItem.Contains("氨基酸") },


                new BindingColumn(SettingsType.Binding, "Temp_TestResult", "检测结果") { ReadOnly = false },
                new BindingColumn(SettingsType.Binding, "IndexRequest", "指标要求"),
                new BindingColumn(SettingsType.Binding, "ItemRemark", "项目备注"),
            };
        }

        /// <summary>
        /// 填充子项目结果加和到父项目结果栏
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected UICommand AddTotalResultToItem(ItemDto item)
        {

            SampleDto editingSample = SamplesSource?.FirstOrDefault(s => s.SampleCode == item.SampleCode);
            return new UICommand
            {
                Caption = "填充加和到父项目",
                IsDefault = false,
                IsCancel = false,
                Command = new DelegateCommand(async () =>
                {
                    decimal result = 0;

                    foreach (SubItemDto subItem in item?.SubItems)
                    {
                        if (IsNumeric(subItem?.Temp_TestResult))
                        {
                            result += Convert.ToDecimal(subItem.Temp_TestResult);
                        }
                    }
                    var oldResult = result == 0 ? string.Empty : result.ToString();
                    IsNeedNumericalRevision(ref oldResult, item?.RoundRule);

                    item.Temp_TestResult = oldResult.ToString();
                    item.Temp_SingleConclusion = Judgement(item?.IndexRequest, item.Temp_TestResult);
                    await _itemService.UpdateAsync(item);
                    await ExcuteIfNullSample(editingSample, TaskDatasSource?.Where(i => i.SampleCode == editingSample.SampleCode));

                })
            };

        }
        #endregion 同步项目检测结果



        #endregion



    }

}