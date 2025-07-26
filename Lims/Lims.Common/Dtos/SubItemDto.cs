using System.Collections.ObjectModel;

namespace Lims.Common.Dtos
{
    public class SubItemDto : BaseDto
    {

        public string? SubItemId
        {
            get; set;
        }


        public string? ItemId
        {
            get;
            set;
        }

        private string? testItem;

        public string? TestItem
        {
            get
            {
                return testItem;
            }
            set
            {
                testItem = value;
                RaisePropertiyChanged(nameof(TestItem));
            }
        }
        
        /// <summary>
        /// 平行一结果
        /// </summary>
        private string? firstTtestResult;

        public string? FirstTestResult
        {
            get
            {
                return firstTtestResult;
            }
            set
            {
                firstTtestResult = value;
                RaisePropertiyChanged(nameof(FirstTestResult));
            }
        }
        private string? secondTestResult;

        public string? SecondTestResult
        {
            get
            {
                return secondTestResult;
            }
            set
            {
                secondTestResult = value;
                RaisePropertiyChanged(nameof(SecondTestResult));
            }
        }
        
        private string? averageTestResult;

        public string? AverageTestResult
        {
            get
            {
                return averageTestResult;
            }
            set
            {
                averageTestResult = value;
                RaisePropertiyChanged(nameof(AverageTestResult));
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
                RaisePropertiyChanged(nameof(Temp_TestResult));
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
                RaisePropertiyChanged(nameof(TestResult));
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
                RaisePropertiyChanged(nameof(ItemRemark));
            }
        }

        private DateTimeOffset createTime;

        public DateTimeOffset CreateTime
        {
            get => createTime;
            set
            {
                createTime = value;
                RaisePropertiyChanged(nameof(CreateTime));
            }
        }

        private string? indexRequest;

        public string? IndexRequest
        {
            get
            {
                return indexRequest;
            }
            set
            {
                indexRequest = value;
                RaisePropertiyChanged(nameof(IndexRequest));
            }
        }        
    }
}
