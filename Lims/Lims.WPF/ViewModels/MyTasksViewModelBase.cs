
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.Xpf;
using DevExpress.Utils.About;
using DevExpress.Xpf.Core;
using DevExpress.XtraPivotGrid.Data;
using Lims.Common.Dtos;
using Lims.Common.Parameters;
using Lims.ToolsForClient;
using Lims.ToolsForClient.Extensions;
using Lims.WPF.Tools;
using Spire.Doc;
using System.Data;
using System.Drawing.Printing;
using System.IO;
using System.Text;
using System.Windows.Controls;
using System.Windows.Forms;
using Task = System.Threading.Tasks.Task;
namespace Lims.WPF.ViewModels
{
    public abstract class MyTasksViewModelBase : SampleAndItemDataViewModelBase
    {
        protected override Task<ObservableCollection<ItemDto>> GetItemsSource(SampleDto sample)
        {
            return Task.FromResult(TaskDatasSource.Where(i => i.SampleCode == sample.SampleCode).OrderBy(i => i.ItemId).ToObservableCollection());
        }


        protected override async Task LoadMainDatas(UserDto? user)
        {
            ShowMainDatasLoadingPanel = true;
            pre_MyFocusedSampelRowIndex = FocusedSampleRowHandle;

            ItemFilterParam itemFilterParam = new ItemFilterParam()
            {
                TestProgress = (int)RelativeProgress,
                Tester = user.UserName,
                Operation = Operation.Equal,
            };
            var response = await _itemService.GetMyItemsAsync(itemFilterParam);
            if (response.Status)
            {
                TaskDatasSource = response.Result.OrderBy(t => t.AppointTime).ToObservableCollection();
                SamplesSource = TaskDatasSource.Select(i => i.Sample).DistinctBy(s => s?.SampleCode).ToObservableCollection();
            }
            FocusedSampleRowHandle = pre_MyFocusedSampelRowIndex;
            ShowMainDatasLoadingPanel = false;
        }



        protected override async void ShowItemsOfFocusedSample(SampleDto sample)
        {
            try
            {
                base.ShowItemsOfFocusedSample(sample);
                await GetMoistureAndDensity(sample);
            }
            catch (Exception)
            {
                // throw;
            }
        }

        public UserDto EditedUser
        {
            get; set;
        }

        private DateTimeOffset? selectedTestDate;

        public DateTimeOffset? SelectedTestDate
        {
            get
            {
                return selectedTestDate;
            }
            set
            {
                selectedTestDate = value;
                RaisePropertyChanged(nameof(SelectedTestDate));
            }
        }


        /// <summary>
        /// 批量同步检验日期
        /// </summary>
        /// <param name="selectedTasks"></param>
        /// <returns></returns>
        [Command]
        public Task MultiEditTestDate(ObservableCollection<ItemDto> selectedTasks)
        {
            selectedTestDate = selectedTasks[0].TestDate == null ? DateTime.Today : selectedTasks[0].TestDate;
            IDialogService dialogService = GetService<IDialogService>("UpdateTestDateViewDialogService");
            dialogService.ShowDialog(
                new List<UICommand> {
                        new UICommand{Caption = "同步检验日期",IsDefault = false,IsCancel = false,Command = new DelegateCommand(async () =>
                        {
                            DateTimeOffset? testDate = SelectedTestDate;
                            if (_messageBoxService.ShowMessage($"已选中 {selectedTasks.Count} 项,确认同步检验日期({testDate?.ToString("yyyy-MM-dd")})?", "", MessageButton.OKCancel, MessageIcon.Question, MessageResult.OK) == MessageResult.OK)
                            {
                                if (selectedTasks.Count > 200)
                                {
                                    _messageBoxService.ShowMessage("单次同步项目数量过多（不得超过200项），同步检测日期失败");
                                    return;
                                }
                                ItemDto[] tasks = new ItemDto[selectedTasks.Count];

                                selectedTasks.CopyTo(tasks, 0);

                                foreach (var task in tasks)


                                {
                                    task.TestDate = testDate;

                                    TaskDatasSource.FirstOrDefault(i => i.ItemId == task.ItemId).TestDate = testDate;

                                    selectedTasks.Remove(selectedTasks.FirstOrDefault(i => i.ItemId == task.ItemId));
                                }
                                var items = tasks.Cast<ItemDto>().ToList();
                                await _itemService.UpdateRangeAsync(items);
                                await ShowNotifaction("", "同步检测日期完成!", "");
                            }
                        })},
                        new UICommand{Caption = "取消",IsDefault = false,IsCancel = true,}
                }
                , "", this);
            return Task.CompletedTask;
        }

