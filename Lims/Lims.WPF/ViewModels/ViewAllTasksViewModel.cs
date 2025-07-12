using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.POCO;
using Lims.Common.Dtos;
using Lims.Common.Parameters;
using Lims.ToolsForClient;

namespace Lims.WPF.ViewModels
{
    public class ViewAllTasksViewModel : SampleAndItemDataViewModelBase
    {

        public static ViewAllTasksViewModel Create(string caption)
        {
            return ViewModelSource.Create(() => new ViewAllTasksViewModel()
            {
                Caption = caption,
            });
        }

        public override TestProgress GetRelativeProgress()
        {
            return TestProgress.无;
        }

        protected override async Task LoadMainDatas(UserDto? user)
        {
            //sample_BeginDate = DateTimeOffset.Now.AddMonths(-6);
            ShowMainDatasLoadingPanel = true;
            pre_MyFocusedSampelRowIndex = FocusedSampleRowHandle;

            SampleFilterParam param = new SampleFilterParam
            {
                MinDate = Sample_BeginDate,
                MaxDate = Sample_EndDate,
                WithItems = false
            };

            var response = await _sampleService.GetSamplesAsync(param);

            if (response.Status)
                SamplesSource = response.Result.OrderByDescending(s => s.CreateTime).ToObservableCollection();
            FocusedSampleRowHandle = pre_MyFocusedSampelRowIndex;
            ShowMainDatasLoadingPanel = false;
        }

    }
}