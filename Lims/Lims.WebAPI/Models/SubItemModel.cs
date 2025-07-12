using SqlSugar;

namespace Lims.WebAPI.Models
{
    [SugarTable("SubItemModel")]
    [Serializable]
    public class SubItemModel : BaseModel
    {
        /// <summary>
        /// 子项目ID（主键）
        /// </summary>
        [SugarColumn(ColumnName = "SubItemId", ColumnDataType = "varchar", IsPrimaryKey = true, IsNullable = false)]
        public string SubItemId { get; set; }

        /// <summary>
        /// 所属项目ID
        /// </summary>
        [SugarColumn(ColumnName = "ItemId", ColumnDataType = "varchar", IsNullable = false)]
        public string ItemId { get; set; }

        /// <summary>
        /// 检测项目
        /// </summary>
        [SugarColumn(ColumnName = "TestItem", ColumnDataType = "varchar", IsNullable = false, DefaultValue = "/")]
        public string? TestItem { get; set; }

        /// <summary>
        /// 临时检测结果
        /// </summary>
        [SugarColumn(ColumnName = "Temp_TestResult", ColumnDataType = "varchar", IsNullable = true)]
        public string? Temp_TestResult { get; set; }

        /// <summary>
        /// 检测结果
        /// </summary>
        [SugarColumn(ColumnName = "TestResult", ColumnDataType = "varchar", IsNullable = true)]
        public string? TestResult { get; set; }

        /// <summary>
        /// 第一次检测结果
        /// </summary>
        [SugarColumn(ColumnName = "FirstTestResult", ColumnDataType = "varchar", IsNullable = true)]
        public string? FirstTestResult { get; set; }

        /// <summary>
        /// 第二次检测结果
        /// </summary>
        [SugarColumn(ColumnName = "SecondTestResult", ColumnDataType = "varchar", IsNullable = true)]
        public string? SecondTestResult { get; set; }

        /// <summary>
        /// 平均检测结果
        /// </summary>
        [SugarColumn(ColumnName = "AverageTestResult", ColumnDataType = "varchar", IsNullable = true)]
        public string? AverageTestResult { get; set; }

        /// <summary>
        /// 项目备注
        /// </summary>
        [SugarColumn(ColumnName = "ItemRemark", ColumnDataType = "varchar", IsNullable = true)]
        public string? ItemRemark { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(ColumnName = "CreateTime", ColumnDataType = "timestamptz", IsNullable = false)]
        public DateTimeOffset CreateTime { get; set; }

        /// <summary>
        /// 指标要求
        /// </summary>
        [SugarColumn(ColumnName = "IndexRequest", ColumnDataType = "varchar", IsNullable = false, DefaultValue = "/")]
        public string? IndexRequest { get; set; }
    }
}