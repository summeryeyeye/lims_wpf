using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.POCO;
using Lims.Common.Dtos;
using Lims.Common.Parameters;
using Lims.ToolsForClient;
using NPOI.Util;
using System.Windows.Input;

namespace Lims.WPF.ViewModels
{
    public class CurrentTasksManageViewModel : SampleAndItemDataViewModelBase
    {

        protected override async void OnInitializeInRuntime()
        {
            base.OnInitializeInRuntime();
            StandardsSource = (await _methodStandardService.GetAllAsync()).Result;
        }

        public static CurrentTasksManageViewModel Create(string caption)
        {
            return ViewModelSource.Create(() => new CurrentTasksManageViewModel()
            {
                Caption = caption,
            });
        }
        private DateTime sample_BeginDate = DateTime.Today.AddMonths(-4);

        public override DateTime Sample_BeginDate
        {
            get => sample_BeginDate;
            set
            {
                sample_BeginDate = value;
                LoadMainDatas(CurrentUser);
            }
        }
        protected override async Task LoadMainDatas(UserDto? user)
        {

            ShowMainDatasLoadingPanel = true;
            pre_MyFocusedSampelRowIndex = FocusedSampleRowHandle;
            try
            {
                if (ShowCompeleteDatas)
                {
                    SampleFilterParam param = new SampleFilterParam()
                    {
                        MinDate = Sample_BeginDate,
                        MaxDate = Sample_EndDate,
                        WithItems = false
                    };
                    var response = await _sampleService.GetSamplesAsync(param);

                    if (response.Status)
                        SamplesSource = response.Result.OrderByDescending(s => s.CreateTime).ToObservableCollection();
                }
                else
                {
                    ItemFilterParam param = new ItemFilterParam()
                    {
                        TestProgress = (int)RelativeProgress,
                        Operation = Operation.Lower,                       
                    };
                    var response = await _itemService.GetAllItemsByTestProgressAsync(param);
                    if (response.Status)
                    {
                        TaskDatasSource = response.Result.OrderByDescending(t => t.ResultSubmitTime).ToObservableCollection();
                        itemDtos = TaskDatasSource?.Copy();
                        SamplesSource = TaskDatasSource.Select(s => s.Sample).DistinctBy(s => s.SampleCode).OrderBy(s => s.SampleCode).ToObservableCollection();
                    }

                }
            }
            catch (Exception)
            {
                throw;
            }
            FocusedSampleRowHandle = pre_MyFocusedSampelRowIndex;
            ShowMainDatasLoadingPanel = false;
        }

        [Command]
        public override void ShowAllItemsOfCurrentSample(SampleDto sample)
        {
            ReportDataPreview(sample);
        }
        public List<MethodStandardDto> CurrentMethods
        {
            get; set;
        }
        private List<MethodStandardDto>? standardsSource;
        public List<MethodStandardDto>? StandardsSource
        {
            get
            {
                return standardsSource;
            }
            set
            {
                standardsSource = value;
                RaisePropertiesChanged(nameof(StandardsSource));
            }
        }




        private MethodStandardDto edittingStandardMethod;

