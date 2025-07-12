using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.POCO;
using DevExpress.Mvvm.Xpf;
using DevExpress.Xpf.Core;
using Lims.Common.Dtos;
using Lims.Common.Parameters;
using Lims.ToolsForClient;
using Lims.ToolsForClient.Extensions;
using Newtonsoft.Json;
using NPOI.Util;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

namespace Lims.WPF.ViewModels
{
    public class TasksCreateViewModel : SampleAndItemDataViewModelBase
    {
        public static TasksCreateViewModel Create(string caption)
        {
            return ViewModelSource.Create(() => new TasksCreateViewModel()
            {
                Caption = caption,
            });
        }
        public TasksCreateViewModel()
        {

            defaultsample = JsonConvert.DeserializeObject<SampleDto>(File.ReadAllText(limsPath + @"\工具库\配置文件\DefaultSampleInfo.json"));



            InitSampleInfo();

        }
        protected override async void OnInitializeInRuntime()
        {
            base.OnInitializeInRuntime();



            await InitMethodInfo();
            //await GetTesterNames();
        }

        private List<ProductStandardDto> _ProductStandardDBInfos = new();
        private ObservableCollection<ProductNode> productStandards = new();
        public ObservableCollection<ProductNode> ProductStandards
        {
            get
            => productStandards;
            set
            {
                productStandards = value;
                RaisePropertyChanged(nameof(ProductStandards));
            }
        }
        private ObservableCollection<MethodNode> testItemsSource = new();
        public ObservableCollection<MethodNode> TestItemsSource
        {
            get => testItemsSource;
            set
            {
                testItemsSource = value;
                RaisePropertyChanged(nameof(TestItemsSource));
            }
        }
        private static ObservableCollection<ProductNode> menuNodes = new();
        public ObservableCollection<ProductNode> MenuNodes
        {
            get
            => menuNodes;
            set
            {
                menuNodes = value;
                RaisePropertyChanged(nameof(MenuNodes));
            }
        }
        private string searchString;
        public string SearchString
        {
            get
            => searchString;
            set
            {
                searchString = value;
                RaisePropertyChanged(nameof(SearchString));
            }
        }
        private bool isCheckAll;
        public bool IsCheckAll
        {
            get
            => isCheckAll;
            set
            {
                isCheckAll = value;
                RaisePropertyChanged(nameof(IsCheckAll));
            }
        }

        private SampleDto currentSample;
        public SampleDto CurrentSample
        {
            get
            => currentSample;
            set
            {
                currentSample = value;
                RaisePropertyChanged(nameof(CurrentSample));
            }
        }

        private int selectedSampleStateIndex;

        public int SelectedSampleStateIndex
        {
            get
            {
                return selectedSampleStateIndex;
            }
            set
            {

                CurrentSample.SampleState = SampleStates[value].SampleState;
                selectedSampleStateIndex = value;
                SwitchSampleState();

                //CurrentSample.SampleState = value;
                RaisePropertyChanged(nameof(SelectedSampleStateIndex));

            }
        }
        /// <summary>
        /// 切换样品状态
        /// </summary>
        private async void SwitchSampleState()
        {
            //清空预览栏，确保预览栏内项目的样品状态信息一致
            PreviewSources.Clear();
            GetRootNodes(SearchString);
            await SearchByItemOrMethod(SearchString);
        }


        private bool isOverDate;

        public bool IsOverDate
        {
            get { return isOverDate; }
            set { isOverDate = value; RaisePropertyChanged(nameof(IsOverDate)); }
        }


