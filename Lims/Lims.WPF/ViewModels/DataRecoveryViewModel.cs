//using DevExpress.Mvvm;
//using DevExpress.Mvvm.DataAnnotations;
//using DevExpress.Mvvm.Native;
//using DevExpress.Mvvm.POCO;
//using DevExpress.Mvvm.Xpf;
//using Lims.Common.Dtos;
//using Lims.Common.Parameters;
//using Lims.ToolsForClient;
//using Lims.ToolsForClient.Extensions;
//using Lims.WPF.Resources;
//using System.Data;
//using System.Text.RegularExpressions;
//using System.Windows;

//namespace Lims.WPF.ViewModels
//{
//    public class DataRecoveryViewModel : MyTasksViewModelBase
//    {
//        public static DataRecoveryViewModel Create(string caption)
//        {
//            return ViewModelSource.Create(() => new DataRecoveryViewModel()
//            {
//                Caption = caption,
//            });
//        }
//        public DataRecoveryViewModel()
//        {
//            AllowAutoRounding = Convert.ToBoolean(cfa.AppSettings.Settings["AllowAutoRounding"].Value);

//        }



//        protected override async Task LoadMainDatas(UserDto user)
//        {
//            ShowMainDatasLoadingPanel = true;
//            pre_MyFocusedSampelRowIndex = FocusedSampleRowHandle;

//            ItemFilterParam itemFilterParam = new ItemFilterParam()
//            {
//                TestProgress = (int)RelativeProgress,
//                Operation = Operation.Equal,
//            };
//            var response = await _itemService.GetAllItemsByTestProgressAsync(itemFilterParam);
//            if (response.Status)
//            {
//                TaskDatasSource = response.Result.OrderBy(t => t.AppointTime).Where(i => i.IsOverDate).ToObservableCollection();
//                SamplesSource = TaskDatasSource.Select(i => i.Sample).DistinctBy(s => s.SampleCode).ToObservableCollection();
//            }
//            FocusedSampleRowHandle = pre_MyFocusedSampelRowIndex;
//            ShowMainDatasLoadingPanel = false;
//        }


//        #region 同步项目检测结果

//        /// <summary>
//        /// 同步项目检测结果
//        /// </summary>
//        /// <param name="e"></param>
//        [Command]
//        public void Temp_TestItemResultChanged(CellValueChangedArgs e)
//        {
//            string oldValue = e.Value == null ? string.Empty : e.Value.ToString();
//            var editingItem = e.Item as ItemDto;
//            ItemTempResultChanged(editingItem, oldValue);
//        }
//        private async void ItemTempResultChanged(ItemDto editingItem, string oldValue)
//        {
//            if (AllowAutoRounding && !string.IsNullOrEmpty(oldValue))
//            {
//                IsNeedNumericalRevision(ref oldValue, editingItem.RoundRule);
//                editingItem.Temp_TestResult = oldValue;
//            }

//            var conclusion = Judgement(editingItem.IndexRequest, editingItem.Temp_TestResult);
//            editingItem.Temp_SingleConclusion = conclusion;

//            await _itemService.UpdateAsync(editingItem);
//            var edittingSample = SamplesSource.FirstOrDefault(s => s.SampleCode == editingItem.SampleCode);


//            if (edittingSample != null)
//                edittingSample.Items = await GetAllItemsOfSample(edittingSample);
//        }

//        public SubItemDto FocusedSubItem
//        {
//            get;
//            set;
//        }

//        /// <summary>
//        /// 同步子项目检测结果
//        /// </summary>
//        /// <param name="e"></param>
//        [Command]
//        public async Task Temp_TestSubItemResultChanged(CellValueChangedArgs e)
//        {
//            await _subItemService.UpdateAsync(FocusedSubItem);
//        }

//        #endregion 同步项目检测结果       