        public MethodStandardDto EdittingStandardMethod
        {
            get => edittingStandardMethod;
            set
            {
                edittingStandardMethod = value;
                EdittingItem.ReportUnit = value.TestUnit;
                RaisePropertiesChanged(nameof(EdittingStandardMethod));


            }
        }
        /// <summary>
        /// 变更项目信息
        /// </summary>
        /// <param name="item"></param>
        [Command]
        public async Task ChangeItemInfo(ItemDto item)
        {
            if (item == null) return;
            EdittingItem = (ItemDto)item.Clone();
            var response = await _methodStandardService.GetMethodStandardsByKeyItemAsync(new MethodStandardFilterParam() { KeyItem = item.MethodStandard.KeyItem, SampleState = item.Sample.SampleState });
            CurrentMethods = response.Result;
            EdittingStandardMethod = CurrentMethods.FirstOrDefault(s => s.Id == item.MethodStandardId);


            var dialogService = GetService<IDialogService>("EditItemInfoViewDialogService");
            dialogService.ShowDialog(
                new List<UICommand> {
                        new() {Caption = "变更项目信息",IsDefault = false,IsCancel = false,Command = new DelegateCommand( async () =>
                        {
                                EdittingItem.MethodStandardId=EdittingStandardMethod.Id;
                                EdittingItem.MethodStandard=EdittingStandardMethod;
                                EdittingItem.TestMethod=edittingStandardMethod.TestMethod;
                                //EdittingItem.ReportUnit=edittingStandardMethod.TestUnit;
                                await _itemService.UpdateAsync(EdittingItem);
                                item=EdittingItem;
                                //ItemsSource[ItemsSource.IndexOf(item)]=EdittingItem;

                        })},
                        new() {Caption = "取消",IsDefault = false,IsCancel = true,}
                }
                , "", this);




            LoggerDto logger = new(DateTimeOffset.Now)
            {
                LogLevel = LogLevel.INFO,
                ActionType = ActionType.变更项目信息,
                PublisherIP = LoggerDto.GetLocalIP(),
                PublisherName = CurrentUser.UserName,
                ReceiverName = item.Tester,
                SampleCode = item.SampleCode,
                TestItem = item.TestItem,
                Message = "变更项目信息",
            };

            // await _loggerService.CreateAsync(logger);
            //await ShowNotifaction(EdittingItem.SampleCode + "  " + EdittingItem.TestItem, "变更项目信息成功！", "");
        }
        [Command]
        public async Task ReEditReport(SampleDto edittingSample)
        {
            //var items =await GetAllItemsOfSample(edittingSample);



            if (ItemsSource.Max(i => i.TestProgress) != 107 || ItemsSource.Max(i => i.TestProgress) != 107)
            {
                _messageBoxService.ShowMessage("只能修改已完成三审的任务！");
                return;
            }
            if (_messageBoxService.ShowMessage("确认修改报告？", "", MessageButton.OKCancel, MessageIcon.Warning, MessageResult.Cancel) == MessageResult.OK)
            {

                foreach (var item in ItemsSource)
                    item.TestProgress = 106;
                //edittingSample.MinTestProgress = 106;
                //edittingSample.MaxTestProgress = 106;
                await _itemService.UpdateRangeAsync(ItemsSource.ToList());
                await _sampleService.UpdateAsync(edittingSample);
                await ShowNotifaction(edittingSample.SampleCode, "该样品下所有项目已退回至任务三审！", "");
            }


        }
        /// <summary>
        /// 添加样品附件
        /// </summary>
        /// <param name="edittingSample"></param>
        /// <returns></returns>
        [Command]
        public  void AddSampleAttachment(SampleDto edittingSample)
        {
            var data = _iOpenFileDialogService.ShowDialog();

        }
        public UserDto EditedUser
        {
            get; set;
        }