        /// <summary>
        /// 同步单个样检测日期
        /// </summary>
        /// <param name="focusedItem"></param>
        /// <returns></returns>
        [Command]
        public Task EditTestDate(ItemDto focusedItem)
        {
            selectedTestDate = focusedItem.TestDate == null ? DateTime.Today : focusedItem.TestDate;
            IDialogService dialogService = GetService<IDialogService>("UpdateTestDateViewDialogService");
            dialogService.ShowDialog(
                new List<UICommand> {
                        new UICommand{Caption = "同步检验日期",IsDefault = false,IsCancel = false,Command = new DelegateCommand(async () =>
                        {
                            var testDate = SelectedTestDate;
                            focusedItem.TestDate = testDate;

                            TaskDatasSource.FirstOrDefault(i => i.ItemId == focusedItem.ItemId).TestDate = testDate;

                            await _itemService.UpdateAsync(focusedItem);
                            await ShowNotifaction("", "同步检测日期完成!", "");

                        })},
                        new UICommand{Caption = "取消",IsDefault = false,IsCancel = true,}
                }
                , "", this);
            return Task.CompletedTask;
        }
        /// <summary>
        /// 批量变更分析人
        /// </summary>
        /// <param name="datas"></param>
        [Command]
        public Task MultiChangeItemTester(ObservableCollection<ItemDto> datas)
        {
            ItemDto[] items = new ItemDto[datas.Count];
            datas.CopyTo(items, 0);

            IDialogService dialogService = GetService<IDialogService>("ChangeItemTesterDialogService");
            dialogService.ShowDialog(
                new List<UICommand> {
                        new UICommand{Caption = "变更分析人",IsDefault = true,IsCancel = false,Command = new DelegateCommand(async () =>
                        {
                            bool result=false;
                            foreach (var item in items)
                            {
                                EdittingItem = item;

                                if (EditedUser != null && EdittingItem.Tester != EditedUser.UserName)
                                {
                                    int newProgress = (int)TestProgress.待领取;
                                    EdittingItem.Tester = EditedUser.UserName;
                                    EdittingItem.TestProgress = newProgress;
                                    EdittingItem.Temp_TestResult=string.Empty;
                                    EdittingItem.TestResult=string.Empty;

                                    await _itemService.UpdateAsync(EdittingItem);

                                    TaskDatasSource.Remove(item);


                                    result=true;
                                }
                            }
                            if (result)
                            {
                                await ShowNotifaction("", "变更分析人成功！", "");
                            }
                        })},
                        new UICommand{Caption = "取消",IsDefault = false,IsCancel = true,}
                }
                , "", this);
            return Task.CompletedTask;
        }


        /// <summary>
        /// 双击同步模板信息到预览栏
        /// </summary>
        [Command]
        public async void OriginalRecordTemplateRowDoubleClick(RowClickArgs args)
        {
            var value = args.Item as MethodStandardDto;
            TemplateInfo.FileName = value?.OriginalRecordTemplateFilePath;
            TemplateInfo.InstrumentsInfo = value?.Instruments;
            await ShowNotifaction("", "已将选中项模板信息同步到预览区!", "");
        }
        private RecordTemplate templateInfo = new();

        public RecordTemplate TemplateInfo
        {
            get
            {
                return templateInfo;
            }
            set
            {
                templateInfo = value;
                RaisePropertyChanged(nameof(TemplateInfo));
            }
        }
        #region 打印原始记录
        /// <summary>
        /// 打印单个原始记录
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [Command]
        public async Task PrintOriginalRecord(ItemDto item)
        {
            try
            {
                MethodStandardDto std = item.MethodStandard;
                // await OriginalRecordPrint(std, item);
                if (await OriginalRecordPrint(std, item))
                    await ShowNotifaction("", "打印成功!", "");
                //DXMessageBox.Show("打印成功！");
            }
            catch (Exception e)
            {
                DXMessageBox.Show(e.Message);
            }

        }

