namespace Lims.Common.Parameters
{

    public class SampleFilterParam
    {
        public string? SampleCodes { get; set; }
        public string? SampleCodeKeyWord { get; set; }
        public DateTime? MinDate
        {
            get; set;
        }
        public DateTime? MaxDate
        {
            get; set;
        }
        public bool WithItems { get; set; } = true;
        //public int RelativeProgress
        //{
        //    get; set;
        //}
        public string? TaskType
        {
            get; set;
        }
        //public Operation Operation
        //{
        //    get; set;
        //}
        //public bool IsShowCompeleteDatas { get; set; }
    }
}
