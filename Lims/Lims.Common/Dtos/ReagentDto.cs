namespace Lims.Common.Dtos
{
    public class ReagentDto : BaseDto
    {
        public int Id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 分子式
        /// </summary>
        public string? MolecularFormula { get; set; }
        /// <summary>
        /// CAS号
        /// </summary>
        public string? CASNumber { get; set; }
        /// <summary>
        /// 别名
        /// </summary>
        public string? Alias { get; set; }
        /// <summary>
        /// 位置
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 规格
        /// </summary>
        public string? Speciffication { get; set; }
        /// <summary>
        /// 纯度
        /// </summary>
        public string? Purity { get; set; }
        /// <summary>
        /// 库存数量
        /// </summary>
        private int count;

        public int Count
        {
            get { return count; }
            set
            {
                
                count = value; RaisePropertiesChanged(nameof(Count));
            }
        }

    }
}
