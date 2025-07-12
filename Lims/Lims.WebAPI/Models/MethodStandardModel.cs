using SqlSugar;

namespace Lims.WebAPI.Models
{
    [SugarTable("MethodStandardModel")]
    [Serializable]
    public class MethodStandardModel : StandardBaseModel
    {
        /// <summary>
        /// 主键，自增ID
        /// </summary>
        [SugarColumn(ColumnName = "Id", ColumnDataType = "int4", IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        /// <summary>
        /// 检测项目
        /// </summary>
        [SugarColumn(ColumnName = "TestItem", ColumnDataType ="varchar", IsNullable = false)]
        public string? TestItem { get; set; }

        /// <summary>
        /// 检测方法
        /// </summary>
        [SugarColumn(ColumnName = "TestMethod", ColumnDataType ="varchar", IsNullable = false)]
        public string? TestMethod { get; set; }

        /// <summary>
        /// 检测人
        /// </summary>
        [SugarColumn(ColumnName = "Tester", ColumnDataType ="varchar", IsNullable = false)]
        public string? Tester { get; set; }

        /// <summary>
        /// 样品状态
        /// </summary>
        [SugarColumn(ColumnName = "SampleState", ColumnDataType ="varchar", IsNullable = false, DefaultValue = "固体")]
        public string? SampleState { get; set; }

        /// <summary>
        /// 检测单位
        /// </summary>
        [SugarColumn(ColumnName = "TestUnit", ColumnDataType ="varchar", IsNullable = true)]
        public string? TestUnit { get; set; }

        /// <summary>
        /// 四舍五入规则
        /// </summary>
        [SugarColumn(ColumnName = "RoundRule", ColumnDataType ="varchar", IsNullable = true)]
        public string? RoundRule { get; set; }

        /// <summary>
        /// 关键项目
        /// </summary>
        [SugarColumn(ColumnName = "KeyItem", ColumnDataType ="varchar", IsNullable = true)]
        public string? KeyItem { get; set; }

        /// <summary>
        /// 最后更新人
        /// </summary>
        [SugarColumn(ColumnName = "LastUpdater", ColumnDataType ="varchar", IsNullable = false)]
        public string? LastUpdater { get; set; }

        #region 原始记录

        /// <summary>
        /// 原始记录模板文件路径
        /// </summary>
        [SugarColumn(ColumnName = "OriginalRecordTemplateFilePath", ColumnDataType ="varchar", DefaultValue = "0|0")]
        public string? OriginalRecordTemplateFilePath { get; set; }

        /// <summary>
        /// 仪器
        /// </summary>
        [SugarColumn(ColumnName = "Instruments", ColumnDataType ="varchar")]
        public string? Instruments { get; set; }

        /// <summary>
        /// 是否打印子项目
        /// </summary>
        [SugarColumn(ColumnName = "IsPrintSubItem", ColumnDataType = "bool")]
        public bool IsPrintSubItem { get; set; }

        /// <summary>
        /// 原始记录分组
        /// </summary>
        [SugarColumn(ColumnName = "OriginalRecordGroup", ColumnDataType ="varchar")]
        public string? OriginalRecordGroup { get; set; }

        #endregion 原始记录
    }
}
