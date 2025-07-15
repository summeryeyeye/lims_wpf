namespace Lims.Common.Parameters
{
    public class ItemFilterParam
    {
        public ItemFilterParam()
        {
            
        }
        public ItemFilterParam(string? sampleCode)
        {
            this.SampleCode = sampleCode;
        }
        public Operation? Operation { get; set; }
        public string? Tester { get; set; }
        public string? SampleCode { get; set; }
        public int? MethodStandardId { get; set; }
        public string? SampleCodes { get; set; }
        public int? TestProgress { get; set; }
        public string? KeyItem { get; set; }


        public DateTime? MinDate { get; set; }
        public DateTime? MaxDate { get; set; }


        //public string? TestItemKeyWord_1 { get; set; }
       // public string? TestItemKeyWord_2 { get; set; }

        //public bool WithMethod = true;
        //public bool WithPruduct = true;
        //public bool WithSample = true;
    }
    public enum Operation
    {
        Lower = 0,
        Equal = 1,
        Higher = 2,
    }
}
