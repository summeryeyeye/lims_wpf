using SqlSugar;

namespace Lims.WebAPI.Models
{
    [Serializable]    public class SubItemStandardModel : BaseModel
    {
        /// <summary>
        /// 主键，自增ID
        /// </summary>
        [SugarColumn(ColumnDataType = "int4", IsNullable = false, IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        /// <summary>
        /// 子项目名称
        /// </summary>
        [SugarColumn(ColumnDataType = "varchar", IsNullable = false)]
        public string? SubitemName { get; set; }

        /// <summary>
        /// 父级名称（可多级）
        /// </summary>
        [SugarColumn(ColumnDataType = "varchar", IsNullable = false)]
        public string? ParentNames { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        [SugarColumn(ColumnDataType = "varchar", IsNullable = false)]
        public string? SubitemType { get; set; }

        /// <summary>
        /// 基质
        /// </summary>
        [SugarColumn(ColumnName = "Substratum", ColumnDataType = "varchar", IsNullable = true)]
        public string? Substratum { get; set; }

        /// <summary>
        /// 批号
        /// </summary>
        [SugarColumn(ColumnName = "BatchNumber", ColumnDataType = "varchar", IsNullable = true)]
        public string? BatchNumber { get; set; }

        /// <summary>
        /// 厂家
        /// </summary>
        [SugarColumn(ColumnName = "Campany", ColumnDataType = "varchar", IsNullable = true)]
        public string? Campany { get; set; }

        /// <summary>
        /// 仪器
        /// </summary>
        [SugarColumn(ColumnName = "Instruments", ColumnDataType = "varchar", IsNullable = true)]
        public string? Instruments { get; set; }
    }
}