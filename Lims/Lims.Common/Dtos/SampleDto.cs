using System.Collections.ObjectModel;

namespace Lims.Common.Dtos
{
    public class SampleDto : BaseDto
    {
        private string? sampleCode;

        public string? SampleCode
        {
            get => sampleCode;
            set
            {
                sampleCode = value;
                RaisePropertiyChanged(nameof(SampleCode));
            }
        }
        private string? sampleName;

        public string? SampleName
        {
            get => sampleName;
            set
            {
                sampleName = value;
                RaisePropertiyChanged(nameof(SampleName));
            }
        }

        private string? sampleState;

        public string? SampleState
        {
            get => sampleState;
            set
            {
                sampleState = value;
                RaisePropertiyChanged(nameof(SampleState));
            }
        }

        private bool isUrgent;

        public bool IsUrgent
        {
            get => isUrgent;
            set
            {
                isUrgent = value;
                RaisePropertiyChanged(nameof(IsUrgent));
            }
        }

        private string? taskType;

        public string? TaskType
        {
            get => taskType;
            set
            {
                taskType = value;
                RaisePropertiyChanged(nameof(TaskType));
            }
        }

        /// <summary>
        /// 客户
        /// </summary>
        private string? enterpriseOfSender;

        public string? EnterpriseOfSender
        {
            get => enterpriseOfSender;
            set
            {
                enterpriseOfSender = value;
                RaisePropertiyChanged(nameof(EnterpriseOfSender));
            }
        }

        private string? sampleRemark;

        public string? SampleRemark
        {
            get => sampleRemark;
            set
            {
                sampleRemark = value;
                RaisePropertiyChanged(nameof(SampleRemark));
            }
        }

        private DateTimeOffset createTime = DateTimeOffset.Now;

        public DateTimeOffset CreateTime
        {
            get => createTime;
            set
            {
                createTime = value;
                RaisePropertiyChanged(nameof(CreateTime));
            }
        }

        private DateTimeOffset? firstAuditTime;

        public DateTimeOffset? FirstAuditTime
        {
            get => firstAuditTime;
            set
            {
                firstAuditTime = value;
                RaisePropertiyChanged(nameof(FirstAuditTime));
            }
        }

        private DateTimeOffset? secondAuditTime;

        public DateTimeOffset? SecondAuditTime
        {
            get => secondAuditTime;
            set
            {
                secondAuditTime = value;
                RaisePropertiyChanged(nameof(SecondAuditTime));
            }
        }

        private DateTimeOffset? completeTime;

        public DateTimeOffset? CompleteTime
        {
            get => completeTime;
            set
            {
                completeTime = value;
                RaisePropertiyChanged(nameof(CompleteTime));
            }
        }

        private string? checkRemark;

        public string? CheckRemark
        {
            get => checkRemark;
            set
            {
                checkRemark = value;
                RaisePropertiyChanged(nameof(CheckRemark));
            }
        }


        //private int minTestProgress;

        //public int MinTestProgress
        //{
        //    get => minTestProgress;
        //    set
        //    {
        //        minTestProgress = value;
        //        RaisePropertiesChanged(nameof(MinTestProgress));
        //    }
        //}

        //private int maxTestProgress;

        //public int MaxTestProgress
        //{
        //    get => maxTestProgress;
        //    set
        //    {
        //        maxTestProgress = value;
        //        RaisePropertiesChanged(nameof(MaxTestProgress));
        //    }
        //}

        /// <summary>
        /// 水分
        /// </summary>
        private string? moistureContent;

        public string? MoistureContent
        {
            get { return moistureContent; }
            set { moistureContent = value; RaisePropertiyChanged(nameof(MoistureContent)); }
        }
        /// <summary>
        /// 密度
        /// </summary>
        private string? density;

        public string? Density
        {
            get { return density; }
            set { density = value; RaisePropertiyChanged(nameof(Density)); }
        }





        private List<ItemDto?> items;

        public List<ItemDto?> Items
        {
            get => items;
            set
            {
                items = value;
                RaisePropertiyChanged(nameof(Items));
            }
        }
        
        public bool CurrentUrgent { get; set; }
              

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