        /// <summary>
        /// 批量打印原始记录
        /// </summary>
        /// <returns></returns>
        [Command]
        public void TaskList_PrintOriginalRecord()
        {
            ItemDto[] itemModels = new ItemDto[selectedTaskDatas.Count];
            selectedTaskDatas.CopyTo(itemModels, 0);
            int count = selectedTaskDatas.Count;
            int succeedItemsCount = 0;

            var dialogService = GetService<IDialogService>("PrinterSelecterViewDialogService");
            dialogService.ShowDialog(
            new List<UICommand> {
                new UICommand{Caption = "打印",IsDefault = true,IsCancel = false,Command = new DelegateCommand(new Action(async () =>
                {
                    try
                {
                    var dic=new Dictionary<string,int>();
                    foreach (var item in itemModels)
                    {
                        MethodStandardDto std = item.MethodStandard;
                        if (std != null && !string.IsNullOrEmpty(std.OriginalRecordTemplateFilePath))
                        {
                            if (!string.IsNullOrEmpty(std.OriginalRecordGroup ))
                            {
                                if (dic.ContainsKey($"{item.SampleCode}-{std.OriginalRecordGroup}"))
                                {
                                        succeedItemsCount++;
                                        await MarkOriginalRecord(item, true);
                                        continue;
                                }
                            }

                            if (await OriginalRecordPrint(std, item,Printers[CurrentPrinterIndex].PrinterName))
                            {
                                dic[$"{item.SampleCode}-{std.OriginalRecordGroup}"] = 1;
                                succeedItemsCount++;
                            }

                        }
                    }
                }
                catch (Exception)
                {
                    DXMessageBox.Show("打印失败！");
                }
                finally
                {
                    DXMessageBox.Show($"共选中 {count} 项，打印成功 {succeedItemsCount} 项！");
                }
                }))},
                new UICommand{Caption = "取消",IsDefault = false,IsCancel = true,}
            }
            , "", this);
        }
        private async Task<bool> OriginalRecordPrint(MethodStandardDto std, ItemDto item, string printerName = "", int tableIndex = 0)
        {
            String[] fieldNames;

            List<string> fieldValues;

            ItemDto DensityItem = (await _itemService.GetFirstItemBySampleCodeAndKeyItemAsync(new ItemFilterParam(item.SampleCode) { KeyItem = "密度" })).Result;

            ReportModel reportModel = new ReportModel();
            reportModel.SampleCode = item.SampleCode;
            reportModel.TestMethod = item.TestMethod;
            reportModel.TestItem = item.TestItem;
            reportModel.TestResult = item.TestResult;
            reportModel.TestDate = item.TestDate?.ToString("yyyy-MM-dd");
            reportModel.FirstSampleWeight = item.FirstSampleWeight;
            reportModel.SecondSampleWeight = item.SecondSampleWeight;

            //reportModel.Instruments = item.Instruments;
            //StringBuilder subInstruments = new StringBuilder();
            var instrumentsInfo = std.Instruments;
            string myInstrumentsInfo = instrumentsInfo;

            if (!string.IsNullOrEmpty(instrumentsInfo) && instrumentsInfo.Contains('：'))
            {
                myInstrumentsInfo = instrumentsInfo.Split('|').FirstOrDefault(i => i.Contains(item.Tester)).Split('：')[1];

            }
            StringBuilder infoStr = new StringBuilder();
            #region 打印子项目
            if (std.IsPrintSubItem && item.SubItems != null && item.SubItems.Count > 0)
            {
                if (std.KeyItem.Contains("活菌数"))
                {
                    foreach (var subItem in item.SubItems)
                    {
                        DataRow dr = reportModel.DataTable.NewRow();

                        dr["Index"] = item.SubItems.IndexOf(subItem) + 1;
                        dr["SubItem"] = subItem.TestItem;
                        reportModel.DataTable.Rows.Add(dr);

                        var subRespone = await _subItemStandardService.GetSubItemStandardsBySubItemAsync(new SubItemStandardFilterParam() { Name = subItem.TestItem });
                        if (subRespone.Status)
                        {
                            var subStandard = subRespone.Result.FirstOrDefault();
                            if (subStandard != null)
                            {
                                var info = new List<string>
                                                {
                                                    subStandard.Substratum,
                                                    subStandard.BatchNumber,
                                                    subStandard.Campany
                                                };

                                infoStr.AppendLine($"{subStandard.SubitemName}({string.Join('-', info)})");
                                //subInstruments.Append(subStandard.Instruments);
                            }
                        }
                    }


                    List<string> subInstrumentInfos = new List<string>();

                    if (item.SubItems != null && item.SubItems.Count > 0)
                    {
                        foreach (var subItem in item.SubItems)
                        {
                            var subRespone = await _subItemStandardService.GetSubItemStandardsBySubItemAsync(new SubItemStandardFilterParam() { Name = subItem.TestItem });
                            if (subRespone.Status)
                            {
                                subInstrumentInfos.Add(subRespone.Result.FirstOrDefault().Instruments);
                            }
                        }
                    }

                    if (subInstrumentInfos.Count > 0)
                    {
                        var subInstrumentInfo = string.Join(",", subInstrumentInfos.Distinct());
                        myInstrumentsInfo = string.Join(',', myInstrumentsInfo, subInstrumentInfo);
                    }



                }
                else if (std.KeyItem.Contains("Aa"))
                {
                    try
                    {

                        if (DensityItem != null)
                        {
                            var densityContent = DensityItem.TestResult;
                            reportModel.Density = densityContent;
                        }


                        foreach (var subItem in item.SubItems)
                        {
                            DataRow dr = reportModel.DataTable.NewRow();

                            dr["Index"] = item.SubItems.IndexOf(subItem) + 1;
                            dr["SubItem"] = subItem.TestItem;
                            dr["FirstSubItemTestResult"] = subItem.FirstTestResult;
                            dr["SecondSubItemTestResult"] = subItem.SecondTestResult;
                            dr["AverageTestResult"] = subItem.AverageTestResult;
                            dr["SubItemTestResult"] = subItem.TestResult;

                            reportModel.DataTable.Rows.Add(dr);
                        }


                    }
                    catch (Exception ex)
                    {

                        throw new Exception(ex.Message);
                    }

                }
            }
            #endregion
            reportModel.Instruments = myInstrumentsInfo;
            reportModel.Info = infoStr.ToString();
            // reportModel.Instruments = std.Instruments;


            var properties = reportModel.GetType().GetProperties().Where(p => p.PropertyType.IsValueType || p.PropertyType.Name.StartsWith("String")).ToList();

            //反射获取属性名集合
            fieldNames = properties.Select(p => p.Name).ToArray();
            //fieldValues = new object[properties.Count];
            //反射获取属性值集合
            fieldValues = new List<string>();
            foreach (var p in properties.Select(p => p.GetValue(reportModel, null)))
            {
                fieldValues.Add(p == null ? string.Empty : p.ToString());
            }
            using (Document document = new Spire.Doc.Document())
            {
                document.LoadFromFile(std.OriginalRecordTemplateFilePath);
                document.MailMerge.Execute(fieldNames, fieldValues.ToArray());
                var dt = reportModel.DataTable;

                if (dt.Rows.Count > 0 && dt != null)
                {
                    document.MailMerge.ExecuteWidthRegion(dt);
                }

                await MarkOriginalRecord(item, true);
                if (string.IsNullOrEmpty(printerName))
                    printerName = (new System.Drawing.Printing.PrintDocument()).PrinterSettings.PrinterName;
                PrintDocument printDocument = document.PrintDocument;
                printDocument.PrinterSettings.PrinterName = printerName;

                printDocument.Print();
                return true;
            }
        }

