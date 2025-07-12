using System.Collections.ObjectModel;

namespace Lims.Common.Dtos
{
    public class ItemDto : BaseDto
    {
        public ItemDto()
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="methodStandardId">检测标准Id</param>
        /// <param name="reportUnit">报告单位</param>
        /// <param name="productStandardId">产品标准Id</param>
        /// <param name="indexRequest">指标要求</param>
        public ItemDto(int methodStandardId, string reportUnit, string testItem, int productStandardId, string indexRequest)
        {
            this.MethodStandardId = methodStandardId;
            this.ProductStandardId = productStandardId;
            this.TestItem = testItem;
            this.ReportUnit = reportUnit;
            this.IndexRequest = indexRequest;
        }

        public string? ItemId { get; set; }

        public static IList<ItemDto>? ReportSource { get; set; }

        #region 产品属性
        public int ProductStandardId { get; set; }
        public ProductStandardDto? ProductStandard { get; set; }


        #region automapper映射        
        public string? ExecuteStandard { get; set; }
        public string? ProductType { get; set; }
        public string? ProductClass { get; set; }
        public string? ProductForm { get; set; }
        #endregion

        #endregion



        #region 方法属性
        public int MethodStandardId { get; set; }
        public MethodStandardDto? MethodStandard { get; set; }

        #region automapper映射
        private string? testMethod;

        public string? TestMethod
        {
            get { return testMethod; }
            set { testMethod = value; RaisePropertiesChanged(nameof(TestMethod)); }
        }

        public string? RoundRule { get; set; }
        #endregion



        #endregion 方法属性


        private string? firstSampleWeight;

        public string? FirstSampleWeight
        {
            get { return firstSampleWeight; }
            set
            {
                firstSampleWeight = value;
                RaisePropertiesChanged(nameof(FirstSampleWeight));
            }
        }

        private string? secondSampleWeight;

        public string? SecondSampleWeight
        {
            get { return secondSampleWeight; }
            set
            {
                secondSampleWeight = value;
                RaisePropertiesChanged(nameof(SecondSampleWeight));
            }
        }




        private string testItem;
        public string TestItem
        {
            get
            {
                return testItem;
            }
            set
            {
                testItem = value; RaisePropertiesChanged(nameof(TestItem));
            }
        }
        private string? tester;

        public string? Tester
        {
            get
            {
                return tester;
            }
            set
            {
                tester = value; RaisePropertiesChanged(nameof(Tester));
            }
        }
        private string? reportUnit;
        public string? ReportUnit
        {
            get
            {
                return reportUnit;
            }
            set
            {
                reportUnit = value;
                RaisePropertiesChanged(nameof(ReportUnit));
            }
        }

        private string? indexRequest;

        public string? IndexRequest
        {
            get { return indexRequest; }
            set { indexRequest = value; RaisePropertiesChanged(nameof(IndexRequest)); }
        }

        /// <summary>
        /// 有剂型则显示剂型，无则显示样品状态
        /// </summary>
        public string? SampleFormOrState
        {
            get; set;
        }

        private string? sampleCode;

        public string? SampleCode
        {
            get => sampleCode;
            set
            {
                sampleCode = value;
            }
        }

        private DateTimeOffset appointTime;

        public DateTimeOffset AppointTime
        {
            get => appointTime;
            set
            {
                appointTime = value;
                RaisePropertiesChanged(nameof(AppointTime));
            }
        }
        private int preTestProgress;

        public int PreTestProgress
        {
            get => preTestProgress;
            set
            {
                preTestProgress = value;
                RaisePropertiesChanged(nameof(PreTestProgress));
            }
        }
        private int testProgress;

        public int TestProgress
        {
            get => testProgress;
            set
            {
                testProgress = value;
                RaisePropertiesChanged(nameof(TestProgress));
            }
        }

        private string? temp_TestResult;

        public string? Temp_TestResult
        {
            get
            {
                return temp_TestResult;
            }
            set
            {
                temp_TestResult = value;
                RaisePropertiesChanged(nameof(Temp_TestResult));
            }
        }

        private string? testResult;

        public string? TestResult
        {
            get
            {
                return testResult;
            }
            set
            {
                testResult = value;
                RaisePropertiesChanged(nameof(TestResult));
            }
        }

        private string? itemRemark;

        public string? ItemRemark
        {
            get
            {
                return itemRemark;
            }
            set
            {
                itemRemark = value;
                RaisePropertiesChanged(nameof(ItemRemark));
            }
        }

        private DateTimeOffset? resultSubmitTime;

        public DateTimeOffset? ResultSubmitTime
        {
            get => resultSubmitTime;
            set
            {
                resultSubmitTime = value;
                RaisePropertiesChanged(nameof(ResultSubmitTime));
            }
        }

        private string? temp_SingleConclusion = "/";

        public string? Temp_SingleConclusion
        {
            get => temp_SingleConclusion;
            set
            {
                temp_SingleConclusion = value;
                RaisePropertiesChanged(nameof(Temp_SingleConclusion));
            }
        }
        private string? singleConclusion = "/";

        public string? SingleConclusion
        {
            get => singleConclusion;
            set
            {
                singleConclusion = value;
                RaisePropertiesChanged(nameof(SingleConclusion));
            }
        }

        private DateTimeOffset? testDate;

        public DateTimeOffset? TestDate
        {
            get => testDate;
            set
            {
                testDate = value;
                RaisePropertiesChanged(nameof(TestDate));
            }
        }
        /// <summary>
        /// 检测备注
        /// </summary>
        private string? testRemark;

        public string? TestRemark
        {
            get { return testRemark; }
            set { testRemark = value; RaisePropertiesChanged(nameof(TestRemark)); }
        }

        /// <summary>
        /// 原始记录是否完成
        /// </summary>
        private bool isOriginalRecordComplete = false;

        public bool IsOriginalRecordComplete
        {
            get => isOriginalRecordComplete;
            set
            {
                isOriginalRecordComplete = value;
                RaisePropertiesChanged(nameof(IsOriginalRecordComplete));
            }
        }

        /// <summary>
        /// 是否超期补录
        /// </summary>
        private bool isOverDate;

        public bool IsOverDate
        {
            get { return isOverDate; }
            set { isOverDate = value; RaisePropertiesChanged(nameof(IsOverDate)); }
        }

        public SampleDto? Sample { get; set; }



        private ObservableCollection<SubItemDto>? subItems;

        public ObservableCollection<SubItemDto>? SubItems
        {
            get
            {
                return subItems;
            }
            set
            {
                subItems = value;
                RaisePropertiesChanged(nameof(SubItems));
            }
        }

        private ObservableCollection<SubItemDto>? selectedRetestSubItems = new();

        public ObservableCollection<SubItemDto>? SelectedRetestSubItems
        {
            get { return selectedRetestSubItems; }
            set { selectedRetestSubItems = value; }
        }













    }
}
