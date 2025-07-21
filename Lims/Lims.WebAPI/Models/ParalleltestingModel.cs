using SqlSugar;

namespace Lims.WebAPI.Models
{
    [Serializable]
    public class ParalleltestingModel
    {
        [SugarColumn(ColumnDataType = "int4", IsPrimaryKey = true, IsNullable = false,IsIdentity =true)]
        public int Id { get; set; }
        [SugarColumn(ColumnDataType = "varchar", IsNullable = false)]
        public string? ParentId { get; set; }
        [SugarColumn(ColumnDataType = "varchar", IsNullable = false)]
        public string? Weight { get; set; }
        [SugarColumn(ColumnDataType = "int4", IsNullable = false)]
        public int Index { get; set; }
        [SugarColumn(ColumnDataType = "varchar", IsNullable = false)]
        public string? TestResult { get; set; }
        [SugarColumn(ColumnDataType = "varchar", IsNullable = false)]
        public string? AttachmentPath { get; set; }
    }
}