        public ObservableCollection<MethodStandardDto> MyEdittingStandards { get; set; } = new();
        /// <summary>
        /// 打开编辑原始记录模板页面
        /// </summary>
        /// <returns></returns>
        [Command]
        public async Task OriginalRecordTemplateEdit()
        {
            var response = await _methodStandardService.GetMethodStandardsByTesterAsync(new MethodStandardFilterParam() { TesterName = UserDto.Inatance.UserName, TesterGroup = UserDto.Inatance.UserGroup });
            if (response.Status)
            {
                MyEdittingStandards = response.Result.ToObservableCollection();
                var dialogService = GetService<IDialogService>("OriginalRecordTemplateEditViewDialogService");
                dialogService.ShowDialog(

                    new List<UICommand> {
                        new UICommand{Caption = "关闭",IsDefault = false,IsCancel = true,}
                    }
                    , "", this);
            }


        }
        [Command]
        public void FileOpenDialog()
        {
            TemplateInfo.FileName = string.Empty;
            Microsoft.Win32.OpenFileDialog dlg = new()
            {
                DefaultExt = ".docx",
                Filter = "(*.doc,*.docx,)|*.doc;*.docx;"
            };

            bool? result = dlg.ShowDialog();

            if ((bool)result)
                TemplateInfo.FileName = dlg.FileName;

        }
        public ObservableCollection<MethodStandardDto> SelectedStandards { get; set; } = new();
        [Command]
        public async Task SaveOriginalRecordGroup()
        {

            foreach (var item in SelectedStandards)
                item.OriginalRecordGroup = TemplateInfo.OriginalRecordGroup;

            await _methodStandardService.UpdateRangeAsync(SelectedStandards.ToList());
            _messageBoxService.ShowMessage("信息同步成功！");

        }

