using System.Data;

namespace Lims.Common.Dtos
{
    public class TaskCountDto
    {
        public DataTable MyReceivableTasks
        {
            get; set;
        }=new DataTable();
        public DataTable MyTestingTasks
        {
            get; set;
        } = new DataTable();
        public DataTable MyReturnedTasks
        {
            get; set;
        } = new DataTable();
        public DataTable MyUnreadLogs
        {
            get; set;
        } = new DataTable();

        public int unFinishedTasks
        {
            get; set;
        }
        public int firstCheckTasks
        {
            get; set;
        }
        public int sencondCheckTasks
        {
            get; set;
        }
        public int thirdCheckTasks
        {
            get; set;
        }
        public string userName { get; set; }
        public TaskCountDto(string userName)
        {
            this.userName = userName;
        }
        public TaskCountDto()
        {
            MyReceivableTasks.Columns.AddRange([new DataColumn("Tester"), new DataColumn("count")]);
            MyTestingTasks.Columns.AddRange([new DataColumn("Tester"), new DataColumn("count")]);
            MyReturnedTasks.Columns.AddRange([new DataColumn("Tester"), new DataColumn("count")]);
            MyUnreadLogs.Columns.AddRange([new DataColumn("Tester"), new DataColumn("count")]);
        }
    }
}
