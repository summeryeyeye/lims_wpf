using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using Lims.Common.Dtos;
using Lims.ToolsForClient;

namespace Lims.WPF.ViewModels
{
    public class TasksFirstCheckViewModel : TasksCheckViewModelBase
    {


        public static TasksFirstCheckViewModel Create(string caption)
        {
            return ViewModelSource.Create(() => new TasksFirstCheckViewModel()
            {
                Caption = caption,
            });
        }

        public override TestProgress GetRelativeProgress()
        {
            return TestProgress.数据一审;
        }

        #region 任务审核
        protected virtual async Task PassChecking(SampleDto sample)
        {
            //var newProgress = (int)RelativeProgress + 1;

            foreach (var item in sample.Items)            
                item.TestProgress = (int)TestProgress.数据二审;

            await _itemService.UpdateRangeAsync(sample.Items.ToList());
            sample.FirstAuditTime = DateTimeOffset.Now;
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



    }
}