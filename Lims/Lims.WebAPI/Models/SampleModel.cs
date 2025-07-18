﻿using SqlSugar;
using System.Collections.ObjectModel;

namespace Lims.WebAPI.Models
{ 
    [Serializable]
    public class SampleModel : BaseModel
    {
        /// <summary>
        /// 样品编号（主键）
        /// </summary>
        [SugarColumn(ColumnDataType = "varchar", IsPrimaryKey = true, IsNullable = false)]
        public string? SampleCode { get; set; }

        /// <summary>
        /// 样品名称
        /// </summary>
        [SugarColumn(ColumnDataType = "varchar", IsNullable = false, DefaultValue = "未命名")]
        public string? SampleName { get; set; }

        /// <summary>
        /// 样品状态
        /// </summary>
        [SugarColumn(ColumnDataType = "varchar", IsNullable = false)]
        public string? SampleState { get; set; }

        /// <summary>
        /// 是否加急
        /// </summary>
        [SugarColumn(ColumnDataType = "bool", IsNullable = false, DefaultValue = "0")]
        public bool IsUrgent { get; set; }

        /// <summary>
        /// 当前是否加急
        /// </summary>
        [SugarColumn(ColumnDataType = "bool", IsNullable = false, DefaultValue ="false")]
        public bool CurrentUrgent { get; set; }

        /// <summary>
        /// 任务类型
        /// </summary>
        [SugarColumn(ColumnDataType = "varchar", IsNullable = false)]
        public string? TaskType { get; set; }

        /// <summary>
        /// 客户
        /// </summary>
        [SugarColumn(ColumnDataType = "varchar", IsNullable = true, DefaultValue = "/")]
        public string? EnterpriseOfSender { get; set; }

        /// <summary>
        /// 样品备注
        /// </summary>
        [SugarColumn(ColumnDataType = "varchar", IsNullable = true)]
        public string? SampleRemark { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(ColumnDataType = "timestamptz", IsNullable = false)]
        public DateTimeOffset CreateTime { get; set; }

        /// <summary>
        /// 一审时间
        /// </summary>
        [SugarColumn(ColumnDataType = "timestamptz", IsNullable = true)]
        public DateTimeOffset? FirstAuditTime { get; set; }

        /// <summary>
        /// 二审时间
        /// </summary>
        [SugarColumn(ColumnDataType = "timestamptz", IsNullable = true)]
        public DateTimeOffset? SecondAuditTime { get; set; }

        /// <summary>
        /// 完成时间
        /// </summary>
        [SugarColumn(ColumnDataType = "timestamptz", IsNullable = true)]
        public DateTimeOffset? CompleteTime { get; set; }

        /// <summary>
        /// 审核备注
        /// </summary>
        [SugarColumn(ColumnDataType = "varchar", IsNullable = true)]
        public string? CheckRemark { get; set; }

        /// <summary>
        /// 关联检测项目
        /// </summary>
        [Navigate(NavigateType.OneToMany, nameof(ItemModel.SampleCode))]
        public ObservableCollection<ItemModel>? Items { get; set; }
    }

    public enum TaskType
    {
        一般检验 = 0,

        年度检验 = 1,

        执法检验 = 2,

        方法验证 = 3,

        全项检测 = 4,
    }
}