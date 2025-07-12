using SqlSugar;

namespace Lims.WebAPI.Models
{
    [SugarTable("SubstratumModel")]
    [Serializable]
    public class SubstratumModel
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        [SugarColumn(ColumnName = "Id", ColumnDataType = "int4", IsPrimaryKey = true)]
        public int Id { get; set; }

        /// <summary>
        /// 基质名称
        /// </summary>
        [SugarColumn(ColumnName = "Name", ColumnDataType = "varchar", IsNullable = false)]
        public string Name { get; set; }

        /// <summary>
        /// 批号
        /// </summary>
        [SugarColumn(ColumnName = "BatchNumber", ColumnDataType = "varchar", IsNullable = false)]
        public string BatchNumber { get; set; }

        /// <summary>
        /// 厂家
        /// </summary>
        [SugarColumn(ColumnName = "Campany", ColumnDataType = "varchar", IsNullable = false)]
        public string Campany { get; set; }

        /// <summary>
        /// 启用日期
        /// </summary>
        [SugarColumn(ColumnName = "BeginDate", ColumnDataType = "date", IsNullable = true)]
        public DateOnly BeginDate { get; set; }

        /// <summary>
        /// 停用日期
        /// </summary>
        [SugarColumn(ColumnName = "EndDate", ColumnDataType = "date", IsNullable = true)]
        public DateOnly EndDate { get; set; }
    }
}
