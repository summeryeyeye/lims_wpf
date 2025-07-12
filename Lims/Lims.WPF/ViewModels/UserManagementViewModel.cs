using DevExpress.Mvvm.POCO;
using Lims.Common.Dtos;

namespace Lims.WPF.ViewModels
{
    public class UserManagementViewModel : DocumentViewModelBase
    {
        public static UserManagementViewModel Create(string caption)
        {
            return ViewModelSource.Create(() => new UserManagementViewModel()
            {
                Caption = caption,
            });
        }
        protected override Task LoadMainDatas(UserDto? user)
        {
            return Task.CompletedTask;
        }
    }
}