//        /// <summary>
//        /// 提交任务窗口
//        /// </summary>
//        [Command]
//        public void SubmitTaskList()
//        {
//            try
//            {
//                ObservableCollection<ItemDto> source = selectedTaskDatas.Where(i => !string.IsNullOrWhiteSpace(i.Temp_TestResult)).ToObservableCollection();

//                TaskListPreviewSources = source;
//                if (TaskListPreviewSources != null && TaskListPreviewSources.Count > 0)
//                {
//                    ItemDto[] tasks = new ItemDto[TaskListPreviewSources.Count];
//                    TaskListPreviewSources.CopyTo(tasks, 0);
//                    DateTimeOffset now = DateTimeOffset.Now;
//                    IDialogService dialogService = GetService<IDialogService>("MySubmittedTaskListPreviewDialogService");
//                    _ = dialogService.ShowDialog(
//                        new List<UICommand> {
//                        new UICommand{Caption = "确认提交",IsDefault = true,IsCancel = false,Command = new DelegateCommand(async() =>
//                        {
//                            foreach (var task in tasks)
//                            {
//                                SampleDto editingSample = SamplesSource.FirstOrDefault(s=>s.SampleCode==task.SampleCode);
//                                int newProgress = task.PreTestProgress == (int)TestProgress.无 ? (int)TestProgress.数据一审 : task.PreTestProgress;
//                                task.TestResult = task.Temp_TestResult;
//                                task.SingleConclusion=task.Temp_SingleConclusion;
//                                task.TestProgress = newProgress;
//                                task.PreTestProgress = (int)TestProgress.无;
//                                task.ResultSubmitTime = now;

//                                if (task.SubItems != null && task.SubItems.Count > 0)
//                                {
//                                    foreach (SubItemDto subitem in task.SubItems)
//                                    {
//                                        subitem.TestResult = subitem.Temp_TestResult;
//                                    }
//                                    await _subItemService.UpdateRangeAsync(new List<SubItemDto>(task.SubItems));
//                                }


//                                TaskDatasSource.Remove(task);
//                                ItemsSource.Remove(task);
//                            }
//                            await _itemService.UpdateRangeAsync(tasks.ToList());

//                            var samples=tasks.Select(i=>i.Sample).DistinctBy(s=>s.SampleCode);

//                            foreach (var sample in samples)
//                                await  UpdateSampleTestProgress(sample);    
//                            foreach (var sample in samples)
//                                await  ExcuteIfNullSample(SamplesSource.FirstOrDefault(s=>s.SampleCode==sample.SampleCode),TaskDatasSource.Where(i=>i.SampleCode==sample.SampleCode));
//                        })},
//                        new UICommand{Caption = "取消选择不可提交项",IsDefault = false,IsCancel = true,Command=new DelegateCommand(() =>
//                        {
//                           SelectedTaskDatas=source;
//                        })},
//                            new UICommand{Caption = "取消",IsDefault = false,IsCancel = true},
//                        }
//                        , "", this);
//                }
//                else
//                {
//                    _messageBoxService.ShowMessage("不存在结果不为空的选中项！");
//                }
//            }
//            catch (Exception)
//            {
//                throw;
//            }
//        }

//        [Command]
//        public void EditTestRemark()
//        {
//            InputBoxText = FocusedItem.TestRemark;
//            var dialogService = GetService<IDialogService>("InPutBoxViewDialogService");
//            dialogService.ShowDialog(
//                new List<UICommand> {
//                        new UICommand{Caption = "确认",IsDefault = false,IsCancel = false,Command = new DelegateCommand(async () =>
//                        {
//                            FocusedItem.TestRemark=string.IsNullOrEmpty(InputBoxText)?null:InputBoxText;
//                            await _itemService.UpdateAsync(FocusedItem);
//                        })},
//                        new UICommand{Caption = "取消",IsDefault = false,IsCancel = true,}
//                }
//                , "", this);
//        }


//        public List<ItemDto> SubmitableItems
//        {
//            get; set;
//        }

