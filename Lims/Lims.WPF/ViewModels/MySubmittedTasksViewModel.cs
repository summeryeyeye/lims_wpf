using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.POCO;
using Lims.Common.Dtos;
using Lims.Common.Parameters;
using Lims.ToolsForClient;
using Lims.WPF.Services;
using Lims.WPF.Tools;
using Spire.Doc;

namespace Lims.WPF.ViewModels
{
    public class MySubmittedTasksViewModel : MyTasksViewModelBase
    {
        protected override async Task LoadMainDatas(UserDto? user)
        {
            ShowMainDatasLoadingPanel = true;
            pre_MyFocusedSampelRowIndex = FocusedSampleRowHandle;
            ItemFilterParam itemFilterParam = new ItemFilterParam()
            {
                TestProgress = (int)RelativeProgress,
                Tester = user.UserName,
                Operation = Operation.Higher,
                MinDate = Sample_BeginDate,
                MaxDate = Sample_EndDate,
            };

            var response = await _itemService.GetMyItemsAsync(itemFilterParam);
            if (response.Status)
            {
                TaskDatasSource = response.Result.OrderByDescending(t => t.ResultSubmitTime).ToObservableCollection();
                SamplesSource = TaskDatasSource.Select(s => s.Sample).DistinctBy(s => s.SampleCode).OrderBy(s => s.SampleCode).ToObservableCollection();
            }
            FocusedSampleRowHandle = pre_MyFocusedSampelRowIndex;
            ShowMainDatasLoadingPanel = false;
        }
        public static MySubmittedTasksViewModel Create(string caption)
        {
            return ViewModelSource.Create(() => new MySubmittedTasksViewModel()
            {
                Caption = caption,
            });
        }
        public override TestProgress GetRelativeProgress()
        {
            return TestProgress.检测中;
        }
        [Command]
        public  override async Task RefreshItemDatas(SampleDto sample)
        {
            if (sample != null)
            {
                // ItemsSource = await GetAllItemsOfSample(sample);
                ItemFilterParam itemFilterParam = new ItemFilterParam()
                {
                    SampleCode = sample.SampleCode,
                    Tester = CurrentUser.UserName,
                    TestProgress = (int)RelativeProgress,
                    Operation = Operation.Higher,
                    MinDate = Sample_BeginDate,
                    MaxDate = Sample_EndDate
                };
                ItemsSource = (await _itemService.GetMyItemsBySampleCodeAsync(itemFilterParam)).Result.ToObservableCollection();
            }
        }

        /// <summary>
        /// 撤销提交
        /// </summary>
        /// <param name="edittingItem"></param>
        /// <returns></returns>
        [Command]
        public async Task CancelSubmit(ItemDto edittingItem)
        {
            if (_messageBoxService.ShowMessage("确认撤销提交?", "警告", MessageButton.OKCancel, MessageIcon.Warning, MessageResult.OK) == MessageResult.OK)
            {
                //SampleDto sample = (await _sampleService.GetSingleAsync(edittingItem.SampleCode)).Result;


                var items = await GetAllItemsOfSample(edittingItem.Sample);
                if (items != null && items?.Min(i => i.TestProgress) > (int)TestProgress.检测中)
                {
                    _messageBoxService.ShowMessage("该样品下所有项目都已提交至数据一审,不支持自主撤销提交!");
                    return;
                }
                var editingSample = SamplesSource.FirstOrDefault(s => s.SampleCode == edittingItem.SampleCode);
                ;

                //int preTestProgress = edittingItem.TestProgress;

                edittingItem.TestProgress = (int)TestProgress.检测中;
                edittingItem.TestResult = null;

                await _itemService.UpdateAsync(edittingItem);
                ItemsSource.Remove(edittingItem);
                TaskDatasSource.Remove(edittingItem);

                await ExcuteIfNullSample(editingSample, TaskDatasSource.Where(i => i.SampleCode == editingSample.SampleCode));
                await ShowNotifaction(edittingItem.SampleCode + "  " + edittingItem.TestItem, "撤销成功！", "");
            }
        }



        private MethodStandardDto selectedMethodStandard;

        public MethodStandardDto SelectedMethodStandard
        {
            get
            {
                return selectedMethodStandard;
            }
            set
            {
                selectedMethodStandard = value;
            }
        }


    }


}