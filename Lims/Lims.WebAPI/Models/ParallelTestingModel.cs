using SqlSugar;

namespace Lims.WebAPI.Models
{
    [Serializable]
    public class ParallelTestingModel
    {
        [SugarColumn(ColumnDataType = "int4", IsPrimaryKey = true, IsNullable = false, IsIdentity = true)]
        public int Id { get; set; }

        [SugarColumn(ColumnDataType = "varchar", IsNullable = false)]
        public string? ParentId { get; set; }

        [SugarColumn(ColumnDataType = "varchar")]
        public string? SampleWeight { get; set; }

        [SugarColumn(ColumnDataType = "varchar")]
        public string? TestResult { get; set; }

        [SugarColumn(ColumnDataType = "varchar", IsNullable = false)]
        public int ParallelIndex { get; set; }

        [SugarColumn(ColumnDataType = "varchar")]
        public string? AttachmentPath { get; set; }

    }
}
