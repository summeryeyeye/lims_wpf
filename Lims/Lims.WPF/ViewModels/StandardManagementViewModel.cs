using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.POCO;
using DevExpress.Mvvm.Xpf;
using Lims.Common.Dtos;
using System.Windows.Input;

namespace Lims.WPF.ViewModels
{
    public class StandardManagementViewModel : DocumentViewModelBase
    {

        public static StandardManagementViewModel Create(string caption)
        {
            return ViewModelSource.Create(() => new StandardManagementViewModel()
            {
                Caption = caption,
            });
        }

        protected override async void OnInitializeInRuntime()
        {
            base.OnInitializeInRuntime();
            await LoadMainDatas(CurrentUser);
            //await GetTesterNames();
        }

        [Command]
        public async Task UpdateMethodStandardState(int standarState)
        {
            FocusedMethod.StandardState = (StandardState)standarState;
            await _methodStandardService.UpdateAsync(FocusedMethod);
        }


        protected async override Task LoadMainDatas(UserDto? user)
        {
            Products = (await _productStandardService.GetAllAsync()).Result.ToObservableCollection();
            Methods = (await _methodStandardService.GetAllAsync()).Result.ToObservableCollection();
        }

        ///// <summary>
        ///// 测试命令
        ///// </summary>
        //[Command]
        //public async Task Test()
        //{
        //    //List<SampleDto> samples = await _sampleService.GetWithChildObjectsAsync(s => s.MinTestProgress > (int)TestProgress.已完成);

        //    //foreach (var sample in samples)
        //    //{
        //    //    sample.MinTestProgress = sample.Items.Min(i => i.TestProgress);
        //    //    sample.MaxTestProgress = sample.Items.Max(selector: i => i.TestProgress);
        //    //}
        //    //await _sampleService.EditRangeAsync(samples);
        //    //_messageBoxService.ShowMessage("done");
        //}

        /// <summary>
        /// 测试命令
        /// </summary>
        [Command]
        public void RefreshSampleItemInfo()
        {
            //inputBoxText=String.Empty;
            //var dialogService = GetService<IDialogService>("InPutBoxViewDialogService");
            //dialogService.ShowDialog(
            //    new List<UICommand> {
            //            new UICommand{Caption = "更新",IsDefault = true,IsCancel = false,Command = new DelegateCommand(async () =>
            //            {
            //                string TipText = inputBoxText;
            //                SampleDto sample=await _iSampleService.SearchAsync(TipText);
            //                if(sample==null)
            //                    return;
            //               var items = await _iItemService.QueryAsync(i => i.SampleCode == TipText);
            //                List<string> infoList = new();
            //                foreach (var item in items)
            //                {
            //                    string info = $"{item.TestItem}&{item.Tester}&{item.TestProgress}&{ValidationHelper.ResultIsSubmittable(item.Temp_TestResult)}&{item.IsOriginalRecordComplete}";
            //                    infoList.Add(info);
            //                }
            //                string infos = string.Join("|", infoList);
            //                //sample.ItemInfo = infos;
            //            //}
            //                //await _iSampleService.EdItAsync(sample);
            //               _messageBoxService.ShowMessage("done");
            //            })},
            //            new UICommand{Caption = "取消",IsDefault = false,IsCancel = true,}
            //    }
            //    , "", this);
        }

        #region MyRegion
        private ObservableCollection<ProductStandardDto> products;
        public ObservableCollection<ProductStandardDto> Products
        {
            get => products; set
            {
                products = value;
                RaisePropertyChanged(nameof(Products));
            }
        }
        public ProductStandardDto FocussedProduct
        {
            get; set;
        }

        private ObservableCollection<MethodStandardDto> methods;
        public ObservableCollection<MethodStandardDto> Methods
        {
            get => methods; set
            {
                methods = value;
                RaisePropertyChanged(nameof(Methods));
            }
        }
        public ObservableCollection<ProductStandardDto> SelectedProducts
        {
            get;
            set;
        } = new();

        [Command]
        public async Task Tongbu()
        {
            ProductStandardDto[] products = new ProductStandardDto[SelectedProducts.Count];
            SelectedProducts.CopyTo(products, 0);

            foreach (var product in products)
            {
                product.TestMethodId = FocusedMethod.Id;
                SelectedProducts.Remove(product);
            }
            await _productStandardService.UpdateRangeAsync(products.ToList());
        }

        [Command]
        public async Task MethodEdit(CellValueChangedArgs e)
        {
            var method = e.Item as MethodStandardDto;
            method.LastUpdater = CurrentUser.UserName;
            await _methodStandardService.UpdateAsync(method);

        }
        [Command]
        public async Task RefreshMethod()
        {
            Products = (await _productStandardService.GetAllAsync()).Result.ToObservableCollection();
            Methods = (await _methodStandardService.GetAllAsync()).Result.ToObservableCollection();
        }
        public MethodStandardDto FocusedMethod
        {
            get;
            set;
        }
        [Command]
        public async Task StandardKeyUp(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Delete:
                    if (_messageBoxService.ShowMessage("确定删除该方法?", "", MessageButton.OKCancel) == MessageResult.OK)
                    {
                        var response = await _itemService.GetAllItemsByMethodStandardIdAsync(new Common.Parameters.ItemFilterParam { MethodStandardId = FocusedMethod.Id });
                        if (response.Status && response.Result.Count > 0)
                        {
                            _messageBoxService.ShowMessage("存在项目引用该方法，请更正后重试!");
                            return;
                        }

                        await _methodStandardService.DeleteAsync(FocusedMethod.Id);
                        Methods.Remove(FocusedMethod);
                    }
                    break;
            }
        }

        public ObservableCollection<SubItemStandardDto> SubItemStandardsSource
        {
            get; set;
        }

        [Command]
        public async Task ShowEditStandardSubItemView()
        {
            SubItemStandardsSource = (await _subItemStandardService.GetAllAsync()).Result.ToObservableCollection();
            var dialogService = GetService<IDialogService>("EditStandardSubItemViewDialogService");
            dialogService.ShowDialog(
                new List<UICommand> {
                        new UICommand{Caption = "取消",IsDefault = false,IsCancel = true,}
                }
                , "", this);

        }
        [Command]
        public async Task StandardSubItemChangedEdit(CellValueChangedArgs e)
        {
            var standardSubItem = e.Item as SubItemStandardDto;
            await _subItemStandardService.UpdateAsync(standardSubItem);
        }

        #endregion
    }
}