        //}
        /// <summary>
        /// 变更项目分析人
        /// </summary>
        /// <param name="item"></param>
        [Command]
        public void ChangeItemTester(ItemDto item)
        {
            EdittingItem = item;
            var dialogService = GetService<IDialogService>("ChangeItemTesterDialogService");
            dialogService.ShowDialog(
                new List<UICommand> {
                        new UICommand{Caption = "变更分析人",IsDefault = true,IsCancel = false,Command = new DelegateCommand(async () =>
                        {
                            if (EditedUser != null && EdittingItem.Tester != EditedUser.UserName){
                                var editingSample = FocusedSample;

                                int newProgress = (int)TestProgress.待领取;



                                EdittingItem.Tester = EditedUser.UserName;
                                EdittingItem.TestProgress = newProgress;
                                EdittingItem.Temp_TestResult=string.Empty;
                                EdittingItem.TestResult=string.Empty;
                                await _itemService.UpdateAsync(EdittingItem);

                                await _loggerService.CreateAsync(new LoggerDto(DateTimeOffset.Now)
                                {
                                    LogLevel=LogLevel.WARN,
                                    ActionType=ActionType.变更项目分析人,
                                    PublisherIP=LoggerDto.GetLocalIP(),
                                    PublisherName=CurrentUser.UserName,
                                    ReceiverName=EditedUser.UserName,
                                    SampleCode=EdittingItem.SampleCode,
                                    TestItem=EdittingItem.TestItem,
                                    Message="变更项目分析人",
                                });

                                await ShowNotifaction(EdittingItem.SampleCode + "  " + EdittingItem.TestItem, "变更分析人成功！", "");
                            }
                        })},
                        new UICommand{Caption = "取消",IsDefault = false,IsCancel = true,}
                }
                , "", this);
        }
        /// <summary>
        /// 删除项目
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [Command]
        public async Task DeleteItem(ItemDto item)
        {
            MessageResult result = _messageBoxService.ShowMessage($"确定删除 {item.SampleCode} {item.TestItem} ？", "删除", MessageButton.YesNo, MessageIcon.Warning, MessageResult.Cancel);
            if (result == MessageResult.Yes)
            {
                var tempItem = (ItemDto)item.Clone();
                var sample = SamplesSource.FirstOrDefault(s => s.SampleCode == tempItem.SampleCode);

                foreach (var s in tempItem.SubItems)
                    await _subItemService.DeleteAsync(s.ItemId);
                /* 
                tempItem.TestProgress = (int)TestProgress.已删除;
                await _itemService.UpdateAsync(tempItem);
                */
                await _itemService.DeleteAsync(tempItem.ItemId);

                ItemsSource.Remove(item);

                sample.Items = await GetAllItemsOfSample(sample);
                FocusedSample = sample;
                if (ItemsSource.Count > 0)
                {
                    //await UpdateSampleTestProgress(sample);
                }
                else
                {
                    await _sampleService.DeleteAsync(tempItem.SampleCode);
                    pre_MyFocusedSampelRowIndex = FocusedSampleRowHandle;
                    SamplesSource.Remove(SamplesSource.FirstOrDefault(s => s.SampleCode == tempItem.SampleCode));
                    FocusedSampleRowHandle = pre_MyFocusedSampelRowIndex;
                    await ShowNotifaction(tempItem.SampleCode, "该样品下无其他项目,样品已自动删除!", "");
                }
                await ShowNotifaction(tempItem.SampleCode + "  " + tempItem.TestItem, "项目已删除", "");

                await _loggerService.CreateAsync(new LoggerDto(DateTimeOffset.Now)
                {
                    LogLevel = LogLevel.WARN,
                    ActionType = ActionType.删除任务,
                    PublisherIP = LoggerDto.GetLocalIP(),
                    PublisherName = CurrentUser.UserName,
                    ReceiverName = tempItem.Tester,
                    SampleCode = tempItem.SampleCode,
                    TestItem = tempItem.TestItem,
                    Message = "项目已删除",
                });
            }

        }
        /// <summary>
        /// 删除子项目
        /// </summary>
        private async void DeleteSubItems()
        {
            if (SelectedPreviewSubItems.Count <= 0)
            {
                _messageBoxService.ShowMessage("请选中项目后操作");
                return;
            }
            var result = _messageBoxService.ShowMessage("确认删除选中子项目？", "确认", MessageButton.YesNo, MessageIcon.Question, MessageResult.Cancel);
            if (result == MessageResult.Yes)
            {
                SubItemDto[] subItems = new SubItemDto[SelectedPreviewSubItems.Count];
                SelectedPreviewSubItems.CopyTo(subItems, 0);
                var item = (await _itemService.GetSingleAsync(subItems[0].ItemId)).Result;

                //SelectedPreviewSubItems.CopyTo(subItems, 0);
                foreach (var subItem in subItems)
                {
                    await _subItemService.DeleteAsync(subItem.SubItemId);
                    EdittingItem.SubItems.Remove(EdittingItem.SubItems.FirstOrDefault(s => s.SubItemId == subItem.SubItemId));

                    if (item.TestProgress <= (int)TestProgress.检测中)
                    {
                        await _loggerService.CreateAsync(new LoggerDto(DateTimeOffset.Now)
                        {
                            LogLevel = LogLevel.WARN,
                            ActionType = ActionType.删除任务,
                            PublisherIP = LoggerDto.GetLocalIP(),
                            PublisherName = CurrentUser.UserName,
                            ReceiverName = EdittingItem.Tester,
                            SampleCode = EdittingItem.SampleCode,
                            TestItem = subItem.TestItem,
                            Message = "子项目已删除",
                        });
                    }
                    else
                    {
                        await _loggerService.CreateAsync(new LoggerDto(DateTimeOffset.Now)
                        {
                            LogLevel = LogLevel.INFO,
                            ActionType = ActionType.删除任务,
                            PublisherIP = LoggerDto.GetLocalIP(),
                            PublisherName = CurrentUser.UserName,
                            ReceiverName = EdittingItem.Tester,
                            SampleCode = EdittingItem.SampleCode,
                            TestItem = subItem.TestItem,
                            Message = "子项目已删除",
                        });
                    }

                }

                await ShowNotifaction(EdittingItem.SampleCode + "  " + EdittingItem.TestItem, "删除子项目完成！", "");
            }
        }
        /// <summary>
        /// 删除样品
        /// </summary>
        /// <param name="sample"></param>
        [Command]
        public async Task DeleteSamples(ObservableCollection<SampleDto> samples)
        {
            if (samples.Count > 5)
            {
                _messageBoxService.ShowMessage("一次删除样品数量不得超过5个，请重试！");
                return;
            }


            MessageResult result = _messageBoxService.ShowMessage($"确定删除勾选样品？", "删除", MessageButton.YesNo, MessageIcon.Warning, MessageResult.Cancel);
            if (result == MessageResult.Yes)
            {
                for (int i = samples.Count - 1; i > -1; i--)
                {
                    var sample = samples[i];

                    var tempSample = (SampleDto)sample.Clone();

                    pre_MyFocusedSampelRowIndex = FocusedSampleRowHandle;
                    SamplesSource.Remove(sample);
                    FocusedSampleRowHandle = pre_MyFocusedSampelRowIndex;

                    var response = await _itemService.GetAllItemsBySampleCodeAsync(new Common.Parameters.ItemFilterParam(sample.SampleCode));
                    if (response.Status)
                    {
                        var items = response.Result;


                        foreach (var item in items)
                        {
                            LoggerDto logger = new LoggerDto(DateTimeOffset.Now)
                            {
                                LogLevel = LogLevel.WARN,
                                ActionType = ActionType.删除任务,
                                PublisherIP = LoggerDto.GetLocalIP(),
                                PublisherName = CurrentUser.UserName,
                                ReceiverName = item.Tester,
                                SampleCode = item.SampleCode,
                                TestItem = item.TestItem,
                                Message = "样品已删除",
                            };
                            await _loggerService.CreateAsync(logger);
                        }

                        foreach (var item in items)
                        {
                            foreach (var s in item.SubItems)
                                await _subItemService.DeleteAsync(s.SubItemId);
                            await _itemService.DeleteAsync(item.ItemId);
                        }

                        await _sampleService.DeleteAsync(tempSample!.SampleCode);

                        await ShowNotifaction(tempSample.SampleCode + "  " + tempSample.SampleName, "样品已删除", "");
                    }

                }


            }
        }
        public List<SubItemClass> StandardSubItems { get; set; } = new();

