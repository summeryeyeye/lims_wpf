using DevExpress.DataAccess.Native.Web;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.Xpf;
using Lims.Common.Dtos;
using Lims.Common.Parameters;
using NPOI.Util;

namespace Lims.WPF.ViewModels
{
    public abstract class TasksCheckViewModelBase : SampleAndItemDataViewModelBase
    {
        protected override async void ShowItemsOfFocusedSample(SampleDto sample)
        {
            try
            {
                ShowItemGridLoadingPanel = true;
                ItemsSource = await GetItemsSource(sample);
                sample.Items = ItemsSource;
                ShowItemGridLoadingPanel = false;
            }
            catch (System.Exception)
            {
                // throw;
            }
        }

        protected override async Task LoadMainDatas(UserDto? user)
        {
            ShowMainDatasLoadingPanel = true;
            pre_MyFocusedSampelRowIndex = FocusedSampleRowHandle;


            var itemFileterParam = new ItemFilterParam
            {
                TestProgress = (int)RelativeProgress,
                Operation = Operation.Equal,
            };
            var itemResponse = await _itemService.GetAllItemsByTestProgressAsync(itemFileterParam);
            if (itemResponse.Status)
            {
                TaskDatasSource = itemResponse.Result.OrderByDescending(t => t.SampleCode).ToObservableCollection();
                itemDtos = TaskDatasSource?.Copy();
                SamplesSource = TaskDatasSource.Select(s => s.Sample).DistinctBy(s => s.SampleCode).OrderBy(s => s.SampleCode).ToObservableCollection();
            }




            //List<string>? samples = new List<string>();
            //if (itemResponse.Status)
            //    samples = itemResponse.Result.DistinctBy(i => i.SampleCode).Select(i => i.SampleCode).ToList();



            //var sampleFilterParam = new SampleFilterParam
            //{
            //    SampleCodes = string.Join(',', samples),
            //    //RelativeProgress = (int)RelativeProgress,
            //    //Operation = Operation.Equal,
            //};
            //var response = await _sampleService.GetSamplesBySampleCodesAsync(sampleFilterParam);
            //if (response.Status)
            //    SamplesSource = response.Result.OrderByDescending(s => s.SampleCode).ToObservableCollection();





            FocusedSampleRowHandle = pre_MyFocusedSampelRowIndex;
            ShowMainDatasLoadingPanel = false;
        }

        [Command]
        public override  async Task RefreshItemDatas(SampleDto sample)
        {
            if (sample != null)
            {
                 ItemsSource = await GetAllItemsOfSample(sample);
                //ItemFilterParam itemFilterParam = new ItemFilterParam()
                //{
                //    SampleCode = sample.SampleCode,
                //    Tester = CurrentUser.UserName,
                //    TestProgress = (int)RelativeProgress,
                //    Operation = Operation.Higher,
                //    MinDate = Sample_BeginDate,
                //    MaxDate = Sample_EndDate
                //};
                //ItemsSource = (await _itemService.GetMyItemsBySampleCodeAsync(itemFilterParam)).Result.ToObservableCollection();
            }
        }



        [Command]
        public void ShowReportsDataPreview(ObservableCollection<SampleDto> samples)
        {
            foreach (var sample in samples)
            {
                ShowAllItemsOfCurrentSample(sample);
            }
        }

        [Command]
        public override void ShowAllItemsOfCurrentSample(SampleDto sample)
        {
            ReportDataPreview(sample);
        }

        [Command]
        public void ReturnTaskComfirmWindow(ItemDto edittingItem)
        {

            var dialogService = GetService<IDialogService>("InPutBoxViewDialogService");
            dialogService.ShowDialog(
                new List<UICommand> {
                        new UICommand{Caption = "确认退回",IsDefault = true,IsCancel = false,Command = new DelegateCommand(async () =>
                        {

                            var editingSample =FocusedSample; //SamplesSource.FirstOrDefault(s=>s.SampleCode==edittingItem.SampleCode); 


                           ReturnTask(edittingItem,editingSample,InputBoxText,edittingItem.TestProgress);

                            var edittingItems=await GetAllItemsOfSample(editingSample);
                            if (edittingItems.All(i=>i.TestProgress<(int)RelativeProgress))
                            {
                              SamplesSource.Remove(editingSample);
                            }else
                            {
                                editingSample.Items=edittingItems;
                            }


                            await ShowNotifaction(edittingItem.SampleCode + "  " + edittingItem.TestItem, "任务退回成功！", "");

                        })},
                        new UICommand{Caption = "取消",IsDefault = false,IsCancel = true,}
                }
                , "", this);
        }

        [Command]
        public async Task EditChekingRemark(CellValueChangedArgs e)
        {
            SampleDto sample = e.Item as SampleDto;
            await _sampleService.UpdateAsync(sample);
        }
    }
}