//        /// <summary>
//        /// 提交样品窗口
//        /// </summary>
//        [Command]
//        public void SubmitSample()
//        {
//            try
//            {
//                SubmitableItems = ItemsSource.Where(i => !string.IsNullOrWhiteSpace(i.Temp_TestResult)).ToList();

//                if (SubmitableItems != null && SubmitableItems.Count > 0)
//                {
//                    IDialogService dialogService = GetService<IDialogService>("MyItemsOfSamplePreviewDialogService");
//                    dialogService.ShowDialog(
//                        new List<UICommand> {
//                        new UICommand{Caption = "确认",IsDefault = true,IsCancel = false,Command = new DelegateCommand(async () =>
//                        {
//                            //ItemDto[] items = new ItemDto[SubmitableItems.Count];
//                            //SubmitableItems.CopyTo(items, 0);

//                            SampleDto editingSample = FocusedSample;

//                            //var itemInfo = editingSample.ItemInfo;

//                            //List<string> infos = oldinfo.Split("|").ToList();
//                            DateTimeOffset now = DateTimeOffset.Now;
//                            foreach (ItemDto item in SubmitableItems)
//                            {
//                                item.TestResult = item.Temp_TestResult;
//                                item.SingleConclusion=item.Temp_SingleConclusion;
//                                int newProgress = item.PreTestProgress == (int)TestProgress.无 ? (int)TestProgress.数据一审 : item.PreTestProgress;
//                                item.TestProgress = newProgress;
//                                item.PreTestProgress = (int)TestProgress.无;
//                                item.ResultSubmitTime = now;

//                                if (item.SubItems != null && item.SubItems.Count > 0)
//                                {
//                                    foreach (SubItemDto subitem in item.SubItems)
//                                    {
//                                        subitem.TestResult = subitem.Temp_TestResult;
//                                    }
//                                    await _subItemService.UpdateRangeAsync(new List<SubItemDto>(item.SubItems));
//                                }

//                                TaskDatasSource.Remove(TaskDatasSource.FirstOrDefault(t => t.ItemId == item.ItemId));
//                                ItemsSource.Remove(ItemsSource.FirstOrDefault(i=>i.ItemId==item.ItemId));
//                            }
//                            await _itemService.UpdateRangeAsync(SubmitableItems);
//                            await  ExcuteIfNullSample(editingSample,TaskDatasSource.Where(i=>i.SampleCode==editingSample.SampleCode));

//                            await ShowNotifaction(editingSample.SampleCode + "  " + editingSample.SampleName, "提交成功！", "");
//                            //UpdateGrid();
//                        })},
//                        new UICommand{Caption = "取消",IsDefault = false,IsCancel = true,}
//                        }
//                        , "", this);
//                }
//            }
//            catch (Exception)
//            {
//                // throw;
//            }
//        }

//        /// <summary>
//        /// 提交单个结果
//        /// </summary>
//        [Command]
//        public async Task SubmitData(ItemDto item)
//        {
//            if (item == null)
//                return;
//            SampleDto editingSample = SamplesSource.FirstOrDefault(s => s.SampleCode == item.SampleCode);
//            if (editingSample == null)
//                return;
//            int newProgress = item.PreTestProgress == (int)TestProgress.无 ? (int)TestProgress.数据一审 : item.PreTestProgress;

//            item.TestResult = item.Temp_TestResult;
//            item.SingleConclusion = item.Temp_SingleConclusion;
//            item.TestProgress = newProgress;
//            item.ResultSubmitTime = DateTimeOffset.Now;
//            TaskDatasSource.Remove(item);
//            ItemsSource.Remove(item);
//            await _itemService.UpdateAsync(item);
//            await ExcuteIfNullSample(editingSample, TaskDatasSource.Where(i => i.SampleCode == editingSample.SampleCode));

