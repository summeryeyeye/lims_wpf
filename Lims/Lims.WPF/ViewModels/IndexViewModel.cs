using DevExpress.Mvvm.POCO;
using Lims.Common.Dtos;

namespace Lims.WPF.ViewModels
{
    public class IndexViewModel : DocumentViewModelBase
    {
        public IndexViewModel()
        {

        }


        public static IndexViewModel Create(string caption)
        {
            return ViewModelSource.Create(() => new IndexViewModel()
            {
                Caption = caption,
            });
        }

        protected override Task LoadMainDatas(UserDto? user)
        {
            return Task.CompletedTask;
            //throw new System.NotImplementedException();
        }
    }
}