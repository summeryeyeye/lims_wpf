using Lims.Common.Dtos;
using Lims.WebAPI.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Npgsql;
using SqlSugar;
using System.Data;
using System.Threading.Tasks;
using DbType = SqlSugar.DbType;

namespace Lims.WebAPI.Singleton
{
    public class TaskCount
    {
        //public static TaskCount Instance;

        //private static readonly object locker = new object();

        private readonly IHubContext<ChatHub> _context;
        private readonly ISqlSugarClient _client;
        //private static string userName;
        public TaskCount(IHubContext<ChatHub> context, ISqlSugarClient client)
        {
            this._context = context;
            this._client = client;
            //SqlDependency.Start(_connStr);//传入连接字符串,启动基于数据库的监听

        }

        private readonly static string _connStr = AppConfigurtaionServices.Configuration.GetSection("ConnectionStrings:POSTGRESQL").Value;
        /// <summary>
        /// 监听postgresql
        /// </summary>
        public void ListenPostgresql()
        {
            GetTaskCount();            
            using var conn = new NpgsqlConnection(_connStr);
            conn.Open();

            // 订阅通知
            conn.Notification += async (o, e) =>
            {
                //Console.WriteLine($"收到通知: {e.Channel}, 数据: {e.Payload}");

                // 在这里执行你的操作
                GetTaskCount(); // 获取任务计数并发送到客户端
            };

            using (var cmd = new NpgsqlCommand("LISTEN item_changed;LISTEN logger_changed", conn))
                cmd.ExecuteNonQuery();

            // 持续监听
            while (true)
            {
                conn.Wait();
            }

        }
        public void GetTaskCount()
        {           
            using (var Db = _client)
            {
                TaskCountDto taskCountDto = new TaskCountDto();
                taskCountDto.MyReceivableTasks = Db.Ado.GetDataTable("SELECT \"Tester\",Count(\"ItemId\") count FROM \"ItemModel\" WHERE \"TestProgress\"=101 Group BY \"Tester\"");
                taskCountDto.MyTestingTasks = Db.Ado.GetDataTable("SELECT \"Tester\",Count(\"ItemId\") count FROM \"ItemModel\" WHERE \"TestProgress\"=103 Group BY \"Tester\"");
                taskCountDto.MyReturnedTasks = Db.Ado.GetDataTable("SELECT \"Tester\",Count(\"ItemId\") count FROM \"ItemModel\" WHERE \"TestProgress\"=102 Group BY \"Tester\"");
                taskCountDto.MyUnreadLogs = Db.Ado.GetDataTable("SELECT \"ReceiverName\" Tester, COUNT(\"Id\") count FROM \"LoggerModel\" WHERE \"IsReaded\"=FALSE AND \"LogLevel\"=3 Group BY \"ReceiverName\"");
                taskCountDto.unFinishedTasks = Db.Ado.GetInt("SELECT Count(\"ItemId\") count FROM \"ItemModel\" WHERE \"TestProgress\"<104");
                taskCountDto.firstCheckTasks = Db.Ado.GetInt("SELECT COUNT(\"SampleCode\") count FROM (SELECT \"SampleCode\" FROM \"ItemModel\" GROUP BY \"SampleCode\" HAVING SUM(CASE WHEN \"TestProgress\" <> 104 THEN 1 ELSE 0 END) = 0 ) t");
                taskCountDto.sencondCheckTasks = Db.Ado.GetInt("SELECT COUNT(\"SampleCode\") count  FROM (SELECT \"SampleCode\" FROM \"ItemModel\" GROUP BY \"SampleCode\" HAVING SUM(CASE WHEN \"TestProgress\" <> 105 THEN 1 ELSE 0 END) = 0 ) t");
                taskCountDto.thirdCheckTasks = Db.Ado.GetInt("SELECT COUNT(\"SampleCode\") count  FROM (SELECT \"SampleCode\" FROM \"ItemModel\" GROUP BY \"SampleCode\" HAVING SUM(CASE WHEN \"TestProgress\" <> 106 THEN 1 ELSE 0 END) = 0 ) t");


                string json = Newtonsoft.Json.JsonConvert.SerializeObject(taskCountDto);
                _context.Clients.All.SendAsync("TaskCount", json);

                ListenPostgresql();
                //Db.Dispose();

            }
        }
       
    }
}
