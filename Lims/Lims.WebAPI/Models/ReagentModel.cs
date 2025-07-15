using SqlSugar;

namespace Lims.WebAPI.Models
{
    [Serializable]
    public class ReagentModel : BaseModel
    {
        /// <summary>
        /// 主键，自增ID
        /// </summary>
        [SugarColumn(ColumnDataType = "int4", IsPrimaryKey = true, IsNullable = false, IsIdentity = true)]
        public int Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [SugarColumn(ColumnDataType = "varchar", IsNullable = false)]
        public string RagentName { get; set; }

        /// <summary>
        /// 分子式
        /// </summary>
        [SugarColumn(ColumnDataType = "varchar")]
        public string? MolecularFormula { get; set; }

        /// <summary>
        /// CAS号
        /// </summary>
        [SugarColumn(ColumnDataType = "varchar")]
        public string? CASNumber { get; set; }

        /// <summary>
        /// 别名
        /// </summary>
        [SugarColumn(ColumnDataType = "varchar")]
        public string? Alias { get; set; }

        /// <summary>
        /// 位置
        /// </summary>
        [SugarColumn(ColumnDataType = "varchar")]
        public string Address { get; set; }

        /// <summary>
        /// 规格
        /// </summary>
        [SugarColumn(ColumnDataType = "varchar")]
        public string? Speciffication { get; set; }

        /// <summary>
        /// 纯度
        /// </summary>
        [SugarColumn(ColumnDataType = "varchar")]
        public string? Purity { get; set; }

        /// <summary>
        /// 库存数量
        /// </summary>
        [SugarColumn(ColumnDataType = "int4", IsNullable = false, DefaultValue = "1")]
        public int Count { get; set; }
    }
}
