using SqlSugar;

namespace Lims.WebAPI.Models
{
    [Serializable]
    public class ParallelTestingModel
    {

        [SugarColumn(ColumnDataType = "varchar", IsPrimaryKey = true, IsNullable = false)]
        public string? ItemId
        {
            get; set;
        }

        [SugarColumn(ColumnDataType = "varchar")]
        public string? FirstSampleWeight
        {
            get; set;
        }
        [SugarColumn(ColumnDataType = "varchar")]
        public string? SecondSampleWeight
        {
            get; set;
        }

        [SugarColumn(ColumnDataType = "varchar")]
        public string? FirstTestResult
        {
            get; set;
        }
        [SugarColumn(ColumnDataType = "varchar")]
        public string? SecondTestResult
        {
            get; set;
        }

        [SugarColumn(ColumnDataType = "varchar")]
        public string? FirstAttachmentPath
        {
            get; set;
        }
        [SugarColumn(ColumnDataType = "varchar")]
        public string? SecondAttachmentPath
        {
            get; set;
        }

    }
}
