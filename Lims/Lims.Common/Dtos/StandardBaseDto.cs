namespace Lims.Common.Dtos
{
    public class StandardBaseDto : BaseDto
    {
        /// <summary>
        /// 标准状态
        /// </summary> 
        private StandardState standardState;

        public StandardState StandardState
        {
            get { return standardState; }
            set { standardState = value; RaisePropertiesChanged(nameof(StandardState)); }
        }
        public string LastUpdater { get; set; }
    }
    public enum StandardState
    {
        /// <summary>
        /// 过期，废止
        /// </summary>
        Expire,
        /// <summary>
        /// 部分有效
        /// </summary>
        PartialValidity,
        /// <summary>
        /// 有效
        /// </summary>
        Validity,

    }




}