        [Command]
        public async Task AddMoreSubItem(ItemDto itemModel)
        {
            StandardSubItems.Clear();
            EdittingItem = itemModel;
            var response = await _subItemStandardService.GetSubItemStandardsByTestItemAsync(new SubItemStandardFilterParam() { ParentNames = itemModel.TestItem });
            if (response.Status)
            {
                var subItemStandards = response.Result.OrderBy(s => s.Id);

                foreach (var item in subItemStandards.DistinctBy(s => s.Type))
                {
                    StandardSubItems.Add(new SubItemClass { Text = item.Type });
                }
                foreach (var item in StandardSubItems)
                {
                    var subs = subItemStandards.Where(s => s.Type == item.Text);

                    List<SubItem> subitems = new List<SubItem>();

                    foreach (var s in subs)
                    {
                        subitems.Add(new SubItem { TestItem = s.Name });
                    }
                    item.SubItems = subitems;
                }

                var dialogService = GetService<IDialogService>("AddSubItemsDialogService");
                dialogService.ShowDialog(new List<UICommand> {
                        new UICommand{Caption = "关闭窗口",IsDefault = false,IsCancel = true,}
                }, "", this);
            }

        }

        [Command]
        public void CheckAllItems(object[] objs)
        {
            bool CheckAll = (bool)objs[0];
            IEnumerable<ICheckable> source = (IEnumerable<ICheckable>)objs[1];
            if (source == null)
                return;
            foreach (var item in source)
            {
                item.IsChecked = CheckAll;
            }
        }

