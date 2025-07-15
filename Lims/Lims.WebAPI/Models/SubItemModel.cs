using SqlSugar;

namespace Lims.WebAPI.Models
{   
    [Serializable]
    public class SubItemModel : BaseModel
    {
        /// <summary>
        /// 子项目ID（主键）
        /// </summary>
        [SugarColumn(ColumnDataType = "varchar", IsPrimaryKey = true, IsNullable = false)]
        public string? SubItemId { get; set; }

        /// <summary>
        /// 所属项目ID
        /// </summary>
        [SugarColumn(ColumnDataType = "varchar", IsNullable = false)]
        public string? ItemId { get; set; }

        /// <summary>
        /// 检测项目
        /// </summary>
        [SugarColumn(ColumnDataType = "varchar", IsNullable = false, DefaultValue = "/")]
        public string? TestItem { get; set; }

        /// <summary>
        /// 临时检测结果
        /// </summary>
        [SugarColumn(ColumnDataType = "varchar", IsNullable = true)]
        public string? Temp_TestResult { get; set; }

        /// <summary>
        /// 检测结果
        /// </summary>
        [SugarColumn(ColumnDataType = "varchar", IsNullable = true)]
        public string? TestResult { get; set; }

        /// <summary>
        /// 第一次检测结果
        /// </summary>
        [SugarColumn(ColumnDataType = "varchar", IsNullable = true)]
        public string? FirstTestResult { get; set; }

        /// <summary>
        /// 第二次检测结果
        /// </summary>
        [SugarColumn(ColumnDataType = "varchar", IsNullable = true)]
        public string? SecondTestResult { get; set; }

        /// <summary>
        /// 平均检测结果
        /// </summary>
        [SugarColumn(ColumnDataType = "varchar", IsNullable = true)]
        public string? AverageTestResult { get; set; }

        /// <summary>
        /// 项目备注
        /// </summary>
        [SugarColumn(ColumnDataType = "varchar", IsNullable = true)]
        public string? ItemRemark { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(ColumnDataType = "timestamptz", IsNullable = false)]
        public DateTimeOffset CreateTime { get; set; }

        /// <summary>
        /// 指标要求
        /// </summary>
        [SugarColumn(ColumnDataType = "varchar", IsNullable = false, DefaultValue = "/")]
        public string? IndexRequest { get; set; }
    }
}