//            if (item.SubItems != null && item.SubItems.Count > 0)
//            {
//                foreach (var s in item.SubItems)
//                    s.TestResult = s.Temp_TestResult;
//                await _subItemService.UpdateRangeAsync(item.SubItems.ToList());
//            }
//            await ShowNotifaction("", "提交成功！", "");
//        }

//        /// <summary>
//        /// 变更项目信息
//        /// </summary>
//        /// <param name="item"></param>
//        [Command]
//        public void ChangeItemInfo(ItemDto item)
//        {
//            EdittingItem = (ItemDto)item.Clone();

//            IDialogService dialogService = GetService<IDialogService>("EditItemInfoViewDialogService");
//            dialogService.ShowDialog(
//                new List<UICommand> {
//                        new UICommand{Caption = "变更项目信息",IsDefault = true,IsCancel = false,Command = new DelegateCommand( async () =>
//                        {
//                            await _itemService.UpdateAsync(EdittingItem);

//                            TaskDatasSource[TaskDatasSource.FindTaskDataIndex(item)]=(await _itemService.GetSingleAsync(item.ItemId)).Result;
//                            ItemsSource[ItemsSource.IndexOf(item)]=EdittingItem;

//                            await ShowNotifaction(EdittingItem.SampleCode + "  " + EdittingItem.TestItem, "变更项目信息成功！", "");
//                        })},
//                        new UICommand{Caption = "取消",IsDefault = false,IsCancel = true,}
//                }
//                , "", this);
//        }

//        /// <summary>
//        /// 变更项目分析人
//        /// </summary>
//        /// <param name="item"></param>
//        [Command]
//        public void ChangeItemTester(ItemDto item)
//        {
//            EdittingItem = item;
//            IDialogService dialogService = GetService<IDialogService>("ChangeItemTesterDialogService");
//            dialogService.ShowDialog(
//                new List<UICommand> {
//                        new UICommand{Caption = "变更分析人",IsDefault = true,IsCancel = false,Command = new DelegateCommand(async () =>
//                        {
//                            if (EditedUser != null && EdittingItem.Tester != EditedUser.UserName){
//                                SampleDto editingSample = FocusedSample;
//                                int newProgress = (int)TestProgress.待领取;

//                                EdittingItem.Tester = EditedUser.UserName;
//                                EdittingItem.TestProgress = newProgress;
//                                EdittingItem.Temp_TestResult=string.Empty;
//                                EdittingItem.TestResult=string.Empty;
//                                await _itemService.UpdateAsync(EdittingItem);

//                                await _loggerService.CreateAsync(new LoggerDto(DateTimeOffset.Now)
//                                {
//                                    LogLevel=LogLevel.WARN,
//                                    ActionType=ActionType.变更项目分析人,
//                                    PublisherIP=LoggerDto.GetLocalIP(),
//                                    PublisherName=CurrentUser.UserName,
//                                    ReceiverName=EditedUser.UserName,
//                                    SampleCode=item.SampleCode,
//                                    TestItem=item.TestItem,
//                                    Message="变更项目分析人",
//                                });

//                                TaskDatasSource.Remove(EdittingItem);
//                                ItemsSource.Remove(item);
//                                if (editingSample != null)
//                                {
//                                    if (ItemsSource.Count==0)
//                                    {
//                                        pre_MyFocusedSampelRowIndex = FocusedSampleRowHandle;
//                                        SamplesSource.Remove(editingSample);
//                                        FocusedSampleRowHandle = pre_MyFocusedSampelRowIndex;
//                                    }
//                                    else
//                                    {
//                                        var response=await _itemService.GetAllItemsBySampleCodeAsync(new Common.Parameters.ItemFilterParam(editingSample.SampleCode));
//                                        if (response .Status)
//                                        {
//                                            editingSample.Items = response.Result.ToObservableCollection();
//                                            FocusedSample = editingSample;
//                                        }
//                                    }
//                                }
//                                await ShowNotifaction(EdittingItem.SampleCode + "  " + EdittingItem.TestItem, "变更分析人成功！", "");
//                            }
//                        })},
//                        new UICommand{Caption = "取消",IsDefault = false,IsCancel = true,}
//                }
//                , "", this);
//        }

