using SqlSugar;

namespace Lims.WebAPI.Models
{

    [SugarTable("UserModel")]
    [Serializable]
    public class UserModel : BaseModel
    {
        [SugarColumn(IsIgnore = true)]
        public static UserModel? Inatance
        {
            get;
            set;
        }

        [SugarColumn(ColumnName = "UserId", ColumnDataType = "varchar", IsPrimaryKey = true)]
        public string? UserId
        {
            get; set;
        }

        [SugarColumn(ColumnName = "Password", ColumnDataType = "varchar", IsNullable = false)]
        public string? Password
        {
            get; set;
        }

        [SugarColumn(ColumnName = "UserName", ColumnDataType = "varchar", IsNullable = false)]
        public string? UserName
        {
            get; set;
        }

        [SugarColumn(ColumnName= "SuperiorName", ColumnDataType = "varchar", IsNullable = false)]
        public string? SuperiorName
        {
            get; set;
        }

        [SugarColumn(ColumnName = "Group", ColumnDataType = "varchar", IsNullable = false)]
        public string? Group
        {
            get; set;
        }

        [SugarColumn(ColumnName = "CanRead", ColumnDataType = "bool", IsNullable = false, DefaultValue = "0")]
        public bool CanRead
        {
            get;
            set;
        }

        [SugarColumn(ColumnName = "CanTest", ColumnDataType = "bool", IsNullable = false, DefaultValue = "0")]
        public bool CanTest
        {
            get; set;
        }

        [SugarColumn(ColumnName = "CanCheck", ColumnDataType = "bool", IsNullable = false, DefaultValue = "0")]
        public bool CanCheck
        {
            get;
            set;
        }

        [SugarColumn(ColumnName = "CanFirstCheck", ColumnDataType = "bool", IsNullable = false, DefaultValue = "0")]
        public bool CanFirstCheck
        {
            get; set;
        }

        [SugarColumn(ColumnName = "CanSecondCheck", ColumnDataType = "bool", IsNullable = false, DefaultValue = "0")]
        public bool CanSecondCheck
        {
            get; set;
        }

        [SugarColumn(ColumnName = "CanThirdCheck", ColumnDataType = "bool", IsNullable = false, DefaultValue = "0")]
        public bool CanThirdCheck
        {
            get; set;
        }

        [SugarColumn(ColumnName = "CanManage", ColumnDataType = "bool", IsNullable = false, DefaultValue = "0")]
        public bool CanManage
        {
            get; set;
        }

        [SugarColumn(ColumnName = "IsAdmin", ColumnDataType = "bool", IsNullable = false, DefaultValue = "0")]
        public bool IsAdmin
        {
            get; set;
        }
    }
}