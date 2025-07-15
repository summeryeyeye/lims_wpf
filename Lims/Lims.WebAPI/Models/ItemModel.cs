using SqlSugar;
using System.Collections.ObjectModel;

namespace Lims.WebAPI.Models
{   
    [Serializable]
    public class ItemModel : BaseModel
    {
        //[SugarColumn(IsIgnore = true)]
        //public static IList<ItemModel> ReportSource { get; set; }

        [SugarColumn(ColumnDataType = "varchar", IsPrimaryKey = true, IsNullable = false)]
        public string? ItemId { get; set; }

        #region 产品属性
        [SugarColumn(ColumnDataType = "int4", IsNullable = false)]
        public int ProductStandardId { get; set; }

        [Navigate(NavigateType.OneToOne, nameof(ProductStandardId))]
        public ProductStandardModel? ProductStandard { get; set; }
        #endregion

        #region 方法属性
        [SugarColumn(ColumnDataType = "int4", IsNullable = false)]
        public int MethodStandardId { get; set; }

        [Navigate(NavigateType.OneToOne, nameof(MethodStandardId))]
        public MethodStandardModel? MethodStandard { get; set; }
        #endregion

        [SugarColumn(ColumnDataType = "varchar", IsNullable = true)]
        public string? FirstSampleWeight { get; set; }

        [SugarColumn(ColumnDataType = "varchar", IsNullable = true)]
        public string? SecondSampleWeight { get; set; }

        [SugarColumn(ColumnDataType = "varchar", IsNullable = false)]
        public string? TestItem { get; set; }

        [SugarColumn(ColumnDataType = "varchar", IsNullable = false, DefaultValue = "/")]
        public string? Tester { get; set; }

        [SugarColumn(ColumnDataType = "varchar")]
        public string? ReportUnit { get; set; }

        [SugarColumn(ColumnDataType = "varchar")]
        public string? IndexRequest { get; set; }

        [SugarColumn(ColumnDataType = "varchar", IsNullable = false)]
        public string? SampleCode { get; set; }

        [SugarColumn(ColumnDataType = "timestamptz", IsNullable = false)]
        public DateTimeOffset AppointTime { get; set; }

        [SugarColumn(ColumnDataType = "int4", IsNullable = false, DefaultValue = "100")]
        public int PreTestProgress { get; set; }

        [SugarColumn(ColumnDataType = "int4", IsNullable = false, DefaultValue = "101")]
        public int TestProgress { get; set; }

        [SugarColumn(ColumnDataType = "varchar", IsNullable = true)]
        public string? Temp_TestResult { get; set; }

        [SugarColumn(ColumnDataType = "varchar", IsNullable = true)]
        public string? TestResult { get; set; }

        [SugarColumn(ColumnDataType = "varchar", IsNullable = true)]
        public string? ItemRemark { get; set; }

        [SugarColumn(ColumnDataType = "varchar", IsNullable = true, DefaultValue = "/")]
        public string? Temp_SingleConclusion { get; set; }

        [SugarColumn(ColumnDataType = "varchar", IsNullable = true, DefaultValue = "/")]
        public string? SingleConclusion { get; set; }

        [SugarColumn(ColumnDataType = "varchar", IsNullable = true)]
        public string? TestRemark { get; set; }

        [SugarColumn(ColumnDataType = "timestamptz", IsNullable = true)]
        public DateTimeOffset? ResultSubmitTime { get; set; }

        [SugarColumn(ColumnDataType = "timestamptz", IsNullable = true)]
        public DateTimeOffset? TestDate { get; set; }

        [SugarColumn(ColumnDataType = "bool", IsNullable = false, DefaultValue = "false")]
        public bool IsOriginalRecordComplete { get; set; }

        [SugarColumn(ColumnDataType = "bool", IsNullable = false, DefaultValue = "false")]
        public bool IsOverDate { get; set; }

        [Navigate(NavigateType.OneToMany, nameof(SubItemModel.ItemId))]
        public ObservableCollection<SubItemModel>? SubItems { get; set; }

        [Navigate(NavigateType.OneToOne, nameof(SampleCode))]
        public SampleModel? Sample { get; set; }

        //[SugarColumn(IsIgnore = true)]
        //public ObservableCollection<SubItemModel>? SelectedRetestSubItems
        //{
        //    get; set;
        //}       
    }

}