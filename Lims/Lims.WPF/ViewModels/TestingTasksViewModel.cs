using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.POCO;
using Lims.Common.Dtos;
using Lims.Common.Parameters;
using Lims.ToolsForClient;
using NPOI.Util;

namespace Lims.WPF.ViewModels
{
    public class TestingTasksViewModel : SampleAndItemDataViewModelBase
    {
        public TestingTasksViewModel()
        {
            SampleViewWidth = "0.618*";
        }
        protected override async Task LoadMainDatas(UserDto? user)
        {
            ShowMainDatasLoadingPanel = true;
            pre_MyFocusedSampelRowIndex = FocusedSampleRowHandle;

            SelectedTester = null;
            ItemFilterParam itemFilterParam = new ItemFilterParam()
            {
                TestProgress = (int)RelativeProgress + 1,
                Operation = Operation.Lower,
            };

            var response = await _itemService.GetAllItemsByTestProgressAsync(itemFilterParam);
            if (response.Status)
            {
                TaskDatasSource = response.Result.OrderByDescending(t => t.ResultSubmitTime).ToObservableCollection();
                itemDtos = TaskDatasSource?.Copy();
                SamplesSource = TaskDatasSource.Select(s => s.Sample).DistinctBy(s => s.SampleCode).OrderBy(s => s.SampleCode).ToObservableCollection();
            }
            //userFilter(selectedTester);
            FocusedSampleRowHandle = pre_MyFocusedSampelRowIndex;
            ShowMainDatasLoadingPanel = false;
        }
        public static TestingTasksViewModel Create(string caption)
        {
            return ViewModelSource.Create(() => new TestingTasksViewModel()
            {
                Caption = caption,
            });
        }

        [Command]
        public async void MarkToCurrentUrgent()
        {
            if (_messageBoxService.ShowMessage("确认将所选样品标记为临时加急？", "临时加急", MessageButton.OKCancel, MessageIcon.Question, MessageResult.OK) == MessageResult.OK)
            {
                SampleDto[] sampleDtos= new SampleDto[SelectedSamples.Count];
                SelectedSamples.CopyTo(sampleDtos, 0);

                foreach (var sample in sampleDtos)
                {
                    sample.CurrentUrgent = true;
                    await _sampleService.UpdateAsync(sample);

                    foreach (var item in sample.Items)
                    {
                        if (item.TestProgress < 104)
                            await _loggerService.CreateAsync(new LoggerDto(DateTimeOffset.Now)
                            {
                                LogLevel = LogLevel.INFO,
                                ActionType = ActionType.变更样品信息,
                                PublisherIP = LoggerDto.GetLocalIP(),
                                PublisherName = CurrentUser.UserName,
                                ReceiverName = item.Tester,
                                SampleCode = sample.SampleCode,
                                TestItem = item.TestItem,
                                Message = "该样品已被临时加急",
                            });
                    }
                }
                await ShowNotifaction("", "已将选中样品标记为临时加急！", "");
            }
        }
        public override TestProgress GetRelativeProgress()
        {
            return TestProgress.检测中;
        }



        [Command]
        public override  void ShowAllItemsOfCurrentSample(SampleDto sample)
        {
            ReportDataPreview(sample);
        }

        private UserDto selectedTester;

        public UserDto SelectedTester
        {
            get { return selectedTester; }
            set
            {
                selectedTester = value;
                if (value != null)
                    userFilter(value);
                RaisePropertyChanged(nameof(SelectedTester));
            }
        }

        private void userFilter(UserDto tester)
        {
            try
            {

                TaskDatasSource = itemDtos?.Where(i => i.Tester == tester.UserName).ToObservableCollection();
            }
            catch (Exception)
            { }
        }
        protected override void urgentFilter(bool value)
        {
            if (value)
            {
                try
                {
                    TaskDatasSource = itemDtos?.Where(i => i.Sample.IsUrgent || i.Sample.CurrentUrgent).ToObservableCollection();

                    //SamplesSource = SamplesSource?.Where(s => s.IsUrgent || s.CurrentUrgent).ToObservableCollection();
                }
                catch (Exception)
                { }
            }
            else
            {
                TaskDatasSource = itemDtos?.ToObservableCollection();
                //SamplesSource = sampleDtos?.ToObservableCollection();
                SelectedTester = null;
            }
        }
    }
}