//        public int SelectedTesterIndex
//        {
//            get; set;
//        }

//        /// <summary>
//        /// 双击检测项目事件
//        /// </summary>
//        /// <param name="args"></param>
//        [Command]
//        public void EditedItemRowDoubleClick(RowClickArgs args)
//        {
//            if (args.Item is ItemDto item && item != null && item.SubItems.Count > 0)
//            {
//                PopupMyEditedSubItemsView(item);
//            }
//        }
//        /// <summary>
//        /// 弹出编辑子项目窗口
//        /// </summary>
//        /// <param name="item"></param>
//        [Command]
//        public void PopupMyEditedSubItemsView(ItemDto item)
//        {
//            SubItemViewColumns = GetEditedSubItemViewColumns();
//            EdittingItem = item;
//            IDialogService dialogService = GetService<IDialogService>("MyEdiatbleSubItemsViewDialogService");
//            dialogService.ShowDialog(
//                new List<UICommand> {
//                    AddResultCountToItem(item),
//                    new UICommand{Caption = "关闭",IsDefault = false,IsCancel = true,}
//                }
//                , "", this);
//        }
//        /// <summary>
//        /// 填充子项目结果加和到父项目结果栏
//        /// </summary>
//        /// <param name="item"></param>
//        /// <returns></returns>
//        private UICommand AddResultCountToItem(ItemDto item)
//        {

//            SampleDto editingSample = SamplesSource?.FirstOrDefault(s => s.SampleCode == item.SampleCode);
//            return new UICommand
//            {
//                Caption = "填充加和到父项目",
//                IsDefault = false,
//                IsCancel = false,
//                Command = new DelegateCommand(async () =>
//                {
//                    double result = 0;

//                    foreach (SubItemDto subItem in item?.SubItems)
//                    {
//                        if (IsNumeric(subItem?.Temp_TestResult))
//                        {
//                            result += Convert.ToDouble(subItem.Temp_TestResult);
//                        }
//                    }
//                    var oldResult = result == 0 ? string.Empty : result.ToString();
//                    IsNeedNumericalRevision(ref oldResult, item?.RoundRule);

//                    item.Temp_TestResult = oldResult.ToString();
//                    item.Temp_SingleConclusion = Judgement(item?.IndexRequest, item.Temp_TestResult);
//                    await _itemService.UpdateAsync(item);
//                    await ExcuteIfNullSample(editingSample, TaskDatasSource?.Where(i => i.SampleCode == editingSample.SampleCode));

//                })
//            };
//        }
//        protected List<BindingColumn> GetEditedSubItemViewColumns()
//        {
//            return new List<BindingColumn>
//            {
//                new BindingColumn(SettingsType.Binding,"TestItem", "检测项目"),
//                //new BindingColumn(SettingsType.Binding,"ReportUnit","报告单位"),
//                new BindingColumn(SettingsType.Binding,"Temp_TestResult", "检测结果"){ReadOnly=false},
//                new BindingColumn(SettingsType.Binding,"IndexRequest", "指标要求"),
//                new BindingColumn(SettingsType.Binding,"ItemRemark", "项目备注") ,
//            };
//        }
//        protected override void ShowItemsOfFocusedSample(SampleDto sample)
//        {
//            try
//            {
//                base.ShowItemsOfFocusedSample(sample);
//                // sample.Items = await GetAllItemsOfSample(sample);
//            }
//            catch (Exception)
//            {
//                //throw;
//            }
//        }
//        public override TestProgress GetRelativeProgress()
//        {
//            return TestProgress.检测中;
//        }

