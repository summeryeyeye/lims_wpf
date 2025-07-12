namespace Lims.Common.Dtos
{
    public class MethodStandardDto : StandardBaseDto
    {
        public int Id { get; set; }

        public string TestItem { get; set; }

        public string TestMethod { get; set; }

        public string Tester { get; set; }

        public string SampleState { get; set; }

        public string? TestUnit { get; set; }

        public string? RoundRule { get; set; }

        private string? keyItem;

        public string? KeyItem
        {
            get { return keyItem; }
            set { keyItem = value; RaisePropertiesChanged(nameof(KeyItem)); }
        }

        


        #region 原始记录
        private string? originalRecordTemplateFilePath;

        public string? OriginalRecordTemplateFilePath
        {
            get
            {
                return originalRecordTemplateFilePath;
            }
            set
            {
                originalRecordTemplateFilePath = value;
                RaisePropertiesChanged(nameof(OriginalRecordTemplateFilePath));
            }
        }
        private string? instruments;

        public string? Instruments
        {
            get { return instruments; }
            set { instruments = value; RaisePropertiesChanged(nameof(Instruments)); }
        }
        private string? originalRecordGroup;

        public string? OriginalRecordGroup
        {
            get { return originalRecordGroup; }
            set { originalRecordGroup = value; RaisePropertiesChanged(nameof(OriginalRecordGroup)); }
        }
        private bool isPrintSubItem;

        public bool IsPrintSubItem
        {
            get { return isPrintSubItem; }
            set { isPrintSubItem = value; }
        }
        public bool IsSelected { get; set; }



        /*

        private string? sampleCodeLocation;
        public string? SampleCodeLocation
        {
            get
            {
                return sampleCodeLocation;
            }
            set
            {
                sampleCodeLocation = value;
                RaisePropertiesChanged(nameof(SampleCodeLocation));
            }
        }

        private string? testMethodLocation;

        public string? TestMethodLocation
        {
            get
            {
                return testMethodLocation;
            }
            set
            {
                testMethodLocation = value;
                RaisePropertiesChanged(nameof(TestMethodLocation));
            }
        }

        private string? testDateLocation;

        public string? TestDateLocation
        {
            get
            {
                return testDateLocation;
            }
            set
            {
                testDateLocation = value;
                RaisePropertiesChanged(nameof(TestDateLocation));
            }
        }

        private string? testItemLocation;

        public string? TestItemLocation
        {
            get
            {
                return testItemLocation;
            }
            set
            {
                testItemLocation = value;
                RaisePropertiesChanged(nameof(TestItemLocation));
            }
        }

        private string? testResultLocation;

        public string? TestResultLocation
        {
            get
            {
                return testResultLocation;
            }
            set
            {
                testResultLocation = value;
                RaisePropertiesChanged(nameof(TestResultLocation));
            }
        }       

        private string? firstsubItemLocation;

        public string? FirstSubItemLocation
        {
            get { return firstsubItemLocation; }
            set { firstsubItemLocation = value; RaisePropertiesChanged(nameof(FirstSubItemLocation)); }
        }

        private string? lastSubItemLocation;

        public string? LastSubItemLocation
        {
            get { return lastSubItemLocation; }
            set { lastSubItemLocation = value; RaisePropertiesChanged(nameof(LastSubItemLocation)); }
        }
        /// <summary>
        /// 子项目备注信息
        /// </summary>
        private string? infoLocation;
        public string? InfoLocation
        {
            get { return infoLocation; }
            set { infoLocation = value; RaisePropertiesChanged(nameof(InfoLocation)); }
        }

        private string? instrumentsLocation;

        public string? InstrumentsLocation
        {
            get { return instrumentsLocation; }
            set { instrumentsLocation = value; RaisePropertiesChanged(nameof(InstrumentsLocation)); }
        }
       */
        
        #endregion 原始记录
    }
}