        /// <summary>
        /// 编辑原始记录模板信息
        /// </summary>
        /// <returns></returns>
        [Command]
        public async Task UpdateOriginalRecordTemplatInfo()
        {
            if (!string.IsNullOrEmpty(TemplateInfo.FileName))
            {
                Spire.Doc.Document document = new Spire.Doc.Document(TemplateInfo.FileName);
                int[] rcs = new int[2];
                var table = new OriginalTable(document);
                foreach (var item in SelectedStandards)
                {
                    item.OriginalRecordTemplateFilePath = TemplateInfo.FileName;
                    item.Instruments = TemplateInfo.InstrumentsInfo;
                }

                await _methodStandardService.UpdateRangeAsync(SelectedStandards.ToList());
                _messageBoxService.ShowMessage("信息同步成功！");
            }


        }

        [Command]
        public async Task MarkTaskListOriginalRecord()
        {
            try
            {
                if (_messageBoxService.ShowMessage($"已选择{selectedTaskDatas.Count}项，确定标记？", "标记", MessageButton.OKCancel, MessageIcon.Question, MessageResult.OK) == MessageResult.OK)
                {
                    foreach (var data in selectedTaskDatas)
                        await MarkOriginalRecord(data, true);
                    await ShowNotifaction("", "标记成功！", "");
                }
            }
            catch (Exception)
            {
                // throw;
            }
        }
        [Command]
        public async void MarkOriginalRecord(ItemDto item)
        {
            await MarkOriginalRecord(item, !item.IsOriginalRecordComplete);
        }
        private async Task MarkOriginalRecord(ItemDto item, bool isMark)
        {
            item.IsOriginalRecordComplete = isMark;
            try
            {
                TaskDatasSource[TaskDatasSource.FindTaskDataIndex(item)].IsOriginalRecordComplete = isMark;
            }
            catch (Exception)
            {
                //throw;
            }
            await _itemService.UpdateAsync(item);


            var editingSample = SamplesSource.FirstOrDefault(s => s?.SampleCode == item.SampleCode);
            editingSample.Items = await GetAllItemsOfSample(editingSample);
        }
        #endregion



        /// <summary>
        /// 获取所有样品水分密度
        /// </summary>
        /// <returns></returns>
        [Command]
        public async Task ShowMoistureContentAndDensity()
        {
            foreach (SampleDto sample in selectedSamples)
            {
                ItemDto MoistureItem = (await _itemService.GetFirstItemBySampleCodeAndKeyItemAsync(new ItemFilterParam(sample.SampleCode) { KeyItem = "水分" })).Result;
                var moistureContent = MoistureItem != null ? $"{MoistureItem.TestResult} {MoistureItem.ReportUnit}" : "/";
                ItemDto DensityItem = (await _itemService.GetFirstItemBySampleCodeAndKeyItemAsync(new ItemFilterParam(sample.SampleCode) { KeyItem = "密度" })).Result;
                var densityContent = DensityItem != null ? $"{DensityItem.TestResult} {DensityItem.ReportUnit}" :"/";

                sample.MoistureContent = moistureContent;
                sample.Density = densityContent;

            }
            _messageBoxService.ShowMessage("水分&密度 已全部加载完成！");
        }