        [Command]
        public void ShowCreateSubItemsView(ItemDto item)
        {
            var dialogService = GetService<IDialogService>("CreateSubItemsViewDialogService");
            dialogService.ShowDialog(
                new List<UICommand> {
                        new UICommand{Caption = "关闭窗口",IsDefault = false,IsCancel = true,}
                }
                , "", item);
        }
        /// <summary>
        /// 指派任务
        /// </summary>
        /// <returns></returns>
        [Command]
        public async Task CreateTasks()
        {
            string configPath = limsPath + @"\工具库\配置文件\CurrentMicroorganesmTesterModel.json";
            string json = File.ReadAllText(configPath);
            CurrentMicroorganesmTesterModel = JsonConvert.DeserializeObject<MicroorganismTesterModel>(json);




            DateTimeOffset currentTime = DateTimeOffset.Now;
            string preIdentityCode = currentTime.ToString("yyyyMMddHHmmss");
            bool IsCreateNewItem = false;
            if (PreviewSources.Count == 0)
            {
                _messageBoxService.ShowMessage("请添加项目后重试!");
                return;
            }

            List<ItemDto> existItems = null;
            var sampleModel = (await _sampleService.GetSingleAsync(CurrentSampleCode)).Result;
            if (sampleModel != null)
            {
                var response = (await _itemService.GetAllItemsBySampleCodeAsync(new ItemFilterParam(CurrentSampleCode)));
                if (response.Status)
                    existItems = response.Result;
                if (_messageBoxService.ShowMessage("编号已存在,是否继续?", "警告", MessageButton.OKCancel) == MessageResult.Cancel)
                    return;
            }
            else
            {
                CurrentSample.CreateTime = currentTime;

                await _sampleService.CreateAsync(CurrentSample);
                IsExistedSample = true;
            }

            int dataIndex;
            string? itemId;
            bool isNeedChangeTester = false;
            ObservableCollection<ItemDto> itemList = new();
            ObservableCollection<SubItemDto> subItemList = new();

            //string configPath = limsPath + @"\工具库\配置文件\CurrentMicroorganesmTesterModel.json";
            //var json = File.ReadAllText(configPath);

            Dictionary<int, string> dic = new Dictionary<int, string>();
            for (int i = 0; i < CurrentMicroorganesmTesterModel.Testers.Count; i++)
            {
                dic[i + 1] = CurrentMicroorganesmTesterModel.Testers[i];
            }


            var lastTester = CurrentMicroorganesmTesterModel.LastMicroorganismTester;
            foreach (var item in PreviewSources)
            {
                if (existItems != null && existItems.Any(i => i.TestItem == item.TestItem && i.MethodStandardId == item.MethodStandardId))
                    continue;

                dataIndex = PreviewSources.IndexOf(item) + 1;
                itemId = preIdentityCode + dataIndex.ToString("D4") + string.Empty.PadRight(3, '0');
                ItemDto itemModel = (ItemDto)item.Clone();
                string tester = item.Tester;

                #region 微生物分析人
                try
                {
                    if (item.Tester == "微生物")
                    {
                        int lastInt = GetLastInt(CurrentSampleCode);

                        if (item.TestItem.Contains(CurrentMicroorganesmTesterModel.Item))
                        {
                            tester = CurrentMicroorganesmTesterModel.Tester_D;
                        }
                        else
                        {
                            if (CurrentMicroorganesmTesterModel.LastCode_A.Contains(lastInt.ToString()))
                                tester = CurrentMicroorganesmTesterModel.Tester_A;
                            else if (CurrentMicroorganesmTesterModel.LastCode_B.Contains(lastInt.ToString()))
                                tester = CurrentMicroorganesmTesterModel.Tester_B;
                            else if (CurrentMicroorganesmTesterModel.LastCode_C.Contains(lastInt.ToString()))
                                tester = CurrentMicroorganesmTesterModel.Tester_C;
                            else if (CurrentMicroorganesmTesterModel.TurnLastCode.Contains(lastInt.ToString()))
                            {
                                isNeedChangeTester = true;
                                tester = dic[getNextTester(dic, Convert.ToInt32(lastTester))];

                            }

                        }
                    }
                }
                catch (Exception)
                {
                    // _messageBoxService.ShowMessage($"出现了一点小问题({ex.Message}),请联系技术员(确定后可以继续操作!)");
                }
                #endregion

                itemModel.SampleCode = CurrentSampleCode;
                itemModel.TestProgress = 101;
                itemModel.PreTestProgress = 100;
                if (IsOverDate)
                {
                    itemModel.TestProgress = 103;
                    itemModel.IsOverDate = IsOverDate;
                }
                itemModel.ItemId = itemId;

                itemModel.Tester = tester;
                itemModel.AppointTime = currentTime;
                itemList.Add(itemModel);
                IsCreateNewItem = true;

                if (item.SubItems != null && item.SubItems.Count > 0)
                {
                    string parentIDpre = itemId.Substring(0, 14);

                    int secondDataIndex = 0;
                    string? subItemId;
                    foreach (var subItem in item.SubItems)
                    {
                        subItem.ItemId = itemId;
                        subItem.CreateTime = currentTime;

                        int codeIndex = Convert.ToInt32(itemId.Substring(14, 7));
                        var str = string.Empty.PadLeft(8 - (codeIndex - 500 + secondDataIndex).ToString().Length, '0');

                        subItemId = parentIDpre + string.Empty.PadLeft(7 - (codeIndex - 500 + secondDataIndex).ToString().Length, '0') + (codeIndex - 500 + secondDataIndex).ToString();
                        secondDataIndex++;
                        subItem.SubItemId = subItemId;
                        subItemList.Add(subItem);
                    }
                }
            }


            if (isNeedChangeTester)
            {
                CurrentMicroorganesmTesterModel.LastMicroorganismTester = getNextTester(dic, Convert.ToInt32(lastTester));

                json = JsonConvert.SerializeObject(CurrentMicroorganesmTesterModel, Newtonsoft.Json.Formatting.Indented);
                configPath = limsPath + @"\工具库\配置文件\CurrentMicroorganesmTesterModel.json";
                File.WriteAllText(configPath, json);

            }
            var dialogService = GetService<IDialogService>("PrinterSelecterViewDialogService");
            dialogService.ShowDialog(
            new List<UICommand> {
                new UICommand{Caption = "打印任务随行单",IsDefault = true,IsCancel = false,Command = new DelegateCommand(new Action(async () =>
                {
                    await  打印任务随行单(itemList,Printers[CurrentPrinterIndex]);
                }))},
                new UICommand{Caption = "不打印",IsDefault = false,IsCancel = true,}
            }
            , "", this);

            foreach (var item in itemList)
            {
                await _itemService.CreateAsync(item);
            }
            foreach (var subitem in subItemList)
                await _subItemService.CreateAsync(subitem);
            await ShowNotifaction(CurrentSampleCode, "任务指派成功！", "");

        }
        private int getNextTester(Dictionary<int, string> dic, int No)
        {
            if (No == dic.Count)
                No = 0;
            return No + 1;
        }
        public MicroorganismTesterModel CurrentMicroorganesmTesterModel { get; set; }
        /// <summary>
        /// 微生物分析人管理
        /// </summary>
        /// <returns></returns>
        [Command]
        public void MicroorganismTesterManagement()
        {

            var dialogService = GetService<IDialogService>("MicroorganismTesterManagementViewDialogService");
            dialogService.ShowDialog(new List<UICommand> {
                 new UICommand{Caption = "保存",IsDefault = true,IsCancel = false,Command=new DelegateCommand(() =>
                 {
                     string json=JsonConvert.SerializeObject(CurrentMicroorganesmTesterModel, Newtonsoft.Json.Formatting.Indented);
                     string configPath = limsPath + @"\工具库\配置文件\CurrentMicroorganesmTesterModel.json";
                     File.WriteAllText(configPath, json);
                     DXMessageBox.Show("保存成功！");
                 })},
                        new UICommand{Caption = "关闭窗口",IsDefault = false,IsCancel = true,}
                }, "", this);
        }
        private static int GetLastInt(string? sampleCode)
        {
            int lastInt = 0;
            string validCode = string.Empty;
            if (Regex.IsMatch(sampleCode, @"^[0-9]{4}-[0-9]{5}$"))
                validCode = sampleCode.Split('-')[1];
            else if (Regex.IsMatch(sampleCode, @"^[0-9]{5}-[0-9]{4}$"))
                validCode = sampleCode.Split('-')[0];

            lastInt = Convert.ToInt32(validCode.Last().ToString());
            return lastInt;
        }
        private bool isExistedSample;
        public bool IsExistedSample
        {
            get
            => isExistedSample;
            set
            {
                isExistedSample = value;
                RaisePropertyChanged(nameof(IsExistedSample));
            }
        }

