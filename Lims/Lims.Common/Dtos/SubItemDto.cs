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
                RaisePropertiesChanged(nameof(TestItem));
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
                RaisePropertiesChanged(nameof(FirstTestResult));
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
                RaisePropertiesChanged(nameof(SecondTestResult));
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
                RaisePropertiesChanged(nameof(AverageTestResult));
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

        private DateTimeOffset createTime;

        public DateTimeOffset CreateTime
        {
            get => createTime;
            set
            {
                createTime = value;
                RaisePropertiesChanged(nameof(CreateTime));
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
                RaisePropertiesChanged(nameof(IndexRequest));
            }
        }
    }
}