        /// <summary>
        /// 获取当前样品水分密度
        /// </summary>
        /// <param name="sample"></param>
        /// <returns></returns>
        protected async Task GetMoistureAndDensity(SampleDto sample)
        {
            MoistureContent = string.Empty;
            Density = string.Empty;
            try
            {
                if (sample == null)
                    return;

                ItemDto MoistureItem = (await _itemService.GetFirstItemBySampleCodeAndKeyItemAsync(new ItemFilterParam(sample.SampleCode) { KeyItem = "水分" })).Result;
                MoistureContent = MoistureItem != null ? $"{MoistureItem.TestItem}: {MoistureItem.TestResult} {MoistureItem.ReportUnit}" : string.Empty;
                ItemDto DensityItem = (await _itemService.GetFirstItemBySampleCodeAndKeyItemAsync(new ItemFilterParam(sample.SampleCode) { KeyItem = "密度" })).Result;
                Density = DensityItem != null ? $"{DensityItem.TestItem}: {DensityItem.TestResult} {DensityItem.ReportUnit}" : string.Empty;
                if (string.IsNullOrEmpty(MoistureContent)&&string.IsNullOrEmpty(Density))
                {
                    MoistureContent = "未指派水分/密度项目";
                }
            }
            catch (Exception)
            {
                //throw;
            }
        }


    }
    public class RecordTemplate : BindableBase
    {
        private string fileName;

        public string FileName
        {
            get => fileName;
            set
            {
                fileName = value;
                RaisePropertyChanged(nameof(FileName));
            }
        }

        private string? instrumentsInfo;

        public string? InstrumentsInfo
        {
            get
            {
                return instrumentsInfo;
            }
            set
            {
                instrumentsInfo = value;
                RaisePropertyChanged(nameof(InstrumentsInfo));
            }
        }
        private string? originalRecordGroup;

        public string? OriginalRecordGroup
        {
            get
            {
                return originalRecordGroup;
            }
            set
            {
                originalRecordGroup = value;
                RaisePropertyChanged(nameof(OriginalRecordGroup));
            }
        }
    }
    public class ReportModel
    {
        public string SampleCode
        {
            get;
            set;
        }
        public string TestDate
        {
            get;
            set;
        }
        public string TestMethod
        {
            get;
            set;
        }
        public string FirstSampleWeight
        {
            get;
            set;
        }
        public string SecondSampleWeight
        {
            get;
            set;
        }
        /// <summary>
        /// 水分
        /// </summary>
        public string Moisture
        {
            get;
            set;
        }
        /// <summary>
        /// 密度
        /// </summary>
        public string Density
        {
            get;
            set;
        }
        //public string AverageTestResult
        //{
        //    get;
        //    set;
        //}
        public string TestResult
        {
            get;
            set;
        }
        public string TestItem
        {
            get;
            set;
        }
        public DataTable DataTable
        {
            get;
            set;
        }

        public string Instruments { get; set; }
        public string Info { get; set; }
        public ReportModel()
        {
            DataTable = new DataTable();
            DataTable.TableName = "OriginalRecord";

            DataTable.Columns.Add(new DataColumn("Index", typeof(string)));
            DataTable.Columns.Add(new DataColumn("SubItem", typeof(string)));
            DataTable.Columns.Add(new DataColumn("FirstSubItemTestResult", typeof(string)));
            DataTable.Columns.Add(new DataColumn("SecondSubItemTestResult", typeof(string)));
            DataTable.Columns.Add(new DataColumn("AverageTestResult", typeof(string)));
            DataTable.Columns.Add(new DataColumn("SubItemTestResult", typeof(string)));
        }

    }
}

