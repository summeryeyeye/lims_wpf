namespace Lims.Common.Dtos
{
    public class UserDto : BaseDto
    {
        public static UserDto? Inatance
        {
            get;
            set;
        }
        private string? userId;

        public string? UserId
        {
            get
            {
                return userId;
            }
            set
            {
                userId = value;
            }
        }

        private string? passwd;

        public string? Passwd
        {
            get
            {
                return passwd;
            }
            set
            {
                passwd = value;
            }
        }

        private string? userName;

        public string? UserName
        {
            get
            {
                return userName;
            }
            set
            {
                userName = value;
            }
        }
        private string? superiorName;

        public string? SuperiorName
        {
            get
            {
                return superiorName;
            }
            set
            {
                superiorName = value;
            }
        }

        private string? userGroup;

        public string? UserGroup
        {
            get
            {
                return userGroup;
            }
            set
            {
                userGroup = value;
            }
        }
        private bool canRead;

        public bool CanRead
        {
            get
            {
                return canRead;
            }
            set
            {
                canRead = value; RaisePropertiesChanged(nameof(CanRead));
            }
        }

        private bool canTest;

        public bool CanTest
        {
            get
            {
                return canTest;
            }
            set
            {
                canTest = value;
            }
        }

        private bool canCheck;
        public bool CanCheck
        {
            get
            {
                return canCheck;
            }
            set
            {
                canCheck = value; RaisePropertiesChanged(nameof(CanCheck));
            }
        }

        private bool canFirstCheck;

        public bool CanFirstCheck
        {
            get
            {
                return canFirstCheck;
            }
            set
            {
                canFirstCheck = value;
            }
        }

        private bool canSecondCheck;

        public bool CanSecondCheck
        {
            get
            {
                return canSecondCheck;
            }
            set
            {
                canSecondCheck = value;
            }
        }

        private bool canThirdCheck;

        public bool CanThirdCheck
        {
            get
            {
                return canThirdCheck;
            }
            set
            {
                canThirdCheck = value;
            }
        }

        private bool canManage;

        public bool CanManage
        {
            get
            {
                return canManage;
            }
            set
            {
                canManage = value;
            }
        }

        private bool isAdmin;

        public bool IsAdmin
        {
            get
            {
                return isAdmin;
            }
            set
            {
                isAdmin = value;
            }
        }
    }
}
