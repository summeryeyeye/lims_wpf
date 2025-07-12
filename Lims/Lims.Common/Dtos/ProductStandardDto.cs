namespace Lims.Common.Dtos
{
    public class ProductStandardDto : StandardBaseDto
    {
        public int Id { get; set; }
        public string TestItem { get; set; }

        public string ExecuteStandard { get; set; }

        public string ProductType { get; set; }
        public string ProductClass { get; set; }
        public string ProductForm { get; set; }
        public string IndexRequest { get; set; }        
        public string? ProductUnit { get; set; }


        private int testMethodId;
        public int TestMethodId
        {
            get { return testMethodId; }
            set { testMethodId = value; RaisePropertiesChanged(nameof(TestMethodId)); }
        }
        public string SampleState
        {
            get
            {
                return ProductForm.Contains("液") ? "液体" : "固体";
            }
        }

    }
}