        [Command]
        public async Task AddToPreviewSubItems(List<SubItem> subItems)
        {
            if (_messageBoxService.ShowMessage("确认添加子项目到该项目下？", "确认", MessageButton.OKCancel, MessageIcon.Question, MessageResult.OK) == MessageResult.OK)
            {
                bool hasNewSubItems = false;
                var checkedItems = subItems.Where(s => s.IsChecked);
                DateTimeOffset currentTime = DateTimeOffset.Now;

                var response = await _subItemService.GetSubItemByItemIdAsync(new SubItemFilterParam { ItemId = EdittingItem.ItemId });
                if (response.Result.Count > 0)
                {
                    var subitems = response.Result;
                    decimal maxId = Convert.ToDecimal(subitems.Last().SubItemId);

                    foreach (var checkedItem in checkedItems)
                    {
                        if (subitems.Any(c => c.TestItem == checkedItem.TestItem))
                            continue;
                        maxId++;

                        SubItemDto subItem = new SubItemDto
                        {
                            ItemId = EdittingItem.ItemId,
                            TestItem = checkedItem.TestItem,
                            CreateTime = currentTime,
                            SubItemId = maxId.ToString(),
                            //ReportUnit = EdittingItem.ReportUnit,
                        };

                        EdittingItem.SubItems.Add(subItem);
                        await _subItemService.CreateAsync(subItem);
                        hasNewSubItems = true;
                        checkedItem.IsChecked = false;
                    }
                }
                else
                {
                    EdittingItem.SubItems = new();
                    string parentIDpre = EdittingItem.ItemId.Substring(0, 14);
                    int secondDataIndex = 0;
                    int codeIndex = Convert.ToInt32(EdittingItem.ItemId.Substring(14, 7));
                    foreach (var checkedItem in checkedItems)
                    {
                        if (EdittingItem.SubItems.Any(c => c.TestItem == checkedItem.TestItem))
                            continue;

                        var subItemId = parentIDpre + string.Empty.PadLeft(7 - (codeIndex - 500 + secondDataIndex).ToString().Length, '0') + (codeIndex - 500 + secondDataIndex).ToString();
                        secondDataIndex++;
                        SubItemDto subItem = new SubItemDto
                        {
                            ItemId = EdittingItem.ItemId,
                            TestItem = checkedItem.TestItem,
                            CreateTime = currentTime,
                            SubItemId = subItemId,
                            //ReportUnit = EdittingItem.ReportUnit,
                        };
                        EdittingItem.SubItems.Add(subItem);
                        await _subItemService.CreateAsync(subItem);
                        checkedItem.IsChecked = false;
                        hasNewSubItems = true;
                    }
                }

                if (hasNewSubItems)
                {
                    EdittingItem.TestProgress = (int)TestProgress.待领取;
                    await _itemService.UpdateAsync(EdittingItem);
                    var edittingSample = SamplesSource.FirstOrDefault(s => s.SampleCode == EdittingItem.SampleCode);
                    await _sampleService.UpdateAsync(edittingSample);
                    //await UpdateSampleTestProgress(edittingSample);
                }
            }
        }

        [Command]
        public void CheckSubItem(SubItem subitem)
        {
            subitem.IsChecked = !subitem.IsChecked;
        }




        /// <summary>
        /// 子项目操作
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        [Command]
        public  void PreviewSubItemsKeyUp(KeyEventArgs e)
        {

            switch (e.Key)
            {
                case Key.Delete:
                    DeleteSubItems();
                    break;

                default:
                    break;
            }
        }

        public ObservableCollection<SubItemDto> SelectedPreviewSubItems { get; set; } = new();

        public SampleDto EdittingSample
        {
            get; set;
        }
        private bool canChangeUrgent = true;