//        protected override void GetFormattingRules()
//        {
//            var sampleRules = new List<FormattingRule>();
//            sampleRules.Add(new FormattingRule()
//            {
//                Expression = "[IsUrgent]='True'",
//                ApplyToRow = true,
//                Type = FormattingType.Urgent
//            });
//            sampleRules.Add(new FormattingRule()
//            {
//                Expression = "[CurrentUrgent]='True'",
//                ApplyToRow = true,
//                Type = FormattingType.CurrentUrgent
//            });
//            SampleFormatConditionRules = sampleRules;

//            var taskListRules = new List<FormattingRule>();
//            taskListRules.Add(new FormattingRule()
//            {
//                Expression = "[Sample.IsUrgent]='True'",
//                ApplyToRow = true,
//                Type = FormattingType.Urgent
//            });
//            taskListRules.Add(new FormattingRule()
//            {
//                Expression = "[Sample.CurrentUrgent]='True'",
//                ApplyToRow = true,
//                Type = FormattingType.CurrentUrgent
//            });
//            taskListRules.Add(new FormattingRule()
//            {
//                Expression = "[Temp_SingleConclusion]='不符合'",
//                ApplyToRow = true,
//                Type = FormattingType.SingleConclusion
//            });

//            TaskListFormatConditionRules = taskListRules;
//        }





//        #region 单项判定   
//        /// <summary>
//        /// 判定字符串是否为数字
//        /// </summary>
//        /// <param name="value"></param>
//        /// <returns></returns>
//        private static bool IsNumeric(string value)
//        {
//            return !string.IsNullOrEmpty(value) && Regex.IsMatch(value, @"^[+-]?\d*[.]?\d*$");
//        }

//        private string Judgement(string judge, string value)
//        {

//            if (string.IsNullOrEmpty(judge) || string.IsNullOrEmpty(value))
//                return "/";

//            if (!string.IsNullOrEmpty(value) && IsNumeric(value))
//            {
//                if (Regex.IsMatch(judge, @"^[≤≥＜＞][+]?(0|([1-9]\d*))(\.\d+)?$"))
//                {
//                    judge = Regex.Replace(judge, @"^≤", "<=");
//                    judge = Regex.Replace(judge, @"^≥", ">=");
//                    judge = Regex.Replace(judge, @"^＜", "<");
//                    judge = Regex.Replace(judge, @"^＞", ">");
//                    DataTable dataTable = new();

//                    return (bool)dataTable.Compute(value + judge, "") ? "符合" : "不符合";

//                }
//                else if (Regex.IsMatch(judge, @"^[+]?(0|([1-9]\d*))(\.\d+)?[~～-][+]?(0|([1-9]\d*))(\.\d+)?$"))
//                {
//                    decimal min = Convert.ToDecimal(Regex.Match(judge, @"^[+]?(0|([1-9]\d*))(\.\d+)?").Value);
//                    decimal max = Convert.ToDecimal(Regex.Match(judge, @"[+]?(0|([1-9]\d*))(\.\d+)?$").Value);

//                    return Convert.ToDecimal(value) >= min && Convert.ToDecimal(value) <= max ? "符合" : "不符合";
//                }
//            }

//            return "/";

//        }
//        #endregion

//        #region 数值修约
//        private static bool NumericalRevision(ref string result, string rule)
//        {
//            double inputResult = Convert.ToDouble(result);
//            bool IsNeed;
//            string legalResult = string.Empty;
//            if (Regex.IsMatch(rule, @"[Dd][Nn][0-9]"))
//            {
//                int num = Convert.ToInt32(Regex.Match(rule, @"[0-9]").Value);
//                legalResult = Math.Round(inputResult, num,MidpointRounding.ToEven).ToString("F" + num);


