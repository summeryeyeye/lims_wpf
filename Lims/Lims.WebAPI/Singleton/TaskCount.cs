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

        private readonly IHubContext<ChatHub> _context;
        public TaskCount(IHubContext<ChatHub> context)
        {
            this._context = context;
        }

        private readonly static string _connStr = AppConfigurtaionServices.Configuration.GetSection("ConnectionStrings:POSTGRESQL").Value;
        /// <summary>
        /// 监听postgresql
        /// </summary>
        public void ListenPostgresql()
        {
            GetTaskCount();
            using var conn = new NpgsqlConnection(_connStr);

            if (conn.State != ConnectionState.Open)
                conn.Open();
            // 订阅通知
            conn.Notification += (o, e) =>
            {
                //Console.WriteLine($"收到通知: {e.Channel}, 数据: {e.Payload}");

                // 在这里执行你的操作
                GetTaskCount();// 获取任务计数并发送到客户端
            };

            using var cmd = new NpgsqlCommand("LISTEN item_changed;LISTEN sample_changed;LISTEN logger_changed;", conn);

            cmd.ExecuteNonQuery();
            //cmd.Dispose();

            // 持续监听
            while (true)
            {
                conn.Wait();
            }
        }
        public void StopListening()
        {
            using (NpgsqlConnection con = new NpgsqlConnection(_connStr))
            {
                if (con.State != ConnectionState.Open)
                    con.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand("UNLISTEN item_changed;UNLISTEN sample_changed;UNLISTEN logger_changed;", con))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }





        public async void GetTaskCount()
        {
            using (NpgsqlConnection con = new NpgsqlConnection(_connStr))
            {
                try
                {
                    TaskCountDto taskCountDto = new TaskCountDto();
                    var cmdStr = "SELECT tester,Count(itemid) count FROM itemmodel WHERE testprogress=101 Group BY tester;SELECT tester,Count(itemid) count FROM itemmodel WHERE testprogress=103 Group BY tester;SELECT tester,Count(itemid) count FROM itemmodel WHERE testprogress=102 Group BY tester;SELECT receivername tester, COUNT(id) count FROM loggermodel WHERE isreaded=FALSE AND loglevel=3 Group BY receivername;";
                    if (con.State != ConnectionState.Open)
                        con.Open();
                    using (NpgsqlCommand cmd = new NpgsqlCommand(cmdStr, con))
                    {
                        using (Npgsql.NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            taskCountDto.MyReceivableTasks = ConvertDataReaderToDataTable(reader);
                            if (reader.NextResult())
                                taskCountDto.MyTestingTasks = ConvertDataReaderToDataTable(reader);
                            if (reader.NextResult())
                                taskCountDto.MyReturnedTasks = ConvertDataReaderToDataTable(reader);
                            if (reader.NextResult())
                                taskCountDto.MyUnreadLogs = ConvertDataReaderToDataTable(reader);
                        }
                    }
                    using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT Count(itemid) count FROM itemmodel WHERE testprogress<104;SELECT COUNT(samplecode) count FROM (SELECT samplecode FROM itemmodel GROUP BY samplecode HAVING SUM(CASE WHEN testprogress <> 104 THEN 1 ELSE 0 END) = 0 ) t;SELECT COUNT(samplecode) count  FROM (SELECT samplecode FROM itemmodel GROUP BY samplecode HAVING SUM(CASE WHEN testprogress <> 105 THEN 1 ELSE 0 END) = 0 ) t;SELECT COUNT(samplecode) count  FROM (SELECT samplecode FROM itemmodel GROUP BY samplecode HAVING SUM(CASE WHEN testprogress <> 106 THEN 1 ELSE 0 END) = 0 ) t", con))
                    {
                        var reader = cmd.ExecuteReader();
                        while (reader.Read())
                            taskCountDto.unFinishedTasks = reader.GetInt32(0);
                        if (reader.NextResult())
                            while (reader.Read())
                                taskCountDto.firstCheckTasks = reader.GetInt32(0);
                        if (reader.NextResult())
                            while (reader.Read())
                                taskCountDto.sencondCheckTasks = reader.GetInt32(0);
                        if (reader.NextResult())
                            while (reader.Read())
                                taskCountDto.thirdCheckTasks = reader.GetInt32(0);
                    }
                    string json = Newtonsoft.Json.JsonConvert.SerializeObject(taskCountDto);
                    await _context.Clients.All.SendAsync("TaskCount", json);
                }
                catch (Exception)
                {

                    //throw;
                }
                finally
                {
                    con.Close();
                }
            }

        }

        //将DataReader 转化为 DataTable
        static DataTable ConvertDataReaderToDataTable(NpgsqlDataReader dataReader)
        {
            DataTable datatable = new DataTable();
            try
            {    //添加表的列类型和列名
                for (int i = 0; i < dataReader.FieldCount; i++)
                {
                    DataColumn column = new DataColumn();
                    column.DataType = dataReader.GetFieldType(i);
                    column.ColumnName = dataReader.GetName(i);
                    datatable.Columns.Add(column);
                }

                //添加表的数据
                while (dataReader.Read())
                {
                    DataRow row = datatable.NewRow();
                    for (int i = 0; i < dataReader.FieldCount; i++)
                    {
                        row[i] = dataReader[i].ToString();
                    }
                    datatable.Rows.Add(row);
                    row = null;
                }
                //dataReader.Close();
                return datatable;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }
    }
}
