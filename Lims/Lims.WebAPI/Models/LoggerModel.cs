using SqlSugar;

namespace Lims.WebAPI.Models
{
    [SugarTable("LoggerModel")]
    [Serializable]
    public class LoggerModel : BaseModel
    {
        /// <summary>
        /// 日志主键ID，自增
        /// </summary>
        [SugarColumn(ColumnName = "Id", ColumnDataType = "int4", IsPrimaryKey = true, IsNullable = false, IsIdentity = true)]
        public int Id { get; set; }

        /// <summary>
        /// 日志创建时间
        /// </summary>
        [SugarColumn(ColumnName = "CreateTime", ColumnDataType = "timestamptz", IsNullable = false)]
        public DateTimeOffset CreateTime { get; set; }

        /// <summary>
        /// 日志级别
        /// </summary>
        [SugarColumn(ColumnName = "LogLevel", ColumnDataType = "int4", IsNullable = false)]
        public LogLevel LogLevel { get; set; }

        /// <summary>
        /// 操作类型
        /// </summary>
        [SugarColumn(ColumnName = "ActionType", ColumnDataType = "int4", IsNullable = false)]
        public ActionType ActionType { get; set; }

        /// <summary>
        /// 发布者IP
        /// </summary>
        [SugarColumn(ColumnName = "PublisherIP", ColumnDataType = "varchar", IsNullable = false)]
        public string? PublisherIP { get; set; }

        /// <summary>
        /// 发布者名称
        /// </summary>
        [SugarColumn(ColumnName = "PublisherName", ColumnDataType = "varchar", IsNullable = false)]
        public string? PublisherName { get; set; }

        /// <summary>
        /// 接收者名称
        /// </summary>
        [SugarColumn(ColumnName = "ReceiverName", ColumnDataType = "varchar", IsNullable = false)]
        public string? ReceiverName { get; set; }

        /// <summary>
        /// 样品编号
        /// </summary>
        [SugarColumn(ColumnName = "SampleCode", ColumnDataType = "varchar", IsNullable = false)]
        public string? SampleCode { get; set; }

        /// <summary>
        /// 检测项目
        /// </summary>
        [SugarColumn(ColumnName = "TestItem", ColumnDataType = "varchar", IsNullable = false)]
        public string? TestItem { get; set; }

        /// <summary>
        /// 日志消息内容
        /// </summary>
        [SugarColumn(ColumnName = "Message", ColumnDataType = "varchar", IsNullable = true)]
        public string? Message { get; set; }

        /// <summary>
        /// 是否已读
        /// </summary>
        [SugarColumn(ColumnName = "IsReaded", ColumnDataType = "bool", IsNullable = false)]
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