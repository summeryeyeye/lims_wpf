using SqlSugar;

namespace Lims.WebAPI.Models
{   
    [Serializable]
    public class LoggerModel : BaseModel
    {
        /// <summary>
        /// 日志主键ID，自增
        /// </summary>
        [SugarColumn(ColumnDataType = "int4", IsPrimaryKey = true, IsNullable = false, IsIdentity = true)]
        public int Id { get; set; }

        /// <summary>
        /// 日志创建时间
        /// </summary>
        [SugarColumn(ColumnDataType = "timestamptz", IsNullable = false)]
        public DateTimeOffset CreateTime { get; set; }

        /// <summary>
        /// 日志级别
        /// </summary>
        [SugarColumn(ColumnDataType = "int4", IsNullable = false)]
        public LogLevel LogLevel { get; set; }

        /// <summary>
        /// 操作类型
        /// </summary>
        [SugarColumn(ColumnDataType = "int4", IsNullable = false)]
        public ActionType ActionType { get; set; }

        /// <summary>
        /// 发布者IP
        /// </summary>
        [SugarColumn(ColumnDataType = "varchar", IsNullable = false)]
        public string? PublisherIP { get; set; }

        /// <summary>
        /// 发布者名称
        /// </summary>
        [SugarColumn(ColumnDataType = "varchar", IsNullable = false)]
        public string? PublisherName { get; set; }

        /// <summary>
        /// 接收者名称
        /// </summary>
        [SugarColumn(ColumnDataType = "varchar", IsNullable = false)]
        public string? ReceiverName { get; set; }

        /// <summary>
        /// 样品编号
        /// </summary>
        [SugarColumn(ColumnDataType = "varchar", IsNullable = false)]
        public string? SampleCode { get; set; }

        /// <summary>
        /// 检测项目
        /// </summary>
        [SugarColumn(ColumnDataType = "varchar", IsNullable = false)]
        public string? TestItem { get; set; }

        /// <summary>
        /// 日志消息内容
        /// </summary>
        [SugarColumn(ColumnDataType = "varchar", IsNullable = true)]
        public string? Message { get; set; }

        /// <summary>
        /// 是否已读
        /// </summary>
        [SugarColumn(ColumnDataType = "bool", IsNullable = false)]
        public bool IsReaded { get; set; }
    }

    public enum LogLevel
    {
        OFF = 0,
        FATAL = 1,
        ERROR = 2,
        WARN = 3,
        INFO = 4,
        DEBUG = 5,
        ALL = 6,
    }

    public enum ActionType
    {
        删除任务 = 0,
        变更项目信息 = 1,
        退回任务 = 2,
        变更项目分析人 = 3,
        变更样品信息 = 4,
    }
}