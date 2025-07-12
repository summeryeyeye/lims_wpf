using Lims.Common.Dtos;
using SqlSugar;

namespace Lims.WebAPI.Models
{
    public class StandardBaseModel : BaseModel
    {
        [SugarColumn(ColumnName = "StandardState", ColumnDataType = "int4", IsNullable = false, DefaultValue = "2")]
        /// <summary>
        /// 实施日期
        /// </summary>
        public StandardState StandardState { get; set; }



    }
}
