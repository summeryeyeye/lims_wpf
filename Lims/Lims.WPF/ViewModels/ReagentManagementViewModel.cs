using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.POCO;
using Lims.Common.Dtos;

namespace Lims.WPF.ViewModels
{
    public class ReagentManagementViewModel : DocumentViewModelBase
    {
        private ObservableCollection<ReagentDto?> reagents = new ObservableCollection<ReagentDto?>();

        public ObservableCollection<ReagentDto?> Reagents
        {
            get { return reagents; }
            set { reagents = value; RaisePropertyChanged(nameof(Reagents)); }
        }
        public ReagentManagementViewModel()
        {
            _ = LoadMainDatas(CurrentUser);
        }

        protected override async Task LoadMainDatas(UserDto? user)
        {
            Reagents = (await _iReagentService.GetAllAsync()).Result.ToObservableCollection();
            //throw new NotImplementedException();
        }
        public static ReagentManagementViewModel Create(string caption)
        {
            return ViewModelSource.Create(() => new ReagentManagementViewModel()
            {
                Caption = caption,
            });
        }
    }
}