//            }
//            else if (Regex.IsMatch(rule, "[Vv][Nn][0-9]"))
//            {
//                int num = Convert.ToInt32(Regex.Match(rule, @"[0-9]").Value);
//                legalResult = ToScientific(inputResult, num).ToString();
//            }
//            IsNeed = result != legalResult;
//            result = legalResult;
//            return IsNeed;
//        }
//        private static bool IsNeedNumericalRevision(ref string result, string rule)
//        {
//            if (IsNumeric(result) && !string.IsNullOrEmpty(result) && !string.IsNullOrEmpty(rule))
//            {
//                double inputResult = Convert.ToDouble(result);
//                if (Regex.IsMatch(rule, @"^[DdvV][Nn][0-9]$"))
//                {
//                    return NumericalRevision(ref result, rule);

//                }
//                else if (Regex.IsMatch(rule, @"^[iI][fF]\([a-zA-Z][>,<,=][+]?(0|([1-9]\d*))(\.\d+)?,[dDvV][nN][0-9],[dDvV][nN][0-9]\)$"))
//                {
//                    string condition = Regex.Match(rule, @"[a-zA-Z][>,<,=][+]?(0|([1-9]\d*))(\.\d+)?").Value;
//                    condition = Regex.Replace(condition, @"[a-zA-Z]", inputResult.ToString());
//                    DataTable dt = new();

//                    if ((bool)dt.Compute(condition, ""))
//                    {
//                        var rule1 = Regex.Match(rule, @",[dDvV][nN][0-9],").Value;

//                        return NumericalRevision(ref result, rule1);
//                    }
//                    else
//                    {
//                        var rule2 = Regex.Match(rule, @",[dDvV][nN][0-9]\)$").Value;

//                        return NumericalRevision(ref result, rule2);
//                    }

//                }
//            }

//            return false;
//        }
//        /// <summary>
//        /// 科学计数法修约
//        /// </summary>
//        /// <param name="value"></param>
//        /// <param name="decimalDigits"></param>
//        /// <returns></returns>
//        private static string ToScientific(double value, int decimalDigits)
//        {

//            string format = "E" + decimalDigits;
//            return value.ToString(format, CultureInfo.InvariantCulture);

//            //int a;
//            //if (d == 0.0)
//            //    return "0";
//            //a = d > 1 || d < -1 ? n - (int)Math.Log10(Math.Abs(d)) - 1 : n + (int)Math.Log10(1.0 / Math.Abs(d));
//            //return a < 0 ? string.Format("{0:E" + (n - 1) + "}", d) : Math.Round(d, a).ToString("F" + a);
//        }
//        #endregion

//        #region 从剪切板
//        private static bool GetDataFromClipboard(ref string[] strArr)
//        {
//            string str = Clipboard.GetText();
//            if (!string.IsNullOrEmpty(str))
//            {
//                try
//                {
//                    var ary = Regex.Split(str.Trim(), @"\s{1,}");
//                    ary.ForEach(x => x = x.Trim());
//                    strArr = ary;
//                    return true;
//                }
//                catch (Exception)
//                {
//                    return false;
//                }
//            }
//            return false;

//        }

//        public ObservableCollection<SubItemDto> SelectedEditableSubItems { get; set; } = new();

//        private bool isJudgeLimitValue;

//        public bool IsJudgeLimitValue
//        {
//            get { return isJudgeLimitValue; }
//            set { isJudgeLimitValue = value; RaisePropertyChanged(nameof(IsJudgeLimitValue)); }
//        }

//        private string limitValue = "0";

//        public string LimitValue
//        {
//            get { return limitValue; }
//            set { limitValue = value; RaisePropertyChanged(nameof(LimitValue)); }
//        }

//        private string overrunExpress = "未检出";
//        public string OverrunExpress
//        {
//            get { return overrunExpress; }
//            set { overrunExpress = value; RaisePropertyChanged(nameof(OverrunExpress)); }
//        }


