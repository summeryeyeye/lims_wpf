using SqlSugar;
using System.Collections.ObjectModel;

namespace Lims.WebAPI.Models
{
    [SugarTable("ItemModel")]
    [Serializable]
    public class ItemModel : BaseModel
    {
        //[SugarColumn(IsIgnore = true)]
        //public static IList<ItemModel> ReportSource { get; set; }

        [SugarColumn(ColumnName = "ItemId", ColumnDataType = "varchar", IsPrimaryKey = true, IsNullable = false)]
        public string? ItemId { get; set; }

        #region 产品属性
        [SugarColumn(ColumnName = "ProductStandardId", ColumnDataType = "int4", IsNullable = false)]
        public int ProductStandardId { get; set; }

        [Navigate(NavigateType.OneToOne, nameof(ProductStandardId))]
        public ProductStandardModel? ProductStandard { get; set; }
        #endregion

        #region 方法属性
        [SugarColumn(ColumnName = "MethodStandardId", ColumnDataType = "int4", IsNullable = false)]
        public int MethodStandardId { get; set; }

        [Navigate(NavigateType.OneToOne, nameof(MethodStandardId))]
        public MethodStandardModel? MethodStandard { get; set; }
        #endregion

        [SugarColumn(ColumnName = "FirstSampleWeight", ColumnDataType = "varchar", IsNullable = true)]
        public string? FirstSampleWeight { get; set; }

        [SugarColumn(ColumnName = "SecondSampleWeight", ColumnDataType = "varchar", IsNullable = true)]
        public string? SecondSampleWeight { get; set; }

        [SugarColumn(ColumnName = "TestItem", ColumnDataType = "varchar", IsNullable = false)]
        public string? TestItem { get; set; }

        [SugarColumn(ColumnName = "Tester", ColumnDataType = "varchar", IsNullable = false, DefaultValue = "/")]
        public string? Tester { get; set; }

        [SugarColumn(ColumnName = "ReportUnit", ColumnDataType = "varchar")]
        public string? ReportUnit { get; set; }

        [SugarColumn(ColumnName = "IndexRequest", ColumnDataType = "varchar")]
        public string? IndexRequest { get; set; }

        [SugarColumn(ColumnName = "SampleCode", ColumnDataType = "varchar", IsNullable = false)]
        public string? SampleCode { get; set; }

        [SugarColumn(ColumnName = "AppointTime", ColumnDataType = "timestamptz", IsNullable = false)]
        public DateTimeOffset AppointTime { get; set; }

        [SugarColumn(ColumnName = "PreTestProgress", ColumnDataType = "int4", IsNullable = false, DefaultValue = "100")]
        public int PreTestProgress { get; set; }

        [SugarColumn(ColumnName = "TestProgress", ColumnDataType = "int4", IsNullable = false, DefaultValue = "101")]
        public int TestProgress { get; set; }

        [SugarColumn(ColumnName = "Temp_TestResult", ColumnDataType = "varchar", IsNullable = true)]
        public string? Temp_TestResult { get; set; }

        [SugarColumn(ColumnName = "TestResult", ColumnDataType = "varchar", IsNullable = true)]
        public string? TestResult { get; set; }

        [SugarColumn(ColumnName = "ItemRemark", ColumnDataType = "varchar", IsNullable = true)]
        public string? ItemRemark { get; set; }

        [SugarColumn(ColumnName = "Temp_SingleConclusion", ColumnDataType = "varchar", IsNullable = true, DefaultValue = "/")]
        public string? Temp_SingleConclusion { get; set; }

        [SugarColumn(ColumnName = "SingleConclusion", ColumnDataType = "varchar", IsNullable = true, DefaultValue = "/")]
        public string? SingleConclusion { get; set; }

        [SugarColumn(ColumnName = "TestRemark", ColumnDataType = "varchar", IsNullable = true)]
        public string? TestRemark { get; set; }

        [SugarColumn(ColumnName = "ResultSubmitTime", ColumnDataType = "timestamptz", IsNullable = true)]
        public DateTimeOffset? ResultSubmitTime { get; set; }

        [SugarColumn(ColumnName = "TestDate", ColumnDataType = "timestamptz", IsNullable = true)]
        public DateTimeOffset? TestDate { get; set; }

        [SugarColumn(ColumnName = "IsOriginalRecordComplete", ColumnDataType = "bool", IsNullable = false, DefaultValue = "FALSE")]
        public bool IsOriginalRecordComplete { get; set; }

        [SugarColumn(ColumnName = "IsOverDate", ColumnDataType = "bool", IsNullable = false, DefaultValue = "FALSE")]
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