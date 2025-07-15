using SqlSugar;

namespace Lims.WebAPI.Models
{   
    [Serializable]
    public class UserModel : BaseModel
    {
        [SugarColumn(IsIgnore = true)]
        public static UserModel? Inatance
        {
            get;
            set;
        }

        [SugarColumn(ColumnDataType = "varchar", IsPrimaryKey = true)]
        public string? UserId
        {
            get; set;
        }

        [SugarColumn(ColumnDataType = "varchar", IsNullable = false)]
        public string? Passwd
        {
            get; set;
        }

        [SugarColumn(ColumnDataType = "varchar", IsNullable = false)]
        public string? UserName
        {
            get; set;
        }

        [SugarColumn( ColumnDataType = "varchar", IsNullable = false)]
        public string? SuperiorName
        {
            get; set;
        }

        [SugarColumn( ColumnDataType = "varchar", IsNullable = false)]
        public string? UserGroup
        {
            get; set;
        }

        [SugarColumn(ColumnDataType = "bool", IsNullable = false, DefaultValue = "false")]
        public bool CanRead
        {
            get;
            set;
        }

        [SugarColumn(ColumnDataType = "bool", IsNullable = false, DefaultValue = "false")]
        public bool CanTest
        {
            get; set;
        }

        [SugarColumn(ColumnDataType = "bool", IsNullable = false, DefaultValue = "false")]
        public bool CanCheck
        {
            get;
            set;
        }

        [SugarColumn(ColumnDataType = "bool", IsNullable = false, DefaultValue = "false")]
        public bool CanFirstCheck
        {
            get; set;
        }

        [SugarColumn(ColumnDataType = "bool", IsNullable = false, DefaultValue = "false")]
        public bool CanSecondCheck
        {
            get; set;
        }

        [SugarColumn(ColumnDataType = "bool", IsNullable = false, DefaultValue = "false")]
        public bool CanThirdCheck
        {
            get; set;
        }

        [SugarColumn(ColumnDataType = "bool", IsNullable = false, DefaultValue = "false")]
        public bool CanManage
        {
            get; set;
        }

        [SugarColumn(ColumnDataType = "bool", IsNullable = false, DefaultValue = "false")]
        public bool IsAdmin
        {
            get; set;
        }
    }
}