//        /// <summary>
//        /// 从剪切板上粘贴数据到结果栏
//        /// </summary>
//        /// <returns></returns>
//        [Command]
//        public async Task PasteSubItemTempResultFromClipboard()
//        {
//            if (_messageBoxService.ShowMessage("确认将剪切板上的内容粘贴至检测结果栏?", "确认粘贴?", MessageButton.OKCancel, MessageIcon.Question, MessageResult.Cancel) != MessageResult.OK)
//                return;
//            string[] strAry = Array.Empty<string>();
//            var result = GetDataFromClipboard(ref strAry);
//            if (!result)
//            {
//                _messageBoxService.ShowMessage("剪切板为空！");
//                return;
//            }
//            if (strAry.Length != SelectedEditableSubItems.Count)
//            {
//                if (_messageBoxService.ShowMessage("剪切板上数据数量与勾选行数不一致，是否继续？", "警告", MessageButton.OKCancel, MessageIcon.Warning, MessageResult.Cancel) != MessageResult.OK)
//                    return;
//            }

//            for (int i = 0; i < SelectedEditableSubItems.Count; i++)
//            {
//                if (i == strAry.Length)
//                    return;
//                var subItem = SelectedEditableSubItems[i];
//                if (isJudgeLimitValue)
//                    if (IsNumeric(strAry[i].Trim()))
//                    {
//                        subItem.Temp_TestResult = Convert.ToDecimal(strAry[i].Trim()) <= Convert.ToDecimal(limitValue) ? overrunExpress : strAry[i].Trim();
//                        await _subItemService.UpdateAsync(subItem);
//                        continue;
//                    }

//                subItem.Temp_TestResult = strAry[i].Trim();
//                await _subItemService.UpdateAsync(subItem);
//            }

//        }
//        [Command]
//        public void PasteItemTempResultFromClipboard()
//        {
//            if (_messageBoxService.ShowMessage("确认将剪切板上的内容粘贴至检测结果栏?", "确认粘贴?", MessageButton.OKCancel, MessageIcon.Question, MessageResult.Cancel) != MessageResult.OK)
//                return;


//            string[] strAry = Array.Empty<string>();
//            var result = GetDataFromClipboard(ref strAry);
//            if (!result)
//            {
//                _messageBoxService.ShowMessage("剪切板为空！");
//                return;
//            }

//            if (strAry.Length != selectedTaskDatas.Count)
//            {
//                if (_messageBoxService.ShowMessage("剪切板上数据数量与勾选行数不一致，是否继续？", "警告", MessageButton.OKCancel, MessageIcon.Warning, MessageResult.Cancel) != MessageResult.OK)
//                    return;
//            }

//            for (int i = 0; i < selectedTaskDatas.Count; i++)
//            {
//                if (i == strAry.Length)
//                    return;
//                var item = selectedTaskDatas[i];
//                item.Temp_TestResult = strAry[i].Trim();
//                ItemTempResultChanged(item, item.Temp_TestResult);
//                //await _itemService.UpdateAsync(item);
//            }
//            _messageBoxService.ShowMessage("操作完成！");
//        }

//        [Command]
//        public async void ClearTempResultColumn()
//        {
//            foreach (var s in SelectedEditableSubItems)
//            {
//                s.Temp_TestResult = string.Empty;
//                await _subItemService.UpdateAsync(s);
//            }

//        }
//        [Command]
//        public async void ClearItemResultColumn()
//        {
//            foreach (var s in selectedTaskDatas)
//            {
//                s.Temp_TestResult = string.Empty;
//                await _itemService.UpdateAsync(s);
//            }
//        }


//        #endregion

//        private bool allowAutoRounding;

//        public bool AllowAutoRounding
//        {
//            get
//            {
//                return allowAutoRounding;
//            }
//            set
//            {
//                allowAutoRounding = value;
//                RaisePropertyChanged(nameof(AllowAutoRounding));
//                cfa.AppSettings.Settings["AllowAutoRounding"].Value = value.ToString();
//                cfa.Save();
//            }
//        }








//    }
//}
