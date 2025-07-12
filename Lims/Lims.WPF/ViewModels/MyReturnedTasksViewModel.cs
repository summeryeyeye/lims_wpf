using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using Lims.Common.Dtos;
using Lims.ToolsForClient;
using Lims.WPF.Resources;

namespace Lims.WPF.ViewModels
{
    public class MyReturnedTasksViewModel : MyTasksViewModelBase
    {
        public static MyReturnedTasksViewModel Create(string caption)
        {
            return ViewModelSource.Create(() => new MyReturnedTasksViewModel()
            {
                Caption = caption,
            });
        }

        public override TestProgress GetRelativeProgress()
        {
            return TestProgress.任务已退回;
        }


        ///// <summary>
        ///// 领取任务
        ///// </summary>
        ///// <returns></returns>     
        [Command]
        public async Task ReceiveTaskDatas()
        {
            try
            {
                ShowMainDatasLoadingPanel = true;
                ItemDto[] tasks = new ItemDto[selectedTaskDatas.Count];
                selectedTaskDatas.CopyTo(tasks, 0);
                foreach (var task in tasks)
                {
                    task.TestProgress = (int)TestProgress.检测中;
                    TaskDatasSource.Remove(task);
                }
                await _itemService.UpdateRangeAsync(tasks.ToList());
            }
            catch (Exception)
            {

                // throw;
            }
            finally
            {
                ShowMainDatasLoadingPanel = false;
            }
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