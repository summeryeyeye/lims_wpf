using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.POCO;
using Lims.Common.Dtos;
using Lims.Common.Parameters;
using Lims.ToolsForClient;
using Lims.WPF.Services;

namespace Lims.WPF.ViewModels
{
    public class MyReceivableTasksViewModel : MyTasksViewModelBase
    {
        public static MyReceivableTasksViewModel Create(string caption)
        {
            return ViewModelSource.Create(() => new MyReceivableTasksViewModel()
            {
                Caption = caption,
            });
        }
        protected override async Task LoadMainDatas(UserDto? user)
        {
            ShowMainDatasLoadingPanel = true;
            ItemFilterParam itemFilterParam = new ItemFilterParam()
            {
                TestProgress = (int)RelativeProgress,
                Tester = user.UserName,
                Operation = Operation.Equal,
            };
            var response = await _itemService.GetMyItemsAsync(itemFilterParam);
            if (response.Status)
                TaskDatasSource = response.Result.OrderByDescending(t => t.AppointTime).ToObservableCollection();
            //await base.LoadMainDatas();
            ShowMainDatasLoadingPanel = false;
        }
        public override TestProgress GetRelativeProgress()
        {
            return TestProgress.待领取;
        }

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

        

    }
}