        private bool sampleCodeValidate;

        public bool SampleCodeValidate
        {
            get
            {
                return sampleCodeValidate;
            }
            set
            {
                sampleCodeValidate = value;
                RaisePropertyChanged(nameof(SampleCodeValidate));
            }
        }



        private string? currentSampleCode;

        public string? CurrentSampleCode
        {
            get
            {
                return currentSampleCode;
            }
            set
            {

                currentSampleCode = value;
                RaisePropertyChanged(nameof(CurrentSampleCode));
                if (sampleCodeCorrectReg.IsMatch(value))
                {
                    SampleCodeValidate = true;
                    CurrentSample.SampleCode = value;
                    FindExistSample(value);
                }
                else
                {
                    SampleCodeValidate = false;
                }
            }
        }





        public readonly Regex sampleCodeCorrectReg = new(@"^[0-9]{4}-[0-9]{5}$|^[0-9]{5}-[0-9]{4}$");
        [Command]
        public async void FindExistSample(string? sampleCode)
        {
            try
            {
                IsExistedSample = false;

                var sample = (await _sampleService.GetSingleAsync(sampleCode)).Result;

                if (sample != null)
                {
                    IsExistedSample = true;

                    CurrentSample.SampleName = sample.SampleName;
                    CurrentSample.IsUrgent = sample.IsUrgent;
                    CurrentSample.TaskType = sample.TaskType;
                    CurrentSample.SampleState = sample.SampleState;

                    if (_messageBoxService.ShowMessage("该样品已存在，是否将样品所有项目添加到预览栏？", "提示", MessageButton.OKCancel, MessageIcon.Warning, MessageResult.Cancel) == MessageResult.OK)
                    {
                        if (menuNodes.First().Text != sample.SampleState)
                            SwitchSampleState();

                        var existItems = await _itemService.GetAllItemsBySampleCodeAsync(new Common.Parameters.ItemFilterParam(sample.SampleCode));
                        var currentTime = DateTimeOffset.Now;
                        PreviewSources.Clear();


                        foreach (var i in existItems.Result)
                        {
                            var methodResponse = await _methodStandardService.GetSingleAsync(i.MethodStandardId);
                            if (methodResponse.Result == null)
                                _messageBoxService.ShowMessage($"该检测方法（检测项目：{i.TestItem} 方法名称：{i.TestMethod} Id：{i.MethodStandardId}）已失效，请联系管理员变更!");
                            else
                            {
                                i.TestProgress = 101;
                                i.TestResult = string.Empty;
                                i.Temp_TestResult = string.Empty;
                                i.SingleConclusion = "/";
                                i.Temp_SingleConclusion = "/";
                                i.ResultSubmitTime = null;
                                i.TestDate = null;
                                i.TestRemark = null;
                                i.IsOverDate = false;
                                //i.ItemRemark = null;
                                i.IsOriginalRecordComplete = false;
                                i.PreTestProgress = (int)TestProgress.无;

                                if (methodResponse.Status && methodResponse.Result.Tester == "微生物")
                                    i.Tester = methodResponse.Result.Tester;
                                i.AppointTime = currentTime;
                            }
                        }

                        var itemsHasSubItems = existItems.Result.Where(i => i.SubItems != null && i.SubItems.Count > 0);


                        foreach (var i in itemsHasSubItems)
                        {
                            foreach (var s in i.SubItems)
                            {
                                s.TestResult = string.Empty;
                                s.Temp_TestResult = string.Empty;
                            }
                        }




                        PreviewSources.AddRange(existItems.Result);
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// 初始化方法信息
        /// </summary>
        [Command]
        public async Task InitMethodInfo()
        {
            _ProductStandardDBInfos = (await _productStandardService.GetAllAsync()).Result;

            SearchString = string.Empty;

            GetRootNodes(string.Empty);
        }
        [Command]
        public void CheckSubItem(SubItem subitem)
        {
            subitem.IsChecked = !subitem.IsChecked;
        }
        [Command]
        public void GetChildrenNodes(ProductNode node)
        {

            string sampleState = CurrentSample.SampleState; //RadioButton.Content;
            bool isChecked = node.IsChecked;

            node.IsChecked = !isChecked;

            if (node.ProductLevel == ProductLevel.TestItem)
            {
                if (productStandards.All(s => !s.IsChecked))
                {
                    IsCheckAll = false;
                }
                if (productStandards.All(s => s.IsChecked))
                {
                    IsCheckAll = true;
                }
                return;
            }

            GetStandardNodes(node, SearchResultProductInfos);

            menuNodes.Add(node);
            IsCheckAll = false;
        }
        [Command]
        public void GuidToNode(ProductNode node)
        {
            if (node == null)
                return;
            _GuidToNode(node);
        }
        private void _GuidToNode(ProductNode node)
        {
            string sampleState = CurrentSample.SampleState;// RadioButton.Content;

            if ((node.ProductLevel != ProductLevel.Null && node.ProductLevel == menuNodes.Last().ProductLevel) || (node.ProductLevel != ProductLevel.Null && node.ProductLevel == menuNodes.Last().ProductLevel))
                return;
            List<ProductNode> nodes = new();

            nodes = menuNodes.Where(m => m.ProductLevel > node.ProductLevel).ToList();
            foreach (var item in nodes)
            {
                menuNodes.Remove(item);
            }
            GetStandardNodes(node, SearchResultProductInfos);

            if (node.ProductLevel == ProductLevel.SampleState)
            {
                GetRootNodes(SearchString);
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
        public void SearchByProduct(string searchParam)
        {
            GetRootNodes(searchParam);
        }
        [Command]
        public async Task ShowSearchByItemOrMethodView()
        {
            if (!string.IsNullOrWhiteSpace(SearchString))
            {
                await SearchByItemOrMethod(SearchString);
            }
            var dialogService = GetService<IDialogService>("SearchByItemOrMethodViewDialogService");
            dialogService.ShowDialog(new List<UICommand> {
                        new UICommand{Caption = "关闭窗口",IsDefault = false,IsCancel = true,}
                }, "", this);
        }
        [Command]
        public async Task SearchByItemOrMethod(string searchParam)
        {
            MethodsSource.Clear();
            TestItemsSource.Clear();
            if (!string.IsNullOrWhiteSpace(searchParam))
            {
                var response1 = (await _methodStandardService.GetMethodStandardsBySearchWordAsync(new MethodStandardFilterParam() { SearchWord = searchParam, SampleState = currentSample.SampleState }));
                if (response1.Status)
                {
                    var results1 = response1.Result.DistinctBy(i => i.TestItem);
                    SampleStates[selectedSampleStateIndex].MethodsCount = results1.Count();

                    var response2 = (await _methodStandardService.GetMethodStandardsBySearchWordAsync(new MethodStandardFilterParam() { SearchWord = searchParam, SampleState = SampleStates[Math.Abs(selectedSampleStateIndex - 1)].SampleState }));
                    var results2 = response2.Result.DistinctBy(i => i.TestItem);
                    SampleStates[Math.Abs(selectedSampleStateIndex - 1)].MethodsCount = results2.Count();

                    foreach (var i in results1)
                        TestItemsSource.Add(new MethodNode { Text = i.TestItem });
                }
            }
            else
            {
                SampleStates[selectedSampleStateIndex].MethodsCount = 0;
                SampleStates[Math.Abs(selectedSampleStateIndex - 1)].MethodsCount = 0;
            }

        }
        private ObservableCollection<MethodStandardDto> methodsSource = new();
        public ObservableCollection<MethodStandardDto> MethodsSource
        {
            get => methodsSource;
            set
            {
                methodsSource = value;
                RaisePropertyChanged(nameof(MethodsSource));
            }
        }
        [Command]
        public async Task ShowMethods(MethodNode itemNode)
        {
            if (itemNode == null)
                return;

            MethodsSource.Clear();
            MethodsSource = (await _methodStandardService.GetMethodStandardsByTestItemAsync(new MethodStandardFilterParam() { SampleState = currentSample.SampleState, TestItem = itemNode.Text })).Result.ToObservableCollection();

            foreach (var t in TestItemsSource)
                t.IsChecked = false;
            itemNode.IsChecked = true;
        }
        public MethodStandardDto SelectedMethod
        {
            get; set;
        }
        [Command]
        public void GetRootNodes(string searchParam)
        {
            ProductStandards = new ObservableCollection<ProductNode>();
            MenuNodes.Clear();

            //bool _searchBySampleState = SearchBySampleState;
            string _sampleState = CurrentSample?.SampleState;// RadioButton.Content;

            ProductNode rootNode = new()
            {
                Text = _sampleState,
                ParentNode = null,
                ProductLevel = 0,
            };

            menuNodes.Add(rootNode);

            SearchResultProductInfos = _ProductStandardDBInfos.Where(s => s.SampleState == _sampleState && s.ExecuteStandard.Contains(searchParam) && s.ExecuteStandard.Trim().Length > 1).ToList();
            List<string> products = SearchResultProductInfos.DistinctBy(s => s.ExecuteStandard).Select(s => s.ExecuteStandard).ToList();

            SampleStates[selectedSampleStateIndex].ProductsCount = products.Count;
            var data = _ProductStandardDBInfos.Where(s => s.SampleState == SampleStates[Math.Abs(selectedSampleStateIndex - 1)].SampleState && s.ExecuteStandard.Contains(searchParam) && s.ExecuteStandard.Trim().Length > 1).DistinctBy(s => s.ExecuteStandard).Count();

            SampleStates[Math.Abs(selectedSampleStateIndex - 1)].ProductsCount = data;



            foreach (var product in products)
            {
                ProductStandards.Add(new ProductNode()
                {
                    ParentNode = rootNode,
                    ProductLevel = ProductLevel.ExecuteStandard,
                    Text = product,
                });
            }
        }
        [Command]
        public void AddToPreviewSubItems(List<SubItem> subItems)
        {
            var checkedItems = subItems.Where(s => s.IsChecked).ToList();
            if (EdittingItem.SubItems == null)
            {
                EdittingItem.SubItems = new();
            }
            foreach (var subItem in checkedItems)
            {
                if (EdittingItem.SubItems.Any(c => c.TestItem == subItem.TestItem))
                    continue;
                EdittingItem.SubItems.Add(subItem);
            }
        }
        SampleDto defaultsample;
        /// <summary>
        /// 初始化样品信息
        /// </summary>
        private void InitSampleInfo()
        {
            CurrentSample = defaultsample.Copy();

            CurrentSampleCode = defaultsample.SampleCode;
        }
        [Command]
        public async void SaveAsInitializedInfo()
        {
            var defaultsample = new SampleDto();
            defaultsample.IsUrgent = CurrentSample.IsUrgent;
            defaultsample.TaskType = CurrentSample.TaskType;
            defaultsample.SampleCode = CurrentSampleCode;
            defaultsample.SampleState = currentSample.SampleState;
            defaultsample.SampleName = CurrentSample.SampleName;

            string json = JsonConvert.SerializeObject(defaultsample, Newtonsoft.Json.Formatting.Indented);
            string configPath = limsPath + @"\工具库\配置文件\DefaultSampleInfo.json";


            File.WriteAllText(configPath, json);

            await ShowNotifaction("", "保存初始化信息成功!", "");
        }


        public SubItemClass SelectedStandardSubItem
        {
            get; set;
        }
        public List<SubItemClass> StandardSubItems { get; set; } = new();

        [Command]
        public async Task AddSubItems(ItemDto itemModel)
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

                    List<SubItem> subitems = new();

                    foreach (var s in subs)
                    {
                        subitems.Add(new SubItem
                        {
                            TestItem = s.Name,
                            // ReportUnit = itemModel.ReportUnit
                        });
                    }
                    item.SubItems = subitems;
                }

                var dialogService = GetService<IDialogService>("AddSubItemsDialogService");
                dialogService.ShowDialog(new List<UICommand> {
                        new UICommand{Caption = "关闭窗口",IsDefault = false,IsCancel = true,}
                }, "", this);
            }
        }
        public ObservableCollection<ItemDto> PreviewSources { get; set; } = new();
        /// <summary>
        /// 根据产品标准添加项目
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        [Command]
        public async Task AddItemsToPreviewByProductStandard(ObservableCollection<ProductNode> nodes)
        {
            List<ProductNode> checkedNodes = nodes.Where(n => n.IsChecked).ToList();
            //List<ItemDto> items = new();
            foreach (var node in checkedNodes)
            {
                if (PreviewSources.Any(p => p.TestItem == node.Text && p.MethodStandardId == node.MethodStandardId))
                {
                    continue;
                }
                else
                {
                    var method = await _methodStandardService.GetSingleAsync(node.MethodStandardId);

                    if (method.Status)
                    {
                        //if (!PreviewSources.Any(i => i.MethodStandardId == method.Result.Id))
                        //{
                        PreviewSources.Add(new ItemDto(node.MethodStandardId, newSources.FirstOrDefault(s => s.TestItem == node.Text).ProductUnit, node.Text, node.ProductStandardId, newSources.First(s => s.TestItem == node.Text).IndexRequest)
                        {
                            TestProgress = (int)TestProgress.待领取,
                            Tester = method.Result.Tester,
                            MethodStandard = method.Result,
                            ProductStandard = node.ProductStandard,
                            SampleFormOrState = node.ProductStandard.ProductForm,
                        });
                        node.IsChecked = false;
                        // }

                    }
                }
            }
            //PreviewSources.AddRange(items);
            if (nodes.All(n => !n.IsChecked))
            {
                IsCheckAll = false;
            }
        }
        /// <summary>
        /// 根据方法添加项目
        /// </summary>
        [Command]
        public void AddItemsToPreviewByMethodStandard()
        {
            if (SelectedMethod == null || PreviewSources.Any(p => p.MethodStandardId == SelectedMethod.Id))
                return;
            PreviewSources.Add(new ItemDto(SelectedMethod.Id, SelectedMethod.TestUnit, SelectedMethod.TestItem, 0, "")
            {
                MethodStandard = SelectedMethod,
                TestProgress = (int)TestProgress.待领取,
                Tester = SelectedMethod.Tester,
                SampleFormOrState = SampleStates[SelectedSampleStateIndex].SampleState,
            });
        }
        public SubItem SelectedPreviewSubItem
        {
            get; set;
        }
        public TasksCreateViewModel(SubItem selectedPreviewSubItem)
        {
            SelectedPreviewSubItem = selectedPreviewSubItem;
        }
        public List<SubItemDto> SelectedPreviewSubItems { get; set; } = new();
        public TasksCreateViewModel(List<SubItemDto> selectedPreviewSubItems)
        {
            SelectedPreviewSubItems = selectedPreviewSubItems;
        }
        public List<ProductStandardDto> SearchResultProductInfos { get; set; } = new();
        [Command]
        public async Task ScrollsKeyUp(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    InitSampleInfo();
                    await ShowNotifaction("", "样品信息已初始化!", "");
                    break;

                default:
                    break;
            }
        }
        [Command]
        public void CreatItemsPreviewKeyDown(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    PreviewSources.Clear();
                    break;

                case Key.Delete:
                    if (_messageBoxService.ShowMessage("确定移除选中项?", "移除", MessageButton.OKCancel, MessageIcon.Warning, MessageResult.OK) == MessageResult.OK)
                    {
                        PreviewSources.Remove(focusedItem);
                    }
                    break;

                default:
                    break;
            }
        }
        [Command]
        public void PreviewSubItemsKeyUp(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Delete:
                    SubItemDto[] subItems = new SubItemDto[SelectedPreviewSubItems.Count];
                    SelectedPreviewSubItems.CopyTo(subItems);
                    foreach (SubItemDto subItem in subItems)
                    {
                        EdittingItem.SubItems.Remove(subItem);
                    }
                    break;

                default:
                    break;
            }
        }
        [Command]
        public void FilterTextBoxKeyUp(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    var searchStr = (e.OriginalSource as TextBox).Text;
                    SearchByProduct(searchStr);
                    break;
                default:
                    break;
            }
        }
        [Command]
        public async Task AddItemKeyUp(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Back:
                    if (menuNodes == null || menuNodes.Count <= 1)
                        return;

                    ProductNode lastNode = menuNodes.Last();
                    if (lastNode.ParentNode != null)
                    {
                        if (lastNode.ParentNode.ProductLevel == ProductLevel.SampleState)
                        {
                            GetRootNodes(SearchString);
                        }
                        GuidToNode(lastNode.ParentNode);
                    }
                    else
                    {
                        GetRootNodes(SearchString);
                    }
                    break;

                case Key.Enter:
                    if (_currentProductLevel.Equals(ProductLevel.TestItem))
                    {
                        await AddItemsToPreviewByProductStandard(ProductStandards);
                    }
                    break;
            }
        }
        [Command]
        public void AddItemToPreview()
        {

        }
        private ProductLevel _currentProductLevel;
        [Command]
        public void GetSampleName()
        {
            if (menuNodes != null && menuNodes.Count > 1 && menuNodes[1].ProductLevel == ProductLevel.ExecuteStandard)
            {
                try
                {
                    CurrentSample.SampleName = menuNodes[1].Text.Split(' ')[2];
                }
                catch (Exception)
                {

                }
            }
        }
        private List<ProductStandardDto> newSources = new();
        private void GetStandardNodes(ProductNode node, List<ProductStandardDto> sources)
        {
            productStandards.Clear();
            newSources = sources;
            string sampleState = CurrentSample.SampleState;// RadioButton.Content;
            ProductLevel level = node.ProductLevel;
            foreach (var item in menuNodes)
            {
                newSources = newSources.Where(s => s.GetType().GetProperty(item.ProductLevel.ToString()).GetValue(s, null).ToString() == item.Text).ToList();
            }

            newSources = newSources.Where(s => s.GetType().GetProperty(level.ToString()).GetValue(s, null).ToString() == node.Text).ToList();
            level++;
            _currentProductLevel = level;
            List<string> texts = newSources.Select(s => s.GetType().GetProperty(level.ToString()).GetValue(s, null).ToString()).Distinct().ToList();

            ObservableCollection<ProductNode> nds = new();
            foreach (var text in texts)
            {
                ProductNode pn = new()
                {
                    ParentNode = node,
                    ProductLevel = level,
                    Text = text,
                };
                if (level == ProductLevel.TestItem)
                {
                    pn.MethodStandardId = newSources.FirstOrDefault(s => s.TestItem == text).TestMethodId;
                    pn.ProductStandardId = newSources.FirstOrDefault(s => s.TestItem == text).Id;
                    pn.ProductStandard = newSources.FirstOrDefault(s => s.TestItem == text);
                }
                nds.Add(pn);
            }
            ProductStandards = nds;
        }
        [Command]
        public override void ItemRowDoubleClick(RowClickArgs args)
        {
            if (args.Item is ItemDto item && item.SubItems != null && item.SubItems.Count > 0)
                ShowCreateSubItemsView(item);
        }
        [Command]
        public void ShowCreatMethodView(MethodNode method)
        {
            CreattingMethod = new()
            {
                SampleState = CurrentSample.SampleState,
                TestItem = method.Text,
                LastUpdater = CurrentUser.UserName,
            };

            var dialogService = GetService<IDialogService>("CreateMethodViewDialogService");
            dialogService.ShowDialog(new List<UICommand> {
                        new UICommand{Caption = "确定添加",IsDefault = true,IsCancel = false,Command=new DelegateCommand( async() =>
                        {
                           if (string.IsNullOrWhiteSpace(CreattingMethod.TestItem) || string.IsNullOrWhiteSpace(CreattingMethod.TestMethod) || string.IsNullOrWhiteSpace(CreattingMethod.Tester)){
                                _messageBoxService.ShowMessage("方法格式不正确,添加失败!");
                                return;
                            }else
                            {
                                await _methodStandardService.CreateAsync(CreattingMethod);
                                await ShowNotifaction("", "方法添加成功!", "");
                            }
                        })},
                        new UICommand{Caption = "关闭窗口",IsDefault = false,IsCancel = true,}
                }, "", this);

        }
        private ObservableCollection<MethodStandardDto> editingMethods = new()
        {
            new MethodStandardDto(){  SampleState="固体"},
            new MethodStandardDto(){  SampleState="液体"},
        };
        public ObservableCollection<MethodStandardDto> EditingMethods
        {
            get
            {
                return editingMethods;
            }
            set
            {
                editingMethods = value;
            }
        }
        public SubItemStandardDto CreattingStandardSubItem
        {
            get; set;
        }
        [Command]
        public void ShowCreatStandardSubItemView()
        {
            CreattingStandardSubItem = new()
            {
                ParentNames = FocusedItem.TestItem,
            };
            if (SelectedStandardSubItem != null)
                CreattingStandardSubItem.Type = SelectedStandardSubItem.Text;

            var dialogService = GetService<IDialogService>("CreateStandardSubItemViewDialogService");
            dialogService.ShowDialog(new List<UICommand> {
                        new UICommand{Caption = "确定添加",IsDefault = true,IsCancel = false,Command=new DelegateCommand( async() =>
                        {
                           if (string.IsNullOrWhiteSpace(CreattingStandardSubItem.Name) || string.IsNullOrWhiteSpace(CreattingStandardSubItem.ParentNames) || string.IsNullOrWhiteSpace(CreattingStandardSubItem.Type)){
                                _messageBoxService.ShowMessage("标准子项目格式不正确,添加失败!");
                                return;
                            }else
                            {
                                if (!(await _subItemStandardService.GetAnySubItemStandardsByKeyWordAsync(new SubItemStandardFilterParam(){ParentNames=CreattingStandardSubItem.Name})).Result)
                                {
                                    await _subItemStandardService.CreateAsync(CreattingStandardSubItem);
                                    await ShowNotifaction("", "标准子项目添加成功!", "");
                                }else
                                {
                                    _messageBoxService.ShowMessage("该标准子项目已存在,添加失败!");
                                }
                            }

                        })},
                        new UICommand{Caption = "关闭窗口",IsDefault = false,IsCancel = true,}
                }, "", this);
        }

        protected override Task LoadMainDatas(UserDto? user)
        {
            return Task.CompletedTask;
        }

        public override TestProgress GetRelativeProgress()
        {
            return TestProgress.无;
        }
    }
    public class SubItemClass
    {
        public SubItemClass()
        {
            SubItems = new List<SubItem>();
        }

        public string Text
        {
            get; set;
        }

        public string ParentNames
        {
            get; set;
        }

        public List<SubItem> SubItems
        {
            get; set;
        }
    }
    public class SubItem : SubItemDto, ICheckable
    {
        private bool isChecked;

        public bool IsChecked
        {
            get
            => isChecked;

            set
            {
                isChecked = value;
                RaisePropertiesChanged(nameof(IsChecked));
            }
        }
    }
    public class CompBottonModel : BaseDto
    {
        public CompBottonModel()
        {
            //构造函数
        }

        private string content;

        /// <summary>
        /// 单选框相关
        /// </summary>
        public string Content
        {
            get => content;
            set
            {
                content = value;
                RaisePropertiesChanged(nameof(Content));
            }
        }

        public int EditValue
        {
            get; set;
        }

        private bool isCheck;

        /// <summary>
        /// 单选框是否选中
        /// </summary>
        public bool IsCheck
        {
            get
            => isCheck;
            set
            {
                isCheck = value;
                RaisePropertiesChanged(nameof(IsCheck));
            }
        }
    }
    public interface ICheckable
    {
        public bool IsChecked
        {
            get; set;
        }
    }

    public abstract class StandardNode : BaseDto, ICheckable
    {


        private string background = "#87CEFA";

        public string Background
        {
            get
            {
                return background;
            }
            set
            {
                background = value;
            }
        }

        public string? Text
        {
            get; set;
        }


        private bool isChecked = false;

        public bool IsChecked
        {
            get
            {
                return isChecked;
            }
            set
            {
                isChecked = value;
                if (isChecked)
                {
                    background = "#3CB371";
                }
                else
                {
                    background = "#87CEFA";
                }
                RaisePropertiesChanged(nameof(IsChecked));
                RaisePropertiesChanged(nameof(Background));
            }
        }
    }
    public class ProductNode : StandardNode
    {
        public ProductNode()
        {
            ChildrenNodes = new ObservableCollection<ProductNode>();
        }
        public string? CheckBoxVisibility
        {
            get
            {
                if (ProductLevel.Equals(ProductLevel.TestItem))
                {
                    return "Visible";
                }
                else
                {
                    return "Hidden";
                }
            }
        }
        public int FontSize
        {
            get
            {
                if (ProductLevel.Equals(ProductLevel.ExecuteStandard))
                {
                    return 15;
                }
                else
                {
                    return 16;
                }
            }
        }
        public string Alignment
        {
            get
            {
                if (ProductLevel.Equals(ProductLevel.ExecuteStandard))
                {
                    return "Stretch";
                }
                else
                {
                    return "Center";
                }
            }
        }
        public int Size
        {
            get
            {
                if (ProductLevel.Equals(ProductLevel.ExecuteStandard))
                {
                    return 130;
                }
                else
                {
                    return 90;
                }
            }
        }
        public ProductLevel ProductLevel
        {
            get; set;
        }
        public ProductNode? ParentNode
        {
            get; set;
        }
        public ObservableCollection<ProductNode> ChildrenNodes
        {
            get; set;
        } = new();
        public int MethodStandardId
        {
            get; set;
        }
        public int ProductStandardId
        {
            get; set;
        }
        public ProductStandardDto ProductStandard
        {
            get; set;
        }
    }
    public class MethodNode : StandardNode
    {
        public MethodNode()
        {
            ChildrenNodes = new ObservableCollection<MethodNode>();
        }
        public string? CheckBoxVisibility
        {
            get
            {
                if (MethodLevel.Equals(MethodLevel.TestItem))
                {
                    return "Visible";
                }
                else
                {
                    return "Hidden";
                }
            }
        }
        public int FontSize
        {
            get
            {
                if (MethodLevel.Equals(MethodLevel.TestMethod))
                {
                    return 15;
                }
                else
                {
                    return 16;
                }
            }
        }
        public string Alignment
        {
            get
            {
                if (MethodLevel.Equals(MethodLevel.TestMethod))
                {
                    return "Stretch";
                }
                else
                {
                    return "Center";
                }
            }
        }
        public int Size
        {
            get
            {
                return 120;
            }
        }
        public MethodLevel MethodLevel
        {
            get; set;
        }
        public MethodNode? ParentNode
        {
            get; set;
        }
        public ObservableCollection<MethodNode> ChildrenNodes
        {
            get; set;
        } = new();
    }


    public enum ProductLevel
    {
        SampleState,
        ExecuteStandard,
        ProductType,
        ProductClass,
        ProductForm,
        TestItem,
        Null,
    }

    public enum MethodLevel
    {
        SampleState = 0,
        TestItem = 1,
        TestMethod = 2,
        Null = 3,
    }
    public class MicroorganismTesterModel
    {
        private string lastCode_A;
        public string LastCode_A
        {
            get { return lastCode_A; }
            set { lastCode_A = value; }
        }
        private string tester_A;
        public string Tester_A
        {
            get { return tester_A; }
            set { tester_A = value; }
        }
        private string lastCode_B;
        public string LastCode_B
        {
            get { return lastCode_B; }
            set { lastCode_B = value; }
        }
        private string tester_B;
        public string Tester_B
        {
            get { return tester_B; }
            set { tester_B = value; }
        }
        private string lastCode_C;
        public string LastCode_C
        {
            get { return lastCode_C; }
            set { lastCode_C = value; }
        }
        private string tester_C;
        public string Tester_C
        {
            get { return tester_C; }
            set { tester_C = value; }
        }
        private string item;
        public string Item
        {
            get { return item; }
            set { item = value; }
        }
        private string tester_D;
        public string Tester_D
        {
            get { return tester_D; }
            set { tester_D = value; }
        }
        private string turnLastCode;
        public string TurnLastCode
        {
            get { return turnLastCode; }
            set { turnLastCode = value; }
        }
        public int LastMicroorganismTester { get; set; }
        public List<string> Testers { get; set; }
    }

}