        public bool CanChangeUrgent
        {
            get { return canChangeUrgent; }
            set { canChangeUrgent = value; RaisePropertiesChanged(nameof(CanChangeUrgent)); }
        }
        /// <summary>
        /// 变更样品信息
        /// </summary>
        /// <param name="sampleModel"></param>
        [Command]
        public void ChangeSampleInfo(SampleDto sampleModel)
        {
            TimeSpan absTime = DateTimeOffset.Now - sampleModel.CreateTime;

            CanChangeUrgent = absTime.TotalDays < 1;

            EdittingSample = (SampleDto)sampleModel.Clone();
            SelectedSampleStateIndex = EdittingSample.SampleState == "固体" ? 0 : 1;
            var editedSampleStateIndex = SelectedSampleStateIndex;
            var dialogService = GetService<IDialogService>("ChangeSampleInfoDialogService");
            dialogService.ShowDialog(
                new List<UICommand> {
                        new UICommand{Caption = "变更样品信息",IsDefault = true,IsCancel = false,Command = new DelegateCommand( async () =>
                        {
                            string state=SelectedSampleStateIndex==0?"固体":"液体";
                            EdittingSample.SampleState=state;
                            int index=SamplesSource.IndexOf(sampleModel);
                            SamplesSource[index]=EdittingSample;
                            await _sampleService.UpdateAsync(EdittingSample);



                            FocusedSample=sampleModel;
                            await ShowNotifaction(EdittingSample.SampleCode, "变更样品信息成功！", "");
                        })},
                        new UICommand{Caption = "取消",IsDefault = false,IsCancel = true,}
                }
                , "", this);
        }

        public int SelectedSampleStateIndex
        {
            get; set;
        }


        #region 样品复测        
        public ObservableCollection<ItemDto> SelectedItemsToRetest { get; set; } = new();

        public List<SampleDto> RetestSamples { get; set; } = new();

        private bool showCompeleteDatas=true;

        public bool ShowCompeleteDatas
        {
            get { return showCompeleteDatas; }
            set { showCompeleteDatas = value; RaisePropertyChanged(nameof(ShowCompeleteDatas)); }
        }


        private bool isRetestUrgent;

        public bool IsRetestUrgent
        {
            get => isRetestUrgent;
            set
            {
                isRetestUrgent = value;
                RaisePropertyChanged(nameof(IsRetestUrgent));
            }
        }

        private int retestTimes = 1;

        public int RetestTimes
        {
            get => retestTimes;
            set
            {
                retestTimes = value;
                RaisePropertyChanged(nameof(RetestTimes));
            }
        }

        private ObservableCollection<ItemDto> retestingItems = new();

        public ObservableCollection<ItemDto> RetestingItems
        {
            get => retestingItems;
            set
            {
                retestingItems = value;
                RaisePropertyChanged(nameof(RetestingItems));
            }
        }

        private List<ItemDto> selectedRetestingItems = new();

        public List<ItemDto> SelectedRetestingItems
        {
            get => selectedRetestingItems;
            set
            {
                selectedRetestingItems = value;
            }
        }

        [Command]
        public void AddToRetestingItems(ItemDto item)
        {
            if (RetestingItems.Contains(RetestingItems.FirstOrDefault(i => i.TestItem == item.TestItem)))
                return;
            ItemDto itemModel = (ItemDto)item.Clone();
            itemModel.SubItems = new();
            if (item.SubItems != null && item.SubItems.Count > 0)
            {
                var dialogService = GetService<IDialogService>("RetestSubItemsPreviewDialogService");
                dialogService.ShowDialog(
                    new List<UICommand> {
                        new UICommand{Caption = "确认",IsDefault = true,IsCancel = false,Command = new DelegateCommand(  () =>
                        {
                            SubItemDto[] subItemModels=new   SubItemDto[item.SelectedRetestSubItems.Count];
                            item.SelectedRetestSubItems.CopyTo(subItemModels,0);

                            foreach (var subitem in subItemModels)
                                itemModel.SubItems.Add(subitem);
                        })},
                        new UICommand{Caption = "取消",IsDefault = false,IsCancel = true,}
                    }
                    , "", item);
            }
            RetestingItems.Add(itemModel);
        }

        [Command]
        public void AddAllToRetestingItems()
        {
            foreach (var item in AllItemsOfFocusedSample)
            {
                AddToRetestingItems(item);
            }
        }

