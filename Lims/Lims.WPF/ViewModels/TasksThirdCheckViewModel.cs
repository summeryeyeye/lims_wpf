using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using Lims.Common.Dtos;
using Lims.ToolsForClient;

namespace Lims.WPF.ViewModels
{
    public class TasksThirdCheckViewModel : TasksCheckViewModelBase
    {
        public static TasksThirdCheckViewModel Create(string caption)
        {
            return ViewModelSource.Create(() => new TasksThirdCheckViewModel()
            {
                Caption = caption,
            });
        }

        public override TestProgress GetRelativeProgress()
        {
            return TestProgress.数据三审;
        }
        #region 任务审核
        protected virtual async Task PassChecking(SampleDto sample)
        {
            //var newProgress = (int)RelativeProgress + 1;
            foreach (var item in sample.Items)
                item.TestProgress = (int)TestProgress.已完成;
            await _itemService.UpdateRangeAsync(sample.Items.ToList());

            sample.CompleteTime = DateTimeOffset.Now;
            await _sampleService.UpdateAsync(sample);

            //await UpdateSampleTestProgress(sample);
            SamplesSource.Remove(sample);
            await ShowNotifaction(sample.SampleCode, "审核完成！", "");
        }
        [Command]
        public async Task ShowCheckTaskPreview(SampleDto sample)
        {
            try
            {
                AllItemsOfFocusedSample = await GetAllItemsOfSample(sample);
                var dialogService = GetService<IDialogService>("AllItemsOfSamplePreviewDialogService");
                dialogService.ShowDialog(
                    new List<UICommand> {
                        new UICommand{Caption = "审核通过",IsDefault = true,IsCancel = false,Command = new DelegateCommand(async () =>
                        {
                          await  PassChecking(sample);
                        })},
                        new UICommand{Caption = "取消",IsDefault = false,IsCancel = true,}
                    }
                    , "", this);
            }
            catch (System.Exception)
            {
            }
        }
        #endregion 任务审核



        [Command]
        public async Task MultiSamplesPassChecking(ObservableCollection<SampleDto> samples)
        {
            MessageResult result = _messageBoxService.ShowMessage($"确定批量审核选中样品(已选中 {samples.Count()} 个样品)？", "审核", MessageButton.OKCancel, MessageIcon.Question, MessageResult.Cancel);
            if (result != MessageResult.OK)
                return;
            if (!CurrentUser.CanThirdCheck)
                return;
            await ShowNotifaction("", "进程已启动,请稍等！", "");

            DateTimeOffset now = DateTimeOffset.Now;
            //SampleDto[] samples = new SampleDto[SelectedSamples.Count()];
            //SelectedSamples.CopyTo(samples, 0);

            foreach (SampleDto sample in samples)
            {
                //if (sample.Items.Min(i => i.TestProgress) != (int)TestProgress.数据三审)
                //    continue;
                sample.CompleteTime = now;

                await _sampleService.UpdateAsync(sample);

                foreach (var i in sample.Items)
                {
                    i.TestProgress = (int)RelativeProgress + 1;
                    await _itemService.UpdateAsync(i);
                }


                SamplesSource.Remove(SamplesSource.FirstOrDefault(s => s.SampleCode == sample.SampleCode));

                //await UpdateSampleTestProgress(sample);

            }
            await ShowNotifaction("", "审核完成！", "");
        }

    }
}