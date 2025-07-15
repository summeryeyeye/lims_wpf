using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.POCO;
using DevExpress.Mvvm.Xpf;
using DevExpress.Xpf.Editors.Helpers;
using Lims.Common.Dtos;
using Lims.Common.Parameters;
using Lims.ToolsForClient;
using Lims.ToolsForClient.Extensions;
using Lims.WPF.Resources;
using MathNet.Numerics;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace Lims.WPF.ViewModels
{
    public class MyTestingTasksViewModel : MyTasksViewModelBase
    {
        public static MyTestingTasksViewModel Create(string caption)
        {
            return ViewModelSource.Create(() => new MyTestingTasksViewModel()
            {
                Caption = caption,
            });
        }

        public MyTestingTasksViewModel()
        {
            AllowAutoRounding = Convert.ToBoolean(cfa.AppSettings.Settings["AllowAutoRounding"].Value);
        }

        protected override async Task LoadMainDatas(UserDto? user)
        {
            ShowMainDatasLoadingPanel = true;
            pre_MyFocusedSampelRowIndex = FocusedSampleRowHandle;
            TaskDatasSource=new ObservableCollection<ItemDto>();
            SamplesSource=new ObservableCollection<SampleDto?>();

            ItemFilterParam itemFilterParam = new ItemFilterParam()
            {
                TestProgress = (int)RelativeProgress,
                Tester = user.UserName,
                Operation = Operation.Equal,
            };
            var response = await _itemService.GetMyItemsAsync(itemFilterParam);
            if (response.Status)
            {
                TaskDatasSource = response.Result.OrderBy(t => t.AppointTime).Where(i => !i.IsOverDate)
                    .ToObservableCollection();
                SamplesSource = TaskDatasSource.Select(i => i.Sample).DistinctBy(s => s.SampleCode)
                    .ToObservableCollection();
            }

            FocusedSampleRowHandle = pre_MyFocusedSampelRowIndex;
            ShowMainDatasLoadingPanel = false;
        }



        /// <summary>
        /// 提交任务窗口
        /// </summary>
        [Command]
        public Task SubmitTaskList()
        {
            try
            {
                ObservableCollection<ItemDto> source = selectedTaskDatas
                    .Where(i => !string.IsNullOrWhiteSpace(i.Temp_TestResult)).ToObservableCollection();

                TaskListPreviewSources = source;
                if (TaskListPreviewSources != null && TaskListPreviewSources.Count > 0)
                {
                    ItemDto[] tasks = new ItemDto[TaskListPreviewSources.Count];
                    TaskListPreviewSources.CopyTo(tasks, 0);
                    DateTimeOffset now = DateTimeOffset.Now;
                    IDialogService dialogService =
                        GetService<IDialogService>("MySubmittedTaskListPreviewDialogService");
                    _ = dialogService.ShowDialog(
                        new List<UICommand>
                        {
                            new UICommand
                            {
                                Caption = "确认提交", IsDefault = true, IsCancel = false, Command =
                                    new DelegateCommand(async () =>
                                    {
                                        foreach (var task in tasks)
                                        {
                                            SampleDto editingSample =
                                                SamplesSource.FirstOrDefault(s => s.SampleCode == task.SampleCode);
                                            int newProgress = task.PreTestProgress == (int)TestProgress.无
                                                ? (int)TestProgress.数据一审
                                                : task.PreTestProgress;
                                            task.TestResult = task.Temp_TestResult;
                                            task.SingleConclusion = task.Temp_SingleConclusion;
                                            task.TestProgress = newProgress;
                                            task.PreTestProgress = (int)TestProgress.无;
                                            task.ResultSubmitTime = now;

                                            if (task.SubItems != null && task.SubItems.Count > 0)
                                            {
                                                foreach (SubItemDto subitem in task.SubItems)
                                                {
                                                    subitem.TestResult = subitem.Temp_TestResult;
                                                }

                                                await _subItemService.UpdateRangeAsync(
                                                    new List<SubItemDto>(task.SubItems));
                                            }


                                            TaskDatasSource.Remove(task);
                                            ItemsSource.Remove(task);
                                            //SamplesSource.FirstOrDefault(s=>s.SampleCode==task.SampleCode).Items.Remove(SamplesSource.FirstOrDefault(s=>s.SampleCode==task.SampleCode).Items.FirstOrDefault(i=>i.ItemId==task.ItemId));
                                        }

                                        await _itemService.UpdateRangeAsync(tasks.ToList());

                                        var samples = tasks.Select(i => i.Sample).DistinctBy(s => s.SampleCode);
                                        foreach (var sample in samples)
                                        {
                                            //await  UpdateSampleTestProgress(sample);
                                            await ExcuteIfNullSample(
                                                SamplesSource.FirstOrDefault(s => s.SampleCode == sample.SampleCode),
                                                TaskDatasSource.Where(i => i.SampleCode == sample.SampleCode));
                                        }
                                    })
                            },
                            new UICommand
                            {
                                Caption = "取消选择不可提交项", IsDefault = false, IsCancel = true,
                                Command = new DelegateCommand(() => { SelectedTaskDatas = source; })
                            },
                            new UICommand { Caption = "取消", IsDefault = false, IsCancel = true },
                        }
                        , "", this);
                }
                else
                {
                    _messageBoxService.ShowMessage("不存在结果不为空的选中项！");
                }
            }
            catch (Exception)
            {
                throw;
            }

            return Task.CompletedTask;
        }

        [Command]
        public void EditTestRemark()
        {
            InputBoxText = FocusedItem.TestRemark;
            var dialogService = GetService<IDialogService>("InPutBoxViewDialogService");
            dialogService.ShowDialog(
                new List<UICommand>
                {
                    new UICommand
                    {
                        Caption = "确认", IsDefault = false, IsCancel = false, Command = new DelegateCommand(async () =>
                        {
                            FocusedItem.TestRemark = string.IsNullOrEmpty(InputBoxText) ? null : InputBoxText;
                            await _itemService.UpdateAsync(FocusedItem);
                        })
                    },
                    new UICommand { Caption = "取消", IsDefault = false, IsCancel = true, }
                }
                , "", this);
        }


        public List<ItemDto> SubmitableItems { get; set; }

        /// <summary>
        /// 提交样品窗口
        /// </summary>
        [Command]
        public void SubmitSample()
        {
            try
            {
                SubmitableItems = ItemsSource.Where(i => !string.IsNullOrWhiteSpace(i.Temp_TestResult)).ToList();

                if (SubmitableItems != null && SubmitableItems.Count > 0)
                {
                    IDialogService dialogService = GetService<IDialogService>("MyItemsOfSamplePreviewDialogService");
                    dialogService.ShowDialog(
                        new List<UICommand>
                        {
                            new UICommand
                            {
                                Caption = "确认", IsDefault = true, IsCancel = false, Command =
                                    new DelegateCommand(async () =>
                                    {
                                        //ItemDto[] items = new ItemDto[SubmitableItems.Count];
                                        //SubmitableItems.CopyTo(items, 0);

                                        SampleDto editingSample = FocusedSample;

                                        //var itemInfo = editingSample.ItemInfo;

                                        //List<string> infos = oldinfo.Split("|").ToList();
                                        DateTimeOffset now = DateTimeOffset.Now;
                                        foreach (ItemDto item in SubmitableItems)
                                        {
                                            item.TestResult = item.Temp_TestResult;
                                            item.SingleConclusion = item.Temp_SingleConclusion;
                                            int newProgress = item.PreTestProgress == (int)TestProgress.无
                                                ? (int)TestProgress.数据一审
                                                : item.PreTestProgress;
                                            item.TestProgress = newProgress;
                                            item.PreTestProgress = (int)TestProgress.无;
                                            item.ResultSubmitTime = now;

                                            if (item.SubItems != null && item.SubItems.Count > 0)
                                            {
                                                foreach (SubItemDto subitem in item.SubItems)
                                                {
                                                    subitem.TestResult = subitem.Temp_TestResult;
                                                }

                                                await _subItemService.UpdateRangeAsync(
                                                    new List<SubItemDto>(item.SubItems));
                                            }

                                            TaskDatasSource.Remove(
                                                TaskDatasSource.FirstOrDefault(t => t.ItemId == item.ItemId));
                                            ItemsSource.Remove(
                                                ItemsSource.FirstOrDefault(i => i.ItemId == item.ItemId));
                                        }

                                        await _itemService.UpdateRangeAsync(SubmitableItems);


                                        await ExcuteIfNullSample(editingSample,
                                            TaskDatasSource.Where(i => i.SampleCode == editingSample.SampleCode));

                                        await ShowNotifaction(
                                            editingSample.SampleCode + "  " + editingSample.SampleName, "提交成功！", "");
                                        //UpdateGrid();
                                    })
                            },
                            new UICommand { Caption = "取消", IsDefault = false, IsCancel = true, }
                        }
                        , "", this);
                }
            }
            catch (Exception)
            {
                // throw;
            }
        }

        /// <summary>
        /// 提交单个结果
        /// </summary>
        [Command]
        public async Task SubmitData(ItemDto item)
        {
            if (item == null)
                return;
            SampleDto editingSample = SamplesSource.FirstOrDefault(s => s.SampleCode == item.SampleCode);
            if (editingSample == null)
                return;
            int newProgress = item.PreTestProgress == (int)TestProgress.无
                ? (int)TestProgress.数据一审
                : item.PreTestProgress;

            item.TestResult = item.Temp_TestResult;
            item.SingleConclusion = item.Temp_SingleConclusion;
            item.TestProgress = newProgress;
            item.ResultSubmitTime = DateTimeOffset.Now;
            TaskDatasSource.Remove(item);
            ItemsSource.Remove(item);
            await _itemService.UpdateAsync(item);
            await ExcuteIfNullSample(editingSample,
                TaskDatasSource.Where(i => i.SampleCode == editingSample.SampleCode));

            if (item.SubItems != null && item.SubItems.Count > 0)
            {
                foreach (var s in item.SubItems)
                    s.TestResult = s.Temp_TestResult;

                await _subItemService.UpdateRangeAsync(item.SubItems.ToList());
            }

            await ShowNotifaction("", "提交成功！", "");
        }

        /// <summary>
        /// 变更项目信息
        /// </summary>
        /// <param name="item"></param>
        [Command]
        public void ChangeItemInfo(ItemDto item)
        {
            EdittingItem = (ItemDto)item.Clone();

            IDialogService dialogService = GetService<IDialogService>("EditItemInfoViewDialogService");
            dialogService.ShowDialog(
                new List<UICommand>
                {
                    new UICommand
                    {
                        Caption = "变更项目信息", IsDefault = true, IsCancel = false, Command =
                            new DelegateCommand(async () =>
                            {
                                await _itemService.UpdateAsync(EdittingItem);

                                TaskDatasSource[TaskDatasSource.FindTaskDataIndex(item)] =
                                    (await _itemService.GetSingleAsync(item.ItemId)).Result;
                                ItemsSource[ItemsSource.IndexOf(item)] = EdittingItem;

                                await ShowNotifaction(EdittingItem.SampleCode + "  " + EdittingItem.TestItem,
                                    "变更项目信息成功！", "");
                            })
                    },
                    new UICommand { Caption = "取消", IsDefault = false, IsCancel = true, }
                }
                , "", this);
        }

        /// <summary>
        /// 变更项目分析人
        /// </summary>
        /// <param name="item"></param>
        [Command]
        public void ChangeItemTester(ItemDto item)
        {
            EdittingItem = item;
            IDialogService dialogService = GetService<IDialogService>("ChangeItemTesterDialogService");
            dialogService.ShowDialog(
                new List<UICommand>
                {
                    new UICommand
                    {
                        Caption = "变更分析人", IsDefault = true, IsCancel = false, Command = new DelegateCommand(async () =>
                        {
                            if (EditedUser != null && EdittingItem.Tester != EditedUser.UserName)
                            {
                                SampleDto editingSample = FocusedSample;
                                int newProgress = (int)TestProgress.待领取;

                                EdittingItem.Tester = EditedUser.UserName;
                                EdittingItem.TestProgress = newProgress;
                                EdittingItem.Temp_TestResult = string.Empty;
                                EdittingItem.TestResult = string.Empty;
                                await _itemService.UpdateAsync(EdittingItem);

                                await _loggerService.CreateAsync(new LoggerDto(DateTimeOffset.Now)
                                {
                                    LogLevel = LogLevel.WARN,
                                    ActionType = ActionType.变更项目分析人,
                                    PublisherIP = LoggerDto.GetLocalIP(),
                                    PublisherName = CurrentUser.UserName,
                                    ReceiverName = EditedUser.UserName,
                                    SampleCode = item.SampleCode,
                                    TestItem = item.TestItem,
                                    Message = "变更项目分析人",
                                });

                                TaskDatasSource.Remove(EdittingItem);
                                ItemsSource.Remove(item);
                                if (editingSample != null)
                                {
                                    if (ItemsSource.Count == 0)
                                    {
                                        pre_MyFocusedSampelRowIndex = FocusedSampleRowHandle;
                                        SamplesSource.Remove(editingSample);
                                        FocusedSampleRowHandle = pre_MyFocusedSampelRowIndex;
                                    }
                                    else
                                    {
                                        var response =
                                            await _itemService.GetAllItemsBySampleCodeAsync(
                                                new Common.Parameters.ItemFilterParam(editingSample.SampleCode));
                                        if (response.Status)
                                        {
                                            editingSample.Items = response.Result.ToObservableCollection();
                                            FocusedSample = editingSample;
                                        }
                                    }
                                }

                                await ShowNotifaction(EdittingItem.SampleCode + "  " + EdittingItem.TestItem,
                                    "变更分析人成功！", "");
                            }
                        })
                    },
                    new UICommand { Caption = "取消", IsDefault = false, IsCancel = true, }
                }
                , "", this);
        }

        public int SelectedTesterIndex { get; set; }

        protected override async Task CaculateSubTestResultAverage(ItemDto item)
        {
            try
            {
                if (SelectedEditableSubItems.Count > 0)
                {
                    ItemDto DensityItem = (await _itemService.GetFirstItemBySampleCodeAndKeyItemAsync(new ItemFilterParam(item.SampleCode) { KeyItem = "密度" })).Result;
                    foreach (var subItem in SelectedEditableSubItems)
                    {
                        if (IsNumeric(subItem.FirstTestResult) && IsNumeric(subItem.SecondTestResult))
                        {
                            var res = new decimal[] { subItem.FirstTestResult.TryConvertToDecimal(), subItem.SecondTestResult.TryConvertToDecimal() };

                            var ave = Math.Round(res.Average(), 2, MidpointRounding.ToEven);

                            subItem.AverageTestResult = ave.ToString();

                            if (DensityItem != null)
                            {
                                var densityContent = DensityItem?.TestResult;
                                if (IsNumeric(densityContent))
                                {
                                    var density = densityContent.TryConvertToDecimal();
                                    if (density > 0)
                                    {
                                        subItem.Temp_TestResult = Math.Round(ave * 10 * density, 1, MidpointRounding.ToEven).ToString();
                                    }                                   
                                    //await _subItemService.UpdateAsync(subItem);
                                    //continue;
                                }
                            }
                            else
                            {
                                subItem.Temp_TestResult = ave.ToString();

                            }
                            await _subItemService.UpdateAsync(subItem);

                        }
                        else if (subItem.FirstTestResult == "未检出" && subItem.SecondTestResult == "未检出")
                        {

                            subItem.AverageTestResult = "未检出";
                            subItem.Temp_TestResult = "未检出";

                            await _subItemService.UpdateAsync(subItem);
                        }
                        else if ((subItem.FirstTestResult == "未检出" || subItem.SecondTestResult == "未检出") && subItem.FirstTestResult != subItem.SecondTestResult)
                        {
                            _messageBoxService.ShowMessage("结果不平行！");
                            return;

                        }
                        else
                        {
                            subItem.AverageTestResult = "";
                            subItem.Temp_TestResult = "";

                            await _subItemService.UpdateAsync(subItem);

                        }
                    }

                }
                else
                {
                    _messageBoxService.ShowMessage("请勾选需要操作的数据后操作！");
                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }

        }

        /// <summary>
        /// 同步项目检测结果
        /// </summary>
        /// <param name="e"></param>
        [Command]
        public void Temp_TestItemResultChanged(CellValueChangedArgs e)
        {
            try
            {
                string oldValue = e.Value == null ? string.Empty : e.Value.ToString();
                var editingItem = e.Item as ItemDto;
                ItemTempResultChanged(editingItem, oldValue);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [Command]
        public void SwitchAutoRounding()
        {
            AllowAutoRounding = !AllowAutoRounding;
            if (AllowAutoRounding)
                showNotifaction("自动修约已开启！");
            else
                showNotifaction("自动修约已关闭！");
        }


        protected override void ShowItemsOfFocusedSample(SampleDto sample)
        {
            try
            {
                base.ShowItemsOfFocusedSample(sample);
                // sample.Items = await GetAllItemsOfSample(sample);
            }
            catch (Exception)
            {
                //throw;
            }
        }

        public override TestProgress GetRelativeProgress()
        {
            return TestProgress.检测中;
        }

        protected override void GetFormattingRules()
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
                Expression = "[Temp_SingleConclusion]='不符合'",
                ApplyToRow = true,
                Type = FormattingType.SingleConclusion
            });

            TaskListFormatConditionRules = taskListRules;
        }



    }
}