        [Command]
        public void PreviewRetestItemsKeyUp(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Delete:
                    ItemDto[] items = new ItemDto[SelectedRetestingItems.Count];
                    SelectedRetestingItems.CopyTo(items);
                    foreach (ItemDto item in items)
                    {
                        RetestingItems.Remove(item);
                    }
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// 添加复测项目
        /// </summary>
        /// <param name="sampleModel"></param>
        /// <returns></returns>
        [Command]
        public async Task RetestSample(SampleDto sampleModel)
        {
            if (sampleModel.SampleCode.Contains('F'))
            {
                _messageBoxService.ShowMessage("请选择初始样品进行复测操作！");
                return;
            }
            RetestingItems.Clear();
            SelectedItemsToRetest = new();
            string originalCode = sampleModel.SampleCode.PadLeft(10);

            var response = await _sampleService.GetSamplesBySampleCodeKeyWordAsync(new SampleFilterParam() { SampleCodeKeyWord = originalCode });
            if (response.Status)
                RetestSamples = response.Result;

            var existSamples = (await _sampleService.GetAllAsync()).Result;
            AllItemsOfFocusedSample = (await _itemService.GetAllItemsBySampleCodeAsync(new Common.Parameters.ItemFilterParam(sampleModel.SampleCode))).Result.ToObservableCollection();
            var dialogService = GetService<IDialogService>("RetestSampleViewDialogService");
            dialogService.ShowDialog(
                new List<UICommand> {
                        new UICommand{Caption = "确认添加复测",IsDefault = true,IsCancel = false,Command = new DelegateCommand(  async () =>
                        {

                            string? newSampleCode=sampleModel.SampleCode+'F'+RetestTimes;

                            List<ItemDto> existItems = (await _itemService.GetAllItemsBySampleCodeAsync(new ItemFilterParam(){SampleCode=newSampleCode})).Result;

                            DateTimeOffset currentTime=DateTimeOffset.Now;
                            if (!existSamples.Any(s=>s.SampleCode==newSampleCode))
                            {
                                SampleDto newSample=(SampleDto)sampleModel.Clone();
                                newSample.SampleCode=newSampleCode;
                                newSample.IsUrgent=IsRetestUrgent;
                                newSample.CreateTime=currentTime;
                                newSample.CheckRemark=null;
                                newSample.FirstAuditTime=null;
                                newSample.SecondAuditTime=null;
                                newSample.CompleteTime=null;
                                //newSample.MinTestProgress=(int)TestProgress.待领取;
                                //newSample.MaxTestProgress=(int)TestProgress.待领取;
                                await _sampleService.CreateAsync(newSample);
                            }

                            ItemDto[] midItems=new ItemDto[RetestingItems.Count];
                            RetestingItems.CopyTo(midItems,0);
                            string preIdentityCode = DateTimeOffset.Now.ToString("yyyyMMddHHmmss");
                            string? itemId;
                            SampleDto existSample=(await _sampleService.GetSingleAsync(newSampleCode)).Result;
                            for (int i = 0; i < midItems.Count(); i++)
                            {
                                ItemDto item=midItems[i];

                                if (item.TestProgress>(int)TestProgress.检测中)
                                   ReturnTask(item,sampleModel,"样品复测退回");

                                item.SampleCode= newSampleCode;


                                if (!existItems.Any(i=>i.TestItem==item.TestItem))
                                {
                                    itemId=preIdentityCode + (i+existItems.Count+1).ToString("D4") + string.Empty.PadRight(3, '0'); ;
                                    item.ItemId= itemId;

                                    item.AppointTime= currentTime;

                                    item.TestResult= null;
                                    item.Temp_TestResult= null;

                                    item.SingleConclusion= null;
                                    item.Temp_SingleConclusion= null;

                                    item.ResultSubmitTime= null;
                                    item.TestDate= null;
                                    item.IsOriginalRecordComplete= false;
                                    item.PreTestProgress= (int)TestProgress.无;
                                    item.TestProgress= (int)TestProgress.待领取;
                                    await _itemService.CreateAsync(item);
                                }
                                else{
                                    ItemDto existItem=existItems.FirstOrDefault(i=>i.TestItem==item.TestItem);
                                    itemId=existItem.ItemId;
                                    existItem.TestProgress= (int)TestProgress.待领取;
                                    await _itemService.UpdateAsync(existItem);
                                }

                                if (item.SubItems != null && item.SubItems.Count > 0)
                                {
                                    var existSubItems=(await _subItemService.GetSubItemByItemIdAsync(new SubItemFilterParam(){ItemId=itemId})).Result;
                                   string? maxSubItemId=null;
                                    if (existSubItems!=null&&existSubItems.Count()>0)
                                    {
                                        maxSubItemId=existSubItems.Last().ItemId;
                                    }

                                    string parentIDpre = itemId.Substring(0, 14);

                                    //var subItems = await _iSubItemService.QueryAsync(s => s.ParentId == focusedPreviewItem.ItemId);
                                    int secondDataIndex = 0;
                                    string? subItemId;
                                    foreach (var subItem in item.SubItems)
                                    {
                                         if (!existSubItems.Any(s=>s.TestItem==subItem.TestItem))
                                        {
                                            subItem.ItemId = itemId;
                                            subItem.CreateTime = currentTime;

                                            subItem.TestResult= null;
                                            subItem.Temp_TestResult= null;

                                            int codeIndex = Convert.ToInt32(item.ItemId.Substring(14, 7));
                                            var str = string.Empty.PadLeft(8 - (codeIndex - 500 + secondDataIndex).ToString().Length, '0');

                                            subItemId =maxSubItemId==null? parentIDpre + string.Empty.PadLeft(7 - (codeIndex - 500 + secondDataIndex).ToString().Length, '0') + (codeIndex - 500 + secondDataIndex).ToString():(Convert.ToDecimal(maxSubItemId)+secondDataIndex).ToString() ;
                                            secondDataIndex++;
                                            subItem.SubItemId = subItemId;
                                            await _subItemService.CreateAsync(subItem);
                                        }
                                    }
                                }


                            }

                            await ShowNotifaction(newSampleCode, "添加复测成功！", "");
                            //await UpdateSampleTestProgress(existSample);
                        })},
                        new UICommand{Caption = "取消",IsDefault = false,IsCancel = true,}
                }
                , "", this);
        }

        #endregion

        [Command]
        public void PrintTaskList(ObservableCollection<SampleDto> samples)
        {
            var dialogService = GetService<IDialogService>("PrinterSelecterViewDialogService");
            dialogService.ShowDialog(
            new List<UICommand> {
                        new UICommand{Caption = "打印任务随行单",IsDefault = true,IsCancel = false,Command = new DelegateCommand(new Action(async () =>
                        {
                            var param=new ItemFilterParam()
                            {
                                SampleCodes=string.Join("and",samples.Select(s=>s.SampleCode)),
                            };
                            var response=await _itemService.GetAllItemsBySampleCodesAsync(param);

                            if (response.Status)
                            {
                             await 打印任务随行单(response.Result.OrderBy(s=>s.ItemId).ToObservableCollection(), Printers[CurrentPrinterIndex]);
                            }
                        }))},
                        new UICommand{Caption = "取消",IsDefault = false,IsCancel = true,}
            }
            , "", this);
        }


        //[Command]
        //public void PrintTaskList(SampleDto sample)
        //{
        //    //CurrentPrinterIndex = System.Convert.ToInt32(ConfigurationManager.AppSettings["CurrentPrinterIndex"]);
        //    var dialogService = GetService<IDialogService>("PrinterSelecterViewDialogService");
        //    dialogService.ShowDialog(
        //    new List<UICommand> {
        //                new UICommand{Caption = "打印任务随行单",IsDefault = true,IsCancel = false,Command = new DelegateCommand(new Action(async () =>
        //                {

        //                    await 打印任务随行单((await GetAllItemsOfSample(sample)).ToObservableCollection(), Printers[CurrentPrinterIndex]);
        //                }))},
        //                new UICommand{Caption = "取消",IsDefault = false,IsCancel = true,}
        //    }
        //    , "", this);
        //}

        public override TestProgress GetRelativeProgress()
        {
            return TestProgress.已完成;
        }
    }
}