namespace Lims.WPF.Navigations
{
    public static class MyTasks
    {
        //public static string Main { get { return "Main"; } }
        public static string MyReceivableTasks
        {
            get
            {
                return "待领取任务";
            }
        }

        public static string MyTestingTasks
        {
            get
            {
                return "检测中任务";
            }
        }

        public static string MySubmittedTasks
        {
            get
            {
                return "已提交任务";
            }
        }

        public static string MyReturnedTasks
        {
            get
            {
                return "已退回任务";
            }
        }
    }
}