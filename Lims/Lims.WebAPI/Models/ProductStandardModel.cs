using SqlSugar;

namespace Lims.WebAPI.Models
{  
    [Serializable]
    public class ProductStandardModel : StandardBaseModel
    {
        /// <summary>
        /// 主键，自增ID
        /// </summary>
        [SugarColumn(ColumnDataType = "int4", IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        /// <summary>
        /// 执行标准
        /// </summary>
        [SugarColumn(ColumnDataType = "varchar", IsNullable = false)]
        public string? ExecuteStandard { get; set; }

        /// <summary>
        /// 产品类型
        /// </summary>
        [SugarColumn(ColumnDataType = "varchar", IsNullable = false, DefaultValue = "/")]
        public string? ProductType { get; set; }

        /// <summary>
        /// 产品类别
        /// </summary>
        [SugarColumn(ColumnDataType = "varchar", IsNullable = false, DefaultValue = "/")]
        public string? ProductClass { get; set; }

        /// <summary>
        /// 产品形态
        /// </summary>
        [SugarColumn(ColumnDataType = "varchar", IsNullable = false, DefaultValue = "固体")]
        public string? ProductForm { get; set; }

        /// <summary>
        /// 检测项目
        /// </summary>
        [SugarColumn(ColumnDataType = "varchar", IsNullable = false)]
        public string? TestItem { get; set; }

        /// <summary>
        /// 指标要求
        /// </summary>
        [SugarColumn(ColumnDataType = "varchar", IsNullable = false)]
        public string? IndexRequest { get; set; }

        /// <summary>
        /// 最后更新人
        /// </summary>
        [SugarColumn(ColumnDataType = "varchar", IsNullable = false)]
        public string? LastUpdater { get; set; }

        /// <summary>
        /// 产品单位
        /// </summary>
        [SugarColumn(ColumnDataType = "varchar", IsNullable = true)]
        public string? ProductUnit { get; set; }

        /// <summary>
        /// 检测方法ID
        /// </summary>
        [SugarColumn(ColumnDataType = "int4", IsNullable = true)]
        public int TestMethodId { get; set; }

        /// <summary>
        /// 样品状态（只读，自动判断）
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string SampleState
        {
            get
            {
                return ProductForm.Contains("液") ? "液体" : "固体";
            }
        }
    }
}
