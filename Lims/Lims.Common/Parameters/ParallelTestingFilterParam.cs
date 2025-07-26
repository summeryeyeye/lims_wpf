namespace Lims.Common.Parameters
{
    [Serializable]
    public class ParallelTestingFilterParam
    {

        public required string ParentId {
            get; set;
        }

        //public ParallelTestingFilterParam(string? parentId)
        //{
        //    this.ParentId = parentId;
        //}
    }

}