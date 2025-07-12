//using AutoMapper;
//using Lims.Common.Dtos;
//using Lims.WebAPI.Hubs;
//using Lims.WebAPI.Models;
//using Lims.WebAPI.Service;
//using Lims.WebAPI.Service.Interface;
//using Microsoft.AspNetCore.SignalR;
//using Microsoft.Data.SqlClient;
//using SqlSugar;
//using System.Data;
//using DbType = SqlSugar.DbType;

//namespace Lims.WebAPI.Controllers
//{
//    public class TaskCountsController : MyBaseController<ItemModel, ItemDto>
//    {
//        public static TaskCountsController Instance;

//        private static readonly object locker = new object();

//        private readonly IHubContext<ChatHub> _context;

//        private readonly IItemService itemService;

//        private static string userName;
//        public TaskCountsController(IMapper mapper, IItemService itemService, IHubContext<ChatHub> context) : base(mapper, (BaseService<ItemModel>)itemService)
//        {
//            this._context = context;
//            this.itemService = itemService;


//            SqlDependency.Start(_connStr);//传入连接字符串,启动基于数据库的监听



//        }


//        #region sqldependency               
//        //static string _connectionId = string.Empty;
//        public void UpdateGrid(string userName)
//        {
//            // _connectionId = Id;
//            //TaskCountDto taskCountDto = new TaskCountDto(userName);
//            using (SqlConnection connection = new SqlConnection(_connStr))
//            {
//                try
//                {
//                    connection.Open();
//                    using (SqlCommand command = new SqlCommand("SELECT [ItemId],[TestProgress] FROM [dbo].[ItemModel];SELECT [Id],[IsReaded] FROM [dbo].[LoggerModel]", connection))
//                    {
//                        command.CommandType = CommandType.Text;
//                        //connection.Open();
//                        SqlDependency dependency = new SqlDependency(command);
//                        dependency.OnChange += new OnChangeEventHandler(Dependency_OnChange);
//                        // dependency.OnChange -= new OnChangeEventHandler(Dependency_OnChange);
//                        command.ExecuteNonQuery();
//                        GetTaskCount(userName);
//                    }
//                }
//                catch (Exception)
//                {

//                    throw;
//                }
//                finally
//                {
//                    if (connection.State == ConnectionState.Open)
//                    {
//                        connection.Close();
//                    }
//                    connection.Dispose();

//                }
//            }

//        }
//        private void Dependency_OnChange(object sender, SqlNotificationEventArgs e)
//        {
//            if (e.Type == SqlNotificationType.Change) //只有数据发生变化时,才重新获取并数据
//            {
//                UpdateGrid(TaskCountsController.userName);
//            }
//        }
//        #endregion

//        private readonly static string _connStr = AppConfigurtaionServices.Configuration.GetSection("ConnectionStrings:CommonConection").Value;

//        private void GetTaskCount(string userName)
//        {
//            //创建数据库对象 (用法和EF Dappper一样通过new保证线程安全)
//            TaskCountDto taskCountDto = new TaskCountDto(userName);

//            TaskCountsController.userName = userName;// "杨升";// ChatHub.CurrentUserName;
//            using (var Db = new SqlSugarScope(new ConnectionConfig()
//            {
//                ConnectionString = _connStr,
//                DbType = DbType.SqlServer,
//                IsAutoCloseConnection = true
//            }))
//            {
//                taskCountDto.MyReceivableTasks = Db.Ado.GetInt("SELECT COUNT(*) FROM ItemModel WHERE TestProgress=101 AND TESTER=@user", new
//                {
//                    user = userName
//                });
//                taskCountDto.MyTestingTasks = Db.Ado.GetInt("SELECT COUNT(*) FROM ItemModel WHERE TestProgress=103 AND TESTER=@user", new
//                {
//                    user = userName
//                });
//                taskCountDto.MyReturnedTasks = Db.Ado.GetInt("SELECT COUNT(*) FROM ItemModel WHERE TestProgress=102 AND TESTER=@user", new
//                {
//                    user = userName
//                });
//                taskCountDto.MyUnreadLogs = Db.Ado.GetInt("SELECT COUNT(*) count FROM LoggerModel WHERE [IsReaded]=0 AND [LogLevel]=3 AND ReceiverName=@user", new
//                {
//                    user = userName
//                });

//                taskCountDto.unFinishedTasks = Db.Ado.GetInt("SELECT Count(*) count FROM [dbo].[ItemModel] WHERE [TestProgress]<104");
//                taskCountDto.firstCheckTasks = Db.Ado.GetInt("SELECT COUNT(*) count FROM (SELECT SampleCode FROM [dbo].[ItemModel] GROUP BY SampleCode HAVING SUM(CASE WHEN TestProgress <> 104 THEN 1 ELSE 0 END) = 0 ) t");
//                taskCountDto.sencondCheckTasks = Db.Ado.GetInt("SELECT COUNT(*) count  FROM (SELECT SampleCode FROM [dbo].[ItemModel] GROUP BY SampleCode HAVING SUM(CASE WHEN TestProgress <> 105 THEN 1 ELSE 0 END) = 0 ) t");
//                taskCountDto.thirdCheckTasks = Db.Ado.GetInt("SELECT COUNT(*) count  FROM (SELECT SampleCode FROM [dbo].[ItemModel] GROUP BY SampleCode HAVING SUM(CASE WHEN TestProgress <> 106 THEN 1 ELSE 0 END) = 0 ) t");


//                string json = Newtonsoft.Json.JsonConvert.SerializeObject(taskCountDto);
//                _context.Clients.All.SendAsync("TaskCount", json);

//                Db.Dispose();

//            }

//        }




//        /*
//        public static void Register(IHubContext<ChatHub> context)
//        {
//            if (Instance == null)
//            {
//                lock (locker)
//                {
//                    if (Instance == null)
//                    {
//                        Instance = new TaskCountsController(context);
//                    }
//                }
//            }
//        }
//        */

//    }
//}
